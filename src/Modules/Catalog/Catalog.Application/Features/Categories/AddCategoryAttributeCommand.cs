using Shared.Contracts.Catalog.Admin;

namespace LivestockTrading.Catalog.Application.Features.Categories;

/// <summary>
/// Command for adding a CategoryAttribute child entity to a Category aggregate root.
/// AttributeDto contains 9 fields; Category.AddAttribute factory creates the child internally.
/// </summary>
public sealed record AddCategoryAttributeCommand(int CategoryId, AttributeDto Dto, Guid ActorAdminId);
