using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GraphEditor.Commands;
using GraphEditor.Graphics;

namespace GraphEditor.Tools
{
	/// <summary>
	///     Base class for all tools which create new graphic object
	/// </summary>
	internal abstract class ToolObject : Tool
	{
		private Cursor toolCursor;

		/// <summary>
		///     Tool cursor.
		/// </summary>
		protected Cursor ToolCursor
		{
			get { return toolCursor; }
			set { toolCursor = value; }
		}


		/// <summary>
		///     Left mouse is released.
		///     New object is created and resized.
		/// </summary>
		public override void OnMouseUp(GraphCanvas drawingCanvas, MouseButtonEventArgs e)
		{
			if (drawingCanvas.Count > 0)
			{
				drawingCanvas[drawingCanvas.Count - 1].Normalize();

				drawingCanvas.AddCommandToHistory(new CommandAdd(drawingCanvas[drawingCanvas.Count - 1],
				                                                 drawingCanvas.GraphStructure.GetElement(drawingCanvas[drawingCanvas.Count - 1].Id)));
			}
			drawingCanvas.ReleaseMouseCapture();
			// Return to Pointer tool
			drawingCanvas.Tool = ToolType.Pointer;
		}

		/// <summary>
		///     Add new object to drawing canvas.
		///     Function is called when user left-clicks drawing canvas,
		///     and one of ToolObject-derived tools is active.
		/// </summary>
		protected static void AddNewObject(GraphCanvas drawingCanvas, GraphicsBase o)
		{
			HelperFunctions.UnselectAll(drawingCanvas);

			o.IsSelected = true;
			o.Clip = new RectangleGeometry(new Rect(0, 0, drawingCanvas.ActualWidth, drawingCanvas.ActualHeight));
			o.IsTrackerOn = drawingCanvas.Tracker;
			drawingCanvas.GraphicsList.Add(o);
			drawingCanvas.CaptureMouse();
		}

		/// <summary>
		///     Set cursor
		/// </summary>
		public override void SetCursor(GraphCanvas drawingCanvas)
		{
			drawingCanvas.Cursor = toolCursor;
		}
	}
}