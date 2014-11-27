using System.Windows;
using System.Windows.Media;
using GraphEditor.PropertiesClasses;

namespace GraphEditor.Graphics
{
	internal class GraphicsSelectionRectangle : GraphicsRectangleBase
	{
		public GraphicsSelectionRectangle(double left, double top, double right, double bottom, double actualScale)
		{
			rectangleLeft = left;
			rectangleTop = top;
			rectangleRight = right;
			rectangleBottom = bottom;
			graphicsLineWidth = 1.0;
			graphicsActualScale = actualScale;
		}

		public GraphicsSelectionRectangle()
			:
				this(0.0, 0.0, 100.0, 100.0, 1.0)
		{
		}

		/// <summary>
		///     Draw graphics object
		/// </summary>
		public override void Draw(DrawingContext drawingContext)
		{
			/*drawingContext.DrawRectangle(
                null,
                new Pen(Brushes.White, ActualLineWidth),
                Rectangle);*/

			var dashStyle = new DashStyle();
			dashStyle.Dashes.Add(5);

			var dashedPen = new Pen(new SolidColorBrush(Colors.Black), ActualLineWidth) {DashStyle = dashStyle};
			drawingContext.DrawRectangle(null, dashedPen, Rectangle);
		}

		public override bool Contains(Point point)
		{
			return Rectangle.Contains(point);
		}

		public override PropertiesGraphicsBase CreateSerializedObject()
		{
			return null; // not used
		}
	}
}