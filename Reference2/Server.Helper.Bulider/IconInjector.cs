using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Server.Helper.Bulider;

public static class IconInjector
{
	[SuppressUnmanagedCodeSecurity]
	private class NativeMethods
	{
		[DllImport("kernel32")]
		public static extern IntPtr BeginUpdateResource(string fileName, [MarshalAs(UnmanagedType.Bool)] bool deleteExistingResources);

		[DllImport("kernel32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UpdateResource(IntPtr hUpdate, IntPtr type, IntPtr name, short language, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] byte[] data, int dataSize);

		[DllImport("kernel32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EndUpdateResource(IntPtr hUpdate, [MarshalAs(UnmanagedType.Bool)] bool discard);
	}

	private struct ICONDIR
	{
		public ushort Reserved;

		public ushort Type;

		public ushort Count;
	}

	private struct ICONDIRENTRY
	{
		public byte Width;

		public byte Height;

		public byte ColorCount;

		public byte Reserved;

		public ushort Planes;

		public ushort BitCount;

		public int BytesInRes;

		public int ImageOffset;
	}

	private struct BITMAPINFOHEADER
	{
		public uint Size;

		public int Width;

		public int Height;

		public ushort Planes;

		public ushort BitCount;

		public uint Compression;

		public uint SizeImage;

		public int XPelsPerMeter;

		public int YPelsPerMeter;

		public uint ClrUsed;

		public uint ClrImportant;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	private struct GRPICONDIRENTRY
	{
		public byte Width;

		public byte Height;

		public byte ColorCount;

		public byte Reserved;

		public ushort Planes;

		public ushort BitCount;

		public int BytesInRes;

		public ushort ID;
	}

	private class IconFile
	{
		private ICONDIR iconDir;

		private ICONDIRENTRY[] iconEntry;

		private byte[][] iconImage;

		public int ImageCount => iconDir.Count;

		public byte[] ImageData(int index)
		{
			return iconImage[index];
		}

		public static IconFile FromFile(string filename)
		{
			IconFile iconFile = new IconFile();
			byte[] array = File.ReadAllBytes(filename);
			if (array.Length < 6)
			{
				throw new ArgumentException("Invalid icon file: file too small");
			}
			GCHandle gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			iconFile.iconDir = (ICONDIR)Marshal.PtrToStructure(gchandle.AddrOfPinnedObject(), typeof(ICONDIR));
			if (iconFile.iconDir.Count == 0 || iconFile.iconDir.Count > 256)
			{
				gchandle.Free();
				throw new ArgumentException("Invalid icon file: invalid icon count");
			}
			iconFile.iconEntry = new ICONDIRENTRY[iconFile.iconDir.Count];
			iconFile.iconImage = new byte[iconFile.iconDir.Count][];
			int num = Marshal.SizeOf(iconFile.iconDir);
			Type typeFromHandle = typeof(ICONDIRENTRY);
			int num2 = Marshal.SizeOf(typeFromHandle);
			for (int i = 0; i <= iconFile.iconDir.Count - 1; i++)
			{
				if (num + num2 > array.Length)
				{
					gchandle.Free();
					throw new ArgumentException("Invalid icon file: corrupted icon directory entry");
				}
				ICONDIRENTRY icondirentry = (ICONDIRENTRY)Marshal.PtrToStructure(new IntPtr(gchandle.AddrOfPinnedObject().ToInt64() + num), typeFromHandle);
				iconFile.iconEntry[i] = icondirentry;
				if (icondirentry.ImageOffset < 0 || icondirentry.BytesInRes < 0)
				{
					gchandle.Free();
					throw new ArgumentException($"Invalid icon file: negative offset or size (icon {i})");
				}
				if (icondirentry.ImageOffset + icondirentry.BytesInRes > array.Length)
				{
					gchandle.Free();
					throw new ArgumentException($"Invalid icon file: image data out of bounds (icon {i}, offset={icondirentry.ImageOffset}, size={icondirentry.BytesInRes}, file size={array.Length})");
				}
				iconFile.iconImage[i] = new byte[icondirentry.BytesInRes];
				Buffer.BlockCopy(array, icondirentry.ImageOffset, iconFile.iconImage[i], 0, icondirentry.BytesInRes);
				num += num2;
			}
			gchandle.Free();
			return iconFile;
		}

		public byte[] CreateIconGroupData(uint iconBaseID)
		{
			byte[] array = new byte[Marshal.SizeOf(typeof(ICONDIR)) + Marshal.SizeOf(typeof(GRPICONDIRENTRY)) * ImageCount];
			GCHandle gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			Marshal.StructureToPtr(iconDir, gchandle.AddrOfPinnedObject(), fDeleteOld: false);
			int num = Marshal.SizeOf(iconDir);
			for (int i = 0; i <= ImageCount - 1; i++)
			{
				GRPICONDIRENTRY structure = default(GRPICONDIRENTRY);
				BITMAPINFOHEADER bitmapinfoheader = default(BITMAPINFOHEADER);
				GCHandle gchandle2 = GCHandle.Alloc(bitmapinfoheader, GCHandleType.Pinned);
				Marshal.Copy(ImageData(i), 0, gchandle2.AddrOfPinnedObject(), Marshal.SizeOf(typeof(BITMAPINFOHEADER)));
				gchandle2.Free();
				structure.Width = iconEntry[i].Width;
				structure.Height = iconEntry[i].Height;
				structure.ColorCount = iconEntry[i].ColorCount;
				structure.Reserved = iconEntry[i].Reserved;
				structure.Planes = bitmapinfoheader.Planes;
				structure.BitCount = bitmapinfoheader.BitCount;
				structure.BytesInRes = iconEntry[i].BytesInRes;
				structure.ID = Convert.ToUInt16(iconBaseID + i);
				Marshal.StructureToPtr(structure, new IntPtr(gchandle.AddrOfPinnedObject().ToInt64() + num), fDeleteOld: false);
				num += Marshal.SizeOf(typeof(GRPICONDIRENTRY));
			}
			gchandle.Free();
			return array;
		}
	}

	public static void InjectIcon(string exeFileName, string iconFileName)
	{
		InjectIcon(exeFileName, iconFileName, 1u, 1u);
	}

	public static void InjectIcon(string exeFileName, string iconFileName, uint iconGroupID, uint iconBaseID)
	{
		IconFile iconFile = IconFile.FromFile(iconFileName);
		IntPtr hUpdate = NativeMethods.BeginUpdateResource(exeFileName, deleteExistingResources: false);
		byte[] array = iconFile.CreateIconGroupData(iconBaseID);
		NativeMethods.UpdateResource(hUpdate, new IntPtr(14L), new IntPtr(iconGroupID), 0, array, array.Length);
		for (int i = 0; i <= iconFile.ImageCount - 1; i++)
		{
			byte[] array2 = iconFile.ImageData(i);
			NativeMethods.UpdateResource(hUpdate, new IntPtr(3L), new IntPtr(iconBaseID + i), 0, array2, array2.Length);
		}
		NativeMethods.EndUpdateResource(hUpdate, discard: false);
	}
}
