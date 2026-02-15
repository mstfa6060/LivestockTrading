namespace LivestockTrading.Application.RequestHandlers.Conversations.Commands.StartWithProduct;

public class Validator : IRequestValidator
{
	public Validator(ArfBlocksDependencyProvider dependencyProvider)
	{
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
		await Task.CompletedTask;
	}
}

public class RequestModel_Validator : AbstractValidator<RequestModel>
{
	public RequestModel_Validator()
	{
		RuleFor(x => x.ProductId)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ProductErrors.ProductNotFound));

		RuleFor(x => x.SellerId)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.SellerErrors.SellerUserIdRequired));

		RuleFor(x => x.BuyerUserId)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.ConversationErrors.ConversationParticipantsRequired));

		RuleFor(x => x.InitialMessage)
			.NotEmpty()
			.WithMessage(ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.MessageErrors.MessageContentRequired));
	}
}
