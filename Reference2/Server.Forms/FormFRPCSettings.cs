using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Data;
using Server.Helper;

namespace Server.Forms;

public class FormFRPCSettings : FormMaterial
{
	private IContainer components;

	private RJTextBox rjTextBox4;

	private Label lblStatus;

	private RJButton rjButton4;

	private MaterialLabel materialLabel5;

	private MaterialLabel materialLabel1;

	private RJTextBox rjTextBox1;

	private MaterialLabel materialLabel2;

	private RJTextBox rjTextBox2;

	private RJButton rjButton1;

	private MaterialLabel materialLabel3;

	private RJTextBox rjTextBox3;

	private MaterialLabel materialLabel4;

	private RJTextBox rjTextBox5;

	private MaterialLabel materialLabel6;

	private RJComboBox rjComboBox2;

	public FormFRPCSettings()
	{
		InitializeComponent();
		base.Load += FormFRPCSettings_Load;
		rjButton1.Click += rjButton1_Click;
		rjButton4.Click += rjButton4_Click;
		base.FormClosing += FormFRPCSettings_FormClosing;
	}

	private void FormFRPCSettings_Load(object sender, EventArgs e)
	{
		rjComboBox2.Items.Clear();
		rjComboBox2.Items.Add("tcp");
		rjComboBox2.Items.Add("kcp");
		rjComboBox2.Items.Add("websocket");
		rjComboBox2.SelectedIndex = 0;
		FrpcSettings settings = FrpcManager.LoadSettings();
		if (settings != null)
		{
			rjTextBox4.Texts = settings.ServerAddr;
			rjTextBox1.Texts = settings.ServerPort;
			rjTextBox2.Texts = settings.Token;
			rjTextBox3.Texts = settings.LocalPort;
			rjTextBox5.Texts = settings.RemotePort;
			int protoIndex = rjComboBox2.Items.IndexOf(settings.Protocol);
			if (protoIndex >= 0)
			{
				rjComboBox2.SelectedIndex = protoIndex;
			}
		}
		UpdateStatus();
		FrpcManager.OnStatusChanged += OnFrpcStatusChanged;
	}

	private void FormFRPCSettings_FormClosing(object sender, FormClosingEventArgs e)
	{
		SaveCurrentFrpcSettings();
		FrpcManager.OnStatusChanged -= OnFrpcStatusChanged;
	}

	private void rjButton1_Click(object sender, EventArgs e)
	{
		if (FrpcManager.IsRunning)
		{
			FrpcManager.Stop();
			UpdateStatus();
			return;
		}
		SaveCurrentFrpcSettings();
		bool num = FrpcManager.Start();
		UpdateStatus();
		if (!num)
		{
			MessageBox.Show("Failed to start FRPC. Make sure frpc.exe is in the application folder and settings are correct.", "FRPC Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void rjButton4_Click(object sender, EventArgs e)
	{
		using OpenFileDialog ofd = new OpenFileDialog();
		ofd.Filter = "INI files (*.ini)|*.ini|TOML files (*.toml)|*.toml|All files (*.*)|*.*";
		ofd.Title = "Select FRPC config file";
		if (ofd.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		try
		{
			FrpcSettings parsed = FrpcManager.ParseIniFile(File.ReadAllText(ofd.FileName));
			if (parsed != null)
			{
				rjTextBox4.Texts = parsed.ServerAddr;
				rjTextBox1.Texts = parsed.ServerPort;
				rjTextBox2.Texts = parsed.Token;
				rjTextBox3.Texts = parsed.LocalPort;
				rjTextBox5.Texts = parsed.RemotePort;
				int protoIndex = rjComboBox2.Items.IndexOf(parsed.Protocol);
				if (protoIndex >= 0)
				{
					rjComboBox2.SelectedIndex = protoIndex;
				}
				SaveCurrentFrpcSettings();
				MessageBox.Show("Config imported successfully!", "Import", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error importing config: " + ex.Message, "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void SaveCurrentFrpcSettings()
	{
		FrpcManager.SaveSettings(new FrpcSettings
		{
			ServerAddr = rjTextBox4.Texts,
			ServerPort = rjTextBox1.Texts,
			Token = rjTextBox2.Texts,
			LocalPort = rjTextBox3.Texts,
			RemotePort = rjTextBox5.Texts,
			Protocol = (rjComboBox2.Texts ?? "tcp")
		});
	}

	private void UpdateStatus()
	{
		if (FrpcManager.IsRunning)
		{
			lblStatus.Text = "Status: Connected";
			lblStatus.ForeColor = Color.Green;
			rjButton1.Text = "Disconnect";
		}
		else
		{
			lblStatus.Text = "Status: Disconnected";
			lblStatus.ForeColor = Color.Red;
			rjButton1.Text = "Connect";
		}
	}

	private void OnFrpcStatusChanged(string status)
	{
		if (base.InvokeRequired)
		{
			BeginInvoke((Action)delegate
			{
				OnFrpcStatusChanged(status);
			});
			return;
		}
		lblStatus.Text = status;
		if (status.Contains("Connected"))
		{
			lblStatus.ForeColor = Color.Green;
			rjButton1.Text = "Disconnect";
		}
		else
		{
			lblStatus.ForeColor = Color.Red;
			rjButton1.Text = "Connect";
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
		this.rjTextBox4 = new CustomControls.RJControls.RJTextBox();
		this.lblStatus = new System.Windows.Forms.Label();
		this.rjButton4 = new CustomControls.RJControls.RJButton();
		this.materialLabel5 = new MaterialSkin.Controls.MaterialLabel();
		this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
		this.rjTextBox1 = new CustomControls.RJControls.RJTextBox();
		this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
		this.rjTextBox2 = new CustomControls.RJControls.RJTextBox();
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
		this.rjTextBox3 = new CustomControls.RJControls.RJTextBox();
		this.materialLabel4 = new MaterialSkin.Controls.MaterialLabel();
		this.rjTextBox5 = new CustomControls.RJControls.RJTextBox();
		this.materialLabel6 = new MaterialSkin.Controls.MaterialLabel();
		this.rjComboBox2 = new CustomControls.RJControls.RJComboBox();
		base.SuspendLayout();
		this.rjTextBox4.BackColor = System.Drawing.Color.White;
		this.rjTextBox4.BorderColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox4.BorderFocusColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox4.BorderRadius = 0;
		this.rjTextBox4.BorderSize = 2;
		this.rjTextBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox4.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox4.Location = new System.Drawing.Point(9, 96);
		this.rjTextBox4.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox4.Multiline = false;
		this.rjTextBox4.Name = "rjTextBox4";
		this.rjTextBox4.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox4.PasswordChar = false;
		this.rjTextBox4.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox4.PlaceholderText = "Server Addr";
		this.rjTextBox4.Size = new System.Drawing.Size(384, 31);
		this.rjTextBox4.TabIndex = 1;
		this.rjTextBox4.Texts = "";
		this.rjTextBox4.UnderlinedStyle = false;
		this.lblStatus.AutoSize = true;
		this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
		this.lblStatus.ForeColor = System.Drawing.Color.Red;
		this.lblStatus.Location = new System.Drawing.Point(9, 426);
		this.lblStatus.Name = "lblStatus";
		this.lblStatus.Size = new System.Drawing.Size(124, 15);
		this.lblStatus.TabIndex = 7;
		this.lblStatus.Text = "Status: Disconnected";
		this.rjButton4.BackColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjButton4.BackgroundColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjButton4.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton4.BorderRadius = 0;
		this.rjButton4.BorderSize = 0;
		this.rjButton4.FlatAppearance.BorderSize = 0;
		this.rjButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton4.ForeColor = System.Drawing.Color.White;
		this.rjButton4.Location = new System.Drawing.Point(9, 444);
		this.rjButton4.Name = "rjButton4";
		this.rjButton4.Size = new System.Drawing.Size(192, 29);
		this.rjButton4.TabIndex = 67;
		this.rjButton4.Text = "Import cfg";
		this.rjButton4.TextColor = System.Drawing.Color.White;
		this.rjButton4.UseVisualStyleBackColor = false;
		this.materialLabel5.AutoSize = true;
		this.materialLabel5.Depth = 0;
		this.materialLabel5.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel5.Location = new System.Drawing.Point(6, 73);
		this.materialLabel5.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel5.Name = "materialLabel5";
		this.materialLabel5.Size = new System.Drawing.Size(122, 19);
		this.materialLabel5.TabIndex = 68;
		this.materialLabel5.Text = "Server ip address";
		this.materialLabel1.AutoSize = true;
		this.materialLabel1.Depth = 0;
		this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel1.Location = new System.Drawing.Point(6, 130);
		this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel1.Name = "materialLabel1";
		this.materialLabel1.Size = new System.Drawing.Size(77, 19);
		this.materialLabel1.TabIndex = 70;
		this.materialLabel1.Text = "Server port";
		this.rjTextBox1.BackColor = System.Drawing.Color.White;
		this.rjTextBox1.BorderColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox1.BorderFocusColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox1.BorderRadius = 0;
		this.rjTextBox1.BorderSize = 2;
		this.rjTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox1.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox1.Location = new System.Drawing.Point(9, 153);
		this.rjTextBox1.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox1.Multiline = false;
		this.rjTextBox1.Name = "rjTextBox1";
		this.rjTextBox1.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox1.PasswordChar = false;
		this.rjTextBox1.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox1.PlaceholderText = "Server Port";
		this.rjTextBox1.Size = new System.Drawing.Size(384, 31);
		this.rjTextBox1.TabIndex = 69;
		this.rjTextBox1.Texts = "";
		this.rjTextBox1.UnderlinedStyle = false;
		this.materialLabel2.AutoSize = true;
		this.materialLabel2.Depth = 0;
		this.materialLabel2.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel2.Location = new System.Drawing.Point(6, 186);
		this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel2.Name = "materialLabel2";
		this.materialLabel2.Size = new System.Drawing.Size(45, 19);
		this.materialLabel2.TabIndex = 72;
		this.materialLabel2.Text = "Token";
		this.rjTextBox2.BackColor = System.Drawing.Color.White;
		this.rjTextBox2.BorderColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox2.BorderFocusColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox2.BorderRadius = 0;
		this.rjTextBox2.BorderSize = 2;
		this.rjTextBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox2.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox2.Location = new System.Drawing.Point(9, 209);
		this.rjTextBox2.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox2.Multiline = false;
		this.rjTextBox2.Name = "rjTextBox2";
		this.rjTextBox2.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox2.PasswordChar = false;
		this.rjTextBox2.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox2.PlaceholderText = "token";
		this.rjTextBox2.Size = new System.Drawing.Size(384, 31);
		this.rjTextBox2.TabIndex = 71;
		this.rjTextBox2.Texts = "";
		this.rjTextBox2.UnderlinedStyle = false;
		this.rjButton1.BackColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjButton1.BackgroundColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjButton1.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton1.BorderRadius = 0;
		this.rjButton1.BorderSize = 0;
		this.rjButton1.FlatAppearance.BorderSize = 0;
		this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton1.ForeColor = System.Drawing.Color.White;
		this.rjButton1.Location = new System.Drawing.Point(207, 444);
		this.rjButton1.Name = "rjButton1";
		this.rjButton1.Size = new System.Drawing.Size(186, 29);
		this.rjButton1.TabIndex = 73;
		this.rjButton1.Text = "Connect";
		this.rjButton1.TextColor = System.Drawing.Color.White;
		this.rjButton1.UseVisualStyleBackColor = false;
		this.materialLabel3.AutoSize = true;
		this.materialLabel3.Depth = 0;
		this.materialLabel3.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel3.Location = new System.Drawing.Point(6, 245);
		this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel3.Name = "materialLabel3";
		this.materialLabel3.Size = new System.Drawing.Size(72, 19);
		this.materialLabel3.TabIndex = 75;
		this.materialLabel3.Text = "Local port";
		this.rjTextBox3.BackColor = System.Drawing.Color.White;
		this.rjTextBox3.BorderColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox3.BorderFocusColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox3.BorderRadius = 0;
		this.rjTextBox3.BorderSize = 2;
		this.rjTextBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox3.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox3.Location = new System.Drawing.Point(9, 268);
		this.rjTextBox3.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox3.Multiline = false;
		this.rjTextBox3.Name = "rjTextBox3";
		this.rjTextBox3.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox3.PasswordChar = false;
		this.rjTextBox3.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox3.PlaceholderText = "local port";
		this.rjTextBox3.Size = new System.Drawing.Size(384, 31);
		this.rjTextBox3.TabIndex = 74;
		this.rjTextBox3.Texts = "";
		this.rjTextBox3.UnderlinedStyle = false;
		this.materialLabel4.AutoSize = true;
		this.materialLabel4.Depth = 0;
		this.materialLabel4.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel4.Location = new System.Drawing.Point(9, 305);
		this.materialLabel4.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel4.Name = "materialLabel4";
		this.materialLabel4.Size = new System.Drawing.Size(87, 19);
		this.materialLabel4.TabIndex = 77;
		this.materialLabel4.Text = "Remote port";
		this.rjTextBox5.BackColor = System.Drawing.Color.White;
		this.rjTextBox5.BorderColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox5.BorderFocusColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox5.BorderRadius = 0;
		this.rjTextBox5.BorderSize = 2;
		this.rjTextBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox5.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox5.Location = new System.Drawing.Point(12, 328);
		this.rjTextBox5.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox5.Multiline = false;
		this.rjTextBox5.Name = "rjTextBox5";
		this.rjTextBox5.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox5.PasswordChar = false;
		this.rjTextBox5.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox5.PlaceholderText = "remote port";
		this.rjTextBox5.Size = new System.Drawing.Size(384, 31);
		this.rjTextBox5.TabIndex = 76;
		this.rjTextBox5.Texts = "";
		this.rjTextBox5.UnderlinedStyle = false;
		this.materialLabel6.AutoSize = true;
		this.materialLabel6.Depth = 0;
		this.materialLabel6.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel6.Location = new System.Drawing.Point(9, 369);
		this.materialLabel6.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel6.Name = "materialLabel6";
		this.materialLabel6.Size = new System.Drawing.Size(60, 19);
		this.materialLabel6.TabIndex = 78;
		this.materialLabel6.Text = "Protocol";
		this.rjComboBox2.BackColor = System.Drawing.Color.WhiteSmoke;
		this.rjComboBox2.BorderColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjComboBox2.BorderSize = 1;
		this.rjComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox2.ForeColor = System.Drawing.Color.Black;
		this.rjComboBox2.IconColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjComboBox2.ListBackColor = System.Drawing.Color.White;
		this.rjComboBox2.ListTextColor = System.Drawing.Color.Black;
		this.rjComboBox2.Location = new System.Drawing.Point(12, 391);
		this.rjComboBox2.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox2.Name = "rjComboBox2";
		this.rjComboBox2.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox2.Size = new System.Drawing.Size(384, 30);
		this.rjComboBox2.TabIndex = 79;
		this.rjComboBox2.Texts = "";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(403, 479);
		base.Controls.Add(this.materialLabel6);
		base.Controls.Add(this.rjComboBox2);
		base.Controls.Add(this.materialLabel4);
		base.Controls.Add(this.rjTextBox5);
		base.Controls.Add(this.materialLabel3);
		base.Controls.Add(this.rjTextBox3);
		base.Controls.Add(this.rjButton1);
		base.Controls.Add(this.materialLabel2);
		base.Controls.Add(this.rjTextBox2);
		base.Controls.Add(this.materialLabel1);
		base.Controls.Add(this.rjTextBox1);
		base.Controls.Add(this.materialLabel5);
		base.Controls.Add(this.rjButton4);
		base.Controls.Add(this.lblStatus);
		base.Controls.Add(this.rjTextBox4);
		base.Name = "FormFRPCSettings";
		this.Text = "FRPC Settings";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
