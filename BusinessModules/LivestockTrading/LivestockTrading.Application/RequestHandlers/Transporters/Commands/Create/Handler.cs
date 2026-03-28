using System.Text.Json;
using Common.Definitions.Domain.Entities;
using Common.Services.Messaging;
using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Events;

namespace LivestockTrading.Application.RequestHandlers.Transporters.Commands.Create;

public class Handler : IRequestHandler
{
	// LivestockTrading Module ID (seed data'dan)
	private static readonly Guid LivestockTradingModuleId = Guid.Parse("DFD018C9-FC32-42C4-AEFD-70A5942A295E");
	// Transporter Role ID (roles.json'dan)
	private static readonly Guid TransporterRoleId = Guid.Parse("a1000000-0000-0000-0000-000000000005");

	private readonly DataAccess _dataAccessLayer;
	private readonly IRabbitMqPublisher _publisher;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_publisher = dependencyProvider.GetInstance<IRabbitMqPublisher>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		// Duplicate kontrolu
		var existing = await _dataAccessLayer.GetExistingByUserId(request.UserId, cancellationToken);
		if (existing != null)
		{
			return ArfBlocksResults.Success(mapper.MapToResponse(existing));
		}

		var entity = mapper.MapToEntity(request);
		await _dataAccessLayer.AddTransporter(entity);

		// Transporter rolunu ata (best-effort)
		try
		{
			var hasRole = await _dataAccessLayer.UserHasTransporterRole(
				request.UserId, LivestockTradingModuleId, TransporterRoleId);

			if (!hasRole)
			{
				var userRole = new UserRole
				{
					Id = Guid.NewGuid(),
					UserId = request.UserId,
					RoleId = TransporterRoleId,
					ModuleId = LivestockTradingModuleId,
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow,
					IsDeleted = false
				};
				await _dataAccessLayer.AddUserRole(userRole);
			}
		}
		catch (Exception) { }

		// Admin/moderator kullanıcılara bildirim gönder
		await NotifyAdmins(entity, cancellationToken);

		return ArfBlocksResults.Success(mapper.MapToResponse(entity));
	}

	private async Task NotifyAdmins(Transporter transporter, CancellationToken ct)
	{
		try
		{
			var adminUserIds = await _dataAccessLayer.GetAdminModeratorUserIds(ct);
			if (adminUserIds.Count == 0)
				return;

			// Create in-app notifications for each admin/moderator
			var notifications = adminUserIds.Select(userId => new Notification
			{
				UserId = userId,
				Title = "Yeni nakliyeci onay bekliyor",
				Message = $"Yeni nakliyeci profili oluşturuldu: {transporter.CompanyName}",
				Type = NotificationType.TransporterPendingVerification,
				ActionUrl = "/dashboard/transporter-moderation",
				ActionData = JsonSerializer.Serialize(new { TransporterId = transporter.Id, CompanyName = transporter.CompanyName })
			}).ToList();

			await _dataAccessLayer.CreateNotifications(notifications, ct);

			// Publish event for push notifications
			await _publisher.PublishFanout("livestocktrading.notification.push", new TransporterCreatedEvent
			{
				TransporterId = transporter.Id,
				UserId = transporter.UserId,
				CompanyName = transporter.CompanyName,
				ContactPerson = transporter.ContactPerson,
				Email = transporter.Email,
				Phone = transporter.Phone,
				TargetAdminUserIds = adminUserIds
			});
		}
		catch
		{
			// Notification failure should not block transporter creation
		}
	}
}
