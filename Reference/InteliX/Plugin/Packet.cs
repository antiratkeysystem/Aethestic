using System;
using Leb128;
using Plugin.Helper;

namespace Plugin
{
	// Token: 0x02000080 RID: 128
	internal class Packet
	{
		// Token: 0x06000202 RID: 514 RVA: 0x00019DA8 File Offset: 0x00017FA8
		public static void Read(byte[] data)
		{
			try
			{
				object[] array = Leb128.Leb128.Read(data);
				string text = (string)array[0];
				string text2 = text;
				string a = text2;
				if (!(a == "Refresh"))
				{
				}
			}
			catch (Exception ex)
			{
				Client.Error(ex.ToString());
			}
		}
	}
}
