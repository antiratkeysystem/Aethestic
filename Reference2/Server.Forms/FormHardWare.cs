using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormHardWare : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	private Timer timer1;

	private TableLayoutPanel tableLayoutPanel1;

	private Panel panelDisk;

	private Label labelDisk;

	public DataGridView dataGridView2;

	private DataGridViewTextBoxColumn Column1;

	private DataGridViewTextBoxColumn Column2;

	private DataGridViewTextBoxColumn Column3;

	private DataGridViewTextBoxColumn Column4;

	private DataGridViewTextBoxColumn Column5;

	private DataGridViewTextBoxColumn Column6;

	private DataGridViewTextBoxColumn Column7;

	private ContextMenuStrip contextMenuDisk;

	private ToolStripMenuItem enableDiskToolStripMenuItem;

	private ToolStripMenuItem disableDiskToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripMenuItem startToolStripMenuItem;

	private ToolStripMenuItem pauseToolStripMenuItem;

	private ToolStripMenuItem enableDisabledDiskToolStripMenuItem;

	private ToolStripMenuItem lockDriveToolStripMenuItem;

	private ToolStripMenuItem unlockDriveToolStripMenuItem;

	private ToolStripSeparator dToolStripMenuItem1;

	private ToolStripMenuItem stopToolStripMenuItem;

	private ToolStripMenuItem killRemoveToolStripMenuItem;

	private Panel panelRam;

	private Label labelRam;

	public DataGridView gridRam;

	private DataGridViewTextBoxColumn colRamSlot;

	private DataGridViewTextBoxColumn colRamCapacity;

	private DataGridViewTextBoxColumn colRamSpeed;

	private DataGridViewTextBoxColumn colRamType;

	private ContextMenuStrip contextMenuRam;

	private ToolStripMenuItem clearWorkingSetToolStripMenuItem;

	private ToolStripMenuItem refreshRamToolStripMenuItem;

	private Panel panelCpu;

	private Label labelCpu;

	public DataGridView gridCpu;

	private DataGridViewTextBoxColumn colCpuName;

	private DataGridViewTextBoxColumn colCpuCores;

	private DataGridViewTextBoxColumn colCpuThreads;

	private DataGridViewTextBoxColumn colCpuClock;

	private ContextMenuStrip contextMenuCpu;

	private ToolStripMenuItem stressTestToolStripMenuItem;

	private ToolStripMenuItem stopStressToolStripMenuItem;

	private ToolStripMenuItem refreshCpuToolStripMenuItem;

	private Panel panelGpu;

	private Label labelGpu;

	public DataGridView gridGpu;

	private DataGridViewTextBoxColumn colGpuName;

	private DataGridViewTextBoxColumn colGpuMemory;

	private DataGridViewTextBoxColumn colGpuDriver;

	private DataGridViewTextBoxColumn colGpuStatus;

	private ContextMenuStrip contextMenuGpu;

	private ToolStripMenuItem disableGpuToolStripMenuItem;

	private ToolStripMenuItem enableGpuToolStripMenuItem;

	private ToolStripMenuItem refreshGpuToolStripMenuItem;

	private Panel panel1;

	public MaterialLabel materialLabel1;

	public FormHardWare()
	{
		InitializeComponent();
	}

	private void FormHardWare_Load(object sender, EventArgs e)
	{
		timer1.Start();
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		try
		{
			typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty, null, dataGridView2, new object[1] { true });
			typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty, null, gridCpu, new object[1] { true });
			typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty, null, gridGpu, new object[1] { true });
			typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty, null, gridRam, new object[1] { true });
		}
		catch
		{
		}
	}

	private void ChangeScheme(object sender)
	{
		if (!base.IsDisposed && base.IsHandleCreated && dataGridView2 != null && gridRam != null && gridCpu != null && gridGpu != null)
		{
			bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
			Color back = (isDark ? Color.FromArgb(40, 40, 40) : Color.White);
			Color fore = (isDark ? Color.WhiteSmoke : Color.Black);
			Color selBack = FormMaterial.PrimaryColor;
			Color selFore = Color.White;
			ApplyGridTheme(dataGridView2, back, fore, selBack, selFore, isDark);
			ApplyGridTheme(gridRam, back, fore, selBack, selFore, isDark);
			ApplyGridTheme(gridCpu, back, fore, selBack, selFore, isDark);
			ApplyGridTheme(gridGpu, back, fore, selBack, selFore, isDark);
			labelDisk.ForeColor = FormMaterial.PrimaryColor;
			labelRam.ForeColor = FormMaterial.PrimaryColor;
			labelCpu.ForeColor = FormMaterial.PrimaryColor;
			labelGpu.ForeColor = FormMaterial.PrimaryColor;
		}
	}

	private void ApplyGridTheme(DataGridView grid, Color back, Color fore, Color selBack, Color selFore, bool isDark)
	{
		grid.BackgroundColor = back;
		grid.DefaultCellStyle.BackColor = back;
		grid.DefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
		grid.DefaultCellStyle.SelectionBackColor = selBack;
		grid.DefaultCellStyle.SelectionForeColor = selFore;
		if (grid.AlternatingRowsDefaultCellStyle != null)
		{
			grid.AlternatingRowsDefaultCellStyle.BackColor = back;
			grid.AlternatingRowsDefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
			grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = selBack;
			grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = selFore;
		}
		if (grid.ColumnHeadersDefaultCellStyle != null)
		{
			grid.ColumnHeadersDefaultCellStyle.BackColor = back;
			grid.ColumnHeadersDefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
			grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = back;
			grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = FormMaterial.PrimaryColor;
		}
		grid.GridColor = (isDark ? Color.FromArgb(60, 60, 60) : Color.FromArgb(240, 240, 240));
	}

	private void FormHardWare_FormClosing(object sender, FormClosingEventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged -= ChangeScheme;
		if (client != null)
		{
			client.Disconnect();
		}
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		if (parrent != null && !parrent.itsConnect)
		{
			Close();
		}
		if (client != null && !client.itsConnect)
		{
			Close();
		}
	}

	private string GetSelectedDriveLetter()
	{
		if (dataGridView2.SelectedRows.Count == 0)
		{
			return null;
		}
		DataGridViewRow row = dataGridView2.SelectedRows[0];
		if (row.Cells.Count == 0 || row.Cells[0].Value == null)
		{
			return null;
		}
		string s = row.Cells[0].Value.ToString().TrimEnd(':', '\\').Trim();
		if (s.Length != 1)
		{
			return null;
		}
		return s;
	}

	private void startToolStripMenuItem_Click(object sender, EventArgs e)
	{
		string letter = GetSelectedDriveLetter();
		if (!string.IsNullOrEmpty(letter) && client != null && client.itsConnect)
		{
			client.Send(new object[3] { "SetDriveVisible", letter, true });
			materialLabel1.Text = "Showing drive " + letter + "...";
		}
	}

	private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
	{
		string letter = GetSelectedDriveLetter();
		if (!string.IsNullOrEmpty(letter) && client != null && client.itsConnect)
		{
			client.Send(new object[3] { "SetDriveVisible", letter, false });
			materialLabel1.Text = "Hiding drive " + letter + "...";
		}
	}

	private void killRemoveToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(new object[1] { "GetDrives" });
			materialLabel1.Text = "Refreshing...";
		}
	}

	private void stopToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		DataGridViewRow row = dataGridView2.SelectedRows[0];
		string drive = ((row.Cells.Count > 0) ? (row.Cells[0].Value ?? "").ToString() : "");
		string type = ((row.Cells.Count > 1) ? (row.Cells[1].Value ?? "").ToString() : "");
		string total = ((row.Cells.Count > 2) ? (row.Cells[2].Value ?? "").ToString() : "");
		string free = ((row.Cells.Count > 3) ? (row.Cells[3].Value ?? "").ToString() : "");
		string files = ((row.Cells.Count > 4) ? (row.Cells[4].Value ?? "").ToString() : "");
		string visible = ((row.Cells.Count > 5) ? (row.Cells[5].Value ?? "").ToString() : "");
		using FormHardwareDriveInfo infoForm = new FormHardwareDriveInfo(drive, type, total, free, files, visible);
		infoForm.ShowDialog(this);
	}

	private void enableDiskToolStripMenuItem_Click(object sender, EventArgs e)
	{
		string letter = GetSelectedDriveLetter();
		if (!string.IsNullOrEmpty(letter) && client != null && client.itsConnect)
		{
			client.Send(new object[3] { "SetDriveEnabled", letter, true });
			materialLabel1.Text = "Enabling drive " + letter + "...";
		}
	}

	private void disableDiskToolStripMenuItem_Click(object sender, EventArgs e)
	{
		string letter = GetSelectedDriveLetter();
		if (!string.IsNullOrEmpty(letter) && client != null && client.itsConnect)
		{
			client.Send(new object[3] { "SetDriveEnabled", letter, false });
			materialLabel1.Text = "Disabling drive " + letter + "...";
		}
	}

	private void lockDriveToolStripMenuItem_Click(object sender, EventArgs e)
	{
		string letter = GetSelectedDriveLetter();
		if (!string.IsNullOrEmpty(letter) && client != null && client.itsConnect)
		{
			client.Send(new object[3] { "SetDriveLocked", letter, true });
			materialLabel1.Text = "Blocking access to drive " + letter + "...";
		}
	}

	private void unlockDriveToolStripMenuItem_Click(object sender, EventArgs e)
	{
		string letter = GetSelectedDriveLetter();
		if (!string.IsNullOrEmpty(letter) && client != null && client.itsConnect)
		{
			client.Send(new object[3] { "SetDriveLocked", letter, false });
			materialLabel1.Text = "Unblocking drive " + letter + "...";
		}
	}

	private void enableDisabledDiskToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(new object[1] { "EnableDisabledDisk" });
			materialLabel1.Text = "Enabling disabled disk and refreshing...";
		}
	}

	private void stressTestToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(new object[2] { "CpuStress", true });
			materialLabel1.Text = "Starting CPU stress test...";
		}
	}

	private void stopStressToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(new object[2] { "CpuStress", false });
			materialLabel1.Text = "Stopping CPU stress test...";
		}
	}

	private void refreshCpuToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(new object[1] { "GetCpu" });
			materialLabel1.Text = "Refreshing CPU info...";
		}
	}

	private void disableGpuToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(new object[2] { "SetGpuEnabled", false });
			materialLabel1.Text = "Disabling GPU...";
		}
	}

	private void enableGpuToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(new object[2] { "SetGpuEnabled", true });
			materialLabel1.Text = "Enabling GPU...";
		}
	}

	private void refreshGpuToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(new object[1] { "GetGpu" });
			materialLabel1.Text = "Refreshing GPU info...";
		}
	}

	private void clearWorkingSetToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(new object[1] { "ClearRam" });
			materialLabel1.Text = "Clearing working set...";
		}
	}

	private void refreshRamToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(new object[1] { "GetRam" });
			materialLabel1.Text = "Refreshing RAM info...";
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormHardWare));
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.panelDisk = new System.Windows.Forms.Panel();
		this.dataGridView2 = new System.Windows.Forms.DataGridView();
		this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.contextMenuDisk = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.enableDiskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.disableDiskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.enableDisabledDiskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.lockDriveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.unlockDriveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.dToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
		this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.killRemoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.labelDisk = new System.Windows.Forms.Label();
		this.panelRam = new System.Windows.Forms.Panel();
		this.gridRam = new System.Windows.Forms.DataGridView();
		this.colRamSlot = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.colRamCapacity = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.colRamSpeed = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.colRamType = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.contextMenuRam = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.clearWorkingSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.refreshRamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.labelRam = new System.Windows.Forms.Label();
		this.panelCpu = new System.Windows.Forms.Panel();
		this.gridCpu = new System.Windows.Forms.DataGridView();
		this.colCpuName = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.colCpuCores = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.colCpuThreads = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.colCpuClock = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.contextMenuCpu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.stressTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.stopStressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.refreshCpuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.labelCpu = new System.Windows.Forms.Label();
		this.panelGpu = new System.Windows.Forms.Panel();
		this.gridGpu = new System.Windows.Forms.DataGridView();
		this.colGpuName = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.colGpuMemory = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.colGpuDriver = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.colGpuStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.contextMenuGpu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.disableGpuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.enableGpuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.refreshGpuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.labelGpu = new System.Windows.Forms.Label();
		this.panel1 = new System.Windows.Forms.Panel();
		this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
		this.tableLayoutPanel1.SuspendLayout();
		this.panelDisk.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dataGridView2).BeginInit();
		this.contextMenuDisk.SuspendLayout();
		this.panelRam.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridRam).BeginInit();
		this.contextMenuRam.SuspendLayout();
		this.panelCpu.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridCpu).BeginInit();
		this.contextMenuCpu.SuspendLayout();
		this.panelGpu.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridGpu).BeginInit();
		this.contextMenuGpu.SuspendLayout();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.tableLayoutPanel1.ColumnCount = 2;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.Controls.Add(this.panelDisk, 0, 0);
		this.tableLayoutPanel1.Controls.Add(this.panelRam, 1, 0);
		this.tableLayoutPanel1.Controls.Add(this.panelCpu, 0, 1);
		this.tableLayoutPanel1.Controls.Add(this.panelGpu, 1, 1);
		this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 64);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 2;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(995, 521);
		this.tableLayoutPanel1.TabIndex = 0;
		this.panelDisk.Controls.Add(this.dataGridView2);
		this.panelDisk.Controls.Add(this.labelDisk);
		this.panelDisk.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelDisk.Location = new System.Drawing.Point(3, 3);
		this.panelDisk.Name = "panelDisk";
		this.panelDisk.Size = new System.Drawing.Size(491, 254);
		this.panelDisk.TabIndex = 0;
		this.dataGridView2.AllowUserToAddRows = false;
		this.dataGridView2.AllowUserToDeleteRows = false;
		this.dataGridView2.AllowUserToResizeRows = false;
		this.dataGridView2.BackgroundColor = System.Drawing.Color.White;
		this.dataGridView2.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dataGridView2.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.dataGridView2.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dataGridView2.Columns.AddRange(this.Column1, this.Column2, this.Column3, this.Column4, this.Column5, this.Column6, this.Column7);
		this.dataGridView2.ContextMenuStrip = this.contextMenuDisk;
		this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dataGridView2.Enabled = false;
		this.dataGridView2.EnableHeadersVisualStyles = false;
		this.dataGridView2.Location = new System.Drawing.Point(0, 20);
		this.dataGridView2.Name = "dataGridView2";
		this.dataGridView2.ReadOnly = true;
		this.dataGridView2.RowHeadersVisible = false;
		this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dataGridView2.ShowCellErrors = false;
		this.dataGridView2.ShowCellToolTips = false;
		this.dataGridView2.ShowEditingIcon = false;
		this.dataGridView2.ShowRowErrors = false;
		this.dataGridView2.Size = new System.Drawing.Size(491, 234);
		this.dataGridView2.TabIndex = 0;
		this.Column1.HeaderText = "Drive";
		this.Column1.Name = "Column1";
		this.Column1.ReadOnly = true;
		this.Column1.Width = 40;
		this.Column2.HeaderText = "Type";
		this.Column2.Name = "Column2";
		this.Column2.ReadOnly = true;
		this.Column2.Width = 50;
		this.Column3.HeaderText = "Total";
		this.Column3.Name = "Column3";
		this.Column3.ReadOnly = true;
		this.Column3.Width = 55;
		this.Column4.HeaderText = "Free";
		this.Column4.Name = "Column4";
		this.Column4.ReadOnly = true;
		this.Column4.Width = 55;
		this.Column5.HeaderText = "Files";
		this.Column5.Name = "Column5";
		this.Column5.ReadOnly = true;
		this.Column5.Width = 40;
		this.Column6.HeaderText = "Visible";
		this.Column6.Name = "Column6";
		this.Column6.ReadOnly = true;
		this.Column6.Width = 45;
		this.Column7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.Column7.HeaderText = "Locked";
		this.Column7.Name = "Column7";
		this.Column7.ReadOnly = true;
		this.contextMenuDisk.Items.AddRange(new System.Windows.Forms.ToolStripItem[11]
		{
			this.enableDiskToolStripMenuItem, this.disableDiskToolStripMenuItem, this.toolStripSeparator1, this.startToolStripMenuItem, this.pauseToolStripMenuItem, this.enableDisabledDiskToolStripMenuItem, this.lockDriveToolStripMenuItem, this.unlockDriveToolStripMenuItem, this.dToolStripMenuItem1, this.stopToolStripMenuItem,
			this.killRemoveToolStripMenuItem
		});
		this.contextMenuDisk.Name = "contextMenuDisk";
		this.contextMenuDisk.Size = new System.Drawing.Size(203, 214);
		this.enableDiskToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("enableDiskToolStripMenuItem.Image");
		this.enableDiskToolStripMenuItem.Name = "enableDiskToolStripMenuItem";
		this.enableDiskToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
		this.enableDiskToolStripMenuItem.Text = "Enable disk";
		this.enableDiskToolStripMenuItem.Click += new System.EventHandler(enableDiskToolStripMenuItem_Click);
		this.disableDiskToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("disableDiskToolStripMenuItem.Image");
		this.disableDiskToolStripMenuItem.Name = "disableDiskToolStripMenuItem";
		this.disableDiskToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
		this.disableDiskToolStripMenuItem.Text = "Disable disk";
		this.disableDiskToolStripMenuItem.Click += new System.EventHandler(disableDiskToolStripMenuItem_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(199, 6);
		this.startToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("startToolStripMenuItem.Image");
		this.startToolStripMenuItem.Name = "startToolStripMenuItem";
		this.startToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
		this.startToolStripMenuItem.Text = "Show in This PC";
		this.startToolStripMenuItem.Click += new System.EventHandler(startToolStripMenuItem_Click);
		this.pauseToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("pauseToolStripMenuItem.Image");
		this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
		this.pauseToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
		this.pauseToolStripMenuItem.Text = "Hide from This PC";
		this.pauseToolStripMenuItem.Click += new System.EventHandler(pauseToolStripMenuItem_Click);
		this.enableDisabledDiskToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("enableDisabledDiskToolStripMenuItem.Image");
		this.enableDisabledDiskToolStripMenuItem.Name = "enableDisabledDiskToolStripMenuItem";
		this.enableDisabledDiskToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
		this.enableDisabledDiskToolStripMenuItem.Text = "Enable disabled disk";
		this.enableDisabledDiskToolStripMenuItem.Click += new System.EventHandler(enableDisabledDiskToolStripMenuItem_Click);
		this.lockDriveToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("lockDriveToolStripMenuItem.Image");
		this.lockDriveToolStripMenuItem.Name = "lockDriveToolStripMenuItem";
		this.lockDriveToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
		this.lockDriveToolStripMenuItem.Text = "Block access (lock)";
		this.lockDriveToolStripMenuItem.Click += new System.EventHandler(lockDriveToolStripMenuItem_Click);
		this.unlockDriveToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("unlockDriveToolStripMenuItem.Image");
		this.unlockDriveToolStripMenuItem.Name = "unlockDriveToolStripMenuItem";
		this.unlockDriveToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
		this.unlockDriveToolStripMenuItem.Text = "Unblock access (unlock)";
		this.unlockDriveToolStripMenuItem.Click += new System.EventHandler(unlockDriveToolStripMenuItem_Click);
		this.dToolStripMenuItem1.Name = "dToolStripMenuItem1";
		this.dToolStripMenuItem1.Size = new System.Drawing.Size(199, 6);
		this.stopToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("stopToolStripMenuItem.Image");
		this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
		this.stopToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
		this.stopToolStripMenuItem.Text = "Information";
		this.stopToolStripMenuItem.Click += new System.EventHandler(stopToolStripMenuItem_Click);
		this.killRemoveToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("killRemoveToolStripMenuItem.Image");
		this.killRemoveToolStripMenuItem.Name = "killRemoveToolStripMenuItem";
		this.killRemoveToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
		this.killRemoveToolStripMenuItem.Text = "Refresh";
		this.killRemoveToolStripMenuItem.Click += new System.EventHandler(killRemoveToolStripMenuItem_Click);
		this.labelDisk.Dock = System.Windows.Forms.DockStyle.Top;
		this.labelDisk.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
		this.labelDisk.Location = new System.Drawing.Point(0, 0);
		this.labelDisk.Name = "labelDisk";
		this.labelDisk.Size = new System.Drawing.Size(491, 20);
		this.labelDisk.TabIndex = 1;
		this.labelDisk.Text = "  DISK";
		this.labelDisk.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.panelRam.Controls.Add(this.gridRam);
		this.panelRam.Controls.Add(this.labelRam);
		this.panelRam.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelRam.Location = new System.Drawing.Point(500, 3);
		this.panelRam.Name = "panelRam";
		this.panelRam.Size = new System.Drawing.Size(492, 254);
		this.panelRam.TabIndex = 1;
		this.gridRam.AllowUserToAddRows = false;
		this.gridRam.AllowUserToDeleteRows = false;
		this.gridRam.AllowUserToResizeRows = false;
		this.gridRam.BackgroundColor = System.Drawing.Color.White;
		this.gridRam.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.gridRam.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.gridRam.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		this.gridRam.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.gridRam.Columns.AddRange(this.colRamSlot, this.colRamCapacity, this.colRamSpeed, this.colRamType);
		this.gridRam.ContextMenuStrip = this.contextMenuRam;
		this.gridRam.Dock = System.Windows.Forms.DockStyle.Fill;
		this.gridRam.EnableHeadersVisualStyles = false;
		this.gridRam.Location = new System.Drawing.Point(0, 20);
		this.gridRam.Name = "gridRam";
		this.gridRam.ReadOnly = true;
		this.gridRam.RowHeadersVisible = false;
		this.gridRam.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.gridRam.ShowCellErrors = false;
		this.gridRam.ShowCellToolTips = false;
		this.gridRam.ShowEditingIcon = false;
		this.gridRam.ShowRowErrors = false;
		this.gridRam.Size = new System.Drawing.Size(492, 234);
		this.gridRam.TabIndex = 0;
		this.colRamSlot.HeaderText = "Slot";
		this.colRamSlot.Name = "colRamSlot";
		this.colRamSlot.ReadOnly = true;
		this.colRamSlot.Width = 50;
		this.colRamCapacity.HeaderText = "Capacity";
		this.colRamCapacity.Name = "colRamCapacity";
		this.colRamCapacity.ReadOnly = true;
		this.colRamCapacity.Width = 80;
		this.colRamSpeed.HeaderText = "Speed";
		this.colRamSpeed.Name = "colRamSpeed";
		this.colRamSpeed.ReadOnly = true;
		this.colRamSpeed.Width = 70;
		this.colRamType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.colRamType.HeaderText = "Type";
		this.colRamType.Name = "colRamType";
		this.colRamType.ReadOnly = true;
		this.contextMenuRam.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.clearWorkingSetToolStripMenuItem, this.refreshRamToolStripMenuItem });
		this.contextMenuRam.Name = "contextMenuRam";
		this.contextMenuRam.Size = new System.Drawing.Size(169, 48);
		this.clearWorkingSetToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("clearWorkingSetToolStripMenuItem.Image");
		this.clearWorkingSetToolStripMenuItem.Name = "clearWorkingSetToolStripMenuItem";
		this.clearWorkingSetToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
		this.clearWorkingSetToolStripMenuItem.Text = "Clear Working Set";
		this.clearWorkingSetToolStripMenuItem.Click += new System.EventHandler(clearWorkingSetToolStripMenuItem_Click);
		this.refreshRamToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("refreshRamToolStripMenuItem.Image");
		this.refreshRamToolStripMenuItem.Name = "refreshRamToolStripMenuItem";
		this.refreshRamToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
		this.refreshRamToolStripMenuItem.Text = "Refresh";
		this.refreshRamToolStripMenuItem.Click += new System.EventHandler(refreshRamToolStripMenuItem_Click);
		this.labelRam.Dock = System.Windows.Forms.DockStyle.Top;
		this.labelRam.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
		this.labelRam.Location = new System.Drawing.Point(0, 0);
		this.labelRam.Name = "labelRam";
		this.labelRam.Size = new System.Drawing.Size(492, 20);
		this.labelRam.TabIndex = 1;
		this.labelRam.Text = "  RAM";
		this.labelRam.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.panelCpu.Controls.Add(this.gridCpu);
		this.panelCpu.Controls.Add(this.labelCpu);
		this.panelCpu.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelCpu.Location = new System.Drawing.Point(3, 263);
		this.panelCpu.Name = "panelCpu";
		this.panelCpu.Size = new System.Drawing.Size(491, 255);
		this.panelCpu.TabIndex = 2;
		this.gridCpu.AllowUserToAddRows = false;
		this.gridCpu.AllowUserToDeleteRows = false;
		this.gridCpu.AllowUserToResizeRows = false;
		this.gridCpu.BackgroundColor = System.Drawing.Color.White;
		this.gridCpu.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.gridCpu.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.gridCpu.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		this.gridCpu.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.gridCpu.Columns.AddRange(this.colCpuName, this.colCpuCores, this.colCpuThreads, this.colCpuClock);
		this.gridCpu.ContextMenuStrip = this.contextMenuCpu;
		this.gridCpu.Dock = System.Windows.Forms.DockStyle.Fill;
		this.gridCpu.EnableHeadersVisualStyles = false;
		this.gridCpu.Location = new System.Drawing.Point(0, 20);
		this.gridCpu.Name = "gridCpu";
		this.gridCpu.ReadOnly = true;
		this.gridCpu.RowHeadersVisible = false;
		this.gridCpu.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.gridCpu.ShowCellErrors = false;
		this.gridCpu.ShowCellToolTips = false;
		this.gridCpu.ShowEditingIcon = false;
		this.gridCpu.ShowRowErrors = false;
		this.gridCpu.Size = new System.Drawing.Size(491, 235);
		this.gridCpu.TabIndex = 0;
		this.colCpuName.HeaderText = "Name";
		this.colCpuName.Name = "colCpuName";
		this.colCpuName.ReadOnly = true;
		this.colCpuName.Width = 200;
		this.colCpuCores.HeaderText = "Cores";
		this.colCpuCores.Name = "colCpuCores";
		this.colCpuCores.ReadOnly = true;
		this.colCpuCores.Width = 50;
		this.colCpuThreads.HeaderText = "Threads";
		this.colCpuThreads.Name = "colCpuThreads";
		this.colCpuThreads.ReadOnly = true;
		this.colCpuThreads.Width = 60;
		this.colCpuClock.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.colCpuClock.HeaderText = "Clock";
		this.colCpuClock.Name = "colCpuClock";
		this.colCpuClock.ReadOnly = true;
		this.contextMenuCpu.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.stressTestToolStripMenuItem, this.stopStressToolStripMenuItem, this.refreshCpuToolStripMenuItem });
		this.contextMenuCpu.Name = "contextMenuCpu";
		this.contextMenuCpu.Size = new System.Drawing.Size(132, 70);
		this.stressTestToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("stressTestToolStripMenuItem.Image");
		this.stressTestToolStripMenuItem.Name = "stressTestToolStripMenuItem";
		this.stressTestToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
		this.stressTestToolStripMenuItem.Text = "Stress Test";
		this.stressTestToolStripMenuItem.Click += new System.EventHandler(stressTestToolStripMenuItem_Click);
		this.stopStressToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("stopStressToolStripMenuItem.Image");
		this.stopStressToolStripMenuItem.Name = "stopStressToolStripMenuItem";
		this.stopStressToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
		this.stopStressToolStripMenuItem.Text = "Stop Stress";
		this.stopStressToolStripMenuItem.Click += new System.EventHandler(stopStressToolStripMenuItem_Click);
		this.refreshCpuToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("refreshCpuToolStripMenuItem.Image");
		this.refreshCpuToolStripMenuItem.Name = "refreshCpuToolStripMenuItem";
		this.refreshCpuToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
		this.refreshCpuToolStripMenuItem.Text = "Refresh";
		this.refreshCpuToolStripMenuItem.Click += new System.EventHandler(refreshCpuToolStripMenuItem_Click);
		this.labelCpu.Dock = System.Windows.Forms.DockStyle.Top;
		this.labelCpu.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
		this.labelCpu.Location = new System.Drawing.Point(0, 0);
		this.labelCpu.Name = "labelCpu";
		this.labelCpu.Size = new System.Drawing.Size(491, 20);
		this.labelCpu.TabIndex = 1;
		this.labelCpu.Text = "  CPU";
		this.labelCpu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.panelGpu.Controls.Add(this.gridGpu);
		this.panelGpu.Controls.Add(this.labelGpu);
		this.panelGpu.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelGpu.Location = new System.Drawing.Point(500, 263);
		this.panelGpu.Name = "panelGpu";
		this.panelGpu.Size = new System.Drawing.Size(492, 255);
		this.panelGpu.TabIndex = 3;
		this.gridGpu.AllowUserToAddRows = false;
		this.gridGpu.AllowUserToDeleteRows = false;
		this.gridGpu.AllowUserToResizeRows = false;
		this.gridGpu.BackgroundColor = System.Drawing.Color.White;
		this.gridGpu.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.gridGpu.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.gridGpu.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		this.gridGpu.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.gridGpu.Columns.AddRange(this.colGpuName, this.colGpuMemory, this.colGpuDriver, this.colGpuStatus);
		this.gridGpu.ContextMenuStrip = this.contextMenuGpu;
		this.gridGpu.Dock = System.Windows.Forms.DockStyle.Fill;
		this.gridGpu.EnableHeadersVisualStyles = false;
		this.gridGpu.Location = new System.Drawing.Point(0, 20);
		this.gridGpu.Name = "gridGpu";
		this.gridGpu.ReadOnly = true;
		this.gridGpu.RowHeadersVisible = false;
		this.gridGpu.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.gridGpu.ShowCellErrors = false;
		this.gridGpu.ShowCellToolTips = false;
		this.gridGpu.ShowEditingIcon = false;
		this.gridGpu.ShowRowErrors = false;
		this.gridGpu.Size = new System.Drawing.Size(492, 235);
		this.gridGpu.TabIndex = 0;
		this.colGpuName.HeaderText = "Name";
		this.colGpuName.Name = "colGpuName";
		this.colGpuName.ReadOnly = true;
		this.colGpuName.Width = 200;
		this.colGpuMemory.HeaderText = "Memory";
		this.colGpuMemory.Name = "colGpuMemory";
		this.colGpuMemory.ReadOnly = true;
		this.colGpuMemory.Width = 80;
		this.colGpuDriver.HeaderText = "Driver";
		this.colGpuDriver.Name = "colGpuDriver";
		this.colGpuDriver.ReadOnly = true;
		this.colGpuStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.colGpuStatus.HeaderText = "Status";
		this.colGpuStatus.Name = "colGpuStatus";
		this.colGpuStatus.ReadOnly = true;
		this.contextMenuGpu.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.disableGpuToolStripMenuItem, this.enableGpuToolStripMenuItem, this.refreshGpuToolStripMenuItem });
		this.contextMenuGpu.Name = "contextMenuGpu";
		this.contextMenuGpu.Size = new System.Drawing.Size(139, 70);
		this.disableGpuToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("disableGpuToolStripMenuItem.Image");
		this.disableGpuToolStripMenuItem.Name = "disableGpuToolStripMenuItem";
		this.disableGpuToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.disableGpuToolStripMenuItem.Text = "Disable GPU";
		this.disableGpuToolStripMenuItem.Click += new System.EventHandler(disableGpuToolStripMenuItem_Click);
		this.enableGpuToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("enableGpuToolStripMenuItem.Image");
		this.enableGpuToolStripMenuItem.Name = "enableGpuToolStripMenuItem";
		this.enableGpuToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
		this.enableGpuToolStripMenuItem.Text = "Enable GPU";
		this.enableGpuToolStripMenuItem.Click += new System.EventHandler(enableGpuToolStripMenuItem_Click);
		this.refreshGpuToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("refreshGpuToolStripMenuItem.Image");
		this.refreshGpuToolStripMenuItem.Name = "refreshGpuToolStripMenuItem";
		this.refreshGpuToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.refreshGpuToolStripMenuItem.Text = "Refresh";
		this.refreshGpuToolStripMenuItem.Click += new System.EventHandler(refreshGpuToolStripMenuItem_Click);
		this.labelGpu.Dock = System.Windows.Forms.DockStyle.Top;
		this.labelGpu.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
		this.labelGpu.Location = new System.Drawing.Point(0, 0);
		this.labelGpu.Name = "labelGpu";
		this.labelGpu.Size = new System.Drawing.Size(492, 20);
		this.labelGpu.TabIndex = 1;
		this.labelGpu.Text = "  GPU";
		this.labelGpu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.panel1.Controls.Add(this.materialLabel1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel1.Location = new System.Drawing.Point(3, 585);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(995, 20);
		this.panel1.TabIndex = 1;
		this.materialLabel1.AutoSize = true;
		this.materialLabel1.Depth = 0;
		this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel1.Location = new System.Drawing.Point(2, 1);
		this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel1.Name = "materialLabel1";
		this.materialLabel1.Size = new System.Drawing.Size(94, 19);
		this.materialLabel1.TabIndex = 1;
		this.materialLabel1.Text = "Please wait...";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1001, 608);
		base.Controls.Add(this.tableLayoutPanel1);
		base.Controls.Add(this.panel1);
		base.Name = "FormHardWare";
		this.Text = "Hardware";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(FormHardWare_FormClosing);
		base.Load += new System.EventHandler(FormHardWare_Load);
		this.tableLayoutPanel1.ResumeLayout(false);
		this.panelDisk.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dataGridView2).EndInit();
		this.contextMenuDisk.ResumeLayout(false);
		this.panelRam.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridRam).EndInit();
		this.contextMenuRam.ResumeLayout(false);
		this.panelCpu.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridCpu).EndInit();
		this.contextMenuCpu.ResumeLayout(false);
		this.panelGpu.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridGpu).EndInit();
		this.contextMenuGpu.ResumeLayout(false);
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
