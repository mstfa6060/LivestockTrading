using FastEndpoints;
using FluentValidation;

namespace Files.Features.Buckets.Create;

public sealed class CreateBucketValidator : Validator<CreateBucketRequest>
{
    public CreateBucketValidator()
    {
        RuleFor(x => x.Module)
            .NotEmpty().WithMessage("Modül adı zorunludur.")
            .MaximumLength(100);
    }
}
