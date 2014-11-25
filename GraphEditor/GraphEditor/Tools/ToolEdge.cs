using System.IO;
using System.Windows;
using System.Windows.Input;
using GraphEditor.Commands;
using GraphEditor.Graphics;
using GraphEditor.GraphStruct;

namespace GraphEditor.Tools
{
    /// <summary>
    /// Line tool
    /// </summary>
    class ToolEdge : ToolObject
    {
        private bool pressed = false;
        private bool connected = false;
        private GraphicsVertex begin_vertex;
        private GraphicsVertex end_vertex;
        private GraphicsEdge cur_edge;
        public ToolEdge()
        {
			MemoryStream stream = new MemoryStream(GraphEditor.Properties.Resources.Edge);
            ToolCursor = new Cursor(stream);
        }

        /// <summary>
        /// Create new object
        /// </summary>
        public override void OnMouseDown(GraphCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            if (pressed && cur_edge != null && e.ChangedButton == MouseButton.Right)
            {
                drawingCanvas.GraphicsList.Remove(cur_edge);
                cur_edge = null;
                pressed = false;
				drawingCanvas.Tool = ToolType.Pointer;
                return;
            }

            GraphicsBase o;
            Point point = e.GetPosition(drawingCanvas);
            if (e.ChangedButton == MouseButton.Left)
                for (int i = drawingCanvas.GraphicsList.Count - 1; i >= 0; i--)
                {
                    o = drawingCanvas[i];
                    if (o.MakeHitTest(point) == 0 && o is GraphicsVertex)
                    {
                        if (!pressed)
                        {
                            begin_vertex = (GraphicsVertex)o;

                            Point A = begin_vertex.Center;
                            Point B = new Point(point.X + 1, point.Y + 1);
                            HelperFunctions.VertexMargin(ref A, ref B, ((GraphicsVertex)begin_vertex).GetRadius(A, B), ((GraphicsVertex)begin_vertex).GetRadius(A, B));

                            drawingCanvas.GraphicsList.Remove(begin_vertex);
                            cur_edge = new GraphicsEdge(A, new Point(point.X + 1, point.Y + 1), "", drawingCanvas.LineWidth, drawingCanvas.ObjectColor, drawingCanvas.SelectedColor, drawingCanvas.TextColor, drawingCanvas.ActualScale, drawingCanvas.IsOrientedGraph);
                            AddNewObject(drawingCanvas, cur_edge);
                            connected = false;

                            drawingCanvas.GraphicsList.Add(begin_vertex);
                            //begin_vertex = drawingCanvas.GraphicsList[i] as GraphicsVertex;
                        }
                        else
                        {
                            end_vertex = (GraphicsVertex)o;
                            if (end_vertex.Id != begin_vertex.Id)
                            {
                                GraphElementVertex tmp = drawingCanvas.GraphStructure.GetVertex(begin_vertex.Id);
                                foreach (GraphElementEdge edge in tmp.Connections)
                                    if (edge.End.ID == end_vertex.Id || edge.Begin.ID == end_vertex.Id)
                                        return;
                                begin_vertex.IncSize(5);
                                end_vertex.IncSize(5);
                                Point A = begin_vertex.Center;
                                Point B = end_vertex.Center;
                                HelperFunctions.VertexMargin(ref A, ref B, ((GraphicsVertex)begin_vertex).GetRadius(A, B), ((GraphicsVertex)end_vertex).GetRadius(A, B));
                                cur_edge.MoveHandleTo(B, 2);
                                connected = true;
                            }
                            else return;
                        }
                        pressed = !pressed;
                        if (connected)
                        {
                            HelperFunctions.SeclectConnections(drawingCanvas, begin_vertex, true);
                            HelperFunctions.SeclectConnections(drawingCanvas, end_vertex, true);
                            cur_edge.Label = " ";
                            drawingCanvas.GraphStructure.AddConnection(begin_vertex.Id, end_vertex.Id, "", cur_edge.Id);
                        }
                        break;
                    }
                }
        }

        /// <summary>
        /// Set cursor and resize new object.
        /// </summary>
        public override void OnMouseMove(GraphCanvas drawingCanvas, MouseEventArgs e)
        {
            drawingCanvas.Cursor = ToolCursor;

            if (pressed)
            {
                //if (cur_edge == null) return;
                Point A = begin_vertex.Center;
                Point B = e.GetPosition(drawingCanvas);
                HelperFunctions.VertexMargin(ref A, ref B, ((GraphicsVertex)begin_vertex).GetRadius(A, B), 0);
                cur_edge.MoveHandleTo(A, 1);
                cur_edge.MoveHandleTo(e.GetPosition(drawingCanvas), 2);
            }
        }

        public override void OnMouseUp(GraphCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            if (!pressed && cur_edge != null)
            {
                if (drawingCanvas.Count > 0)
                {
                    cur_edge.Normalize();
                    drawingCanvas.AddCommandToHistory(new CommandAdd(cur_edge, drawingCanvas.GraphStructure.GetElement(cur_edge.Id))); 
                }
                drawingCanvas.ReleaseMouseCapture();
				// Return to Pointer tool
				drawingCanvas.Tool = ToolType.Pointer;
                //בûהכמ לועמה
                HelperFunctions.OrgonizeGraphics(drawingCanvas);
            }
        }
    }
}
