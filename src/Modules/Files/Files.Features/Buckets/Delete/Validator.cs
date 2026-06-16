using FastEndpoints;
using FluentValidation;

namespace Files.Features.Buckets.Delete;

public sealed class DeleteBucketValidator : Validator<DeleteBucketRequest>
{
    public DeleteBucketValidator()
    {
        RuleFor(x => x.BucketId)
            .NotEmpty().WithMessage("Bucket ID zorunludur.");
    }
}
