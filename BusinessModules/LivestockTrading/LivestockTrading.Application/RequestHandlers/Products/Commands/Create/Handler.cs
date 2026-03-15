using System.Text.Json;
using Common.Services.Messaging;
using LivestockTrading.Application.Services;
using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Errors;
using LivestockTrading.Domain.Events;
using Common.Services.ErrorCodeGenerator;

namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Create;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly CurrentUserService _currentUserService;
	private readonly IRabbitMqPublisher _publisher;
	private readonly SubscriptionEnforcementService _subscriptionEnforcement;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
		_publisher = dependencyProvider.GetInstance<IRabbitMqPublisher>();
		_subscriptionEnforcement = dependencyProvider.GetInstance<SubscriptionEnforcementService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		// Get seller ID - auto-create if not provided
		var sellerId = await GetOrCreateSellerId(request.SellerId, cancellationToken);

		// SellerId sağlanmadıysa (auto-resolve akışı) limit kontrolü Validator'da yapılmamıştır
		if (!request.SellerId.HasValue || request.SellerId.Value == Guid.Empty)
		{
			var canCreate = await _subscriptionEnforcement.CanCreateListing(sellerId, cancellationToken);
			if (!canCreate)
				throw new ArfBlocksValidationException(
					ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.SellerSubscriptionErrors.SellerSubscriptionListingLimitReached));
		}

		var entity = mapper.MapToEntity(request);
		entity.SellerId = sellerId;

		await _dataAccessLayer.AddProduct(entity);

		// Otomatik fiyat dönüşümü: diğer para birimleri için ProductPrice kayıtları oluştur
		await CreateAutomaticPriceConversions(entity, cancellationToken);

		// Notify admin/moderator users about new product pending approval
		await NotifyAdmins(entity, cancellationToken);

		var response = mapper.MapToResponse(entity);
		return ArfBlocksResults.Success(response);
	}

	private async Task<Guid> GetOrCreateSellerId(Guid? requestSellerId, CancellationToken ct)
	{
		// If a valid SellerId is provided, use it
		if (requestSellerId.HasValue && requestSellerId.Value != Guid.Empty)
			return requestSellerId.Value;

		// Otherwise, get or create seller for current user
		var currentUserId = _currentUserService.GetCurrentUserId();
		var existingSeller = await _dataAccessLayer.GetSellerByUserId(currentUserId, ct);

		if (existingSeller != null)
			return existingSeller.Id;

		// Create new seller for the user
		var displayName = _currentUserService.GetCurrentUserDisplayName() ?? "Satıcı";
		var newSeller = new Seller
		{
			UserId = currentUserId,
			BusinessName = displayName,
			BusinessType = "Bireysel",
			IsActive = true,
			Status = SellerStatus.PendingVerification
		};

		await _dataAccessLayer.CreateSeller(newSeller, ct);
		return newSeller.Id;
	}

	private async Task CreateAutomaticPriceConversions(Product product, CancellationToken ct)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(product.Currency) || product.BasePrice <= 0)
				return;

			var currencies = await _dataAccessLayer.GetActiveCurrencies(ct);
			var currencyMap = currencies.ToDictionary(c => c.Code, c => c, StringComparer.OrdinalIgnoreCase);

			if (!currencyMap.TryGetValue(product.Currency, out var sourceCurrency))
				return;

			var productPrices = new List<ProductPrice>();

			foreach (var targetCurrency in currencies)
			{
				// Kaynak para birimini atla
				if (string.Equals(targetCurrency.Code, product.Currency, StringComparison.OrdinalIgnoreCase))
					continue;

				if (sourceCurrency.ExchangeRateToUSD <= 0 || targetCurrency.ExchangeRateToUSD <= 0)
					continue;

				var amountInUsd = product.BasePrice / sourceCurrency.ExchangeRateToUSD;
				var convertedPrice = Math.Round(amountInUsd * targetCurrency.ExchangeRateToUSD, 2);

				decimal? convertedDiscountedPrice = null;
				if (product.DiscountedPrice.HasValue)
				{
					var discountInUsd = product.DiscountedPrice.Value / sourceCurrency.ExchangeRateToUSD;
					convertedDiscountedPrice = Math.Round(discountInUsd * targetCurrency.ExchangeRateToUSD, 2);
				}

				productPrices.Add(new ProductPrice
				{
					ProductId = product.Id,
					CurrencyCode = targetCurrency.Code,
					Price = convertedPrice,
					DiscountedPrice = convertedDiscountedPrice,
					IsActive = true,
					IsAutomaticConversion = true
				});
			}

			if (productPrices.Count > 0)
				await _dataAccessLayer.CreateProductPrices(productPrices, ct);
		}
		catch
		{
			// Otomatik fiyat dönüşümü başarısız olursa ürün oluşturma engellenmemeli
		}
	}

	private async Task NotifyAdmins(Product product, CancellationToken ct)
	{
		try
		{
			var adminUserIds = await _dataAccessLayer.GetAdminModeratorUserIds(ct);
			if (adminUserIds.Count == 0)
				return;

			var sellerName = _currentUserService.GetCurrentUserDisplayName() ?? "Satıcı";

			// Create in-app notifications for each admin/moderator
			var notifications = adminUserIds.Select(userId => new Notification
			{
				UserId = userId,
				Title = "Yeni ilan onay bekliyor",
				Message = $"{sellerName} yeni bir ilan oluşturdu: {product.Title}",
				Type = NotificationType.ProductPendingApproval,
				ActionUrl = $"/dashboard/moderation",
				ActionData = JsonSerializer.Serialize(new { ProductId = product.Id, Slug = product.Slug })
			}).ToList();

			await _dataAccessLayer.CreateNotifications(notifications, ct);

			// Publish event for push notifications
			await _publisher.PublishFanout("livestocktrading.notification.push", new ProductCreatedEvent
			{
				ProductId = product.Id,
				Title = product.Title,
				ShortDescription = product.ShortDescription,
				BasePrice = product.BasePrice,
				Currency = product.Currency,
				SellerBusinessName = sellerName,
				SellerId = product.SellerId,
				Slug = product.Slug,
				TargetAdminUserIds = adminUserIds
			});
		}
		catch
		{
			// Notification failure should not block product creation
		}
	}
}
