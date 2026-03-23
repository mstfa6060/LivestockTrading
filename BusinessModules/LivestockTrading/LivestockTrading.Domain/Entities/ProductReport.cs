namespace LivestockTrading.Domain.Entities;

public class ProductReport : BaseEntity
{
	public Guid ProductId { get; set; }
	public Guid ReporterUserId { get; set; }
	public int Reason { get; set; } // 0=Fake, 1=WrongPrice, 2=IllegalContent, 3=Spam, 4=Fraud, 5=Other
	public string Description { get; set; }
	public int Status { get; set; } // 0=Pending, 1=Reviewed, 2=Resolved, 3=Dismissed
	public string AdminNote { get; set; }
	public Guid? ReviewedByUserId { get; set; }
	public DateTime? ReviewedAt { get; set; }
}
