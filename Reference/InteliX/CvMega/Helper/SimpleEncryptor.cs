using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CvMega.Helper
{
	// Token: 0x02000087 RID: 135
	public class SimpleEncryptor
	{
		// Token: 0x06000222 RID: 546 RVA: 0x0001B1D0 File Offset: 0x000193D0
		public static string Encrypt(string data)
		{
			return HttpUtility.UrlEncode(Convert.ToBase64String(SimpleEncryptor.XorEncrypt(Encoding.UTF8.GetBytes(data))));
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0001B1FC File Offset: 0x000193FC
		public static string EncryptNonEnc(string data)
		{
			return Convert.ToBase64String(SimpleEncryptor.XorEncrypt(Encoding.UTF8.GetBytes(data)));
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0001B224 File Offset: 0x00019424
		public static string Decrypt(string data)
		{
			return Encoding.UTF8.GetString(SimpleEncryptor.XorEncrypt(Convert.FromBase64String(data)));
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0001B24C File Offset: 0x0001944C
		public static string Hash(string data)
		{
			string result;
			using (MD5 md = MD5.Create())
			{
				byte[] array = md.ComputeHash(Encoding.UTF8.GetBytes(data));
				StringBuilder stringBuilder = new StringBuilder();
				byte[] array2 = array;
				foreach (byte b in array2)
				{
					stringBuilder.Append(b.ToString("x2"));
				}
				result = HttpUtility.UrlEncode(stringBuilder.ToString().Substring(0, 10));
			}
			return result;
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0001B2E4 File Offset: 0x000194E4
		public static byte[] XorEncryptMyKey(byte[] data, byte key)
		{
			byte[] array = new byte[data.Length];
			for (int i = 0; i < data.Length; i++)
			{
				array[i] = (byte)(data[i] ^ key);
			}
			return array;
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0001B320 File Offset: 0x00019520
		public static byte[] XorEncrypt(byte[] dataBytes)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(SimpleEncryptor.secretKey);
			for (int i = 0; i < dataBytes.Length; i++)
			{
				int num = i;
				dataBytes[num] ^= bytes[i % bytes.Length];
			}
			return dataBytes;
		}

		// Token: 0x04000092 RID: 146
		public static readonly string secretKey = "cvls0";
	}
}
