using FastEndpoints;
using FluentValidation;

namespace Files.Features.Files.GetPresignedUrl;

public sealed class GetPresignedUrlValidator : Validator<GetPresignedUrlRequest>
{
    public GetPresignedUrlValidator()
    {
        RuleFor(x => x.FileId).NotEmpty().WithMessage("Dosya ID zorunludur.");
        RuleFor(x => x.ExpirySeconds)
            .InclusiveBetween(60, 604800).WithMessage("Geçerlilik süresi 60-604800 saniye arasında olmalıdır.");
    }
}
