using System.Text.Json;
using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Events;
using LivestockTrading.Workers.NotificationSender.Services;

namespace LivestockTrading.Workers.NotificationSender.EventHandlers;

public class TransporterCreatedNotificationHandler
{
	private readonly IPushNotificationService _pushNotificationService;
	private readonly PushTokenRepository _pushTokenRepository;
	private readonly ILogger<TransporterCreatedNotificationHandler> _logger;

	public TransporterCreatedNotificationHandler(
		IPushNotificationService pushNotificationService,
		PushTokenRepository pushTokenRepository,
		ILogger<TransporterCreatedNotificationHandler> logger)
	{
		_pushNotificationService = pushNotificationService;
		_pushTokenRepository = pushTokenRepository;
		_logger = logger;
	}

	public async Task HandleAsync(TransporterCreatedEvent transporterEvent)
	{
		_logger.LogInformation(
			"Processing TransporterCreatedEvent notification. TransporterId: {TransporterId}, CompanyName: {CompanyName}, TargetAdmins: {AdminCount}",
			transporterEvent.TransporterId,
			transporterEvent.CompanyName,
			transporterEvent.TargetAdminUserIds?.Count ?? 0);

		var title = "Yeni nakliyeci onay bekliyor";
		var body = $"Yeni nakliyeci profili: {transporterEvent.CompanyName}";

		var data = new Dictionary<string, string>
		{
			{ "type", "transporter_pending_verification" },
			{ "transporterId", transporterEvent.TransporterId.ToString() },
			{ "companyName", transporterEvent.CompanyName ?? "" }
		};

		var adminUserIds = transporterEvent.TargetAdminUserIds ?? new List<Guid>();
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

				var actionData = JsonSerializer.Serialize(new
				{
					transporterId = transporterEvent.TransporterId,
					companyName = transporterEvent.CompanyName,
					userId = transporterEvent.UserId
				});

				await _pushTokenRepository.SaveNotification(
					adminUserId,
					title,
					body,
					NotificationType.TransporterPendingVerification,
					actionData);
			}
		}
		else
		{
			_logger.LogWarning("No target admin user IDs provided for TransporterCreatedEvent. TransporterId: {TransporterId}", transporterEvent.TransporterId);
		}
	}
}
