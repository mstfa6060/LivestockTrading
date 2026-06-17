using FastEndpoints;
using FluentValidation;

namespace Files.Features.Files.SetCover;

public sealed class SetCoverValidator : Validator<SetCoverRequest>
{
    public SetCoverValidator()
    {
        RuleFor(x => x.BucketId).NotEmpty().WithMessage("Bucket ID zorunludur.");
        RuleFor(x => x.FileId).NotEmpty().WithMessage("Dosya ID zorunludur.");
    }
}
