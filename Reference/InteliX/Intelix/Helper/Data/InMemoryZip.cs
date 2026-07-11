using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Intelix.Helper.Data
{
	// Token: 0x0200007E RID: 126
	public sealed class InMemoryZip : IDisposable
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060001F7 RID: 503 RVA: 0x000192D9 File Offset: 0x000174D9
		public int Count
		{
			get
			{
				return this._entries.Count;
			}
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x000192E8 File Offset: 0x000174E8
		private static string NormalizeEntryName(string name)
		{
			bool flag = string.IsNullOrWhiteSpace(name);
			if (flag)
			{
				throw new ArgumentException("Entry name is null or empty", "name");
			}
			name = name.Replace('\\', '/').Trim(new char[]
			{
				'/'
			});
			bool flag2 = name.Length != 0;
			if (flag2)
			{
				return name;
			}
			throw new ArgumentException("Invalid entry name", "name");
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x00019350 File Offset: 0x00017550
		public void AddFile(string entryPath, byte[] content)
		{
			bool disposed = this._disposed;
			if (disposed)
			{
				throw new ObjectDisposedException("InMemoryZip");
			}
			bool flag = content != null && content.Length != 0;
			if (flag)
			{
				string key = InMemoryZip.NormalizeEntryName(entryPath);
				byte[] copy = new byte[content.Length];
				Buffer.BlockCopy(content, 0, copy, 0, content.Length);
				this._entries.AddOrUpdate(key, copy, (string text, byte[] old) => copy);
			}
		}

		// Token: 0x060001FA RID: 506 RVA: 0x000193D4 File Offset: 0x000175D4
		public void AddTextFile(string entryPath, string text)
		{
			bool flag = !string.IsNullOrEmpty(text);
			if (flag)
			{
				this.AddFile(entryPath, Encoding.UTF8.GetBytes(text));
			}
		}

		// Token: 0x060001FB RID: 507 RVA: 0x00019404 File Offset: 0x00017604
		public void AddDirectoryFiles(string sourceDirectory, string targetEntryDirectory = "", bool recursive = true)
		{
			bool disposed = this._disposed;
			if (disposed)
			{
				throw new ObjectDisposedException("InMemoryZip");
			}
			bool flag = string.IsNullOrEmpty(sourceDirectory);
			if (flag)
			{
				throw new ArgumentException("sourceDirectory");
			}
			bool flag2 = !Directory.Exists(sourceDirectory);
			if (!flag2)
			{
				SearchOption searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
				string[] files = Directory.GetFiles(sourceDirectory, "*", searchOption);
				foreach (string text in files)
				{
					string text2 = text.Substring(sourceDirectory.Length).TrimStart(new char[]
					{
						Path.DirectorySeparatorChar,
						Path.AltDirectorySeparatorChar
					});
					string text3 = string.IsNullOrEmpty(targetEntryDirectory) ? text2 : Path.Combine(targetEntryDirectory, text2);
					text3 = text3.Replace('\\', '/');
					try
					{
						byte[] content = File.ReadAllBytes(text);
						this.AddFile(text3, content);
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x060001FC RID: 508 RVA: 0x00019508 File Offset: 0x00017708
		public byte[] ToArray(CompressionLevel compression = CompressionLevel.Fastest)
		{
			bool disposed = this._disposed;
			if (disposed)
			{
				throw new ObjectDisposedException("InMemoryZip");
			}
			object buildLock = this._buildLock;
			byte[] result;
			lock (buildLock)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (ZipArchive zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true, Encoding.UTF8))
					{
						foreach (KeyValuePair<string, byte[]> keyValuePair in this._entries)
						{
							using (Stream stream = zipArchive.CreateEntry(keyValuePair.Key, compression).Open())
							{
								byte[] value = keyValuePair.Value;
								stream.Write(value, 0, value.Length);
							}
						}
					}
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		// Token: 0x060001FD RID: 509 RVA: 0x00019638 File Offset: 0x00017838
		public void Clear()
		{
			this._entries.Clear();
		}

		// Token: 0x060001FE RID: 510 RVA: 0x00019648 File Offset: 0x00017848
		public void Dispose()
		{
			bool flag = !this._disposed;
			if (flag)
			{
				this._disposed = true;
				this._entries.Clear();
			}
		}

		// Token: 0x04000076 RID: 118
		private readonly ConcurrentDictionary<string, byte[]> _entries = new ConcurrentDictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x04000077 RID: 119
		private readonly object _buildLock = new object();

		// Token: 0x04000078 RID: 120
		private bool _disposed;
	}
}
