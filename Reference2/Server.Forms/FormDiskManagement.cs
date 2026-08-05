using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormDiskManagement : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	private Timer timer1;

	private DataGridView dataGridView1;

	private DataGridViewTextBoxColumn colDrive;

	private DataGridViewTextBoxColumn colLabel;

	private DataGridViewTextBoxColumn colFileSystem;

	private DataGridViewTextBoxColumn colType;

	private DataGridViewTextBoxColumn colTotalSize;

	private DataGridViewTextBoxColumn colFreeSpace;

	private DataGridViewTextBoxColumn colUsedPercent;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem refreshToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripMenuItem formatToolStripMenuItem;

	private ToolStripMenuItem deletePartitionToolStripMenuItem;

	private ToolStripMenuItem changeLetterToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator2;

	private ToolStripMenuItem ejectToolStripMenuItem;

	public FormDiskManagement()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormDiskManagement_Load(object sender, EventArgs e)
	{
		timer1.Start();
	}

	private void Closing1(object sender, EventArgs e)
	{
		if (client != null)
		{
			client.Disconnect();
		}
	}

	private void timer1_Tick(object sender, EventArgs e)
	{
		if (parrent != null && !parrent.itsConnect)
		{
			Close();
		}
		if (client != null && !client.itsConnect)
		{
			Close();
		}
	}

	public void PopulateDisks(object[] disks)
	{
		if (base.InvokeRequired)
		{
			Invoke((MethodInvoker)delegate
			{
				PopulateDisks(disks);
			});
			return;
		}
		dataGridView1.Rows.Clear();
		if (disks == null)
		{
			return;
		}
		for (int i = 0; i < disks.Length; i++)
		{
			if (disks[i] is object[] disk && disk.Length >= 6)
			{
				string driveLetter = disk[0]?.ToString() ?? "";
				string label = disk[1]?.ToString() ?? "";
				string fileSystem = disk[2]?.ToString() ?? "";
				string driveType = disk[3]?.ToString() ?? "";
				long totalSize = Convert.ToInt64(disk[4]);
				long freeSpace = Convert.ToInt64(disk[5]);
				string totalStr = FormatSize(totalSize);
				string freeStr = FormatSize(freeSpace);
				string usedPercent = ((totalSize > 0) ? (((double)(totalSize - freeSpace) / (double)totalSize * 100.0).ToString("F1") + "%") : "0%");
				dataGridView1.Rows.Add(driveLetter, label, fileSystem, driveType, totalStr, freeStr, usedPercent);
			}
		}
	}

	public void ShowResult(string message, bool success)
	{
		if (base.InvokeRequired)
		{
			Invoke((MethodInvoker)delegate
			{
				ShowResult(message, success);
			});
			return;
		}
		if (success)
		{
			MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}
		else
		{
			MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		RefreshDisks();
	}

	private void RefreshDisks()
	{
		if (client != null && client.itsConnect)
		{
			client.Send(new object[2] { "DiskManagement", "GetDisks" });
		}
	}

	private void btnRefresh_Click(object sender, EventArgs e)
	{
		RefreshDisks();
	}

	private void btnFormat_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			MessageBox.Show("Client not connected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		if (dataGridView1.SelectedRows.Count == 0)
		{
			MessageBox.Show("Select a drive to format!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		string drive = dataGridView1.SelectedRows[0].Cells[0].Value?.ToString();
		if (!string.IsNullOrEmpty(drive))
		{
			if (drive.StartsWith("C"))
			{
				MessageBox.Show("Cannot format system drive C:!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else if (MessageBox.Show("Are you sure you want to format " + drive + "?\nAll data will be lost!", "Confirm Format", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
			{
				client.Send(new object[4] { "DiskManagement", "Format", drive, "NTFS" });
			}
		}
	}

	private void btnDeletePartition_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			MessageBox.Show("Client not connected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		if (dataGridView1.SelectedRows.Count == 0)
		{
			MessageBox.Show("Select a drive!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		string drive = dataGridView1.SelectedRows[0].Cells[0].Value?.ToString();
		if (!string.IsNullOrEmpty(drive))
		{
			if (drive.StartsWith("C"))
			{
				MessageBox.Show("Cannot delete system partition C:!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else if (MessageBox.Show("Are you sure you want to delete partition " + drive + "?\nAll data will be lost!", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
			{
				client.Send(new object[3] { "DiskManagement", "DeletePartition", drive });
			}
		}
	}

	private void btnChangeLetter_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			MessageBox.Show("Client not connected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		if (dataGridView1.SelectedRows.Count == 0)
		{
			MessageBox.Show("Select a drive!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		string oldDrive = dataGridView1.SelectedRows[0].Cells[0].Value?.ToString();
		if (string.IsNullOrEmpty(oldDrive))
		{
			return;
		}
		if (oldDrive.StartsWith("C"))
		{
			MessageBox.Show("Cannot change system drive letter C:!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		string newLetter = ShowInputDialog("Enter new drive letter (e.g. E):", "Change Drive Letter");
		if (!string.IsNullOrWhiteSpace(newLetter))
		{
			newLetter = newLetter.Trim().ToUpper();
			if (newLetter.Length > 2 || !char.IsLetter(newLetter[0]))
			{
				MessageBox.Show("Invalid drive letter!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			client.Send(new object[4] { "DiskManagement", "ChangeLetter", oldDrive, newLetter });
		}
	}

	private void btnEject_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			MessageBox.Show("Client not connected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		if (dataGridView1.SelectedRows.Count == 0)
		{
			MessageBox.Show("Select a drive to eject!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		string drive = dataGridView1.SelectedRows[0].Cells[0].Value?.ToString();
		if (!string.IsNullOrEmpty(drive))
		{
			client.Send(new object[3] { "DiskManagement", "Eject", drive });
		}
	}

	private string FormatSize(long bytes)
	{
		if (bytes <= 0)
		{
			return "0 B";
		}
		string[] sizes = new string[5] { "B", "KB", "MB", "GB", "TB" };
		int order = 0;
		double size = bytes;
		while (size >= 1024.0 && order < sizes.Length - 1)
		{
			order++;
			size /= 1024.0;
		}
		return size.ToString("F2") + " " + sizes[order];
	}

	private string ShowInputDialog(string prompt, string title)
	{
		Form inputForm = new Form();
		inputForm.Text = title;
		inputForm.Size = new Size(350, 150);
		inputForm.StartPosition = FormStartPosition.CenterParent;
		inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
		inputForm.MaximizeBox = false;
		inputForm.MinimizeBox = false;
		Label label = new Label();
		label.Text = prompt;
		label.Location = new Point(10, 15);
		label.Size = new Size(320, 20);
		TextBox textBox = new TextBox();
		textBox.Location = new Point(10, 40);
		textBox.Size = new Size(310, 25);
		Button btnOk = new Button();
		btnOk.Text = "OK";
		btnOk.DialogResult = DialogResult.OK;
		btnOk.Location = new Point(160, 75);
		btnOk.Size = new Size(75, 25);
		Button btnCancel = new Button();
		btnCancel.Text = "Cancel";
		btnCancel.DialogResult = DialogResult.Cancel;
		btnCancel.Location = new Point(245, 75);
		btnCancel.Size = new Size(75, 25);
		inputForm.Controls.AddRange(new Control[4] { label, textBox, btnOk, btnCancel });
		inputForm.AcceptButton = btnOk;
		inputForm.CancelButton = btnCancel;
		if (inputForm.ShowDialog(this) == DialogResult.OK)
		{
			return textBox.Text;
		}
		return null;
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
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
		System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormDiskManagement));
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.dataGridView1 = new System.Windows.Forms.DataGridView();
		this.colDrive = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.colLabel = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.colFileSystem = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.colTotalSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.colFreeSpace = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.colUsedPercent = new System.Windows.Forms.DataGridViewTextBoxColumn();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.formatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deletePartitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.changeLetterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.ejectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.dataGridView1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		base.SuspendLayout();
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.dataGridView1.AllowUserToAddRows = false;
		this.dataGridView1.AllowUserToDeleteRows = false;
		this.dataGridView1.AllowUserToResizeColumns = false;
		this.dataGridView1.AllowUserToResizeRows = false;
		dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(30, 136, 229);
		dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
		this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
		this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
		this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
		this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9f);
		dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White;
		dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(30, 136, 229);
		dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
		this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
		this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		this.dataGridView1.Columns.AddRange(this.colDrive, this.colLabel, this.colFileSystem, this.colType, this.colTotalSize, this.colFreeSpace, this.colUsedPercent);
		this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
		this.dataGridView1.Cursor = System.Windows.Forms.Cursors.Default;
		dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
		dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9f);
		dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
		dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(30, 136, 229);
		dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
		dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
		this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
		this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dataGridView1.EnableHeadersVisualStyles = false;
		this.dataGridView1.GridColor = System.Drawing.Color.FromArgb(240, 240, 240);
		this.dataGridView1.Location = new System.Drawing.Point(3, 64);
		this.dataGridView1.MultiSelect = false;
		this.dataGridView1.Name = "dataGridView1";
		this.dataGridView1.ReadOnly = true;
		this.dataGridView1.RowHeadersVisible = false;
		this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
		this.dataGridView1.ShowCellErrors = false;
		this.dataGridView1.ShowCellToolTips = false;
		this.dataGridView1.ShowEditingIcon = false;
		this.dataGridView1.ShowRowErrors = false;
		this.dataGridView1.Size = new System.Drawing.Size(694, 373);
		this.dataGridView1.TabIndex = 0;
		this.colDrive.HeaderText = "Drive";
		this.colDrive.Name = "colDrive";
		this.colDrive.ReadOnly = true;
		this.colDrive.Width = 50;
		this.colLabel.HeaderText = "Label";
		this.colLabel.Name = "colLabel";
		this.colLabel.ReadOnly = true;
		this.colLabel.Width = 120;
		this.colFileSystem.HeaderText = "FileSystem";
		this.colFileSystem.Name = "colFileSystem";
		this.colFileSystem.ReadOnly = true;
		this.colFileSystem.Width = 80;
		this.colType.HeaderText = "Type";
		this.colType.Name = "colType";
		this.colType.ReadOnly = true;
		this.colType.Width = 90;
		this.colTotalSize.HeaderText = "Total Size";
		this.colTotalSize.Name = "colTotalSize";
		this.colTotalSize.ReadOnly = true;
		this.colFreeSpace.HeaderText = "Free Space";
		this.colFreeSpace.Name = "colFreeSpace";
		this.colFreeSpace.ReadOnly = true;
		this.colUsedPercent.HeaderText = "Used %";
		this.colUsedPercent.Name = "colUsedPercent";
		this.colUsedPercent.ReadOnly = true;
		this.colUsedPercent.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[7] { this.refreshToolStripMenuItem, this.toolStripSeparator1, this.formatToolStripMenuItem, this.deletePartitionToolStripMenuItem, this.changeLetterToolStripMenuItem, this.toolStripSeparator2, this.ejectToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(156, 126);
		this.refreshToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("refreshToolStripMenuItem.Image");
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
		this.refreshToolStripMenuItem.Text = "Refresh";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(btnRefresh_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(152, 6);
		this.formatToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("formatToolStripMenuItem.Image");
		this.formatToolStripMenuItem.Name = "formatToolStripMenuItem";
		this.formatToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
		this.formatToolStripMenuItem.Text = "Format";
		this.formatToolStripMenuItem.Click += new System.EventHandler(btnFormat_Click);
		this.deletePartitionToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("deletePartitionToolStripMenuItem.Image");
		this.deletePartitionToolStripMenuItem.Name = "deletePartitionToolStripMenuItem";
		this.deletePartitionToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
		this.deletePartitionToolStripMenuItem.Text = "Delete Partition";
		this.deletePartitionToolStripMenuItem.Click += new System.EventHandler(btnDeletePartition_Click);
		this.changeLetterToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("changeLetterToolStripMenuItem.Image");
		this.changeLetterToolStripMenuItem.Name = "changeLetterToolStripMenuItem";
		this.changeLetterToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
		this.changeLetterToolStripMenuItem.Text = "Change Letter";
		this.changeLetterToolStripMenuItem.Click += new System.EventHandler(btnChangeLetter_Click);
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		this.toolStripSeparator2.Size = new System.Drawing.Size(152, 6);
		this.ejectToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("ejectToolStripMenuItem.Image");
		this.ejectToolStripMenuItem.Name = "ejectToolStripMenuItem";
		this.ejectToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
		this.ejectToolStripMenuItem.Text = "Eject";
		this.ejectToolStripMenuItem.Click += new System.EventHandler(btnEject_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(700, 440);
		base.Controls.Add(this.dataGridView1);
		base.Name = "FormDiskManagement";
		this.Text = "Disk Management";
		base.Load += new System.EventHandler(FormDiskManagement_Load);
		((System.ComponentModel.ISupportInitialize)this.dataGridView1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
