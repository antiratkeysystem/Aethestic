using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Leb128;
using MaterialSkin;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormSniffer : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private bool isPaused;

	private int totalPackets;

	private IContainer components;

	private Timer timer1;

	public DataGridView dataGridView2;

	private SplitContainer splitContainer1;

	private TabControl materialTabControl1;

	private TabPage tabPage1;

	private TabControl materialTabControl2;

	private TabPage tabPage3;

	private TabPage tabPage4;

	private TabPage tabPage2;

	private TabControl materialTabControl3;

	private TabPage tabPage5;

	private TabPage tabPage6;

	private DataGridViewTextBoxColumn Column1;

	private DataGridViewTextBoxColumn Column2;

	private DataGridViewTextBoxColumn Column3;

	private DataGridViewTextBoxColumn Column5;

	private DataGridViewTextBoxColumn Column6;

	private DataGridViewTextBoxColumn Column4;

	public DataGridView dataGridView1;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;

	public DataGridView dataGridView3;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem clearToolStripMenuItem;

	public DataGridView dataGridView4;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;

	public DataGridView dataGridView5;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;

	private DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;

	public FormSniffer()
	{
		InitializeComponent();
	}

	private void FormSniffer_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		DataGridViewTextBoxColumn colProcess = new DataGridViewTextBoxColumn();
		colProcess.HeaderText = "Process";
		colProcess.Name = "ColumnProcess";
		colProcess.ReadOnly = true;
		colProcess.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
		colProcess.Width = 120;
		colProcess.MinimumWidth = 80;
		dataGridView2.Columns.Add(colProcess);
		ChangeScheme(this);
		if (parrent != null)
		{
			Methods.AppendLogs(parrent.IP, "Sniffer form opened. Waiting for plugin connection... (Admin rights required on client)", Color.Orange);
		}
	}

	private void ChangeScheme(object sender)
	{
		bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color back = (isDark ? Color.FromArgb(40, 40, 40) : SystemColors.Control);
		Color gridBack = (isDark ? Color.FromArgb(30, 30, 30) : Color.White);
		Color cellBack = (isDark ? Color.FromArgb(50, 50, 50) : Color.White);
		Color text = (isDark ? Color.WhiteSmoke : SystemColors.ControlText);
		Color primary = FormMaterial.PrimaryColor;
		BackColor = back;
		DataGridView[] array = new DataGridView[5] { dataGridView1, dataGridView2, dataGridView3, dataGridView4, dataGridView5 };
		foreach (DataGridView grid in array)
		{
			if (grid != null)
			{
				grid.BackgroundColor = gridBack;
				grid.GridColor = (isDark ? Color.FromArgb(60, 60, 60) : Color.FromArgb(220, 220, 220));
				grid.DefaultCellStyle.BackColor = cellBack;
				grid.DefaultCellStyle.ForeColor = primary;
				grid.DefaultCellStyle.SelectionBackColor = primary;
				grid.DefaultCellStyle.SelectionForeColor = Color.White;
				grid.AlternatingRowsDefaultCellStyle.BackColor = cellBack;
				grid.AlternatingRowsDefaultCellStyle.ForeColor = primary;
				grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = primary;
				grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;
				grid.ColumnHeadersDefaultCellStyle.BackColor = (isDark ? Color.FromArgb(35, 35, 35) : Color.FromArgb(240, 240, 240));
				grid.ColumnHeadersDefaultCellStyle.ForeColor = primary;
				grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = (isDark ? Color.FromArgb(35, 35, 35) : Color.FromArgb(240, 240, 240));
				grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = primary;
				grid.EnableHeadersVisualStyles = false;
				grid.BorderStyle = BorderStyle.None;
			}
		}
		contextMenuStrip1.BackColor = back;
		contextMenuStrip1.ForeColor = text;
		foreach (ToolStripItem item in contextMenuStrip1.Items)
		{
			item.BackColor = back;
			item.ForeColor = text;
		}
	}

	public void AddPacket(string method, string url, string status, string type, string size, string headers, string raw, string index, string time)
	{
		AddPacket(method, url, status, type, size, headers, raw, index, time, "");
	}

	public void AddPacket(string method, string url, string status, string type, string size, string headers, string raw, string index, string time, string process)
	{
		bool isAtBottom = IsScrolledToBottom();
		totalPackets++;
		int rowIdx = dataGridView2.Rows.Add();
		DataGridViewRow dataGridViewRow = dataGridView2.Rows[rowIdx];
		dataGridViewRow.Cells[0].Value = totalPackets;
		dataGridViewRow.Cells[1].Value = method;
		dataGridViewRow.Cells[2].Value = url;
		dataGridViewRow.Cells[3].Value = status;
		dataGridViewRow.Cells[4].Value = type;
		dataGridViewRow.Cells[5].Value = size;
		dataGridViewRow.Cells[6].Value = process;
		dataGridViewRow.Tag = new object[3] { headers, raw, time };
		if (isAtBottom && dataGridView2.Rows.Count > 0)
		{
			dataGridView2.FirstDisplayedScrollingRowIndex = dataGridView2.Rows.Count - 1;
		}
	}

	private bool IsScrolledToBottom()
	{
		if (dataGridView2.Rows.Count == 0)
		{
			return true;
		}
		if (dataGridView2.DisplayedRowCount(includePartialRow: false) == 0)
		{
			return true;
		}
		int firstDisplayedScrollingRowIndex = dataGridView2.FirstDisplayedScrollingRowIndex;
		int displayedCount = dataGridView2.DisplayedRowCount(includePartialRow: false);
		int totalRows = dataGridView2.Rows.Count;
		return firstDisplayedScrollingRowIndex + displayedCount >= totalRows - 2;
	}

	private void dataGridView2_SelectionChanged(object sender, EventArgs e)
	{
		try
		{
			if (dataGridView2.SelectedRows.Count <= 0)
			{
				return;
			}
			DataGridViewRow row = dataGridView2.SelectedRows[0];
			if (row.Tag == null)
			{
				return;
			}
			object[] data = (object[])row.Tag;
			string headers = (string)data[0];
			string raw = (string)data[1];
			string time = ((data.Length > 2) ? ((string)data[2]) : "");
			dataGridView1.Rows.Clear();
			dataGridView4.Rows.Clear();
			if (!string.IsNullOrEmpty(headers))
			{
				string[] array = headers.Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string line in array)
				{
					int colonIdx = line.IndexOf(':');
					if (colonIdx > 0)
					{
						string key = line.Substring(0, colonIdx).Trim();
						string val = line.Substring(colonIdx + 1).Trim();
						dataGridView1.Rows.Add(key, val);
						dataGridView4.Rows.Add(key, val);
					}
					else
					{
						dataGridView1.Rows.Add("Info", line.Trim());
						dataGridView4.Rows.Add("Info", line.Trim());
					}
				}
				if (!string.IsNullOrEmpty(time))
				{
					dataGridView1.Rows.Add("Capture Time", time);
					dataGridView4.Rows.Add("Capture Time", time);
				}
			}
			dataGridView3.Rows.Clear();
			dataGridView5.Rows.Clear();
			if (!string.IsNullOrEmpty(raw))
			{
				string[] array2 = SplitHexDump(raw, 48);
				int offset = 0;
				string[] array = array2;
				foreach (string hexLine in array)
				{
					string offsetStr = $"0x{offset:X4}";
					dataGridView3.Rows.Add(offsetStr, hexLine);
					dataGridView5.Rows.Add(offsetStr, hexLine);
					offset += 16;
				}
			}
			else
			{
				dataGridView3.Rows.Add("DATA", "[No payload data]");
				dataGridView5.Rows.Add("DATA", "[No payload data]");
			}
		}
		catch
		{
		}
	}

	private string[] SplitHexDump(string hex, int charsPerLine)
	{
		List<string> lines = new List<string>();
		hex = hex.Trim();
		for (int i = 0; i < hex.Length; i += charsPerLine)
		{
			int len = Math.Min(charsPerLine, hex.Length - i);
			lines.Add(hex.Substring(i, len).Trim());
		}
		return lines.ToArray();
	}

	private void clearToolStripMenuItem_Click(object sender, EventArgs e)
	{
		dataGridView2.Rows.Clear();
		dataGridView1.Rows.Clear();
		dataGridView3.Rows.Clear();
		dataGridView4.Rows.Clear();
		dataGridView5.Rows.Clear();
		totalPackets = 0;
	}

	public void TogglePause()
	{
		isPaused = !isPaused;
		if (client != null)
		{
			string cmd = (isPaused ? "Pause" : "Resume");
			client.Send(LEB128.Write(new object[2] { "Sniffer", cmd }));
		}
	}

	public void SendFilter(bool http, bool https, bool tcp, bool udp)
	{
		if (client != null)
		{
			client.Send(LEB128.Write(new object[6] { "Sniffer", "Filter", http, https, tcp, udp }));
		}
	}

	private void FormSniffer_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (client != null)
		{
			client.Send(LEB128.Write(new object[2] { "Sniffer", "Stop" }));
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
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormSniffer));
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.dataGridView2 = new System.Windows.Forms.DataGridView();
		this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.materialTabControl1 = new System.Windows.Forms.TabControl();
		this.tabPage1 = new System.Windows.Forms.TabPage();
		this.materialTabControl2 = new System.Windows.Forms.TabControl();
		this.tabPage3 = new System.Windows.Forms.TabPage();
		this.dataGridView1 = new System.Windows.Forms.DataGridView();
		this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.tabPage4 = new System.Windows.Forms.TabPage();
		this.dataGridView3 = new System.Windows.Forms.DataGridView();
		this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.tabPage2 = new System.Windows.Forms.TabPage();
		this.materialTabControl3 = new System.Windows.Forms.TabControl();
		this.tabPage5 = new System.Windows.Forms.TabPage();
		this.dataGridView4 = new System.Windows.Forms.DataGridView();
		this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.tabPage6 = new System.Windows.Forms.TabPage();
		this.dataGridView5 = new System.Windows.Forms.DataGridView();
		this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		((System.ComponentModel.ISupportInitialize)this.dataGridView2).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		this.materialTabControl1.SuspendLayout();
		this.tabPage1.SuspendLayout();
		this.materialTabControl2.SuspendLayout();
		this.tabPage3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dataGridView1).BeginInit();
		this.tabPage4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dataGridView3).BeginInit();
		this.tabPage2.SuspendLayout();
		this.materialTabControl3.SuspendLayout();
		this.tabPage5.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dataGridView4).BeginInit();
		this.tabPage6.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dataGridView5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		base.SuspendLayout();
		this.timer1.Interval = 1000;
		this.dataGridView2.AllowDrop = true;
		this.dataGridView2.AllowUserToAddRows = false;
		this.dataGridView2.AllowUserToDeleteRows = false;
		this.dataGridView2.AllowUserToResizeColumns = false;
		this.dataGridView2.AllowUserToResizeRows = false;
		dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(65, 105, 225);
		dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
		this.dataGridView2.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
		this.dataGridView2.BackgroundColor = System.Drawing.Color.White;
		this.dataGridView2.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dataGridView2.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.dataGridView2.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
		dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView2.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
		this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dataGridView2.Columns.AddRange(this.Column1, this.Column2, this.Column3, this.Column5, this.Column6, this.Column4);
		this.dataGridView2.ContextMenuStrip = this.contextMenuStrip1;
		this.dataGridView2.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(65, 105, 225);
		dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dataGridView2.DefaultCellStyle = dataGridViewCellStyle3;
		this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dataGridView2.EnableHeadersVisualStyles = false;
		this.dataGridView2.GridColor = System.Drawing.Color.FromArgb(17, 17, 17);
		this.dataGridView2.Location = new System.Drawing.Point(0, 0);
		this.dataGridView2.Name = "dataGridView2";
		this.dataGridView2.ReadOnly = true;
		this.dataGridView2.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
		dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView2.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
		this.dataGridView2.RowHeadersVisible = false;
		this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dataGridView2.ShowCellErrors = false;
		this.dataGridView2.ShowCellToolTips = false;
		this.dataGridView2.ShowEditingIcon = false;
		this.dataGridView2.ShowRowErrors = false;
		this.dataGridView2.Size = new System.Drawing.Size(1033, 289);
		this.dataGridView2.TabIndex = 19;
		this.dataGridView2.SelectionChanged += new System.EventHandler(dataGridView2_SelectionChanged);
		this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.Column1.HeaderText = "#";
		this.Column1.MinimumWidth = 50;
		this.Column1.Name = "Column1";
		this.Column1.ReadOnly = true;
		this.Column1.Width = 50;
		this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.Column2.HeaderText = "Method";
		this.Column2.MinimumWidth = 80;
		this.Column2.Name = "Column2";
		this.Column2.ReadOnly = true;
		this.Column2.Width = 80;
		this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.Column3.HeaderText = "Url";
		this.Column3.MinimumWidth = 200;
		this.Column3.Name = "Column3";
		this.Column3.ReadOnly = true;
		this.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.Column5.HeaderText = "Status";
		this.Column5.MinimumWidth = 100;
		this.Column5.Name = "Column5";
		this.Column5.ReadOnly = true;
		this.Column6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.Column6.HeaderText = "Type";
		this.Column6.MinimumWidth = 150;
		this.Column6.Name = "Column6";
		this.Column6.ReadOnly = true;
		this.Column6.Width = 150;
		this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.Column4.HeaderText = "Size";
		this.Column4.MinimumWidth = 80;
		this.Column4.Name = "Column4";
		this.Column4.ReadOnly = true;
		this.Column4.Width = 80;
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.clearToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(102, 26);
		this.clearToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("clearToolStripMenuItem.Image");
		this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
		this.clearToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
		this.clearToolStripMenuItem.Text = "Clear";
		this.clearToolStripMenuItem.Click += new System.EventHandler(clearToolStripMenuItem_Click);
		this.materialTabControl1.Controls.Add(this.tabPage1);
		this.materialTabControl1.Controls.Add(this.tabPage2);
		this.materialTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.materialTabControl1.Location = new System.Drawing.Point(0, 0);
		this.materialTabControl1.Multiline = true;
		this.materialTabControl1.Name = "materialTabControl1";
		this.materialTabControl1.SelectedIndex = 0;
		this.materialTabControl1.Size = new System.Drawing.Size(1033, 319);
		this.materialTabControl1.TabIndex = 20;
		this.tabPage1.BackColor = System.Drawing.Color.White;
		this.tabPage1.Controls.Add(this.materialTabControl2);
		this.tabPage1.ForeColor = System.Drawing.Color.Black;
		this.tabPage1.Location = new System.Drawing.Point(4, 22);
		this.tabPage1.Name = "tabPage1";
		this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage1.Size = new System.Drawing.Size(1025, 293);
		this.tabPage1.TabIndex = 0;
		this.tabPage1.Text = "Response";
		this.materialTabControl2.Controls.Add(this.tabPage3);
		this.materialTabControl2.Controls.Add(this.tabPage4);
		this.materialTabControl2.Location = new System.Drawing.Point(6, 6);
		this.materialTabControl2.Multiline = true;
		this.materialTabControl2.Name = "materialTabControl2";
		this.materialTabControl2.SelectedIndex = 0;
		this.materialTabControl2.Size = new System.Drawing.Size(1013, 281);
		this.materialTabControl2.TabIndex = 0;
		this.materialTabControl2.Tag = "";
		this.tabPage3.BackColor = System.Drawing.Color.White;
		this.tabPage3.Controls.Add(this.dataGridView1);
		this.tabPage3.ForeColor = System.Drawing.Color.Black;
		this.tabPage3.Location = new System.Drawing.Point(4, 22);
		this.tabPage3.Name = "tabPage3";
		this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage3.Size = new System.Drawing.Size(1005, 255);
		this.tabPage3.TabIndex = 0;
		this.tabPage3.Text = "Header";
		this.dataGridView1.AllowDrop = true;
		this.dataGridView1.AllowUserToAddRows = false;
		this.dataGridView1.AllowUserToDeleteRows = false;
		this.dataGridView1.AllowUserToResizeColumns = false;
		this.dataGridView1.AllowUserToResizeRows = false;
		dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(65, 105, 225);
		dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White;
		this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
		this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
		this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
		dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle6.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.White;
		dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
		this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dataGridView1.Columns.AddRange(this.dataGridViewTextBoxColumn1, this.dataGridViewTextBoxColumn2);
		this.dataGridView1.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle7.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle7.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle7.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(65, 105, 225);
		dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle7;
		this.dataGridView1.EnableHeadersVisualStyles = false;
		this.dataGridView1.GridColor = System.Drawing.Color.FromArgb(17, 17, 17);
		this.dataGridView1.Location = new System.Drawing.Point(0, 0);
		this.dataGridView1.Name = "dataGridView1";
		this.dataGridView1.ReadOnly = true;
		this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		dataGridViewCellStyle8.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle8.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
		dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
		this.dataGridView1.RowHeadersVisible = false;
		this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dataGridView1.ShowCellErrors = false;
		this.dataGridView1.ShowCellToolTips = false;
		this.dataGridView1.ShowEditingIcon = false;
		this.dataGridView1.ShowRowErrors = false;
		this.dataGridView1.Size = new System.Drawing.Size(1005, 259);
		this.dataGridView1.TabIndex = 22;
		this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.dataGridViewTextBoxColumn1.HeaderText = "Header";
		this.dataGridViewTextBoxColumn1.MinimumWidth = 150;
		this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
		this.dataGridViewTextBoxColumn1.ReadOnly = true;
		this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.dataGridViewTextBoxColumn2.HeaderText = "Value";
		this.dataGridViewTextBoxColumn2.MinimumWidth = 100;
		this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
		this.dataGridViewTextBoxColumn2.ReadOnly = true;
		this.tabPage4.BackColor = System.Drawing.Color.White;
		this.tabPage4.Controls.Add(this.dataGridView3);
		this.tabPage4.ForeColor = System.Drawing.Color.Black;
		this.tabPage4.Location = new System.Drawing.Point(4, 22);
		this.tabPage4.Name = "tabPage4";
		this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage4.Size = new System.Drawing.Size(1005, 255);
		this.tabPage4.TabIndex = 1;
		this.tabPage4.Text = "Raw";
		this.dataGridView3.AllowDrop = true;
		this.dataGridView3.AllowUserToAddRows = false;
		this.dataGridView3.AllowUserToDeleteRows = false;
		this.dataGridView3.AllowUserToResizeColumns = false;
		this.dataGridView3.AllowUserToResizeRows = false;
		dataGridViewCellStyle9.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle9.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(65, 105, 225);
		dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.White;
		this.dataGridView3.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle9;
		this.dataGridView3.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dataGridView3.BackgroundColor = System.Drawing.Color.White;
		this.dataGridView3.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dataGridView3.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.dataGridView3.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
		dataGridViewCellStyle10.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle10.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle10.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.White;
		dataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView3.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
		this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dataGridView3.Columns.AddRange(this.dataGridViewTextBoxColumn3, this.dataGridViewTextBoxColumn4);
		this.dataGridView3.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle11.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle11.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle11.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.FromArgb(65, 105, 225);
		dataGridViewCellStyle11.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dataGridView3.DefaultCellStyle = dataGridViewCellStyle11;
		this.dataGridView3.EnableHeadersVisualStyles = false;
		this.dataGridView3.GridColor = System.Drawing.Color.FromArgb(17, 17, 17);
		this.dataGridView3.Location = new System.Drawing.Point(3, 6);
		this.dataGridView3.Name = "dataGridView3";
		this.dataGridView3.ReadOnly = true;
		this.dataGridView3.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle12.BackColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		dataGridViewCellStyle12.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle12.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
		dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView3.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
		this.dataGridView3.RowHeadersVisible = false;
		this.dataGridView3.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dataGridView3.ShowCellErrors = false;
		this.dataGridView3.ShowCellToolTips = false;
		this.dataGridView3.ShowEditingIcon = false;
		this.dataGridView3.ShowRowErrors = false;
		this.dataGridView3.Size = new System.Drawing.Size(1005, 243);
		this.dataGridView3.TabIndex = 23;
		this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.dataGridViewTextBoxColumn3.HeaderText = "Raw";
		this.dataGridViewTextBoxColumn3.MinimumWidth = 150;
		this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
		this.dataGridViewTextBoxColumn3.ReadOnly = true;
		this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.dataGridViewTextBoxColumn4.HeaderText = "Value";
		this.dataGridViewTextBoxColumn4.MinimumWidth = 100;
		this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
		this.dataGridViewTextBoxColumn4.ReadOnly = true;
		this.tabPage2.BackColor = System.Drawing.Color.White;
		this.tabPage2.Controls.Add(this.materialTabControl3);
		this.tabPage2.ForeColor = System.Drawing.Color.Black;
		this.tabPage2.Location = new System.Drawing.Point(4, 22);
		this.tabPage2.Name = "tabPage2";
		this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage2.Size = new System.Drawing.Size(1025, 293);
		this.tabPage2.TabIndex = 1;
		this.tabPage2.Text = "Request";
		this.materialTabControl3.Controls.Add(this.tabPage5);
		this.materialTabControl3.Controls.Add(this.tabPage6);
		this.materialTabControl3.Location = new System.Drawing.Point(6, 6);
		this.materialTabControl3.Multiline = true;
		this.materialTabControl3.Name = "materialTabControl3";
		this.materialTabControl3.SelectedIndex = 0;
		this.materialTabControl3.Size = new System.Drawing.Size(1013, 281);
		this.materialTabControl3.TabIndex = 1;
		this.materialTabControl3.Tag = "";
		this.tabPage5.BackColor = System.Drawing.Color.White;
		this.tabPage5.Controls.Add(this.dataGridView4);
		this.tabPage5.ForeColor = System.Drawing.Color.Black;
		this.tabPage5.Location = new System.Drawing.Point(4, 22);
		this.tabPage5.Name = "tabPage5";
		this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage5.Size = new System.Drawing.Size(1005, 255);
		this.tabPage5.TabIndex = 0;
		this.tabPage5.Text = "Header";
		this.dataGridView4.AllowDrop = true;
		this.dataGridView4.AllowUserToAddRows = false;
		this.dataGridView4.AllowUserToDeleteRows = false;
		this.dataGridView4.AllowUserToResizeColumns = false;
		this.dataGridView4.AllowUserToResizeRows = false;
		dataGridViewCellStyle13.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle13.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle13.SelectionBackColor = System.Drawing.Color.FromArgb(65, 105, 225);
		dataGridViewCellStyle13.SelectionForeColor = System.Drawing.Color.White;
		this.dataGridView4.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle13;
		this.dataGridView4.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dataGridView4.BackgroundColor = System.Drawing.Color.White;
		this.dataGridView4.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dataGridView4.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.dataGridView4.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
		dataGridViewCellStyle14.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle14.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle14.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle14.SelectionBackColor = System.Drawing.Color.White;
		dataGridViewCellStyle14.SelectionForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView4.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle14;
		this.dataGridView4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dataGridView4.Columns.AddRange(this.dataGridViewTextBoxColumn5, this.dataGridViewTextBoxColumn6);
		this.dataGridView4.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle15.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle15.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle15.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.FromArgb(65, 105, 225);
		dataGridViewCellStyle15.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dataGridView4.DefaultCellStyle = dataGridViewCellStyle15;
		this.dataGridView4.EnableHeadersVisualStyles = false;
		this.dataGridView4.GridColor = System.Drawing.Color.FromArgb(17, 17, 17);
		this.dataGridView4.Location = new System.Drawing.Point(0, -2);
		this.dataGridView4.Name = "dataGridView4";
		this.dataGridView4.ReadOnly = true;
		this.dataGridView4.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle16.BackColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		dataGridViewCellStyle16.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle16.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
		dataGridViewCellStyle16.SelectionBackColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle16.SelectionForeColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView4.RowHeadersDefaultCellStyle = dataGridViewCellStyle16;
		this.dataGridView4.RowHeadersVisible = false;
		this.dataGridView4.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dataGridView4.ShowCellErrors = false;
		this.dataGridView4.ShowCellToolTips = false;
		this.dataGridView4.ShowEditingIcon = false;
		this.dataGridView4.ShowRowErrors = false;
		this.dataGridView4.Size = new System.Drawing.Size(1005, 222);
		this.dataGridView4.TabIndex = 23;
		this.dataGridViewTextBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.dataGridViewTextBoxColumn5.HeaderText = "Header";
		this.dataGridViewTextBoxColumn5.MinimumWidth = 150;
		this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
		this.dataGridViewTextBoxColumn5.ReadOnly = true;
		this.dataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.dataGridViewTextBoxColumn6.HeaderText = "Value";
		this.dataGridViewTextBoxColumn6.MinimumWidth = 100;
		this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
		this.dataGridViewTextBoxColumn6.ReadOnly = true;
		this.tabPage6.BackColor = System.Drawing.Color.White;
		this.tabPage6.Controls.Add(this.dataGridView5);
		this.tabPage6.ForeColor = System.Drawing.Color.Black;
		this.tabPage6.Location = new System.Drawing.Point(4, 22);
		this.tabPage6.Name = "tabPage6";
		this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage6.Size = new System.Drawing.Size(1005, 255);
		this.tabPage6.TabIndex = 1;
		this.tabPage6.Text = "Raw";
		this.dataGridView5.AllowDrop = true;
		this.dataGridView5.AllowUserToAddRows = false;
		this.dataGridView5.AllowUserToDeleteRows = false;
		this.dataGridView5.AllowUserToResizeColumns = false;
		this.dataGridView5.AllowUserToResizeRows = false;
		dataGridViewCellStyle17.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle17.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle17.SelectionBackColor = System.Drawing.Color.FromArgb(65, 105, 225);
		dataGridViewCellStyle17.SelectionForeColor = System.Drawing.Color.White;
		this.dataGridView5.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle17;
		this.dataGridView5.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dataGridView5.BackgroundColor = System.Drawing.Color.White;
		this.dataGridView5.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dataGridView5.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.dataGridView5.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
		dataGridViewCellStyle18.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle18.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle18.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle18.SelectionBackColor = System.Drawing.Color.White;
		dataGridViewCellStyle18.SelectionForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView5.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle18;
		this.dataGridView5.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dataGridView5.Columns.AddRange(this.dataGridViewTextBoxColumn7, this.dataGridViewTextBoxColumn8);
		this.dataGridView5.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle19.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle19.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle19.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle19.SelectionBackColor = System.Drawing.Color.FromArgb(65, 105, 225);
		dataGridViewCellStyle19.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle19.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dataGridView5.DefaultCellStyle = dataGridViewCellStyle19;
		this.dataGridView5.EnableHeadersVisualStyles = false;
		this.dataGridView5.GridColor = System.Drawing.Color.FromArgb(17, 17, 17);
		this.dataGridView5.Location = new System.Drawing.Point(0, -2);
		this.dataGridView5.Name = "dataGridView5";
		this.dataGridView5.ReadOnly = true;
		this.dataGridView5.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle20.BackColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		dataGridViewCellStyle20.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle20.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
		dataGridViewCellStyle20.SelectionBackColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle20.SelectionForeColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle20.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView5.RowHeadersDefaultCellStyle = dataGridViewCellStyle20;
		this.dataGridView5.RowHeadersVisible = false;
		this.dataGridView5.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dataGridView5.ShowCellErrors = false;
		this.dataGridView5.ShowCellToolTips = false;
		this.dataGridView5.ShowEditingIcon = false;
		this.dataGridView5.ShowRowErrors = false;
		this.dataGridView5.Size = new System.Drawing.Size(1005, 254);
		this.dataGridView5.TabIndex = 24;
		this.dataGridViewTextBoxColumn7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.dataGridViewTextBoxColumn7.HeaderText = "Raw";
		this.dataGridViewTextBoxColumn7.MinimumWidth = 150;
		this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
		this.dataGridViewTextBoxColumn7.ReadOnly = true;
		this.dataGridViewTextBoxColumn8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.dataGridViewTextBoxColumn8.HeaderText = "Value";
		this.dataGridViewTextBoxColumn8.MinimumWidth = 100;
		this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
		this.dataGridViewTextBoxColumn8.ReadOnly = true;
		this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainer1.Location = new System.Drawing.Point(3, 64);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this.splitContainer1.Panel1.Controls.Add(this.dataGridView2);
		this.splitContainer1.Panel2.Controls.Add(this.materialTabControl1);
		this.splitContainer1.Size = new System.Drawing.Size(1033, 612);
		this.splitContainer1.SplitterDistance = 289;
		this.splitContainer1.TabIndex = 25;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1039, 679);
		base.Controls.Add(this.splitContainer1);
		base.Name = "FormSniffer";
		this.Text = "Sniffer";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(FormSniffer_FormClosing);
		base.Load += new System.EventHandler(FormSniffer_Load);
		((System.ComponentModel.ISupportInitialize)this.dataGridView2).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		this.materialTabControl1.ResumeLayout(false);
		this.tabPage1.ResumeLayout(false);
		this.materialTabControl2.ResumeLayout(false);
		this.tabPage3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dataGridView1).EndInit();
		this.tabPage4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dataGridView3).EndInit();
		this.tabPage2.ResumeLayout(false);
		this.materialTabControl3.ResumeLayout(false);
		this.tabPage5.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dataGridView4).EndInit();
		this.tabPage6.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dataGridView5).EndInit();
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
		this.splitContainer1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
