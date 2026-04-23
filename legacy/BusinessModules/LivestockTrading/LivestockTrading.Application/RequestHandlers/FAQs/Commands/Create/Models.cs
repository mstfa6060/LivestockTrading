namespace LivestockTrading.Application.RequestHandlers.FAQs.Commands.Create;

public class RequestModel : IRequestModel
{
	public string Question { get; set; }
	public string Answer { get; set; }
	public Guid? CategoryId { get; set; }
	public Guid? ProductId { get; set; }
	public int SortOrder { get; set; }
	public bool IsActive { get; set; } = true;
	public string QuestionTranslations { get; set; }
	public string AnswerTranslations { get; set; }
}

public class ResponseModel : IResponseModel
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
