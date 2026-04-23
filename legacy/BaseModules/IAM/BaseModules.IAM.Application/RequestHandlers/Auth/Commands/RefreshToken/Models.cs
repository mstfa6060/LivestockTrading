namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.RefreshToken;

public class RequestModel : IRequestModel
{
	public string RefreshToken { get; set; }
	public ClientPlatforms Platform { get; set; }
}

public class ResponseModel : IResponseModel
{
	public string Jwt { get; set; }
	public DateTime SessionExpirationDate { get; set; }
	public string RefreshToken { get; set; }
	public UserResponse User { get; set; }

	public class UserResponse
	{
		public Guid Id { get; set; }
		public string Username { get; set; }
		public string DisplayName { get; set; }
		public string Email { get; set; }
		public bool IsPhoneVerified { get; set; }
		public int CountryId { get; set; }
		public string CountryCode { get; set; }
		public string CountryName { get; set; }
		public string Language { get; set; }
		public string CurrencyCode { get; set; }
		public string CurrencySymbol { get; set; }
	}
}

