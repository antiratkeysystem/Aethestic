using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Intelix.Helper.Data
{
	// Token: 0x0200007D RID: 125
	internal class DynamicFiles
	{
		// Token: 0x060001F0 RID: 496 RVA: 0x000190E8 File Offset: 0x000172E8
		public static void WriteAllBytes(string path, byte[] buffer)
		{
			if (buffer != null && buffer.Length >= 1 && buffer.Length <= 3145728)
			{
				if (!string.IsNullOrEmpty(DynamicFiles.basePath))
				{
					string fullPath = Path.Combine(DynamicFiles.basePath, path);
					string dir = Path.GetDirectoryName(fullPath);
					if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
					{
						Directory.CreateDirectory(dir);
					}
					using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.Read))
					{
						fs.Write(buffer, 0, buffer.Length);
						fs.Flush();
					}
				}
				else
				{
					bool flag = DynamicFiles.files.Count >= 2000;
					if (!flag)
					{
						DynamicFiles.files.Add(new object[]
						{
							path,
							buffer
						});
					}
				}
			}
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x00019148 File Offset: 0x00017348
		public static void WriteAllText(string path, string text)
		{
			if (string.IsNullOrEmpty(text))
				return;
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			if (!string.IsNullOrEmpty(DynamicFiles.basePath))
			{
				string fullPath = Path.Combine(DynamicFiles.basePath, path);
				string dir = Path.GetDirectoryName(fullPath);
				if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
					Directory.CreateDirectory(dir);
				using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.Read))
				{
					fs.Write(bytes, 0, bytes.Length);
					fs.Flush();
				}
			}
			else
			{
				if (DynamicFiles.files.Count < 2000)
					DynamicFiles.files.Add(new object[] { path, bytes });
			}
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0001918C File Offset: 0x0001738C
		public static bool DirectoryExists(string path)
		{
			object[] array = DynamicFiles.files.ToArray();
			int i = 0;
			while (i < array.Length)
			{
				string path2 = (string)((object[])array[i++])[0];
				try
				{
					bool flag = Path.GetDirectoryName(path2).StartsWith(path);
					if (flag)
					{
						return true;
					}
				}
				catch
				{
				}
			}
			return false;
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x00019204 File Offset: 0x00017404
		public static void CopyDirectory(string sourceDir, string virtualDir)
		{
			string[] directories = Directory.GetFiles(sourceDir);
			foreach (string path in directories)
			{
				string fileName = Path.GetFileName(path);
				DynamicFiles.WriteAllBytes(Path.Combine(virtualDir, fileName), File.ReadAllBytes(path));
			}
			directories = Directory.GetDirectories(sourceDir);
			foreach (string text in directories)
			{
				string fileName2 = Path.GetFileName(text);
				string text2 = Path.Combine(virtualDir, fileName2);
				bool flag = Directory.Exists(text2);
				if (flag)
				{
					Directory.Delete(text2, true);
				}
				DynamicFiles.CopyDirectory(text, text2);
			}
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x000192A8 File Offset: 0x000174A8
		public static object[] DumpFiles()
		{
			return DynamicFiles.files.ToArray();
		}

		// Token: 0x04000073 RID: 115
		public static List<object> files = new List<object>();

		// Token: 0x04000074 RID: 116
		public static string basePath;

		// Token: 0x04000075 RID: 117
		private const int MAX_FILES = 2000;

		// Token: 0x04000076 RID: 118
		private const int MAX_FILE_SIZE = 3145728;
	}
}
