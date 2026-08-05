using System;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Server.Helper;

public class RegValueHelper
{
	private static string DEFAULT_REG_VALUE = "(Default)";

	private const int MaxBytesToProcess = 1048576;

	private const int MaxStringLength = 2048;

	public static bool IsDefaultValue(string valueName)
	{
		return string.IsNullOrEmpty(valueName);
	}

	public static string GetName(string valueName)
	{
		if (!IsDefaultValue(valueName))
		{
			return valueName;
		}
		return DEFAULT_REG_VALUE;
	}

	public static string RegistryValueToString(RegistrySeeker.RegValueData value)
	{
		if (value == null)
		{
			return string.Empty;
		}
		try
		{
			byte[] data = value.Data ?? Array.Empty<byte>();
			if (data.Length > 1048576)
			{
				data = data.Take(1048576).ToArray();
			}
			switch (value.Kind)
			{
			case RegistryValueKind.Binary:
				if (data.Length == 0)
				{
					return "(zero-length binary value)";
				}
				return BitConverter.ToString(data).Replace("-", " ").ToLowerInvariant();
			case RegistryValueKind.MultiString:
			{
				string[] arr = ByteConverter.ToStringArray(data) ?? Array.Empty<string>();
				for (int i = 0; i < arr.Length; i++)
				{
					arr[i] = SanitizeString(arr[i]);
				}
				return string.Join(" ", arr.Where((string s) => !string.IsNullOrEmpty(s)));
			}
			case RegistryValueKind.DWord:
			{
				if (data.Length < 4)
				{
					return string.Empty;
				}
				uint dword = ByteConverter.ToUInt32(data);
				return $"0x{dword:x8} ({dword})";
			}
			case RegistryValueKind.QWord:
			{
				if (data.Length < 8)
				{
					return string.Empty;
				}
				ulong qword = ByteConverter.ToUInt64(data);
				return $"0x{qword:x8} ({qword})";
			}
			case RegistryValueKind.String:
			case RegistryValueKind.ExpandString:
				return SanitizeString(ByteConverter.ToString(data));
			default:
				return string.Empty;
			}
		}
		catch
		{
			return string.Empty;
		}
	}

	private static string SanitizeString(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return string.Empty;
		}
		StringBuilder sb = new StringBuilder(input.Length);
		foreach (char c in input)
		{
			if (!char.IsControl(c) || c == '\r' || c == '\n' || c == '\t')
			{
				sb.Append(c);
			}
		}
		string result = sb.ToString();
		if (result.Length > 2048)
		{
			result = result.Substring(0, 2048);
		}
		return result;
	}
}
