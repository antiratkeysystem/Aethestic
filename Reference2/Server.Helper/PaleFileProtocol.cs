using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Leb128;

namespace Server.Helper;

internal static class PaleFileProtocol
{
	private static readonly HashSet<string> KnownZipNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Intelix.zip", "Salsa.zip", "Stealer.zip" };

	private static bool UnpackZip(string basePath, byte[] zipBytes)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		if (zipBytes == null || zipBytes.Length < 22)
		{
			return false;
		}
		string baseNorm = Path.GetFullPath(basePath).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
		try
		{
			using (MemoryStream ms = new MemoryStream(zipBytes))
			{
				ZipArchive archive = new ZipArchive((Stream)ms, (ZipArchiveMode)0);
				try
				{
					foreach (ZipArchiveEntry entry in archive.Entries)
					{
						if (string.IsNullOrEmpty(entry.Name))
						{
							continue;
						}
						string[] parts = entry.FullName.Replace('/', Path.DirectorySeparatorChar).TrimEnd(Path.DirectorySeparatorChar).Split(new char[1] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
						if (parts.Length == 0)
						{
							continue;
						}
						List<string> safeParts = new List<string>();
						bool invalid = false;
						string[] array = parts;
						foreach (string part in array)
						{
							if (part == "." || part == ".." || part.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || part.IndexOfAny(new char[7] { ':', '*', '?', '"', '<', '>', '|' }) >= 0)
							{
								invalid = true;
								break;
							}
							safeParts.Add(part);
						}
						if (invalid || safeParts.Count == 0)
						{
							continue;
						}
						string fullPath = Path.GetFullPath(Path.Combine(baseNorm, Path.Combine(safeParts.ToArray())));
						if (!fullPath.StartsWith(baseNorm, StringComparison.OrdinalIgnoreCase))
						{
							continue;
						}
						try
						{
							string dir = Path.GetDirectoryName(fullPath);
							if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
							{
								Directory.CreateDirectory(dir);
							}
							using Stream src = entry.Open();
							using FileStream dst = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.None);
							src.CopyTo(dst);
						}
						catch
						{
						}
					}
				}
				finally
				{
					((IDisposable)archive)?.Dispose();
				}
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	private static string GetSafeRelativePath(string basePath, string fullPath)
	{
		string normalizedBase = Path.GetFullPath(basePath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
		string fullPath2 = Path.GetFullPath(fullPath);
		if (!fullPath2.StartsWith(normalizedBase, StringComparison.OrdinalIgnoreCase))
		{
			throw new UnauthorizedAccessException();
		}
		string text = fullPath2.Substring(normalizedBase.Length);
		if (text.IndexOf("..", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			throw new UnauthorizedAccessException();
		}
		return text;
	}

	public static bool Unpack(string basePath, byte[] buff)
	{
		if (string.IsNullOrWhiteSpace(basePath) || buff == null || buff.Length == 0)
		{
			return false;
		}
		basePath = Path.GetFullPath(basePath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
		if (!Directory.Exists(basePath))
		{
			Directory.CreateDirectory(basePath);
		}
		object[] array;
		try
		{
			array = LEB128.Read(buff);
		}
		catch
		{
			return false;
		}
		if (array == null)
		{
			return false;
		}
		if (array.Length == 2)
		{
			string name = array[0] as string;
			byte[] zipBytes = array[1] as byte[];
			if (!string.IsNullOrWhiteSpace(name) && zipBytes != null && KnownZipNames.Contains(name))
			{
				return UnpackZip(basePath, zipBytes);
			}
		}
		if (array.Length % 2 != 0)
		{
			return false;
		}
		for (int i = 0; i < array.Length; i += 2)
		{
			string relativePath = array[i] as string;
			byte[] fileData = array[i + 1] as byte[];
			if (string.IsNullOrWhiteSpace(relativePath) || fileData == null)
			{
				continue;
			}
			relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).Trim(Path.DirectorySeparatorChar);
			string[] parts = relativePath.Split(new char[1] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length == 0)
			{
				continue;
			}
			List<string> safeParts = new List<string>();
			string[] array2 = parts;
			int num = 0;
			while (true)
			{
				if (num < array2.Length)
				{
					string part = array2[num];
					if (string.Equals(part, ".", StringComparison.OrdinalIgnoreCase) || string.Equals(part, "..", StringComparison.OrdinalIgnoreCase) || part.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || part.IndexOfAny(new char[7] { ':', '*', '?', '"', '<', '>', '|' }) >= 0 || part.Length == 0)
					{
						break;
					}
					safeParts.Add(part);
					num++;
					continue;
				}
				if (safeParts.Count == 0)
				{
					break;
				}
				string reconstructed = Path.Combine(safeParts.ToArray());
				string fullPath = Path.GetFullPath(Path.Combine(basePath, reconstructed));
				if (!fullPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
				{
					break;
				}
				try
				{
					string dir = Path.GetDirectoryName(fullPath);
					if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
					{
						Directory.CreateDirectory(dir);
					}
					using FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.None);
					fs.Write(fileData, 0, fileData.Length);
					fs.Flush();
				}
				catch (UnauthorizedAccessException)
				{
				}
				catch (PathTooLongException)
				{
				}
				catch (DirectoryNotFoundException)
				{
				}
				catch (IOException)
				{
				}
				break;
			}
		}
		return true;
	}

	public static byte[] Pack(string basePath)
	{
		if (string.IsNullOrWhiteSpace(basePath))
		{
			return Array.Empty<byte>();
		}
		basePath = Path.GetFullPath(basePath);
		if (!Directory.Exists(basePath))
		{
			return Array.Empty<byte>();
		}
		List<object> list = new List<object>();
		try
		{
			string[] files = Directory.GetFiles(basePath, "*.*", SearchOption.AllDirectories);
			foreach (string filePath in files)
			{
				try
				{
					string relativePath = GetSafeRelativePath(basePath, filePath);
					byte[] fileBytes = File.ReadAllBytes(filePath);
					list.Add(relativePath);
					list.Add(fileBytes);
				}
				catch
				{
				}
			}
		}
		catch
		{
			return Array.Empty<byte>();
		}
		try
		{
			return LEB128.Write(list.ToArray());
		}
		catch
		{
			return Array.Empty<byte>();
		}
	}
}
