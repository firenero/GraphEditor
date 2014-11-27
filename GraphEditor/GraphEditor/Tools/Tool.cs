using System.Windows.Input;

namespace GraphEditor.Tools
{
	internal abstract class Tool
	{
		public abstract void OnMouseDown(GraphCanvas drawingCanvas, MouseButtonEventArgs e);

		public abstract void OnMouseMove(GraphCanvas drawingCanvas, MouseEventArgs e);

		public abstract void OnMouseUp(GraphCanvas drawingCanvas, MouseButtonEventArgs e);

		public abstract void SetCursor(GraphCanvas drawingCanvas);
	}
}