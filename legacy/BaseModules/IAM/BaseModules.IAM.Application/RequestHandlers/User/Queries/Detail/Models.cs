namespace BaseModules.IAM.Application.RequestHandlers.Users.Queries.Detail;

public class RequestModel : IRequestModel
{
	public Guid UserId { get; set; }
}

public class ResponseModel : IResponseModel
{
	public Guid UserId { get; set; }
	public string UserName { get; set; }
	public string Email { get; set; }
	public string FullName { get; set; }
	public string Language { get; set; }
	public bool IsActive { get; set; }
	public bool IsAvailable { get; set; }
	public string PhoneNumber { get; set; }

	public DateTime? BirthDate { get; set; }
	public string City { get; set; }
	public string District { get; set; }
	public bool IsPhoneVerified { get; set; }

}
