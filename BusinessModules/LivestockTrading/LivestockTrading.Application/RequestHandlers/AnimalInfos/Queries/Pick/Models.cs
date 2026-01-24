namespace LivestockTrading.Application.RequestHandlers.AnimalInfos.Queries.Pick;

public class RequestModel : IRequestModel
{
	public List<Guid> SelectedIds { get; set; }
	public string Keyword { get; set; }
	public int Limit { get; set; } = 10;
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public string BreedName { get; set; }
	public string TagNumber { get; set; }
}
