using System.Text.RegularExpressions;

namespace Server.Helper;

public static class TimeConverter
{
	public static int ConvertToSeconds(int value, string unit)
	{
		if (string.IsNullOrWhiteSpace(unit))
		{
			return value * 60;
		}
		string unitLower = unit.ToLower().Trim();
		if (unitLower.Contains("hour"))
		{
			return value * 3600;
		}
		if (unitLower.Contains("second"))
		{
			return value;
		}
		return value * 60;
	}

	public static (int value, string unit) ParseLegacyFormat(string taskClient)
	{
		if (string.IsNullOrWhiteSpace(taskClient))
		{
			return (value: 0, unit: "Minutes");
		}
		Match match = Regex.Match(taskClient, "(\\d+)\\s*(hour|minute|second)", RegexOptions.IgnoreCase);
		if (match.Success)
		{
			int item = int.Parse(match.Groups[1].Value);
			string unitStr = match.Groups[2].Value.ToLower();
			string unit = "Minutes";
			if (unitStr.Contains("hour"))
			{
				unit = "Hours";
			}
			else if (unitStr.Contains("second"))
			{
				unit = "Seconds";
			}
			return (value: item, unit: unit);
		}
		return (value: 0, unit: "Minutes");
	}
}
