namespace LivestockTrading.Application.RequestHandlers.Messages.Queries.UnreadCount;

/// <summary>
/// Okunmamış Mesaj Sayısı
/// Kullanıcının tüm konuşmalarındaki okunmamış mesaj sayılarını döner.
/// Toplam okunmamış mesaj sayısı ve konuşma bazında detay içerir.
/// </summary>
public class Handler : IRequestHandler
{
	private readonly DataAccess _dataAccessLayer;

	public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
	{
		_dataAccessLayer = (DataAccess)dataAccess;
	}

	public async Task<ArfBlocksRequestResult> Handle(IRequestModel payload, EndpointContext context, CancellationToken cancellationToken)
	{
		var req = (RequestModel)payload;

		var conversations = await _dataAccessLayer.GetUnreadCounts(req.UserId, cancellationToken);

		var response = new ResponseModel
		{
			TotalUnreadCount = conversations.Sum(c => c.UnreadCount),
			Conversations = conversations
		};

		return ArfBlocksResults.Success(response);
	}
}
