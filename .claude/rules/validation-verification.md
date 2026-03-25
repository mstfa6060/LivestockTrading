---
paths:
  - "**/Verificator.cs"
  - "**/Validator.cs"
  - "**/RequestHandlers/**"
---
# Verificator & Validator Pattern

Her endpoint'te Verificator.cs ve Validator.cs dosyalari bulunur:

## Verificator.cs - Yetkilendirme ve varlik kontrolu
```csharp
using {Module}.Infrastructure.Services;

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
        await _authorizationService
            .ForResource(typeof(Verificator).Namespace)
            .VerifyActor()
            .Assert();
    }

    public async Task VerificateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        var request = (RequestModel)payload;
        // Varlik kontrolu (entity mevcut mu?)
        await _dbVerification.ValidateCategoryExists(request.Id, cancellationToken);
    }
}
```

- **Commands (Create/Update/Delete)**: VerificateDomain icinde entity varlik kontrolu yapilir
- **Queries (All/Detail/Pick)**: VerificateDomain genellikle `await Task.CompletedTask;` olarak birakilir

## Validator.cs - Is kurallari ve FluentValidation
```csharp
using FluentValidation;
using {Module}.Domain.Errors;
using {Module}.Infrastructure.Services;
using Common.Services.ErrorCodeGenerator;

public class Validator : IRequestValidator
{
    private readonly LivestockTradingModuleDbValidationService _dbValidator;

    public Validator(ArfBlocksDependencyProvider dependencyProvider)
    {
        _dbValidator = dependencyProvider.GetInstance<LivestockTradingModuleDbValidationService>();
    }

    public void ValidateRequestModel(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        var request = (RequestModel)payload;
        var result = new RequestModel_Validator().Validate(request);
        if (!result.IsValid)
            throw new ArfBlocksValidationException(result.ToString("~"));
    }

    public async Task ValidateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        var request = (RequestModel)payload;
        await _dbValidator.ValidateCategoryExists(request.Id, cancellationToken);
        await _dbValidator.ValidateCategorySlugUnique(request.Slug, request.Id, cancellationToken);
    }
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
    public RequestModel_Validator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CategoryErrors.NameRequired));
    }
}
```

## DbVerificationService vs DbValidationService

| | DbVerificationService | DbValidationService |
|---|---|---|
| Kullanim yeri | Verificator.VerificateDomain | Validator.ValidateDomain |
| Amac | Yetki + hizli varlik kontrolu | Is kurallari + state kontrolu |
| Constructor | `(ModuleDbContext dbContext)` | `(ArfBlocksDependencyProvider dp)` |
| Base class | `DefinitionDbValidationService` | `DefinitionDbValidationService` |
| Konum | `Infrastructure/Services/` | `Infrastructure/Services/` |

## DI Kaydi (ApplicationDependencyProvider)
```csharp
// Services
base.Add<LivestockTradingModuleDbVerificationService>();
base.Add<LivestockTradingModuleDbValidationService>();
```
