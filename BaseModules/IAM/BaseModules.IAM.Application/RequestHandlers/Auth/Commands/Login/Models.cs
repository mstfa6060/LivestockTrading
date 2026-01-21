namespace BaseModules.IAM.Application.RequestHandlers.Auth.Commands.Login;

public class ResponseModel : IResponseModel
{
	public string Jwt { get; set; }
	public string RefreshToken { get; set; }
	public DateTime SessionExpirationDate { get; set; }
	public UserResponse User { get; set; }

	public class UserResponse
	{
		public Guid Id { get; set; }
		public string Username { get; set; }
		public string DisplayName { get; set; }
		public string Email { get; set; }
		public bool IsPhoneVerified { get; set; }

		// Ülke ve Dil Tercihleri
		public int CountryId { get; set; }
		public string CountryCode { get; set; }
		public string CountryName { get; set; }
		public string Language { get; set; }

		// Para Birimi Tercihleri
		public string CurrencyCode { get; set; }
		public string CurrencySymbol { get; set; }
	}
}

public class RequestModel : IRequestModel
{
	public string Provider { get; set; } // native | google | itunes
	public string UserName { get; set; }
	public string Password { get; set; }
	public string Token { get; set; }
	public ClientPlatforms Platform { get; set; }

	// Google ile login başarısızsa oluşturulacak user için:
	public string FirstName { get; set; }
	public string Surname { get; set; }
	public string PhoneNumber { get; set; }
	public DateTime? BirthDate { get; set; }
	public string ExternalProviderUserId { get; set; } // Google "sub" değeri

}

