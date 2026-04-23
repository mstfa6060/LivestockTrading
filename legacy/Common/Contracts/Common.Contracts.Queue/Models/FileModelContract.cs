namespace Common.Contracts.Queue.Models;

public class FileCreateModelContract
{
    public Guid UniqueId { get; set; }
    public Guid CompanyId { get; set; }

    // For EbysPdf
    public string FolderName { get; set; }
    public string ReferenceFileId { get; set; }
    public List<ReplacementItem> ReplacementData { get; set; }

    public string FileName { get; set; }
    public string Extention { get; set; }
    public byte[] Data { get; set; }
    public FileModelTypes Type { get; set; }
}

public class ReplacementItem
{
    public string Key { get; set; }
    public List<string> Values { get; set; }
    public ReplacementItemContentTypes ContentType { get; set; }
    public ReplacementItemValueTypes ValueType { get; set; }
    public string GetValue()
    {
        var value = string.Empty;
        if (Values.Count > 1)
        {
            this.Values.ForEach((reference) =>
            {
                value += $"{reference}\n";
            });
        }
        else
        {
            this.Values.ForEach((reference) =>
            {
                value += $"{reference}";
            });
        }

        // if (this.Values.Count == 1 && value.Length > 2)
        if (this.Values.Count > 1 && value.Length > 2)
        {
            value = value.Substring(0, value.Length - 1);
            // value = value.Replace("\n", "");
        }

        return value;
    }
}

public enum ReplacementItemContentTypes
{
    Text = 0,
    Html = 1,
}

public enum ReplacementItemValueTypes
{
    Single = 0,
    Multiple = 1,
}

public enum FileModelTypes
{
    UserImage = 0,
    GroupImage = 1,
    Upload = 2,
}