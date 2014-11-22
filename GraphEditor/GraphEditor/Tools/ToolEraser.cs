using System.IO;
using System.Windows;
using System.Windows.Input;
using GraphEditor.Graphics;
using GraphEditor.GraphStruct;

namespace GraphEditor.Tools
{
    class ToolEraser : ToolObject
    {
        GraphicsBase deletedObject = null;
        public ToolEraser()
        {
			MemoryStream stream = new MemoryStream(GraphEditor.Properties.Resources.Eraser);
            ToolCursor = new Cursor(stream);
        }

        public override void OnMouseDown(GraphCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right) return;
            Point point = e.GetPosition(drawingCanvas);
            GraphicsBase o;
            deletedObject = null;

            for (int i = drawingCanvas.GraphicsList.Count - 1; i >= 0; i--)
            {
                o = drawingCanvas[i];

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

            Point point = e.GetPosition(drawingCanvas);

            if (e.LeftButton == MouseButtonState.Pressed)
                for (int i = 0; i < drawingCanvas.Count; i++)
                    if(0 == drawingCanvas[i].MakeHitTest(point))
                        Delete(drawingCanvas, drawingCanvas[i]);

            if (!drawingCanvas.IsMouseCaptured)
                return;
        }

        public override void OnMouseUp(GraphCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            if (!drawingCanvas.IsMouseCaptured)
                return;
            Point point = e.GetPosition(drawingCanvas);
            if (deletedObject != null && deletedObject.MakeHitTest(point) == 0)
                Delete(drawingCanvas, deletedObject);
            drawingCanvas.ReleaseMouseCapture();
        }

        private void Delete(GraphCanvas drawingCanvas, GraphicsBase deleted)
        {
            drawingCanvas.UnselectAll();
            deleted.IsSelected = true;
            HelperFunctions.SeclectConnections(drawingCanvas, deleted, false);
            foreach(GraphicsBase sel in drawingCanvas.GraphicsList)
            if (sel.IsSelected && sel is GraphicsEdge)
            {
                GraphElementEdge edge = drawingCanvas.GraphStructure.GetEdge(sel.Id);
                int cnt_vert = 2;
                foreach (GraphicsBase val in drawingCanvas.GraphicsList)
                {
                    if (edge.Begin.ID == val.Id || edge.End.ID == val.Id)
                    {
                        ((GraphicsVertex)val).DecSize(5);
                        HelperFunctions.SeclectConnections(drawingCanvas, val, true);
                        cnt_vert--;
                    }
                    if (cnt_vert <= 0) break;
                }
            }
            drawingCanvas.Delete();
        }

    }
}
