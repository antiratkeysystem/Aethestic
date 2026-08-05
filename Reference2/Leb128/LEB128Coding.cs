using System;
using System.IO;
using System.Text;

namespace Leb128;

internal class LEB128Coding
{
	public static void WriteLeb(Stream stream, byte[] buffer)
	{
		byte[] bytes = BitConverter.GetBytes(buffer.Length);
		stream.Write(bytes, 0, bytes.Length);
		stream.Write(buffer, 0, buffer.Length);
	}

	public static void WriteLeb(Stream stream, string data)
	{
		WriteLeb(stream, Encoding.UTF8.GetBytes(data));
	}

	public static void WriteLeb(Stream stream, bool data)
	{
		stream.WriteByte(Convert.ToByte(data));
	}

	public static void WriteLeb(Stream stream, byte data)
	{
		stream.WriteByte(data);
	}

	public static void WriteLeb(Stream stream, short data)
	{
		byte[] bytes = BitConverter.GetBytes(data);
		stream.Write(bytes, 0, bytes.Length);
	}

	public static void WriteLeb(Stream stream, int data)
	{
		byte[] bytes = BitConverter.GetBytes(data);
		stream.Write(bytes, 0, bytes.Length);
	}

	public static void WriteLeb(Stream stream, long data)
	{
		byte[] bytes = BitConverter.GetBytes(data);
		stream.Write(bytes, 0, bytes.Length);
	}

	public static void WriteLeb(Stream stream, float data)
	{
		byte[] bytes = BitConverter.GetBytes(data);
		stream.Write(bytes, 0, bytes.Length);
	}

	public static void WriteLeb(Stream stream, double data)
	{
		byte[] bytes = BitConverter.GetBytes(data);
		stream.Write(bytes, 0, bytes.Length);
	}

	public static void WriteLeb(Stream stream, ushort data)
	{
		byte[] bytes = BitConverter.GetBytes(data);
		stream.Write(bytes, 0, bytes.Length);
	}

	public static void WriteLeb(Stream stream, uint data)
	{
		byte[] bytes = BitConverter.GetBytes(data);
		stream.Write(bytes, 0, bytes.Length);
	}

	public static void WriteLeb(Stream stream, ulong data)
	{
		byte[] bytes = BitConverter.GetBytes(data);
		stream.Write(bytes, 0, bytes.Length);
	}

	public static byte[] ReadLebArray(Stream stream)
	{
		byte[] array = new byte[4];
		stream.Read(array, 0, 4);
		int num = BitConverter.ToInt32(array, 0);
		byte[] array2 = new byte[num];
		stream.Read(array2, 0, num);
		return array2;
	}

	public static string ReadLebString(Stream stream)
	{
		return Encoding.UTF8.GetString(ReadLebArray(stream));
	}

	public static bool ReadLebBool(Stream stream)
	{
		return Convert.ToBoolean(stream.ReadByte());
	}

	public static byte ReadLebByte(Stream stream)
	{
		return (byte)stream.ReadByte();
	}

	public static short ReadLebShort(Stream stream)
	{
		byte[] array = new byte[2];
		stream.Read(array, 0, array.Length);
		return BitConverter.ToInt16(array, 0);
	}

	public static int ReadLebInt(Stream stream)
	{
		byte[] array = new byte[4];
		stream.Read(array, 0, array.Length);
		return BitConverter.ToInt32(array, 0);
	}

	public static long ReadLebLong(Stream stream)
	{
		byte[] array = new byte[8];
		stream.Read(array, 0, array.Length);
		return BitConverter.ToInt64(array, 0);
	}

	public static float ReadLebFloat(Stream stream)
	{
		byte[] array = new byte[4];
		stream.Read(array, 0, array.Length);
		return BitConverter.ToSingle(array, 0);
	}

	public static double ReadLebDouble(Stream stream)
	{
		byte[] array = new byte[8];
		stream.Read(array, 0, array.Length);
		return BitConverter.ToDouble(array, 0);
	}

	public static ushort ReadLebUshort(Stream stream)
	{
		byte[] array = new byte[2];
		stream.Read(array, 0, array.Length);
		return BitConverter.ToUInt16(array, 0);
	}

	public static uint ReadLebUint(Stream stream)
	{
		byte[] array = new byte[4];
		stream.Read(array, 0, array.Length);
		return BitConverter.ToUInt32(array, 0);
	}

	public static ulong ReadLebUlong(Stream stream)
	{
		byte[] array = new byte[8];
		stream.Read(array, 0, array.Length);
		return BitConverter.ToUInt64(array, 0);
	}
}
