using System;
using System.IO;
using System.Text;

namespace Leb128
{
	// Token: 0x02000005 RID: 5
	internal class Leb128Coding
	{
		// Token: 0x06000006 RID: 6 RVA: 0x000024DC File Offset: 0x000006DC
		public static void WriteLeb(Stream stream, byte[] buffer)
		{
			byte[] bytes = BitConverter.GetBytes(buffer.Length);
			stream.Write(bytes, 0, bytes.Length);
			stream.Write(buffer, 0, buffer.Length);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000250B File Offset: 0x0000070B
		public static void WriteLeb(Stream stream, string data)
		{
			Leb128Coding.WriteLeb(stream, Encoding.UTF8.GetBytes(data));
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002520 File Offset: 0x00000720
		public static void WriteLeb(Stream stream, bool data)
		{
			stream.WriteByte(Convert.ToByte(data));
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002530 File Offset: 0x00000730
		public static void WriteLeb(Stream stream, byte data)
		{
			stream.WriteByte(data);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000253C File Offset: 0x0000073C
		public static void WriteLeb(Stream stream, short data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			stream.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002560 File Offset: 0x00000760
		public static void WriteLeb(Stream stream, int data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			stream.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002584 File Offset: 0x00000784
		public static void WriteLeb(Stream stream, long data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			stream.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000025A8 File Offset: 0x000007A8
		public static void WriteLeb(Stream stream, float data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			stream.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000025CC File Offset: 0x000007CC
		public static void WriteLeb(Stream stream, double data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			stream.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000025F0 File Offset: 0x000007F0
		public static void WriteLeb(Stream stream, ushort data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			stream.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002614 File Offset: 0x00000814
		public static void WriteLeb(Stream stream, uint data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			stream.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002638 File Offset: 0x00000838
		public static void WriteLeb(Stream stream, ulong data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			stream.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000265C File Offset: 0x0000085C
		public static byte[] ReadLebArray(Stream stream)
		{
			byte[] array = new byte[4];
			stream.Read(array, 0, 4);
			int num = BitConverter.ToInt32(array, 0);
			byte[] array2 = new byte[num];
			stream.Read(array2, 0, num);
			return array2;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x0000269C File Offset: 0x0000089C
		public static string ReadLebString(Stream stream)
		{
			return Encoding.UTF8.GetString(Leb128Coding.ReadLebArray(stream));
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000026C0 File Offset: 0x000008C0
		public static bool ReadLebBool(Stream stream)
		{
			return Convert.ToBoolean(stream.ReadByte());
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000026E0 File Offset: 0x000008E0
		public static byte ReadLebByte(Stream stream)
		{
			return (byte)stream.ReadByte();
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000026FC File Offset: 0x000008FC
		public static short ReadLebShort(Stream stream)
		{
			byte[] array = new byte[2];
			stream.Read(array, 0, array.Length);
			return BitConverter.ToInt16(array, 0);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002728 File Offset: 0x00000928
		public static int ReadLebInt(Stream stream)
		{
			byte[] array = new byte[4];
			stream.Read(array, 0, array.Length);
			return BitConverter.ToInt32(array, 0);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002754 File Offset: 0x00000954
		public static long ReadLebLong(Stream stream)
		{
			byte[] array = new byte[8];
			stream.Read(array, 0, array.Length);
			return BitConverter.ToInt64(array, 0);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002780 File Offset: 0x00000980
		public static float ReadLebFloat(Stream stream)
		{
			byte[] array = new byte[4];
			stream.Read(array, 0, array.Length);
			return BitConverter.ToSingle(array, 0);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000027AC File Offset: 0x000009AC
		public static double ReadLebDouble(Stream stream)
		{
			byte[] array = new byte[8];
			stream.Read(array, 0, array.Length);
			return BitConverter.ToDouble(array, 0);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000027D8 File Offset: 0x000009D8
		public static ushort ReadLebUshort(Stream stream)
		{
			byte[] array = new byte[2];
			stream.Read(array, 0, array.Length);
			return BitConverter.ToUInt16(array, 0);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002804 File Offset: 0x00000A04
		public static uint ReadLebUint(Stream stream)
		{
			byte[] array = new byte[4];
			stream.Read(array, 0, array.Length);
			return BitConverter.ToUInt32(array, 0);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002830 File Offset: 0x00000A30
		public static ulong ReadLebUlong(Stream stream)
		{
			byte[] array = new byte[8];
			stream.Read(array, 0, array.Length);
			return BitConverter.ToUInt64(array, 0);
		}
	}
}
