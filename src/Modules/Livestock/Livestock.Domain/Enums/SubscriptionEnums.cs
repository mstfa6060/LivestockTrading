namespace Livestock.Domain.Enums;

public enum SubscriptionTargetType { Seller = 0, Transporter = 1 }
public enum SubscriptionTier { Free = 0, Basic = 1, Pro = 2, Business = 3 }
public enum SubscriptionStatus { Active = 0, Expired = 1, Cancelled = 2, GracePeriod = 3 }
public enum SubscriptionPeriod { Monthly = 0, Yearly = 1 }
public enum SubscriptionPlatform { Apple = 0, Google = 1, Web = 2 }
public enum BoostType { Daily = 0, Weekly = 1, Mega = 2 }
public enum IAPTransactionType { Subscription = 0, Boost = 1 }
public enum IAPTransactionStatus { Pending = 0, Validated = 1, Failed = 2, Refunded = 3 }
