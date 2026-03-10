using System.Text.Json;
using Common.Services.Messaging;
using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Events;
using Microsoft.Extensions.Logging;

namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Create;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly CurrentUserService _currentUserService;
	private readonly IRabbitMqPublisher _publisher;
	private readonly ILogger<Handler> _logger;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
		_publisher = dependencyProvider.GetInstance<IRabbitMqPublisher>();
		_logger = dependencyProvider.GetInstance<ILogger<Handler>>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		// Get seller ID - auto-create if not provided
		var sellerId = await GetOrCreateSellerId(request.SellerId, cancellationToken);

		var entity = mapper.MapToEntity(request);
		entity.SellerId = sellerId;

		await _dataAccessLayer.AddProduct(entity);

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
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Failed to send admin notifications for product {ProductId}", product.Id);
		}
	}
}
