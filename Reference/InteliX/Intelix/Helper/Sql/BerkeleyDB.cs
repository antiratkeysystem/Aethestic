using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Intelix.Helper.Sql
{
	// Token: 0x02000067 RID: 103
	public class BerkeleyDB
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000179 RID: 377 RVA: 0x0001293D File Offset: 0x00010B3D
		public List<KeyValuePair<string, string>> Keys { get; }

		// Token: 0x0600017A RID: 378 RVA: 0x00012948 File Offset: 0x00010B48
		public BerkeleyDB(byte[] file)
		{
			List<byte> list = new List<byte>();
			this.Keys = new List<KeyValuePair<string, string>>();
			using (MemoryStream memoryStream = new MemoryStream(file))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					int i = 0;
					int num = (int)binaryReader.BaseStream.Length;
					while (i < num)
					{
						list.Add(binaryReader.ReadByte());
						i++;
					}
				}
			}
			string text = BitConverter.ToString(BerkeleyDB.Extract(list.ToArray(), 0, 4, false)).Replace("-", "");
			int num2 = BitConverter.ToInt32(BerkeleyDB.Extract(list.ToArray(), 12, 4, true), 0);
			bool flag = !text.Equals("00061561");
			if (!flag)
			{
				int num3 = int.Parse(BitConverter.ToString(BerkeleyDB.Extract(list.ToArray(), 56, 4, false)).Replace("-", ""));
				int num4 = 1;
				while (this.Keys.Count < num3)
				{
					string[] array = new string[(num3 - this.Keys.Count) * 2];
					for (int j = 0; j < (num3 - this.Keys.Count) * 2; j++)
					{
						array[j] = BitConverter.ToString(BerkeleyDB.Extract(list.ToArray(), num2 * num4 + 2 + j * 2, 2, true)).Replace("-", "");
					}
					Array.Sort<string>(array);
					for (int k = 0; k < array.Length; k += 2)
					{
						int num5 = Convert.ToInt32(array[k], 16) + num2 * num4;
						int num6 = Convert.ToInt32(array[k + 1], 16) + num2 * num4;
						int num7 = (k + 2 >= array.Length) ? (num2 + num2 * num4) : (Convert.ToInt32(array[k + 2], 16) + num2 * num4);
						string @string = Encoding.ASCII.GetString(BerkeleyDB.Extract(list.ToArray(), num6, num7 - num6, false));
						string value = BitConverter.ToString(BerkeleyDB.Extract(list.ToArray(), num5, num6 - num5, false));
						bool flag2 = !string.IsNullOrWhiteSpace(@string);
						if (flag2)
						{
							this.Keys.Add(new KeyValuePair<string, string>(@string, value));
						}
					}
					num4++;
				}
			}
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00012BDC File Offset: 0x00010DDC
		private static byte[] Extract(byte[] source, int start, int length, bool littleEndian)
		{
			byte[] array = new byte[length];
			int num = 0;
			for (int i = start; i < start + length; i++)
			{
				array[num] = source[i];
				num++;
			}
			if (littleEndian)
			{
				Array.Reverse(array);
			}
			return array;
		}
	}
}
