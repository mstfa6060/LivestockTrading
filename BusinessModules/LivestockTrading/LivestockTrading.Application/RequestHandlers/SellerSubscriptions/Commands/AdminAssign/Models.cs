namespace LivestockTrading.Application.RequestHandlers.SellerSubscriptions.Commands.AdminAssign;

// Admin/Moderator herhangi bir satıcıya odeme dogrulamasi olmadan plan atayabilir.
// Kullanim: harici odeme akisi olmayan kanallarda (kurumsal anlasma, manuel destek talebi)
// veya prod'da otomatik upgrade akisi tamamlanana kadar gecici cozum.
public class RequestModel : IRequestModel
{
	public Guid SellerId { get; set; }
	public Guid SubscriptionPlanId { get; set; }
	/// <summary>0 = Monthly, 1 = Yearly</summary>
	public int Period { get; set; }
	/// <summary>Opsiyonel admin notu (audit icin OriginalTransactionId'ye yazilir)</summary>
	public string Note { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public Guid SellerId { get; set; }
	public Guid SubscriptionPlanId { get; set; }
	public int Status { get; set; }
	public int Period { get; set; }
	public int Platform { get; set; }
	public DateTime StartedAt { get; set; }
	public DateTime ExpiresAt { get; set; }
	public bool AutoRenew { get; set; }
	public DateTime CreatedAt { get; set; }
}
