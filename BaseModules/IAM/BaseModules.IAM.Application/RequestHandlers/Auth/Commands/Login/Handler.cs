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

	// 🆕 Sabit tanımlar - Hirovo
	private static readonly Guid HirovoCompanyId = Guid.Parse("C9D8C846-10FC-466D-8F45-A4FA4E856ABD");
	private static readonly Guid HirovoModuleId = Guid.Parse("DFD018C9-FC32-42C4-AEFD-70A5942A295E");
	private static readonly Guid HirovoDefaultUserRoleId = Guid.Parse("B3F8A7D1-4E2C-4A3E-8B5A-D3E7B9C5E2F1");

	// AnimalMarket sabitleri (mevcut)
	private static readonly Guid AnimalMarketModuleId = Guid.Parse("70B6430F-CEB8-4854-85C7-B69B0CE74495");
	private static readonly Guid AnimalMarketCompanyId = Guid.Parse("9DAE9CBD-82B1-4EAD-BD2B-9C5FE5146A2A");

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
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
				user = await _dataAccessLayer.GetUser(request.UserName, request.CompanyId);
				break;

			case "google":
				var googleUserId = request.ExternalProviderUserId;
				user = await _dataAccessLayer.GetUserByExternalId("google", googleUserId, request.CompanyId);

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
						CompanyId = request.CompanyId,
						IsActive = true,
						PhoneNumber = request.PhoneNumber,
						BirthDate = request.BirthDate ?? DateTime.UtcNow.AddYears(-18),
						City = "",
						District = "",
						IsAvailable = true,
						Description = "",
					};

					// ✅ Hirovo için rol ataması - ModuleId eklendi!
					if (request.CompanyId == HirovoCompanyId)
					{
						var role = await _dataAccessLayer.GetRoleById(HirovoDefaultUserRoleId);

						_dataAccessLayer.AddUserRoleWithModule(new UserRole
						{
							Id = Guid.NewGuid(),
							UserId = user.Id,
							CompanyId = HirovoCompanyId,
							RoleId = role.Id,
							ModuleId = HirovoModuleId, // ✅ Hirovo ModuleId eklendi!
							CreatedAt = DateTime.UtcNow,
							UpdatedAt = DateTime.UtcNow,
							IsDeleted = false
						});
					}

					_dataAccessLayer.AddUser(user);
					await _dataAccessLayer.SaveChanges();
				}
				else
				{
					// Mevcut Google kullanıcısının telefon numarasını güncelle
					user.UserName = request.UserName;
					user.Surname = request.Surname;
					user.FirstName = request.FirstName;
					_dataAccessLayer.UpdateUser(user);
					await _dataAccessLayer.SaveChanges();
					Console.WriteLine($"Login Debug - Telefon numarası güncellendi: {request.PhoneNumber}");
				}
				break;

			case "apple":
			case "itunes":
				var appleUserId = request.ExternalProviderUserId;
				user = await _dataAccessLayer.GetUserByExternalId("apple", appleUserId, request.CompanyId);

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
						CompanyId = request.CompanyId,
						IsActive = true,
						PhoneNumber = request.PhoneNumber,
						BirthDate = request.BirthDate ?? DateTime.UtcNow.AddYears(-18),
						City = "",
						District = "",
						IsAvailable = true,
						Description = "",
					};

					// ✅ Hirovo için rol ataması - ModuleId eklendi!
					if (request.CompanyId == HirovoCompanyId)
					{
						var role = await _dataAccessLayer.GetRoleById(HirovoDefaultUserRoleId);

						_dataAccessLayer.AddUserRoleWithModule(new UserRole
						{
							Id = Guid.NewGuid(),
							UserId = user.Id,
							CompanyId = HirovoCompanyId,
							RoleId = role.Id,
							ModuleId = HirovoModuleId, // ✅ Hirovo ModuleId eklendi!
							CreatedAt = DateTime.UtcNow,
							UpdatedAt = DateTime.UtcNow,
							IsDeleted = false
						});
					}

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
					Console.WriteLine($"Login Debug - Apple kullanıcı bilgileri güncellendi: {request.UserName}");
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
				Console.WriteLine($"Login Debug - Native telefon numarası güncellendi: {request.PhoneNumber}");
			}
		}

		// 🆕 AnimalMarket admin email kontrolü - JWT oluşturmadan önce (MEVCUT KOD - DEĞİŞMEDİ)
		var hasAnimalMarketRole = await _dataAccessLayer.HasModuleRole(user.Id, AnimalMarketModuleId);

		if (!hasAnimalMarketRole && user.Email != null)
		{
			Guid? roleIdToAssign = null;

			if (user.Email.ToLower() == "madentechnology@gmail.com")
			{
				roleIdToAssign = Guid.Parse("10000000-0000-0000-0000-000000000100");
				Console.WriteLine($"Login Debug - Administrator rolü atanıyor: {user.Email}");
			}
			else if (user.Email.ToLower() == "admin@besilik.com")
			{
				roleIdToAssign = Guid.Parse("10000000-0000-0000-0000-000000000101");
				Console.WriteLine($"Login Debug - Admin rolü atanıyor: {user.Email}");
			}

			if (roleIdToAssign.HasValue)
			{
				var adminUserRole = new UserRole
				{
					Id = Guid.NewGuid(),
					UserId = user.Id,
					RoleId = roleIdToAssign.Value,
					ModuleId = AnimalMarketModuleId,
					CompanyId = AnimalMarketCompanyId,
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow,
					IsDeleted = false
				};

				_dataAccessLayer.AddUserRoleWithModule(adminUserRole);
				await _dataAccessLayer.SaveChanges();
				Console.WriteLine($"Login Debug - AnimalMarket admin rolü başarıyla atandı: {user.Email}");
			}
		}

		// 🆕 ADIM 6: Kullanıcının rollerini ModuleId ile birlikte çek
		var userRoles = await _dataAccessLayer.GetUserRolesWithModule(user.Id, user.CompanyId);

		// Rolleri "ModuleName.RoleName" formatında hazırla
		var roleStrings = userRoles
			.Select(ur => $"{ur.ModuleName}.{ur.RoleName}")
			.ToList();

		Console.WriteLine($"Login Debug - User Roles: {string.Join(", ", roleStrings)}");

		var expiresAt = DateTime.UtcNow.AddDays(_jwtService.GetExpirationDayCount());
		var refreshToken = _jwtService.GenerateSecureRefreshToken();

		Console.WriteLine($"Login Debug - User ID: {user.Id}, UserName: {user.UserName}, Email: {user.Email}");

		var refreshTokenId = _dataAccessLayer.AddRefreshToken(
			user,
			refreshToken,
			expiresAt,
			request.Platform.ToString(),
			_httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
		);

		Console.WriteLine($"Login Debug - RefreshTokenId: {refreshTokenId}");

		var token = _jwtService.GenerateJwt(
			user.Id,
			user.CompanyId,
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

		Console.WriteLine($"Login Debug - Generated JWT: {(string.IsNullOrEmpty(token) ? "NULL/EMPTY" : "SUCCESS")}");
		Console.WriteLine($"Login Debug - JWT Length: {token?.Length ?? 0}");

		var response = mapper.MapToResponse(token, expiresAt, user, refreshToken);
		Console.WriteLine($"Login Debug - Response JWT: {response.Jwt ?? "NULL"}");

		await _dataAccessLayer.SaveChanges();
		return ArfBlocksResults.Success(response);
	}
}