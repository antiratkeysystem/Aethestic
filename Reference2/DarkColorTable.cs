using System.Drawing;
using System.Windows.Forms;

internal class DarkColorTable : ProfessionalColorTable
{
	public override Color MenuItemSelected => Color.FromArgb(60, 60, 60);

	public override Color MenuItemBorder => Color.FromArgb(80, 80, 80);

	public override Color MenuBorder => Color.FromArgb(80, 80, 80);

	public override Color MenuItemSelectedGradientBegin => Color.FromArgb(60, 60, 60);

	public override Color MenuItemSelectedGradientEnd => Color.FromArgb(60, 60, 60);

	public override Color MenuItemPressedGradientBegin => Color.FromArgb(50, 50, 50);

	public override Color MenuItemPressedGradientEnd => Color.FromArgb(50, 50, 50);

	public override Color MenuStripGradientBegin => Color.FromArgb(40, 40, 40);

	public override Color MenuStripGradientEnd => Color.FromArgb(40, 40, 40);

	public override Color ToolStripDropDownBackground => Color.FromArgb(40, 40, 40);

	public override Color ImageMarginGradientBegin => Color.FromArgb(40, 40, 40);

	public override Color ImageMarginGradientMiddle => Color.FromArgb(40, 40, 40);

	public override Color ImageMarginGradientEnd => Color.FromArgb(40, 40, 40);

	public override Color SeparatorDark => Color.FromArgb(80, 80, 80);

	public override Color SeparatorLight => Color.FromArgb(60, 60, 60);
}
