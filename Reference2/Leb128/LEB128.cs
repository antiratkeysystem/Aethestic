using System;
using System.Collections.Generic;
using System.IO;

namespace Leb128;

public class LEB128
{
	public static object[] Read(byte[] data)
	{
		List<object> list = new List<object>();
		using MemoryStream memoryStream = new MemoryStream(data);
		while (true)
		{
			int num = memoryStream.ReadByte();
			switch (num)
			{
			case 0:
				list.Add(LEB128Coding.ReadLebString(memoryStream));
				break;
			case 1:
				list.Add(LEB128Coding.ReadLebBool(memoryStream));
				break;
			case 2:
				list.Add(LEB128Coding.ReadLebByte(memoryStream));
				break;
			case 3:
				list.Add(LEB128Coding.ReadLebShort(memoryStream));
				break;
			case 4:
				list.Add(LEB128Coding.ReadLebInt(memoryStream));
				break;
			case 5:
				list.Add(LEB128Coding.ReadLebLong(memoryStream));
				break;
			case 6:
				list.Add(LEB128Coding.ReadLebFloat(memoryStream));
				break;
			case 7:
				list.Add(LEB128Coding.ReadLebDouble(memoryStream));
				break;
			case 8:
				list.Add(LEB128Coding.ReadLebArray(memoryStream));
				break;
			case 9:
				list.Add(LEB128Coding.ReadLebUshort(memoryStream));
				break;
			case 10:
				list.Add(LEB128Coding.ReadLebUint(memoryStream));
				break;
			case 11:
				list.Add(LEB128Coding.ReadLebUlong(memoryStream));
				break;
			case 12:
				list.Add(Read(LEB128Coding.ReadLebArray(memoryStream)));
				break;
			default:
				throw new Exception(num.ToString());
			case -1:
				return list.ToArray();
			}
		}
	}

	public static byte[] Write(object[] data)
	{
		using MemoryStream memoryStream = new MemoryStream();
		foreach (object obj in data)
		{
			if (obj is string)
			{
				memoryStream.WriteByte(0);
				LEB128Coding.WriteLeb(memoryStream, (string)obj);
				continue;
			}
			if (obj is bool)
			{
				memoryStream.WriteByte(1);
				LEB128Coding.WriteLeb(memoryStream, (bool)obj);
				continue;
			}
			if (obj is byte)
			{
				memoryStream.WriteByte(2);
				LEB128Coding.WriteLeb(memoryStream, (byte)obj);
				continue;
			}
			if (obj is short)
			{
				memoryStream.WriteByte(3);
				LEB128Coding.WriteLeb(memoryStream, (short)obj);
				continue;
			}
			if (obj is int)
			{
				memoryStream.WriteByte(4);
				LEB128Coding.WriteLeb(memoryStream, (int)obj);
				continue;
			}
			if (obj is long)
			{
				memoryStream.WriteByte(5);
				LEB128Coding.WriteLeb(memoryStream, (long)obj);
				continue;
			}
			if (obj is float)
			{
				memoryStream.WriteByte(6);
				LEB128Coding.WriteLeb(memoryStream, (float)obj);
				continue;
			}
			if (obj is double)
			{
				memoryStream.WriteByte(7);
				LEB128Coding.WriteLeb(memoryStream, (double)obj);
				continue;
			}
			if (obj is byte[])
			{
				memoryStream.WriteByte(8);
				LEB128Coding.WriteLeb(memoryStream, (byte[])obj);
				continue;
			}
			if (obj is ushort)
			{
				memoryStream.WriteByte(9);
				LEB128Coding.WriteLeb(memoryStream, (ushort)obj);
				continue;
			}
			if (obj is uint)
			{
				memoryStream.WriteByte(10);
				LEB128Coding.WriteLeb(memoryStream, (uint)obj);
				continue;
			}
			if (obj is ulong)
			{
				memoryStream.WriteByte(11);
				LEB128Coding.WriteLeb(memoryStream, (ulong)obj);
				continue;
			}
			if (!(obj is object[]))
			{
				throw new Exception(obj.GetType().Name);
			}
			memoryStream.WriteByte(12);
			LEB128Coding.WriteLeb(memoryStream, Write((object[])obj));
		}
		return memoryStream.ToArray();
	}
}
