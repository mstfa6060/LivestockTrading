using Common.Services.Messaging;
using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Events;

namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Approve;

public class Handler : IRequestHandler
{
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

		var product = await _dataAccessLayer.GetProductWithDetails(request.Id);

		if (product == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		// Update product status to Active (approved)
		product.Status = ProductStatus.Active;
		product.PublishedAt = DateTime.UtcNow;
		product.UpdatedAt = DateTime.UtcNow;

		await _dataAccessLayer.SaveChanges();

		// Publish event for social media posting & notifications
		await _publisher.PublishFanout("livestocktrading.socialmedia.post", new ProductApprovedEvent
		{
			ProductId = product.Id,
			Title = product.Title,
			Description = product.Description,
			ShortDescription = product.ShortDescription,
			BasePrice = product.BasePrice,
			Currency = product.Currency,
			CategoryName = product.Category?.Name,
			BrandName = product.Brand?.Name,
			SellerBusinessName = product.Seller?.BusinessName,
			SellerId = product.SellerId,
			Slug = product.Slug,
			CountryCode = product.Location?.CountryCode,
			City = product.Location?.City,
			CoverImageUrl = product.CoverImageFileId,
			MediaBucketId = product.MediaBucketId
		});

		var response = mapper.MapToResponse(product);
		return ArfBlocksResults.Success(response);
	}
}
