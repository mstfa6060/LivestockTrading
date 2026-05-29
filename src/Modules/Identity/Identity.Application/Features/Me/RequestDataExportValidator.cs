using FluentValidation;

namespace LivestockTrading.Identity.Application.Features.Me;

public sealed class RequestDataExportValidator : AbstractValidator<RequestDataExportCommand>
{
    public RequestDataExportValidator()
    {
        RuleFor(x => x.Dto.Format)
            .NotEmpty().WithMessage("Format is required.")
            .Must(f => f == "json").WithMessage("Only 'json' format is supported (Faz 1).");
    }
}
