using LivestockTrading.Catalog.Application.Abstractions;
using LivestockTrading.Catalog.Domain.Entities;
using MassTransit;
using Shared.Domain;
using Shared.Results;
using Shared.Text;
using Shared.ValueObjects;

namespace LivestockTrading.Catalog.Application.Features.Locations;

public sealed class CreateLocationHandler : IConsumer<CreateLocationCommand>
{
    private readonly IReferenceDataRepository _repo;

    public CreateLocationHandler(IReferenceDataRepository repo) => _repo = repo;

    public async Task Consume(ConsumeContext<CreateLocationCommand> context)
    {
        var ct = context.CancellationToken;
        var dto = context.Message.Dto;

        try
        {
            // KAYDET-22: Contracts→Domain LocationLevel inline switch + defansif _ => throw (ham cast yasak)
            var domainLevel = dto.Level switch
            {
                Shared.Contracts.Catalog.LocationLevel.Country => LocationLevel.Country,
                Shared.Contracts.Catalog.LocationLevel.Region => LocationLevel.Region,
                Shared.Contracts.Catalog.LocationLevel.State => LocationLevel.State,
                Shared.Contracts.Catalog.LocationLevel.District => LocationLevel.District,
                Shared.Contracts.Catalog.LocationLevel.Neighborhood => LocationLevel.Neighborhood,
                _ => throw new DomainException($"Unknown LocationLevel: {dto.Level}")
            };

            // Slug = SlugHelper.Normalize(Code) — Plan-4.5
            var slug = SlugHelper.Normalize(dto.Code);

            // Path = parent zinciri — Plan-4.5
            string path;
            if (dto.ParentId is null)
            {
                path = $"{dto.CountryCode.ToLowerInvariant()}/{slug}";
            }
            else
            {
                var parent = await _repo.GetLocationByIdAsync(dto.ParentId.Value, ct);
                if (parent is null)
                {
                    await context.RespondAsync<Result<int>>(Result.Failure<int>(
                        new Error("NOT_FOUND_PARENT_LOCATION", $"Parent location not found: {dto.ParentId}")));
                    return;
                }
                path = $"{parent.Path}/{slug}";
            }

            // CountryCode VO — ctor (CC1: Parse/From YOK); geçersiz → DomainException → LOCATION_RULE_VIOLATION
            var countryCode = new CountryCode(dto.CountryCode);

            var location = new Location(
                parentId: dto.ParentId,
                level: domainLevel,
                countryCode: countryCode,
                code: dto.Code,
                slug: slug,
                path: path,
                name: dto.Name,
                nativeName: dto.NativeName,
                centroid: null,
                population: dto.Population,
                displayOrder: dto.DisplayOrder);

            await _repo.AddLocationAsync(location, ct);
            await context.RespondAsync<Result<int>>(Result.Success(location.Id));
        }
        catch (DomainException ex)
        {
            await context.RespondAsync<Result<int>>(Result.Failure<int>(
                new Error("LOCATION_RULE_VIOLATION", ex.Message)));
        }
    }
}
