using Common.Definitions.Base.Enums;

namespace Common.Contracts.Queue.Models;

public class NotificationModelContract
{
	public Guid UserId { get; set; }
	public string DisplayName { get; set; }
	public NotificationModel Notification { get; set; }

	public class NotificationModel
	{
		public Guid Id { get; set; }
		public DateTime CreatedAt { get; set; }

		public NotificationStatusEnumContract Status { get; set; }
		public UserModules Module { get; set; }

		// public NotificationTypesEnumContract NotificationType { get; set; }

		public string Title { get; set; }
		public string Content { get; set; }
		public EmailTemplateData TemplateData { get; set; }

		public NotificationActionTypesEnumContract ActionType { get; set; }
		public string EntityKey { get; set; }
		public Guid PrimaryEntityId { get; set; }
		public Guid SecondaryEntityId { get; set; }
	}
}

public enum NotificationActionTypesEnumContract
{
	None = 0,
	Navigate = 1,
}

public class EmailTemplateData
{
	public string Module { get; set; }
	public string HeaderLegend { get; set; }
	public string CreatedAt { get; set; }

	public string DescriptionLegend { get; set; }
	public string Content { get; set; }
	public string NavigationUrl { get; set; }
	public string NavigationText { get; set; }

	public Dictionary<string, string> Data { get; set; }
}

// Attention: Update Enums according to their original references

public enum NotificationStatusEnumContract
{
	Unread = 0,
	Read = 1,
}

// public enum NotificationTypesEnumContract
// {
//     Mail = 0,
//     Sms = 1,
//     PushNotification = 2,
// }