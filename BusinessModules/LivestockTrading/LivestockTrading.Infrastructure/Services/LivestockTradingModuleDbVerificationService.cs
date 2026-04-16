using Arfware.ArfBlocks.Core.Exceptions;
using Common.Definitions.Infrastructure.Services;
using LivestockTrading.Domain.Errors;
using LivestockTrading.Infrastructure.RelationalDB;
using Common.Services.ErrorCodeGenerator;
using Microsoft.EntityFrameworkCore;

namespace LivestockTrading.Infrastructure.Services;

public class LivestockTradingModuleDbVerificationService : DefinitionDbValidationService
{
	private readonly LivestockTradingModuleDbContext _dbContext;

	public LivestockTradingModuleDbVerificationService(LivestockTradingModuleDbContext dbContext) : base(dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task ValidateCategoryExists(Guid categoryId, CancellationToken ct = default)
	{
		var exists = await _dbContext.Categories
			.AsNoTracking()
			.AnyAsync(c => c.Id == categoryId && !c.IsDeleted, ct);

		if (!exists)
		{
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CategoryErrors.CategoryNotFound));
		}
	}

	public async Task ValidateCategoryIsActive(Guid categoryId, CancellationToken ct = default)
	{
		var category = await _dbContext.Categories
			.AsNoTracking()
			.FirstOrDefaultAsync(c => c.Id == categoryId && !c.IsDeleted, ct);

		if (category == null)
		{
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CategoryErrors.CategoryNotFound));
		}

		if (!category.IsActive)
		{
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CategoryErrors.CategoryNotActive));
		}
	}

	public async Task ValidateParentCategoryExists(Guid? parentCategoryId, CancellationToken ct = default)
	{
		if (parentCategoryId == null || parentCategoryId == Guid.Empty)
			return;

		var exists = await _dbContext.Categories
			.AsNoTracking()
			.AnyAsync(c => c.Id == parentCategoryId && !c.IsDeleted, ct);

		if (!exists)
		{
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CategoryErrors.ParentCategoryNotFound));
		}
	}

	// Ownership: Product belongs to the current user's seller profile
	public async Task ValidateProductOwnership(Guid productId, Guid currentUserId, CancellationToken ct = default)
	{
		var isOwner = await _dbContext.Products
			.AsNoTracking()
			.Where(p => p.Id == productId && !p.IsDeleted)
			.Join(_dbContext.Sellers.AsNoTracking().Where(s => !s.IsDeleted),
				p => p.SellerId,
				s => s.Id,
				(p, s) => s.UserId)
			.AnyAsync(userId => userId == currentUserId, ct);

		if (!isOwner)
		{
			throw new ArfBlocksVerificationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.AuthorizationErrors.OwnershipRequired));
		}
	}

	// Ownership: Farm belongs to the current user's seller profile
	public async Task ValidateFarmOwnership(Guid farmId, Guid currentUserId, CancellationToken ct = default)
	{
		var isOwner = await _dbContext.Farms
			.AsNoTracking()
			.Where(f => f.Id == farmId && !f.IsDeleted)
			.Join(_dbContext.Sellers.AsNoTracking().Where(s => !s.IsDeleted),
				f => f.SellerId,
				s => s.Id,
				(f, s) => s.UserId)
			.AnyAsync(userId => userId == currentUserId, ct);

		if (!isOwner)
		{
			throw new ArfBlocksVerificationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.AuthorizationErrors.OwnershipRequired));
		}
	}

	// Brand
	public async Task ValidateBrandExists(Guid brandId, CancellationToken ct = default)
	{
		var exists = await _dbContext.Brands.AsNoTracking().AnyAsync(e => e.Id == brandId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.BrandErrors.BrandNotFound));
	}

	// Product
	public async Task ValidateProductExists(Guid productId, CancellationToken ct = default)
	{
		var exists = await _dbContext.Products.AsNoTracking().AnyAsync(e => e.Id == productId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductErrors.ProductNotFound));
	}

	// Location
	public async Task ValidateLocationExists(Guid locationId, CancellationToken ct = default)
	{
		var exists = await _dbContext.Locations.AsNoTracking().AnyAsync(e => e.Id == locationId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.LocationErrors.LocationNotFound));
	}

	// Seller
	public async Task ValidateSellerExists(Guid sellerId, CancellationToken ct = default)
	{
		var exists = await _dbContext.Sellers.AsNoTracking().AnyAsync(e => e.Id == sellerId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.SellerErrors.SellerNotFound));
	}

	// Farm
	public async Task ValidateFarmExists(Guid farmId, CancellationToken ct = default)
	{
		var exists = await _dbContext.Farms.AsNoTracking().AnyAsync(e => e.Id == farmId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.FarmErrors.FarmNotFound));
	}

	// ProductVariant
	public async Task ValidateProductVariantExists(Guid variantId, CancellationToken ct = default)
	{
		var exists = await _dbContext.ProductVariants.AsNoTracking().AnyAsync(e => e.Id == variantId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductVariantErrors.VariantNotFound));
	}

	// ProductPrice
	public async Task ValidateProductPriceExists(Guid priceId, CancellationToken ct = default)
	{
		var exists = await _dbContext.ProductPrices.AsNoTracking().AnyAsync(e => e.Id == priceId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductPriceErrors.PriceNotFound));
	}

	// AnimalInfo
	public async Task ValidateAnimalInfoExists(Guid animalInfoId, CancellationToken ct = default)
	{
		var exists = await _dbContext.AnimalInfos.AsNoTracking().AnyAsync(e => e.Id == animalInfoId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.AnimalInfoErrors.AnimalInfoNotFound));
	}

	// HealthRecord
	public async Task ValidateHealthRecordExists(Guid healthRecordId, CancellationToken ct = default)
	{
		var exists = await _dbContext.HealthRecords.AsNoTracking().AnyAsync(e => e.Id == healthRecordId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.HealthRecordErrors.HealthRecordNotFound));
	}

	// Vaccination
	public async Task ValidateVaccinationExists(Guid vaccinationId, CancellationToken ct = default)
	{
		var exists = await _dbContext.Vaccinations.AsNoTracking().AnyAsync(e => e.Id == vaccinationId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.VaccinationErrors.VaccinationNotFound));
	}

	// ChemicalInfo
	public async Task ValidateChemicalInfoExists(Guid chemicalInfoId, CancellationToken ct = default)
	{
		var exists = await _dbContext.ChemicalInfos.AsNoTracking().AnyAsync(e => e.Id == chemicalInfoId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ChemicalInfoErrors.ChemicalInfoNotFound));
	}

	// MachineryInfo
	public async Task ValidateMachineryInfoExists(Guid machineryInfoId, CancellationToken ct = default)
	{
		var exists = await _dbContext.MachineryInfos.AsNoTracking().AnyAsync(e => e.Id == machineryInfoId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.MachineryInfoErrors.MachineryInfoNotFound));
	}

	// SeedInfo
	public async Task ValidateSeedInfoExists(Guid seedInfoId, CancellationToken ct = default)
	{
		var exists = await _dbContext.SeedInfos.AsNoTracking().AnyAsync(e => e.Id == seedInfoId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.SeedInfoErrors.SeedInfoNotFound));
	}

	// FeedInfo
	public async Task ValidateFeedInfoExists(Guid feedInfoId, CancellationToken ct = default)
	{
		var exists = await _dbContext.FeedInfos.AsNoTracking().AnyAsync(e => e.Id == feedInfoId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.FeedInfoErrors.FeedInfoNotFound));
	}

	// VeterinaryInfo
	public async Task ValidateVeterinaryInfoExists(Guid veterinaryInfoId, CancellationToken ct = default)
	{
		var exists = await _dbContext.VeterinaryInfos.AsNoTracking().AnyAsync(e => e.Id == veterinaryInfoId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.VeterinaryInfoErrors.VeterinaryInfoNotFound));
	}

	// ProductReview
	public async Task ValidateProductReviewExists(Guid reviewId, CancellationToken ct = default)
	{
		var exists = await _dbContext.ProductReviews.AsNoTracking().AnyAsync(e => e.Id == reviewId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductReviewErrors.ProductReviewNotFound));
	}

	// SellerReview
	public async Task ValidateSellerReviewExists(Guid reviewId, CancellationToken ct = default)
	{
		var exists = await _dbContext.SellerReviews.AsNoTracking().AnyAsync(e => e.Id == reviewId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.SellerReviewErrors.SellerReviewNotFound));
	}

	// TransporterReview
	public async Task ValidateTransporterReviewExists(Guid reviewId, CancellationToken ct = default)
	{
		var exists = await _dbContext.TransporterReviews.AsNoTracking().AnyAsync(e => e.Id == reviewId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.TransporterReviewErrors.TransporterReviewNotFound));
	}

	// FavoriteProduct
	public async Task ValidateFavoriteProductExists(Guid favoriteId, CancellationToken ct = default)
	{
		var exists = await _dbContext.FavoriteProducts.AsNoTracking().AnyAsync(e => e.Id == favoriteId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.FavoriteProductErrors.FavoriteNotFound));
	}

	// Notification
	public async Task ValidateNotificationExists(Guid notificationId, CancellationToken ct = default)
	{
		var exists = await _dbContext.Notifications.AsNoTracking().AnyAsync(e => e.Id == notificationId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.NotificationErrors.NotificationNotFound));
	}

	// SearchHistory
	public async Task ValidateSearchHistoryExists(Guid searchHistoryId, CancellationToken ct = default)
	{
		var exists = await _dbContext.SearchHistories.AsNoTracking().AnyAsync(e => e.Id == searchHistoryId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.SearchHistoryErrors.SearchHistoryNotFound));
	}

	// ProductViewHistory
	public async Task ValidateProductViewHistoryExists(Guid viewHistoryId, CancellationToken ct = default)
	{
		var exists = await _dbContext.ProductViewHistories.AsNoTracking().AnyAsync(e => e.Id == viewHistoryId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductViewHistoryErrors.ViewHistoryNotFound));
	}

	// UserPreferences
	public async Task ValidateUserPreferencesExists(Guid preferencesId, CancellationToken ct = default)
	{
		var exists = await _dbContext.UserPreferences.AsNoTracking().AnyAsync(e => e.Id == preferencesId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.UserPreferencesErrors.PreferencesNotFound));
	}

	// Conversation
	public async Task ValidateConversationExists(Guid conversationId, CancellationToken ct = default)
	{
		var exists = await _dbContext.Conversations.AsNoTracking().AnyAsync(e => e.Id == conversationId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ConversationErrors.ConversationNotFound));
	}

	// Message
	public async Task ValidateMessageExists(Guid messageId, CancellationToken ct = default)
	{
		var exists = await _dbContext.Messages.AsNoTracking().AnyAsync(e => e.Id == messageId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.MessageErrors.MessageNotFound));
	}

	// Offer
	public async Task ValidateOfferExists(Guid offerId, CancellationToken ct = default)
	{
		var exists = await _dbContext.Offers.AsNoTracking().AnyAsync(e => e.Id == offerId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.OfferErrors.OfferNotFound));
	}

	// Deal
	public async Task ValidateDealExists(Guid dealId, CancellationToken ct = default)
	{
		var exists = await _dbContext.Deals.AsNoTracking().AnyAsync(e => e.Id == dealId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.DealErrors.DealNotFound));
	}

	// Transporter
	public async Task ValidateTransporterExists(Guid transporterId, CancellationToken ct = default)
	{
		var exists = await _dbContext.Transporters.AsNoTracking().AnyAsync(e => e.Id == transporterId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.TransporterErrors.TransporterNotFound));
	}

	// TransportRequest
	public async Task ValidateTransportRequestExists(Guid requestId, CancellationToken ct = default)
	{
		var exists = await _dbContext.TransportRequests.AsNoTracking().AnyAsync(e => e.Id == requestId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.TransportRequestErrors.TransportRequestNotFound));
	}

	// TransportOffer
	public async Task ValidateTransportOfferExists(Guid offerId, CancellationToken ct = default)
	{
		var exists = await _dbContext.TransportOffers.AsNoTracking().AnyAsync(e => e.Id == offerId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.TransportOfferErrors.TransportOfferNotFound));
	}

	// TransportTracking
	public async Task ValidateTransportTrackingExists(Guid trackingId, CancellationToken ct = default)
	{
		var exists = await _dbContext.TransportTrackings.AsNoTracking().AnyAsync(e => e.Id == trackingId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.TransportTrackingErrors.TransportTrackingNotFound));
	}

	// Ownership: Seller profile belongs to the current user
	public async Task ValidateSellerOwnership(Guid sellerId, Guid currentUserId, CancellationToken ct = default)
	{
		var isOwner = await _dbContext.Sellers
			.AsNoTracking()
			.AnyAsync(s => s.Id == sellerId && !s.IsDeleted && s.UserId == currentUserId, ct);

		if (!isOwner)
		{
			throw new ArfBlocksVerificationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.AuthorizationErrors.OwnershipRequired));
		}
	}

	// Ownership: Transporter profile belongs to the current user
	public async Task ValidateTransporterOwnership(Guid transporterId, Guid currentUserId, CancellationToken ct = default)
	{
		var isOwner = await _dbContext.Transporters
			.AsNoTracking()
			.AnyAsync(t => t.Id == transporterId && !t.IsDeleted && t.UserId == currentUserId, ct);

		if (!isOwner)
		{
			throw new ArfBlocksVerificationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.AuthorizationErrors.OwnershipRequired));
		}
	}

	// Ownership: ProductImage belongs to the current user's seller profile (via Product → Seller)
	public async Task ValidateProductImageOwnership(Guid imageId, Guid currentUserId, CancellationToken ct = default)
	{
		var isOwner = await _dbContext.ProductImages
			.AsNoTracking()
			.Where(pi => pi.Id == imageId && !pi.IsDeleted)
			.Join(_dbContext.Products.AsNoTracking().Where(p => !p.IsDeleted),
				pi => pi.ProductId,
				p => p.Id,
				(pi, p) => p.SellerId)
			.Join(_dbContext.Sellers.AsNoTracking().Where(s => !s.IsDeleted),
				sellerId => sellerId,
				s => s.Id,
				(sellerId, s) => s.UserId)
			.AnyAsync(userId => userId == currentUserId, ct);

		if (!isOwner)
		{
			throw new ArfBlocksVerificationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.AuthorizationErrors.OwnershipRequired));
		}
	}

	// ProductImage
	public async Task ValidateProductImageExists(Guid imageId, CancellationToken ct = default)
	{
		var exists = await _dbContext.ProductImages.AsNoTracking().AnyAsync(e => e.Id == imageId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductImageErrors.ImageNotFound));
	}

	// Currency
	public async Task ValidateCurrencyExists(Guid currencyId, CancellationToken ct = default)
	{
		var exists = await _dbContext.Currencies.AsNoTracking().AnyAsync(e => e.Id == currencyId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CurrencyErrors.CurrencyNotFound));
	}

	// Language
	public async Task ValidateLanguageExists(Guid languageId, CancellationToken ct = default)
	{
		var exists = await _dbContext.Languages.AsNoTracking().AnyAsync(e => e.Id == languageId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.LanguageErrors.LanguageNotFound));
	}

	// PaymentMethod
	public async Task ValidatePaymentMethodExists(Guid paymentMethodId, CancellationToken ct = default)
	{
		var exists = await _dbContext.PaymentMethods.AsNoTracking().AnyAsync(e => e.Id == paymentMethodId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.PaymentMethodErrors.PaymentMethodNotFound));
	}

	// ShippingCarrier
	public async Task ValidateShippingCarrierExists(Guid carrierId, CancellationToken ct = default)
	{
		var exists = await _dbContext.ShippingCarriers.AsNoTracking().AnyAsync(e => e.Id == carrierId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ShippingCarrierErrors.ShippingCarrierNotFound));
	}

	// FAQ
	public async Task ValidateFAQExists(Guid faqId, CancellationToken ct = default)
	{
		var exists = await _dbContext.FAQs.AsNoTracking().AnyAsync(e => e.Id == faqId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.FAQErrors.FAQNotFound));
	}

	// Banner
	public async Task ValidateBannerExists(Guid bannerId, CancellationToken ct = default)
	{
		var exists = await _dbContext.Banners.AsNoTracking().AnyAsync(e => e.Id == bannerId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.BannerErrors.BannerNotFound));
	}

	// TaxRate
	public async Task ValidateTaxRateExists(Guid taxRateId, CancellationToken ct = default)
	{
		var exists = await _dbContext.TaxRates.AsNoTracking().AnyAsync(e => e.Id == taxRateId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.TaxRateErrors.TaxRateNotFound));
	}

	// ShippingZone
	public async Task ValidateShippingZoneExists(Guid zoneId, CancellationToken ct = default)
	{
		var exists = await _dbContext.ShippingZones.AsNoTracking().AnyAsync(e => e.Id == zoneId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ShippingZoneErrors.ShippingZoneNotFound));
	}

	// ShippingRate
	public async Task ValidateShippingRateExists(Guid rateId, CancellationToken ct = default)
	{
		var exists = await _dbContext.ShippingRates.AsNoTracking().AnyAsync(e => e.Id == rateId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ShippingRateErrors.ShippingRateNotFound));
	}

	// BoostPackage
	public async Task ValidateBoostPackageExists(Guid boostPackageId, CancellationToken ct = default)
	{
		var exists = await _dbContext.BoostPackages.AsNoTracking().AnyAsync(e => e.Id == boostPackageId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.BoostPackageErrors.BoostPackageNotFound));
	}

	// SubscriptionPlan
	public async Task ValidateSubscriptionPlanExists(Guid planId, CancellationToken ct = default)
	{
		var exists = await _dbContext.SubscriptionPlans.AsNoTracking().AnyAsync(e => e.Id == planId && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.SubscriptionPlanErrors.SubscriptionPlanNotFound));
	}

	// AppVersion
	public async Task ValidateAppVersionExists(Guid id, CancellationToken ct = default)
	{
		var exists = await _dbContext.AppVersionConfigs.AsNoTracking().AnyAsync(e => e.Id == id && !e.IsDeleted, ct);
		if (!exists)
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.AppVersionErrors.AppVersionNotFound));
	}

	public async Task ValidateAppVersionPlatformUnique(int platform, Guid? excludeId = null, CancellationToken ct = default)
	{
		var query = _dbContext.AppVersionConfigs.AsNoTracking().Where(e => e.Platform == platform && !e.IsDeleted);
		if (excludeId.HasValue) query = query.Where(e => e.Id != excludeId.Value);
		if (await query.AnyAsync(ct))
			throw new ArfBlocksValidationException(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.AppVersionErrors.AppVersionPlatformAlreadyExists));
	}
}
