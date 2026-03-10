using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.NotificationSender.Services;

namespace LivestockTrading.Workers.NotificationSender.EventHandlers;

public class ProductCreatedNotificationHandler
{
	private readonly IPushNotificationService _pushNotificationService;
	private readonly ILogger<ProductCreatedNotificationHandler> _logger;

	public ProductCreatedNotificationHandler(
		IPushNotificationService pushNotificationService,
		ILogger<ProductCreatedNotificationHandler> logger)
	{
		_pushNotificationService = pushNotificationService;
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

		_logger.LogInformation(
			"Push notification prepared - Title: {Title}, Body: {Body}, TargetAdminUserIds: {AdminIds}",
			title,
			body,
			string.Join(",", productEvent.TargetAdminUserIds ?? new List<Guid>()));

		// TODO: Fetch admin push tokens from database and send
		// When push token integration is complete, uncomment:
		// foreach (var adminUserId in productEvent.TargetAdminUserIds ?? new List<Guid>())
		// {
		//     var tokens = await GetUserPushTokens(adminUserId);
		//     if (tokens.Any())
		//     {
		//         var successCount = await _pushNotificationService.SendPushToMultipleAsync(tokens, title, body, data);
		//         _logger.LogInformation("Sent push to {Count} devices for admin {UserId}", successCount, adminUserId);
		//     }
		// }

		await Task.CompletedTask;
	}
}
