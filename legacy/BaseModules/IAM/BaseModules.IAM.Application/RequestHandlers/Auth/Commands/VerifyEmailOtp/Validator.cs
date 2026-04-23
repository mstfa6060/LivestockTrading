namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.VerifyEmailOtp;

public class Validator : IRequestValidator
{
    public Validator(ArfBlocksDependencyProvider dependencyProvider) { }

    public void ValidateRequestModel(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        var requestModel = (RequestModel)payload;
        var validationResult = new RequestModel_Validator().Validate(requestModel);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToString("~");
            throw new ArfBlocksValidationException(errors);
        }
    }

    public async Task ValidateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
    public RequestModel_Validator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email.Required")
            .EmailAddress().WithMessage("Email.Invalid");

        RuleFor(x => x.OtpCode)
            .NotEmpty().WithMessage("OtpCode.Required")
            .Length(6).WithMessage("OtpCode.LengthMustBe6");
    }
}
