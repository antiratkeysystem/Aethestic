using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Leb128;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormNetExecutor : FormMaterial
{
	public Clients parrent;

	public Clients client;

	private IContainer components;

	private ListBox listBoxScripts;

	private MaterialButton buttonStartSelected;

	private MaterialButton buttonRefresh;

	public RichTextBox richTextBoxOutput;

	private MaterialLabel materialLabel1;

	private MaterialLabel materialLabel2;

	private MaterialTextBox textBoxScriptName;

	private MaterialComboBox comboBoxScriptLanguage;

	private RichTextBox richTextBoxScriptCode;

	private MaterialButton buttonSaveScript;

	private MaterialLabel materialLabel3;

	private MaterialLabel materialLabel4;

	private MaterialLabel materialLabel5;

	public FormNetExecutor()
	{
		InitializeComponent();
	}

	private void FormNetExecutor_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		if (parrent != null)
		{
			Text = "NetExecutor - " + parrent.Hwid;
		}
		LoadScripts();
	}

	private void ChangeScheme(object sender)
	{
		bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color back = (isDark ? Color.FromArgb(40, 40, 40) : SystemColors.Control);
		Color editorBack = (isDark ? Color.FromArgb(30, 30, 30) : Color.White);
		if (!isDark)
		{
			_ = SystemColors.ControlText;
		}
		else
		{
			_ = Color.WhiteSmoke;
		}
		BackColor = back;
		listBoxScripts.BackColor = (isDark ? Color.FromArgb(45, 45, 48) : Color.White);
		listBoxScripts.ForeColor = (isDark ? Color.Gainsboro : Color.Black);
		richTextBoxScriptCode.BackColor = editorBack;
		richTextBoxScriptCode.ForeColor = (isDark ? Color.WhiteSmoke : Color.Black);
		richTextBoxOutput.BackColor = (isDark ? Color.FromArgb(20, 20, 20) : Color.White);
		richTextBoxOutput.ForeColor = (isDark ? Color.LightGreen : Color.Black);
	}

	private void LoadScripts()
	{
		try
		{
			listBoxScripts.Items.Clear();
			string scriptsDir = Path.Combine(Application.StartupPath, "NetExecutorScripts");
			if (!Directory.Exists(scriptsDir))
			{
				Directory.CreateDirectory(scriptsDir);
				File.WriteAllText(Path.Combine(scriptsDir, "example.ps1"), "Write-Host 'Hello from NetExecutor!'");
			}
			string[] files = Directory.GetFiles(scriptsDir, "*.*");
			foreach (string file in files)
			{
				listBoxScripts.Items.Add(Path.GetFileName(file));
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error loading scripts: " + ex.Message);
		}
	}

	private void listBoxScripts_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (listBoxScripts.SelectedItem == null)
		{
			return;
		}
		try
		{
			string scriptName = listBoxScripts.SelectedItem.ToString();
			string path = Path.Combine(Path.Combine(Application.StartupPath, "NetExecutorScripts"), scriptName);
			string code = File.ReadAllText(path);
			richTextBoxScriptCode.Text = code;
			textBoxScriptName.Text = Path.GetFileNameWithoutExtension(scriptName);
			switch (Path.GetExtension(path).ToLower())
			{
			case ".ps1":
				comboBoxScriptLanguage.Text = "PowerShell";
				break;
			case ".py":
				comboBoxScriptLanguage.Text = "Python";
				break;
			case ".cs":
				comboBoxScriptLanguage.Text = "C#";
				break;
			}
		}
		catch
		{
		}
	}

	private void comboBoxScriptLanguage_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (comboBoxScriptLanguage.Text == "C#" && (string.IsNullOrEmpty(richTextBoxScriptCode.Text) || !richTextBoxScriptCode.Text.Contains("namespace Plugin")))
		{
			richTextBoxScriptCode.Text = "using System;\r\nusing System.Windows.Forms;\r\nusing System.Drawing;\r\nusing System.IO;\r\nusing System.Diagnostics;\r\n\r\nnamespace Plugin {\r\n    public class Program {\r\n        public static void Main() {\r\n            // код сюда\r\n        }\r\n    }\r\n}";
		}
	}

	private void buttonSaveScript_Click(object sender, EventArgs e)
	{
		try
		{
			string scriptName = textBoxScriptName.Text.Trim();
			string language = comboBoxScriptLanguage.Text;
			string code = richTextBoxScriptCode.Text;
			if (string.IsNullOrEmpty(scriptName))
			{
				MessageBox.Show("Please enter a script name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			string extension = "";
			switch (language)
			{
			case "PowerShell":
				extension = ".ps1";
				break;
			case "Python":
				extension = ".py";
				break;
			case "C#":
				extension = ".cs";
				break;
			}
			string scriptsDir = Path.Combine(Application.StartupPath, "NetExecutorScripts");
			if (!Directory.Exists(scriptsDir))
			{
				Directory.CreateDirectory(scriptsDir);
			}
			File.WriteAllText(Path.Combine(scriptsDir, scriptName + extension), code);
			LoadScripts();
			MessageBox.Show("Script saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error saving script: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void buttonRefresh_Click(object sender, EventArgs e)
	{
		LoadScripts();
	}

	private void buttonStartSelected_Click(object sender, EventArgs e)
	{
		string language = comboBoxScriptLanguage.Text;
		string code = richTextBoxScriptCode.Text;
		if (string.IsNullOrEmpty(language) || string.IsNullOrEmpty(code))
		{
			MessageBox.Show("Please select a language and enter the script code.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		try
		{
			byte[] scriptPack = LEB128.Write(new object[3] { "RunNetExecutor", language, code });
			if (client == parrent)
			{
				string pluginPath = "Plugin\\NetExec.dll";
				if (!File.Exists(pluginPath))
				{
					MessageBox.Show("Plugin file not found: " + pluginPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				string checksum = Methods.GetChecksum(pluginPath);
				client.Send(new object[3] { "Invoke", checksum, scriptPack });
				richTextBoxOutput.AppendText("[" + DateTime.Now.ToLongTimeString() + "] Sending plugin and execution request...\n");
			}
			else
			{
				client.Send(scriptPack);
				richTextBoxOutput.AppendText("[" + DateTime.Now.ToLongTimeString() + "] Execution request sent...\n");
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error: " + ex.Message);
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
		this.listBoxScripts = new System.Windows.Forms.ListBox();
		this.buttonStartSelected = new MaterialSkin.Controls.MaterialButton();
		this.buttonRefresh = new MaterialSkin.Controls.MaterialButton();
		this.richTextBoxOutput = new System.Windows.Forms.RichTextBox();
		this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
		this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
		this.textBoxScriptName = new MaterialSkin.Controls.MaterialTextBox();
		this.comboBoxScriptLanguage = new MaterialSkin.Controls.MaterialComboBox();
		this.richTextBoxScriptCode = new System.Windows.Forms.RichTextBox();
		this.buttonSaveScript = new MaterialSkin.Controls.MaterialButton();
		this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
		this.materialLabel4 = new MaterialSkin.Controls.MaterialLabel();
		this.materialLabel5 = new MaterialSkin.Controls.MaterialLabel();
		base.SuspendLayout();
		this.listBoxScripts.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
		this.listBoxScripts.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.listBoxScripts.Font = new System.Drawing.Font("Consolas", 10f);
		this.listBoxScripts.ForeColor = System.Drawing.Color.Gainsboro;
		this.listBoxScripts.FormattingEnabled = true;
		this.listBoxScripts.ItemHeight = 15;
		this.listBoxScripts.Location = new System.Drawing.Point(12, 100);
		this.listBoxScripts.Name = "listBoxScripts";
		this.listBoxScripts.Size = new System.Drawing.Size(250, 400);
		this.listBoxScripts.TabIndex = 0;
		this.listBoxScripts.SelectedIndexChanged += new System.EventHandler(listBoxScripts_SelectedIndexChanged);
		this.buttonStartSelected.AutoSize = false;
		this.buttonStartSelected.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.buttonStartSelected.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.buttonStartSelected.Depth = 0;
		this.buttonStartSelected.HighEmphasis = true;
		this.buttonStartSelected.Icon = null;
		this.buttonStartSelected.Location = new System.Drawing.Point(12, 510);
		this.buttonStartSelected.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.buttonStartSelected.MouseState = MaterialSkin.MouseState.HOVER;
		this.buttonStartSelected.Name = "buttonStartSelected";
		this.buttonStartSelected.NoAccentTextColor = System.Drawing.Color.Empty;
		this.buttonStartSelected.Size = new System.Drawing.Size(120, 36);
		this.buttonStartSelected.TabIndex = 1;
		this.buttonStartSelected.Text = "RUN SELECTED";
		this.buttonStartSelected.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.buttonStartSelected.UseAccentColor = false;
		this.buttonStartSelected.UseVisualStyleBackColor = true;
		this.buttonStartSelected.Click += new System.EventHandler(buttonStartSelected_Click);
		this.buttonRefresh.AutoSize = false;
		this.buttonRefresh.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.buttonRefresh.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.buttonRefresh.Depth = 0;
		this.buttonRefresh.HighEmphasis = true;
		this.buttonRefresh.Icon = null;
		this.buttonRefresh.Location = new System.Drawing.Point(142, 510);
		this.buttonRefresh.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.buttonRefresh.MouseState = MaterialSkin.MouseState.HOVER;
		this.buttonRefresh.Name = "buttonRefresh";
		this.buttonRefresh.NoAccentTextColor = System.Drawing.Color.Empty;
		this.buttonRefresh.Size = new System.Drawing.Size(120, 36);
		this.buttonRefresh.TabIndex = 2;
		this.buttonRefresh.Text = "REFRESH";
		this.buttonRefresh.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.buttonRefresh.UseAccentColor = false;
		this.buttonRefresh.UseVisualStyleBackColor = true;
		this.buttonRefresh.Click += new System.EventHandler(buttonRefresh_Click);
		this.richTextBoxOutput.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
		this.richTextBoxOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.richTextBoxOutput.Font = new System.Drawing.Font("Consolas", 9f);
		this.richTextBoxOutput.ForeColor = System.Drawing.Color.LightGreen;
		this.richTextBoxOutput.Location = new System.Drawing.Point(280, 420);
		this.richTextBoxOutput.Name = "richTextBoxOutput";
		this.richTextBoxOutput.ReadOnly = true;
		this.richTextBoxOutput.Size = new System.Drawing.Size(700, 126);
		this.richTextBoxOutput.TabIndex = 3;
		this.richTextBoxOutput.Text = "";
		this.materialLabel1.AutoSize = true;
		this.materialLabel1.Depth = 0;
		this.materialLabel1.Font = new System.Drawing.Font("Roboto", 11f);
		this.materialLabel1.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.materialLabel1.Location = new System.Drawing.Point(12, 75);
		this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel1.Name = "materialLabel1";
		this.materialLabel1.Size = new System.Drawing.Size(139, 19);
		this.materialLabel1.TabIndex = 4;
		this.materialLabel1.Text = "Saved NetExecutors:";
		this.materialLabel2.AutoSize = true;
		this.materialLabel2.Depth = 0;
		this.materialLabel2.Font = new System.Drawing.Font("Roboto", 11f);
		this.materialLabel2.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.materialLabel2.Location = new System.Drawing.Point(280, 395);
		this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel2.Name = "materialLabel2";
		this.materialLabel2.Size = new System.Drawing.Size(104, 19);
		this.materialLabel2.TabIndex = 5;
		this.materialLabel2.Text = "Execution Log:";
		this.textBoxScriptName.AnimateReadOnly = false;
		this.textBoxScriptName.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.textBoxScriptName.Depth = 0;
		this.textBoxScriptName.Font = new System.Drawing.Font("Roboto", 16f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.textBoxScriptName.LeadingIcon = null;
		this.textBoxScriptName.Location = new System.Drawing.Point(280, 100);
		this.textBoxScriptName.MaxLength = 50;
		this.textBoxScriptName.MouseState = MaterialSkin.MouseState.OUT;
		this.textBoxScriptName.Multiline = false;
		this.textBoxScriptName.Name = "textBoxScriptName";
		this.textBoxScriptName.Size = new System.Drawing.Size(250, 50);
		this.textBoxScriptName.TabIndex = 6;
		this.textBoxScriptName.Text = "";
		this.textBoxScriptName.TrailingIcon = null;
		this.comboBoxScriptLanguage.AutoResize = false;
		this.comboBoxScriptLanguage.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.comboBoxScriptLanguage.Depth = 0;
		this.comboBoxScriptLanguage.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
		this.comboBoxScriptLanguage.DropDownHeight = 174;
		this.comboBoxScriptLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxScriptLanguage.DropDownWidth = 121;
		this.comboBoxScriptLanguage.Font = new System.Drawing.Font("Microsoft Sans Serif", 14f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.comboBoxScriptLanguage.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.comboBoxScriptLanguage.FormattingEnabled = true;
		this.comboBoxScriptLanguage.IntegralHeight = false;
		this.comboBoxScriptLanguage.ItemHeight = 43;
		this.comboBoxScriptLanguage.Items.AddRange(new object[3] { "PowerShell", "Python", "C#" });
		this.comboBoxScriptLanguage.Location = new System.Drawing.Point(550, 100);
		this.comboBoxScriptLanguage.MaxDropDownItems = 4;
		this.comboBoxScriptLanguage.MouseState = MaterialSkin.MouseState.OUT;
		this.comboBoxScriptLanguage.Name = "comboBoxScriptLanguage";
		this.comboBoxScriptLanguage.Size = new System.Drawing.Size(150, 49);
		this.comboBoxScriptLanguage.StartIndex = 0;
		this.comboBoxScriptLanguage.TabIndex = 7;
		this.comboBoxScriptLanguage.SelectedIndexChanged += new System.EventHandler(comboBoxScriptLanguage_SelectedIndexChanged);
		this.richTextBoxScriptCode.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
		this.richTextBoxScriptCode.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.richTextBoxScriptCode.Font = new System.Drawing.Font("Consolas", 10f);
		this.richTextBoxScriptCode.ForeColor = System.Drawing.Color.Gainsboro;
		this.richTextBoxScriptCode.Location = new System.Drawing.Point(280, 180);
		this.richTextBoxScriptCode.Name = "richTextBoxScriptCode";
		this.richTextBoxScriptCode.Size = new System.Drawing.Size(700, 200);
		this.richTextBoxScriptCode.TabIndex = 8;
		this.richTextBoxScriptCode.Text = "";
		this.buttonSaveScript.AutoSize = false;
		this.buttonSaveScript.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.buttonSaveScript.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.buttonSaveScript.Depth = 0;
		this.buttonSaveScript.HighEmphasis = true;
		this.buttonSaveScript.Icon = null;
		this.buttonSaveScript.Location = new System.Drawing.Point(710, 100);
		this.buttonSaveScript.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.buttonSaveScript.MouseState = MaterialSkin.MouseState.HOVER;
		this.buttonSaveScript.Name = "buttonSaveScript";
		this.buttonSaveScript.NoAccentTextColor = System.Drawing.Color.Empty;
		this.buttonSaveScript.Size = new System.Drawing.Size(120, 50);
		this.buttonSaveScript.TabIndex = 9;
		this.buttonSaveScript.Text = "SAVE";
		this.buttonSaveScript.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.buttonSaveScript.UseAccentColor = false;
		this.buttonSaveScript.UseVisualStyleBackColor = true;
		this.buttonSaveScript.Click += new System.EventHandler(buttonSaveScript_Click);
		this.materialLabel3.AutoSize = true;
		this.materialLabel3.Depth = 0;
		this.materialLabel3.Font = new System.Drawing.Font("Roboto", 11f);
		this.materialLabel3.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.materialLabel3.Location = new System.Drawing.Point(280, 75);
		this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel3.Name = "materialLabel3";
		this.materialLabel3.Size = new System.Drawing.Size(53, 19);
		this.materialLabel3.TabIndex = 10;
		this.materialLabel3.Text = "Name:";
		this.materialLabel4.AutoSize = true;
		this.materialLabel4.Depth = 0;
		this.materialLabel4.Font = new System.Drawing.Font("Roboto", 11f);
		this.materialLabel4.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.materialLabel4.Location = new System.Drawing.Point(550, 75);
		this.materialLabel4.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel4.Name = "materialLabel4";
		this.materialLabel4.Size = new System.Drawing.Size(78, 19);
		this.materialLabel4.TabIndex = 11;
		this.materialLabel4.Text = "Language:";
		this.materialLabel5.AutoSize = true;
		this.materialLabel5.Depth = 0;
		this.materialLabel5.Font = new System.Drawing.Font("Roboto", 11f);
		this.materialLabel5.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.materialLabel5.Location = new System.Drawing.Point(280, 155);
		this.materialLabel5.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel5.Name = "materialLabel5";
		this.materialLabel5.Size = new System.Drawing.Size(48, 19);
		this.materialLabel5.TabIndex = 12;
		this.materialLabel5.Text = "Code:";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(1000, 560);
		base.Controls.Add(this.materialLabel5);
		base.Controls.Add(this.materialLabel4);
		base.Controls.Add(this.materialLabel3);
		base.Controls.Add(this.buttonSaveScript);
		base.Controls.Add(this.richTextBoxScriptCode);
		base.Controls.Add(this.comboBoxScriptLanguage);
		base.Controls.Add(this.textBoxScriptName);
		base.Controls.Add(this.materialLabel2);
		base.Controls.Add(this.materialLabel1);
		base.Controls.Add(this.richTextBoxOutput);
		base.Controls.Add(this.buttonRefresh);
		base.Controls.Add(this.buttonStartSelected);
		base.Controls.Add(this.listBoxScripts);
		base.Name = "FormNetExecutor";
		this.Text = "NetExecutor";
		base.Load += new System.EventHandler(FormNetExecutor_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
