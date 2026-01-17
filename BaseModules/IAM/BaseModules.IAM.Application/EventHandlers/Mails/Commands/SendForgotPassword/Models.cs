using Common.Contracts.Event.Models;
using Common.Contracts.PubSub.Models;

namespace BaseModules.IAM.Application.EventHandlers.Mails.Queries.SendForgotPassword;

public class RequestModel : EventModelContract, IRequestModel
{
	public string Email { get; set; }
}


public class ResponseModel : EventHandlerResponseModel, IResponseModel
{
	public List<ForgotUser> Users { get; set; }

	public class ForgotUser
	{
		public Guid Id { get; set; }
		public string Email { get; set; }
		public string DisplayName { get; set; }
		public string Token { get; set; }
	}
}
