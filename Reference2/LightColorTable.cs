using System.Drawing;
using System.Windows.Forms;

internal class LightColorTable : ProfessionalColorTable
{
	public override Color ToolStripDropDownBackground => Color.White;

	public override Color ImageMarginGradientBegin => Color.FromArgb(245, 245, 245);

	public override Color ImageMarginGradientMiddle => Color.FromArgb(245, 245, 245);

	public override Color ImageMarginGradientEnd => Color.FromArgb(245, 245, 245);
}
