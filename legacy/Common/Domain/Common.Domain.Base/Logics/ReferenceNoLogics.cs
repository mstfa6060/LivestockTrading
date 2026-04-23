using System.Net.NetworkInformation;

namespace Common.Definitions.Base.Logics;

public class ReferenceNoLogics
{
	public static string GetAdjustedReferenceNo(int number)
	{
		int length = 6;
		var numberAsString = number.ToString();
		for (int i = numberAsString.Length; i < length; i++)
		{
			numberAsString = "0" + numberAsString;
		}

		return numberAsString;
	}
}
