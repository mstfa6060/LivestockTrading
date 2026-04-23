using FastEndpoints;
using FluentValidation;

namespace Files.Features.Files.Upload;

public sealed class UploadValidator : Validator<UploadRequest>
{
    public UploadValidator()
    {
        // BucketId may be Guid.Empty on the first upload — the endpoint
        // auto-creates a bucket from ModuleName/BucketType in that case.
        // Enforce that ModuleName + BucketType are supplied when they have
        // to drive that creation.
        When(x => x.BucketId == System.Guid.Empty, () =>
        {
            RuleFor(x => x.ModuleName)
                .NotEmpty()
                .WithMessage("ModuleName, yeni bir bucket oluşturulurken zorunludur.");
            RuleFor(x => x.BucketType)
                .NotNull()
                .WithMessage("BucketType, yeni bir bucket oluşturulurken zorunludur.");
        });
    }
}
