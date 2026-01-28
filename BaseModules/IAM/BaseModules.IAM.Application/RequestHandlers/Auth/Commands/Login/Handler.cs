using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.Login;

/// <summary>
/// Kullanıcı Giriş İşlemi
/// Bu endpoint, kullanıcıların sisteme giriş yapmasını sağlar.
/// Native ve Google olmak üzere iki farklı giriş yöntemini destekler.
/// Başarılı girişte JWT token ve refresh token döner.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly ArfBlocksCommunicator _communicator;
	private readonly IJwtService _jwtService;
	private readonly IHttpContextAccessor _httpContextAccessor;

	// LivestockTrading sabitleri
	private static readonly Guid LivestockTradingModuleId = Guid.Parse("DFD018C9-FC32-42C4-AEFD-70A5942A295E");
	private static readonly Guid AdminRoleId = Guid.Parse("a1000000-0000-0000-0000-000000000001");
	private static readonly Guid BuyerRoleId = Guid.Parse("a1000000-0000-0000-0000-000000000006");

	// Admin email adresleri - bu kullanıcılar giriş yaptığında otomatik Admin rolü alır
	private static readonly string[] AdminEmails = new[]
	{
		"nagehanyazici13@gmail.com",
		"m.mustafaocak@gmail.com"
	};

	public Handler(ArfBlocksDependencyProvider dependencyProvider, DataAccess dataAccess)
	{
		_dataAccessLayer = dataAccess;
		_communicator = dependencyProvider.GetInstance<ArfBlocksCommunicator>();
		_jwtService = dependencyProvider.GetInstance<IJwtService>();
		_httpContextAccessor = dependencyProvider.GetInstance<IHttpContextAccessor>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();
		Common.Definitions.Domain.Entities.User user = null;

		switch (request.Provider?.ToLower())
		{
			case "native":
				user = await _dataAccessLayer.GetUser(request.UserName);
				break;

			case "google":
				var googleUserId = request.ExternalProviderUserId;
				user = await _dataAccessLayer.GetUserByExternalId("google", googleUserId);

				if (user == null)
				{
					user = new Common.Definitions.Domain.Entities.User
					{
						Id = Guid.NewGuid(),
						UserName = request.UserName,
						Email = request.UserName,
						FirstName = request.FirstName ?? "Google",
						Surname = request.Surname ?? "User",
						AuthProvider = "google",
						ProviderKey = googleUserId,
						UserSource = UserSources.Google,
						IsActive = true,
						PhoneNumber = request.PhoneNumber,
						BirthDate = request.BirthDate ?? DateTime.UtcNow.AddYears(-18),
						City = "",
						District = "",
						IsAvailable = true,
						Description = "",
					};

					// Email adresine göre rol belirle (Admin veya Buyer)
					var isGoogleAdmin = AdminEmails.Contains(user.Email, StringComparer.OrdinalIgnoreCase);
					var googleRoleId = isGoogleAdmin ? AdminRoleId : BuyerRoleId;

					_dataAccessLayer.AddUserRoleWithModule(new UserRole
					{
						Id = Guid.NewGuid(),
						UserId = user.Id,
						RoleId = googleRoleId,
						ModuleId = LivestockTradingModuleId,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow,
						IsDeleted = false
					});

					_dataAccessLayer.AddUser(user);
					await _dataAccessLayer.SaveChanges();
				}
				else
				{
					// Mevcut Google kullanıcısının bilgilerini güncelle
					user.UserName = request.UserName;
					user.Surname = request.Surname;
					user.FirstName = request.FirstName;
					_dataAccessLayer.UpdateUser(user);
					await _dataAccessLayer.SaveChanges();
				}
				break;

			case "apple":
			case "itunes":
				var appleUserId = request.ExternalProviderUserId;
				user = await _dataAccessLayer.GetUserByExternalId("apple", appleUserId);

				if (user == null)
				{
					user = new Common.Definitions.Domain.Entities.User
					{
						Id = Guid.NewGuid(),
						UserName = request.UserName,
						Email = request.UserName,
						FirstName = request.FirstName ?? "Apple",
						Surname = request.Surname ?? "User",
						AuthProvider = "apple",
						ProviderKey = appleUserId,
						UserSource = UserSources.Apple,
						IsActive = true,
						PhoneNumber = request.PhoneNumber,
						BirthDate = request.BirthDate ?? DateTime.UtcNow.AddYears(-18),
						City = "",
						District = "",
						IsAvailable = true,
						Description = "",
					};

					// Email adresine göre rol belirle (Admin veya Buyer)
					var isAppleAdmin = AdminEmails.Contains(user.Email, StringComparer.OrdinalIgnoreCase);
					var appleRoleId = isAppleAdmin ? AdminRoleId : BuyerRoleId;

					_dataAccessLayer.AddUserRoleWithModule(new UserRole
					{
						Id = Guid.NewGuid(),
						UserId = user.Id,
						RoleId = appleRoleId,
						ModuleId = LivestockTradingModuleId,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow,
						IsDeleted = false
					});

					_dataAccessLayer.AddUser(user);
					await _dataAccessLayer.SaveChanges();
				}
				else
				{
					// Mevcut Apple kullanıcısının bilgilerini güncelle
					user.UserName = request.UserName;
					user.Surname = request.Surname;
					user.FirstName = request.FirstName;
					_dataAccessLayer.UpdateUser(user);
					await _dataAccessLayer.SaveChanges();
				}
				break;

			default:
				throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.ProviderInvalid));
		}

		if (request.Provider.ToLower() == "native")
		{
			if (user == null || !SecurityHelper.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
				throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.InvalidCredentials));

			// Native login için telefon numarası güncelle
			if (!string.IsNullOrEmpty(request.PhoneNumber) && user.PhoneNumber != request.PhoneNumber)
			{
				user.PhoneNumber = request.PhoneNumber;
				_dataAccessLayer.UpdateUser(user);
				await _dataAccessLayer.SaveChanges();
			}
		}

		// Kullanıcının rollerini ModuleId ile birlikte çek
		var userRoles = await _dataAccessLayer.GetUserRolesWithModule(user.Id);

		// Rolleri "ModuleName.RoleName" formatında hazırla
		var roleStrings = userRoles
			.Select(ur => $"{ur.ModuleName}.{ur.RoleName}")
			.ToList();

		var expiresAt = DateTime.UtcNow.AddDays(_jwtService.GetExpirationDayCount());
		var refreshToken = _jwtService.GenerateSecureRefreshToken();

		var refreshTokenId = _dataAccessLayer.AddRefreshToken(
			user,
			refreshToken,
			expiresAt,
			request.Platform.ToString(),
			_httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
		);

		var token = _jwtService.GenerateJwt(
			user.Id,
			Guid.Empty,
			user.UserName,
			$"{user.FirstName} {user.Surname}",
			user.Email,
			user.BucketId,
			request.Platform,
			expiresAt,
			user.UserSource,
			refreshTokenId,
			roleStrings
		);

		var response = mapper.MapToResponse(token, expiresAt, user, refreshToken);

		await _dataAccessLayer.SaveChanges();
		return ArfBlocksResults.Success(response);
	}
}
