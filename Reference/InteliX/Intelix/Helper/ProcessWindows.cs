using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Intelix.Helper
{
	// Token: 0x02000062 RID: 98
	public static class ProcessWindows
	{
		// Token: 0x06000163 RID: 355 RVA: 0x00011A98 File Offset: 0x0000FC98
		public static List<ProcessWindows.ProcInfo> GetProcInfos()
		{
			return new List<ProcessWindows.ProcInfo>(ProcessWindows._procInfos.Value);
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00011ABC File Offset: 0x0000FCBC
		public static List<string> FindFolder(string folderName)
		{
			bool flag = string.IsNullOrWhiteSpace(folderName);
			List<string> result;
			if (flag)
			{
				result = new List<string>();
			}
			else
			{
				ConcurrentDictionary<string, byte> concurrentDictionary = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
				ProcessWindows.SearchNearby(folderName, true, concurrentDictionary, 3);
				result = new List<string>(concurrentDictionary.Keys);
			}
			return result;
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00011B04 File Offset: 0x0000FD04
		public static List<string> FindFile(string fileName)
		{
			bool flag = string.IsNullOrWhiteSpace(fileName);
			List<string> result;
			if (flag)
			{
				result = new List<string>();
			}
			else
			{
				ConcurrentDictionary<string, byte> concurrentDictionary = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
				ProcessWindows.SearchNearby(fileName, false, concurrentDictionary, 3);
				result = new List<string>(concurrentDictionary.Keys);
			}
			return result;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00011B4C File Offset: 0x0000FD4C
		private static void SearchNearby(string target, bool isDirectory, ConcurrentDictionary<string, byte> found, int maxUp = 3)
		{
			bool flag = string.IsNullOrEmpty(target);
			if (!flag)
			{
				List<ProcessWindows.ProcInfo> value = ProcessWindows._procInfos.Value;
				bool flag2 = value == null || value.Count == 0;
				if (!flag2)
				{
					string t = target.Trim();
					Parallel.ForEach<ProcessWindows.ProcInfo>(value, delegate(ProcessWindows.ProcInfo proc)
					{
						try
						{
							string path = proc.Path;
							bool flag3 = !string.IsNullOrEmpty(path);
							if (flag3)
							{
								string directoryName = Path.GetDirectoryName(path);
								bool flag4 = !string.IsNullOrEmpty(directoryName);
								if (flag4)
								{
									for (int i = 0; i < maxUp; i++)
									{
										bool flag5 = string.IsNullOrEmpty(directoryName);
										if (flag5)
										{
											break;
										}
										string path2 = Path.Combine(directoryName, t);
										bool isDirectory2 = isDirectory;
										if (isDirectory2)
										{
											bool flag6 = Directory.Exists(path2);
											if (flag6)
											{
												try
												{
													found.TryAdd(Path.GetFullPath(path2), 0);
												}
												catch
												{
												}
											}
										}
										else
										{
											bool flag7 = File.Exists(path2);
											if (flag7)
											{
												try
												{
													found.TryAdd(Path.GetFullPath(path2), 0);
												}
												catch
												{
												}
											}
										}
										directoryName = Path.GetDirectoryName(directoryName);
									}
								}
							}
						}
						catch
						{
						}
					});
				}
			}
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00011BC4 File Offset: 0x0000FDC4
		private static List<ProcessWindows.ProcInfo> BuildCache()
		{
			ConcurrentDictionary<string, ProcessWindows.ProcInfo> result = new ConcurrentDictionary<string, ProcessWindows.ProcInfo>(StringComparer.OrdinalIgnoreCase);
			uint num = 4096U;
			uint[] pids = new uint[num];
			uint num2;
			bool flag = !NativeMethods.EnumProcesses(pids, num * 4U, out num2);
			if (flag)
			{
				num = 65536U;
				pids = new uint[num];
				bool flag2 = !NativeMethods.EnumProcesses(pids, num * 4U, out num2);
				if (flag2)
				{
					return new List<ProcessWindows.ProcInfo>();
				}
			}
			int num3 = (int)(num2 / 4U);
			bool flag3 = num3 <= 0;
			List<ProcessWindows.ProcInfo> result2;
			if (flag3)
			{
				result2 = new List<ProcessWindows.ProcInfo>();
			}
			else
			{
				ThreadLocal<StringBuilder> sbLocal = new ThreadLocal<StringBuilder>(() => new StringBuilder(1024));
				Parallel.For(0, num3, delegate(int i)
				{
					uint num4 = pids[i];
					bool flag4 = num4 == 0U || num4 == 4U;
					if (!flag4)
					{
						IntPtr intPtr = IntPtr.Zero;
						try
						{
							intPtr = NativeMethods.OpenProcess(5136U, false, (int)num4);
							bool flag5 = !(intPtr == IntPtr.Zero);
							if (flag5)
							{
								string text = null;
								try
								{
									StringBuilder value = sbLocal.Value;
									value.Clear();
									uint capacity = (uint)value.Capacity;
									bool flag6 = NativeMethods.QueryFullProcessImageName(intPtr, 0, value, ref capacity);
									if (flag6)
									{
										text = value.ToString(0, (int)capacity);
									}
								}
								catch
								{
								}
								string s = null;
								bool flag7 = !string.IsNullOrEmpty(text);
								if (flag7)
								{
									try
									{
										s = Path.GetFileNameWithoutExtension(text);
									}
									catch
									{
										s = text;
									}
								}
								else
								{
									s = "";
								}
								string text2 = "";
								try
								{
									NativeMethods.PROCESS_MEMORY_COUNTERS_EX process_MEMORY_COUNTERS_EX = default(NativeMethods.PROCESS_MEMORY_COUNTERS_EX);
									process_MEMORY_COUNTERS_EX.cb = (uint)Marshal.SizeOf(typeof(NativeMethods.PROCESS_MEMORY_COUNTERS_EX));
									bool processMemoryInfo = NativeMethods.GetProcessMemoryInfo(intPtr, out process_MEMORY_COUNTERS_EX, process_MEMORY_COUNTERS_EX.cb);
									if (processMemoryInfo)
									{
										text2 = ProcessWindows.FormatBytes((long)process_MEMORY_COUNTERS_EX.WorkingSetSize.ToUInt64());
									}
								}
								catch
								{
								}
								ProcessWindows.ProcInfo procInfo = new ProcessWindows.ProcInfo
								{
									Name = ProcessWindows.SafeString(s),
									Pid = num4.ToString(),
									Path = ProcessWindows.SafeString(text),
									Memory = (text2 ?? "")
								};
								string key = procInfo.Path + "|" + procInfo.Pid;
								result.TryAdd(key, procInfo);
							}
						}
						catch
						{
						}
						finally
						{
							try
							{
								bool flag8 = intPtr != IntPtr.Zero;
								if (flag8)
								{
									NativeMethods.CloseHandle(intPtr);
								}
							}
							catch
							{
							}
						}
					}
				});
				sbLocal.Dispose();
				result2 = new List<ProcessWindows.ProcInfo>(result.Values);
			}
			return result2;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00011CC8 File Offset: 0x0000FEC8
		private static string SafeString(string s)
		{
			bool flag = !string.IsNullOrEmpty(s);
			string result;
			if (flag)
			{
				result = s;
			}
			else
			{
				result = "";
			}
			return result;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00011CF4 File Offset: 0x0000FEF4
		private static string FormatBytes(long bytes)
		{
			bool flag = bytes >= 1073741824L;
			string result;
			if (flag)
			{
				result = ((double)bytes / 1073741824.0).ToString("0.##") + " GB";
			}
			else
			{
				bool flag2 = bytes >= 1048576L;
				if (flag2)
				{
					result = ((double)bytes / 1048576.0).ToString("0.##") + " MB";
				}
				else
				{
					bool flag3 = bytes >= 1024L;
					if (flag3)
					{
						result = ((double)bytes / 1024.0).ToString("0.##") + " KB";
					}
					else
					{
						result = bytes.ToString() + " B";
					}
				}
			}
			return result;
		}

		// Token: 0x04000033 RID: 51
		private static readonly Lazy<List<ProcessWindows.ProcInfo>> _procInfos = new Lazy<List<ProcessWindows.ProcInfo>>(new Func<List<ProcessWindows.ProcInfo>>(ProcessWindows.BuildCache), true);

		// Token: 0x020000E7 RID: 231
		public class ProcInfo
		{
			// Token: 0x17000026 RID: 38
			// (get) Token: 0x06000320 RID: 800 RVA: 0x0001EE38 File Offset: 0x0001D038
			// (set) Token: 0x06000321 RID: 801 RVA: 0x0001EE40 File Offset: 0x0001D040
			public string Name { get; set; }

			// Token: 0x17000027 RID: 39
			// (get) Token: 0x06000322 RID: 802 RVA: 0x0001EE49 File Offset: 0x0001D049
			// (set) Token: 0x06000323 RID: 803 RVA: 0x0001EE51 File Offset: 0x0001D051
			public string Pid { get; set; }

			// Token: 0x17000028 RID: 40
			// (get) Token: 0x06000324 RID: 804 RVA: 0x0001EE5A File Offset: 0x0001D05A
			// (set) Token: 0x06000325 RID: 805 RVA: 0x0001EE62 File Offset: 0x0001D062
			public string Path { get; set; }

			// Token: 0x17000029 RID: 41
			// (get) Token: 0x06000326 RID: 806 RVA: 0x0001EE6B File Offset: 0x0001D06B
			// (set) Token: 0x06000327 RID: 807 RVA: 0x0001EE73 File Offset: 0x0001D073
			public string Memory { get; set; }
		}
	}
}
