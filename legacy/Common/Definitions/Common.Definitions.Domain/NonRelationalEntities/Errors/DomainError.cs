namespace Common.Definitions.Domain.NonRelational.Errors;

public class DomainErrors
{
	public class CommonErrors
	{
		// Model Validation

		// Database Validation
		public static string UserIdNotValid { get; set; } = "";
		public static string BucketIdNotValid { get; set; } = "";
		public static string FileEntryIdNotValid { get; set; } = "";
		public static string FilePayloadNotFound { get; set; } = "";
		public static string FilePayloadIsEmpty { get; set; } = "";
	}

	public class BucketErrors
	{
		// Model Validation
		public static string BucketIdIsNull { get; set; } = "";
		public static string ChangeIdIsNull { get; set; } = "";

		// Database Validation
		public static string FileBucketNotExist { get; set; } = "";
		public static string FileEntryNotExist { get; set; } = "";
		public static string BuckedIdIsEmptyString { get; set; } = "";
		public static string InvalidaFileExtension { get; set; } = "";
	}
}