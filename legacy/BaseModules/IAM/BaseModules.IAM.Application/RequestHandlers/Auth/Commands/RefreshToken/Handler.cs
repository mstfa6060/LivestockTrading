namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.RefreshToken;

/// <summary>
/// Token Yenileme İşlemi
/// Bu endpoint, süresi dolmuş JWT token'ını yenilemek için kullanılır.
/// Refresh token ile yeni bir JWT ve refresh token çifti üretir.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly IJwtService _jwtService;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, DataAccess dataAccess)
	{
		_dataAccessLayer = dataAccess;
		_jwtService = dependencyProvider.GetInstance<IJwtService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var mapper = new Mapper();
		var request = (RequestModel)payload;

		var result = await _dataAccessLayer.GetUserByRefreshToken(request.RefreshToken);
		var user = result.User;
		var oldTokenId = result.RefreshTokenId;

		// Eski refresh token'ı sil
		await _dataAccessLayer.DeleteRefreshTokenById(oldTokenId);

		// Yeni refresh token oluştur
		var newRefreshToken = _jwtService.GenerateSecureRefreshToken();
		var expiresAt = DateTime.UtcNow.AddDays(_jwtService.GetExpirationDayCount());

		var newRefreshTokenId = _dataAccessLayer.AddRefreshToken(
			user,
			newRefreshToken,
			expiresAt,
			request.Platform.ToString(),
			null // IP alma işlemini senin kullanımına göre genişletebiliriz
		);

		// Kullanıcıyı güncelle
		await _dataAccessLayer.SaveChangesAsync();

		// Kullanıcının rollerini ModuleId ile birlikte çek
		var userRoles = await _dataAccessLayer.GetUserRolesWithModule(user.Id);

		// Rolleri "ModuleName.RoleName" formatında hazırla
		var roleStrings = userRoles
			.Select(ur => $"{ur.ModuleName}.{ur.RoleName}")
			.ToList();

		// Rolleri JWT'ye ekle
		var jwt = _jwtService.GenerateJwt(
			user.Id,
			Guid.Empty,
			user.UserName,
			$"{user.FirstName} {user.Surname}",
			user.Email,
			user.BucketId,
			request.Platform,
			expiresAt,
			user.UserSource,
			newRefreshTokenId,
			roleStrings
		);

		var response = mapper.Map(jwt, expiresAt, newRefreshToken, user);
		return ArfBlocksResults.Success(response);
	}
}