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
}
