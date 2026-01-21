// namespace LivestockTrading.Application.RequestHandlers.Students.Commands.Create;

// public class Validator : IRequestValidator
// {
//     public Validator(ArfBlocksDependencyProvider dp)
//     {
//     }

//     public void ValidateRequestModel(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
//     {
//         var m = (RequestModel)payload;
//         var result = new RequestModel_Validator().Validate(m);
//         if (!result.IsValid)
//             throw new ArfBlocksValidationException(result.ToString("~"));
//     }

//     public Task ValidateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
//     {
//         return Task.CompletedTask;
//     }
// }

// public class RequestModel_Validator : AbstractValidator<RequestModel>
// {
//     public RequestModel_Validator()
//     {
//         RuleFor(x => x.StudentNumber)
//             .NotEmpty()
//             .WithErrorCode(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.NameNotValid));

//         RuleFor(x => x.FirstName)
//             .NotEmpty()
//             .WithErrorCode(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.NameNotValid));

//         RuleFor(x => x.LastName)
//             .NotEmpty()
//             .WithErrorCode(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.NameNotValid));

//         RuleFor(x => x.Email)
//             .EmailAddress()
//             .When(x => !string.IsNullOrEmpty(x.Email))
//             .WithErrorCode(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.NameNotValid));
//     }
// }
