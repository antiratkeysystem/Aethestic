using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Intelix.Helper
{
	// Token: 0x0200005B RID: 91
	public static class HwidGenerator
	{
		// Token: 0x06000138 RID: 312 RVA: 0x00010AEC File Offset: 0x0000ECEC
		public static string GetHwid()
		{
			bool flag = HwidGenerator._hwid != null;
			string hwid;
			if (flag)
			{
				hwid = HwidGenerator._hwid;
			}
			else
			{
				object @lock = HwidGenerator._lock;
				bool flag2 = false;
				try
				{
					Monitor.Enter(@lock, ref flag2);
					bool flag3 = HwidGenerator._hwid != null;
					if (flag3)
					{
						hwid = HwidGenerator._hwid;
					}
					else
					{
						List<string> list = new List<string>();
						string mg = null;
						string cpuName = null;
						List<string> vols = null;
						List<string> macs = null;
						Task task = Task.Run(delegate()
						{
							mg = HwidGenerator.GetMachineGuid();
						});
						Task task2 = Task.Run(delegate()
						{
							cpuName = HwidGenerator.GetCpuName();
						});
						Task task3 = Task.Run(delegate()
						{
							vols = HwidGenerator.GetFixedVolumeSerials();
						});
						Task task4 = Task.Run(delegate()
						{
							macs = HwidGenerator.GetMacAddresses();
						});
						Task.WaitAll(new Task[]
						{
							task,
							task2,
							task3,
							task4
						});
						bool flag4 = !string.IsNullOrEmpty(mg);
						if (flag4)
						{
							list.Add("MG:" + mg);
						}
						bool flag5 = !string.IsNullOrEmpty(cpuName);
						if (flag5)
						{
							list.Add("CPU:" + cpuName);
						}
						list.Add("Cores:" + Environment.ProcessorCount.ToString());
						bool flag6 = vols != null && vols.Count > 0;
						if (flag6)
						{
							list.Add("VOLS:" + string.Join(",", vols));
						}
						bool flag7 = macs != null && macs.Count > 0;
						if (flag7)
						{
							list.Add("MACS:" + string.Join(",", macs));
						}
						list.Add("MN:" + Environment.MachineName);
						HwidGenerator._hwid = HwidGenerator.ComputeSha256(string.Join("|", list));
						hwid = HwidGenerator._hwid;
					}
				}
				finally
				{
					if (flag2)
					{
						Monitor.Exit(@lock);
					}
				}
			}
			return hwid;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00010D48 File Offset: 0x0000EF48
		private static string ComputeSha256(string input)
		{
			string result;
			using (SHA256 sha = SHA256.Create())
			{
				byte[] array = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
				StringBuilder stringBuilder = new StringBuilder(array.Length * 2);
				byte[] array2 = array;
				foreach (byte b in array2)
				{
					stringBuilder.Append(b.ToString("x2"));
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00010DD8 File Offset: 0x0000EFD8
		private static string GetMachineGuid()
		{
			string result;
			try
			{
				using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\\Microsoft\\Cryptography"))
				{
					string text = ((registryKey != null) ? registryKey.GetValue("MachineGuid") : null) as string;
					bool flag = !string.IsNullOrEmpty(text);
					if (flag)
					{
						return text.Trim();
					}
				}
				using (RegistryKey registryKey2 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\Cryptography"))
				{
					string text2 = ((registryKey2 != null) ? registryKey2.GetValue("MachineGuid") : null) as string;
					result = (((text2 != null) ? text2.Trim() : null) ?? "");
				}
			}
			catch
			{
				result = "";
			}
			return result;
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00010EC8 File Offset: 0x0000F0C8
		private static string GetCpuName()
		{
			string result;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0"))
				{
					string text = ((registryKey != null) ? registryKey.GetValue("ProcessorNameString") : null) as string;
					bool flag = !string.IsNullOrEmpty(text);
					if (flag)
					{
						result = text.Trim();
					}
					else
					{
						string text2 = ((registryKey != null) ? registryKey.GetValue("VendorIdentifier") : null) as string;
						result = (((text2 != null) ? text2.Trim() : null) ?? "");
					}
				}
			}
			catch
			{
				result = "";
			}
			return result;
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00010F78 File Offset: 0x0000F178
		private static List<string> GetFixedVolumeSerials()
		{
			List<string> list = new List<string>();
			try
			{
				DriveInfo[] drives = DriveInfo.GetDrives();
				foreach (DriveInfo driveInfo in drives)
				{
					bool flag = driveInfo.DriveType == DriveType.Fixed && driveInfo.IsReady;
					if (flag)
					{
						StringBuilder stringBuilder = new StringBuilder(261);
						StringBuilder stringBuilder2 = new StringBuilder(261);
						uint num;
						uint num2;
						uint num3;
						bool volumeInformation = NativeMethods.GetVolumeInformation(driveInfo.RootDirectory.FullName, stringBuilder, stringBuilder.Capacity, out num, out num2, out num3, stringBuilder2, stringBuilder2.Capacity);
						if (volumeInformation)
						{
							list.Add(num.ToString("X8").ToLowerInvariant());
						}
					}
				}
			}
			catch
			{
			}
			return list;
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00011050 File Offset: 0x0000F250
		private static List<string> GetMacAddresses()
		{
			List<string> list = new List<string>();
			try
			{
				NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
				foreach (NetworkInterface networkInterface in allNetworkInterfaces)
				{
					bool flag = networkInterface.OperationalStatus != OperationalStatus.Up;
					if (!flag)
					{
						byte[] addressBytes = networkInterface.GetPhysicalAddress().GetAddressBytes();
						bool flag2 = addressBytes.Length == 0;
						if (!flag2)
						{
							StringBuilder stringBuilder = new StringBuilder();
							for (int j = 0; j < addressBytes.Length; j++)
							{
								bool flag3 = j > 0;
								if (flag3)
								{
									stringBuilder.Append(':');
								}
								stringBuilder.Append(addressBytes[j].ToString("x2"));
							}
							list.Add(stringBuilder.ToString());
						}
					}
				}
			}
			catch
			{
			}
			return list;
		}

		// Token: 0x04000023 RID: 35
		private static string _hwid;

		// Token: 0x04000024 RID: 36
		private static readonly object _lock = new object();
	}
}
