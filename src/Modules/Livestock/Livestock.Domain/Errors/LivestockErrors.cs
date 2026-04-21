namespace Livestock.Domain.Errors;

public static class LivestockErrors
{
    public static class Common
    {
        public const string NotFound = "Record not found.";
        public const string Unauthorized = "You are not authorized to perform this action.";
        public const string InvalidId = "The provided ID is invalid.";
    }

    public static class CategoryErrors
    {
        public const string CategoryNotFound = "Category not found.";
        public const string CategoryNameRequired = "Category name is required.";
        public const string CategorySlugRequired = "Category slug is required.";
        public const string CategorySlugAlreadyExists = "A category with this slug already exists.";
    }

    public static class BrandErrors
    {
        public const string BrandNotFound = "Brand not found.";
        public const string BrandNameRequired = "Brand name is required.";
        public const string BrandSlugRequired = "Brand slug is required.";
        public const string BrandSlugAlreadyExists = "A brand with this slug already exists.";
    }

    public static class ProductErrors
    {
        public const string ProductNotFound = "Product not found.";
        public const string ProductTitleRequired = "Product title is required.";
        public const string ProductPriceRequired = "Product price is required.";
        public const string ProductCategoryRequired = "Product category is required.";
        public const string ProductNotOwnedBySeller = "This product does not belong to you.";
        public const string ProductCannotBeEdited = "This product cannot be edited in its current status.";
        public const string ProductAlreadyApproved = "This product is already approved.";
        public const string ProductAlreadyRejected = "This product is already rejected.";
    }

    public static class SellerErrors
    {
        public const string SellerNotFound = "Seller not found.";
        public const string SellerBusinessNameRequired = "Business name is required.";
        public const string SellerAlreadyExists = "You are already registered as a seller.";
        public const string SellerNotVerified = "Your seller account is not verified yet.";
        public const string SellerSuspended = "Your seller account is suspended.";
        public const string SellerAlreadyVerified = "Seller is already verified.";
        public const string SellerAlreadySuspended = "Seller is already suspended.";
    }

    public static class FarmErrors
    {
        public const string FarmNotFound = "Farm not found.";
        public const string FarmNameRequired = "Farm name is required.";
        public const string FarmNotOwnedBySeller = "This farm does not belong to you.";
    }

    public static class OfferErrors
    {
        public const string OfferNotFound = "Offer not found.";
        public const string OfferPriceRequired = "Offer price is required.";
        public const string OfferAlreadyProcessed = "This offer has already been processed.";
        public const string OfferCannotCounterOwn = "You cannot counter your own offer.";
        public const string OfferNotPending = "This offer is not in pending status.";
    }

    public static class DealErrors
    {
        public const string DealNotFound = "Deal not found.";
        public const string DealAlreadyCompleted = "This deal is already completed.";
        public const string DealAlreadyCancelled = "This deal is already cancelled.";
    }

    public static class ConversationErrors
    {
        public const string ConversationNotFound = "Conversation not found.";
        public const string ConversationAlreadyExists = "A conversation between these users about this product already exists.";
        public const string ConversationNotParticipant = "You are not a participant of this conversation.";
    }

    public static class MessageErrors
    {
        public const string MessageNotFound = "Message not found.";
        public const string MessageContentRequired = "Message content is required.";
    }

    public static class TransportErrors
    {
        public const string TransportRequestNotFound = "Transport request not found.";
        public const string TransportOfferNotFound = "Transport offer not found.";
        public const string TransportOfferAlreadyExists = "You already submitted an offer for this request.";
        public const string TransportRequestNotOpen = "This transport request is no longer accepting offers.";
        public const string TransporterNotFound = "Transporter not found.";
        public const string TransporterNotVerified = "Your transporter account is not verified.";
        public const string TransporterAlreadyVerified = "Transporter is already verified.";
        public const string TransporterAlreadySuspended = "Transporter is already suspended.";
    }

    public static class ReviewErrors
    {
        public const string ReviewNotFound = "Review not found.";
        public const string ReviewRatingRequired = "Rating is required.";
        public const string ReviewRatingOutOfRange = "Rating must be between 1 and 5.";
        public const string ReviewAlreadySubmitted = "You have already submitted a review.";
    }

    public static class SubscriptionErrors
    {
        public const string SubscriptionPlanNotFound = "Subscription plan not found.";
        public const string SubscriptionNotFound = "Subscription not found.";
        public const string BoostPackageNotFound = "Boost package not found.";
        public const string AlreadyHasActiveSubscription = "You already have an active subscription.";
        public const string ListingLimitReached = "You have reached your listing limit. Upgrade your subscription to create more listings.";
    }

    public static class NotificationErrors
    {
        public const string NotificationNotFound = "Notification not found.";
        public const string NotificationNotOwned = "This notification does not belong to you.";
    }

    public static class LocationErrors
    {
        public const string LocationNotFound = "Location not found.";
        public const string LocationCountryRequired = "Country code is required.";
    }

    public static class AppVersionErrors
    {
        public const string AppVersionNotFound = "App version config not found.";
        public const string AppVersionPlatformRequired = "Platform is required.";
        public const string AppVersionAlreadyExists = "App version config for this platform already exists.";
    }

    public static class CurrencyErrors
    {
        public const string CurrencyNotFound = "Currency not found.";
        public const string CurrencyCodeRequired = "Currency code is required.";
        public const string CurrencyCodeAlreadyExists = "A currency with this code already exists.";
    }

    public static class ReportErrors
    {
        public const string ReportNotFound = "Report not found.";
        public const string ReportAlreadyResolved = "This report is already resolved.";
    }

    public static class AdminErrors
    {
        public const string UserNotFound = "User not found.";
        public const string UserAlreadyBanned = "User is already banned.";
        public const string UserAlreadyActive = "User is already active.";
    }

    public static class AnimalInfoErrors
    {
        public const string AnimalInfoNotFound = "Animal info not found.";
        public const string AnimalInfoNotOwnedBySeller = "This animal info does not belong to you.";
        public const string AnimalInfoAlreadyExists = "Animal info for this product already exists.";
    }

    public static class HealthRecordErrors
    {
        public const string HealthRecordNotFound = "Health record not found.";
        public const string HealthRecordNotOwnedBySeller = "This health record does not belong to you.";
    }

    public static class VaccinationErrors
    {
        public const string VaccinationNotFound = "Vaccination record not found.";
        public const string VaccinationNotOwnedBySeller = "This vaccination record does not belong to you.";
    }

    public static class FeedInfoErrors
    {
        public const string FeedInfoNotFound = "Feed info not found.";
        public const string FeedInfoNotOwnedBySeller = "This feed info does not belong to you.";
        public const string FeedInfoAlreadyExists = "Feed info for this product already exists.";
    }

    public static class ChemicalInfoErrors
    {
        public const string ChemicalInfoNotFound = "Chemical info not found.";
        public const string ChemicalInfoNotOwnedBySeller = "This chemical info does not belong to you.";
        public const string ChemicalInfoAlreadyExists = "Chemical info for this product already exists.";
    }

    public static class MachineryInfoErrors
    {
        public const string MachineryInfoNotFound = "Machinery info not found.";
        public const string MachineryInfoNotOwnedBySeller = "This machinery info does not belong to you.";
        public const string MachineryInfoAlreadyExists = "Machinery info for this product already exists.";
    }

    public static class SeedInfoErrors
    {
        public const string SeedInfoNotFound = "Seed info not found.";
        public const string SeedInfoNotOwnedBySeller = "This seed info does not belong to you.";
        public const string SeedInfoAlreadyExists = "Seed info for this product already exists.";
    }

    public static class VeterinaryInfoErrors
    {
        public const string VeterinaryInfoNotFound = "Veterinary info not found.";
        public const string VeterinaryInfoNotOwnedBySeller = "This veterinary info does not belong to you.";
        public const string VeterinaryInfoAlreadyExists = "Veterinary info for this product already exists.";
    }

    public static class ProductVariantErrors
    {
        public const string ProductVariantNotFound = "Product variant not found.";
        public const string ProductVariantNotOwnedBySeller = "This variant does not belong to you.";
    }

    public static class UserPreferenceErrors
    {
        public const string UserPreferenceNotFound = "User preferences not found.";
    }

    public static class SearchHistoryErrors
    {
        public const string SearchHistoryNotFound = "Search history not found.";
    }

    public static class ProductViewHistoryErrors
    {
        public const string ProductViewHistoryNotFound = "Product view history not found.";
    }
}
