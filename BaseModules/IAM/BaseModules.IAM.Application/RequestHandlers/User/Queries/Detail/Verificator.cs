namespace BaseModules.IAM.Application.RequestHandlers.Users.Queries.Detail;

public class Verificator : IRequestVerificator
{
    private readonly AuthorizationService _authorizationService;

    public Verificator(ArfBlocksDependencyProvider dependencyProvider)
    {
        _authorizationService = dependencyProvider.GetInstance<AuthorizationService>();
    }

    public async Task VerificateActor(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        var request = (RequestModel)payload;

        await _authorizationService
            .ForResource(typeof(Verificator).Namespace)
            .VerifyTenant<Common.Definitions.Domain.Entities.User>(request.UserId)
            .VerifyActor()
            .Assert();
    }

    public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
