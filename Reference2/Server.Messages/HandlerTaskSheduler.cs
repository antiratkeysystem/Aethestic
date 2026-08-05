using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerTaskSheduler
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
		switch ((string)objects[1])
		{
		case "Connect":
		{
			FormTaskSheduler form4 = (FormTaskSheduler)Application.OpenForms["TaskScheduler:" + (string)objects[2]];
			if (form4 == null)
			{
				client.Disconnect();
				break;
			}
			form4.Invoke((MethodInvoker)delegate
			{
				form4.Text = "Task Scheduler [" + (string)objects[2] + "]";
				form4.client = client;
				form4.materialLabel1.Enabled = true;
				form4.dataGridView1.Enabled = true;
				form4.materialLabel1.Text = "Connected successfully";
			});
			client.Tag = form4;
			client.Hwid = (string)objects[2];
			break;
		}
		case "List":
		{
			if (client.Tag == null)
			{
				break;
			}
			FormTaskSheduler form2 = (FormTaskSheduler)client.Tag;
			form2.Invoke((MethodInvoker)delegate
			{
				form2.dataGridView1.Rows.Clear();
				form2.materialLabel1.Text = "Tasks received";
				for (int i = 2; i < objects.Length; i += 4)
				{
					DataGridViewRow dataGridViewRow = new DataGridViewRow
					{
						Cells = 
						{
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = (string)objects[i]
							},
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = (string)objects[i + 1]
							},
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = (string)objects[i + 2]
							},
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = (string)objects[i + 3]
							}
						}
					};
					form2.dataGridView1.Rows.Add(dataGridViewRow);
				}
			});
			break;
		}
		case "Message":
			if (client.Tag != null)
			{
				FormTaskSheduler form3 = (FormTaskSheduler)client.Tag;
				form3.Invoke((MethodInvoker)delegate
				{
					form3.materialLabel1.Text = (string)objects[2];
				});
			}
			break;
		case "Error":
			if (client.Tag != null)
			{
				FormTaskSheduler form = (FormTaskSheduler)client.Tag;
				form.Invoke((MethodInvoker)delegate
				{
					form.materialLabel1.Text = "Error: " + (string)objects[2];
				});
			}
			break;
		}
	}
}
