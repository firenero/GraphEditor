using System.Globalization;
using System.IO;
using System.Windows.Input;
using GraphEditor.Commands;
using GraphEditor.Graphics;
using GraphEditor.Properties;

namespace GraphEditor.Tools
{
	/// <summary>
	///     Ellipse tool
	/// </summary>
	internal class ToolVertex : ToolRectangleBase
	{
		public ToolVertex()
		{
			var stream = new MemoryStream(Resources.Vertex);
			ToolCursor = new Cursor(stream);
		}

		/// <summary>
		///     Create new rectangle
		/// </summary>
		public override void OnMouseDown(GraphCanvas drawingCanvas, MouseButtonEventArgs e)
		{
			var p = e.GetPosition(drawingCanvas);
			switch (e.ChangedButton)
			{
				case MouseButton.Left:
					AddNewObject(drawingCanvas,
					             new GraphicsVertex(p, 20.0, GenerateName(drawingCanvas), drawingCanvas.LineWidth, drawingCanvas.ObjectColor, drawingCanvas.SelectedColor,
					                                drawingCanvas.TextColor, drawingCanvas.ActualScale));
					drawingCanvas.GraphStructure.AddVertex(GenerateName(drawingCanvas), drawingCanvas[drawingCanvas.Count - 1].Id);
					OnMouseUp(drawingCanvas, e);
					break;
				case MouseButton.Right:
					drawingCanvas.Tool = ToolType.Pointer;
					break;
			}
		}

		public override void OnMouseUp(GraphCanvas drawingCanvas, MouseButtonEventArgs e)
		{
			if (drawingCanvas.Count > 0)
			{
				drawingCanvas[drawingCanvas.Count - 1].Normalize();

				drawingCanvas.AddCommandToHistory(new CommandAdd(drawingCanvas[drawingCanvas.Count - 1],
																 drawingCanvas.GraphStructure.GetElement(drawingCanvas[drawingCanvas.Count - 1].Id)));
			}
			drawingCanvas.ReleaseMouseCapture();
			if (Keyboard.Modifiers != ModifierKeys.Control)
				drawingCanvas.Tool = ToolType.Pointer;
		}

		private string GenerateName(GraphCanvas drawingCanvas)
		{
			return "Vertex" + drawingCanvas.GraphStructure.Vertices.Count.ToString(CultureInfo.InvariantCulture);
		}
	}
}