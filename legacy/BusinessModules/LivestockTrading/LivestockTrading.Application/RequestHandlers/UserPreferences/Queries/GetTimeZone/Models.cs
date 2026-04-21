namespace LivestockTrading.Application.RequestHandlers.UserPreferences.Queries.GetTimeZone;

/// <summary>
/// Returns only the TimeZone preference for a given user id. Intended for
/// cross-user features like "other party local time in chat" where exposing
/// the full UserPreferences row would leak unrelated settings.
/// </summary>
public class RequestModel : IRequestModel
{
	public Guid UserId { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid UserId { get; set; }

	/// <summary>IANA timezone identifier (e.g. "Europe/Istanbul"). Empty when the user has no preference saved.</summary>
	public string TimeZone { get; set; }
}
