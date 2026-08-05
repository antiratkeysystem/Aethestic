using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Leb128;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormWindowsCustomizer : FormMaterial
{
	public Clients client;

	public Clients parrent;

	public bool isUpdating;

	private IContainer components;

	private MaterialCheckbox cbGodMode;

	private MaterialCheckbox cbDeleteConfirmation;

	private MaterialCheckbox cbAutoRebootLoggedUsers;

	private MaterialCheckbox cbChangeShutdownUpdates;

	private MaterialCheckbox cbWinKeys;

	private MaterialCheckbox cbWebServiceDialog;

	private MaterialCheckbox cbAutoRestartCrash;

	private MaterialCheckbox cbErrorReporting;

	private MaterialCheckbox cbFilePrintSharing;

	private MaterialCheckbox cbKernelPaging;

	private MaterialCheckbox cbClearPageFileShutdown;

	private MaterialCheckbox cbBootDefragmentation;

	private MaterialCheckbox cbReserveBandwidth;

	private MaterialCheckbox cbVerboseMessages;

	private MaterialCheckbox cbSeparateExplorer;

	private MaterialCheckbox cbCrashCtrlScroll;

	private MaterialCheckbox cbMobilityCenter;

	private MaterialCheckbox cbDisplayWinVersion;

	private MaterialCheckbox cbDisplayTrayItems;

	private MaterialCheckbox cbWindowAnimations;

	private MaterialCheckbox cbAeroShake;

	private MaterialCheckbox cbWindowSnap;

	private MaterialCheckbox cbLockScreen;

	private MaterialCheckbox cbDarkTheme;

	private MaterialCheckbox cbBalloonNotification;

	private MaterialCheckbox cbActionCenter;

	private MaterialCheckbox cbClassicVolumeMixer;

	private MaterialCheckbox cbNotificationBalloons;

	private MaterialCheckbox cbShowLibraries;

	private MaterialCheckbox cbShowRecycleBinComputer;

	private MaterialCheckbox cbShowDesktopPreview;

	private MaterialCheckbox cbExplorerCheckboxSelection;

	private MaterialLabel lblSystem;

	private MaterialLabel lblAppearance;

	private MaterialLabel lblExplorer;

	public FormWindowsCustomizer()
	{
		InitializeComponent();
	}

	private void FormWindowsCustomizer_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		base.FormClosing += FormWindowsCustomizer_FormClosing;
		ChangeScheme(this);
		if (client != null)
		{
			client.Send(LEB128.Write(new object[1] { "GetSettings" }));
		}
	}

	private void FormWindowsCustomizer_FormClosing(object sender, FormClosingEventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged -= ChangeScheme;
	}

	private void ChangeScheme(object sender)
	{
		if (base.IsDisposed)
		{
			return;
		}
		try
		{
			Color backColor = ((MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK) ? Color.FromArgb(50, 50, 50) : Color.White);
			Color foreColor = ((MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK) ? Color.White : Color.Black);
			foreach (Control c in base.Controls)
			{
				if (c is MaterialCheckbox cb)
				{
					cb.BackColor = backColor;
					cb.ForeColor = foreColor;
				}
				else if (c is MaterialLabel lbl)
				{
					lbl.BackColor = backColor;
					lbl.ForeColor = foreColor;
				}
			}
			BackColor = backColor;
		}
		catch
		{
		}
	}

	private void CheckBox_CheckedChanged(object sender, EventArgs e)
	{
		if (!isUpdating && client != null && sender is MaterialCheckbox { Tag: not null } cb)
		{
			client.Send(LEB128.Write(new object[3]
			{
				"UpdateSetting",
				cb.Tag.ToString(),
				cb.Checked
			}));
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
		this.cbGodMode = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbDeleteConfirmation = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbAutoRebootLoggedUsers = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbChangeShutdownUpdates = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbWinKeys = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbWebServiceDialog = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbAutoRestartCrash = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbErrorReporting = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbFilePrintSharing = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbKernelPaging = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbClearPageFileShutdown = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbBootDefragmentation = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbReserveBandwidth = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbVerboseMessages = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbSeparateExplorer = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbCrashCtrlScroll = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbMobilityCenter = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbDisplayWinVersion = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbDisplayTrayItems = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbWindowAnimations = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbAeroShake = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbWindowSnap = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbLockScreen = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbDarkTheme = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbBalloonNotification = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbActionCenter = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbClassicVolumeMixer = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbNotificationBalloons = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbShowLibraries = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbShowRecycleBinComputer = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbShowDesktopPreview = new MaterialSkin.Controls.MaterialCheckbox();
		this.cbExplorerCheckboxSelection = new MaterialSkin.Controls.MaterialCheckbox();
		this.lblSystem = new MaterialSkin.Controls.MaterialLabel();
		this.lblAppearance = new MaterialSkin.Controls.MaterialLabel();
		this.lblExplorer = new MaterialSkin.Controls.MaterialLabel();
		base.SuspendLayout();
		this.cbGodMode.AutoSize = true;
		this.cbGodMode.Depth = 0;
		this.cbGodMode.Location = new System.Drawing.Point(13, 95);
		this.cbGodMode.Margin = new System.Windows.Forms.Padding(0);
		this.cbGodMode.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbGodMode.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbGodMode.Name = "cbGodMode";
		this.cbGodMode.ReadOnly = false;
		this.cbGodMode.Ripple = true;
		this.cbGodMode.Size = new System.Drawing.Size(108, 37);
		this.cbGodMode.TabIndex = 0;
		this.cbGodMode.Tag = "GodMode";
		this.cbGodMode.Text = "God Mode";
		this.cbGodMode.UseVisualStyleBackColor = true;
		this.cbGodMode.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbDeleteConfirmation.AutoSize = true;
		this.cbDeleteConfirmation.Depth = 0;
		this.cbDeleteConfirmation.Location = new System.Drawing.Point(13, 130);
		this.cbDeleteConfirmation.Margin = new System.Windows.Forms.Padding(0);
		this.cbDeleteConfirmation.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbDeleteConfirmation.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbDeleteConfirmation.Name = "cbDeleteConfirmation";
		this.cbDeleteConfirmation.ReadOnly = false;
		this.cbDeleteConfirmation.Ripple = true;
		this.cbDeleteConfirmation.Size = new System.Drawing.Size(226, 37);
		this.cbDeleteConfirmation.TabIndex = 1;
		this.cbDeleteConfirmation.Tag = "DeleteConfirmation";
		this.cbDeleteConfirmation.Text = "Delete Confirmation Dialog";
		this.cbDeleteConfirmation.UseVisualStyleBackColor = true;
		this.cbDeleteConfirmation.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbAutoRebootLoggedUsers.AutoSize = true;
		this.cbAutoRebootLoggedUsers.Depth = 0;
		this.cbAutoRebootLoggedUsers.Location = new System.Drawing.Point(13, 165);
		this.cbAutoRebootLoggedUsers.Margin = new System.Windows.Forms.Padding(0);
		this.cbAutoRebootLoggedUsers.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbAutoRebootLoggedUsers.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbAutoRebootLoggedUsers.Name = "cbAutoRebootLoggedUsers";
		this.cbAutoRebootLoggedUsers.ReadOnly = false;
		this.cbAutoRebootLoggedUsers.Ripple = true;
		this.cbAutoRebootLoggedUsers.Size = new System.Drawing.Size(307, 37);
		this.cbAutoRebootLoggedUsers.TabIndex = 2;
		this.cbAutoRebootLoggedUsers.Tag = "AutoReboot";
		this.cbAutoRebootLoggedUsers.Text = "Automatic reboot with logged on users";
		this.cbAutoRebootLoggedUsers.UseVisualStyleBackColor = true;
		this.cbAutoRebootLoggedUsers.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbChangeShutdownUpdates.AutoSize = true;
		this.cbChangeShutdownUpdates.Depth = 0;
		this.cbChangeShutdownUpdates.Location = new System.Drawing.Point(13, 198);
		this.cbChangeShutdownUpdates.Margin = new System.Windows.Forms.Padding(0);
		this.cbChangeShutdownUpdates.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbChangeShutdownUpdates.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbChangeShutdownUpdates.Name = "cbChangeShutdownUpdates";
		this.cbChangeShutdownUpdates.ReadOnly = false;
		this.cbChangeShutdownUpdates.Ripple = true;
		this.cbChangeShutdownUpdates.Size = new System.Drawing.Size(435, 37);
		this.cbChangeShutdownUpdates.TabIndex = 3;
		this.cbChangeShutdownUpdates.Tag = "ShutdownUpdates";
		this.cbChangeShutdownUpdates.Text = "Change default shutdown option if updates are available";
		this.cbChangeShutdownUpdates.UseVisualStyleBackColor = true;
		this.cbChangeShutdownUpdates.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbWinKeys.AutoSize = true;
		this.cbWinKeys.Depth = 0;
		this.cbWinKeys.Location = new System.Drawing.Point(13, 235);
		this.cbWinKeys.Margin = new System.Windows.Forms.Padding(0);
		this.cbWinKeys.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbWinKeys.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbWinKeys.Name = "cbWinKeys";
		this.cbWinKeys.ReadOnly = false;
		this.cbWinKeys.Ripple = true;
		this.cbWinKeys.Size = new System.Drawing.Size(281, 37);
		this.cbWinKeys.TabIndex = 4;
		this.cbWinKeys.Tag = "WinKeys";
		this.cbWinKeys.Text = "Enable Windows key combinations";
		this.cbWinKeys.UseVisualStyleBackColor = true;
		this.cbWinKeys.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbWebServiceDialog.AutoSize = true;
		this.cbWebServiceDialog.Depth = 0;
		this.cbWebServiceDialog.Location = new System.Drawing.Point(13, 270);
		this.cbWebServiceDialog.Margin = new System.Windows.Forms.Padding(0);
		this.cbWebServiceDialog.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbWebServiceDialog.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbWebServiceDialog.Name = "cbWebServiceDialog";
		this.cbWebServiceDialog.ReadOnly = false;
		this.cbWebServiceDialog.Ripple = true;
		this.cbWebServiceDialog.Size = new System.Drawing.Size(477, 37);
		this.cbWebServiceDialog.TabIndex = 5;
		this.cbWebServiceDialog.Tag = "WebServiceDialog";
		this.cbWebServiceDialog.Text = "Enable \"Use the Web service to find the correct program\" dialog";
		this.cbWebServiceDialog.UseVisualStyleBackColor = true;
		this.cbWebServiceDialog.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbAutoRestartCrash.AutoSize = true;
		this.cbAutoRestartCrash.Depth = 0;
		this.cbAutoRestartCrash.Location = new System.Drawing.Point(13, 305);
		this.cbAutoRestartCrash.Margin = new System.Windows.Forms.Padding(0);
		this.cbAutoRestartCrash.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbAutoRestartCrash.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbAutoRestartCrash.Name = "cbAutoRestartCrash";
		this.cbAutoRestartCrash.ReadOnly = false;
		this.cbAutoRestartCrash.Ripple = true;
		this.cbAutoRestartCrash.Size = new System.Drawing.Size(292, 37);
		this.cbAutoRestartCrash.TabIndex = 6;
		this.cbAutoRestartCrash.Tag = "AutoRestartCrash";
		this.cbAutoRestartCrash.Text = "Automatic restart after system crash";
		this.cbAutoRestartCrash.UseVisualStyleBackColor = true;
		this.cbAutoRestartCrash.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbErrorReporting.AutoSize = true;
		this.cbErrorReporting.Depth = 0;
		this.cbErrorReporting.Location = new System.Drawing.Point(13, 340);
		this.cbErrorReporting.Margin = new System.Windows.Forms.Padding(0);
		this.cbErrorReporting.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbErrorReporting.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbErrorReporting.Name = "cbErrorReporting";
		this.cbErrorReporting.ReadOnly = false;
		this.cbErrorReporting.Ripple = true;
		this.cbErrorReporting.Size = new System.Drawing.Size(135, 37);
		this.cbErrorReporting.TabIndex = 7;
		this.cbErrorReporting.Tag = "ErrorReporting";
		this.cbErrorReporting.Text = "Error reporting";
		this.cbErrorReporting.UseVisualStyleBackColor = true;
		this.cbErrorReporting.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbFilePrintSharing.AutoSize = true;
		this.cbFilePrintSharing.Depth = 0;
		this.cbFilePrintSharing.Location = new System.Drawing.Point(13, 375);
		this.cbFilePrintSharing.Margin = new System.Windows.Forms.Padding(0);
		this.cbFilePrintSharing.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbFilePrintSharing.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbFilePrintSharing.Name = "cbFilePrintSharing";
		this.cbFilePrintSharing.ReadOnly = false;
		this.cbFilePrintSharing.Ripple = true;
		this.cbFilePrintSharing.Size = new System.Drawing.Size(157, 37);
		this.cbFilePrintSharing.TabIndex = 8;
		this.cbFilePrintSharing.Tag = "FileSharing";
		this.cbFilePrintSharing.Text = "File/Print sharing";
		this.cbFilePrintSharing.UseVisualStyleBackColor = true;
		this.cbFilePrintSharing.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbKernelPaging.AutoSize = true;
		this.cbKernelPaging.Depth = 0;
		this.cbKernelPaging.Location = new System.Drawing.Point(13, 410);
		this.cbKernelPaging.Margin = new System.Windows.Forms.Padding(0);
		this.cbKernelPaging.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbKernelPaging.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbKernelPaging.Name = "cbKernelPaging";
		this.cbKernelPaging.ReadOnly = false;
		this.cbKernelPaging.Ripple = true;
		this.cbKernelPaging.Size = new System.Drawing.Size(132, 37);
		this.cbKernelPaging.TabIndex = 9;
		this.cbKernelPaging.Tag = "KernelPaging";
		this.cbKernelPaging.Text = "Kernel paging";
		this.cbKernelPaging.UseVisualStyleBackColor = true;
		this.cbKernelPaging.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbClearPageFileShutdown.AutoSize = true;
		this.cbClearPageFileShutdown.Depth = 0;
		this.cbClearPageFileShutdown.Location = new System.Drawing.Point(13, 445);
		this.cbClearPageFileShutdown.Margin = new System.Windows.Forms.Padding(0);
		this.cbClearPageFileShutdown.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbClearPageFileShutdown.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbClearPageFileShutdown.Name = "cbClearPageFileShutdown";
		this.cbClearPageFileShutdown.ReadOnly = false;
		this.cbClearPageFileShutdown.Ripple = true;
		this.cbClearPageFileShutdown.Size = new System.Drawing.Size(247, 37);
		this.cbClearPageFileShutdown.TabIndex = 10;
		this.cbClearPageFileShutdown.Tag = "ClearPageFile";
		this.cbClearPageFileShutdown.Text = "Clear page file after shutdown";
		this.cbClearPageFileShutdown.UseVisualStyleBackColor = true;
		this.cbClearPageFileShutdown.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbBootDefragmentation.AutoSize = true;
		this.cbBootDefragmentation.Depth = 0;
		this.cbBootDefragmentation.Location = new System.Drawing.Point(13, 480);
		this.cbBootDefragmentation.Margin = new System.Windows.Forms.Padding(0);
		this.cbBootDefragmentation.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbBootDefragmentation.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbBootDefragmentation.Name = "cbBootDefragmentation";
		this.cbBootDefragmentation.ReadOnly = false;
		this.cbBootDefragmentation.Ripple = true;
		this.cbBootDefragmentation.Size = new System.Drawing.Size(190, 37);
		this.cbBootDefragmentation.TabIndex = 11;
		this.cbBootDefragmentation.Tag = "BootDefrag";
		this.cbBootDefragmentation.Text = "Boot defragmentation";
		this.cbBootDefragmentation.UseVisualStyleBackColor = true;
		this.cbBootDefragmentation.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbReserveBandwidth.AutoSize = true;
		this.cbReserveBandwidth.Depth = 0;
		this.cbReserveBandwidth.Location = new System.Drawing.Point(13, 515);
		this.cbReserveBandwidth.Margin = new System.Windows.Forms.Padding(0);
		this.cbReserveBandwidth.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbReserveBandwidth.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbReserveBandwidth.Name = "cbReserveBandwidth";
		this.cbReserveBandwidth.ReadOnly = false;
		this.cbReserveBandwidth.Ripple = true;
		this.cbReserveBandwidth.Size = new System.Drawing.Size(274, 37);
		this.cbReserveBandwidth.TabIndex = 12;
		this.cbReserveBandwidth.Tag = "ReserveBandwidth";
		this.cbReserveBandwidth.Text = "Reserve bandwidth for the system";
		this.cbReserveBandwidth.UseVisualStyleBackColor = true;
		this.cbReserveBandwidth.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbVerboseMessages.AutoSize = true;
		this.cbVerboseMessages.Depth = 0;
		this.cbVerboseMessages.Location = new System.Drawing.Point(13, 550);
		this.cbVerboseMessages.Margin = new System.Windows.Forms.Padding(0);
		this.cbVerboseMessages.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbVerboseMessages.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbVerboseMessages.Name = "cbVerboseMessages";
		this.cbVerboseMessages.ReadOnly = false;
		this.cbVerboseMessages.Ripple = true;
		this.cbVerboseMessages.Size = new System.Drawing.Size(382, 37);
		this.cbVerboseMessages.TabIndex = 13;
		this.cbVerboseMessages.Tag = "VerboseMessages";
		this.cbVerboseMessages.Text = "Verbose messages for startup, shutdown, logon...";
		this.cbVerboseMessages.UseVisualStyleBackColor = true;
		this.cbVerboseMessages.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbSeparateExplorer.AutoSize = true;
		this.cbSeparateExplorer.Depth = 0;
		this.cbSeparateExplorer.Location = new System.Drawing.Point(13, 585);
		this.cbSeparateExplorer.Margin = new System.Windows.Forms.Padding(0);
		this.cbSeparateExplorer.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbSeparateExplorer.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbSeparateExplorer.Name = "cbSeparateExplorer";
		this.cbSeparateExplorer.ReadOnly = false;
		this.cbSeparateExplorer.Ripple = true;
		this.cbSeparateExplorer.Size = new System.Drawing.Size(218, 37);
		this.cbSeparateExplorer.TabIndex = 14;
		this.cbSeparateExplorer.Tag = "SeparateExplorer";
		this.cbSeparateExplorer.Text = "Separate Explorer process";
		this.cbSeparateExplorer.UseVisualStyleBackColor = true;
		this.cbSeparateExplorer.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbCrashCtrlScroll.AutoSize = true;
		this.cbCrashCtrlScroll.Depth = 0;
		this.cbCrashCtrlScroll.Location = new System.Drawing.Point(13, 620);
		this.cbCrashCtrlScroll.Margin = new System.Windows.Forms.Padding(0);
		this.cbCrashCtrlScroll.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbCrashCtrlScroll.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbCrashCtrlScroll.Name = "cbCrashCtrlScroll";
		this.cbCrashCtrlScroll.ReadOnly = false;
		this.cbCrashCtrlScroll.Ripple = true;
		this.cbCrashCtrlScroll.Size = new System.Drawing.Size(179, 37);
		this.cbCrashCtrlScroll.TabIndex = 15;
		this.cbCrashCtrlScroll.Tag = "CrashCtrlScroll";
		this.cbCrashCtrlScroll.Text = "Crash on ctrl + scroll";
		this.cbCrashCtrlScroll.UseVisualStyleBackColor = true;
		this.cbCrashCtrlScroll.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbMobilityCenter.AutoSize = true;
		this.cbMobilityCenter.Depth = 0;
		this.cbMobilityCenter.Location = new System.Drawing.Point(13, 655);
		this.cbMobilityCenter.Margin = new System.Windows.Forms.Padding(0);
		this.cbMobilityCenter.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbMobilityCenter.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbMobilityCenter.Name = "cbMobilityCenter";
		this.cbMobilityCenter.ReadOnly = false;
		this.cbMobilityCenter.Ripple = true;
		this.cbMobilityCenter.Size = new System.Drawing.Size(141, 37);
		this.cbMobilityCenter.TabIndex = 16;
		this.cbMobilityCenter.Tag = "MobilityCenter";
		this.cbMobilityCenter.Text = "Mobility Center";
		this.cbMobilityCenter.UseVisualStyleBackColor = true;
		this.cbMobilityCenter.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbDisplayWinVersion.AutoSize = true;
		this.cbDisplayWinVersion.Depth = 0;
		this.cbDisplayWinVersion.Location = new System.Drawing.Point(521, 102);
		this.cbDisplayWinVersion.Margin = new System.Windows.Forms.Padding(0);
		this.cbDisplayWinVersion.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbDisplayWinVersion.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbDisplayWinVersion.Name = "cbDisplayWinVersion";
		this.cbDisplayWinVersion.ReadOnly = false;
		this.cbDisplayWinVersion.Ripple = true;
		this.cbDisplayWinVersion.Size = new System.Drawing.Size(212, 37);
		this.cbDisplayWinVersion.TabIndex = 17;
		this.cbDisplayWinVersion.Tag = "DisplayWinVer";
		this.cbDisplayWinVersion.Text = "Display Windows version";
		this.cbDisplayWinVersion.UseVisualStyleBackColor = true;
		this.cbDisplayWinVersion.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbDisplayTrayItems.AutoSize = true;
		this.cbDisplayTrayItems.Depth = 0;
		this.cbDisplayTrayItems.Location = new System.Drawing.Point(521, 137);
		this.cbDisplayTrayItems.Margin = new System.Windows.Forms.Padding(0);
		this.cbDisplayTrayItems.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbDisplayTrayItems.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbDisplayTrayItems.Name = "cbDisplayTrayItems";
		this.cbDisplayTrayItems.ReadOnly = false;
		this.cbDisplayTrayItems.Ripple = true;
		this.cbDisplayTrayItems.Size = new System.Drawing.Size(162, 37);
		this.cbDisplayTrayItems.TabIndex = 18;
		this.cbDisplayTrayItems.Tag = "DisplayTray";
		this.cbDisplayTrayItems.Text = "Display tray items";
		this.cbDisplayTrayItems.UseVisualStyleBackColor = true;
		this.cbDisplayTrayItems.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbWindowAnimations.AutoSize = true;
		this.cbWindowAnimations.Depth = 0;
		this.cbWindowAnimations.Location = new System.Drawing.Point(521, 172);
		this.cbWindowAnimations.Margin = new System.Windows.Forms.Padding(0);
		this.cbWindowAnimations.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbWindowAnimations.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbWindowAnimations.Name = "cbWindowAnimations";
		this.cbWindowAnimations.ReadOnly = false;
		this.cbWindowAnimations.Ripple = true;
		this.cbWindowAnimations.Size = new System.Drawing.Size(176, 37);
		this.cbWindowAnimations.TabIndex = 19;
		this.cbWindowAnimations.Tag = "WinAnimations";
		this.cbWindowAnimations.Text = "Window animations";
		this.cbWindowAnimations.UseVisualStyleBackColor = true;
		this.cbWindowAnimations.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbAeroShake.AutoSize = true;
		this.cbAeroShake.Depth = 0;
		this.cbAeroShake.Location = new System.Drawing.Point(521, 207);
		this.cbAeroShake.Margin = new System.Windows.Forms.Padding(0);
		this.cbAeroShake.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbAeroShake.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbAeroShake.Name = "cbAeroShake";
		this.cbAeroShake.ReadOnly = false;
		this.cbAeroShake.Ripple = true;
		this.cbAeroShake.Size = new System.Drawing.Size(115, 37);
		this.cbAeroShake.TabIndex = 20;
		this.cbAeroShake.Tag = "AeroShake";
		this.cbAeroShake.Text = "Aero Shake";
		this.cbAeroShake.UseVisualStyleBackColor = true;
		this.cbAeroShake.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbWindowSnap.AutoSize = true;
		this.cbWindowSnap.Depth = 0;
		this.cbWindowSnap.Location = new System.Drawing.Point(521, 242);
		this.cbWindowSnap.Margin = new System.Windows.Forms.Padding(0);
		this.cbWindowSnap.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbWindowSnap.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbWindowSnap.Name = "cbWindowSnap";
		this.cbWindowSnap.ReadOnly = false;
		this.cbWindowSnap.Ripple = true;
		this.cbWindowSnap.Size = new System.Drawing.Size(131, 37);
		this.cbWindowSnap.TabIndex = 21;
		this.cbWindowSnap.Tag = "WinSnap";
		this.cbWindowSnap.Text = "Window snap";
		this.cbWindowSnap.UseVisualStyleBackColor = true;
		this.cbWindowSnap.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbLockScreen.AutoSize = true;
		this.cbLockScreen.Depth = 0;
		this.cbLockScreen.Location = new System.Drawing.Point(524, 324);
		this.cbLockScreen.Margin = new System.Windows.Forms.Padding(0);
		this.cbLockScreen.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbLockScreen.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbLockScreen.Name = "cbLockScreen";
		this.cbLockScreen.ReadOnly = false;
		this.cbLockScreen.Ripple = true;
		this.cbLockScreen.Size = new System.Drawing.Size(119, 37);
		this.cbLockScreen.TabIndex = 22;
		this.cbLockScreen.Tag = "LockScreen";
		this.cbLockScreen.Text = "Lock screen";
		this.cbLockScreen.UseVisualStyleBackColor = true;
		this.cbLockScreen.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbDarkTheme.AutoSize = true;
		this.cbDarkTheme.Depth = 0;
		this.cbDarkTheme.Location = new System.Drawing.Point(524, 359);
		this.cbDarkTheme.Margin = new System.Windows.Forms.Padding(0);
		this.cbDarkTheme.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbDarkTheme.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbDarkTheme.Name = "cbDarkTheme";
		this.cbDarkTheme.ReadOnly = false;
		this.cbDarkTheme.Ripple = true;
		this.cbDarkTheme.Size = new System.Drawing.Size(116, 37);
		this.cbDarkTheme.TabIndex = 23;
		this.cbDarkTheme.Tag = "DarkTheme";
		this.cbDarkTheme.Text = "Dark theme";
		this.cbDarkTheme.UseVisualStyleBackColor = true;
		this.cbDarkTheme.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbBalloonNotification.AutoSize = true;
		this.cbBalloonNotification.Depth = 0;
		this.cbBalloonNotification.Location = new System.Drawing.Point(524, 394);
		this.cbBalloonNotification.Margin = new System.Windows.Forms.Padding(0);
		this.cbBalloonNotification.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbBalloonNotification.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbBalloonNotification.Name = "cbBalloonNotification";
		this.cbBalloonNotification.ReadOnly = false;
		this.cbBalloonNotification.Ripple = true;
		this.cbBalloonNotification.Size = new System.Drawing.Size(174, 37);
		this.cbBalloonNotification.TabIndex = 24;
		this.cbBalloonNotification.Tag = "BalloonNotif";
		this.cbBalloonNotification.Text = "Balloon notification";
		this.cbBalloonNotification.UseVisualStyleBackColor = true;
		this.cbBalloonNotification.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbActionCenter.AutoSize = true;
		this.cbActionCenter.Depth = 0;
		this.cbActionCenter.Location = new System.Drawing.Point(524, 429);
		this.cbActionCenter.Margin = new System.Windows.Forms.Padding(0);
		this.cbActionCenter.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbActionCenter.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbActionCenter.Name = "cbActionCenter";
		this.cbActionCenter.ReadOnly = false;
		this.cbActionCenter.Ripple = true;
		this.cbActionCenter.Size = new System.Drawing.Size(127, 37);
		this.cbActionCenter.TabIndex = 25;
		this.cbActionCenter.Tag = "ActionCenter";
		this.cbActionCenter.Text = "Action center";
		this.cbActionCenter.UseVisualStyleBackColor = true;
		this.cbActionCenter.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbClassicVolumeMixer.AutoSize = true;
		this.cbClassicVolumeMixer.Depth = 0;
		this.cbClassicVolumeMixer.Location = new System.Drawing.Point(524, 464);
		this.cbClassicVolumeMixer.Margin = new System.Windows.Forms.Padding(0);
		this.cbClassicVolumeMixer.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbClassicVolumeMixer.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbClassicVolumeMixer.Name = "cbClassicVolumeMixer";
		this.cbClassicVolumeMixer.ReadOnly = false;
		this.cbClassicVolumeMixer.Ripple = true;
		this.cbClassicVolumeMixer.Size = new System.Drawing.Size(185, 37);
		this.cbClassicVolumeMixer.TabIndex = 26;
		this.cbClassicVolumeMixer.Tag = "ClassicVolume";
		this.cbClassicVolumeMixer.Text = "Classic volume mixer";
		this.cbClassicVolumeMixer.UseVisualStyleBackColor = true;
		this.cbClassicVolumeMixer.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbNotificationBalloons.AutoSize = true;
		this.cbNotificationBalloons.Depth = 0;
		this.cbNotificationBalloons.Location = new System.Drawing.Point(740, 102);
		this.cbNotificationBalloons.Margin = new System.Windows.Forms.Padding(0);
		this.cbNotificationBalloons.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbNotificationBalloons.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbNotificationBalloons.Name = "cbNotificationBalloons";
		this.cbNotificationBalloons.ReadOnly = false;
		this.cbNotificationBalloons.Ripple = true;
		this.cbNotificationBalloons.Size = new System.Drawing.Size(183, 37);
		this.cbNotificationBalloons.TabIndex = 27;
		this.cbNotificationBalloons.Tag = "NotifBalloons";
		this.cbNotificationBalloons.Text = "Notification balloons";
		this.cbNotificationBalloons.UseVisualStyleBackColor = true;
		this.cbNotificationBalloons.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbShowLibraries.AutoSize = true;
		this.cbShowLibraries.Depth = 0;
		this.cbShowLibraries.Location = new System.Drawing.Point(740, 137);
		this.cbShowLibraries.Margin = new System.Windows.Forms.Padding(0);
		this.cbShowLibraries.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbShowLibraries.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbShowLibraries.Name = "cbShowLibraries";
		this.cbShowLibraries.ReadOnly = false;
		this.cbShowLibraries.Ripple = true;
		this.cbShowLibraries.Size = new System.Drawing.Size(135, 37);
		this.cbShowLibraries.TabIndex = 28;
		this.cbShowLibraries.Tag = "ShowLibraries";
		this.cbShowLibraries.Text = "Show libraries";
		this.cbShowLibraries.UseVisualStyleBackColor = true;
		this.cbShowLibraries.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbShowRecycleBinComputer.AutoSize = true;
		this.cbShowRecycleBinComputer.Depth = 0;
		this.cbShowRecycleBinComputer.Location = new System.Drawing.Point(740, 172);
		this.cbShowRecycleBinComputer.Margin = new System.Windows.Forms.Padding(0);
		this.cbShowRecycleBinComputer.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbShowRecycleBinComputer.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbShowRecycleBinComputer.Name = "cbShowRecycleBinComputer";
		this.cbShowRecycleBinComputer.ReadOnly = false;
		this.cbShowRecycleBinComputer.Ripple = true;
		this.cbShowRecycleBinComputer.Size = new System.Drawing.Size(247, 37);
		this.cbShowRecycleBinComputer.TabIndex = 29;
		this.cbShowRecycleBinComputer.Tag = "RecycleBinComputer";
		this.cbShowRecycleBinComputer.Text = "Show recycle bin on computer";
		this.cbShowRecycleBinComputer.UseVisualStyleBackColor = true;
		this.cbShowRecycleBinComputer.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbShowDesktopPreview.AutoSize = true;
		this.cbShowDesktopPreview.Depth = 0;
		this.cbShowDesktopPreview.Location = new System.Drawing.Point(740, 207);
		this.cbShowDesktopPreview.Margin = new System.Windows.Forms.Padding(0);
		this.cbShowDesktopPreview.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbShowDesktopPreview.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbShowDesktopPreview.Name = "cbShowDesktopPreview";
		this.cbShowDesktopPreview.ReadOnly = false;
		this.cbShowDesktopPreview.Ripple = true;
		this.cbShowDesktopPreview.Size = new System.Drawing.Size(193, 37);
		this.cbShowDesktopPreview.TabIndex = 30;
		this.cbShowDesktopPreview.Tag = "DesktopPreview";
		this.cbShowDesktopPreview.Text = "Show desktop preview";
		this.cbShowDesktopPreview.UseVisualStyleBackColor = true;
		this.cbShowDesktopPreview.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.cbExplorerCheckboxSelection.AutoSize = true;
		this.cbExplorerCheckboxSelection.Depth = 0;
		this.cbExplorerCheckboxSelection.Location = new System.Drawing.Point(740, 242);
		this.cbExplorerCheckboxSelection.Margin = new System.Windows.Forms.Padding(0);
		this.cbExplorerCheckboxSelection.MouseLocation = new System.Drawing.Point(-1, -1);
		this.cbExplorerCheckboxSelection.MouseState = MaterialSkin.MouseState.HOVER;
		this.cbExplorerCheckboxSelection.Name = "cbExplorerCheckboxSelection";
		this.cbExplorerCheckboxSelection.ReadOnly = false;
		this.cbExplorerCheckboxSelection.Ripple = true;
		this.cbExplorerCheckboxSelection.Size = new System.Drawing.Size(230, 37);
		this.cbExplorerCheckboxSelection.TabIndex = 31;
		this.cbExplorerCheckboxSelection.Tag = "ExplorerCheckbox";
		this.cbExplorerCheckboxSelection.Text = "Explorer checkbox selection";
		this.cbExplorerCheckboxSelection.UseVisualStyleBackColor = true;
		this.cbExplorerCheckboxSelection.CheckedChanged += new System.EventHandler(CheckBox_CheckedChanged);
		this.lblSystem.AutoSize = true;
		this.lblSystem.Depth = 0;
		this.lblSystem.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblSystem.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.lblSystem.Location = new System.Drawing.Point(13, 70);
		this.lblSystem.MouseState = MaterialSkin.MouseState.HOVER;
		this.lblSystem.Name = "lblSystem";
		this.lblSystem.Size = new System.Drawing.Size(116, 19);
		this.lblSystem.TabIndex = 32;
		this.lblSystem.Text = "System Settings";
		this.lblAppearance.AutoSize = true;
		this.lblAppearance.Depth = 0;
		this.lblAppearance.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblAppearance.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.lblAppearance.Location = new System.Drawing.Point(521, 77);
		this.lblAppearance.MouseState = MaterialSkin.MouseState.HOVER;
		this.lblAppearance.Name = "lblAppearance";
		this.lblAppearance.Size = new System.Drawing.Size(147, 19);
		this.lblAppearance.TabIndex = 33;
		this.lblAppearance.Text = "Appearance Settings";
		this.lblExplorer.AutoSize = true;
		this.lblExplorer.Depth = 0;
		this.lblExplorer.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblExplorer.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.lblExplorer.Location = new System.Drawing.Point(740, 77);
		this.lblExplorer.MouseState = MaterialSkin.MouseState.HOVER;
		this.lblExplorer.Name = "lblExplorer";
		this.lblExplorer.Size = new System.Drawing.Size(120, 19);
		this.lblExplorer.TabIndex = 34;
		this.lblExplorer.Text = "Explorer Settings";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1013, 700);
		base.Controls.Add(this.cbExplorerCheckboxSelection);
		base.Controls.Add(this.cbShowDesktopPreview);
		base.Controls.Add(this.cbShowRecycleBinComputer);
		base.Controls.Add(this.cbShowLibraries);
		base.Controls.Add(this.cbNotificationBalloons);
		base.Controls.Add(this.cbClassicVolumeMixer);
		base.Controls.Add(this.cbActionCenter);
		base.Controls.Add(this.cbBalloonNotification);
		base.Controls.Add(this.cbDarkTheme);
		base.Controls.Add(this.cbLockScreen);
		base.Controls.Add(this.cbWindowSnap);
		base.Controls.Add(this.cbAeroShake);
		base.Controls.Add(this.cbWindowAnimations);
		base.Controls.Add(this.cbDisplayTrayItems);
		base.Controls.Add(this.cbDisplayWinVersion);
		base.Controls.Add(this.cbMobilityCenter);
		base.Controls.Add(this.cbCrashCtrlScroll);
		base.Controls.Add(this.cbSeparateExplorer);
		base.Controls.Add(this.cbVerboseMessages);
		base.Controls.Add(this.cbReserveBandwidth);
		base.Controls.Add(this.cbBootDefragmentation);
		base.Controls.Add(this.cbClearPageFileShutdown);
		base.Controls.Add(this.cbKernelPaging);
		base.Controls.Add(this.cbFilePrintSharing);
		base.Controls.Add(this.cbErrorReporting);
		base.Controls.Add(this.cbAutoRestartCrash);
		base.Controls.Add(this.cbWebServiceDialog);
		base.Controls.Add(this.cbWinKeys);
		base.Controls.Add(this.cbChangeShutdownUpdates);
		base.Controls.Add(this.cbAutoRebootLoggedUsers);
		base.Controls.Add(this.cbDeleteConfirmation);
		base.Controls.Add(this.cbGodMode);
		base.Controls.Add(this.lblSystem);
		base.Controls.Add(this.lblAppearance);
		base.Controls.Add(this.lblExplorer);
		base.Name = "FormWindowsCustomizer";
		this.Text = "Windows Customizer";
		base.Load += new System.EventHandler(FormWindowsCustomizer_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
