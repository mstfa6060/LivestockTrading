namespace Livestock.Features.Offers.Create;

public record CreateOfferRequest(Guid ProductId, decimal OfferedPrice, string CurrencyCode, int Quantity, string? Note);
