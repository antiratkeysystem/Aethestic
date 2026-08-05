using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using CustomControls.RJControls;
using Leb128;
using MaterialSkin;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormFunAudio : FormMaterial
{
	private Dictionary<string, string> soundFiles = new Dictionary<string, string>();

	private Timer timer1;

	public Clients client;

	public Clients parrent;

	private IContainer components;

	private Label materialLabel1;

	private Label label1;

	private RJButton rjButton1;

	private RJButton rjButton2;

	private RJButton rjButton3;

	private RJButton rjButton4;

	private RJButton rjButton5;

	private RJButton rjButton6;

	private RJButton rjButton7;

	private RJButton rjButton8;

	private RJButton rjButton9;

	private RJButton rjButton10;

	private RJButton rjButton11;

	private RJButton rjButton12;

	private RJButton rjButton13;

	private RJButton rjButton14;

	private RJButton rjButton15;

	private RJButton rjButton16;

	private RJButton rjButton17;

	private RJButton rjButton18;

	private RJButton rjButton19;

	private RJButton rjButton20;

	private RJButton rjButton21;

	private RJButton rjButton22;

	private RJButton rjButton23;

	private RJButton rjButton24;

	private RJButton rjButton25;

	private RJButton rjButton26;

	private RJButton rjButton27;

	private RJButton rjButton31;

	private RJButton rjButton32;

	private RJButton rjButton33;

	public FormFunAudio()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
		InitializeSoundFiles();
	}

	private void InitializeSoundFiles()
	{
		soundFiles["rjButton1"] = "discord_message.wav";
		soundFiles["rjButton2"] = "skype_message.wav";
		soundFiles["rjButton3"] = "telegram_message.wav";
		soundFiles["rjButton4"] = "vk_message.wav";
		soundFiles["rjButton5"] = "skype_disconnect.wav";
		soundFiles["rjButton6"] = "discord_disconnect.wav";
		soundFiles["rjButton7"] = "viber_message.wav";
		soundFiles["rjButton8"] = "skype_call.wav";
		soundFiles["rjButton9"] = "discord_call.wav";
		soundFiles["rjButton10"] = "death.wav";
		soundFiles["rjButton11"] = "door_2.wav";
		soundFiles["rjButton12"] = "bass.wav";
		soundFiles["rjButton13"] = "terrorist_win.wav";
		soundFiles["rjButton14"] = "door_1.wav";
		soundFiles["rjButton15"] = "women.wav";
		soundFiles["rjButton16"] = "go go go.wav";
		soundFiles["rjButton17"] = "pocman.wav";
		soundFiles["rjButton18"] = "hello.wav";
		soundFiles["rjButton19"] = "notification.wav";
		soundFiles["rjButton20"] = "disconnect.wav";
		soundFiles["rjButton21"] = "connect.wav";
		soundFiles["rjButton22"] = "error.wav";
		soundFiles["rjButton23"] = "scp.wav";
		soundFiles["rjButton24"] = "orgazm.wav";
		soundFiles["rjButton25"] = "girl bass_bust.wav";
		soundFiles["rjButton26"] = "girl.wav";
		soundFiles["rjButton27"] = "allahy_akbar.wav";
		soundFiles["rjButton31"] = "omlet.wav";
		soundFiles["rjButton32"] = "traxnyt.wav";
		soundFiles["rjButton33"] = "PikaBybyChi.wav";
	}

	private void FormFunAudio_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		MaterialSkinManager.Instance.ColorSchemeChanged += ChangeScheme;
		ChangeScheme(this);
		if (timer1 == null)
		{
			timer1 = new Timer();
			timer1.Interval = 1000;
			timer1.Tick += timer1_Tick;
		}
		timer1.Start();
		foreach (Control control in base.Controls)
		{
			if (control is RJButton)
			{
				control.Click += Button_Click;
			}
		}
	}

	private void ChangeScheme(object sender)
	{
		bool num = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color primary = MaterialSkinManager.Instance.ColorScheme.PrimaryColor;
		Color back = (num ? Color.FromArgb(40, 40, 40) : Color.White);
		Color text = (num ? Color.WhiteSmoke : Color.Black);
		BackColor = back;
		foreach (Control ctrl in base.Controls)
		{
			if (ctrl is RJButton button)
			{
				button.BackgroundColor = primary;
				button.BackColor = primary;
			}
			else if (ctrl is Label label)
			{
				label.ForeColor = text;
			}
			else if (ctrl is Panel panel)
			{
				panel.BackColor = back;
				panel.ForeColor = text;
			}
		}
	}

	private void Button_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			MessageBox.Show("Client is not connected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		else
		{
			if (!(sender is RJButton { Name: var buttonName }))
			{
				return;
			}
			if (soundFiles.ContainsKey(buttonName))
			{
				string soundFileName = soundFiles[buttonName];
				try
				{
					byte[] fileData = GetSoundFromPluginResources(soundFileName);
					if (fileData == null || fileData.Length == 0)
					{
						MessageBox.Show("Failed to load sound: " + soundFileName + "\nCheck if Plugin/FunAudio.dll exists and contains embedded resources.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						return;
					}
					byte[] pack = LEB128.Write(new object[3] { "PlaySoundFromBytes", fileData, 100 });
					client.Send(pack);
					return;
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error playing sound: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
			}
			MessageBox.Show("Sound mapping not found for button: " + buttonName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
	}

	private byte[] GetSoundFromPluginResources(string soundFileName)
	{
		try
		{
			string pluginPath = Path.Combine(Application.StartupPath, "Plugin", "FunAudio.dll");
			if (!File.Exists(pluginPath))
			{
				MessageBox.Show("Plugin not found at: " + pluginPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return null;
			}
			Assembly assembly = Assembly.LoadFrom(pluginPath);
			string resourceName = "FunAudio.Sounds." + soundFileName;
			string[] allResources = assembly.GetManifestResourceNames();
			using Stream stream = assembly.GetManifestResourceStream(resourceName);
			if (stream == null)
			{
				string availableResources = string.Join("\n", allResources.Take(10));
				MessageBox.Show("Resource not found: " + resourceName + "\n\nFirst 10 available resources:\n" + availableResources, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return null;
			}
			byte[] buffer = new byte[stream.Length];
			stream.Read(buffer, 0, buffer.Length);
			return buffer;
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error loading resource: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return null;
		}
	}

	public void StopAllSounds()
	{
		if (client == null || !client.itsConnect)
		{
			return;
		}
		try
		{
			byte[] pack = LEB128.Write(new object[1] { "StopAllSounds" });
			client.Send(pack);
		}
		catch (Exception)
		{
		}
	}

	private void Closing1(object sender, FormClosingEventArgs e)
	{
		if (client != null)
		{
			client.Disconnect();
		}
		if (timer1 != null)
		{
			timer1.Stop();
			timer1.Dispose();
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
		this.materialLabel1 = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		this.rjButton2 = new CustomControls.RJControls.RJButton();
		this.rjButton3 = new CustomControls.RJControls.RJButton();
		this.rjButton4 = new CustomControls.RJControls.RJButton();
		this.rjButton5 = new CustomControls.RJControls.RJButton();
		this.rjButton6 = new CustomControls.RJControls.RJButton();
		this.rjButton7 = new CustomControls.RJControls.RJButton();
		this.rjButton8 = new CustomControls.RJControls.RJButton();
		this.rjButton9 = new CustomControls.RJControls.RJButton();
		this.rjButton10 = new CustomControls.RJControls.RJButton();
		this.rjButton11 = new CustomControls.RJControls.RJButton();
		this.rjButton12 = new CustomControls.RJControls.RJButton();
		this.rjButton13 = new CustomControls.RJControls.RJButton();
		this.rjButton14 = new CustomControls.RJControls.RJButton();
		this.rjButton15 = new CustomControls.RJControls.RJButton();
		this.rjButton16 = new CustomControls.RJControls.RJButton();
		this.rjButton17 = new CustomControls.RJControls.RJButton();
		this.rjButton18 = new CustomControls.RJControls.RJButton();
		this.rjButton19 = new CustomControls.RJControls.RJButton();
		this.rjButton20 = new CustomControls.RJControls.RJButton();
		this.rjButton21 = new CustomControls.RJControls.RJButton();
		this.rjButton22 = new CustomControls.RJControls.RJButton();
		this.rjButton23 = new CustomControls.RJControls.RJButton();
		this.rjButton24 = new CustomControls.RJControls.RJButton();
		this.rjButton25 = new CustomControls.RJControls.RJButton();
		this.rjButton26 = new CustomControls.RJControls.RJButton();
		this.rjButton27 = new CustomControls.RJControls.RJButton();
		this.rjButton31 = new CustomControls.RJControls.RJButton();
		this.rjButton32 = new CustomControls.RJControls.RJButton();
		this.rjButton33 = new CustomControls.RJControls.RJButton();
		base.SuspendLayout();
		this.materialLabel1.AutoSize = true;
		this.materialLabel1.Font = new System.Drawing.Font("Cambria", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.materialLabel1.ForeColor = System.Drawing.Color.Black;
		this.materialLabel1.Location = new System.Drawing.Point(188, 82);
		this.materialLabel1.MaximumSize = new System.Drawing.Size(270, 0);
		this.materialLabel1.Name = "materialLabel1";
		this.materialLabel1.Size = new System.Drawing.Size(105, 22);
		this.materialLabel1.TabIndex = 5;
		this.materialLabel1.Text = "Messangers";
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Cambria", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label1.ForeColor = System.Drawing.Color.Black;
		this.label1.Location = new System.Drawing.Point(201, 222);
		this.label1.MaximumSize = new System.Drawing.Size(270, 0);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(73, 22);
		this.label1.TabIndex = 6;
		this.label1.Text = "Trolling";
		this.rjButton1.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton1.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton1.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton1.BorderRadius = 0;
		this.rjButton1.BorderSize = 0;
		this.rjButton1.FlatAppearance.BorderSize = 0;
		this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton1.ForeColor = System.Drawing.Color.White;
		this.rjButton1.Location = new System.Drawing.Point(43, 107);
		this.rjButton1.Name = "rjButton1";
		this.rjButton1.Size = new System.Drawing.Size(129, 31);
		this.rjButton1.TabIndex = 67;
		this.rjButton1.Text = "Discord Message";
		this.rjButton1.TextColor = System.Drawing.Color.White;
		this.rjButton1.UseVisualStyleBackColor = false;
		this.rjButton2.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton2.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton2.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton2.BorderRadius = 0;
		this.rjButton2.BorderSize = 0;
		this.rjButton2.FlatAppearance.BorderSize = 0;
		this.rjButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton2.ForeColor = System.Drawing.Color.White;
		this.rjButton2.Location = new System.Drawing.Point(176, 107);
		this.rjButton2.Name = "rjButton2";
		this.rjButton2.Size = new System.Drawing.Size(129, 31);
		this.rjButton2.TabIndex = 68;
		this.rjButton2.Text = "Skype Message";
		this.rjButton2.TextColor = System.Drawing.Color.White;
		this.rjButton2.UseVisualStyleBackColor = false;
		this.rjButton3.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton3.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton3.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton3.BorderRadius = 0;
		this.rjButton3.BorderSize = 0;
		this.rjButton3.FlatAppearance.BorderSize = 0;
		this.rjButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton3.ForeColor = System.Drawing.Color.White;
		this.rjButton3.Location = new System.Drawing.Point(311, 107);
		this.rjButton3.Name = "rjButton3";
		this.rjButton3.Size = new System.Drawing.Size(129, 31);
		this.rjButton3.TabIndex = 69;
		this.rjButton3.Text = "Telegram Message";
		this.rjButton3.TextColor = System.Drawing.Color.White;
		this.rjButton3.UseVisualStyleBackColor = false;
		this.rjButton4.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton4.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton4.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton4.BorderRadius = 0;
		this.rjButton4.BorderSize = 0;
		this.rjButton4.FlatAppearance.BorderSize = 0;
		this.rjButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton4.ForeColor = System.Drawing.Color.White;
		this.rjButton4.Location = new System.Drawing.Point(311, 144);
		this.rjButton4.Name = "rjButton4";
		this.rjButton4.Size = new System.Drawing.Size(129, 31);
		this.rjButton4.TabIndex = 72;
		this.rjButton4.Text = "VK Message";
		this.rjButton4.TextColor = System.Drawing.Color.White;
		this.rjButton4.UseVisualStyleBackColor = false;
		this.rjButton5.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton5.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton5.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton5.BorderRadius = 0;
		this.rjButton5.BorderSize = 0;
		this.rjButton5.FlatAppearance.BorderSize = 0;
		this.rjButton5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton5.ForeColor = System.Drawing.Color.White;
		this.rjButton5.Location = new System.Drawing.Point(176, 144);
		this.rjButton5.Name = "rjButton5";
		this.rjButton5.Size = new System.Drawing.Size(129, 31);
		this.rjButton5.TabIndex = 71;
		this.rjButton5.Text = "Skype Disconnect";
		this.rjButton5.TextColor = System.Drawing.Color.White;
		this.rjButton5.UseVisualStyleBackColor = false;
		this.rjButton6.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton6.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton6.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton6.BorderRadius = 0;
		this.rjButton6.BorderSize = 0;
		this.rjButton6.FlatAppearance.BorderSize = 0;
		this.rjButton6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton6.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton6.ForeColor = System.Drawing.Color.White;
		this.rjButton6.Location = new System.Drawing.Point(43, 144);
		this.rjButton6.Name = "rjButton6";
		this.rjButton6.Size = new System.Drawing.Size(129, 31);
		this.rjButton6.TabIndex = 70;
		this.rjButton6.Text = "Discord Disconnect";
		this.rjButton6.TextColor = System.Drawing.Color.White;
		this.rjButton6.UseVisualStyleBackColor = false;
		this.rjButton7.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton7.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton7.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton7.BorderRadius = 0;
		this.rjButton7.BorderSize = 0;
		this.rjButton7.FlatAppearance.BorderSize = 0;
		this.rjButton7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton7.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton7.ForeColor = System.Drawing.Color.White;
		this.rjButton7.Location = new System.Drawing.Point(311, 181);
		this.rjButton7.Name = "rjButton7";
		this.rjButton7.Size = new System.Drawing.Size(129, 31);
		this.rjButton7.TabIndex = 75;
		this.rjButton7.Text = "Viber Message";
		this.rjButton7.TextColor = System.Drawing.Color.White;
		this.rjButton7.UseVisualStyleBackColor = false;
		this.rjButton8.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton8.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton8.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton8.BorderRadius = 0;
		this.rjButton8.BorderSize = 0;
		this.rjButton8.FlatAppearance.BorderSize = 0;
		this.rjButton8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton8.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton8.ForeColor = System.Drawing.Color.White;
		this.rjButton8.Location = new System.Drawing.Point(176, 181);
		this.rjButton8.Name = "rjButton8";
		this.rjButton8.Size = new System.Drawing.Size(129, 31);
		this.rjButton8.TabIndex = 74;
		this.rjButton8.Text = "Skype Call";
		this.rjButton8.TextColor = System.Drawing.Color.White;
		this.rjButton8.UseVisualStyleBackColor = false;
		this.rjButton9.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton9.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton9.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton9.BorderRadius = 0;
		this.rjButton9.BorderSize = 0;
		this.rjButton9.FlatAppearance.BorderSize = 0;
		this.rjButton9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton9.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton9.ForeColor = System.Drawing.Color.White;
		this.rjButton9.Location = new System.Drawing.Point(43, 181);
		this.rjButton9.Name = "rjButton9";
		this.rjButton9.Size = new System.Drawing.Size(129, 31);
		this.rjButton9.TabIndex = 73;
		this.rjButton9.Text = "Discord Call";
		this.rjButton9.TextColor = System.Drawing.Color.White;
		this.rjButton9.UseVisualStyleBackColor = false;
		this.rjButton10.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton10.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton10.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton10.BorderRadius = 0;
		this.rjButton10.BorderSize = 0;
		this.rjButton10.FlatAppearance.BorderSize = 0;
		this.rjButton10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton10.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton10.ForeColor = System.Drawing.Color.White;
		this.rjButton10.Location = new System.Drawing.Point(311, 330);
		this.rjButton10.Name = "rjButton10";
		this.rjButton10.Size = new System.Drawing.Size(129, 31);
		this.rjButton10.TabIndex = 84;
		this.rjButton10.Text = "death";
		this.rjButton10.TextColor = System.Drawing.Color.White;
		this.rjButton10.UseVisualStyleBackColor = false;
		this.rjButton11.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton11.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton11.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton11.BorderRadius = 0;
		this.rjButton11.BorderSize = 0;
		this.rjButton11.FlatAppearance.BorderSize = 0;
		this.rjButton11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton11.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton11.ForeColor = System.Drawing.Color.White;
		this.rjButton11.Location = new System.Drawing.Point(176, 330);
		this.rjButton11.Name = "rjButton11";
		this.rjButton11.Size = new System.Drawing.Size(129, 31);
		this.rjButton11.TabIndex = 83;
		this.rjButton11.Text = "door 2";
		this.rjButton11.TextColor = System.Drawing.Color.White;
		this.rjButton11.UseVisualStyleBackColor = false;
		this.rjButton12.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton12.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton12.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton12.BorderRadius = 0;
		this.rjButton12.BorderSize = 0;
		this.rjButton12.FlatAppearance.BorderSize = 0;
		this.rjButton12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton12.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton12.ForeColor = System.Drawing.Color.White;
		this.rjButton12.Location = new System.Drawing.Point(43, 330);
		this.rjButton12.Name = "rjButton12";
		this.rjButton12.Size = new System.Drawing.Size(129, 31);
		this.rjButton12.TabIndex = 82;
		this.rjButton12.Text = "bass";
		this.rjButton12.TextColor = System.Drawing.Color.White;
		this.rjButton12.UseVisualStyleBackColor = false;
		this.rjButton13.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton13.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton13.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton13.BorderRadius = 0;
		this.rjButton13.BorderSize = 0;
		this.rjButton13.FlatAppearance.BorderSize = 0;
		this.rjButton13.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton13.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton13.ForeColor = System.Drawing.Color.White;
		this.rjButton13.Location = new System.Drawing.Point(311, 293);
		this.rjButton13.Name = "rjButton13";
		this.rjButton13.Size = new System.Drawing.Size(129, 31);
		this.rjButton13.TabIndex = 81;
		this.rjButton13.Text = "terrorist win";
		this.rjButton13.TextColor = System.Drawing.Color.White;
		this.rjButton13.UseVisualStyleBackColor = false;
		this.rjButton14.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton14.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton14.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton14.BorderRadius = 0;
		this.rjButton14.BorderSize = 0;
		this.rjButton14.FlatAppearance.BorderSize = 0;
		this.rjButton14.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton14.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton14.ForeColor = System.Drawing.Color.White;
		this.rjButton14.Location = new System.Drawing.Point(176, 293);
		this.rjButton14.Name = "rjButton14";
		this.rjButton14.Size = new System.Drawing.Size(129, 31);
		this.rjButton14.TabIndex = 80;
		this.rjButton14.Text = "door 1";
		this.rjButton14.TextColor = System.Drawing.Color.White;
		this.rjButton14.UseVisualStyleBackColor = false;
		this.rjButton15.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton15.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton15.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton15.BorderRadius = 0;
		this.rjButton15.BorderSize = 0;
		this.rjButton15.FlatAppearance.BorderSize = 0;
		this.rjButton15.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton15.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton15.ForeColor = System.Drawing.Color.White;
		this.rjButton15.Location = new System.Drawing.Point(43, 293);
		this.rjButton15.Name = "rjButton15";
		this.rjButton15.Size = new System.Drawing.Size(129, 31);
		this.rjButton15.TabIndex = 79;
		this.rjButton15.Text = "women";
		this.rjButton15.TextColor = System.Drawing.Color.White;
		this.rjButton15.UseVisualStyleBackColor = false;
		this.rjButton16.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton16.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton16.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton16.BorderRadius = 0;
		this.rjButton16.BorderSize = 0;
		this.rjButton16.FlatAppearance.BorderSize = 0;
		this.rjButton16.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton16.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton16.ForeColor = System.Drawing.Color.White;
		this.rjButton16.Location = new System.Drawing.Point(311, 256);
		this.rjButton16.Name = "rjButton16";
		this.rjButton16.Size = new System.Drawing.Size(129, 31);
		this.rjButton16.TabIndex = 78;
		this.rjButton16.Text = "go go go";
		this.rjButton16.TextColor = System.Drawing.Color.White;
		this.rjButton16.UseVisualStyleBackColor = false;
		this.rjButton17.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton17.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton17.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton17.BorderRadius = 0;
		this.rjButton17.BorderSize = 0;
		this.rjButton17.FlatAppearance.BorderSize = 0;
		this.rjButton17.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton17.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton17.ForeColor = System.Drawing.Color.White;
		this.rjButton17.Location = new System.Drawing.Point(176, 256);
		this.rjButton17.Name = "rjButton17";
		this.rjButton17.Size = new System.Drawing.Size(129, 31);
		this.rjButton17.TabIndex = 77;
		this.rjButton17.Text = "Pacman";
		this.rjButton17.TextColor = System.Drawing.Color.White;
		this.rjButton17.UseVisualStyleBackColor = false;
		this.rjButton18.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton18.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton18.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton18.BorderRadius = 0;
		this.rjButton18.BorderSize = 0;
		this.rjButton18.FlatAppearance.BorderSize = 0;
		this.rjButton18.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton18.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton18.ForeColor = System.Drawing.Color.White;
		this.rjButton18.Location = new System.Drawing.Point(43, 256);
		this.rjButton18.Name = "rjButton18";
		this.rjButton18.Size = new System.Drawing.Size(129, 31);
		this.rjButton18.TabIndex = 76;
		this.rjButton18.Text = "Hello";
		this.rjButton18.TextColor = System.Drawing.Color.White;
		this.rjButton18.UseVisualStyleBackColor = false;
		this.rjButton19.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton19.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton19.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton19.BorderRadius = 0;
		this.rjButton19.BorderSize = 0;
		this.rjButton19.FlatAppearance.BorderSize = 0;
		this.rjButton19.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton19.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton19.ForeColor = System.Drawing.Color.White;
		this.rjButton19.Location = new System.Drawing.Point(311, 441);
		this.rjButton19.Name = "rjButton19";
		this.rjButton19.Size = new System.Drawing.Size(129, 31);
		this.rjButton19.TabIndex = 93;
		this.rjButton19.Text = "Notification";
		this.rjButton19.TextColor = System.Drawing.Color.White;
		this.rjButton19.UseVisualStyleBackColor = false;
		this.rjButton20.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton20.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton20.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton20.BorderRadius = 0;
		this.rjButton20.BorderSize = 0;
		this.rjButton20.FlatAppearance.BorderSize = 0;
		this.rjButton20.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton20.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton20.ForeColor = System.Drawing.Color.White;
		this.rjButton20.Location = new System.Drawing.Point(176, 441);
		this.rjButton20.Name = "rjButton20";
		this.rjButton20.Size = new System.Drawing.Size(129, 31);
		this.rjButton20.TabIndex = 92;
		this.rjButton20.Text = "Disconnect";
		this.rjButton20.TextColor = System.Drawing.Color.White;
		this.rjButton20.UseVisualStyleBackColor = false;
		this.rjButton21.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton21.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton21.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton21.BorderRadius = 0;
		this.rjButton21.BorderSize = 0;
		this.rjButton21.FlatAppearance.BorderSize = 0;
		this.rjButton21.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton21.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton21.ForeColor = System.Drawing.Color.White;
		this.rjButton21.Location = new System.Drawing.Point(43, 441);
		this.rjButton21.Name = "rjButton21";
		this.rjButton21.Size = new System.Drawing.Size(129, 31);
		this.rjButton21.TabIndex = 91;
		this.rjButton21.Text = "Connect";
		this.rjButton21.TextColor = System.Drawing.Color.White;
		this.rjButton21.UseVisualStyleBackColor = false;
		this.rjButton22.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton22.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton22.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton22.BorderRadius = 0;
		this.rjButton22.BorderSize = 0;
		this.rjButton22.FlatAppearance.BorderSize = 0;
		this.rjButton22.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton22.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton22.ForeColor = System.Drawing.Color.White;
		this.rjButton22.Location = new System.Drawing.Point(311, 404);
		this.rjButton22.Name = "rjButton22";
		this.rjButton22.Size = new System.Drawing.Size(129, 31);
		this.rjButton22.TabIndex = 90;
		this.rjButton22.Text = "Error";
		this.rjButton22.TextColor = System.Drawing.Color.White;
		this.rjButton22.UseVisualStyleBackColor = false;
		this.rjButton23.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton23.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton23.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton23.BorderRadius = 0;
		this.rjButton23.BorderSize = 0;
		this.rjButton23.FlatAppearance.BorderSize = 0;
		this.rjButton23.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton23.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton23.ForeColor = System.Drawing.Color.White;
		this.rjButton23.Location = new System.Drawing.Point(176, 404);
		this.rjButton23.Name = "rjButton23";
		this.rjButton23.Size = new System.Drawing.Size(129, 31);
		this.rjButton23.TabIndex = 89;
		this.rjButton23.Text = "Scp";
		this.rjButton23.TextColor = System.Drawing.Color.White;
		this.rjButton23.UseVisualStyleBackColor = false;
		this.rjButton24.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton24.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton24.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton24.BorderRadius = 0;
		this.rjButton24.BorderSize = 0;
		this.rjButton24.FlatAppearance.BorderSize = 0;
		this.rjButton24.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton24.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton24.ForeColor = System.Drawing.Color.White;
		this.rjButton24.Location = new System.Drawing.Point(43, 404);
		this.rjButton24.Name = "rjButton24";
		this.rjButton24.Size = new System.Drawing.Size(129, 31);
		this.rjButton24.TabIndex = 88;
		this.rjButton24.Text = "Orgazm";
		this.rjButton24.TextColor = System.Drawing.Color.White;
		this.rjButton24.UseVisualStyleBackColor = false;
		this.rjButton25.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton25.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton25.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton25.BorderRadius = 0;
		this.rjButton25.BorderSize = 0;
		this.rjButton25.FlatAppearance.BorderSize = 0;
		this.rjButton25.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton25.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton25.ForeColor = System.Drawing.Color.White;
		this.rjButton25.Location = new System.Drawing.Point(311, 367);
		this.rjButton25.Name = "rjButton25";
		this.rjButton25.Size = new System.Drawing.Size(129, 31);
		this.rjButton25.TabIndex = 87;
		this.rjButton25.Text = "Get Bass Bust";
		this.rjButton25.TextColor = System.Drawing.Color.White;
		this.rjButton25.UseVisualStyleBackColor = false;
		this.rjButton26.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton26.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton26.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton26.BorderRadius = 0;
		this.rjButton26.BorderSize = 0;
		this.rjButton26.FlatAppearance.BorderSize = 0;
		this.rjButton26.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton26.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton26.ForeColor = System.Drawing.Color.White;
		this.rjButton26.Location = new System.Drawing.Point(176, 367);
		this.rjButton26.Name = "rjButton26";
		this.rjButton26.Size = new System.Drawing.Size(129, 31);
		this.rjButton26.TabIndex = 86;
		this.rjButton26.Text = "Girl";
		this.rjButton26.TextColor = System.Drawing.Color.White;
		this.rjButton26.UseVisualStyleBackColor = false;
		this.rjButton27.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton27.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton27.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton27.BorderRadius = 0;
		this.rjButton27.BorderSize = 0;
		this.rjButton27.FlatAppearance.BorderSize = 0;
		this.rjButton27.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton27.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton27.ForeColor = System.Drawing.Color.White;
		this.rjButton27.Location = new System.Drawing.Point(43, 367);
		this.rjButton27.Name = "rjButton27";
		this.rjButton27.Size = new System.Drawing.Size(129, 31);
		this.rjButton27.TabIndex = 85;
		this.rjButton27.Text = "Alahy Akbar";
		this.rjButton27.TextColor = System.Drawing.Color.White;
		this.rjButton27.UseVisualStyleBackColor = false;
		this.rjButton31.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton31.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton31.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton31.BorderRadius = 0;
		this.rjButton31.BorderSize = 0;
		this.rjButton31.FlatAppearance.BorderSize = 0;
		this.rjButton31.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton31.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton31.ForeColor = System.Drawing.Color.White;
		this.rjButton31.Location = new System.Drawing.Point(311, 478);
		this.rjButton31.Name = "rjButton31";
		this.rjButton31.Size = new System.Drawing.Size(129, 31);
		this.rjButton31.TabIndex = 96;
		this.rjButton31.Text = "Omlet";
		this.rjButton31.TextColor = System.Drawing.Color.White;
		this.rjButton31.UseVisualStyleBackColor = false;
		this.rjButton32.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton32.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton32.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton32.BorderRadius = 0;
		this.rjButton32.BorderSize = 0;
		this.rjButton32.FlatAppearance.BorderSize = 0;
		this.rjButton32.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton32.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton32.ForeColor = System.Drawing.Color.White;
		this.rjButton32.Location = new System.Drawing.Point(176, 478);
		this.rjButton32.Name = "rjButton32";
		this.rjButton32.Size = new System.Drawing.Size(129, 31);
		this.rjButton32.TabIndex = 95;
		this.rjButton32.Text = "traxnyt.mp3";
		this.rjButton32.TextColor = System.Drawing.Color.White;
		this.rjButton32.UseVisualStyleBackColor = false;
		this.rjButton33.BackColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton33.BackgroundColor = System.Drawing.Color.FromArgb(152, 251, 152);
		this.rjButton33.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton33.BorderRadius = 0;
		this.rjButton33.BorderSize = 0;
		this.rjButton33.FlatAppearance.BorderSize = 0;
		this.rjButton33.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton33.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjButton33.ForeColor = System.Drawing.Color.White;
		this.rjButton33.Location = new System.Drawing.Point(43, 478);
		this.rjButton33.Name = "rjButton33";
		this.rjButton33.Size = new System.Drawing.Size(129, 31);
		this.rjButton33.TabIndex = 94;
		this.rjButton33.Text = "PikaBybyChi";
		this.rjButton33.TextColor = System.Drawing.Color.White;
		this.rjButton33.UseVisualStyleBackColor = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(480, 541);
		base.Controls.Add(this.rjButton31);
		base.Controls.Add(this.rjButton32);
		base.Controls.Add(this.rjButton33);
		base.Controls.Add(this.rjButton19);
		base.Controls.Add(this.rjButton20);
		base.Controls.Add(this.rjButton21);
		base.Controls.Add(this.rjButton22);
		base.Controls.Add(this.rjButton23);
		base.Controls.Add(this.rjButton24);
		base.Controls.Add(this.rjButton25);
		base.Controls.Add(this.rjButton26);
		base.Controls.Add(this.rjButton27);
		base.Controls.Add(this.rjButton10);
		base.Controls.Add(this.rjButton11);
		base.Controls.Add(this.rjButton12);
		base.Controls.Add(this.rjButton13);
		base.Controls.Add(this.rjButton14);
		base.Controls.Add(this.rjButton15);
		base.Controls.Add(this.rjButton16);
		base.Controls.Add(this.rjButton17);
		base.Controls.Add(this.rjButton18);
		base.Controls.Add(this.rjButton7);
		base.Controls.Add(this.rjButton8);
		base.Controls.Add(this.rjButton9);
		base.Controls.Add(this.rjButton4);
		base.Controls.Add(this.rjButton5);
		base.Controls.Add(this.rjButton6);
		base.Controls.Add(this.rjButton3);
		base.Controls.Add(this.rjButton2);
		base.Controls.Add(this.rjButton1);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.materialLabel1);
		base.Name = "FormFunAudio";
		this.Text = "FunAudio";
		base.Load += new System.EventHandler(FormFunAudio_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
