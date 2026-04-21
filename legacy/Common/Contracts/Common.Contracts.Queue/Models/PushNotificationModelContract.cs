
using Common.Definitions.Base.Enums;

namespace Common.Contracts.Queue.Models;

public class PushNotificationModelContract
{
	public Guid NotificationId { get; set; }
	public string[] TargetDeviceTokens { get; set; }
	public string Title { get; set; }
	public string Body { get; set; }
	public bool IsContentHtml { get; set; }
	public DataModelContract Data { get; set; }

	public class DataModelContract
	{
		public Guid PrimaryId { get; set; }
		public Guid SecondaryId { get; set; }
		public string EntityKey { get; set; }
		public UserModules Module { get; set; }
		public NotificationActionTypesContract ActionType { get; set; }
	}
}

public enum NotificationActionTypesContract
{
	None = 0,
	Navigate = 1,
}