namespace Livestock.Domain.Errors;

// Symbolic codes — the frontend translates them via its locale dictionary
// (livestock-frontend/common/livestock-api/src/errors/locales/...). The
// previous English/Turkish strings were inlined here and shipped raw to the
// UI, which both broke i18n and made every string change a backend deploy.
public static class LivestockErrors
{
    public static class Common
    {
        public const string NotFound = "COMMON_NOT_FOUND";
        public const string Unauthorized = "COMMON_UNAUTHORIZED";
        public const string InvalidId = "COMMON_INVALID_ID";
    }

    public static class CategoryErrors
    {
        public const string CategoryNotFound = "CATEGORY_NOT_FOUND";
        public const string CategoryNameRequired = "CATEGORY_NAME_REQUIRED";
        public const string CategorySlugRequired = "CATEGORY_SLUG_REQUIRED";
        public const string CategorySlugAlreadyExists = "CATEGORY_SLUG_ALREADY_EXISTS";
    }

    public static class BrandErrors
    {
        public const string BrandNotFound = "BRAND_NOT_FOUND";
        public const string BrandNameRequired = "BRAND_NAME_REQUIRED";
        public const string BrandSlugRequired = "BRAND_SLUG_REQUIRED";
        public const string BrandSlugAlreadyExists = "BRAND_SLUG_ALREADY_EXISTS";
    }

    public static class ProductErrors
    {
        public const string ProductNotFound = "PRODUCT_NOT_FOUND";
        public const string ProductTitleRequired = "PRODUCT_TITLE_REQUIRED";
        public const string ProductPriceRequired = "PRODUCT_PRICE_REQUIRED";
        public const string ProductCategoryRequired = "PRODUCT_CATEGORY_REQUIRED";
        public const string ProductNotOwnedBySeller = "PRODUCT_NOT_OWNED_BY_SELLER";
        public const string ProductCannotBeEdited = "PRODUCT_CANNOT_BE_EDITED";
        public const string ProductAlreadyApproved = "PRODUCT_ALREADY_APPROVED";
        public const string ProductAlreadyRejected = "PRODUCT_ALREADY_REJECTED";
    }

    public static class SellerErrors
    {
        public const string SellerNotFound = "SELLER_NOT_FOUND";
        public const string SellerBusinessNameRequired = "SELLER_BUSINESS_NAME_REQUIRED";
        public const string SellerAlreadyExists = "SELLER_ALREADY_EXISTS";
        public const string SellerNotVerified = "SELLER_NOT_VERIFIED";
        public const string SellerSuspended = "SELLER_SUSPENDED";
        public const string SellerAlreadyVerified = "SELLER_ALREADY_VERIFIED";
        public const string SellerAlreadySuspended = "SELLER_ALREADY_SUSPENDED";
    }

    public static class FarmErrors
    {
        public const string FarmNotFound = "FARM_NOT_FOUND";
        public const string FarmNameRequired = "FARM_NAME_REQUIRED";
        public const string FarmNotOwnedBySeller = "FARM_NOT_OWNED_BY_SELLER";
    }

    public static class OfferErrors
    {
        public const string OfferNotFound = "OFFER_NOT_FOUND";
        public const string OfferPriceRequired = "OFFER_PRICE_REQUIRED";
        public const string OfferAlreadyProcessed = "OFFER_ALREADY_PROCESSED";
        public const string OfferCannotCounterOwn = "OFFER_CANNOT_COUNTER_OWN";
        public const string OfferNotPending = "OFFER_NOT_PENDING";
    }

    public static class DealErrors
    {
        public const string DealNotFound = "DEAL_NOT_FOUND";
        public const string DealAlreadyCompleted = "DEAL_ALREADY_COMPLETED";
        public const string DealAlreadyCancelled = "DEAL_ALREADY_CANCELLED";
    }

    public static class ConversationErrors
    {
        public const string ConversationNotFound = "CONVERSATION_NOT_FOUND";
        public const string ConversationAlreadyExists = "CONVERSATION_ALREADY_EXISTS";
        public const string ConversationNotParticipant = "CONVERSATION_NOT_PARTICIPANT";
    }

    public static class MessageErrors
    {
        public const string MessageNotFound = "MESSAGE_NOT_FOUND";
        public const string MessageContentRequired = "MESSAGE_CONTENT_REQUIRED";
    }

    public static class TransportErrors
    {
        public const string TransportRequestNotFound = "TRANSPORT_REQUEST_NOT_FOUND";
        public const string TransportOfferNotFound = "TRANSPORT_OFFER_NOT_FOUND";
        public const string TransportOfferAlreadyExists = "TRANSPORT_OFFER_ALREADY_EXISTS";
        public const string TransportRequestNotOpen = "TRANSPORT_REQUEST_NOT_OPEN";
        public const string TransporterNotFound = "TRANSPORTER_NOT_FOUND";
        public const string TransporterNotVerified = "TRANSPORTER_NOT_VERIFIED";
        public const string TransporterAlreadyVerified = "TRANSPORTER_ALREADY_VERIFIED";
        public const string TransporterAlreadySuspended = "TRANSPORTER_ALREADY_SUSPENDED";
    }

    public static class ReviewErrors
    {
        public const string ReviewNotFound = "REVIEW_NOT_FOUND";
        public const string ReviewRatingRequired = "REVIEW_RATING_REQUIRED";
        public const string ReviewRatingOutOfRange = "REVIEW_RATING_OUT_OF_RANGE";
        public const string ReviewAlreadySubmitted = "REVIEW_ALREADY_SUBMITTED";
    }

    public static class ShippingErrors
    {
        public const string CarrierNotFound = "SHIPPING_CARRIER_NOT_FOUND";
        public const string CarrierCodeAlreadyExists = "SHIPPING_CARRIER_CODE_ALREADY_EXISTS";
        public const string ZoneNotFound = "SHIPPING_ZONE_NOT_FOUND";
        public const string RateNotFound = "SHIPPING_RATE_NOT_FOUND";
        public const string RateInvalidWeightRange = "SHIPPING_RATE_INVALID_WEIGHT_RANGE";
    }

    public static class SubscriptionErrors
    {
        public const string SubscriptionPlanNotFound = "SUBSCRIPTION_PLAN_NOT_FOUND";
        public const string SubscriptionNotFound = "SUBSCRIPTION_NOT_FOUND";
        public const string BoostPackageNotFound = "BOOST_PACKAGE_NOT_FOUND";
        public const string AlreadyHasActiveSubscription = "SUBSCRIPTION_ALREADY_ACTIVE";
        public const string ListingLimitReached = "SUBSCRIPTION_LISTING_LIMIT_REACHED";
        public const string SubscriptionReceiptRequired = "SUBSCRIPTION_RECEIPT_REQUIRED";
        public const string SubscriptionStoreTransactionIdRequired = "SUBSCRIPTION_STORE_TRANSACTION_ID_REQUIRED";
    }

    public static class NotificationErrors
    {
        public const string NotificationNotFound = "NOTIFICATION_NOT_FOUND";
        public const string NotificationNotOwned = "NOTIFICATION_NOT_OWNED";
    }

    public static class LocationErrors
    {
        public const string LocationNotFound = "LOCATION_NOT_FOUND";
        public const string LocationCountryRequired = "LOCATION_COUNTRY_REQUIRED";
    }

    public static class AppVersionErrors
    {
        public const string AppVersionNotFound = "APP_VERSION_NOT_FOUND";
        public const string AppVersionPlatformRequired = "APP_VERSION_PLATFORM_REQUIRED";
        public const string AppVersionAlreadyExists = "APP_VERSION_ALREADY_EXISTS";
    }

    public static class CurrencyErrors
    {
        public const string CurrencyNotFound = "CURRENCY_NOT_FOUND";
        public const string CurrencyCodeRequired = "CURRENCY_CODE_REQUIRED";
        public const string CurrencyCodeAlreadyExists = "CURRENCY_CODE_ALREADY_EXISTS";
    }

    public static class ReportErrors
    {
        public const string ReportNotFound = "REPORT_NOT_FOUND";
        public const string ReportAlreadyResolved = "REPORT_ALREADY_RESOLVED";
    }

    public static class AdminErrors
    {
        public const string UserNotFound = "ADMIN_USER_NOT_FOUND";
        public const string UserAlreadyBanned = "ADMIN_USER_ALREADY_BANNED";
        public const string UserAlreadyActive = "ADMIN_USER_ALREADY_ACTIVE";
    }

    public static class AnimalInfoErrors
    {
        public const string AnimalInfoNotFound = "ANIMAL_INFO_NOT_FOUND";
        public const string AnimalInfoNotOwnedBySeller = "ANIMAL_INFO_NOT_OWNED_BY_SELLER";
        public const string AnimalInfoAlreadyExists = "ANIMAL_INFO_ALREADY_EXISTS";
    }

    public static class HealthRecordErrors
    {
        public const string HealthRecordNotFound = "HEALTH_RECORD_NOT_FOUND";
        public const string HealthRecordNotOwnedBySeller = "HEALTH_RECORD_NOT_OWNED_BY_SELLER";
    }

    public static class VaccinationErrors
    {
        public const string VaccinationNotFound = "VACCINATION_NOT_FOUND";
        public const string VaccinationNotOwnedBySeller = "VACCINATION_NOT_OWNED_BY_SELLER";
    }

    public static class FeedInfoErrors
    {
        public const string FeedInfoNotFound = "FEED_INFO_NOT_FOUND";
        public const string FeedInfoNotOwnedBySeller = "FEED_INFO_NOT_OWNED_BY_SELLER";
        public const string FeedInfoAlreadyExists = "FEED_INFO_ALREADY_EXISTS";
    }

    public static class ChemicalInfoErrors
    {
        public const string ChemicalInfoNotFound = "CHEMICAL_INFO_NOT_FOUND";
        public const string ChemicalInfoNotOwnedBySeller = "CHEMICAL_INFO_NOT_OWNED_BY_SELLER";
        public const string ChemicalInfoAlreadyExists = "CHEMICAL_INFO_ALREADY_EXISTS";
    }

    public static class MachineryInfoErrors
    {
        public const string MachineryInfoNotFound = "MACHINERY_INFO_NOT_FOUND";
        public const string MachineryInfoNotOwnedBySeller = "MACHINERY_INFO_NOT_OWNED_BY_SELLER";
        public const string MachineryInfoAlreadyExists = "MACHINERY_INFO_ALREADY_EXISTS";
    }

    public static class SeedInfoErrors
    {
        public const string SeedInfoNotFound = "SEED_INFO_NOT_FOUND";
        public const string SeedInfoNotOwnedBySeller = "SEED_INFO_NOT_OWNED_BY_SELLER";
        public const string SeedInfoAlreadyExists = "SEED_INFO_ALREADY_EXISTS";
    }

    public static class VeterinaryInfoErrors
    {
        public const string VeterinaryInfoNotFound = "VETERINARY_INFO_NOT_FOUND";
        public const string VeterinaryInfoNotOwnedBySeller = "VETERINARY_INFO_NOT_OWNED_BY_SELLER";
        public const string VeterinaryInfoAlreadyExists = "VETERINARY_INFO_ALREADY_EXISTS";
    }

    public static class ProductVariantErrors
    {
        public const string ProductVariantNotFound = "PRODUCT_VARIANT_NOT_FOUND";
        public const string ProductVariantNotOwnedBySeller = "PRODUCT_VARIANT_NOT_OWNED_BY_SELLER";
    }

    public static class UserPreferenceErrors
    {
        public const string UserPreferenceNotFound = "USER_PREFERENCE_NOT_FOUND";
    }

    public static class SearchHistoryErrors
    {
        public const string SearchHistoryNotFound = "SEARCH_HISTORY_NOT_FOUND";
    }

    public static class ProductViewHistoryErrors
    {
        public const string ProductViewHistoryNotFound = "PRODUCT_VIEW_HISTORY_NOT_FOUND";
    }
}
