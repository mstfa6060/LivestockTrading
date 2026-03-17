namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Delete;

/// <summary>
/// Hesap Silme (Account Deletion)
/// Kullanicinin kendi hesabini sifre onayiyla kalici olarak (soft-delete) silmesi.
/// Iliskili tum veriler (Seller, Products, Conversations, Messages, vb.) temizlenir.
/// Refresh token'lar iptal edilir.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly CurrentUserService _currentUserService;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, DataAccess dataAccess)
	{
		_dataAccessLayer = dataAccess;
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		// Get current user ID from JWT
		var currentUserId = _currentUserService.GetCurrentUserId();

		// Get the user with tracking enabled for update
		var user = await _dataAccessLayer.GetUserById(currentUserId, cancellationToken);
		if (user == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserNotFound));

		// Verify password
		var isPasswordValid = SecurityHelper.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);
		if (!isPasswordValid)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => DomainErrors.UserErrors.UserDeletePasswordIncorrect));

		// 1. Soft-delete related LivestockTrading data (Sellers, Products, Conversations, Messages, etc.)
		await _dataAccessLayer.SoftDeleteRelatedLivestockTradingData(currentUserId, cancellationToken);

		// 2. Soft-delete user roles
		await _dataAccessLayer.SoftDeleteUserRoles(currentUserId, cancellationToken);

		// 3. Invalidate all refresh tokens (force logout from all devices)
		await _dataAccessLayer.DeleteAllRefreshTokens(currentUserId, cancellationToken);

		// 4. Soft-delete the user account
		await _dataAccessLayer.SoftDeleteUser(user, cancellationToken);

		var response = mapper.MapToResponse(true, "Hesabiniz basariyla silindi.");
		return ArfBlocksResults.Success(response);
	}
}
