using Common.Contracts.Event.Models;
using Common.Contracts.PubSub.Models;

namespace BaseModules.IAM.Application.EventHandlers.Sms.Queries.SendOtpSms;

public class RequestModel : EventModelContract, IRequestModel
{
	public string PhoneNumber { get; set; }
	public string Message { get; set; }
}

public class ResponseModel : EventHandlerResponseModel, IResponseModel
{
	public List<OtpSmsUser> Users { get; set; }

	public class OtpSmsUser
	{
		public Guid Id { get; set; }
		public string PhoneNumber { get; set; }
		public string DisplayName { get; set; }
		public string Message { get; set; }
	}
}
