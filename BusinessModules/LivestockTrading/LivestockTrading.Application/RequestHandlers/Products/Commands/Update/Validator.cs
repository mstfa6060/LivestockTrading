using FluentValidation;
using LivestockTrading.Application.Authorization;
using LivestockTrading.Domain.Entities;
using LivestockTrading.Domain.Errors;
using LivestockTrading.Infrastructure.Services;
using Common.Services.ErrorCodeGenerator;

namespace LivestockTrading.Application.RequestHandlers.Products.Commands.Update;

public class Validator : IRequestValidator
{
	private readonly LivestockTradingModuleDbValidationService _dbValidator;
	private readonly PermissionService _permissionService;

	public Validator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dbValidator = dependencyProvider.GetInstance<LivestockTradingModuleDbValidationService>();
		_permissionService = dependencyProvider.GetInstance<PermissionService>();
	}

	public void ValidateRequestModel(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var result = new RequestModel_Validator().Validate(request);
		if (!result.IsValid)
			throw new ArfBlocksValidationException(result.ToString("~"));
	}

	public async Task ValidateDomain(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		await _dbValidator.ValidateProductExists(request.Id, cancellationToken);
		await _dbValidator.ValidateProductSlugUnique(request.Slug, request.Id, cancellationToken);

		// Sellers can only set status to Draft or PendingApproval.
		// Admin/Moderator can set any status (via Approve/Reject endpoints or directly).
		if (!_permissionService.IsModerator())
		{
			var requestedStatus = (ProductStatus)request.Status;
			var allowedStatuses = new[] { ProductStatus.Draft, ProductStatus.PendingApproval };
			if (!allowedStatuses.Contains(requestedStatus))
			{
				throw new ArfBlocksValidationException(
					ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductErrors.ProductSellerStatusTransitionNotAllowed));
			}
		}
	}
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		RuleFor(x => x.Title)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductErrors.ProductTitleRequired));

		RuleFor(x => x.Slug)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductErrors.ProductSlugRequired));
	}
}
