using System.Drawing;
using System.Windows.Forms;

namespace Server.Helper;

public class CenteredFlagImageCell : DataGridViewImageCell
{
	protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
	{
		base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts & ~DataGridViewPaintParts.ContentForeground);
		if (value is Image img)
		{
			int padL = cellStyle.Padding.Left;
			int padT = cellStyle.Padding.Top;
			int contentW = cellBounds.Width - padL - cellStyle.Padding.Right;
			int contentH = cellBounds.Height - padT - cellStyle.Padding.Bottom;
			int x = cellBounds.X + padL + (contentW - img.Width) / 2;
			int y = cellBounds.Y + padT + (contentH - img.Height) / 2;
			graphics.DrawImage(img, x, y, img.Width, img.Height);
		}
	}
}
