using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Leb128;
using MaterialSkin;
using MaterialSkin.Controls;
using NAudio.Wave;
using Newtonsoft.Json;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormSounds : FormMaterial
{
	private string selectedFilePath;

	private bool isPlaying;

	private Dictionary<string, string> musicFiles = new Dictionary<string, string>();

	private Timer timer1;

	public Clients client;

	public Clients parrent;

	private IContainer components;

	private MaterialButton materialButton1;

	private MaterialSlider materialSlider1;

	private ListBox materialListBox1;

	private MaterialButton materialButton2;

	private MaterialButton materialButton3;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private ToolStripMenuItem removeAllToolStripMenuItem;

	public FormSounds()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
		LoadMusicList();
	}

	private byte[] ConvertToWav(string inputPath, int volume)
	{
		try
		{
			using AudioFileReader reader = new AudioFileReader(inputPath);
			reader.Volume = (float)volume / 100f;
			using MemoryStream ms = new MemoryStream();
			using (WaveFileWriter writer = new WaveFileWriter(ms, reader.WaveFormat))
			{
				byte[] buffer = new byte[reader.WaveFormat.AverageBytesPerSecond];
				int read;
				while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
				{
					writer.Write(buffer, 0, read);
				}
			}
			return ms.ToArray();
		}
		catch (Exception ex)
		{
			throw new Exception("Conversion to WAV failed: " + ex.Message);
		}
	}

	private void FormSounds_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		if (timer1 == null)
		{
			timer1 = new Timer();
			timer1.Interval = 1000;
			timer1.Tick += timer1_Tick;
		}
		timer1.Start();
		if (materialButton2 != null)
		{
			materialButton2.Click += MaterialButtonAddMusic_Click;
		}
		if (materialButton1 != null)
		{
			materialButton1.Click += MaterialButtonPlaySound_Click;
		}
		if (materialListBox1 != null)
		{
			materialListBox1.SelectedIndexChanged += MaterialListBox1_SelectedIndexChanged;
		}
		if (materialSlider1 == null)
		{
			return;
		}
		try
		{
			materialSlider1.Value = 100;
			PropertyInfo minProperty = materialSlider1.GetType().GetProperty("RangeMin");
			PropertyInfo maxProperty = materialSlider1.GetType().GetProperty("RangeMax");
			if (minProperty != null)
			{
				minProperty.SetValue(materialSlider1, 0);
			}
			if (maxProperty != null)
			{
				maxProperty.SetValue(materialSlider1, 100);
			}
		}
		catch
		{
		}
	}

	private void ChangeScheme(object sender)
	{
		bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color back = (isDark ? Color.FromArgb(40, 40, 40) : Color.White);
		Color text = (isDark ? Color.WhiteSmoke : Color.Black);
		BackColor = back;
		if (materialListBox1 != null)
		{
			materialListBox1.BackColor = back;
			materialListBox1.ForeColor = text;
		}
		if (materialSlider1 != null)
		{
			materialSlider1.ForeColor = (isDark ? Color.WhiteSmoke : Color.Black);
			materialSlider1.BackColor = back;
			materialSlider1.Invalidate();
		}
		foreach (Control ctrl in base.Controls)
		{
			if (ctrl is Panel)
			{
				ctrl.BackColor = back;
				ctrl.ForeColor = text;
			}
		}
	}

	private void LoadMusicList()
	{
		try
		{
			string configPath = Path.Combine(Application.StartupPath, "local", "Sounds.json");
			if (!File.Exists(configPath))
			{
				return;
			}
			Dictionary<string, string> savedFiles = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(configPath));
			if (savedFiles == null)
			{
				return;
			}
			musicFiles = savedFiles;
			foreach (string fileName in musicFiles.Keys)
			{
				if (File.Exists(musicFiles[fileName]))
				{
					materialListBox1.Items.Add(fileName);
				}
			}
		}
		catch
		{
		}
	}

	private void SaveMusicList()
	{
		try
		{
			string dir = Path.Combine(Application.StartupPath, "local");
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			string path = Path.Combine(dir, "Sounds.json");
			string json = JsonConvert.SerializeObject(musicFiles, Formatting.Indented);
			File.WriteAllText(path, json);
		}
		catch
		{
		}
	}

	private void MaterialButtonAddMusic_Click(object sender, EventArgs e)
	{
		using OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "Audio files|*.mp3;*.wav;*.wma;*.m4a;*.aac;*.flac;*.ogg|All files|*.*";
		openFileDialog.FilterIndex = 1;
		openFileDialog.RestoreDirectory = true;
		openFileDialog.Multiselect = true;
		if (openFileDialog.ShowDialog() != DialogResult.OK || materialListBox1 == null)
		{
			return;
		}
		bool changed = false;
		string[] fileNames = openFileDialog.FileNames;
		foreach (string filePath in fileNames)
		{
			if (File.Exists(filePath))
			{
				string fileName = Path.GetFileName(filePath);
				if (!musicFiles.ContainsKey(fileName))
				{
					musicFiles[fileName] = filePath;
					materialListBox1.Items.Add(fileName);
					changed = true;
				}
			}
		}
		if (changed)
		{
			SaveMusicList();
		}
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (materialListBox1.SelectedItem != null)
		{
			string selectedFileName = materialListBox1.SelectedItem.ToString();
			musicFiles.Remove(selectedFileName);
			materialListBox1.Items.RemoveAt(materialListBox1.SelectedIndex);
			SaveMusicList();
		}
	}

	private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
	{
		musicFiles.Clear();
		materialListBox1.Items.Clear();
		SaveMusicList();
	}

	private void MaterialListBox1_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (materialListBox1 != null && materialListBox1.SelectedItem != null)
		{
			string selectedFileName = materialListBox1.SelectedItem.ToString();
			if (!string.IsNullOrEmpty(selectedFileName) && musicFiles.ContainsKey(selectedFileName))
			{
				selectedFilePath = musicFiles[selectedFileName];
			}
		}
	}

	private void MaterialButtonPlaySound_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			MessageBox.Show("Client not connected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		if (materialListBox1 == null || materialListBox1.SelectedItem == null)
		{
			MessageBox.Show("Please select a music file from the list first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		string selectedFileName = materialListBox1.SelectedItem.ToString();
		if (string.IsNullOrEmpty(selectedFileName) || !musicFiles.ContainsKey(selectedFileName))
		{
			MessageBox.Show("Selected file not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		selectedFilePath = musicFiles[selectedFileName];
		if (string.IsNullOrEmpty(selectedFilePath) || !File.Exists(selectedFilePath))
		{
			MessageBox.Show("Selected file not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		try
		{
			int volume = GetSliderValue();
			byte[] fileData = File.ReadAllBytes(selectedFilePath);
			byte[] pack = LEB128.Write(new object[4]
			{
				"PlaySoundFromBytes",
				fileData,
				volume,
				Path.GetExtension(selectedFilePath)
			});
			client.Send(pack);
			isPlaying = true;
			if (materialButton1 != null)
			{
				materialButton1.Text = "Playing...";
				materialButton1.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error reading sound file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void MaterialButtonStopSound_Click(object sender, EventArgs e)
	{
		StopSound();
	}

	private void StopSound()
	{
		if (client == null || !client.itsConnect)
		{
			return;
		}
		byte[] pack = LEB128.Write(new object[1] { "StopSound" });
		client.Send(pack);
		isPlaying = false;
		if (materialButton1 != null)
		{
			materialButton1.Invoke((MethodInvoker)delegate
			{
				materialButton1.Text = "Play Sound";
				materialButton1.Enabled = true;
			});
		}
	}

	private void MaterialSlider1_MouseUp(object sender, MouseEventArgs e)
	{
		if (client != null && client.itsConnect && isPlaying)
		{
			int volume = GetSliderValue();
			byte[] pack = LEB128.Write(new object[2] { "SetVolume", volume });
			client.Send(pack);
		}
	}

	private void MaterialSlider1_MouseMove(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left && client != null && client.itsConnect && isPlaying)
		{
			int volume = GetSliderValue();
			byte[] pack = LEB128.Write(new object[2] { "SetVolume", volume });
			client.Send(pack);
		}
	}

	private int GetSliderValue()
	{
		if (materialSlider1 == null)
		{
			return 100;
		}
		try
		{
			return materialSlider1.Value;
		}
		catch
		{
			try
			{
				PropertyInfo valueProperty = materialSlider1.GetType().GetProperty("Value");
				if (valueProperty != null)
				{
					return (int)valueProperty.GetValue(materialSlider1);
				}
			}
			catch
			{
			}
			return 100;
		}
	}

	private void Closing1(object sender, EventArgs e)
	{
		if (client != null)
		{
			if (isPlaying)
			{
				StopSound();
			}
			client.Disconnect();
		}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormSounds));
		this.materialButton1 = new MaterialSkin.Controls.MaterialButton();
		this.materialSlider1 = new MaterialSkin.Controls.MaterialSlider();
		this.materialListBox1 = new System.Windows.Forms.ListBox();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.materialButton2 = new MaterialSkin.Controls.MaterialButton();
		this.materialButton3 = new MaterialSkin.Controls.MaterialButton();
		this.contextMenuStrip1.SuspendLayout();
		base.SuspendLayout();
		this.materialButton1.AutoSize = false;
		this.materialButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.materialButton1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.materialButton1.Depth = 0;
		this.materialButton1.HighEmphasis = true;
		this.materialButton1.Icon = null;
		this.materialButton1.Location = new System.Drawing.Point(6, 278);
		this.materialButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialButton1.Name = "materialButton1";
		this.materialButton1.NoAccentTextColor = System.Drawing.Color.Empty;
		this.materialButton1.Size = new System.Drawing.Size(148, 36);
		this.materialButton1.TabIndex = 0;
		this.materialButton1.Text = "Play Sound";
		this.materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.materialButton1.UseAccentColor = false;
		this.materialButton1.UseVisualStyleBackColor = true;
		this.materialSlider1.Depth = 0;
		this.materialSlider1.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.materialSlider1.Location = new System.Drawing.Point(6, 193);
		this.materialSlider1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSlider1.Name = "materialSlider1";
		this.materialSlider1.Size = new System.Drawing.Size(301, 40);
		this.materialSlider1.TabIndex = 1;
		this.materialSlider1.Text = "Volume";
		this.materialSlider1.Value = 100;
		this.materialSlider1.MouseMove += new System.Windows.Forms.MouseEventHandler(MaterialSlider1_MouseMove);
		this.materialSlider1.MouseUp += new System.Windows.Forms.MouseEventHandler(MaterialSlider1_MouseUp);
		this.materialListBox1.BackColor = System.Drawing.Color.White;
		this.materialListBox1.ContextMenuStrip = this.contextMenuStrip1;
		this.materialListBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.materialListBox1.ForeColor = System.Drawing.Color.Black;
		this.materialListBox1.FormattingEnabled = true;
		this.materialListBox1.ItemHeight = 15;
		this.materialListBox1.Location = new System.Drawing.Point(6, 67);
		this.materialListBox1.Name = "materialListBox1";
		this.materialListBox1.Size = new System.Drawing.Size(301, 109);
		this.materialListBox1.TabIndex = 2;
		this.contextMenuStrip1.BackColor = System.Drawing.Color.White;
		this.contextMenuStrip1.ForeColor = System.Drawing.Color.Black;
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.removeToolStripMenuItem, this.removeAllToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(135, 48);
		this.removeToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.removeToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
		this.removeToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("removeToolStripMenuItem.Image");
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(removeToolStripMenuItem_Click);
		this.removeAllToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.removeAllToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
		this.removeAllToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("removeAllToolStripMenuItem.Image");
		this.removeAllToolStripMenuItem.Name = "removeAllToolStripMenuItem";
		this.removeAllToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
		this.removeAllToolStripMenuItem.Text = "Remove All";
		this.removeAllToolStripMenuItem.Click += new System.EventHandler(removeAllToolStripMenuItem_Click);
		this.materialButton2.AutoSize = false;
		this.materialButton2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.materialButton2.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.materialButton2.Depth = 0;
		this.materialButton2.HighEmphasis = true;
		this.materialButton2.Icon = null;
		this.materialButton2.Location = new System.Drawing.Point(6, 239);
		this.materialButton2.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.materialButton2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialButton2.Name = "materialButton2";
		this.materialButton2.NoAccentTextColor = System.Drawing.Color.Empty;
		this.materialButton2.Size = new System.Drawing.Size(301, 36);
		this.materialButton2.TabIndex = 3;
		this.materialButton2.Text = "Add music";
		this.materialButton2.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.materialButton2.UseAccentColor = false;
		this.materialButton2.UseVisualStyleBackColor = true;
		this.materialButton3.AutoSize = false;
		this.materialButton3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.materialButton3.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.materialButton3.Depth = 0;
		this.materialButton3.HighEmphasis = true;
		this.materialButton3.Icon = null;
		this.materialButton3.Location = new System.Drawing.Point(159, 278);
		this.materialButton3.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.materialButton3.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialButton3.Name = "materialButton3";
		this.materialButton3.NoAccentTextColor = System.Drawing.Color.Empty;
		this.materialButton3.Size = new System.Drawing.Size(148, 36);
		this.materialButton3.TabIndex = 4;
		this.materialButton3.Text = "Stop Sound";
		this.materialButton3.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.materialButton3.UseAccentColor = false;
		this.materialButton3.UseVisualStyleBackColor = true;
		this.materialButton3.Click += new System.EventHandler(MaterialButtonStopSound_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(315, 321);
		base.Controls.Add(this.materialButton3);
		base.Controls.Add(this.materialButton2);
		base.Controls.Add(this.materialListBox1);
		base.Controls.Add(this.materialSlider1);
		base.Controls.Add(this.materialButton1);
		base.Name = "FormSounds";
		this.Text = "Sounder";
		base.Load += new System.EventHandler(FormSounds_Load);
		this.contextMenuStrip1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
