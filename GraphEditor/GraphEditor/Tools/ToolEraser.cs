using System.IO;
using System.Windows.Input;
using GraphEditor.Graphics;
using GraphEditor.Properties;

namespace GraphEditor.Tools
{
	internal class ToolEraser : ToolObject
	{
		private GraphicsBase deletedObject;

		public ToolEraser()
		{
			var stream = new MemoryStream(Resources.Eraser);
			ToolCursor = new Cursor(stream);
		}

		public override void OnMouseDown(GraphCanvas drawingCanvas, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Right) return;
			var point = e.GetPosition(drawingCanvas);
			deletedObject = null;

			for (int i = drawingCanvas.GraphicsList.Count - 1; i >= 0; i--)
			{
				var o = drawingCanvas[i];

				if (o.MakeHitTest(point) == 0)
				{
					deletedObject = o;
					break;
				}
			}

			if (deletedObject == null)
				HelperFunctions.UnselectAll(drawingCanvas);
			drawingCanvas.CaptureMouse();
		}

		public override void OnMouseMove(GraphCanvas drawingCanvas, MouseEventArgs e)
		{
			if (e.MiddleButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
				return;

			var point = e.GetPosition(drawingCanvas);

			if (e.LeftButton == MouseButtonState.Pressed)
				for (int i = 0; i < drawingCanvas.Count; i++)
					if (0 == drawingCanvas[i].MakeHitTest(point))
						Delete(drawingCanvas, drawingCanvas[i]);

			if (!drawingCanvas.IsMouseCaptured)
				return;
		}

		public override void OnMouseUp(GraphCanvas drawingCanvas, MouseButtonEventArgs e)
		{
			if (!drawingCanvas.IsMouseCaptured)
				return;
			var point = e.GetPosition(drawingCanvas);
			if (deletedObject != null && deletedObject.MakeHitTest(point) == 0)
				Delete(drawingCanvas, deletedObject);
			drawingCanvas.ReleaseMouseCapture();
		}

		private void Delete(GraphCanvas drawingCanvas, GraphicsBase deleted)
		{
			drawingCanvas.UnselectAll();
			deleted.IsSelected = true;
			HelperFunctions.SeclectConnections(drawingCanvas, deleted, false);
			foreach (GraphicsBase sel in drawingCanvas.GraphicsList)
				if (sel.IsSelected && sel is GraphicsEdge)
				{
					var edge = drawingCanvas.GraphStructure.GetEdge(sel.Id);
					int cntVert = 2;
					foreach (GraphicsBase val in drawingCanvas.GraphicsList)
					{
						if (edge.Begin.ID == val.Id || edge.End.ID == val.Id)
						{
							((GraphicsVertex) val).DecSize(5);
							HelperFunctions.SeclectConnections(drawingCanvas, val, true);
							cntVert--;
						}
						if (cntVert <= 0) break;
					}
				}
			drawingCanvas.Delete();
		}
	}
}