using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Obfuscator.Obfuscator.Anti.Runtime;

internal class AntiDumpRun
{
	internal enum MemoryProtection
	{
		ExecuteReadWrite = 0x40
	}

	private unsafe static void CopyBlock(void* destination, void* source, uint byteCount)
	{
	}

	private unsafe static void InitBlock(void* startAddress, byte value, uint byteCount)
	{
	}

	[DllImport("kernel32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, [MarshalAs(UnmanagedType.U4)] MemoryProtection flNewProtect, [MarshalAs(UnmanagedType.U4)] out MemoryProtection lpflOldProtect);

	private unsafe static void Initialize()
	{
		Module module = typeof(AntiDumpRun).Module;
		byte* ptr = (byte*)(void*)Marshal.GetHINSTANCE(module);
		byte* ptr2 = ptr + 60;
		ptr2 = ptr + (uint)(*(int*)ptr2);
		ptr2 += 6;
		ushort num = *(ushort*)ptr2;
		ptr2 += 14;
		ushort num2 = *(ushort*)ptr2;
		ptr2 = ptr2 + 4 + (int)num2;
		byte* ptr3 = stackalloc byte[11];
		MemoryProtection memoryProtection;
		if (module.FullyQualifiedName[0] != '<')
		{
			byte* ptr4 = ptr + (uint)(*((int*)ptr2 - 4));
			if (*((uint*)ptr2 - 30) != 0)
			{
				byte* ptr5 = ptr + (uint)(*((int*)ptr2 - 30));
				byte* ptr6 = ptr + (uint)(*(int*)ptr5);
				byte* num3 = ptr + (uint)((int*)ptr5)[3];
				byte* ptr8 = ptr + (uint)(*(int*)ptr6) + 2;
				VirtualProtect(new IntPtr(num3), 11u, MemoryProtection.ExecuteReadWrite, out memoryProtection);
				*(int*)ptr3 = 1818522734;
				((int*)ptr3)[1] = 1818504812;
				((short*)ptr3)[4] = 108;
				ptr3[10] = 0;
				CopyBlock(num3, ptr3, 11u);
				VirtualProtect(new IntPtr(ptr8), 11u, MemoryProtection.ExecuteReadWrite, out memoryProtection);
				*(int*)ptr3 = 1866691662;
				((int*)ptr3)[1] = 1852404846;
				((short*)ptr3)[4] = 25973;
				ptr3[10] = 0;
				CopyBlock(ptr8, ptr3, 11u);
			}
			for (int i = 0; i < num; i++)
			{
				VirtualProtect(new IntPtr(ptr2), 8u, MemoryProtection.ExecuteReadWrite, out memoryProtection);
				InitBlock(ptr2, 0, 8u);
				ptr2 += 40;
			}
			VirtualProtect(new IntPtr(ptr4), 72u, MemoryProtection.ExecuteReadWrite, out memoryProtection);
			byte* ptr9 = ptr + (uint)((int*)ptr4)[2];
			InitBlock(ptr4, 0, 16u);
			VirtualProtect(new IntPtr(ptr9), 4u, MemoryProtection.ExecuteReadWrite, out memoryProtection);
			*(int*)ptr9 = 0;
			ptr9 += 12;
			ptr9 += (uint)(*(int*)ptr9);
			ptr9 = (byte*)(((long)ptr9 + 7L) & -4);
			ptr9 += 2;
			ushort num4 = *ptr9;
			ptr9 += 2;
			for (int j = 0; j < num4; j++)
			{
				VirtualProtect(new IntPtr(ptr9), 8u, MemoryProtection.ExecuteReadWrite, out memoryProtection);
				ptr9 += 4;
				ptr9 += 4;
				for (int k = 0; k < 8; k++)
				{
					VirtualProtect(new IntPtr(ptr9), 4u, MemoryProtection.ExecuteReadWrite, out memoryProtection);
					*ptr9 = 0;
					ptr9++;
					if (*ptr9 == 0)
					{
						ptr9 += 3;
						break;
					}
					*ptr9 = 0;
					ptr9++;
					if (*ptr9 == 0)
					{
						ptr9 += 2;
						break;
					}
					*ptr9 = 0;
					ptr9++;
					if (*ptr9 == 0)
					{
						ptr9++;
						break;
					}
					*ptr9 = 0;
					ptr9++;
				}
			}
			return;
		}
		uint num5 = *((uint*)ptr2 - 4);
		uint num6 = *((uint*)ptr2 - 30);
		uint[] array = new uint[num];
		uint[] array2 = new uint[num];
		uint[] array3 = new uint[num];
		for (int l = 0; l < num; l++)
		{
			VirtualProtect(new IntPtr(ptr2), 8u, MemoryProtection.ExecuteReadWrite, out memoryProtection);
			Marshal.Copy(new byte[8], 0, (IntPtr)ptr2, 8);
			array[l] = ((uint*)ptr2)[3];
			array2[l] = ((uint*)ptr2)[2];
			array3[l] = ((uint*)ptr2)[5];
			ptr2 += 40;
		}
		if (num6 != 0)
		{
			for (int m = 0; m < num; m++)
			{
				if (array[m] <= num6 && num6 < array[m] + array2[m])
				{
					num6 = num6 - array[m] + array3[m];
					break;
				}
			}
			byte* ptr10 = ptr + num6;
			uint num7 = *(uint*)ptr10;
			for (int n = 0; n < num; n++)
			{
				if (array[n] <= num7 && num7 < array[n] + array2[n])
				{
					num7 = num7 - array[n] + array3[n];
					break;
				}
			}
			byte* ptr11 = ptr + num7;
			uint num8 = ((uint*)ptr10)[3];
			for (int num9 = 0; num9 < num; num9++)
			{
				if (array[num9] <= num8 && num8 < array[num9] + array2[num9])
				{
					num8 = num8 - array[num9] + array3[num9];
					break;
				}
			}
			uint num10 = *(uint*)ptr11 + 2;
			for (int num11 = 0; num11 < num; num11++)
			{
				if (array[num11] <= num10 && num10 < array[num11] + array2[num11])
				{
					num10 = num10 - array[num11] + array3[num11];
					break;
				}
			}
			VirtualProtect(new IntPtr(ptr + num8), 11u, MemoryProtection.ExecuteReadWrite, out memoryProtection);
			*(int*)ptr3 = 1818522734;
			((int*)ptr3)[1] = 1818504812;
			((short*)ptr3)[4] = 108;
			ptr3[10] = 0;
			CopyBlock(ptr + num8, ptr3, 11u);
			VirtualProtect(new IntPtr(ptr + num10), 11u, MemoryProtection.ExecuteReadWrite, out memoryProtection);
			*(int*)ptr3 = 1866691662;
			((int*)ptr3)[1] = 1852404846;
			((short*)ptr3)[4] = 25973;
			ptr3[10] = 0;
			CopyBlock(ptr + num10, ptr3, 11u);
		}
		for (int num12 = 0; num12 < num; num12++)
		{
			if (array[num12] <= num5 && num5 < array[num12] + array2[num12])
			{
				num5 = num5 - array[num12] + array3[num12];
				break;
			}
		}
		byte* ptr12 = ptr + num5;
		VirtualProtect(new IntPtr(ptr12), 72u, MemoryProtection.ExecuteReadWrite, out memoryProtection);
		uint num13 = ((uint*)ptr12)[2];
		for (int num14 = 0; num14 < num; num14++)
		{
			if (array[num14] <= num13 && num13 < array[num14] + array2[num14])
			{
				num13 = num13 - array[num14] + array3[num14];
				break;
			}
		}
		InitBlock(ptr12, 0, 16u);
		byte* ptr13 = ptr + num13;
		VirtualProtect(new IntPtr(ptr13), 4u, MemoryProtection.ExecuteReadWrite, out memoryProtection);
		*(int*)ptr13 = 0;
		ptr13 += 12;
		ptr13 += (uint)(*(int*)ptr13);
		ptr13 = (byte*)(((long)ptr13 + 7L) & -4);
		ptr13 += 2;
		ushort num15 = *ptr13;
		ptr13 += 2;
		for (int num16 = 0; num16 < num15; num16++)
		{
			VirtualProtect(new IntPtr(ptr13), 8u, MemoryProtection.ExecuteReadWrite, out memoryProtection);
			ptr13 += 4;
			ptr13 += 4;
			for (int num17 = 0; num17 < 8; num17++)
			{
				VirtualProtect(new IntPtr(ptr13), 4u, MemoryProtection.ExecuteReadWrite, out memoryProtection);
				*ptr13 = 0;
				ptr13++;
				if (*ptr13 == 0)
				{
					ptr13 += 3;
					break;
				}
				*ptr13 = 0;
				ptr13++;
				if (*ptr13 == 0)
				{
					ptr13 += 2;
					break;
				}
				*ptr13 = 0;
				ptr13++;
				if (*ptr13 == 0)
				{
					ptr13++;
					break;
				}
				*ptr13 = 0;
				ptr13++;
			}
		}
	}
}
