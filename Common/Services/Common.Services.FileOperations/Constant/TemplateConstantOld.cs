namespace Common.Services.FileOperations.Constant;

public class TemplateConstantOld
{
    public const string OLUR = "OLUR";
    public const string RESMI = "RESMI";
    public const string X_Creator_Organization = "X-Creator-Organization";

    public const string PRIORITY = "PRIORITY";
    public const string DOCUMENT_NUMBER = "DOCUMENT_NUMBER";
    public const string DOCUMENT_DATE = "DOCUMENT_DATE";
    public const string ESIGNED_LABEL = "ESIGNED_LABEL";
    public const string ACCESS_CODE = "ACCESS_CODE";
    public const string AUTHOR_NAMESURNAME = "AUTHOR_NAMESURNAME";
    public const string AUTHOR_TITLE = "AUTHOR_TITLE";
    public const string SUBJECT = "SUBJECT";
    public const string WRITING = "WRITING";
    public const string MUHATAP = "MUHATAP";
    public const string DOCREF = "DOCREF";
    public const string DOCADD = "DOCADD";
    public const string DOCADD_START = "DOCADD_START";
    public const string DOCADD_END = "DOCADD_END";
    public const string DOCADD_LABEL = "DOCADD_LABEL";
    public const string DLV = "DLV";
    public const string DLV_P = "DLV_P";
    public const string DLV_S = "DLV_S";
    public const string DLV_START = "DLV_START";
    public const string DLV_END = "DLV_END";
    public const string DLV_LABEL = "DLV_LABEL";
    public const string ONAY_A = "ONAY_A";
    public const string ONAY_B = "ONAY_B";
    public const string ONAY_C = "ONAY_C";
    public const string HE = "he";
    public const string HS = "hs";
    public const string STAR_START = "*_START";
    public const string STAR_END = "*_END";
    public const string STAR = "{*}";
    public const string H = "{h:";
    public const string NOT_H = "h:!";
    public const string VALIDATION_URL = "VALIDATION_URL";

    public static string GetSearchKey(string key)
    {
        return $"{{{key}}}";
    }
}