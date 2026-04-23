namespace Livestock.Domain.Enums;

public enum OfferStatus { Pending = 0, Accepted = 1, Rejected = 2, Countered = 3, Expired = 4, Withdrawn = 5 }
public enum DealStatus { Agreed = 0, AwaitingPayment = 1, Paid = 2, Preparing = 3, InDelivery = 4, Delivered = 5, Completed = 6, Cancelled = 7 }
public enum DeliveryMethod { ToBeDecided = 0, SelfPickup = 1, SellerDelivery = 2, ThirdPartyTransport = 3, Courier = 4 }
public enum ConversationStatus { Active = 0, Archived = 1, Resolved = 2, Blocked = 3 }
