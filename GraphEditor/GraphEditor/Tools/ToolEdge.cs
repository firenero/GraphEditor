using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GraphEditor.Commands;
using GraphEditor.Graphics;
using GraphEditor.Properties;

namespace GraphEditor.Tools
{
	/// <summary>
	///     Line tool
	/// </summary>
	internal class ToolEdge : ToolObject
	{
		private GraphicsVertex beginVertex;
		private bool connected;
		private GraphicsEdge curEdge;
		private GraphicsVertex endVertex;
		private bool pressed;

		public ToolEdge()
		{
			var stream = new MemoryStream(Resources.Edge);
			ToolCursor = new Cursor(stream);
		}

		/// <summary>
		///     Create new object
		/// </summary>
		public override void OnMouseDown(GraphCanvas drawingCanvas, MouseButtonEventArgs e)
		{
			if (pressed && curEdge != null && e.ChangedButton == MouseButton.Right)
			{
				drawingCanvas.GraphicsList.Remove(curEdge);
				curEdge = null;
				pressed = false;
				drawingCanvas.Tool = ToolType.Pointer;
				return;
			}

			var point = e.GetPosition(drawingCanvas);
			if (e.ChangedButton == MouseButton.Left)
				for (int i = drawingCanvas.GraphicsList.Count - 1; i >= 0; i--)
				{
					var o = drawingCanvas[i];
					if (o.MakeHitTest(point) == 0 && o is GraphicsVertex)
					{
						if (!pressed)
						{
							beginVertex = (GraphicsVertex) o;

							var a = beginVertex.Center;
							var b = new Point(point.X + 1, point.Y + 1);
							HelperFunctions.VertexMargin(ref a, ref b, beginVertex.GetRadius(), beginVertex.GetRadius());

							drawingCanvas.GraphicsList.Remove(beginVertex);
							curEdge = new GraphicsEdge(a, new Point(point.X + 1, point.Y + 1), "", drawingCanvas.LineWidth, drawingCanvas.ObjectColor, drawingCanvas.SelectedColor,
							                           drawingCanvas.TextColor, drawingCanvas.ActualScale, drawingCanvas.IsOrientedGraph);
							AddNewObject(drawingCanvas, curEdge);
							connected = false;

							drawingCanvas.GraphicsList.Add(beginVertex);
							//begin_vertex = drawingCanvas.GraphicsList[i] as GraphicsVertex;
						}
						else
						{
							endVertex = (GraphicsVertex) o;
							if (endVertex.Id != beginVertex.Id)
							{
								var tmp = drawingCanvas.GraphStructure.GetVertex(beginVertex.Id);
								if (tmp.Connections.Any(edge => edge.End.ID == endVertex.Id || edge.Begin.ID == endVertex.Id))
								{
									return;
								}
								beginVertex.IncSize(5);
								endVertex.IncSize(5);
								var a = beginVertex.Center;
								var b = endVertex.Center;
								HelperFunctions.VertexMargin(ref a, ref b, beginVertex.GetRadius(), endVertex.GetRadius());
								curEdge.MoveHandleTo(b, 2);
								connected = true;
							}
							else return;
						}
						pressed = !pressed;
						if (connected)
						{
							HelperFunctions.SeclectConnections(drawingCanvas, beginVertex, true);
							HelperFunctions.SeclectConnections(drawingCanvas, endVertex, true);
							curEdge.Label = "0";
							drawingCanvas.GraphStructure.AddConnection(beginVertex.Id, endVertex.Id, 0, curEdge.Id);
							OnMouseUp(drawingCanvas, e);
						}
						break;
					}
				}
		}

		/// <summary>
		///     Set cursor and resize new object.
		/// </summary>
		public override void OnMouseMove(GraphCanvas drawingCanvas, MouseEventArgs e)
		{
			drawingCanvas.Cursor = ToolCursor;

			if (pressed)
			{
				//if (cur_edge == null) return;
				var a = beginVertex.Center;
				var b = e.GetPosition(drawingCanvas);
				HelperFunctions.VertexMargin(ref a, ref b, beginVertex.GetRadius(), 0);
				curEdge.MoveHandleTo(a, 1);
				curEdge.MoveHandleTo(e.GetPosition(drawingCanvas), 2);
			}
		}

		public override void OnMouseUp(GraphCanvas drawingCanvas, MouseButtonEventArgs e)
		{
			if (!pressed && curEdge != null)
			{
				if (drawingCanvas.Count > 0)
				{
					curEdge.Normalize();
					drawingCanvas.AddCommandToHistory(new CommandAdd(curEdge, drawingCanvas.GraphStructure.GetElement(curEdge.Id)));
				}
				drawingCanvas.ReleaseMouseCapture();
				if (Keyboard.Modifiers != ModifierKeys.Control)
					drawingCanvas.Tool = ToolType.Pointer;
				//בûהכמ לועמה
				HelperFunctions.OrgonizeGraphics(drawingCanvas);
			}
		}
	}
}