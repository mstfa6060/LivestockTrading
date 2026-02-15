namespace LivestockTrading.Application.RequestHandlers.Messages.Queries.UnreadCount;

public class RequestModel : IRequestModel
{
	/// <summary>
	/// Okunmamış mesaj sayısını almak istenen kullanıcının ID'si
	/// </summary>
	public Guid UserId { get; set; }
}

public class ResponseModel : IResponseModel
{
	/// <summary>
	/// Toplam okunmamış mesaj sayısı
	/// </summary>
	public int TotalUnreadCount { get; set; }

	/// <summary>
	/// Konuşma bazında okunmamış mesaj sayıları
	/// </summary>
	public List<ConversationUnreadItem> Conversations { get; set; }
}

public class ConversationUnreadItem
{
	public Guid ConversationId { get; set; }
	public int UnreadCount { get; set; }

	/// <summary>
	/// Son mesajın ilk 100 karakteri
	/// </summary>
	public string LastMessage { get; set; }

	public DateTime? LastMessageAt { get; set; }

	/// <summary>
	/// Son okunmamış mesajı gönderen kullanıcının ID'si
	/// </summary>
	public Guid SenderUserId { get; set; }

	/// <summary>
	/// Gönderenin görünen adı (frontend user cache'den çözümler)
	/// </summary>
	public string SenderDisplayName { get; set; }
}
