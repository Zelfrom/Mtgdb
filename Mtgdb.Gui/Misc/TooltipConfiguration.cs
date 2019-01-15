using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Mtgdb.Controls;
using Ninject;

namespace Mtgdb.Gui
{
	public class TooltipConfiguration
	{
		private readonly TooltipForm _defaultTooltip;
		private readonly TooltipForm _quickFilterTooltip;
		private readonly TooltipController _quickFilterTooltipController;

		public TooltipConfiguration(
			[Named(GuiModule.DefaultTooltipScope)] TooltipForm defaultTooltip,
			[Named(GuiModule.QuickFilterTooltipScope)] TooltipForm quickFilterTooltip,
			[Named(GuiModule.QuickFilterTooltipScope)] TooltipController quickFilterTooltipController)
		{
			_defaultTooltip = defaultTooltip;
			_quickFilterTooltip = quickFilterTooltip;
			_quickFilterTooltipController = quickFilterTooltipController;
		}

		public void Setup()
		{
			_quickFilterTooltip.BackColor = SystemColors.Window;
			_quickFilterTooltip.TooltipBorderStyle = DashStyle.Solid;
			_quickFilterTooltip.TextPadding = new Padding(1, 1, 1, 1);
			_quickFilterTooltip.VisibleBorders = AnchorStyles.None;
			_quickFilterTooltip.TooltipMargin = 0;

			_quickFilterTooltip.ScaleDpi();
			_defaultTooltip.ScaleDpi();

			_quickFilterTooltipController.DelayMs = 0;
			_quickFilterTooltipController.ToggleOnAlt = false;
		}
	}
}