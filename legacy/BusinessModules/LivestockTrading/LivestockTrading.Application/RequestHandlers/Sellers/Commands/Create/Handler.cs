using System.Text.Json;
using Common.Definitions.Domain.Entities;
using Common.Services.Messaging;
using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Events;

namespace LivestockTrading.Application.RequestHandlers.Sellers.Commands.Create;

public class Handler : IRequestHandler
{
	// LivestockTrading Module ID (seed data'dan)
	private static readonly Guid LivestockTradingModuleId = Guid.Parse("DFD018C9-FC32-42C4-AEFD-70A5942A295E");
	// Seller Role ID (roles.json'dan)
	private static readonly Guid SellerRoleId = Guid.Parse("a1000000-0000-0000-0000-000000000004");

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

		// Kullanıcının zaten seller profili varsa onu döndür (duplicate engelle)
		var existingSeller = await _dataAccessLayer.GetExistingSellerByUserId(request.UserId, cancellationToken);
		if (existingSeller != null)
		{
			var existingResponse = mapper.MapToResponse(existingSeller);
			return ArfBlocksResults.Success(existingResponse);
		}

		var entity = mapper.MapToEntity(request);

		// Satıcı profilini oluştur
		await _dataAccessLayer.AddSeller(entity);

		// Kullanıcıya Seller rolünü ata (best-effort, satıcı oluşturmayı engellemez)
		try
		{
			var hasSellerRole = await _dataAccessLayer.UserHasSellerRole(
				request.UserId,
				LivestockTradingModuleId,
				SellerRoleId);

			if (!hasSellerRole)
			{
				var userRole = new UserRole
				{
					Id = Guid.NewGuid(),
					UserId = request.UserId,
					RoleId = SellerRoleId,
					ModuleId = LivestockTradingModuleId,
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow,
					IsDeleted = false
				};
				await _dataAccessLayer.AddUserRole(userRole);
			}
		}
		catch (Exception)
		{
			// Rol ataması başarısız olsa da satıcı profili oluşturulmuş durumda
		}

		// Admin/moderator kullanıcılara bildirim gönder
		await NotifyAdmins(entity, cancellationToken);

		var response = mapper.MapToResponse(entity);
		return ArfBlocksResults.Success(response);
	}

	private async Task NotifyAdmins(Seller seller, CancellationToken ct)
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
				Title = "Yeni satıcı onay bekliyor",
				Message = $"Yeni satıcı profili oluşturuldu: {seller.BusinessName}",
				Type = NotificationType.SellerPendingVerification,
				ActionUrl = "/dashboard/seller-moderation",
				ActionData = JsonSerializer.Serialize(new { SellerId = seller.Id, BusinessName = seller.BusinessName })
			}).ToList();

			await _dataAccessLayer.CreateNotifications(notifications, ct);

			// Publish event for push notifications
			await _publisher.PublishFanout("livestocktrading.notification.push", new SellerCreatedEvent
			{
				SellerId = seller.Id,
				UserId = seller.UserId,
				BusinessName = seller.BusinessName,
				BusinessType = seller.BusinessType,
				Email = seller.Email,
				Phone = seller.Phone,
				TargetAdminUserIds = adminUserIds
			});
		}
		catch
		{
			// Notification failure should not block seller creation
		}
	}
}
