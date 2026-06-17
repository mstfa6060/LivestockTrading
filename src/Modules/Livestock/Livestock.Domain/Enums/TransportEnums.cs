namespace Livestock.Domain.Enums;

public enum TransporterStatus { PendingVerification = 0, Active = 1, Suspended = 2, Banned = 3, Inactive = 4 }
public enum TransportType { Road = 0, Sea = 1, Air = 2, Rail = 3, Multimodal = 4 }
public enum TransportRequestStatus { Pending = 0, InPool = 1, ReceivingOffers = 2, Assigned = 3, PickedUp = 4, InTransit = 5, Delivered = 6, Cancelled = 7, Completed = 8 }
public enum TransportOfferStatus { Pending = 0, Accepted = 1, Rejected = 2, Expired = 3, Withdrawn = 4 }
public enum TrackingStatus { Update = 0, ArrivedAtPickup = 1, Loaded = 2, Departed = 3, RestStop = 4, BorderCrossing = 5, ArrivedAtDestination = 6, Unloaded = 7, Delivered = 8, Delay = 9, Issue = 10 }
