using LivestockTrading.Infrastructure.Services;

namespace LivestockTrading.Application.RequestHandlers.Messages.Queries.All;

public class Verificator : IRequestVerificator
{
	private readonly LivestockTradingModuleDbVerificationService _dbVerification;
	private readonly CurrentUserService _currentUserService;

	public Verificator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbVerification = dependencyProvider.GetInstance<LivestockTradingModuleDbVerificationService>();
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
	}

	public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		// JWT Gateway'de dogrulaniyor; rol kisitlamasi yok.
		await Task.CompletedTask;
	}

	public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		// Conversation membership JWT'de tasinmadigi icin DB kontrolu zorunlu.
		var request = (RequestModel)payload;
		await _dbVerification.ValidateUserIsParticipantOfConversation(request.ConversationId, _currentUserService.GetCurrentUserId(), cancellationToken);
	}
}
