using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Win32;
using Server.Connectings;
using Server.Helper;

namespace Server.Forms;

public class FormRegedit : FormMaterial
{
	public Clients client;

	public Clients parrent;

	private IContainer components;

	public System.Windows.Forms.Timer timer1;

	public ImageList imageList1;

	private ImageList imageRegistryKeyTypeList;

	private ImageList imageRegistryDirectoryList;

	public MaterialLabel materialLabel1;

	private Panel panel3;

	public RegistryTreeView tvRegistryDirectory;

	private ColumnHeader hName;

	private ColumnHeader hType;

	private ColumnHeader hValue;

	private ContextMenuStrip tv_ContextMenuStrip;

	private ToolStripMenuItem newToolStripMenuItem;

	private ToolStripMenuItem keyToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator2;

	private ToolStripMenuItem stringValueToolStripMenuItem;

	private ToolStripMenuItem binaryValueToolStripMenuItem;

	private ToolStripMenuItem dWORD32bitValueToolStripMenuItem;

	private ToolStripMenuItem qWORD64bitValueToolStripMenuItem;

	private ToolStripMenuItem multiStringValueToolStripMenuItem;

	private ToolStripMenuItem expandableStringValueToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ToolStripMenuItem renameToolStripMenuItem;

	private ContextMenuStrip selectedItem_ContextMenuStrip;

	private ToolStripMenuItem modifyToolStripMenuItem;

	private ToolStripMenuItem modifyBinaryDataToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem1;

	private ToolStripMenuItem renameToolStripMenuItem1;

	private ContextMenuStrip lst_ContextMenuStrip;

	private ToolStripMenuItem newToolStripMenuItem1;

	private ToolStripMenuItem keyToolStripMenuItem1;

	private ToolStripSeparator toolStripSeparator4;

	private ToolStripMenuItem stringValueToolStripMenuItem1;

	private ToolStripMenuItem binaryValueToolStripMenuItem1;

	private ToolStripMenuItem dWORD32bitValueToolStripMenuItem1;

	private ToolStripMenuItem qWORD64bitValueToolStripMenuItem1;

	private ToolStripMenuItem multiStringValueToolStripMenuItem1;

	private ToolStripMenuItem expandableStringValueToolStripMenuItem1;

	public AeroListView lstRegistryValues;

	public FormRegedit()
	{
		InitializeComponent();
		base.FormClosing += Closing1;
	}

	private void FormRegedit_Load(object sender, EventArgs e)
	{
		materialLabel1.Enabled = true;
		MaterialSkinManager.Instance.ThemeChanged += ChangeScheme;
		ChangeScheme(this);
		timer1.Start();
		tvRegistryDirectory.AfterLabelEdit += tvRegistryDirectory_AfterLabelEdit;
		tvRegistryDirectory.BeforeExpand += tvRegistryDirectory_BeforeExpand;
		tvRegistryDirectory.BeforeSelect += tvRegistryDirectory_BeforeSelect;
		tvRegistryDirectory.NodeMouseClick += tvRegistryDirectory_NodeMouseClick;
		tvRegistryDirectory.KeyUp += tvRegistryDirectory_KeyUp;
		typeof(RegistryTreeView).InvokeMember("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty, null, tvRegistryDirectory, new object[1] { true });
		typeof(AeroListView).InvokeMember("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty, null, lstRegistryValues, new object[1] { true });
	}

	private void ChangeScheme(object sender)
	{
		bool num = MaterialSkinManager.Instance.Theme == MaterialSkinManager.Themes.DARK;
		Color backColor = (num ? Color.FromArgb(40, 40, 40) : Color.White);
		Color foreColor = (num ? Color.WhiteSmoke : FormMaterial.PrimaryColor);
		tvRegistryDirectory.BackColor = backColor;
		tvRegistryDirectory.ForeColor = foreColor;
		lstRegistryValues.BackColor = backColor;
		lstRegistryValues.ForeColor = foreColor;
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
		if (!parrent.itsConnect)
		{
			Close();
		}
		if (client != null && !client.itsConnect)
		{
			Close();
		}
	}

	private void AddRootKey(RegistrySeeker.RegSeekerMatch match)
	{
		TreeNode treeNode = CreateNode(match.Key, match.Key, match.Data);
		treeNode.Nodes.Add(new TreeNode());
		tvRegistryDirectory.Nodes.Add(treeNode);
	}

	private TreeNode AddKeyToTree(TreeNode parent, RegistrySeeker.RegSeekerMatch subKey)
	{
		TreeNode treeNode = CreateNode(subKey.Key, subKey.Key, subKey.Data);
		parent.Nodes.Add(treeNode);
		if (subKey.HasSubKeys)
		{
			treeNode.Nodes.Add(new TreeNode());
		}
		return treeNode;
	}

	private TreeNode CreateNode(string key, string text, object tag)
	{
		return new TreeNode
		{
			Text = text,
			Name = key,
			Tag = tag
		};
	}

	public void AddKeys(string rootKey, RegistrySeeker.RegSeekerMatch[] matches)
	{
		materialLabel1.Visible = false;
		if (string.IsNullOrEmpty(rootKey))
		{
			tvRegistryDirectory.BeginUpdate();
			RegistrySeeker.RegSeekerMatch[] array = matches;
			foreach (RegistrySeeker.RegSeekerMatch match in array)
			{
				AddRootKey(match);
			}
			tvRegistryDirectory.SelectedNode = tvRegistryDirectory.Nodes[0];
			tvRegistryDirectory.EndUpdate();
			return;
		}
		TreeNode treeNode = GetTreeNode(rootKey);
		if (treeNode != null)
		{
			tvRegistryDirectory.BeginUpdate();
			RegistrySeeker.RegSeekerMatch[] array = matches;
			foreach (RegistrySeeker.RegSeekerMatch subKey in array)
			{
				AddKeyToTree(treeNode, subKey);
			}
			treeNode.Expand();
			tvRegistryDirectory.EndUpdate();
		}
	}

	public void CreateNewKey(string rootKey, RegistrySeeker.RegSeekerMatch match)
	{
		TreeNode treeNode = AddKeyToTree(GetTreeNode(rootKey), match);
		treeNode.EnsureVisible();
		tvRegistryDirectory.SelectedNode = treeNode;
		treeNode.Expand();
		tvRegistryDirectory.LabelEdit = true;
		treeNode.BeginEdit();
	}

	public void DeleteKey(string rootKey, string subKey)
	{
		TreeNode treeNode = GetTreeNode(rootKey);
		if (treeNode.Nodes.ContainsKey(subKey))
		{
			treeNode.Nodes.RemoveByKey(subKey);
		}
	}

	public void RenameKey(string rootKey, string oldName, string newName)
	{
		TreeNode treeNode = GetTreeNode(rootKey);
		if (treeNode.Nodes.ContainsKey(oldName))
		{
			treeNode.Nodes[oldName].Text = newName;
			treeNode.Nodes[oldName].Name = newName;
			tvRegistryDirectory.SelectedNode = treeNode.Nodes[newName];
		}
	}

	private TreeNode GetTreeNode(string path)
	{
		string[] array = path.Split('\\');
		TreeNode treeNode = tvRegistryDirectory.Nodes[array[0]];
		if (treeNode == null)
		{
			return null;
		}
		for (int i = 1; i < array.Length; i++)
		{
			treeNode = treeNode.Nodes[array[i]];
			if (treeNode == null)
			{
				return null;
			}
		}
		return treeNode;
	}

	public void CreateValue(string keyPath, RegistrySeeker.RegValueData value)
	{
		TreeNode treeNode = GetTreeNode(keyPath);
		if (treeNode != null)
		{
			List<RegistrySeeker.RegValueData> list = ((RegistrySeeker.RegValueData[])treeNode.Tag).ToList();
			list.Add(value);
			treeNode.Tag = list.ToArray();
			if (tvRegistryDirectory.SelectedNode == treeNode)
			{
				RegistryValueLstItem registryValueLstItem = new RegistryValueLstItem(value);
				lstRegistryValues.Items.Add(registryValueLstItem);
				lstRegistryValues.SelectedIndices.Clear();
				registryValueLstItem.Selected = true;
				lstRegistryValues.LabelEdit = true;
				registryValueLstItem.BeginEdit();
			}
			tvRegistryDirectory.SelectedNode = treeNode;
		}
	}

	public void DeleteValue(string keyPath, string valueName)
	{
		TreeNode treeNode = GetTreeNode(keyPath);
		if (treeNode == null)
		{
			return;
		}
		if (!RegValueHelper.IsDefaultValue(valueName))
		{
			treeNode.Tag = ((RegistrySeeker.RegValueData[])treeNode.Tag).Where((RegistrySeeker.RegValueData value) => value.Name != valueName).ToArray();
			if (tvRegistryDirectory.SelectedNode == treeNode)
			{
				lstRegistryValues.Items.RemoveByKey(valueName);
			}
		}
		else
		{
			RegistrySeeker.RegValueData regValueData = ((RegistrySeeker.RegValueData[])treeNode.Tag).First((RegistrySeeker.RegValueData item) => item.Name == valueName);
			if (tvRegistryDirectory.SelectedNode == treeNode)
			{
				RegistryValueLstItem registryValueLstItem = lstRegistryValues.Items.Cast<RegistryValueLstItem>().SingleOrDefault((RegistryValueLstItem item) => item.Name == valueName);
				if (registryValueLstItem != null)
				{
					registryValueLstItem.Data = regValueData.Kind.RegistryTypeToString(null);
				}
			}
		}
		tvRegistryDirectory.SelectedNode = treeNode;
	}

	public void RenameValue(string keyPath, string oldName, string newName)
	{
		TreeNode treeNode = GetTreeNode(keyPath);
		if (treeNode == null)
		{
			return;
		}
		((RegistrySeeker.RegValueData[])treeNode.Tag).First((RegistrySeeker.RegValueData item) => item.Name == oldName).Name = newName;
		if (tvRegistryDirectory.SelectedNode == treeNode)
		{
			RegistryValueLstItem registryValueLstItem = lstRegistryValues.Items.Cast<RegistryValueLstItem>().SingleOrDefault((RegistryValueLstItem item) => item.Name == oldName);
			if (registryValueLstItem != null)
			{
				registryValueLstItem.RegName = newName;
			}
		}
		tvRegistryDirectory.SelectedNode = treeNode;
	}

	public void ChangeValue(string keyPath, RegistrySeeker.RegValueData value)
	{
		TreeNode treeNode = GetTreeNode(keyPath);
		if (treeNode == null)
		{
			return;
		}
		RegistrySeeker.RegValueData dest = ((RegistrySeeker.RegValueData[])treeNode.Tag).First((RegistrySeeker.RegValueData item) => item.Name == value.Name);
		ChangeRegistryValue(value, dest);
		if (tvRegistryDirectory.SelectedNode == treeNode)
		{
			RegistryValueLstItem registryValueLstItem = lstRegistryValues.Items.Cast<RegistryValueLstItem>().SingleOrDefault((RegistryValueLstItem item) => item.Name == value.Name);
			if (registryValueLstItem != null)
			{
				registryValueLstItem.Data = RegValueHelper.RegistryValueToString(value);
			}
		}
		tvRegistryDirectory.SelectedNode = treeNode;
	}

	private void ChangeRegistryValue(RegistrySeeker.RegValueData source, RegistrySeeker.RegValueData dest)
	{
		if (source.Kind == dest.Kind)
		{
			dest.Data = source.Data;
		}
	}

	private void tvRegistryDirectory_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
	{
		if (e.Label == null)
		{
			tvRegistryDirectory.LabelEdit = false;
			return;
		}
		e.CancelEdit = true;
		if (e.Label.Length <= 0)
		{
			MessageBox.Show("Invalid label. \nThe label cannot be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			e.Node.BeginEdit();
			return;
		}
		if (e.Node.Parent.Nodes.ContainsKey(e.Label))
		{
			MessageBox.Show("Invalid label. \nA node with that label already exists.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			e.Node.BeginEdit();
			return;
		}
		client.Send(new object[4]
		{
			"RenameRegistryKey",
			e.Node.Name,
			e.Label,
			e.Node.Parent.FullPath
		});
		tvRegistryDirectory.LabelEdit = false;
	}

	private void tvRegistryDirectory_BeforeExpand(object sender, TreeViewCancelEventArgs e)
	{
		TreeNode node = e.Node;
		if (string.IsNullOrEmpty(node.FirstNode.Name))
		{
			tvRegistryDirectory.SuspendLayout();
			node.Nodes.Clear();
			client.Send(new object[2]
			{
				"LoadRegistryKey",
				e.Node.FullPath
			});
			Thread.Sleep(500);
			tvRegistryDirectory.ResumeLayout();
			e.Cancel = true;
		}
	}

	private void tvRegistryDirectory_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
	{
		if (e.Button == MouseButtons.Right)
		{
			tvRegistryDirectory.SelectedNode = e.Node;
			Point position = new Point(e.X, e.Y);
			CreateTreeViewMenuStrip();
			tv_ContextMenuStrip.Show(tvRegistryDirectory, position);
		}
	}

	private void tvRegistryDirectory_BeforeSelect(object sender, TreeViewCancelEventArgs e)
	{
		UpdateLstRegistryValues(e.Node);
	}

	private void UpdateLstRegistryValues(TreeNode node)
	{
		materialLabel1.Text = "Update Values: " + node.FullPath;
		PopulateLstRegistryValues((RegistrySeeker.RegValueData[])node.Tag);
	}

	private void tvRegistryDirectory_KeyUp(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Delete && GetDeleteState())
		{
			deleteRegistryKey_Click(this, e);
		}
	}

	private void PopulateLstRegistryValues(RegistrySeeker.RegValueData[] values)
	{
		lstRegistryValues.BeginUpdate();
		lstRegistryValues.Items.Clear();
		values = values.OrderBy((RegistrySeeker.RegValueData regValueData) => regValueData.Name).ToArray();
		RegistrySeeker.RegValueData[] array = values;
		foreach (RegistrySeeker.RegValueData value2 in array)
		{
			lstRegistryValues.Items.Add(new RegistryValueLstItem(value2));
		}
		lstRegistryValues.EndUpdate();
	}

	private void CreateTreeViewMenuStrip()
	{
		renameToolStripMenuItem.Enabled = tvRegistryDirectory.SelectedNode.Parent != null;
		deleteToolStripMenuItem.Enabled = tvRegistryDirectory.SelectedNode.Parent != null;
	}

	private void CreateListViewMenuStrip()
	{
		ToolStripMenuItem toolStripMenuItem = modifyToolStripMenuItem;
		bool enabled = (modifyBinaryDataToolStripMenuItem.Enabled = lstRegistryValues.SelectedItems.Count == 1);
		toolStripMenuItem.Enabled = enabled;
		renameToolStripMenuItem1.Enabled = lstRegistryValues.SelectedItems.Count == 1 && !RegValueHelper.IsDefaultValue(lstRegistryValues.SelectedItems[0].Name);
		deleteToolStripMenuItem1.Enabled = tvRegistryDirectory.SelectedNode != null && lstRegistryValues.SelectedItems.Count > 0;
	}

	private void menuStripExit_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void menuStripDelete_Click(object sender, EventArgs e)
	{
		if (tvRegistryDirectory.Focused)
		{
			deleteRegistryKey_Click(this, e);
		}
		else if (lstRegistryValues.Focused)
		{
			deleteRegistryValue_Click(this, e);
		}
	}

	private void menuStripRename_Click(object sender, EventArgs e)
	{
		if (tvRegistryDirectory.Focused)
		{
			renameRegistryKey_Click(this, e);
		}
		else if (lstRegistryValues.Focused)
		{
			renameRegistryValue_Click(this, e);
		}
	}

	private void lstRegistryKeys_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right)
		{
			Point position = new Point(e.X, e.Y);
			if (lstRegistryValues.GetItemAt(position.X, position.Y) == null)
			{
				lst_ContextMenuStrip.Show(lstRegistryValues, position);
				return;
			}
			CreateListViewMenuStrip();
			selectedItem_ContextMenuStrip.Show(lstRegistryValues, position);
		}
	}

	private void lstRegistryKeys_AfterLabelEdit(object sender, LabelEditEventArgs e)
	{
		if (e.Label == null || tvRegistryDirectory.SelectedNode == null)
		{
			lstRegistryValues.LabelEdit = false;
			return;
		}
		e.CancelEdit = true;
		int item = e.Item;
		if (e.Label.Length <= 0)
		{
			MessageBox.Show("Invalid label. \nThe label cannot be blank.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			lstRegistryValues.Items[item].BeginEdit();
			return;
		}
		if (lstRegistryValues.Items.ContainsKey(e.Label))
		{
			MessageBox.Show("Invalid label. \nA node with that label already exists.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			lstRegistryValues.Items[item].BeginEdit();
			return;
		}
		client.Send(new object[4]
		{
			"RenameRegistryValue",
			lstRegistryValues.Items[item].Name,
			e.Label,
			tvRegistryDirectory.SelectedNode.FullPath
		});
		lstRegistryValues.LabelEdit = false;
	}

	private void lstRegistryKeys_KeyUp(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Delete && GetDeleteState())
		{
			deleteRegistryValue_Click(this, e);
		}
	}

	private void createNewRegistryKey_Click(object sender, EventArgs e)
	{
		if (!tvRegistryDirectory.SelectedNode.IsExpanded && tvRegistryDirectory.SelectedNode.Nodes.Count > 0)
		{
			tvRegistryDirectory.AfterExpand += createRegistryKey_AfterExpand;
			tvRegistryDirectory.SelectedNode.Expand();
		}
		else
		{
			client.Send(new object[2]
			{
				"CreateRegistryKey",
				tvRegistryDirectory.SelectedNode.FullPath
			});
		}
	}

	private void deleteRegistryKey_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show("Are you sure you want to permanently delete this key and all of its subkeys?", "Confirm Key Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
		{
			string fullPath = tvRegistryDirectory.SelectedNode.Parent.FullPath;
			client.Send(new object[3]
			{
				"DeleteRegistryKey",
				tvRegistryDirectory.SelectedNode.Name,
				fullPath
			});
		}
	}

	private void renameRegistryKey_Click(object sender, EventArgs e)
	{
		tvRegistryDirectory.LabelEdit = true;
		tvRegistryDirectory.SelectedNode.BeginEdit();
	}

	private void createStringRegistryValue_Click(object sender, EventArgs e)
	{
		if (tvRegistryDirectory.SelectedNode != null)
		{
			client.Send(new object[3]
			{
				"CreateRegistryValue",
				tvRegistryDirectory.SelectedNode.FullPath,
				"1"
			});
		}
	}

	private void createBinaryRegistryValue_Click(object sender, EventArgs e)
	{
		if (tvRegistryDirectory.SelectedNode != null)
		{
			client.Send(new object[3]
			{
				"CreateRegistryValue",
				tvRegistryDirectory.SelectedNode.FullPath,
				"3"
			});
		}
	}

	private void createDwordRegistryValue_Click(object sender, EventArgs e)
	{
		if (tvRegistryDirectory.SelectedNode != null)
		{
			client.Send(new object[3]
			{
				"CreateRegistryValue",
				tvRegistryDirectory.SelectedNode.FullPath,
				"4"
			});
		}
	}

	private void createQwordRegistryValue_Click(object sender, EventArgs e)
	{
		if (tvRegistryDirectory.SelectedNode != null)
		{
			client.Send(new object[3]
			{
				"CreateRegistryValue",
				tvRegistryDirectory.SelectedNode.FullPath,
				"11"
			});
		}
	}

	private void createMultiStringRegistryValue_Click(object sender, EventArgs e)
	{
		if (tvRegistryDirectory.SelectedNode != null)
		{
			client.Send(new object[3]
			{
				"CreateRegistryValue",
				tvRegistryDirectory.SelectedNode.FullPath,
				"7"
			});
		}
	}

	private void createExpandStringRegistryValue_Click(object sender, EventArgs e)
	{
		if (tvRegistryDirectory.SelectedNode != null)
		{
			client.Send(new object[3]
			{
				"CreateRegistryValue",
				tvRegistryDirectory.SelectedNode.FullPath,
				"2"
			});
		}
	}

	private void deleteRegistryValue_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show("Deleting certain registry values could cause system instability. Are you sure you want to permanently delete " + ((lstRegistryValues.SelectedItems.Count == 1) ? "this value?" : "these values?"), "Confirm Value Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
		{
			return;
		}
		foreach (object obj in lstRegistryValues.SelectedItems)
		{
			if (obj.GetType() == typeof(RegistryValueLstItem))
			{
				RegistryValueLstItem registryValueLstItem = (RegistryValueLstItem)obj;
				client.Send(new object[3]
				{
					"DeleteRegistryValue",
					tvRegistryDirectory.SelectedNode.FullPath,
					registryValueLstItem.RegName
				});
			}
		}
	}

	private void renameRegistryValue_Click(object sender, EventArgs e)
	{
		lstRegistryValues.LabelEdit = true;
		lstRegistryValues.SelectedItems[0].BeginEdit();
	}

	private void modifyRegistryValue_Click(object sender, EventArgs e)
	{
		CreateEditForm(isBinary: false);
	}

	private void modifyBinaryDataRegistryValue_Click(object sender, EventArgs e)
	{
		CreateEditForm(isBinary: true);
	}

	private void createRegistryKey_AfterExpand(object sender, TreeViewEventArgs e)
	{
		if (e.Node == tvRegistryDirectory.SelectedNode)
		{
			createNewRegistryKey_Click(this, e);
			tvRegistryDirectory.AfterExpand -= createRegistryKey_AfterExpand;
		}
	}

	private bool GetDeleteState()
	{
		if (lstRegistryValues.Focused)
		{
			return lstRegistryValues.SelectedItems.Count > 0;
		}
		if (tvRegistryDirectory.Focused && tvRegistryDirectory.SelectedNode != null)
		{
			return tvRegistryDirectory.SelectedNode.Parent != null;
		}
		return false;
	}

	private bool GetRenameState()
	{
		if (!lstRegistryValues.Focused)
		{
			if (tvRegistryDirectory.Focused && tvRegistryDirectory.SelectedNode != null)
			{
				return tvRegistryDirectory.SelectedNode.Parent != null;
			}
			return false;
		}
		if (lstRegistryValues.SelectedItems.Count == 1)
		{
			return !RegValueHelper.IsDefaultValue(lstRegistryValues.SelectedItems[0].Name);
		}
		return false;
	}

	private Form GetEditForm(RegistrySeeker.RegValueData value, RegistryValueKind valueKind)
	{
		switch (valueKind)
		{
		case RegistryValueKind.String:
		case RegistryValueKind.ExpandString:
			return new FormRegisterValueEditString(value);
		case RegistryValueKind.Binary:
			return new FormRegisterValueEditBinary(value);
		case RegistryValueKind.DWord:
		case RegistryValueKind.QWord:
			return new FormRegisterValueEditWord(value);
		case RegistryValueKind.MultiString:
			return new FormRegisterValueEditMultiString(value);
		default:
			return null;
		}
	}

	private void CreateEditForm(bool isBinary)
	{
		_ = tvRegistryDirectory.SelectedNode.FullPath;
		string name = lstRegistryValues.SelectedItems[0].Name;
		RegistrySeeker.RegValueData regValueData = ((RegistrySeeker.RegValueData[])tvRegistryDirectory.SelectedNode.Tag).ToList().Find((RegistrySeeker.RegValueData item) => item.Name == name);
		RegistryValueKind valueKind = (isBinary ? RegistryValueKind.Binary : regValueData.Kind);
		using Form editForm = GetEditForm(regValueData, valueKind);
		if (editForm.ShowDialog() == DialogResult.OK)
		{
			client.Send(new object[1] { "ChangeRegistryValue" });
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
		System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(Server.Forms.FormRegedit));
		this.timer1 = new System.Windows.Forms.Timer(this.components);
		this.imageList1 = new System.Windows.Forms.ImageList(this.components);
		this.imageRegistryKeyTypeList = new System.Windows.Forms.ImageList(this.components);
		this.imageRegistryDirectoryList = new System.Windows.Forms.ImageList(this.components);
		this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
		this.panel3 = new System.Windows.Forms.Panel();
		this.tvRegistryDirectory = new Server.Helper.RegistryTreeView();
		this.lstRegistryValues = new Server.Helper.AeroListView();
		this.hName = new System.Windows.Forms.ColumnHeader();
		this.hType = new System.Windows.Forms.ColumnHeader();
		this.hValue = new System.Windows.Forms.ColumnHeader();
		this.tv_ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.keyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.stringValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.binaryValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.dWORD32bitValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.qWORD64bitValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.multiStringValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.expandableStringValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.selectedItem_ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.modifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.modifyBinaryDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.renameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.lst_ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.newToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.keyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
		this.stringValueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.binaryValueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.dWORD32bitValueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.qWORD64bitValueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.multiStringValueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.expandableStringValueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.panel3.SuspendLayout();
		this.tv_ContextMenuStrip.SuspendLayout();
		this.selectedItem_ContextMenuStrip.SuspendLayout();
		this.lst_ContextMenuStrip.SuspendLayout();
		base.SuspendLayout();
		this.timer1.Interval = 1000;
		this.timer1.Tick += new System.EventHandler(timer1_Tick);
		this.imageList1.ImageStream = (System.Windows.Forms.ImageListStreamer)componentResourceManager.GetObject("imageList1.ImageStream");
		this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
		this.imageList1.Images.SetKeyName(0, "regedit_1983.png");
		this.imageList1.Images.SetKeyName(1, "folder.png");
		this.imageRegistryKeyTypeList.ImageStream = (System.Windows.Forms.ImageListStreamer)componentResourceManager.GetObject("imageRegistryKeyTypeList.ImageStream");
		this.imageRegistryKeyTypeList.TransparentColor = System.Drawing.Color.Transparent;
		this.imageRegistryKeyTypeList.Images.SetKeyName(0, "reg_string.png");
		this.imageRegistryKeyTypeList.Images.SetKeyName(1, "reg_binary.png");
		this.imageRegistryDirectoryList.ImageStream = (System.Windows.Forms.ImageListStreamer)componentResourceManager.GetObject("imageRegistryDirectoryList.ImageStream");
		this.imageRegistryDirectoryList.TransparentColor = System.Drawing.Color.Transparent;
		this.imageRegistryDirectoryList.Images.SetKeyName(0, "folder.png");
		this.materialLabel1.AutoSize = true;
		this.materialLabel1.Depth = 0;
		this.materialLabel1.Enabled = false;
		this.materialLabel1.Font = new System.Drawing.Font("Roboto", 14f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.materialLabel1.Location = new System.Drawing.Point(6, 3);
		this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
		this.materialLabel1.Name = "materialLabel1";
		this.materialLabel1.Size = new System.Drawing.Size(94, 19);
		this.materialLabel1.TabIndex = 0;
		this.materialLabel1.Text = "Please wait...";
		this.panel3.Controls.Add(this.materialLabel1);
		this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel3.Location = new System.Drawing.Point(3, 548);
		this.panel3.Name = "panel3";
		this.panel3.Size = new System.Drawing.Size(867, 23);
		this.panel3.TabIndex = 18;
		this.tvRegistryDirectory.BackColor = System.Drawing.Color.White;
		this.tvRegistryDirectory.Dock = System.Windows.Forms.DockStyle.Left;
		this.tvRegistryDirectory.Enabled = false;
		this.tvRegistryDirectory.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.tvRegistryDirectory.HideSelection = false;
		this.tvRegistryDirectory.ImageIndex = 0;
		this.tvRegistryDirectory.ImageList = this.imageRegistryDirectoryList;
		this.tvRegistryDirectory.Location = new System.Drawing.Point(3, 64);
		this.tvRegistryDirectory.Name = "tvRegistryDirectory";
		this.tvRegistryDirectory.SelectedImageIndex = 0;
		this.tvRegistryDirectory.Size = new System.Drawing.Size(257, 484);
		this.tvRegistryDirectory.TabIndex = 19;
		this.lstRegistryValues.BackColor = System.Drawing.Color.White;
		this.lstRegistryValues.Columns.AddRange(new System.Windows.Forms.ColumnHeader[3] { this.hName, this.hType, this.hValue });
		this.lstRegistryValues.Dock = System.Windows.Forms.DockStyle.Fill;
		this.lstRegistryValues.Enabled = false;
		this.lstRegistryValues.ForeColor = System.Drawing.Color.MediumSlateBlue;
		this.lstRegistryValues.FullRowSelect = true;
		this.lstRegistryValues.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
		this.lstRegistryValues.HideSelection = false;
		this.lstRegistryValues.Location = new System.Drawing.Point(260, 64);
		this.lstRegistryValues.Name = "lstRegistryValues";
		this.lstRegistryValues.Size = new System.Drawing.Size(610, 484);
		this.lstRegistryValues.SmallImageList = this.imageRegistryKeyTypeList;
		this.lstRegistryValues.TabIndex = 20;
		this.lstRegistryValues.UseCompatibleStateImageBehavior = false;
		this.lstRegistryValues.View = System.Windows.Forms.View.Details;
		this.lstRegistryValues.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(lstRegistryKeys_AfterLabelEdit);
		this.lstRegistryValues.KeyUp += new System.Windows.Forms.KeyEventHandler(lstRegistryKeys_KeyUp);
		this.lstRegistryValues.MouseUp += new System.Windows.Forms.MouseEventHandler(lstRegistryKeys_MouseClick);
		this.hName.Text = "Name";
		this.hName.Width = 173;
		this.hType.Text = "Type";
		this.hType.Width = 104;
		this.hValue.Text = "Value";
		this.hValue.Width = 214;
		this.tv_ContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.tv_ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.newToolStripMenuItem, this.deleteToolStripMenuItem, this.renameToolStripMenuItem });
		this.tv_ContextMenuStrip.Name = "contextMenuStrip";
		this.tv_ContextMenuStrip.Size = new System.Drawing.Size(185, 104);
		this.newToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.keyToolStripMenuItem, this.toolStripSeparator2, this.stringValueToolStripMenuItem, this.binaryValueToolStripMenuItem, this.dWORD32bitValueToolStripMenuItem, this.qWORD64bitValueToolStripMenuItem, this.multiStringValueToolStripMenuItem, this.expandableStringValueToolStripMenuItem });
		this.newToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.newToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("newToolStripMenuItem.Image");
		this.newToolStripMenuItem.Name = "newToolStripMenuItem";
		this.newToolStripMenuItem.Size = new System.Drawing.Size(184, 26);
		this.newToolStripMenuItem.Text = "New";
		this.keyToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.keyToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.keyToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("keyToolStripMenuItem.Image");
		this.keyToolStripMenuItem.Name = "keyToolStripMenuItem";
		this.keyToolStripMenuItem.Size = new System.Drawing.Size(204, 26);
		this.keyToolStripMenuItem.Text = "Key";
		this.keyToolStripMenuItem.Click += new System.EventHandler(createNewRegistryKey_Click);
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		this.toolStripSeparator2.Size = new System.Drawing.Size(197, 6);
		this.stringValueToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.stringValueToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.stringValueToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("stringValueToolStripMenuItem.Image");
		this.stringValueToolStripMenuItem.Name = "stringValueToolStripMenuItem";
		this.stringValueToolStripMenuItem.Size = new System.Drawing.Size(204, 26);
		this.stringValueToolStripMenuItem.Text = "String Value";
		this.stringValueToolStripMenuItem.Click += new System.EventHandler(createStringRegistryValue_Click);
		this.binaryValueToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.binaryValueToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.binaryValueToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("binaryValueToolStripMenuItem.Image");
		this.binaryValueToolStripMenuItem.Name = "binaryValueToolStripMenuItem";
		this.binaryValueToolStripMenuItem.Size = new System.Drawing.Size(204, 26);
		this.binaryValueToolStripMenuItem.Text = "Binary Value";
		this.binaryValueToolStripMenuItem.Click += new System.EventHandler(createBinaryRegistryValue_Click);
		this.dWORD32bitValueToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.dWORD32bitValueToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.dWORD32bitValueToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("dWORD32bitValueToolStripMenuItem.Image");
		this.dWORD32bitValueToolStripMenuItem.Name = "dWORD32bitValueToolStripMenuItem";
		this.dWORD32bitValueToolStripMenuItem.Size = new System.Drawing.Size(204, 26);
		this.dWORD32bitValueToolStripMenuItem.Text = "DWORD (32-bit) Value";
		this.dWORD32bitValueToolStripMenuItem.Click += new System.EventHandler(createDwordRegistryValue_Click);
		this.qWORD64bitValueToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.qWORD64bitValueToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.qWORD64bitValueToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("qWORD64bitValueToolStripMenuItem.Image");
		this.qWORD64bitValueToolStripMenuItem.Name = "qWORD64bitValueToolStripMenuItem";
		this.qWORD64bitValueToolStripMenuItem.Size = new System.Drawing.Size(204, 26);
		this.qWORD64bitValueToolStripMenuItem.Text = "QWORD (64-bit) Value";
		this.qWORD64bitValueToolStripMenuItem.Click += new System.EventHandler(createQwordRegistryValue_Click);
		this.multiStringValueToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.multiStringValueToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.multiStringValueToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("multiStringValueToolStripMenuItem.Image");
		this.multiStringValueToolStripMenuItem.Name = "multiStringValueToolStripMenuItem";
		this.multiStringValueToolStripMenuItem.Size = new System.Drawing.Size(204, 26);
		this.multiStringValueToolStripMenuItem.Text = "Multi-String Value";
		this.multiStringValueToolStripMenuItem.Click += new System.EventHandler(createMultiStringRegistryValue_Click);
		this.expandableStringValueToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.expandableStringValueToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.expandableStringValueToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("expandableStringValueToolStripMenuItem.Image");
		this.expandableStringValueToolStripMenuItem.Name = "expandableStringValueToolStripMenuItem";
		this.expandableStringValueToolStripMenuItem.Size = new System.Drawing.Size(204, 26);
		this.expandableStringValueToolStripMenuItem.Text = "Expandable String Value";
		this.expandableStringValueToolStripMenuItem.Click += new System.EventHandler(createExpandStringRegistryValue_Click);
		this.deleteToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.deleteToolStripMenuItem.Enabled = false;
		this.deleteToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.deleteToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("deleteToolStripMenuItem.Image");
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(184, 26);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(renameRegistryKey_Click);
		this.renameToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.renameToolStripMenuItem.Enabled = false;
		this.renameToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.renameToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("renameToolStripMenuItem.Image");
		this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
		this.renameToolStripMenuItem.Size = new System.Drawing.Size(184, 26);
		this.renameToolStripMenuItem.Text = "Rename";
		this.renameToolStripMenuItem.Click += new System.EventHandler(renameRegistryValue_Click);
		this.selectedItem_ContextMenuStrip.BackColor = System.Drawing.Color.White;
		this.selectedItem_ContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.selectedItem_ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.modifyToolStripMenuItem, this.modifyBinaryDataToolStripMenuItem, this.deleteToolStripMenuItem1, this.renameToolStripMenuItem1 });
		this.selectedItem_ContextMenuStrip.Name = "selectedItem_ContextMenuStrip";
		this.selectedItem_ContextMenuStrip.Size = new System.Drawing.Size(189, 108);
		this.modifyToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.modifyToolStripMenuItem.Enabled = false;
		this.modifyToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.modifyToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.modifyToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("modifyToolStripMenuItem.Image");
		this.modifyToolStripMenuItem.Name = "modifyToolStripMenuItem";
		this.modifyToolStripMenuItem.Size = new System.Drawing.Size(188, 26);
		this.modifyToolStripMenuItem.Text = "Modify...";
		this.modifyToolStripMenuItem.Click += new System.EventHandler(modifyRegistryValue_Click);
		this.modifyBinaryDataToolStripMenuItem.BackColor = System.Drawing.Color.White;
		this.modifyBinaryDataToolStripMenuItem.Enabled = false;
		this.modifyBinaryDataToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.modifyBinaryDataToolStripMenuItem.Image = (System.Drawing.Image)componentResourceManager.GetObject("modifyBinaryDataToolStripMenuItem.Image");
		this.modifyBinaryDataToolStripMenuItem.Name = "modifyBinaryDataToolStripMenuItem";
		this.modifyBinaryDataToolStripMenuItem.Size = new System.Drawing.Size(188, 26);
		this.modifyBinaryDataToolStripMenuItem.Text = "Modify Binary Data...";
		this.modifyBinaryDataToolStripMenuItem.Click += new System.EventHandler(modifyBinaryDataRegistryValue_Click);
		this.deleteToolStripMenuItem1.BackColor = System.Drawing.Color.White;
		this.deleteToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.deleteToolStripMenuItem1.Image = (System.Drawing.Image)componentResourceManager.GetObject("deleteToolStripMenuItem1.Image");
		this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
		this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(188, 26);
		this.deleteToolStripMenuItem1.Text = "Delete";
		this.deleteToolStripMenuItem1.Click += new System.EventHandler(deleteRegistryValue_Click);
		this.renameToolStripMenuItem1.BackColor = System.Drawing.Color.White;
		this.renameToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.renameToolStripMenuItem1.Image = (System.Drawing.Image)componentResourceManager.GetObject("renameToolStripMenuItem1.Image");
		this.renameToolStripMenuItem1.Name = "renameToolStripMenuItem1";
		this.renameToolStripMenuItem1.Size = new System.Drawing.Size(188, 26);
		this.renameToolStripMenuItem1.Text = "Rename";
		this.renameToolStripMenuItem1.Click += new System.EventHandler(renameRegistryValue_Click);
		this.lst_ContextMenuStrip.BackColor = System.Drawing.Color.White;
		this.lst_ContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.lst_ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.newToolStripMenuItem1 });
		this.lst_ContextMenuStrip.Name = "lst_ContextMenuStrip";
		this.lst_ContextMenuStrip.Size = new System.Drawing.Size(103, 30);
		this.newToolStripMenuItem1.BackColor = System.Drawing.Color.White;
		this.newToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.keyToolStripMenuItem1, this.toolStripSeparator4, this.stringValueToolStripMenuItem1, this.binaryValueToolStripMenuItem1, this.dWORD32bitValueToolStripMenuItem1, this.qWORD64bitValueToolStripMenuItem1, this.multiStringValueToolStripMenuItem1, this.expandableStringValueToolStripMenuItem1 });
		this.newToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.newToolStripMenuItem1.Image = (System.Drawing.Image)componentResourceManager.GetObject("newToolStripMenuItem1.Image");
		this.newToolStripMenuItem1.Name = "newToolStripMenuItem1";
		this.newToolStripMenuItem1.Size = new System.Drawing.Size(102, 26);
		this.newToolStripMenuItem1.Text = "New";
		this.keyToolStripMenuItem1.BackColor = System.Drawing.Color.White;
		this.keyToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.keyToolStripMenuItem1.Image = (System.Drawing.Image)componentResourceManager.GetObject("keyToolStripMenuItem1.Image");
		this.keyToolStripMenuItem1.Name = "keyToolStripMenuItem1";
		this.keyToolStripMenuItem1.Size = new System.Drawing.Size(204, 26);
		this.keyToolStripMenuItem1.Text = "Key";
		this.keyToolStripMenuItem1.Click += new System.EventHandler(createNewRegistryKey_Click);
		this.toolStripSeparator4.Name = "toolStripSeparator4";
		this.toolStripSeparator4.Size = new System.Drawing.Size(197, 6);
		this.stringValueToolStripMenuItem1.BackColor = System.Drawing.Color.White;
		this.stringValueToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.stringValueToolStripMenuItem1.Image = (System.Drawing.Image)componentResourceManager.GetObject("stringValueToolStripMenuItem1.Image");
		this.stringValueToolStripMenuItem1.Name = "stringValueToolStripMenuItem1";
		this.stringValueToolStripMenuItem1.Size = new System.Drawing.Size(204, 26);
		this.stringValueToolStripMenuItem1.Text = "String Value";
		this.stringValueToolStripMenuItem1.Click += new System.EventHandler(createStringRegistryValue_Click);
		this.binaryValueToolStripMenuItem1.BackColor = System.Drawing.Color.White;
		this.binaryValueToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.binaryValueToolStripMenuItem1.Image = (System.Drawing.Image)componentResourceManager.GetObject("binaryValueToolStripMenuItem1.Image");
		this.binaryValueToolStripMenuItem1.Name = "binaryValueToolStripMenuItem1";
		this.binaryValueToolStripMenuItem1.Size = new System.Drawing.Size(204, 26);
		this.binaryValueToolStripMenuItem1.Text = "Binary Value";
		this.binaryValueToolStripMenuItem1.Click += new System.EventHandler(createBinaryRegistryValue_Click);
		this.dWORD32bitValueToolStripMenuItem1.BackColor = System.Drawing.Color.White;
		this.dWORD32bitValueToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.dWORD32bitValueToolStripMenuItem1.Image = (System.Drawing.Image)componentResourceManager.GetObject("dWORD32bitValueToolStripMenuItem1.Image");
		this.dWORD32bitValueToolStripMenuItem1.Name = "dWORD32bitValueToolStripMenuItem1";
		this.dWORD32bitValueToolStripMenuItem1.Size = new System.Drawing.Size(204, 26);
		this.dWORD32bitValueToolStripMenuItem1.Text = "DWORD (32-bit) Value";
		this.dWORD32bitValueToolStripMenuItem1.Click += new System.EventHandler(createDwordRegistryValue_Click);
		this.qWORD64bitValueToolStripMenuItem1.BackColor = System.Drawing.Color.White;
		this.qWORD64bitValueToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.qWORD64bitValueToolStripMenuItem1.Image = (System.Drawing.Image)componentResourceManager.GetObject("qWORD64bitValueToolStripMenuItem1.Image");
		this.qWORD64bitValueToolStripMenuItem1.Name = "qWORD64bitValueToolStripMenuItem1";
		this.qWORD64bitValueToolStripMenuItem1.Size = new System.Drawing.Size(204, 26);
		this.qWORD64bitValueToolStripMenuItem1.Text = "QWORD (64-bit) Value";
		this.qWORD64bitValueToolStripMenuItem1.Click += new System.EventHandler(createQwordRegistryValue_Click);
		this.multiStringValueToolStripMenuItem1.BackColor = System.Drawing.Color.White;
		this.multiStringValueToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.multiStringValueToolStripMenuItem1.Image = (System.Drawing.Image)componentResourceManager.GetObject("multiStringValueToolStripMenuItem1.Image");
		this.multiStringValueToolStripMenuItem1.Name = "multiStringValueToolStripMenuItem1";
		this.multiStringValueToolStripMenuItem1.Size = new System.Drawing.Size(204, 26);
		this.multiStringValueToolStripMenuItem1.Text = "Multi-String Value";
		this.multiStringValueToolStripMenuItem1.Click += new System.EventHandler(createMultiStringRegistryValue_Click);
		this.expandableStringValueToolStripMenuItem1.BackColor = System.Drawing.Color.White;
		this.expandableStringValueToolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(0, 192, 192);
		this.expandableStringValueToolStripMenuItem1.Image = (System.Drawing.Image)componentResourceManager.GetObject("expandableStringValueToolStripMenuItem1.Image");
		this.expandableStringValueToolStripMenuItem1.Name = "expandableStringValueToolStripMenuItem1";
		this.expandableStringValueToolStripMenuItem1.Size = new System.Drawing.Size(204, 26);
		this.expandableStringValueToolStripMenuItem1.Text = "Expandable String Value";
		this.expandableStringValueToolStripMenuItem1.Click += new System.EventHandler(createExpandStringRegistryValue_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(873, 574);
		base.Controls.Add(this.lstRegistryValues);
		base.Controls.Add(this.tvRegistryDirectory);
		base.Controls.Add(this.panel3);
		base.Name = "FormRegedit";
		this.Text = "Regedit";
		base.Load += new System.EventHandler(FormRegedit_Load);
		this.panel3.ResumeLayout(false);
		this.panel3.PerformLayout();
		this.tv_ContextMenuStrip.ResumeLayout(false);
		this.selectedItem_ContextMenuStrip.ResumeLayout(false);
		this.lst_ContextMenuStrip.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
