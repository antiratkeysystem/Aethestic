using System.Collections;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerService
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
		case "Status":
		{
			if (client.Tag == null)
			{
				client.Disconnect();
				return;
			}
			FormService form2 = (FormService)client.Tag;
			form2.Invoke((MethodInvoker)delegate
			{
				form2.materialLabel1.Text = "Succues status";
				foreach (DataGridViewRow dataGridViewRow in (IEnumerable)form2.dataGridView2.Rows)
				{
					if (dataGridViewRow.Cells[1].Value as string == (string)objects[2])
					{
						dataGridViewRow.Cells[1].Value = (string)objects[3];
						break;
					}
				}
			});
			return;
		}
		case "Error":
		{
			if (client.Tag == null)
			{
				client.Disconnect();
				return;
			}
			FormService form3 = (FormService)client.Tag;
			form3.Invoke((MethodInvoker)delegate
			{
				form3.materialLabel1.Text = "Error: " + (string)objects[2];
			});
			return;
		}
		case "Connect":
		{
			FormService form = (FormService)Application.OpenForms["Service:" + (string)objects[2]];
			if (form == null)
			{
				client.Disconnect();
				return;
			}
			form.Invoke((MethodInvoker)delegate
			{
				form.Text = "Service [" + (string)objects[2] + "]";
				form.client = client;
				form.materialLabel1.Enabled = true;
				form.dataGridView2.Enabled = true;
				form.materialLabel1.Text = "Succues Connect";
			});
			client.Tag = form;
			client.Hwid = (string)objects[2];
			return;
		}
		}
		if (!(a == "List"))
		{
			return;
		}
		if (client.Tag == null)
		{
			client.Disconnect();
			return;
		}
		FormService form4 = (FormService)client.Tag;
		form4.Invoke((MethodInvoker)delegate
		{
			form4.dataGridView2.Rows.Clear();
			form4.materialLabel1.Text = "Succues list";
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
						}
					}
				};
				form4.dataGridView2.Rows.Add(dataGridViewRow);
			}
		});
	}
}
