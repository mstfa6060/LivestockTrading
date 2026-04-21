using LivestockTrading.Application.Services;

namespace LivestockTrading.Application.RequestHandlers.Categories.Commands.Update;

public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;
	private readonly AutoTranslationService _translationService;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
		_translationService = dependencyProvider.GetInstance<AutoTranslationService>();
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var request = (RequestModel)payload;
		var mapper = new Mapper();

		var category = await _dataAccessLayer.GetCategoryById(request.Id);

		if (category == null)
			throw new ArfBlocksValidationException(
				ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));

		// İsim değiştiyse çevirileri yeniden üret
		bool nameChanged = !string.IsNullOrWhiteSpace(request.Name) && request.Name != category.Name;
		bool descChanged = !string.IsNullOrWhiteSpace(request.Description) && request.Description != category.Description;

		if (nameChanged && string.IsNullOrWhiteSpace(request.NameTranslations))
		{
			request.NameTranslations = await _translationService.TranslateToAllLanguages(request.Name);
		}
		else if (!string.IsNullOrWhiteSpace(request.NameTranslations))
		{
			request.NameTranslations = await _translationService.FillMissingTranslations(request.NameTranslations, request.Name ?? category.Name);
		}

		if (descChanged && string.IsNullOrWhiteSpace(request.DescriptionTranslations))
		{
			request.DescriptionTranslations = await _translationService.TranslateToAllLanguages(request.Description);
		}
		else if (!string.IsNullOrWhiteSpace(request.DescriptionTranslations))
		{
			request.DescriptionTranslations = await _translationService.FillMissingTranslations(request.DescriptionTranslations, request.Description ?? category.Description);
		}

		mapper.MapToEntity(request, category);

		await _dataAccessLayer.SaveChanges();

		var response = mapper.MapToResponse(category);
		return ArfBlocksResults.Success(response);
	}
}
