using FluentValidation;
using LivestockTrading.Domain.Errors;
using Common.Services.ErrorCodeGenerator;
using Common.Services.Auth.CurrentUser;

namespace LivestockTrading.Application.RequestHandlers.ProductReports.Commands.Create;

public class Validator : IRequestValidator
{
	private readonly DataAccess _dataAccess;
	private readonly CurrentUserService _currentUserService;

	public Validator(ArfBlocksDependencyProvider dependencyProvider)
	{
		_dataAccess = dependencyProvider.GetInstance<DataAccess>();
		_currentUserService = dependencyProvider.GetInstance<CurrentUserService>();
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
		var currentUserId = _currentUserService.GetCurrentUserId();

		// Check duplicate report
		var isDuplicate = await _dataAccess.CheckDuplicate(request.ProductId, currentUserId);
		if (isDuplicate)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductReportErrors.ProductReportAlreadyExists));
	}
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.ProductId)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductReportErrors.ProductReportProductIdRequired));

		RuleFor(x => x.Reason)
			.InclusiveBetween(0, 5)
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductReportErrors.ProductReportReasonRequired));

		RuleFor(x => x.Description)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductReportErrors.ProductReportDescriptionRequired));
	}
}
