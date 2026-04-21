namespace LivestockTrading.Application.RequestHandlers.FAQs.Queries.All;

public class RequestModel : IRequestModel
{
	public XSorting Sorting { get; set; }
	public List<XFilterItem> Filters { get; set; }
	public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
	public Guid Id { get; set; }
	public string Question { get; set; }
	public string Answer { get; set; }
	public Guid? CategoryId { get; set; }
	public Guid? ProductId { get; set; }
	public int SortOrder { get; set; }
	public bool IsActive { get; set; }
	public string QuestionTranslations { get; set; }
	public string AnswerTranslations { get; set; }
	public DateTime CreatedAt { get; set; }
}
