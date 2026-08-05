using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Server.Helper;

public static class SecurityHelper
{
	private static readonly Regex SafeHwidRegex = new Regex("^[a-zA-Z0-9_\\-]+$", RegexOptions.Compiled);

	public static string SanitizeHwid(string hwid)
	{
		if (string.IsNullOrEmpty(hwid))
		{
			return "Unknown";
		}
		string sanitized = new string(hwid.Where((char c) => char.IsLetterOrDigit(c) || c == '_' || c == '-').ToArray());
		if (string.IsNullOrEmpty(sanitized))
		{
			return "InvalidHWID";
		}
		if (sanitized.Length > 64)
		{
			sanitized = sanitized.Substring(0, 64);
		}
		return sanitized;
	}

	public static bool IsSafePath(string basePath, string relativePath)
	{
		try
		{
			string fullPath = Path.GetFullPath(Path.Combine(basePath, relativePath));
			string fullBasePath = Path.GetFullPath(basePath);
			return fullPath.StartsWith(fullBasePath, StringComparison.OrdinalIgnoreCase);
		}
		catch
		{
			return false;
		}
	}

	public static string SanitizeFilename(string filename)
	{
		if (string.IsNullOrEmpty(filename))
		{
			return "unknown_file";
		}
		char[] invalidChars = Path.GetInvalidFileNameChars();
		string sanitized = new string(filename.Where((char c) => !invalidChars.Contains(c)).ToArray());
		sanitized = sanitized.Replace("..", "").Replace("/", "").Replace("\\", "");
		if (string.IsNullOrEmpty(sanitized))
		{
			return "invalid_file";
		}
		return sanitized;
	}
}
