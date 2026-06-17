using FastEndpoints;
using FluentValidation;

namespace Files.Features.Files.Delete;

public sealed class DeleteFileValidator : Validator<DeleteFileRequest>
{
    public DeleteFileValidator()
    {
        RuleFor(x => x.BucketId).NotEmpty().WithMessage("Bucket ID zorunludur.");
        RuleFor(x => x.FileId).NotEmpty().WithMessage("Dosya ID zorunludur.");
    }
}
