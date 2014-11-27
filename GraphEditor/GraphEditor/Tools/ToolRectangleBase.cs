using System.Windows.Input;

namespace GraphEditor.Tools
{
	/// <summary>
	///     Base class for rectangle-based tools
	/// </summary>
	internal abstract class ToolRectangleBase : ToolObject
	{
		/// <summary>
		///     Set cursor and resize new object.
		/// </summary>
		public override void OnMouseMove(GraphCanvas drawingCanvas, MouseEventArgs e)
		{
			drawingCanvas.Cursor = ToolCursor;
			/*
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (drawingCanvas.IsMouseCaptured)
                {
                    if ( drawingCanvas.Count > 0 )
                    {
                        drawingCanvas[drawingCanvas.Count - 1].MoveHandleTo(
                            e.GetPosition(drawingCanvas), 5);
                    }
                }

            }*/
		}
	}
}