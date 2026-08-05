using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Helper;

namespace Server.Forms;

public class FormAuthorization : FormMaterial
{
	private const string PASTEBIN_URL = "https://pastebin.com/raw/g5eGRNGJ";

	private const string ADMIN_ID = "5569123078";

	private const string API_SERVER_URL = "";

	private const int HTTP_TIMEOUT = 10000;

	private const int MAX_RETRIES = 2;

	private static readonly string LOCAL_FOLDER = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Local");

	private static readonly string DB_PATH = Path.Combine(LOCAL_FOLDER, "licenses.db");

	private static readonly string CONFIG_PATH = Path.Combine(LOCAL_FOLDER, "license_config.dat");

	private static readonly string AUTH_LOG_PATH = Path.Combine(LOCAL_FOLDER, "auth.log");

	private string _cachedToken;

	private IContainer components;

	private RJTextBox rjTextBox7;

	private RJTextBox rjTextBox1;

	private MaterialButton materialButton1;

	private MaterialLabel materialLabel2;

	private MaterialCheckbox materialCheckbox1;

	private MaterialLabel materialLabel3;

	private PictureBox pictureBox1;

	private MaterialLabel materialLabel1;

	private Label labelError;

	public FormAuthorization()
	{
		InitializeComponent();
		try
		{
			if (!Directory.Exists(LOCAL_FOLDER))
			{
				Directory.CreateDirectory(LOCAL_FOLDER);
			}
		}
		catch
		{
		}
		LoadSavedCredentials();
	}

	private void LoadSavedCredentials()
	{
		try
		{
			if (!File.Exists(CONFIG_PATH))
			{
				return;
			}
			string encrypted = File.ReadAllText(CONFIG_PATH);
			string decrypted = DecryptCredentials(encrypted);
			if (!string.IsNullOrEmpty(decrypted))
			{
				string[] parts = decrypted.Split('\n');
				if (parts.Length >= 1 && !string.IsNullOrEmpty(parts[0]))
				{
					rjTextBox7.Texts = parts[0].Trim();
					materialCheckbox1.Checked = true;
				}
				if (parts.Length >= 2 && !string.IsNullOrEmpty(parts[1]))
				{
					rjTextBox1.Texts = parts[1].Trim();
				}
			}
		}
		catch
		{
		}
	}

	private void SaveCredentials(string username, string key)
	{
		try
		{
			if (materialCheckbox1.Checked)
			{
				string data = username + "\n" + key;
				string encrypted = EncryptCredentials(data);
				File.WriteAllText(CONFIG_PATH, encrypted);
			}
			else if (File.Exists(CONFIG_PATH))
			{
				File.Delete(CONFIG_PATH);
			}
		}
		catch
		{
		}
	}

	private string EncryptCredentials(string plainText)
	{
		try
		{
			byte[] key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(GetHWID() + "LiberiumSalt2024"));
			byte[] iv = new byte[16];
			Array.Copy(key, iv, 16);
			using Aes aes = Aes.Create();
			aes.Key = key;
			aes.IV = iv;
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;
			using ICryptoTransform encryptor = aes.CreateEncryptor();
			byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
			return Convert.ToBase64String(encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length));
		}
		catch
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
		}
	}

	private string DecryptCredentials(string cipherText)
	{
		try
		{
			byte[] key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(GetHWID() + "LiberiumSalt2024"));
			byte[] iv = new byte[16];
			Array.Copy(key, iv, 16);
			using Aes aes = Aes.Create();
			aes.Key = key;
			aes.IV = iv;
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;
			using ICryptoTransform decryptor = aes.CreateDecryptor();
			byte[] cipherBytes = Convert.FromBase64String(cipherText);
			byte[] decrypted = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
			return Encoding.UTF8.GetString(decrypted);
		}
		catch
		{
			return null;
		}
	}

	private void materialButton1_Click(object sender, EventArgs e)
	{
		labelError.Text = "";
		labelError.ForeColor = Color.Red;
		string username = rjTextBox7.Texts.Trim().TrimStart('@').ToLower();
		string key = rjTextBox1.Texts.Trim().ToUpper();
		if (string.IsNullOrEmpty(username))
		{
			ShowError("❌ Enter username");
			rjTextBox7.Focus();
			return;
		}
		if (username.Length < 3)
		{
			ShowError("❌ Username too short (minimum 3 characters)");
			rjTextBox7.Focus();
			return;
		}
		if (username.Length > 32)
		{
			ShowError("❌ Username too long (maximum 32 characters)");
			rjTextBox7.Focus();
			return;
		}
		if (string.IsNullOrEmpty(key))
		{
			ShowError("❌ Enter license key");
			rjTextBox1.Focus();
			return;
		}
		if (key.Length != 19 || key.Count((char c) => c == '-') != 3)
		{
			ShowError("❌ Incorrect key format (XXXX-XXXX-XXXX-XXXX)");
			rjTextBox1.Focus();
			return;
		}
		if (!key.Replace("-", "").All((char c) => "0123456789ABCDEF".Contains(c)))
		{
			ShowError("❌ Key contains invalid characters (only 0-9, A-F)");
			rjTextBox1.Focus();
			return;
		}
		SetUIEnabled(enabled: false);
		materialButton1.Text = "Checking...";
		ShowStatus("⏳ Connecting to license server...");
		try
		{
			string botToken = GetBotToken();
			if (string.IsNullOrEmpty(botToken))
			{
				ShowError("❌ Cannot reach license server. Check internet.");
				WriteAuthLog("FAILED: " + username + " - Cannot get bot token");
				ResetUI();
				return;
			}
			ShowStatus("⏳ Checking license server status...");
			if (!CheckBotOnline(botToken))
			{
				ShowError("❌ License server offline. Contact admin.");
				WriteAuthLog("FAILED: " + username + " - Bot offline");
				ResetUI();
				return;
			}
			ShowStatus("⏳ Verifying license...");
			string hwid = GetHWID();
			LicenseVerifyResult result;
			if (!string.IsNullOrEmpty(""))
			{
				result = VerifyLicenseViaAPI(username, key, hwid);
			}
			else
			{
				if (!File.Exists(DB_PATH))
				{
					ShowError("❌ License database not found. Contact admin.");
					WriteAuthLog("FAILED: " + username + " - Database not found at " + DB_PATH);
					ResetUI();
					return;
				}
				result = VerifyLicenseLocal(username, key, hwid);
			}
			if (result.Success)
			{
				ShowStatus($"✅ License valid ({result.DaysLeft} days left)");
				SaveCredentials(username, key);
				UpdateLastLogin(key);
				BindHWID(key, hwid);
				try
				{
					SendLoginNotification(botToken, username, key, hwid, result.DaysLeft);
				}
				catch
				{
				}
				WriteAuthLog($"SUCCESS: {username} | Key: {key} | HWID: {hwid} | Days: {result.DaysLeft}");
				if (result.DaysLeft <= 3 && result.DaysLeft > 0)
				{
					MessageBox.Show($"⚠\ufe0f Your license expires in {result.DaysLeft} day(s)!\n\n" + $"Expiration: {result.ExpiresAt:yyyy-MM-dd}\n\n" + "Contact administrator to extend.", "License Expiring Soon", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				base.DialogResult = DialogResult.OK;
				Close();
			}
			else
			{
				ShowError("❌ " + result.ErrorMessage);
				WriteAuthLog("FAILED: " + username + " - " + result.ErrorMessage);
				rjTextBox1.Texts = "";
				rjTextBox1.Focus();
			}
		}
		catch (Exception ex)
		{
			ShowError("❌ Unexpected error: " + ex.Message);
			WriteAuthLog("ERROR: " + username + " - " + ex.Message);
		}
		finally
		{
			ResetUI();
		}
	}

	private LicenseVerifyResult VerifyLicenseLocal(string username, string key, string hwid)
	{
		try
		{
			using SQLiteConnection connection = new SQLiteConnection("Data Source=" + DB_PATH + ";Version=3;Read Only=True;");
			connection.Open();
			using SQLiteCommand command = new SQLiteCommand("\r\n                        SELECT telegram_username, expires_at, is_active, hwid\r\n                        FROM licenses\r\n                        WHERE license_key = @key\r\n                        LIMIT 1\r\n                    ", connection);
			command.Parameters.AddWithValue("@key", key);
			using SQLiteDataReader reader = command.ExecuteReader();
			if (!reader.Read())
			{
				return new LicenseVerifyResult
				{
					Success = false,
					ErrorMessage = "License not found in database"
				};
			}
			string obj = reader["telegram_username"].ToString().ToLower();
			string expiresStr = reader["expires_at"].ToString();
			bool isActive = Convert.ToInt32(reader["is_active"]) == 1;
			string dbHwid = reader["hwid"]?.ToString();
			if (obj != username)
			{
				return new LicenseVerifyResult
				{
					Success = false,
					ErrorMessage = "Username mismatch"
				};
			}
			if (!isActive)
			{
				return new LicenseVerifyResult
				{
					Success = false,
					ErrorMessage = "License has been revoked"
				};
			}
			if (!DateTime.TryParse(expiresStr, out var expiresAt))
			{
				return new LicenseVerifyResult
				{
					Success = false,
					ErrorMessage = "Invalid license data"
				};
			}
			if (DateTime.Now > expiresAt)
			{
				return new LicenseVerifyResult
				{
					Success = false,
					ErrorMessage = $"License expired on {expiresAt:yyyy-MM-dd}"
				};
			}
			if (!string.IsNullOrEmpty(dbHwid) && dbHwid != "UNKNOWN" && dbHwid != hwid && hwid != "UNKNOWN")
			{
				return new LicenseVerifyResult
				{
					Success = false,
					ErrorMessage = "Hardware ID mismatch. Contact admin to reset."
				};
			}
			int daysLeft = (expiresAt - DateTime.Now).Days;
			return new LicenseVerifyResult
			{
				Success = true,
				DaysLeft = daysLeft,
				ExpiresAt = expiresAt
			};
		}
		catch (Exception ex)
		{
			return new LicenseVerifyResult
			{
				Success = false,
				ErrorMessage = "Database error: " + ex.Message
			};
		}
	}

	private LicenseVerifyResult VerifyLicenseViaAPI(string username, string key, string hwid)
	{
		try
		{
			string url = "";
			string jsonPayload = "{\"username\":\"" + username + "\",\"key\":\"" + key + "\",\"hwid\":\"" + hwid + "\"}";
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "POST";
			request.ContentType = "application/json";
			request.Timeout = 10000;
			request.UserAgent = "LiberiumPanel/2.0";
			byte[] data = Encoding.UTF8.GetBytes(jsonPayload);
			request.ContentLength = data.Length;
			using (Stream stream = request.GetRequestStream())
			{
				stream.Write(data, 0, data.Length);
			}
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				using Stream responseStream = response.GetResponseStream();
				using StreamReader reader = new StreamReader(responseStream);
				string body = reader.ReadToEnd();
				if (body.Contains("\"success\":true") || body.Contains("\"success\": true"))
				{
					int daysLeft = ExtractIntFromJson(body, "days_left");
					string expiresAt = ExtractStringFromJson(body, "expires_at");
					DateTime expires = DateTime.Now.AddDays(daysLeft);
					if (!string.IsNullOrEmpty(expiresAt))
					{
						DateTime.TryParse(expiresAt, out expires);
					}
					return new LicenseVerifyResult
					{
						Success = true,
						DaysLeft = daysLeft,
						ExpiresAt = expires
					};
				}
			}
			return new LicenseVerifyResult
			{
				Success = false,
				ErrorMessage = "Server returned invalid response"
			};
		}
		catch (WebException ex)
		{
			try
			{
				if (ex.Response != null)
				{
					using Stream stream2 = ex.Response.GetResponseStream();
					using StreamReader reader2 = new StreamReader(stream2);
					string errorBody = reader2.ReadToEnd();
					string detail = ExtractStringFromJson(errorBody, "detail");
					if (!string.IsNullOrEmpty(detail))
					{
						return new LicenseVerifyResult
						{
							Success = false,
							ErrorMessage = detail
						};
					}
				}
			}
			catch
			{
			}
			return new LicenseVerifyResult
			{
				Success = false,
				ErrorMessage = "Cannot connect to license server"
			};
		}
		catch (Exception ex2)
		{
			return new LicenseVerifyResult
			{
				Success = false,
				ErrorMessage = "API error: " + ex2.Message
			};
		}
	}

	private int ExtractIntFromJson(string json, string key)
	{
		try
		{
			string pattern = "\"" + key + "\"\\s*:\\s*(\\d+)";
			Match match = Regex.Match(json, pattern);
			if (match.Success)
			{
				return int.Parse(match.Groups[1].Value);
			}
		}
		catch
		{
		}
		return 0;
	}

	private string ExtractStringFromJson(string json, string key)
	{
		try
		{
			string pattern = "\"" + key + "\"\\s*:\\s*\"([^\"]+)\"";
			Match match = Regex.Match(json, pattern);
			if (match.Success)
			{
				return match.Groups[1].Value;
			}
		}
		catch
		{
		}
		return null;
	}

	private string GetBotToken()
	{
		if (!string.IsNullOrEmpty(_cachedToken))
		{
			return _cachedToken;
		}
		for (int i = 0; i < 2; i++)
		{
			try
			{
				HttpWebRequest obj = (HttpWebRequest)WebRequest.Create("https://pastebin.com/raw/g5eGRNGJ");
				obj.Timeout = 10000;
				obj.UserAgent = "Mozilla/5.0";
				using HttpWebResponse response = (HttpWebResponse)obj.GetResponse();
				using Stream stream = response.GetResponseStream();
				using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
				string token = reader.ReadToEnd().Trim();
				if (!string.IsNullOrEmpty(token) && token.Contains(":") && token.Length > 20)
				{
					_cachedToken = token;
					return token;
				}
			}
			catch
			{
				if (i < 1)
				{
					Thread.Sleep(1000);
				}
			}
		}
		return null;
	}

	private bool CheckBotOnline(string botToken)
	{
		try
		{
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create("https://api.telegram.org/bot" + botToken + "/getMe");
			obj.Timeout = 10000;
			obj.Method = "GET";
			using HttpWebResponse response = (HttpWebResponse)obj.GetResponse();
			using Stream stream = response.GetResponseStream();
			using StreamReader reader = new StreamReader(stream);
			return reader.ReadToEnd().Contains("\"ok\":true");
		}
		catch
		{
			return false;
		}
	}

	private void SendLoginNotification(string botToken, string username, string key, string hwid, int daysLeft)
	{
		try
		{
			string pcName = Environment.MachineName;
			string osVersion = Environment.OSVersion.VersionString;
			string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			string ipAddress = GetExternalIP();
			string message = "\ud83d\udd10 *Authorization Successful*\n\n\ud83d\udc64 Username: @" + username + "\n\ud83d\udd11 Key: `" + key + "`\n" + $"⏰ Days left: {daysLeft}\n" + "\ud83d\udd50 Time: " + currentTime + "\n\ud83d\udcbb PC: " + pcName + "\n\ud83d\udda5 OS: " + osVersion + "\n\ud83c\udf10 IP: " + ipAddress + "\n\ud83d\udd12 HWID: " + hwid;
			string requestUriString = "https://api.telegram.org/bot" + botToken + "/sendMessage";
			string postData = "chat_id=5569123078&text=" + Uri.EscapeDataString(message) + "&parse_mode=Markdown";
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUriString);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.Timeout = 5000;
			byte[] data = Encoding.UTF8.GetBytes(postData);
			request.ContentLength = data.Length;
			using (Stream stream = request.GetRequestStream())
			{
				stream.Write(data, 0, data.Length);
			}
			request.GetResponse().Close();
		}
		catch
		{
		}
	}

	private string GetExternalIP()
	{
		try
		{
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create("https://api.ipify.org");
			obj.Timeout = 3000;
			using HttpWebResponse response = (HttpWebResponse)obj.GetResponse();
			using Stream stream = response.GetResponseStream();
			using StreamReader reader = new StreamReader(stream);
			return reader.ReadToEnd().Trim();
		}
		catch
		{
			return "Unknown";
		}
	}

	private void BindHWID(string key, string hwid)
	{
		if (string.IsNullOrEmpty(hwid) || hwid == "UNKNOWN")
		{
			return;
		}
		try
		{
			using SQLiteConnection connection = new SQLiteConnection("Data Source=" + DB_PATH + ";Version=3;");
			connection.Open();
			using SQLiteCommand command = new SQLiteCommand("UPDATE licenses SET hwid = @hwid WHERE license_key = @key AND (hwid IS NULL OR hwid = '' OR hwid = 'UNKNOWN')", connection);
			command.Parameters.AddWithValue("@hwid", hwid);
			command.Parameters.AddWithValue("@key", key);
			command.ExecuteNonQuery();
		}
		catch
		{
		}
	}

	private void UpdateLastLogin(string key)
	{
		try
		{
			using SQLiteConnection connection = new SQLiteConnection("Data Source=" + DB_PATH + ";Version=3;");
			connection.Open();
			using SQLiteCommand command = new SQLiteCommand("UPDATE licenses SET last_login = @now WHERE license_key = @key", connection);
			command.Parameters.AddWithValue("@now", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			command.Parameters.AddWithValue("@key", key);
			command.ExecuteNonQuery();
		}
		catch
		{
		}
	}

	private string GetHWID()
	{
		try
		{
			string cpuId = GetWMIValue("Win32_Processor", "ProcessorId");
			string boardSerial = GetWMIValue("Win32_BaseBoard", "SerialNumber");
			string diskSerial = GetWMIValue("Win32_DiskDrive", "SerialNumber");
			string combined = cpuId + "|" + boardSerial + "|" + diskSerial;
			using SHA256 sha256 = SHA256.Create();
			return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(combined))).Replace("-", "").Substring(0, 16);
		}
		catch
		{
			return "UNKNOWN";
		}
	}

	private string GetWMIValue(string wmiClass, string property)
	{
		try
		{
			using ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT " + property + " FROM " + wmiClass);
			foreach (ManagementBaseObject item in searcher.Get())
			{
				object value = item[property];
				if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
				{
					return value.ToString().Trim();
				}
			}
		}
		catch
		{
		}
		return "UNKNOWN";
	}

	private void ShowError(string message)
	{
		labelError.Text = message;
		labelError.ForeColor = Color.Red;
	}

	private void ShowStatus(string message)
	{
		labelError.Text = message;
		labelError.ForeColor = Color.Orange;
		Application.DoEvents();
	}

	private void SetUIEnabled(bool enabled)
	{
		rjTextBox7.Enabled = enabled;
		rjTextBox1.Enabled = enabled;
		materialButton1.Enabled = enabled;
		materialCheckbox1.Enabled = enabled;
		Cursor = (enabled ? Cursors.Default : Cursors.WaitCursor);
	}

	private void ResetUI()
	{
		SetUIEnabled(enabled: true);
		materialButton1.Text = "LOGIN";
	}

	private void WriteAuthLog(string message)
	{
		try
		{
			string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			File.AppendAllText(AUTH_LOG_PATH, "[" + timestamp + "] " + message + "\r\n");
		}
		catch
		{
		}
	}

	private void FormAuthorization_Load(object sender, EventArgs e)
	{
		rjTextBox7.Focus();
		WriteAuthLog("=== Application started ===");
		if (!File.Exists(DB_PATH))
		{
			ShowStatus("⚠\ufe0f Database not found. Copy licenses.db to Local folder.");
		}
	}

	private void FormAuthorization_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (base.DialogResult != DialogResult.OK)
		{
			base.DialogResult = DialogResult.Cancel;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormAuthorization));
		this.rjTextBox7 = new CustomControls.RJControls.RJTextBox();
		this.rjTextBox1 = new CustomControls.RJControls.RJTextBox();
		this.materialButton1 = new MaterialSkin.Controls.MaterialButton();
		this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
		this.materialCheckbox1 = new MaterialSkin.Controls.MaterialCheckbox();
		this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
		this.labelError = new System.Windows.Forms.Label();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		base.SuspendLayout();
		this.rjTextBox7.BackColor = System.Drawing.Color.White;
		this.rjTextBox7.BorderColor = System.Drawing.Color.FromArgb(245, 245, 245);
		this.rjTextBox7.BorderFocusColor = System.Drawing.Color.FromArgb(245, 245, 245);
		this.rjTextBox7.BorderRadius = 0;
		this.rjTextBox7.BorderSize = 1;
		this.rjTextBox7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox7.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox7.Location = new System.Drawing.Point(7, 89);
		this.rjTextBox7.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox7.Multiline = false;
		this.rjTextBox7.Name = "rjTextBox7";
		this.rjTextBox7.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox7.PasswordChar = false;
		this.rjTextBox7.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox7.PlaceholderText = "username";
		this.rjTextBox7.Size = new System.Drawing.Size(328, 31);
		this.rjTextBox7.TabIndex = 47;
		this.rjTextBox7.Texts = "";
		this.rjTextBox7.UnderlinedStyle = false;
		this.rjTextBox1.BackColor = System.Drawing.Color.White;
		this.rjTextBox1.BorderColor = System.Drawing.Color.FromArgb(245, 245, 245);
		this.rjTextBox1.BorderFocusColor = System.Drawing.Color.FromArgb(245, 245, 245);
		this.rjTextBox1.BorderRadius = 0;
		this.rjTextBox1.BorderSize = 1;
		this.rjTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox1.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox1.Location = new System.Drawing.Point(7, 147);
		this.rjTextBox1.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox1.Multiline = false;
		this.rjTextBox1.Name = "rjTextBox1";
		this.rjTextBox1.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox1.PasswordChar = false;
		this.rjTextBox1.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox1.PlaceholderText = "authorization key";
		this.rjTextBox1.Size = new System.Drawing.Size(329, 31);
		this.rjTextBox1.TabIndex = 48;
		this.rjTextBox1.Texts = "";
		this.rjTextBox1.UnderlinedStyle = false;
		this.materialButton1.AutoSize = false;
		this.materialButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.materialButton1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.materialButton1.Depth = 0;
		this.materialButton1.HighEmphasis = true;
		this.materialButton1.Icon = null;
		this.materialButton1.Location = new System.Drawing.Point(7, 225);
		this.materialButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialButton1.Name = "materialButton1";
		this.materialButton1.NoAccentTextColor = System.Drawing.Color.Empty;
		this.materialButton1.Size = new System.Drawing.Size(328, 36);
		this.materialButton1.TabIndex = 49;
		this.materialButton1.Text = "LOGIN";
		this.materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.materialButton1.UseAccentColor = false;
		this.materialButton1.UseVisualStyleBackColor = true;
		this.materialButton1.Click += new System.EventHandler(materialButton1_Click);
		this.materialLabel2.AutoSize = true;
		this.materialLabel2.Depth = 0;
		this.materialLabel2.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel2.Location = new System.Drawing.Point(4, 66);
		this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel2.Name = "materialLabel2";
		this.materialLabel2.Size = new System.Drawing.Size(147, 19);
		this.materialLabel2.TabIndex = 50;
		this.materialLabel2.Text = "Telegram Username:";
		this.materialCheckbox1.AutoSize = true;
		this.materialCheckbox1.Depth = 0;
		this.materialCheckbox1.Location = new System.Drawing.Point(5, 182);
		this.materialCheckbox1.Margin = new System.Windows.Forms.Padding(0);
		this.materialCheckbox1.MouseLocation = new System.Drawing.Point(-1, -1);
		this.materialCheckbox1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialCheckbox1.Name = "materialCheckbox1";
		this.materialCheckbox1.ReadOnly = false;
		this.materialCheckbox1.Ripple = true;
		this.materialCheckbox1.Size = new System.Drawing.Size(137, 37);
		this.materialCheckbox1.TabIndex = 51;
		this.materialCheckbox1.Text = "Remember me";
		this.materialCheckbox1.UseVisualStyleBackColor = true;
		this.materialLabel3.AutoSize = true;
		this.materialLabel3.Depth = 0;
		this.materialLabel3.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel3.Location = new System.Drawing.Point(5, 124);
		this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel3.Name = "materialLabel3";
		this.materialLabel3.Size = new System.Drawing.Size(128, 19);
		this.materialLabel3.TabIndex = 52;
		this.materialLabel3.Text = "Authorization key:";
		this.pictureBox1.BackColor = System.Drawing.Color.White;
		this.pictureBox1.Image = (System.Drawing.Image)resources.GetObject("pictureBox1.Image");
		this.pictureBox1.Location = new System.Drawing.Point(343, 66);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(202, 195);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.pictureBox1.TabIndex = 39;
		this.pictureBox1.TabStop = false;
		this.materialLabel1.AutoSize = true;
		this.materialLabel1.Depth = 0;
		this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel1.Location = new System.Drawing.Point(17, 134);
		this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel1.Name = "materialLabel1";
		this.materialLabel1.Size = new System.Drawing.Size(1, 0);
		this.materialLabel1.TabIndex = 35;
		this.labelError.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelError.ForeColor = System.Drawing.Color.Red;
		this.labelError.Location = new System.Drawing.Point(153, 192);
		this.labelError.Name = "labelError";
		this.labelError.Size = new System.Drawing.Size(182, 24);
		this.labelError.TabIndex = 53;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(551, 267);
		base.Controls.Add(this.labelError);
		base.Controls.Add(this.materialLabel3);
		base.Controls.Add(this.materialCheckbox1);
		base.Controls.Add(this.materialLabel2);
		base.Controls.Add(this.materialButton1);
		base.Controls.Add(this.rjTextBox1);
		base.Controls.Add(this.rjTextBox7);
		base.Controls.Add(this.pictureBox1);
		base.Controls.Add(this.materialLabel1);
		base.Name = "FormAuthorization";
		this.Text = "Authorization";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(FormAuthorization_FormClosing);
		base.Load += new System.EventHandler(FormAuthorization_Load);
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
