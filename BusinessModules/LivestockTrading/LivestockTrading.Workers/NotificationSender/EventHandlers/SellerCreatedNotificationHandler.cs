using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.NotificationSender.Services;

namespace LivestockTrading.Workers.NotificationSender.EventHandlers;

public class SellerCreatedNotificationHandler
{
	private readonly IPushNotificationService _pushNotificationService;
	private readonly PushTokenRepository _pushTokenRepository;
	private readonly ILogger<SellerCreatedNotificationHandler> _logger;

	public SellerCreatedNotificationHandler(
		IPushNotificationService pushNotificationService,
		PushTokenRepository pushTokenRepository,
		ILogger<SellerCreatedNotificationHandler> logger)
	{
		_pushNotificationService = pushNotificationService;
		_pushTokenRepository = pushTokenRepository;
		_logger = logger;
	}

	public async Task HandleAsync(SellerCreatedEvent sellerEvent)
	{
		_logger.LogInformation(
			"Processing SellerCreatedEvent notification. SellerId: {SellerId}, BusinessName: {BusinessName}, TargetAdmins: {AdminCount}",
			sellerEvent.SellerId,
			sellerEvent.BusinessName,
			sellerEvent.TargetAdminUserIds?.Count ?? 0);

		var title = "Yeni satıcı onay bekliyor";
		var body = $"Yeni satıcı profili: {sellerEvent.BusinessName}";

		var data = new Dictionary<string, string>
		{
			{ "type", "seller_pending_verification" },
			{ "sellerId", sellerEvent.SellerId.ToString() },
			{ "businessName", sellerEvent.BusinessName ?? "" }
		};

		var adminUserIds = sellerEvent.TargetAdminUserIds ?? new List<Guid>();
		if (adminUserIds.Count > 0)
		{
			var tokensByUser = await _pushTokenRepository.GetActiveTokensForUsers(adminUserIds);

			foreach (var adminUserId in adminUserIds)
			{
				if (!await _pushTokenRepository.ShouldSendPushNow(adminUserId))
					continue;

				if (tokensByUser.TryGetValue(adminUserId, out var tokens) && tokens.Count > 0)
				{
					var (successCount, invalidTokens) = await _pushNotificationService.SendPushWithCleanupAsync(tokens, title, body, data);
					_logger.LogInformation("Sent push to {SuccessCount}/{TotalCount} devices for admin {UserId}",
						successCount, tokens.Count, adminUserId);

					if (invalidTokens.Count > 0)
						await _pushTokenRepository.RevokeTokens(invalidTokens);
				}
			}
		}
		else
		{
			_logger.LogWarning("No target admin user IDs provided for SellerCreatedEvent. SellerId: {SellerId}", sellerEvent.SellerId);
		}
	}
}
