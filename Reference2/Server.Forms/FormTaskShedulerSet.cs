using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using Server.Helper;

namespace Server.Forms;

public class FormTaskShedulerSet : FormMaterial
{
	private IContainer components;

	private MaterialLabel label1;

	private TextBox textBoxName;

	private MaterialLabel label2;

	private TextBox textBoxPath;

	private MaterialLabel label3;

	private TextBox textBoxArgs;

	private Button buttonOk;

	private Button buttonCancel;

	public string TaskName { get; set; }

	public string TaskPath { get; set; }

	public string TaskArguments { get; set; }

	public FormTaskShedulerSet()
	{
		InitializeComponent();
	}

	private void buttonOk_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(textBoxName.Text) || string.IsNullOrWhiteSpace(textBoxPath.Text))
		{
			MessageBox.Show("Name and Path cannot be empty.");
			return;
		}
		TaskName = textBoxName.Text;
		TaskPath = textBoxPath.Text;
		TaskArguments = textBoxArgs.Text;
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void buttonCancel_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
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
		this.label1 = new MaterialSkin.Controls.MaterialLabel();
		this.textBoxName = new System.Windows.Forms.TextBox();
		this.label2 = new MaterialSkin.Controls.MaterialLabel();
		this.textBoxPath = new System.Windows.Forms.TextBox();
		this.label3 = new MaterialSkin.Controls.MaterialLabel();
		this.textBoxArgs = new System.Windows.Forms.TextBox();
		this.buttonOk = new System.Windows.Forms.Button();
		this.buttonCancel = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Depth = 0;
		this.label1.Font = new System.Drawing.Font("Roboto", 11f);
		this.label1.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.label1.Location = new System.Drawing.Point(12, 75);
		this.label1.MouseState = MaterialSkin.MouseState.HOVER;
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(86, 19);
		this.label1.TabIndex = 0;
		this.label1.Text = "Task Name";
		this.textBoxName.Location = new System.Drawing.Point(12, 97);
		this.textBoxName.Name = "textBoxName";
		this.textBoxName.Size = new System.Drawing.Size(260, 20);
		this.textBoxName.TabIndex = 1;
		this.label2.AutoSize = true;
		this.label2.Depth = 0;
		this.label2.Font = new System.Drawing.Font("Roboto", 11f);
		this.label2.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.label2.Location = new System.Drawing.Point(12, 130);
		this.label2.MouseState = MaterialSkin.MouseState.HOVER;
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(76, 19);
		this.label2.TabIndex = 2;
		this.label2.Text = "Task Path";
		this.textBoxPath.Location = new System.Drawing.Point(12, 152);
		this.textBoxPath.Name = "textBoxPath";
		this.textBoxPath.Size = new System.Drawing.Size(260, 20);
		this.textBoxPath.TabIndex = 3;
		this.label3.AutoSize = true;
		this.label3.Depth = 0;
		this.label3.Font = new System.Drawing.Font("Roboto", 11f);
		this.label3.ForeColor = System.Drawing.Color.FromArgb(222, 0, 0, 0);
		this.label3.Location = new System.Drawing.Point(12, 185);
		this.label3.MouseState = MaterialSkin.MouseState.HOVER;
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(82, 19);
		this.label3.TabIndex = 4;
		this.label3.Text = "Arguments";
		this.textBoxArgs.Location = new System.Drawing.Point(12, 207);
		this.textBoxArgs.Name = "textBoxArgs";
		this.textBoxArgs.Size = new System.Drawing.Size(260, 20);
		this.textBoxArgs.TabIndex = 5;
		this.buttonOk.Location = new System.Drawing.Point(12, 243);
		this.buttonOk.Name = "buttonOk";
		this.buttonOk.Size = new System.Drawing.Size(120, 30);
		this.buttonOk.TabIndex = 6;
		this.buttonOk.Text = "OK";
		this.buttonOk.UseVisualStyleBackColor = true;
		this.buttonOk.Click += new System.EventHandler(buttonOk_Click);
		this.buttonCancel.Location = new System.Drawing.Point(152, 243);
		this.buttonCancel.Name = "buttonCancel";
		this.buttonCancel.Size = new System.Drawing.Size(120, 30);
		this.buttonCancel.TabIndex = 7;
		this.buttonCancel.Text = "Cancel";
		this.buttonCancel.UseVisualStyleBackColor = true;
		this.buttonCancel.Click += new System.EventHandler(buttonCancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(284, 285);
		base.Controls.Add(this.buttonCancel);
		base.Controls.Add(this.buttonOk);
		base.Controls.Add(this.textBoxArgs);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.textBoxPath);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.textBoxName);
		base.Controls.Add(this.label1);
		base.Name = "FormTaskShedulerSet";
		this.Text = "Set Task";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
