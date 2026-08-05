using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Leb128;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormTaskSheduler : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	public DataGridView dataGridView1;

	private DataGridViewTextBoxColumn Column1;

	private DataGridViewTextBoxColumn Column2;

	private DataGridViewTextBoxColumn Column3;

	private DataGridViewTextBoxColumn Column4;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem refreshToolStripMenuItem;

	private ToolStripMenuItem addToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ToolStripMenuItem runToolStripMenuItem;

	private Timer timer1;

	public MaterialLabel materialLabel1;

	public FormTaskSheduler()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormTaskSheduler_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		timer1.Start();
	}

	private void ChangeScheme(object sender)
	{
		if (base.IsDisposed)
		{
			return;
		}
		try
		{
			bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
			Color backColor = (isDark ? Color.FromArgb(40, 40, 40) : Color.White);
			Color foreColor = (isDark ? Color.White : Color.Black);
			Color gridForeColor = (isDark ? Color.White : FormMaterial.PrimaryColor);
			BackColor = backColor;
			dataGridView1.BackgroundColor = backColor;
			dataGridView1.GridColor = (isDark ? Color.FromArgb(60, 60, 60) : Color.FromArgb(17, 17, 17));
			dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = backColor;
			dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = gridForeColor;
			dataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = backColor;
			dataGridView1.DefaultCellStyle.BackColor = backColor;
			dataGridView1.DefaultCellStyle.ForeColor = gridForeColor;
			dataGridView1.DefaultCellStyle.SelectionBackColor = gridForeColor;
			dataGridView1.DefaultCellStyle.SelectionForeColor = backColor;
			materialLabel1.BackColor = backColor;
			materialLabel1.ForeColor = foreColor;
		}
		catch
		{
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
			client.Send(LEB128.Write(new object[2] { "TaskScheduler", "List" }));
		}
	}

	private void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client == null)
		{
			return;
		}
		using FormTaskShedulerSet formSet = new FormTaskShedulerSet();
		if (formSet.ShowDialog() == DialogResult.OK)
		{
			client.Send(LEB128.Write(new object[5] { "TaskScheduler", "Add", formSet.TaskName, formSet.TaskPath, formSet.TaskArguments }));
		}
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client == null || dataGridView1.SelectedRows.Count <= 0)
		{
			return;
		}
		foreach (DataGridViewRow row in dataGridView1.SelectedRows)
		{
			client.Send(LEB128.Write(new object[3]
			{
				"TaskScheduler",
				"Remove",
				row.Cells[0].Value.ToString()
			}));
		}
	}

	private void runToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (client == null || dataGridView1.SelectedRows.Count <= 0)
		{
			return;
		}
		foreach (DataGridViewRow row in dataGridView1.SelectedRows)
		{
			client.Send(LEB128.Write(new object[3]
			{
				"TaskScheduler",
				"Run",
				row.Cells[0].Value.ToString()
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormTaskSheduler));
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
		this.dataGridView1 = new System.Windows.Forms.DataGridView();
		this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dataGridView1).BeginInit();
		base.SuspendLayout();
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.refreshToolStripMenuItem, this.addToolStripMenuItem, this.deleteToolStripMenuItem, this.runToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(114, 92);
		this.refreshToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("refreshToolStripMenuItem.Image");
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.refreshToolStripMenuItem.Text = "Refresh";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(refreshToolStripMenuItem_Click);
		this.addToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("addToolStripMenuItem.Image");
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(addToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("deleteToolStripMenuItem.Image");
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(deleteToolStripMenuItem_Click);
		this.runToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("runToolStripMenuItem.Image");
		this.runToolStripMenuItem.Name = "runToolStripMenuItem";
		this.runToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.runToolStripMenuItem.Text = "Run";
		this.runToolStripMenuItem.Click += new System.EventHandler(runToolStripMenuItem_Click);
		this.timer1.Interval = 2000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.materialLabel1.AutoSize = true;
		this.materialLabel1.Depth = 0;
		this.materialLabel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel1.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.materialLabel1.Location = new System.Drawing.Point(3, 513);
		this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel1.Name = "materialLabel1";
		this.materialLabel1.Size = new System.Drawing.Size(47, 19);
		this.materialLabel1.TabIndex = 1;
		this.materialLabel1.Text = "Status";
		this.dataGridView1.AllowUserToAddRows = false;
		this.dataGridView1.AllowUserToDeleteRows = false;
		this.dataGridView1.AllowUserToResizeColumns = false;
		this.dataGridView1.AllowUserToResizeRows = false;
		this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
		this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
		dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White;
		dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
		this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dataGridView1.Columns.AddRange(this.Column1, this.Column2, this.Column3, this.Column4);
		this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
		this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dataGridView1.EnableHeadersVisualStyles = false;
		this.dataGridView1.GridColor = System.Drawing.Color.FromArgb(17, 17, 17);
		this.dataGridView1.Location = new System.Drawing.Point(3, 64);
		this.dataGridView1.Name = "dataGridView1";
		this.dataGridView1.ReadOnly = true;
		this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
		dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
		this.dataGridView1.RowHeadersVisible = false;
		this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dataGridView1.Size = new System.Drawing.Size(794, 449);
		this.dataGridView1.TabIndex = 2;
		this.Column1.HeaderText = "Task Name";
		this.Column1.Name = "Column1";
		this.Column1.ReadOnly = true;
		this.Column1.Width = 200;
		this.Column2.HeaderText = "Next Run Time";
		this.Column2.Name = "Column2";
		this.Column2.ReadOnly = true;
		this.Column2.Width = 150;
		this.Column3.HeaderText = "Status";
		this.Column3.Name = "Column3";
		this.Column3.ReadOnly = true;
		this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.Column4.HeaderText = "Task Path";
		this.Column4.Name = "Column4";
		this.Column4.ReadOnly = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 535);
		base.Controls.Add(this.dataGridView1);
		base.Controls.Add(this.materialLabel1);
		base.Name = "FormTaskSheduler";
		this.Text = "Task Scheduler";
		base.Load += new System.EventHandler(FormTaskSheduler_Load);
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dataGridView1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
