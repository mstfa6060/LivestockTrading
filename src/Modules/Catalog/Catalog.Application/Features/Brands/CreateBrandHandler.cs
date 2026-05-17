using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Domain.Aggregates;
using MassTransit;
using Shared.Domain;
using Shared.Results;
using Shared.ValueObjects;

namespace LivestockTrading.Catalog.Application.Features.Brands;

public sealed class CreateBrandHandler : IConsumer<CreateBrandCommand>
{
    private readonly IBrandRepository _brandRepo;
    private readonly ICategoryRepository _categoryRepo;

    public CreateBrandHandler(IBrandRepository brandRepo, ICategoryRepository categoryRepo)
    {
        _brandRepo = brandRepo;
        _categoryRepo = categoryRepo;
    }

    public async Task Consume(ConsumeContext<CreateBrandCommand> context)
    {
        var dto = context.Message.Dto;
        var ct = context.CancellationToken;

        try
        {
            // N-CategoryCode resolve (fail-fast: ilk bulunamayan → NOT_FOUND_PARENT_CATEGORY)
            var categoryIds = new List<int>();
            foreach (var categoryCode in dto.CategoryCodes)
            {
                var category = await _categoryRepo.GetByCodeAsync(categoryCode, ct);
                if (category is null)
                {
                    var notFoundError = new Error(
                        "NOT_FOUND_PARENT_CATEGORY",
                        $"Parent category not found: {categoryCode}");
                    await context.RespondAsync<Result<Guid>>(Result.Failure<Guid>(notFoundError));
                    return;
                }
                categoryIds.Add(category.Id);
            }

            // string? → CountryCode? (ctor 2-char/ascii guard; geçersiz → DomainException)
            CountryCode? originCountry = dto.OriginCountryCode is null
                ? null
                : new CountryCode(dto.OriginCountryCode);

            var brand = Brand.CreateByAdmin(
                slug: dto.Slug,
                name: dto.Name,
                adminUserId: context.Message.ActorAdminId,
                categoryIds: categoryIds,
                logoUrl: dto.LogoUrl,
                website: dto.Website,
                originCountry: originCountry,
                description: dto.Description,
                displayOrder: dto.DisplayOrder);

            await _brandRepo.AddAsync(brand, ct);
            await context.RespondAsync<Result<Guid>>(Result.Success(brand.Id));
        }
        catch (DomainException ex)
        {
            var error = new Error("BRAND_RULE_VIOLATION", ex.Message);
            await context.RespondAsync<Result<Guid>>(Result.Failure<Guid>(error));
        }
    }
}
