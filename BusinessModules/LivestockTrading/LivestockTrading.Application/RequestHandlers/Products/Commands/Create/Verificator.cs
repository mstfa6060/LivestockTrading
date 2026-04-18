using LivestockTrading.Application.Authorization;
using LivestockTrading.Infrastructure.Services;
using Common.Services.Auth.CurrentUser;

namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Create;

public class Verificator : IRequestVerificator
{
	private readonly PermissionService _permissionService;
	private readonly LivestockTradingModuleDbVerificationService _dbVerification;
	private readonly CurrentUserService _currentUserService;

	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_permissionService = dependencyProvider.GetInstance<PermissionService>();
		_dbVerification = dependencyProvider.GetInstance<LivestockTradingModuleDbVerificationService>();
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
	}

	public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		// Rol kontrolü: Sadece Seller veya Admin ürün oluşturabilir
		_permissionService.RequireAnyRole(
			Constants.LivestockTradingConstants.Roles.Seller,
			Constants.LivestockTradingConstants.Roles.Admin);
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;

		// Eğer SellerId açıkça gönderilmişse, o Seller'ın mevcut kullanıcıya ait olup olmadığını kontrol et.
		// Admin/Moderator herhangi bir seller adına ilan oluşturabilir; normal Seller sadece kendisi için.
		if (request.SellerId.HasValue && request.SellerId.Value != Guid.Empty && !_permissionService.IsModerator())
		{
			var currentUserId = _currentUserService.GetCurrentUserId();
			await _dbVerification.ValidateSellerOwnership(request.SellerId.Value, currentUserId, cancellationToken);
		}
	}
}
