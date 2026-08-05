using System;
using System.Collections.Concurrent;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerEventLog
{
	private static readonly ConcurrentDictionary<string, int> _listCount = new ConcurrentDictionary<string, int>();

	private static readonly ConcurrentDictionary<string, DateTime> _listWindowStart = new ConcurrentDictionary<string, DateTime>();

	private const int MaxListsPerWindow = 5;

	private const int MaxEventLogRows = 5000;

	public static void Read(Clients client, object[] objects)
	{
		if (objects.Length < 2)
		{
			return;
		}
		string command = (string)objects[1];
		string clientKey = client.IP + ":eventlog";
		DateTime now = DateTime.UtcNow;
		if (command == "List")
		{
			if (_listWindowStart.TryGetValue(clientKey, out var windowStart) && (now - windowStart).TotalSeconds < 10.0)
			{
				if (_listCount.AddOrUpdate(clientKey, 1, (string k, int v) => v + 1) > 5)
				{
					return;
				}
			}
			else
			{
				_listWindowStart[clientKey] = now;
				_listCount[clientKey] = 1;
			}
		}
		switch (command)
		{
		case "Ready":
		{
			FormEventlog form6 = null;
			string hwid = null;
			if (!string.IsNullOrEmpty(client.Hwid))
			{
				form6 = (FormEventlog)Application.OpenForms["EventLog:" + client.Hwid];
				if (form6 != null)
				{
					hwid = client.Hwid;
				}
			}
			if (form6 == null)
			{
				foreach (Form openForm in Application.OpenForms)
				{
					if (!(openForm is FormEventlog eventLogForm) || !openForm.Name.StartsWith("EventLog:"))
					{
						continue;
					}
					string formHwid = openForm.Name.Substring(9);
					if (objects.Length >= 3 && objects[2] is string receivedHwid && string.Equals(formHwid, receivedHwid, StringComparison.OrdinalIgnoreCase))
					{
						form6 = eventLogForm;
						hwid = formHwid;
						if (!string.IsNullOrEmpty(hwid) && string.IsNullOrEmpty(client.Hwid))
						{
							client.Hwid = hwid;
						}
						break;
					}
				}
			}
			if (form6 == null)
			{
				break;
			}
			if (string.IsNullOrEmpty(client.Hwid) && !string.IsNullOrEmpty(hwid))
			{
				client.Hwid = hwid;
			}
			form6.Invoke((MethodInvoker)delegate
			{
				form6.client = client;
				if (!string.IsNullOrEmpty(hwid))
				{
					form6.Text = "EventLog [" + hwid + "]";
				}
				form6.materialLabel1.Enabled = true;
				form6.dataGridView2.Enabled = true;
				form6.materialLabel1.Text = "Success Connect";
			});
			client.Tag = form6;
			break;
		}
		case "Error":
		{
			if (client.Tag == null)
			{
				client.Disconnect();
				break;
			}
			FormEventlog form5 = (FormEventlog)client.Tag;
			string errMsg = ((objects.Length > 2) ? ((string)objects[2]) : "Unknown error");
			if (errMsg != null && errMsg.Length > 300)
			{
				errMsg = errMsg.Substring(0, 300) + "...";
			}
			form5.Invoke((MethodInvoker)delegate
			{
				form5.materialLabel1.Text = "Error: " + errMsg;
			});
			break;
		}
		case "List":
		{
			if (client.Tag == null)
			{
				client.Disconnect();
				break;
			}
			FormEventlog form4 = (FormEventlog)client.Tag;
			form4.Invoke((MethodInvoker)delegate
			{
				form4.dataGridView2.Rows.Clear();
				form4.materialLabel1.Text = "Success list";
				int num = 2;
				int num2 = 0;
				while (num < objects.Length && num2 < 5000 && num + 4 < objects.Length)
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
					num2++;
				}
			});
			break;
		}
		}
	}
}
