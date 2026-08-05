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

public class FormWifi : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	private Timer timer1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem stopToolStripMenuItem;

	private ToolStripMenuItem killRemoveToolStripMenuItem;

	private Panel panel1;

	public DataGridView dataGridView2;

	public MaterialLabel materialLabel1;

	private ToolStripMenuItem startToolStripMenuItem;

	private ToolStripMenuItem pauseToolStripMenuItem;

	private DataGridViewTextBoxColumn Column1;

	private DataGridViewTextBoxColumn Column2;

	private DataGridViewTextBoxColumn Column3;

	public FormWifi()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormProcess_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		timer1.Start();
		typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty, null, dataGridView2, new object[1] { true });
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
		if (dataGridView2 != null)
		{
			dataGridView2.BackgroundColor = back;
			dataGridView2.ColumnHeadersDefaultCellStyle.SelectionForeColor = primary;
			dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = primary;
			dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = back;
			dataGridView2.DefaultCellStyle.SelectionBackColor = primary;
			dataGridView2.DefaultCellStyle.ForeColor = primary;
			dataGridView2.DefaultCellStyle.BackColor = back;
		}
	}

	private void Closing1(object sender, EventArgs e)
	{
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

	private string[] DisplayNameSelected()
	{
		List<string> list = new List<string>();
		foreach (DataGridViewRow selectedRow in dataGridView2.SelectedRows)
		{
			list.Add((string)selectedRow.Cells[0].Value);
		}
		return list.ToArray();
	}

	private void killRemoveToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client != null && client.itsConnect)
		{
			client.Send(LEB128.Write(new object[1] { "Refresh" }));
		}
	}

	private void startToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (dataGridView2.SelectedRows.Count != 0 && client != null && client.itsConnect)
		{
			string[] array = DisplayNameSelected();
			foreach (string name in array)
			{
				client.Send(LEB128.Write(new object[2] { "Start", name }));
			}
		}
	}

	private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (dataGridView2.SelectedRows.Count != 0 && client != null && client.itsConnect)
		{
			string[] array = DisplayNameSelected();
			foreach (string name in array)
			{
				client.Send(LEB128.Write(new object[2] { "Pause", name }));
			}
		}
	}

	private void stopToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (dataGridView2.SelectedRows.Count != 0 && client != null && client.itsConnect)
		{
			string[] array = DisplayNameSelected();
			foreach (string name in array)
			{
				client.Send(LEB128.Write(new object[2] { "Stop", name }));
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormWifi));
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.killRemoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.panel1 = new System.Windows.Forms.Panel();
		this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
		this.dataGridView2 = new System.Windows.Forms.DataGridView();
		this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.contextMenuStrip1.SuspendLayout();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dataGridView2).BeginInit();
		base.SuspendLayout();
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.startToolStripMenuItem, this.pauseToolStripMenuItem, this.stopToolStripMenuItem, this.killRemoveToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(114, 92);
		this.startToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(32, 32, 32);
		this.startToolStripMenuItem.ForeColor = System.Drawing.Color.White;
		this.startToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("startToolStripMenuItem.Image");
		this.startToolStripMenuItem.Name = "startToolStripMenuItem";
		this.startToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.startToolStripMenuItem.Text = "Start";
		this.startToolStripMenuItem.Click += new System.EventHandler(startToolStripMenuItem_Click);
		this.pauseToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(32, 32, 32);
		this.pauseToolStripMenuItem.ForeColor = System.Drawing.Color.White;
		this.pauseToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("pauseToolStripMenuItem.Image");
		this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
		this.pauseToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.pauseToolStripMenuItem.Text = "Pause";
		this.pauseToolStripMenuItem.Click += new System.EventHandler(pauseToolStripMenuItem_Click);
		this.stopToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(32, 32, 32);
		this.stopToolStripMenuItem.ForeColor = System.Drawing.Color.White;
		this.stopToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("stopToolStripMenuItem.Image");
		this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
		this.stopToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.stopToolStripMenuItem.Text = "Stop";
		this.stopToolStripMenuItem.Click += new System.EventHandler(stopToolStripMenuItem_Click);
		this.killRemoveToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(32, 32, 32);
		this.killRemoveToolStripMenuItem.ForeColor = System.Drawing.Color.White;
		this.killRemoveToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("killRemoveToolStripMenuItem.Image");
		this.killRemoveToolStripMenuItem.Name = "killRemoveToolStripMenuItem";
		this.killRemoveToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.killRemoveToolStripMenuItem.Text = "Refresh";
		this.killRemoveToolStripMenuItem.Click += new System.EventHandler(killRemoveToolStripMenuItem_Click);
		this.panel1.Controls.Add(this.materialLabel1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel1.Location = new System.Drawing.Point(3, 512);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(651, 20);
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
		this.dataGridView2.AllowDrop = true;
		this.dataGridView2.AllowUserToAddRows = false;
		this.dataGridView2.AllowUserToDeleteRows = false;
		this.dataGridView2.AllowUserToResizeColumns = false;
		this.dataGridView2.AllowUserToResizeRows = false;
		this.dataGridView2.BackgroundColor = System.Drawing.Color.FromArgb(32, 32, 32);
		this.dataGridView2.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dataGridView2.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.dataGridView2.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
		dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(32, 32, 32);
		dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(32, 32, 32);
		dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView2.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
		this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dataGridView2.Columns.AddRange(this.Column1, this.Column2, this.Column3);
		this.dataGridView2.ContextMenuStrip = this.contextMenuStrip1;
		this.dataGridView2.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(32, 32, 32);
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(32, 32, 32);
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dataGridView2.DefaultCellStyle = dataGridViewCellStyle2;
		this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dataGridView2.Enabled = false;
		this.dataGridView2.EnableHeadersVisualStyles = false;
		this.dataGridView2.GridColor = System.Drawing.Color.FromArgb(17, 17, 17);
		this.dataGridView2.Location = new System.Drawing.Point(3, 64);
		this.dataGridView2.Name = "dataGridView2";
		this.dataGridView2.ReadOnly = true;
		this.dataGridView2.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
		dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView2.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
		this.dataGridView2.RowHeadersVisible = false;
		this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dataGridView2.ShowCellErrors = false;
		this.dataGridView2.ShowCellToolTips = false;
		this.dataGridView2.ShowEditingIcon = false;
		this.dataGridView2.ShowRowErrors = false;
		this.dataGridView2.Size = new System.Drawing.Size(651, 448);
		this.dataGridView2.TabIndex = 17;
		this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.Column1.HeaderText = "Name";
		this.Column1.Name = "Column1";
		this.Column1.ReadOnly = true;
		this.Column1.Width = 64;
		this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.Column2.HeaderText = "Status";
		this.Column2.MinimumWidth = 80;
		this.Column2.Name = "Column2";
		this.Column2.ReadOnly = true;
		this.Column2.Width = 80;
		this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.Column3.HeaderText = "Adapter";
		this.Column3.Name = "Column3";
		this.Column3.ReadOnly = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(657, 535);
		base.Controls.Add(this.dataGridView2);
		base.Controls.Add(this.panel1);
		base.Name = "FormWifi";
		this.Text = "Wifi";
		base.Load += new System.EventHandler(FormProcess_Load);
		this.contextMenuStrip1.ResumeLayout(false);
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.dataGridView2).EndInit();
		base.ResumeLayout(false);
	}
}
