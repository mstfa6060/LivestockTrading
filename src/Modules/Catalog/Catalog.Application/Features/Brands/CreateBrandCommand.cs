using Shared.Contracts.Catalog.Admin;

namespace LivestockTrading.Catalog.Application.Features.Brands;

/// <summary>
/// Command for creating a new Brand aggregate root (admin path).
/// Dto.CategoryCodes resolved to int category ids via ICategoryRepository.GetByCodeAsync
/// (fail-fast on first missing), then Brand.CreateByAdmin(...). Slug uniqueness is a DB
/// constraint (Wave 3); Brand Id is server-generated Guid v7.
/// </summary>
public sealed record CreateBrandCommand(CreateBrandDto Dto, Guid ActorAdminId);
