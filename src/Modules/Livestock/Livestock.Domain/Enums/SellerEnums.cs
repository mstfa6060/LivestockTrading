namespace Livestock.Domain.Enums;

public enum SellerStatus { PendingVerification = 0, Active = 1, Suspended = 2, Banned = 3, Inactive = 4 }
public enum FarmType { Livestock = 0, Crop = 1, Dairy = 2, Poultry = 3, Aquaculture = 4, Greenhouse = 5, Vineyard = 6, Orchard = 7, MixedFarming = 8, Other = 99 }
public enum LocationType { ProductLocation = 0, FarmLocation = 1, UserAddress = 2, WarehouseLocation = 3, ShippingAddress = 4, BillingAddress = 5 }
