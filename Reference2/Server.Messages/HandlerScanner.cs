using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;
using Server.Helper;

namespace Server.Messages;

internal class HandlerScanner
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
			if (objects.Length < 3 || !(objects[2] is string))
			{
				break;
			}
			string hwid = SecurityHelper.SanitizeHwid((string)objects[2]);
			FormScanner form2 = (FormScanner)Application.OpenForms["Scanner:" + hwid];
			if (form2 == null)
			{
				client.Disconnect();
				break;
			}
			form2.Invoke((MethodInvoker)delegate
			{
				form2.Text = "Scanner [" + hwid + "]";
				form2.client = client;
				form2.materialLabel1.Enabled = true;
				form2.dataGridView2.Enabled = true;
				form2.materialLabel1.Text = "Connected successfully";
			});
			client.Tag = form2;
			client.Hwid = hwid;
			break;
		}
		case "List":
		{
			if (client.Tag == null)
			{
				break;
			}
			FormScanner form3 = (FormScanner)client.Tag;
			form3.Invoke((MethodInvoker)delegate
			{
				form3.dataGridView2.Rows.Clear();
				form3.materialLabel1.Text = "Data received";
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
								Value = (int)objects[num++]
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
					form3.dataGridView2.Rows.Add(dataGridViewRow);
				}
			});
			break;
		}
		case "Error":
			if (client.Tag != null)
			{
				FormScanner form = (FormScanner)client.Tag;
				form.Invoke((MethodInvoker)delegate
				{
					form.materialLabel1.Text = "Error: " + (string)objects[2];
				});
			}
			break;
		}
	}
}
