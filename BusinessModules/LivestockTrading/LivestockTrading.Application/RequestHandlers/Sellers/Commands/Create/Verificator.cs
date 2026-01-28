using LivestockTrading.Infrastructure.Services;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Commands.Create;

public class Verificator : IRequestVerificator
{
	private readonly AuthorizationService _authorizationService;
	private readonly LivestockTradingModuleDbVerificationService _dbVerification;

	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_authorizationService = dependencyProvider.GetInstance<AuthorizationService>();
		_dbVerification = dependencyProvider.GetInstance<LivestockTradingModuleDbVerificationService>();
	}

	public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		// Satıcı başvurusu: Giriş yapmış herkes başvurabilir (Buyer → Seller)
		// Özel rol kontrolü yok - sadece authentication
		await _authorizationService
			.ForResource(typeof(Verificator).Namespace)
			.VerifyActor()
			.Assert();
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		// TODO: Kullanıcının zaten Seller olmadığını kontrol et
		await Task.CompletedTask;
	}
}
