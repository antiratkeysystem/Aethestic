using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using Newtonsoft.Json;
using Server.Data;
using Server.Helper;

namespace Server.Forms;

public class FormCustombackground : FormMaterial
{
	private string selectedPath = "";

	private Image originalImage;

	private PictureBox selectedPictureBox;

	private IContainer components;

	private PictureBox pictureBox1;

	private MaterialSlider materialSlider1;

	private MaterialButton materialButton1;

	private MaterialButton materialButton2;

	private FlowLayoutPanel slotsPanel;

	public FormCustombackground()
	{
		InitializeComponent();
		base.Load += FormCustombackground_Load;
	}

	private void SetupSlotsPanel()
	{
		slotsPanel.WrapContents = false;
		slotsPanel.FlowDirection = FlowDirection.LeftToRight;
		slotsPanel.BackColor = Color.Transparent;
	}

	private void FormCustombackground_Load(object sender, EventArgs e)
	{
		SetupSlotsPanel();
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		if (Program.form != null && Program.form.settings != null)
		{
			materialSlider1.Value = Program.form.settings.BackgroundOpacity;
			if (!string.IsNullOrEmpty(Program.form.settings.BackgroundPath) && File.Exists(Program.form.settings.BackgroundPath))
			{
				selectedPath = Program.form.settings.BackgroundPath;
				LoadPreview(selectedPath);
				SelectPictureBox(pictureBox1);
			}
			if (Program.form.settings.BackgroundSlots == null)
			{
				Program.form.settings.BackgroundSlots = new string[10];
			}
		}
		materialButton2.Click += MaterialButton2_Click;
		materialButton1.Click += MaterialButton1_Click;
		materialSlider1.onValueChanged += MaterialSlider1_ValueChanged;
		base.FormClosing += FormCustombackground_FormClosing;
		pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
		pictureBox1.Click += PictureBox1_Click;
		pictureBox1.Cursor = Cursors.Hand;
		LoadSavedPresets();
	}

	private void LoadSavedPresets()
	{
		try
		{
			slotsPanel.Controls.Clear();
			Settings settings = Program.form.settings;
			for (int i = 0; i < settings.BackgroundSlots.Length; i++)
			{
				PictureBox slot = new PictureBox();
				slot.Size = new Size(150, 90);
				slot.SizeMode = PictureBoxSizeMode.Zoom;
				slot.BorderStyle = BorderStyle.FixedSingle;
				slot.Margin = new Padding(5);
				slot.Cursor = Cursors.Hand;
				slot.Tag = i;
				string path = settings.BackgroundSlots[i];
				if (!string.IsNullOrEmpty(path) && File.Exists(path))
				{
					try
					{
						slot.Image = Image.FromFile(path);
					}
					catch
					{
					}
				}
				else
				{
					slot.BackColor = Color.FromArgb(40, 40, 40);
				}
				slot.Click += Slot_Click;
				slot.DoubleClick += Slot_DoubleClick;
				ContextMenuStrip cms = new ContextMenuStrip();
				cms.Items.Add("Clear Slot").Click += delegate
				{
					int num = (int)slot.Tag;
					settings.BackgroundSlots[num] = null;
					if (slot.Image != null)
					{
						slot.Image.Dispose();
					}
					slot.Image = null;
					slot.BackColor = Color.FromArgb(40, 40, 40);
					SaveSettings();
				};
				slot.ContextMenuStrip = cms;
				slotsPanel.Controls.Add(slot);
			}
			ApplyThemeRecursive(slotsPanel, MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK);
		}
		catch
		{
		}
	}

	private void Slot_Click(object sender, EventArgs e)
	{
		PictureBox slot = sender as PictureBox;
		int idx = (int)slot.Tag;
		string path = Program.form.settings.BackgroundSlots[idx];
		if (!string.IsNullOrEmpty(path) && File.Exists(path))
		{
			selectedPath = path;
			LoadPreview(selectedPath);
			SelectPictureBox(slot);
		}
	}

	private void Slot_DoubleClick(object sender, EventArgs e)
	{
		PictureBox slot = sender as PictureBox;
		int idx = (int)slot.Tag;
		using OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
		openFileDialog.Title = "Select Background for Slot " + (idx + 1);
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			string path = openFileDialog.FileName;
			Program.form.settings.BackgroundSlots[idx] = path;
			if (slot.Image != null)
			{
				slot.Image.Dispose();
			}
			try
			{
				slot.Image = Image.FromFile(path);
			}
			catch
			{
			}
			slot.BackColor = Color.Transparent;
			selectedPath = path;
			LoadPreview(selectedPath);
			SelectPictureBox(slot);
			SaveSettings();
		}
	}

	private void SaveSettings()
	{
		try
		{
			File.WriteAllText("local\\Settings.json", JsonConvert.SerializeObject(Program.form.settings));
		}
		catch
		{
		}
	}

	private void SelectPictureBox(PictureBox pictureBox)
	{
		pictureBox1.BorderStyle = BorderStyle.None;
		foreach (Control control in slotsPanel.Controls)
		{
			if (control is PictureBox pb)
			{
				pb.BorderStyle = BorderStyle.FixedSingle;
			}
		}
		pictureBox.BorderStyle = BorderStyle.Fixed3D;
		selectedPictureBox = pictureBox;
	}

	private void PictureBox1_Click(object sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty(selectedPath))
		{
			SelectPictureBox(pictureBox1);
		}
	}

	private void PictureBox2_Click(object sender, EventArgs e)
	{
	}

	private void PictureBox3_Click(object sender, EventArgs e)
	{
	}

	private void PictureBox2_DoubleClick(object sender, EventArgs e)
	{
	}

	private void PictureBox3_DoubleClick(object sender, EventArgs e)
	{
	}

	private void MaterialSlider1_ValueChanged(object sender, int newValue)
	{
		if (originalImage != null)
		{
			UpdatePreviewWithOpacity(newValue);
		}
	}

	private void UpdatePreviewWithOpacity(int opacityValue)
	{
		try
		{
			if (originalImage == null)
			{
				return;
			}
			if (pictureBox1.Image != null && pictureBox1.Image != originalImage)
			{
				pictureBox1.Image.Dispose();
			}
			Bitmap previewImage = new Bitmap(originalImage.Width, originalImage.Height);
			using (Graphics g = Graphics.FromImage(previewImage))
			{
				g.DrawImage(originalImage, 0, 0, originalImage.Width, originalImage.Height);
				float opacity = (float)opacityValue / 100f;
				using SolidBrush brush = new SolidBrush(Color.FromArgb((int)((1f - opacity) * 255f), 0, 0, 0));
				g.FillRectangle(brush, 0, 0, originalImage.Width, originalImage.Height);
			}
			pictureBox1.Image = previewImage;
		}
		catch
		{
		}
	}

	private void LoadPreview(string path)
	{
		try
		{
			if (originalImage != null)
			{
				originalImage.Dispose();
			}
			if (pictureBox1.Image != null && pictureBox1.Image != originalImage)
			{
				pictureBox1.Image.Dispose();
			}
			originalImage = Image.FromFile(path);
			UpdatePreviewWithOpacity(materialSlider1.Value);
		}
		catch
		{
		}
	}

	private void MaterialButton2_Click(object sender, EventArgs e)
	{
		using OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
		openFileDialog.Title = "Select Custom Background";
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			selectedPath = openFileDialog.FileName;
			LoadPreview(selectedPath);
			SelectPictureBox(pictureBox1);
		}
	}

	private void MaterialButton1_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(selectedPath))
		{
			MessageBox.Show("Please select an image first!", "No Image Selected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
		else if (Program.form != null && Program.form.settings != null)
		{
			Program.form.settings.BackgroundPath = selectedPath;
			Program.form.settings.BackgroundOpacity = materialSlider1.Value;
			Program.form.settings.Background = true;
			try
			{
				File.WriteAllText("local\\Settings.json", JsonConvert.SerializeObject(Program.form.settings));
			}
			catch
			{
			}
			if (Program.form != null)
			{
				Program.form.ApplyBackground();
			}
			base.DialogResult = DialogResult.OK;
			Close();
		}
	}

	private void FormCustombackground_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (originalImage != null)
		{
			originalImage.Dispose();
			originalImage = null;
		}
		if (pictureBox1.Image != null && pictureBox1.Image != originalImage)
		{
			pictureBox1.Image.Dispose();
		}
	}

	private void ChangeScheme(object sender)
	{
		bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		ApplyThemeRecursive(this, isDark);
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
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.materialSlider1 = new MaterialSkin.Controls.MaterialSlider();
		this.materialButton1 = new MaterialSkin.Controls.MaterialButton();
		this.materialButton2 = new MaterialSkin.Controls.MaterialButton();
		this.slotsPanel = new System.Windows.Forms.FlowLayoutPanel();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		base.SuspendLayout();
		this.pictureBox1.Location = new System.Drawing.Point(6, 67);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(211, 127);
		this.pictureBox1.TabIndex = 0;
		this.pictureBox1.TabStop = false;
		this.materialSlider1.Depth = 0;
		this.materialSlider1.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.materialSlider1.Location = new System.Drawing.Point(6, 200);
		this.materialSlider1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSlider1.Name = "materialSlider1";
		this.materialSlider1.RangeMin = 0;
		this.materialSlider1.RangeMax = 100;
		this.materialSlider1.Size = new System.Drawing.Size(314, 40);
		this.materialSlider1.TabIndex = 1;
		this.materialSlider1.Text = "opacity";
		this.materialSlider1.Value = 50;
		this.materialButton1.AutoSize = false;
		this.materialButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.materialButton1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.materialButton1.Depth = 0;
		this.materialButton1.HighEmphasis = true;
		this.materialButton1.Icon = null;
		this.materialButton1.Location = new System.Drawing.Point(493, 203);
		this.materialButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialButton1.Name = "materialButton1";
		this.materialButton1.NoAccentTextColor = System.Drawing.Color.Empty;
		this.materialButton1.Size = new System.Drawing.Size(158, 36);
		this.materialButton1.TabIndex = 4;
		this.materialButton1.Text = "SET CHANGE";
		this.materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.materialButton1.UseAccentColor = false;
		this.materialButton1.UseVisualStyleBackColor = true;
		this.materialButton2.AutoSize = false;
		this.materialButton2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.materialButton2.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.materialButton2.Depth = 0;
		this.materialButton2.HighEmphasis = true;
		this.materialButton2.Icon = null;
		this.materialButton2.Location = new System.Drawing.Point(327, 203);
		this.materialButton2.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.materialButton2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialButton2.Name = "materialButton2";
		this.materialButton2.NoAccentTextColor = System.Drawing.Color.Empty;
		this.materialButton2.Size = new System.Drawing.Size(158, 36);
		this.materialButton2.TabIndex = 5;
		this.materialButton2.Text = "CHANGE";
		this.materialButton2.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.materialButton2.UseAccentColor = false;
		this.materialButton2.UseVisualStyleBackColor = true;
		this.slotsPanel.AutoScroll = true;
		this.slotsPanel.Location = new System.Drawing.Point(223, 67);
		this.slotsPanel.Name = "slotsPanel";
		this.slotsPanel.Size = new System.Drawing.Size(435, 127);
		this.slotsPanel.TabIndex = 6;
		this.slotsPanel.WrapContents = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(664, 250);
		base.Controls.Add(this.slotsPanel);
		base.Controls.Add(this.materialButton2);
		base.Controls.Add(this.materialButton1);
		base.Controls.Add(this.materialSlider1);
		base.Controls.Add(this.pictureBox1);
		base.Name = "FormCustombackground";
		this.Text = "Custom background";
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		base.ResumeLayout(false);
	}
}
