using System.ComponentModel.DataAnnotations;

namespace BaseModules.IAM.Application.RequestHandlers.Users.Commands.Update;

public class RequestModel : IRequestModel
{
	public Guid Id { get; set; }
	public string FirstName { get; set; }
	public string Surname { get; set; }

	[MaxLength(32)]
	public string PhoneNumber { get; set; }

	/// <summary>
	/// Kullanıcının ülkesi (FK -> Countries)
	/// </summary>
	public int? CountryId { get; set; }

	/// <summary>
	/// Kullanıcının dil tercihi (ISO 639-1, örn: "tr", "en", "de")
	/// </summary>
	[MaxLength(10)]
	public string Language { get; set; }

	/// <summary>
	/// Kullanıcının tercih ettiği para birimi kodu (ISO 4217, örn: "TRY", "USD", "EUR")
	/// Null ise ülkenin varsayılan para birimi kullanılır
	/// </summary>
	[MaxLength(3)]
	public string PreferredCurrencyCode { get; set; }

	/// <summary>
	/// Kullanıcının profil fotoğrafı BucketId (FileProvider)
	/// </summary>
	public string AvatarUrl { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid Id { get; set; }
	public string UserName { get; set; }
	public string Email { get; set; }
	public string FirstName { get; set; }
	public string Surname { get; set; }
	public string PhoneNumber { get; set; }
	public bool IsActive { get; set; }

	// Ülke ve Dil Tercihleri
	public int CountryId { get; set; }
	public string CountryCode { get; set; }
	public string CountryName { get; set; }
	public string Language { get; set; }

	// Para Birimi Tercihleri
	public string CurrencyCode { get; set; }
	public string CurrencySymbol { get; set; }

	// Avatar
	public string AvatarUrl { get; set; }
}
