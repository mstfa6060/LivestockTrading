using LivestockTrading.Application.Services;

namespace LivestockTrading.Application.RequestHandlers.Categories.Commands.Create;

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

		// Çeviriler frontend'den gelmediyse otomatik üret
		if (string.IsNullOrWhiteSpace(request.NameTranslations) && !string.IsNullOrWhiteSpace(request.Name))
		{
			request.NameTranslations = await _translationService.TranslateToAllLanguages(request.Name);
		}
		else if (!string.IsNullOrWhiteSpace(request.NameTranslations))
		{
			request.NameTranslations = await _translationService.FillMissingTranslations(request.NameTranslations, request.Name);
		}

		if (string.IsNullOrWhiteSpace(request.DescriptionTranslations) && !string.IsNullOrWhiteSpace(request.Description))
		{
			request.DescriptionTranslations = await _translationService.TranslateToAllLanguages(request.Description);
		}
		else if (!string.IsNullOrWhiteSpace(request.DescriptionTranslations))
		{
			request.DescriptionTranslations = await _translationService.FillMissingTranslations(request.DescriptionTranslations, request.Description);
		}

		var entity = mapper.MapToEntity(request);

		await _dataAccessLayer.AddCategory(entity);

		var response = mapper.MapToResponse(entity);
		return ArfBlocksResults.Success(response);
	}
}
