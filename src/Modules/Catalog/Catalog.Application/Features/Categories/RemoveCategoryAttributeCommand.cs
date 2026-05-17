namespace LivestockTrading.Catalog.Application.Features.Categories;

/// <summary>
/// Command for removing a CategoryAttribute child entity from a Category aggregate root.
/// Id-only operation (CategoryId + AttributeId Guid); no DTO body.
/// </summary>
public sealed record RemoveCategoryAttributeCommand(int CategoryId, Guid AttributeId, Guid ActorAdminId);
