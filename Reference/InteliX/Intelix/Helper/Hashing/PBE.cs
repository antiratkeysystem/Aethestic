using System;
using System.Security.Cryptography;

namespace Intelix.Helper.Hashing
{
	// Token: 0x02000069 RID: 105
	public class PBE
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000188 RID: 392 RVA: 0x00013BF0 File Offset: 0x00011DF0
		private byte[] Ciphertext { get; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000189 RID: 393 RVA: 0x00013BF8 File Offset: 0x00011DF8
		private byte[] GlobalSalt { get; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600018A RID: 394 RVA: 0x00013C00 File Offset: 0x00011E00
		private byte[] MasterPass { get; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600018B RID: 395 RVA: 0x00013C08 File Offset: 0x00011E08
		private byte[] EntrySalt { get; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600018C RID: 396 RVA: 0x00013C10 File Offset: 0x00011E10
		private byte[] PartIv { get; }

		// Token: 0x0600018D RID: 397 RVA: 0x00013C18 File Offset: 0x00011E18
		public PBE(byte[] ciphertext, byte[] globalSalt, byte[] masterPassword, byte[] entrySalt, byte[] partIv)
		{
			this.Ciphertext = ciphertext;
			this.GlobalSalt = globalSalt;
			this.MasterPass = masterPassword;
			this.EntrySalt = entrySalt;
			this.PartIv = partIv;
		}

		// Token: 0x0600018E RID: 398 RVA: 0x00013C48 File Offset: 0x00011E48
		public byte[] Compute()
		{
			byte[] array = new byte[this.GlobalSalt.Length + this.MasterPass.Length];
			Buffer.BlockCopy(this.GlobalSalt, 0, array, 0, this.GlobalSalt.Length);
			Buffer.BlockCopy(this.MasterPass, 0, array, this.GlobalSalt.Length, this.MasterPass.Length);
			byte[] password = new SHA1Managed().ComputeHash(array);
			byte[] array2 = new byte[]
			{
				4,
				14
			};
			byte[] array3 = new byte[array2.Length + this.PartIv.Length];
			Buffer.BlockCopy(array2, 0, array3, 0, array2.Length);
			Buffer.BlockCopy(this.PartIv, 0, array3, array2.Length, this.PartIv.Length);
			byte[] bytes = new PBKDF2(new HMACSHA256(), password, this.EntrySalt, 1).GetBytes(32);
			return new AesManaged
			{
				Mode = CipherMode.CBC,
				BlockSize = 128,
				KeySize = 256,
				Padding = PaddingMode.Zeros
			}.CreateDecryptor(bytes, array3).TransformFinalBlock(this.Ciphertext, 0, this.Ciphertext.Length);
		}
	}
}
