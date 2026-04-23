
namespace BaseModules.IAM.Application.RequestHandlers.Roles.Commands.Create;

public class Verificator : IRequestVerificator
{
    private readonly IamDbValidationService _dbVerificator;
    private readonly CurrentUserService _currentUserService;
    private readonly AuthorizationService _authorizationService;

    public Verificator(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbVerificator = dependencyProvider.GetInstance<IamDbValidationService>();
        _currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
        _authorizationService = dependencyProvider.GetInstance<AuthorizationService>();
    }

    public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        var requestPayload = (RequestModel)payload;
        await _authorizationService.ForResource(typeof(Verificator).Namespace)
                                 .VerifyActor()
                                 .Assert();

        // Sadece sistem adminlerine izin ver
        await _authorizationService.CheckAccess(_currentUserService.GetCurrentUserId(), "Roles.Create");

        await Task.CompletedTask;

    }

    public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        //NOP:
        await Task.CompletedTask;

        var requestPayload = (RequestModel)payload;


    }
}