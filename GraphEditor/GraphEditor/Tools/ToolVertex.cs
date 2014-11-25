using System.IO;
using System.Windows;
using System.Windows.Input;
using GraphEditor.Graphics;

namespace GraphEditor.Tools
{
    /// <summary>
    /// Ellipse tool
    /// </summary>
    class ToolVertex : ToolRectangleBase
    {
        public ToolVertex()
        {
			MemoryStream stream = new MemoryStream(GraphEditor.Properties.Resources.Vertex);
            ToolCursor = new Cursor(stream);
        }

        /// <summary>
        /// Create new rectangle
        /// </summary>
        public override void OnMouseDown(GraphCanvas drawingCanvas, MouseButtonEventArgs e)
        {
	        Point p = e.GetPosition(drawingCanvas);
	        switch (e.ChangedButton)
	        {
		        case MouseButton.Left:
			        AddNewObject(drawingCanvas, new GraphicsVertex(p, 20.0, "Vertex", drawingCanvas.LineWidth, drawingCanvas.ObjectColor, drawingCanvas.SelectedColor, drawingCanvas.TextColor, drawingCanvas.ActualScale));
			        drawingCanvas.GraphStructure.AddVertex("Vertex", ((GraphicsVertex)drawingCanvas[drawingCanvas.Count - 1]).Id);
			        break;
		        case MouseButton.Right:
			        drawingCanvas.Tool = ToolType.Pointer;
			        break;
	        }
        }
    }
}
