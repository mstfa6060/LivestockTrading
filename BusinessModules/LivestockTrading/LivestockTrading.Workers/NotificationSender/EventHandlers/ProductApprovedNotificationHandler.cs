using System.Text.Json;
using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.NotificationSender.Services;

namespace LivestockTrading.Workers.NotificationSender.EventHandlers;

public class ProductApprovedNotificationHandler
{
	private readonly IPushNotificationService _pushNotificationService;
	private readonly PushTokenRepository _pushTokenRepository;
	private readonly ILogger<ProductApprovedNotificationHandler> _logger;

	public ProductApprovedNotificationHandler(
		IPushNotificationService pushNotificationService,
		PushTokenRepository pushTokenRepository,
		ILogger<ProductApprovedNotificationHandler> logger)
	{
		_pushNotificationService = pushNotificationService;
		_pushTokenRepository = pushTokenRepository;
		_logger = logger;
	}

	public async Task HandleAsync(ProductApprovedEvent productEvent)
	{
		_logger.LogInformation(
			"Processing ProductApprovedEvent notification. ProductId: {ProductId}, Title: {Title}, SellerId: {SellerId}",
			productEvent.ProductId,
			productEvent.Title,
			productEvent.SellerId);

		// Seller'in UserId'sini bul (IAM User ID)
		var sellerUserId = await _pushTokenRepository.GetSellerUserId(productEvent.SellerId);
		if (sellerUserId == null || sellerUserId == Guid.Empty)
		{
			_logger.LogWarning("Could not find UserId for SellerId: {SellerId}", productEvent.SellerId);
			return;
		}

		var title = "Ilaniniz onaylandi!";
		var body = $"'{productEvent.Title}' ilani onaylandi ve yayina alindi.";

		var data = new Dictionary<string, string>
		{
			{ "type", "product_approved" },
			{ "productId", productEvent.ProductId.ToString() },
			{ "slug", productEvent.Slug ?? "" }
		};

		// Push bildirim tercihi kontrolu
		if (!await _pushTokenRepository.IsPushEnabled(sellerUserId.Value))
		{
			_logger.LogInformation("Push notifications disabled for seller user {UserId}, skipping", sellerUserId.Value);
		}
		else
		{
			var tokens = await _pushTokenRepository.GetActiveTokensForUser(sellerUserId.Value);
			if (tokens.Count > 0)
			{
				var (successCount, invalidTokens) = await _pushNotificationService.SendPushWithCleanupAsync(tokens, title, body, data);
				_logger.LogInformation("Sent push notification to {SuccessCount}/{TotalCount} devices for seller user {UserId}",
					successCount, tokens.Count, sellerUserId.Value);

				if (invalidTokens.Count > 0)
					await _pushTokenRepository.RevokeTokens(invalidTokens);
			}
			else
			{
				_logger.LogInformation("No active push tokens found for seller user {UserId}", sellerUserId.Value);
			}
		}

		// In-app bildirim kaydet
		var actionData = JsonSerializer.Serialize(new
		{
			productId = productEvent.ProductId,
			slug = productEvent.Slug,
			title = productEvent.Title
		});

		await _pushTokenRepository.SaveNotification(
			sellerUserId.Value,
			title,
			body,
			NotificationType.ProductApproved,
			actionData);
	}
}
