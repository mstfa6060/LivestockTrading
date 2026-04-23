namespace Common.Services.FileOperations.Constant;

public class TemplateConstant
{
    public const string OLUR = "OLUR";
    public const string RESMI = "RESMI";
    public const string X_Creator_Organization = "X-Creator-Organization";

    public const string ESIGNED_LABEL = "ESIGNED_LABEL";
    public const string ACCESS_CODE = "ACCESS_CODE";
    public const string AUTHOR_NAMESURNAME = "AUTHOR_NAMESURNAME";
    public const string AUTHOR_TITLE = "AUTHOR_TITLE";
    public const string VALIDATION_URL = "VALIDATION_URL";

    public const string DOCUMENT_NO = "DOCUMENT_NO";
    public const string SUBJECT = "SUBJECT";
    public const string PRIORITY = "PRIORITY";
    public const string DOCUMENT_DATE = "DOCUMENT_DATE";

    public const string INTERLOCUTOR = "INTERLOCUTOR";

    public const string REFERENCE = "REFERENCE";
    public const string WRITING = "WRITING";

    public const string CORRESPONDANCE_START = "CORRESPONDANCE_START";
    public const string CORRESPONDANCE_END = "CORRESPONDANCE_END";
    public const string APPROVE_START = "APPROVE_START";
    public const string APPROVE_END = "APPROVE_END";
    public const string APPROVER_1 = "APPROVER_1";
    public const string APPROVER_2 = "APPROVER_2";
    public const string APPROVER_3 = "APPROVER_3";

    public const string ATTACHMENT_LABEL = "ATTACHMENT_LABEL";
    public const string ATTACHMENT_START = "ATTACHMENT_START";
    public const string ATTACHMENT = "ATTACHMENT";
    public const string ATTACHMENT_END = "ATTACHMENT_END";

    public const string DISTRIBUTION_LABEL = "DISTRIBUTION_LABEL";
    public const string DISTRIBUTION_START = "DISTRIBUTION_START";
    public const string DISTRIBUTION_PROPERLY = "DISTRIBUTION_PROPERLY";
    public const string DISTRIBUTION_INFORMATION = "DISTRIBUTION_INFORMATION";
    public const string DISTRIBUTION_END = "DISTRIBUTION_END";

    public const string STAR = "{*}";

    public static string GetSearchKey(string key)
    {
        return $"{{{key}}}";
    }
}