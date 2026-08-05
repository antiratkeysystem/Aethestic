using System;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerPiano
{
	public static void Read(Clients client, object[] objects)
	{
		if (objects.Length < 2 || !((string)objects[1] == "Ready"))
		{
			return;
		}
		FormPiano form = null;
		string hwid = null;
		if (!string.IsNullOrEmpty(client.Hwid))
		{
			form = (FormPiano)Application.OpenForms["Piano:" + client.Hwid];
			if (form != null)
			{
				hwid = client.Hwid;
			}
		}
		if (form == null)
		{
			foreach (Form openForm in Application.OpenForms)
			{
				if (!(openForm is FormPiano pianoForm) || !openForm.Name.StartsWith("Piano:"))
				{
					continue;
				}
				string formHwid = openForm.Name.Substring(6);
				if (objects.Length >= 3 && objects[2] is string receivedHwid && string.Equals(formHwid, receivedHwid, StringComparison.OrdinalIgnoreCase))
				{
					form = pianoForm;
					hwid = formHwid;
					if (!string.IsNullOrEmpty(hwid) && string.IsNullOrEmpty(client.Hwid))
					{
						client.Hwid = hwid;
					}
					break;
				}
			}
		}
		if (form == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(client.Hwid) && !string.IsNullOrEmpty(hwid))
		{
			client.Hwid = hwid;
		}
		form.Invoke((MethodInvoker)delegate
		{
			form.client = client;
			if (!string.IsNullOrEmpty(hwid))
			{
				form.Text = "Piano [" + hwid + "]";
			}
			form.rjButton1.Enabled = true;
			form.rjButton2.Enabled = true;
			form.rjButton3.Enabled = true;
			form.rjButton4.Enabled = true;
			form.rjButton5.Enabled = true;
			form.rjButton6.Enabled = true;
			form.rjButton7.Enabled = true;
			form.rjButton8.Enabled = true;
			form.rjButton9.Enabled = true;
			form.rjButton10.Enabled = true;
			form.rjButton11.Enabled = true;
			form.rjButton12.Enabled = true;
			form.rjButton13.Enabled = true;
			form.rjButton14.Enabled = true;
			form.rjButton15.Enabled = true;
			form.rjButton16.Enabled = true;
			form.rjButton17.Enabled = true;
			form.rjButton18.Enabled = true;
			form.rjButton19.Enabled = true;
			form.rjButton20.Enabled = true;
			form.rjButton21.Enabled = true;
			form.rjButton22.Enabled = true;
			form.rjButton23.Enabled = true;
			form.rjButton24.Enabled = true;
			form.rjButton25.Enabled = true;
			form.rjButton26.Enabled = true;
			form.rjButton27.Enabled = true;
			form.rjButton28.Enabled = true;
		});
		client.Tag = form;
	}
}
