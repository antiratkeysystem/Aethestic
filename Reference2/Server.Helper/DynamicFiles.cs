using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server.Helper;

internal class DynamicFiles
{
	public static void Save(string path, object[] Dynamicfls)
	{
		if (string.IsNullOrWhiteSpace(path) || Dynamicfls == null || Dynamicfls.Length == 0)
		{
			return;
		}
		try
		{
			string baseRoot = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
			char[] invalidFileChars = Path.GetInvalidFileNameChars();
			char[] invalidPathChars = Path.GetInvalidPathChars();
			for (int idx = 0; idx < Dynamicfls.Length; idx++)
			{
				if (!(Dynamicfls[idx] is object[] entry) || entry.Length < 2)
				{
					continue;
				}
				string name = entry[0] as string;
				byte[] bytes = entry[1] as byte[];
				if (string.IsNullOrWhiteSpace(name) || bytes == null || bytes.Length == 0)
				{
					continue;
				}
				name = name.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).Trim(Path.DirectorySeparatorChar);
				string[] parts = name.Split(new char[1] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length == 0)
				{
					continue;
				}
				List<string> safeParts = new List<string>();
				string[] array = parts;
				foreach (string part in array)
				{
					if (string.Equals(part, "..", StringComparison.OrdinalIgnoreCase) || string.Equals(part, ".", StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					StringBuilder cleaned = new StringBuilder();
					string text = part;
					foreach (char c in text)
					{
						bool isValid = true;
						char[] array2 = invalidFileChars;
						foreach (char invalid in array2)
						{
							if (c == invalid)
							{
								isValid = false;
								break;
							}
						}
						if (!isValid)
						{
							continue;
						}
						array2 = invalidPathChars;
						foreach (char invalid2 in array2)
						{
							if (c == invalid2)
							{
								isValid = false;
								break;
							}
						}
						if (isValid && c != Path.DirectorySeparatorChar && c != Path.AltDirectorySeparatorChar)
						{
							cleaned.Append(c);
						}
					}
					string cleanedPart = cleaned.ToString();
					if (!string.IsNullOrEmpty(cleanedPart))
					{
						safeParts.Add(cleanedPart);
					}
				}
				if (safeParts.Count == 0)
				{
					continue;
				}
				string safeRelativePath = Path.Combine(safeParts.ToArray());
				string fullPath = Path.GetFullPath(Path.Combine(baseRoot, safeRelativePath));
				if (fullPath.StartsWith(baseRoot, StringComparison.OrdinalIgnoreCase))
				{
					string directory = Path.GetDirectoryName(fullPath);
					if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
					{
						Directory.CreateDirectory(directory);
					}
					File.WriteAllBytes(fullPath, bytes);
				}
			}
		}
		catch
		{
		}
	}
}
