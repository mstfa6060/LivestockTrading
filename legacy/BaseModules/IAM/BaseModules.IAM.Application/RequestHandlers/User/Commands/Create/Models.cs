using System.ComponentModel.DataAnnotations;

namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Create;

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public string UserName { get; set; }
	public string Email { get; set; }
	public string FirstName { get; set; }
	public string Surname { get; set; }
	public bool IsActive { get; set; }

	// Ülke ve Dil Tercihleri
	public int CountryId { get; set; }
	public string CountryCode { get; set; }
	public string CountryName { get; set; }
	public string Language { get; set; }

	// Para Birimi Tercihleri
	public string CurrencyCode { get; set; }
	public string CurrencySymbol { get; set; }
}



public class RequestModel : IRequestModel
{
	public string UserName { get; set; }
	public string FirstName { get; set; }
	public string Surname { get; set; }
	public string Email { get; set; }
	public string Password { get; set; } // Sadece Manual kayıtlar için zorunlu
	public string ProviderId { get; set; } // Google veya Apple ID'si
	public UserSources UserSource { get; set; } // RegistrationType yerine geçiyor
	public string Description { get; set; }
	public string PhoneNumber { get; set; }

	/// <summary>
	/// Kullanıcının ülkesi (zorunlu)
	/// IP'den otomatik tespit edilir, kullanıcı onaylar veya değiştirir
	/// </summary>
	[Required]
	public int CountryId { get; set; }

	/// <summary>
	/// Kullanıcının dil tercihi (zorunlu)
	/// Örn: "tr-TR", "en-US", "de-DE"
	/// </summary>
	[Required]
	[MaxLength(10)]
	public string Language { get; set; }

	/// <summary>
	/// Kullanıcının para birimi tercihi (opsiyonel)
	/// Null ise ülkenin varsayılan para birimi kullanılır
	/// Örn: "TRY", "USD", "EUR"
	/// </summary>
	[MaxLength(3)]
	public string PreferredCurrencyCode { get; set; }
}
