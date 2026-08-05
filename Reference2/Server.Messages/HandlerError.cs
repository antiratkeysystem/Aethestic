using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Server.Connectings;
using Server.Helper;

namespace Server.Messages;

internal class HandlerError
{
	private static ConcurrentDictionary<string, int> clientErrorCounts;

	private static ConcurrentDictionary<string, int> ipErrorCounts;

	private const int MAX_ERRORS_PER_CLIENT = 20;

	private const int MAX_ERRORS_PER_IP = 50;

	private static bool _loggedFlood;

	static HandlerError()
	{
		clientErrorCounts = new ConcurrentDictionary<string, int>();
		ipErrorCounts = new ConcurrentDictionary<string, int>();
		_loggedFlood = false;
		new Timer(delegate
		{
			KeyValuePair<string, int>[] array = clientErrorCounts.ToArray();
			int value2;
			for (int i = 0; i < array.Length; i++)
			{
				KeyValuePair<string, int> keyValuePair = array[i];
				clientErrorCounts.AddOrUpdate(keyValuePair.Key, 0, (string key, int count) => Math.Max(0, count - 5));
				if (clientErrorCounts.TryGetValue(keyValuePair.Key, out var value) && value <= 0)
				{
					clientErrorCounts.TryRemove(keyValuePair.Key, out value2);
				}
			}
			array = ipErrorCounts.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				KeyValuePair<string, int> keyValuePair2 = array[i];
				ipErrorCounts.AddOrUpdate(keyValuePair2.Key, 0, (string key, int count) => Math.Max(0, count - 10));
				if (ipErrorCounts.TryGetValue(keyValuePair2.Key, out var value3) && value3 <= 0)
				{
					ipErrorCounts.TryRemove(keyValuePair2.Key, out value2);
				}
			}
			_loggedFlood = false;
		}, null, TimeSpan.FromSeconds(10.0), TimeSpan.FromSeconds(10.0));
	}

	public static void Read(Clients client, object[] objects)
	{
		if (client == null || objects == null || objects.Length < 2)
		{
			client?.Disconnect();
			return;
		}
		string ip = client.IP;
		string clientHwid = client.Hwid ?? ip;
		if (ipErrorCounts.AddOrUpdate(ip, 1, (string key, int count) => count + 1) > 50)
		{
			client.Disconnect();
		}
		else if (clientErrorCounts.AddOrUpdate(clientHwid, 1, (string key, int count) => count + 1) <= 20)
		{
			string errorMessage = (string)objects[1];
			if (errorMessage != null && errorMessage.Length > 300)
			{
				errorMessage = errorMessage.Substring(0, 300) + "...";
			}
			Methods.AppendLogs(ip, "Error: " + errorMessage, Color.Red);
		}
	}
}
