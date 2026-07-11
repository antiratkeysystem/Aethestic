using System;
using System.Collections.Generic;
using System.IO;
using Leb128;

namespace Plugin.Helper
{
	internal class PaleFileProtocol
	{
		public static byte[] Pack(string basePath)
		{
			if (string.IsNullOrWhiteSpace(basePath) || !Directory.Exists(basePath))
				return new byte[0];
			string baseNorm = Path.GetFullPath(basePath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
			var list = new List<object>();
			foreach (string fullPath in Directory.GetFiles(basePath, "*.*", SearchOption.AllDirectories))
			{
				try
				{
					string fullNorm = Path.GetFullPath(fullPath);
					if (!fullNorm.StartsWith(baseNorm, StringComparison.OrdinalIgnoreCase))
						continue;
					string relativePath = fullNorm.Substring(baseNorm.Length).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
					byte[] fileData = File.ReadAllBytes(fullPath);
					if (fileData.Length == 0)
						continue;
					list.Add(relativePath);
					list.Add(fileData);
				}
				catch { }
			}
			if (list.Count == 0)
				return new byte[0];
			return Leb128.Leb128.Write(list.ToArray());
		}
	}
}
