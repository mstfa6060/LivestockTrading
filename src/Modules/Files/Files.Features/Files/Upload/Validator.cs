using FastEndpoints;
using FluentValidation;

namespace Files.Features.Files.Upload;

public sealed class UploadValidator : Validator<UploadRequest>
{
    public UploadValidator()
    {
        RuleFor(x => x.BucketId)
            .NotEmpty().WithMessage("Bucket ID zorunludur.");
    }
}
