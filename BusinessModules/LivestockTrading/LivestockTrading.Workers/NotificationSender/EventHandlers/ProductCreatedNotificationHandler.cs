using System.Text.Json;
using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.NotificationSender.Services;

namespace LivestockTrading.Workers.NotificationSender.EventHandlers;

public class ProductCreatedNotificationHandler
{
	private readonly IPushNotificationService _pushNotificationService;
	private readonly PushTokenRepository _pushTokenRepository;
	private readonly ILogger<ProductCreatedNotificationHandler> _logger;

	public ProductCreatedNotificationHandler(
		IPushNotificationService pushNotificationService,
		PushTokenRepository pushTokenRepository,
		ILogger<ProductCreatedNotificationHandler> logger)
	{
		_pushNotificationService = pushNotificationService;
		_pushTokenRepository = pushTokenRepository;
		_logger = logger;
	}

	public async Task HandleAsync(ProductCreatedEvent productEvent)
	{
		_logger.LogInformation(
			"Processing ProductCreatedEvent notification. ProductId: {ProductId}, Title: {Title}, TargetAdmins: {AdminCount}",
			productEvent.ProductId,
			productEvent.Title,
			productEvent.TargetAdminUserIds?.Count ?? 0);

		var title = "Yeni ilan onay bekliyor";
		var body = !string.IsNullOrEmpty(productEvent.SellerBusinessName)
			? $"{productEvent.SellerBusinessName}: {productEvent.Title} - {productEvent.BasePrice} {productEvent.Currency}"
			: $"Yeni ilan: {productEvent.Title} - {productEvent.BasePrice} {productEvent.Currency}";

		var data = new Dictionary<string, string>
		{
			{ "type", "product_pending_approval" },
			{ "productId", productEvent.ProductId.ToString() },
			{ "slug", productEvent.Slug ?? "" },
			{ "sellerId", productEvent.SellerId.ToString() }
		};

		var adminUserIds = productEvent.TargetAdminUserIds ?? new List<Guid>();
		if (adminUserIds.Count > 0)
		{
			var tokensByUser = await _pushTokenRepository.GetActiveTokensForUsers(adminUserIds);

			foreach (var adminUserId in adminUserIds)
			{
				if (!await _pushTokenRepository.IsPushEnabled(adminUserId))
					continue;

				if (tokensByUser.TryGetValue(adminUserId, out var tokens) && tokens.Count > 0)
				{
					var (successCount, invalidTokens) = await _pushNotificationService.SendPushWithCleanupAsync(tokens, title, body, data);
					_logger.LogInformation("Sent push to {SuccessCount}/{TotalCount} devices for admin {UserId}",
						successCount, tokens.Count, adminUserId);

					if (invalidTokens.Count > 0)
						await _pushTokenRepository.RevokeTokens(invalidTokens);
				}

				// In-app bildirim kaydet
				var actionData = JsonSerializer.Serialize(new
				{
					productId = productEvent.ProductId,
					slug = productEvent.Slug,
					sellerId = productEvent.SellerId,
					sellerName = productEvent.SellerBusinessName
				});

				await _pushTokenRepository.SaveNotification(
					adminUserId,
					title,
					body,
					NotificationType.ProductPendingApproval,
					actionData);
			}
		}
		else
		{
			_logger.LogWarning("No target admin user IDs provided for ProductCreatedEvent. ProductId: {ProductId}", productEvent.ProductId);
		}
	}
}
