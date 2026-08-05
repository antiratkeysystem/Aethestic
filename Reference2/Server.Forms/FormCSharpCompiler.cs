using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CustomControls.RJControls;
using Leb128;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormCSharpCompiler : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private Dictionary<string, string> projectFiles = new Dictionary<string, string>();

	private string currentFile;

	private bool suppressTextChanged;

	private const string ClassTemplate = "using System;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing System.Text;\r\n\r\nnamespace MyProject\r\n{{\r\n    public class {0}\r\n    {{\r\n        public {0}()\r\n        {{\r\n        }}\r\n    }}\r\n}}";

	private const string ProgramTemplate = "using System;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing System.Text;\r\n\r\nnamespace MyProject\r\n{\r\n    class Program\r\n    {\r\n        static void Main(string[] args)\r\n        {\r\n            Console.WriteLine(\"Hello World!\");\r\n            Console.ReadLine();\r\n        }\r\n    }\r\n}";

	private IContainer components;

	private Timer timer1;

	private TreeView treeViewFiles;

	public RichTextBox richTextBoxCode;

	public RichTextBox richTextBoxOutput;

	private RJButton btnCompile;

	private RJButton btnCompileRun;

	private RJButton btnSave;

	private RJButton btnClearOutput;

	private RJButton btnAddClass;

	private RJButton btnRemoveFile;

	private RJButton btnRenameFile;

	private RJComboBox cmbOutputType;

	private RJTextBox txtOutputName;

	private Label labelFiles;

	private Label labelCode;

	private Label labelOutput;

	public FormCSharpCompiler()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormCSharpCompiler_Load(object sender, EventArgs e)
	{
		timer1.Start();
		AddFile("Program.cs", "using System;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing System.Text;\r\n\r\nnamespace MyProject\r\n{\r\n    class Program\r\n    {\r\n        static void Main(string[] args)\r\n        {\r\n            Console.WriteLine(\"Hello World!\");\r\n            Console.ReadLine();\r\n        }\r\n    }\r\n}");
		SelectFile("Program.cs");
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

	private void AddFile(string fileName, string content)
	{
		if (projectFiles.ContainsKey(fileName))
		{
			MessageBox.Show("File '" + fileName + "' already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		projectFiles[fileName] = content;
		RefreshTreeView();
	}

	private void SelectFile(string fileName)
	{
		if (!projectFiles.ContainsKey(fileName))
		{
			return;
		}
		SaveCurrentFile();
		currentFile = fileName;
		suppressTextChanged = true;
		richTextBoxCode.Text = projectFiles[fileName];
		suppressTextChanged = false;
		labelCode.Text = "Code: " + fileName;
		ApplySyntaxHighlighting();
		foreach (TreeNode node in treeViewFiles.Nodes)
		{
			if (node.Text == fileName)
			{
				treeViewFiles.SelectedNode = node;
				break;
			}
		}
	}

	private void SaveCurrentFile()
	{
		if (currentFile != null && projectFiles.ContainsKey(currentFile))
		{
			projectFiles[currentFile] = richTextBoxCode.Text;
		}
	}

	private void RefreshTreeView()
	{
		treeViewFiles.Nodes.Clear();
		foreach (KeyValuePair<string, string> projectFile in projectFiles)
		{
			TreeNode node = new TreeNode(projectFile.Key);
			treeViewFiles.Nodes.Add(node);
		}
	}

	private void treeViewFiles_AfterSelect(object sender, TreeViewEventArgs e)
	{
		if (e.Node != null && projectFiles.ContainsKey(e.Node.Text))
		{
			SelectFile(e.Node.Text);
		}
	}

	private void btnAddClass_Click(object sender, EventArgs e)
	{
		string className = ShowInputDialog("Enter class name:", "Add New Class");
		if (!string.IsNullOrWhiteSpace(className))
		{
			className = className.Replace(".cs", "").Trim();
			if (!Regex.IsMatch(className, "^[a-zA-Z_][a-zA-Z0-9_]*$"))
			{
				MessageBox.Show("Invalid class name! Use only letters, numbers and underscore.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			string fileName = className + ".cs";
			string content = string.Format("using System;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing System.Text;\r\n\r\nnamespace MyProject\r\n{{\r\n    public class {0}\r\n    {{\r\n        public {0}()\r\n        {{\r\n        }}\r\n    }}\r\n}}", className);
			AddFile(fileName, content);
			SelectFile(fileName);
		}
	}

	private void btnRemoveFile_Click(object sender, EventArgs e)
	{
		if (treeViewFiles.SelectedNode == null)
		{
			MessageBox.Show("Select a file to remove!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		string fileName = treeViewFiles.SelectedNode.Text;
		if (projectFiles.Count <= 1)
		{
			MessageBox.Show("Cannot remove the last file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
		else
		{
			if (MessageBox.Show("Remove '" + fileName + "'?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			projectFiles.Remove(fileName);
			RefreshTreeView();
			if (!(currentFile == fileName))
			{
				return;
			}
			currentFile = null;
			using Dictionary<string, string>.Enumerator enumerator = projectFiles.GetEnumerator();
			if (enumerator.MoveNext())
			{
				SelectFile(enumerator.Current.Key);
			}
		}
	}

	private void btnRenameFile_Click(object sender, EventArgs e)
	{
		if (treeViewFiles.SelectedNode == null)
		{
			MessageBox.Show("Select a file to rename!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		string oldName = treeViewFiles.SelectedNode.Text;
		string newName = ShowInputDialog("Enter new file name:", "Rename File", oldName);
		if (string.IsNullOrWhiteSpace(newName) || newName == oldName)
		{
			return;
		}
		if (!newName.EndsWith(".cs"))
		{
			newName += ".cs";
		}
		if (projectFiles.ContainsKey(newName))
		{
			MessageBox.Show("File '" + newName + "' already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		string content = projectFiles[oldName];
		projectFiles.Remove(oldName);
		projectFiles[newName] = content;
		if (currentFile == oldName)
		{
			currentFile = newName;
		}
		RefreshTreeView();
		SelectFile(newName);
	}

	private void btnCompile_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			AppendOutput("Error: Client not connected!", Color.Red);
			return;
		}
		SaveCurrentFile();
		SendCompileCommand(executeInMemory: false);
	}

	private void btnCompileRun_Click(object sender, EventArgs e)
	{
		if (client == null || !client.itsConnect)
		{
			AppendOutput("Error: Client not connected!", Color.Red);
			return;
		}
		SaveCurrentFile();
		SendCompileCommand(executeInMemory: true);
	}

	private void btnSave_Click(object sender, EventArgs e)
	{
		SaveCurrentFile();
		AppendOutput("All files saved to memory.", Color.Green);
		SetStatus("Saved " + projectFiles.Count + " file(s)");
	}

	private void btnClearOutput_Click(object sender, EventArgs e)
	{
		richTextBoxOutput.Clear();
	}

	private void richTextBoxCode_TextChanged(object sender, EventArgs e)
	{
	}

	private void SendCompileCommand(bool executeInMemory)
	{
		string outputType = cmbOutputType.Texts ?? "exe";
		string outputName = txtOutputName.Texts.Trim();
		if (string.IsNullOrEmpty(outputName))
		{
			outputName = "output." + outputType;
		}
		if (!outputName.EndsWith("." + outputType))
		{
			outputName = outputName + "." + outputType;
		}
		AppendOutput("Sending compilation request...", Color.Blue);
		if (executeInMemory)
		{
			if (projectFiles.Count == 1)
			{
				foreach (KeyValuePair<string, string> file in projectFiles)
				{
					client.Send(new object[3] { "CSharpCompiler", "Execute", file.Value });
				}
				return;
			}
			List<object> filesList = new List<object>();
			foreach (KeyValuePair<string, string> file2 in projectFiles)
			{
				filesList.Add(new object[2] { file2.Key, file2.Value });
			}
			byte[] projectData = LEB128.Write(filesList.ToArray());
			client.Send(new object[5] { "CSharpCompiler", "CompileProject", projectData, outputType, outputName });
			return;
		}
		if (projectFiles.Count == 1)
		{
			foreach (KeyValuePair<string, string> file3 in projectFiles)
			{
				client.Send(new object[5] { "CSharpCompiler", "Compile", file3.Value, outputType, outputName });
			}
			return;
		}
		List<object> filesList2 = new List<object>();
		foreach (KeyValuePair<string, string> file4 in projectFiles)
		{
			filesList2.Add(new object[2] { file4.Key, file4.Value });
		}
		byte[] projectData2 = LEB128.Write(filesList2.ToArray());
		client.Send(new object[5] { "CSharpCompiler", "CompileProject", projectData2, outputType, outputName });
	}

	private void ApplySyntaxHighlighting()
	{
		if (richTextBoxCode.Text.Length <= 50000)
		{
			suppressTextChanged = true;
			int selStart = richTextBoxCode.SelectionStart;
			int selLength = richTextBoxCode.SelectionLength;
			richTextBoxCode.SelectAll();
			richTextBoxCode.SelectionColor = Color.Black;
			string[] array = new string[82]
			{
				"abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
				"class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum",
				"event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto",
				"if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace",
				"new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public",
				"readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string",
				"struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked",
				"unsafe", "ushort", "using", "var", "virtual", "void", "volatile", "while", "async", "await",
				"dynamic", "yield"
			};
			foreach (string keyword in array)
			{
				HighlightWord(keyword, Color.Blue);
			}
			HighlightPattern("\"[^\"\\\\]*(\\\\.[^\"\\\\]*)*\"", Color.FromArgb(163, 21, 21));
			HighlightPattern("//.*$", Color.Green);
			richTextBoxCode.SelectionStart = selStart;
			richTextBoxCode.SelectionLength = selLength;
			suppressTextChanged = false;
		}
	}

	private void HighlightWord(string word, Color color)
	{
		foreach (Match match in new Regex("\\b" + word + "\\b", RegexOptions.Multiline).Matches(richTextBoxCode.Text))
		{
			richTextBoxCode.Select(match.Index, match.Length);
			richTextBoxCode.SelectionColor = color;
		}
	}

	private void HighlightPattern(string pattern, Color color)
	{
		foreach (Match match in new Regex(pattern, RegexOptions.Multiline).Matches(richTextBoxCode.Text))
		{
			richTextBoxCode.Select(match.Index, match.Length);
			richTextBoxCode.SelectionColor = color;
		}
	}

	public void AppendOutput(string text, Color color)
	{
		if (base.InvokeRequired)
		{
			Invoke((MethodInvoker)delegate
			{
				AppendOutput(text, color);
			});
			return;
		}
		richTextBoxOutput.SelectionStart = richTextBoxOutput.TextLength;
		richTextBoxOutput.SelectionColor = color;
		richTextBoxOutput.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text + "\n");
		richTextBoxOutput.ScrollToCaret();
	}

	public void SetStatus(string status)
	{
		if (base.InvokeRequired)
		{
			Invoke((MethodInvoker)delegate
			{
				SetStatus(status);
			});
		}
	}

	private string ShowInputDialog(string prompt, string title, string defaultValue = "")
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
		textBox.Text = defaultValue;
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
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.treeViewFiles = new System.Windows.Forms.TreeView();
		this.richTextBoxCode = new System.Windows.Forms.RichTextBox();
		this.richTextBoxOutput = new System.Windows.Forms.RichTextBox();
		this.btnCompile = new CustomControls.RJControls.RJButton();
		this.btnCompileRun = new CustomControls.RJControls.RJButton();
		this.btnSave = new CustomControls.RJControls.RJButton();
		this.btnClearOutput = new CustomControls.RJControls.RJButton();
		this.btnAddClass = new CustomControls.RJControls.RJButton();
		this.btnRemoveFile = new CustomControls.RJControls.RJButton();
		this.btnRenameFile = new CustomControls.RJControls.RJButton();
		this.cmbOutputType = new CustomControls.RJControls.RJComboBox();
		this.txtOutputName = new CustomControls.RJControls.RJTextBox();
		this.labelFiles = new System.Windows.Forms.Label();
		this.labelCode = new System.Windows.Forms.Label();
		this.labelOutput = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.treeViewFiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.treeViewFiles.Font = new System.Drawing.Font("Consolas", 10f);
		this.treeViewFiles.FullRowSelect = true;
		this.treeViewFiles.HideSelection = false;
		this.treeViewFiles.ItemHeight = 22;
		this.treeViewFiles.Location = new System.Drawing.Point(772, 87);
		this.treeViewFiles.Name = "treeViewFiles";
		this.treeViewFiles.Size = new System.Drawing.Size(200, 340);
		this.treeViewFiles.TabIndex = 0;
		this.treeViewFiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(treeViewFiles_AfterSelect);
		this.richTextBoxCode.AcceptsTab = true;
		this.richTextBoxCode.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.richTextBoxCode.Font = new System.Drawing.Font("Consolas", 10f);
		this.richTextBoxCode.Location = new System.Drawing.Point(6, 87);
		this.richTextBoxCode.Name = "richTextBoxCode";
		this.richTextBoxCode.Size = new System.Drawing.Size(760, 312);
		this.richTextBoxCode.TabIndex = 4;
		this.richTextBoxCode.Text = "";
		this.richTextBoxCode.WordWrap = false;
		this.richTextBoxCode.TextChanged += new System.EventHandler(richTextBoxCode_TextChanged);
		this.richTextBoxOutput.BackColor = System.Drawing.Color.White;
		this.richTextBoxOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.richTextBoxOutput.Font = new System.Drawing.Font("Consolas", 9f);
		this.richTextBoxOutput.Location = new System.Drawing.Point(6, 424);
		this.richTextBoxOutput.Name = "richTextBoxOutput";
		this.richTextBoxOutput.ReadOnly = true;
		this.richTextBoxOutput.Size = new System.Drawing.Size(760, 85);
		this.richTextBoxOutput.TabIndex = 5;
		this.richTextBoxOutput.Text = "";
		this.btnCompile.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.btnCompile.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.btnCompile.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.btnCompile.BorderRadius = 0;
		this.btnCompile.BorderSize = 0;
		this.btnCompile.FlatAppearance.BorderSize = 0;
		this.btnCompile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCompile.Font = new System.Drawing.Font("Arial", 9.5f, System.Drawing.FontStyle.Bold);
		this.btnCompile.ForeColor = System.Drawing.Color.White;
		this.btnCompile.Location = new System.Drawing.Point(7, 515);
		this.btnCompile.Name = "btnCompile";
		this.btnCompile.Size = new System.Drawing.Size(110, 33);
		this.btnCompile.TabIndex = 6;
		this.btnCompile.Text = "COMPILE";
		this.btnCompile.TextColor = System.Drawing.Color.White;
		this.btnCompile.UseVisualStyleBackColor = false;
		this.btnCompile.Click += new System.EventHandler(btnCompile_Click);
		this.btnCompileRun.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.btnCompileRun.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.btnCompileRun.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.btnCompileRun.BorderRadius = 0;
		this.btnCompileRun.BorderSize = 0;
		this.btnCompileRun.FlatAppearance.BorderSize = 0;
		this.btnCompileRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnCompileRun.Font = new System.Drawing.Font("Arial", 9.5f, System.Drawing.FontStyle.Bold);
		this.btnCompileRun.ForeColor = System.Drawing.Color.White;
		this.btnCompileRun.Location = new System.Drawing.Point(123, 515);
		this.btnCompileRun.Name = "btnCompileRun";
		this.btnCompileRun.Size = new System.Drawing.Size(140, 33);
		this.btnCompileRun.TabIndex = 7;
		this.btnCompileRun.Text = "COMPILE && RUN";
		this.btnCompileRun.TextColor = System.Drawing.Color.White;
		this.btnCompileRun.UseVisualStyleBackColor = false;
		this.btnCompileRun.Click += new System.EventHandler(btnCompileRun_Click);
		this.btnSave.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.btnSave.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.btnSave.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.btnSave.BorderRadius = 0;
		this.btnSave.BorderSize = 0;
		this.btnSave.FlatAppearance.BorderSize = 0;
		this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnSave.Font = new System.Drawing.Font("Arial", 9.5f, System.Drawing.FontStyle.Bold);
		this.btnSave.ForeColor = System.Drawing.Color.White;
		this.btnSave.Location = new System.Drawing.Point(269, 515);
		this.btnSave.Name = "btnSave";
		this.btnSave.Size = new System.Drawing.Size(80, 33);
		this.btnSave.TabIndex = 8;
		this.btnSave.Text = "SAVE";
		this.btnSave.TextColor = System.Drawing.Color.White;
		this.btnSave.UseVisualStyleBackColor = false;
		this.btnSave.Click += new System.EventHandler(btnSave_Click);
		this.btnClearOutput.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.btnClearOutput.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.btnClearOutput.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.btnClearOutput.BorderRadius = 0;
		this.btnClearOutput.BorderSize = 0;
		this.btnClearOutput.FlatAppearance.BorderSize = 0;
		this.btnClearOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnClearOutput.Font = new System.Drawing.Font("Arial", 9.5f, System.Drawing.FontStyle.Bold);
		this.btnClearOutput.ForeColor = System.Drawing.Color.White;
		this.btnClearOutput.Location = new System.Drawing.Point(355, 515);
		this.btnClearOutput.Name = "btnClearOutput";
		this.btnClearOutput.Size = new System.Drawing.Size(90, 33);
		this.btnClearOutput.TabIndex = 9;
		this.btnClearOutput.Text = "CLEAR";
		this.btnClearOutput.TextColor = System.Drawing.Color.White;
		this.btnClearOutput.UseVisualStyleBackColor = false;
		this.btnClearOutput.Click += new System.EventHandler(btnClearOutput_Click);
		this.btnAddClass.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.btnAddClass.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.btnAddClass.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.btnAddClass.BorderRadius = 0;
		this.btnAddClass.BorderSize = 0;
		this.btnAddClass.FlatAppearance.BorderSize = 0;
		this.btnAddClass.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnAddClass.Font = new System.Drawing.Font("Arial", 9.5f, System.Drawing.FontStyle.Bold);
		this.btnAddClass.ForeColor = System.Drawing.Color.White;
		this.btnAddClass.Location = new System.Drawing.Point(772, 437);
		this.btnAddClass.Name = "btnAddClass";
		this.btnAddClass.Size = new System.Drawing.Size(200, 33);
		this.btnAddClass.TabIndex = 1;
		this.btnAddClass.Text = "ADD CLASS";
		this.btnAddClass.TextColor = System.Drawing.Color.White;
		this.btnAddClass.UseVisualStyleBackColor = false;
		this.btnAddClass.Click += new System.EventHandler(btnAddClass_Click);
		this.btnRemoveFile.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.btnRemoveFile.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.btnRemoveFile.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.btnRemoveFile.BorderRadius = 0;
		this.btnRemoveFile.BorderSize = 0;
		this.btnRemoveFile.FlatAppearance.BorderSize = 0;
		this.btnRemoveFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnRemoveFile.Font = new System.Drawing.Font("Arial", 9.5f, System.Drawing.FontStyle.Bold);
		this.btnRemoveFile.ForeColor = System.Drawing.Color.White;
		this.btnRemoveFile.Location = new System.Drawing.Point(772, 476);
		this.btnRemoveFile.Name = "btnRemoveFile";
		this.btnRemoveFile.Size = new System.Drawing.Size(98, 33);
		this.btnRemoveFile.TabIndex = 2;
		this.btnRemoveFile.Text = "REMOVE";
		this.btnRemoveFile.TextColor = System.Drawing.Color.White;
		this.btnRemoveFile.UseVisualStyleBackColor = false;
		this.btnRemoveFile.Click += new System.EventHandler(btnRemoveFile_Click);
		this.btnRenameFile.BackColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.btnRenameFile.BackgroundColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.btnRenameFile.BorderColor = System.Drawing.Color.PaleVioletRed;
		this.btnRenameFile.BorderRadius = 0;
		this.btnRenameFile.BorderSize = 0;
		this.btnRenameFile.FlatAppearance.BorderSize = 0;
		this.btnRenameFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.btnRenameFile.Font = new System.Drawing.Font("Arial", 9.5f, System.Drawing.FontStyle.Bold);
		this.btnRenameFile.ForeColor = System.Drawing.Color.White;
		this.btnRenameFile.Location = new System.Drawing.Point(874, 476);
		this.btnRenameFile.Name = "btnRenameFile";
		this.btnRenameFile.Size = new System.Drawing.Size(98, 33);
		this.btnRenameFile.TabIndex = 3;
		this.btnRenameFile.Text = "RENAME";
		this.btnRenameFile.TextColor = System.Drawing.Color.White;
		this.btnRenameFile.UseVisualStyleBackColor = false;
		this.btnRenameFile.Click += new System.EventHandler(btnRenameFile_Click);
		this.cmbOutputType.BackColor = System.Drawing.Color.White;
		this.cmbOutputType.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.cmbOutputType.BorderSize = 1;
		this.cmbOutputType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbOutputType.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.cmbOutputType.ForeColor = System.Drawing.Color.DimGray;
		this.cmbOutputType.IconColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.cmbOutputType.Items.AddRange(new object[2] { "exe", "dll" });
		this.cmbOutputType.ListBackColor = System.Drawing.Color.White;
		this.cmbOutputType.ListTextColor = System.Drawing.Color.DimGray;
		this.cmbOutputType.Location = new System.Drawing.Point(451, 515);
		this.cmbOutputType.MinimumSize = new System.Drawing.Size(80, 30);
		this.cmbOutputType.Name = "cmbOutputType";
		this.cmbOutputType.Padding = new System.Windows.Forms.Padding(1);
		this.cmbOutputType.Size = new System.Drawing.Size(315, 33);
		this.cmbOutputType.TabIndex = 10;
		this.cmbOutputType.Texts = "exe";
		this.txtOutputName.BackColor = System.Drawing.SystemColors.Window;
		this.txtOutputName.BorderColor = System.Drawing.Color.FromArgb(192, 0, 192);
		this.txtOutputName.BorderFocusColor = System.Drawing.Color.HotPink;
		this.txtOutputName.BorderRadius = 0;
		this.txtOutputName.BorderSize = 1;
		this.txtOutputName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5f);
		this.txtOutputName.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.txtOutputName.Location = new System.Drawing.Point(772, 517);
		this.txtOutputName.Margin = new System.Windows.Forms.Padding(4);
		this.txtOutputName.Multiline = false;
		this.txtOutputName.Name = "txtOutputName";
		this.txtOutputName.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
		this.txtOutputName.PasswordChar = false;
		this.txtOutputName.PlaceholderColor = System.Drawing.Color.DarkGray;
		this.txtOutputName.PlaceholderText = "output.exe";
		this.txtOutputName.Size = new System.Drawing.Size(200, 31);
		this.txtOutputName.TabIndex = 11;
		this.txtOutputName.Texts = "output.exe";
		this.txtOutputName.UnderlinedStyle = false;
		this.labelFiles.AutoSize = true;
		this.labelFiles.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.labelFiles.Location = new System.Drawing.Point(769, 67);
		this.labelFiles.Name = "labelFiles";
		this.labelFiles.Size = new System.Drawing.Size(77, 17);
		this.labelFiles.TabIndex = 20;
		this.labelFiles.Text = "Project Files";
		this.labelCode.AutoSize = true;
		this.labelCode.Font = new System.Drawing.Font("Segoe UI", 9.75f);
		this.labelCode.Location = new System.Drawing.Point(6, 67);
		this.labelCode.Name = "labelCode";
		this.labelCode.Size = new System.Drawing.Size(39, 17);
		this.labelCode.TabIndex = 21;
		this.labelCode.Text = "Code";
		this.labelOutput.AutoSize = true;
		this.labelOutput.Font = new System.Drawing.Font("Segoe UI", 9.75f);
		this.labelOutput.Location = new System.Drawing.Point(6, 402);
		this.labelOutput.Name = "labelOutput";
		this.labelOutput.Size = new System.Drawing.Size(48, 17);
		this.labelOutput.TabIndex = 22;
		this.labelOutput.Text = "Output";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(978, 554);
		base.Controls.Add(this.txtOutputName);
		base.Controls.Add(this.cmbOutputType);
		base.Controls.Add(this.btnClearOutput);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.btnCompileRun);
		base.Controls.Add(this.btnCompile);
		base.Controls.Add(this.richTextBoxOutput);
		base.Controls.Add(this.labelOutput);
		base.Controls.Add(this.richTextBoxCode);
		base.Controls.Add(this.labelCode);
		base.Controls.Add(this.btnRenameFile);
		base.Controls.Add(this.btnRemoveFile);
		base.Controls.Add(this.btnAddClass);
		base.Controls.Add(this.treeViewFiles);
		base.Controls.Add(this.labelFiles);
		base.Name = "FormCSharpCompiler";
		this.Text = "CSharp Compiler";
		base.Load += new System.EventHandler(FormCSharpCompiler_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
