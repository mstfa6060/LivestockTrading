$appPath = "D:\Projects\GlobalLivestock\backend\BusinessModules\LivestockTrading\LivestockTrading.Application"

$replacements = @{
    # Brand
    'BrandErrors\.NameRequired' = 'BrandErrors.BrandNameRequired'
    'BrandErrors\.SlugRequired' = 'BrandErrors.BrandSlugRequired'

    # Category
    'CategoryErrors\.NameRequired' = 'CategoryErrors.CategoryNameRequired'
    'CategoryErrors\.SlugRequired' = 'CategoryErrors.CategorySlugRequired'

    # Product
    'ProductErrors\.TitleRequired' = 'ProductErrors.ProductTitleRequired'
    'ProductErrors\.SlugRequired' = 'ProductErrors.ProductSlugRequired'
    'ProductErrors\.CategoryRequired' = 'ProductErrors.ProductCategoryRequired'
    'ProductErrors\.SellerRequired' = 'ProductErrors.ProductSellerRequired'
    'ProductErrors\.LocationRequired' = 'ProductErrors.ProductLocationRequired'
    'ProductErrors\.PriceRequired' = 'ProductErrors.ProductPriceRequired'

    # Location
    'LocationErrors\.NameRequired' = 'LocationErrors.LocationNameRequired'

    # Seller
    'SellerErrors\.BusinessNameRequired' = 'SellerErrors.SellerBusinessNameRequired'
    'SellerErrors\.UserIdRequired' = 'SellerErrors.SellerUserIdRequired'

    # Farm
    'FarmErrors\.NameRequired' = 'FarmErrors.FarmNameRequired'
    'FarmErrors\.SellerRequired' = 'FarmErrors.FarmSellerRequired'
    'FarmErrors\.LocationRequired' = 'FarmErrors.FarmLocationRequired'

    # ProductVariant
    'ProductVariantErrors\.NameRequired' = 'ProductVariantErrors.VariantNameRequired'
    'ProductVariantErrors\.ProductRequired' = 'ProductVariantErrors.VariantProductRequired'

    # ProductPrice
    'ProductPriceErrors\.CurrencyCodeRequired' = 'ProductPriceErrors.PriceCurrencyCodeRequired'
    'ProductPriceErrors\.PriceRequired' = 'ProductPriceErrors.PriceAmountRequired'
    'ProductPriceErrors\.ProductRequired' = 'ProductPriceErrors.PriceProductRequired'

    # ProductImage
    'ProductImageErrors\.UrlRequired' = 'ProductImageErrors.ImageUrlRequired'
    'ProductImageErrors\.ProductRequired' = 'ProductImageErrors.ImageProductRequired'

    # ProductVideo
    'ProductVideoErrors\.UrlRequired' = 'ProductVideoErrors.VideoUrlRequired'
    'ProductVideoErrors\.ProductRequired' = 'ProductVideoErrors.VideoProductRequired'

    # ProductDocument
    'ProductDocumentErrors\.UrlRequired' = 'ProductDocumentErrors.DocumentUrlRequired'
    'ProductDocumentErrors\.FileNameRequired' = 'ProductDocumentErrors.DocumentFileNameRequired'
    'ProductDocumentErrors\.ProductRequired' = 'ProductDocumentErrors.DocumentProductRequired'

    # AnimalInfo
    'AnimalInfoErrors\.BreedNameRequired' = 'AnimalInfoErrors.AnimalBreedNameRequired'
    'AnimalInfoErrors\.ProductRequired' = 'AnimalInfoErrors.AnimalProductRequired'

    # HealthRecord
    'HealthRecordErrors\.RecordTypeRequired' = 'HealthRecordErrors.HealthRecordTypeRequired'
    'HealthRecordErrors\.RecordDateRequired' = 'HealthRecordErrors.HealthRecordDateRequired'
    'HealthRecordErrors\.AnimalInfoRequired' = 'HealthRecordErrors.HealthRecordAnimalInfoRequired'

    # Vaccination
    'VaccinationErrors\.VaccineNameRequired' = 'VaccinationErrors.VaccinationNameRequired'
    'VaccinationErrors\.VaccinationDateRequired' = 'VaccinationErrors.VaccinationDateRequired'
    'VaccinationErrors\.AnimalInfoRequired' = 'VaccinationErrors.VaccinationAnimalInfoRequired'

    # ChemicalInfo
    'ChemicalInfoErrors\.ProductRequired' = 'ChemicalInfoErrors.ChemicalProductRequired'

    # MachineryInfo
    'MachineryInfoErrors\.ProductRequired' = 'MachineryInfoErrors.MachineryProductRequired'

    # SeedInfo
    'SeedInfoErrors\.ProductRequired' = 'SeedInfoErrors.SeedProductRequired'

    # FeedInfo
    'FeedInfoErrors\.ProductRequired' = 'FeedInfoErrors.FeedProductRequired'

    # VeterinaryInfo
    'VeterinaryInfoErrors\.ProductRequired' = 'VeterinaryInfoErrors.VeterinaryProductRequired'

    # ProductReview
    'ProductReviewErrors\.ProductRequired' = 'ProductReviewErrors.ProductReviewProductRequired'
    'ProductReviewErrors\.UserRequired' = 'ProductReviewErrors.ProductReviewUserRequired'
    'ProductReviewErrors\.RatingRequired' = 'ProductReviewErrors.ProductReviewRatingRequired'

    # SellerReview
    'SellerReviewErrors\.SellerRequired' = 'SellerReviewErrors.SellerReviewSellerRequired'
    'SellerReviewErrors\.UserRequired' = 'SellerReviewErrors.SellerReviewUserRequired'
    'SellerReviewErrors\.RatingRequired' = 'SellerReviewErrors.SellerReviewRatingRequired'

    # TransporterReview
    'TransporterReviewErrors\.TransporterRequired' = 'TransporterReviewErrors.TransporterReviewTransporterRequired'
    'TransporterReviewErrors\.UserRequired' = 'TransporterReviewErrors.TransporterReviewUserRequired'
    'TransporterReviewErrors\.RatingRequired' = 'TransporterReviewErrors.TransporterReviewRatingRequired'

    # FavoriteProduct
    'FavoriteProductErrors\.ProductRequired' = 'FavoriteProductErrors.FavoriteProductRequired'
    'FavoriteProductErrors\.UserRequired' = 'FavoriteProductErrors.FavoriteUserRequired'

    # Notification
    'NotificationErrors\.TitleRequired' = 'NotificationErrors.NotificationTitleRequired'
    'NotificationErrors\.MessageRequired' = 'NotificationErrors.NotificationMessageRequired'
    'NotificationErrors\.UserRequired' = 'NotificationErrors.NotificationUserRequired'

    # SearchHistory
    'SearchHistoryErrors\.QueryRequired' = 'SearchHistoryErrors.SearchHistoryQueryRequired'
    'SearchHistoryErrors\.UserRequired' = 'SearchHistoryErrors.SearchHistoryUserRequired'

    # ProductViewHistory
    'ProductViewHistoryErrors\.ProductRequired' = 'ProductViewHistoryErrors.ViewHistoryProductRequired'
    'ProductViewHistoryErrors\.UserRequired' = 'ProductViewHistoryErrors.ViewHistoryUserRequired'

    # UserPreferences
    'UserPreferencesErrors\.UserRequired' = 'UserPreferencesErrors.PreferencesUserRequired'

    # Conversation
    'ConversationErrors\.ParticipantsRequired' = 'ConversationErrors.ConversationParticipantsRequired'

    # Message
    'MessageErrors\.ContentRequired' = 'MessageErrors.MessageContentRequired'
    'MessageErrors\.ConversationRequired' = 'MessageErrors.MessageConversationRequired'
    'MessageErrors\.SenderRequired' = 'MessageErrors.MessageSenderRequired'
    'MessageErrors\.RecipientRequired' = 'MessageErrors.MessageRecipientRequired'

    # Offer
    'OfferErrors\.ProductRequired' = 'OfferErrors.OfferProductRequired'
    'OfferErrors\.PriceRequired' = 'OfferErrors.OfferPriceRequired'
    'OfferErrors\.QuantityRequired' = 'OfferErrors.OfferQuantityRequired'
    'OfferErrors\.BuyerRequired' = 'OfferErrors.OfferBuyerRequired'
    'OfferErrors\.SellerRequired' = 'OfferErrors.OfferSellerRequired'

    # Deal
    'DealErrors\.NumberRequired' = 'DealErrors.DealNumberRequired'
    'DealErrors\.ProductRequired' = 'DealErrors.DealProductRequired'
    'DealErrors\.SellerRequired' = 'DealErrors.DealSellerRequired'
    'DealErrors\.BuyerRequired' = 'DealErrors.DealBuyerRequired'
    'DealErrors\.PriceRequired' = 'DealErrors.DealPriceRequired'

    # Transporter
    'TransporterErrors\.CompanyNameRequired' = 'TransporterErrors.TransporterCompanyNameRequired'
    'TransporterErrors\.EmailRequired' = 'TransporterErrors.TransporterEmailRequired'
    'TransporterErrors\.UserRequired' = 'TransporterErrors.TransporterUserRequired'

    # TransportRequest
    'TransportRequestErrors\.ProductRequired' = 'TransportRequestErrors.TransportRequestProductRequired'
    'TransportRequestErrors\.SellerRequired' = 'TransportRequestErrors.TransportRequestSellerRequired'
    'TransportRequestErrors\.BuyerRequired' = 'TransportRequestErrors.TransportRequestBuyerRequired'
    'TransportRequestErrors\.PickupLocationRequired' = 'TransportRequestErrors.TransportRequestPickupLocationRequired'
    'TransportRequestErrors\.DeliveryLocationRequired' = 'TransportRequestErrors.TransportRequestDeliveryLocationRequired'

    # TransportOffer
    'TransportOfferErrors\.TransportRequestRequired' = 'TransportOfferErrors.TransportOfferRequestRequired'
    'TransportOfferErrors\.TransporterRequired' = 'TransportOfferErrors.TransportOfferTransporterRequired'
    'TransportOfferErrors\.PriceRequired' = 'TransportOfferErrors.TransportOfferPriceRequired'

    # TransportTracking
    'TransportTrackingErrors\.TransportRequestRequired' = 'TransportTrackingErrors.TransportTrackingRequestRequired'

    # Currency
    'CurrencyErrors\.CodeRequired' = 'CurrencyErrors.CurrencyCodeRequired'
    'CurrencyErrors\.NameRequired' = 'CurrencyErrors.CurrencyNameRequired'
    'CurrencyErrors\.SymbolRequired' = 'CurrencyErrors.CurrencySymbolRequired'

    # Language
    'LanguageErrors\.CodeRequired' = 'LanguageErrors.LanguageCodeRequired'
    'LanguageErrors\.NameRequired' = 'LanguageErrors.LanguageNameRequired'

    # PaymentMethod
    'PaymentMethodErrors\.NameRequired' = 'PaymentMethodErrors.PaymentMethodNameRequired'
    'PaymentMethodErrors\.CodeRequired' = 'PaymentMethodErrors.PaymentMethodCodeRequired'

    # ShippingCarrier
    'ShippingCarrierErrors\.NameRequired' = 'ShippingCarrierErrors.ShippingCarrierNameRequired'
    'ShippingCarrierErrors\.CodeRequired' = 'ShippingCarrierErrors.ShippingCarrierCodeRequired'

    # FAQ
    'FAQErrors\.QuestionRequired' = 'FAQErrors.FAQQuestionRequired'
    'FAQErrors\.AnswerRequired' = 'FAQErrors.FAQAnswerRequired'

    # Banner
    'BannerErrors\.TitleRequired' = 'BannerErrors.BannerTitleRequired'
    'BannerErrors\.ImageUrlRequired' = 'BannerErrors.BannerImageUrlRequired'
    'BannerErrors\.DateRequired' = 'BannerErrors.BannerDateRequired'
    'BannerErrors\.StartDateRequired' = 'BannerErrors.BannerStartDateRequired'
    'BannerErrors\.EndDateRequired' = 'BannerErrors.BannerEndDateRequired'

    # TaxRate
    'TaxRateErrors\.CountryCodeRequired' = 'TaxRateErrors.TaxRateCountryCodeRequired'
    'TaxRateErrors\.NameRequired' = 'TaxRateErrors.TaxRateNameRequired'
    'TaxRateErrors\.RateRequired' = 'TaxRateErrors.TaxRateAmountRequired'

    # ShippingZone
    'ShippingZoneErrors\.NameRequired' = 'ShippingZoneErrors.ShippingZoneNameRequired'

    # ShippingRate
    'ShippingRateErrors\.ZoneRequired' = 'ShippingRateErrors.ShippingRateZoneRequired'
    'ShippingRateErrors\.CostRequired' = 'ShippingRateErrors.ShippingRateCostRequired'
}

$files = Get-ChildItem -Path $appPath -Recurse -Filter "*.cs"
$count = 0

foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName -Raw
    $modified = $false

    foreach ($key in $replacements.Keys) {
        if ($content -match $key) {
            $content = $content -replace $key, $replacements[$key]
            $modified = $true
        }
    }

    if ($modified) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        $count++
        Write-Host "Updated: $($file.FullName)"
    }
}

Write-Host "`nTotal files updated: $count"
