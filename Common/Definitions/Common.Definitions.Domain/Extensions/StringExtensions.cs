namespace Common.Definitions.Domain.Extentions;

public static class StringExtensions
{
	public static string ToLowerCase(this string value)
	{
		if (string.IsNullOrEmpty(value))
			return value;

		return value
				.Trim()
				.Replace("I", "ı")
				.Replace("İ", "i")
				.Replace("Ö", "ö")
				.Replace("Ü", "ü")
				.Replace("Ç", "ç")
				.Replace("Ş", "ş")
				.Replace("Ğ", "ğ");
	}
}