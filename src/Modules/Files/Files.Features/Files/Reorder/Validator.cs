using FastEndpoints;
using FluentValidation;

namespace Files.Features.Files.Reorder;

public sealed class ReorderValidator : Validator<ReorderRequest>
{
    public ReorderValidator()
    {
        RuleFor(x => x.BucketId).NotEmpty().WithMessage("Bucket ID zorunludur.");
        RuleFor(x => x.FileIds)
            .NotEmpty().WithMessage("Sıralama listesi boş olamaz.")
            .Must(ids => ids.Distinct().Count() == ids.Count).WithMessage("Listede tekrarlayan ID var.");
    }
}
