using System;
using System.Collections;
using System.Windows.Forms;

namespace Server;

public class CustomComparer : IComparer
{
	public int Compare(object x, object y)
	{
		DataGridViewRow dataGridViewRow = (DataGridViewRow)x;
		DataGridViewRow dataGridViewRow2 = (DataGridViewRow)y;
		int num = Convert.ToInt32(dataGridViewRow.Cells[0].Tag);
		int num2 = Convert.ToInt32(dataGridViewRow2.Cells[0].Tag);
		if (num == 1 && num2 == 0)
		{
			return -1;
		}
		if (num == 0 && num2 == 1)
		{
			return 1;
		}
		string text = dataGridViewRow.Cells[1].Value.ToString();
		string text2 = dataGridViewRow2.Cells[1].Value.ToString();
		if (text == "..." || text2 == "...")
		{
			return 0;
		}
		return string.Compare(text, text2);
	}
}
