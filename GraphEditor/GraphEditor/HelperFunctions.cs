using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GraphEditor.Commands;
using GraphEditor.Graphics;
using GraphEditor.GraphStruct;
using GraphEditor.Properties;

namespace GraphEditor
{
	internal static class HelperFunctions
	{
		public static Cursor DefaultCursor
		{
			get
			{
				var stream = new MemoryStream(Resources.Normal);
				return new Cursor(stream);
			}
		}

		public static void UnselectAll(GraphCanvas graphCanvas)
		{
			for (int i = 0; i < graphCanvas.Count; i++)
			{
				graphCanvas[i].IsSelected = false;
			}
		}

		public static void SelectAll(GraphCanvas drawingCanvas)
		{
			for (int i = 0; i < drawingCanvas.Count; i++)
			{
				drawingCanvas[i].IsSelected = true;
			}
		}

		/// <summary>
		///     Apply new line width
		/// </summary>
		public static bool ApplyLineWidth(GraphCanvas drawingCanvas, double value, bool addToHistory)
		{
			var command = new CommandChangeState(drawingCanvas);
			bool wasChange = false;


			// LineWidth is set for all objects except of GraphicsText.
			// Though GraphicsText has this property, it should remain constant.

			foreach (var g in drawingCanvas.Selection)
			{
				if ( /*g is GraphicsRectangle ||
                     g is GraphicsEllipseVertex ||*/
					g is GraphicsEdge /*||
                     g is GraphicsPolyLine*/)
				{
					if (g.LineWidth != value)
					{
						g.LineWidth = value;
						wasChange = true;
					}
				}
			}

			if (wasChange && addToHistory)
			{
				command.NewState(drawingCanvas);
				drawingCanvas.AddCommandToHistory(command);
			}

			return wasChange;
		}

		/// <summary>
		///     Apply new color
		/// </summary>
		public static bool ApplyColor(GraphCanvas drawingCanvas, Color value, bool addToHistory)
		{
			var command = new CommandChangeState(drawingCanvas);
			bool wasChange = false;

			foreach (var g in drawingCanvas.Selection)
			{
				if (g.ObjectColor != value)
				{
					g.ObjectColor = value;
					wasChange = true;
				}
			}

			if (wasChange && addToHistory)
			{
				command.NewState(drawingCanvas);
				drawingCanvas.AddCommandToHistory(command);
			}

			return wasChange;
		}

		public static void SeclectConnections(GraphCanvas drawingCanvas, GraphicsBase graphics, bool moveToCenter)
		{
			if (graphics is GraphicsVertex)
			{
				int vId = graphics.Id;
				var v = drawingCanvas.GraphStructure.GetVertex(vId);
				if (null == v)
					return;
				var con = v.Connections;
				foreach (var edge in con)
				{
					if (edge.Begin.ID == v.ID)
					{
						foreach (var line in drawingCanvas.GraphicsList)
							if (edge.ID == ((GraphicsBase) line).Id)
							{
								if (moveToCenter)
								{
									var ge = drawingCanvas.GraphicsList.Cast<Visual>().Where(el => ((GraphicsBase) el).Id == edge.End.ID).Cast<GraphicsVertex>().FirstOrDefault();
									var a = ((GraphicsVertex) graphics).Center;
									var b = ge.Center;
									VertexMargin(ref a, ref b, ((GraphicsVertex) graphics).GetRadius(), ge.GetRadius());
									((GraphicsEdge) line).MoveHandleTo(a, 1);
									((GraphicsEdge) line).MoveHandleTo(b, 2);
								}
								else ((GraphicsEdge) line).IsSelected = true;
								break;
							}
					}
					if (edge.End.ID == v.ID)
					{
						foreach (var line in drawingCanvas.GraphicsList)
							if (edge.ID == ((GraphicsBase) line).Id)
							{
								if (moveToCenter)
								{
									var ge = drawingCanvas.GraphicsList.Cast<Visual>().Where(el => ((GraphicsBase) el).Id == edge.Begin.ID).Cast<GraphicsVertex>().FirstOrDefault();
									var a = ((GraphicsVertex) graphics).Center;
									var b = ge.Center;
									VertexMargin(ref a, ref b, ((GraphicsVertex) graphics).GetRadius(), ge.GetRadius());
									((GraphicsEdge) line).MoveHandleTo(a, 2);
									((GraphicsEdge) line).MoveHandleTo(b, 1);
								}
								else ((GraphicsEdge) line).IsSelected = true;
								break;
							}
					}
				}
			}
		}

		public static void SelectElements(GraphCanvas drawingCanvas, IEnumerable<GraphElementBase> itemsToSelect)
		{
			foreach (var graphElementBase in itemsToSelect)
			{
				foreach (var graphic in drawingCanvas.GraphicsList.Cast<GraphicsBase>())
				{
					if (graphic.Id == graphElementBase.ID)
					{
						graphic.IsSelected = true;
					}
				}
			}
		}

		public static void VertexMargin(ref Point a, ref Point b, double radiusA, double radiusB)
		{
			double temp = Math.Atan2(b.Y - a.Y, b.X - a.X);
			a.X = a.X + radiusA * Math.Cos(temp);
			a.Y = a.Y + radiusA * Math.Sin(temp);
			b.X = b.X - radiusB * Math.Cos(temp);
			b.Y = b.Y - radiusB * Math.Sin(temp);
		}

		public static GraphicsBase GetGraphics(GraphCanvas canvas, GraphElementBase element)
		{
			return canvas.GraphicsList.Cast<GraphicsBase>().FirstOrDefault(vert => vert.Id == element.ID);
		}

		public static GraphElementBase GetGraphElement(GraphCanvas canvas, GraphicsBase graphics)
		{
			return canvas.GraphStructure.GetElement(graphics.Id);
		}

		public static GraphicsVertex GetBeginEdge(GraphCanvas canvas, GraphicsEdge edge)
		{
			return
				canvas.GraphicsList.Cast<GraphicsBase>().Where(el => el.Id == ((GraphElementEdge) GetGraphElement(canvas, edge)).Begin.ID).Cast<GraphicsVertex>().FirstOrDefault();
		}

		public static GraphicsVertex GetEndEdge(GraphCanvas canvas, GraphicsEdge edge)
		{
			return canvas.GraphicsList.Cast<GraphicsBase>().Where(el => el.Id == ((GraphElementEdge) GetGraphElement(canvas, edge)).End.ID).Cast<GraphicsVertex>().FirstOrDefault();
		}


		/// <summary>
		///     Delete selected graphic objects
		/// </summary>
		public static void DeleteSelection(GraphCanvas drawingCanvas)
		{
			var command = new CommandDelete(drawingCanvas);
			bool wasChange = false;

			foreach (GraphicsBase g in drawingCanvas.GraphicsList)
				if (g.IsSelected)
					if (g is GraphicsVertex)
					{
						SeclectConnections(drawingCanvas, g, false);
						drawingCanvas.GraphStructure.RemoveVertex(g.Id);
					}
					else if (g is GraphicsEdge)
						drawingCanvas.GraphStructure.RemoveConnection(g.Id);

			for (int i = drawingCanvas.Count - 1; i >= 0; i--)
			{
				if (drawingCanvas[i].IsSelected)
				{
					drawingCanvas.GraphicsList.RemoveAt(i);
					wasChange = true;
				}
			}

			if (wasChange)
			{
				drawingCanvas.AddCommandToHistory(command);
			}
		}

		public static void OrgonizeGraphics(GraphCanvas drawingCanvas)
		{
			var list = drawingCanvas.GraphicsList.OfType<GraphicsEdge>().Select(v => v as GraphicsBase).ToList();
			list.AddRange(drawingCanvas.GraphicsList.OfType<GraphicsVertex>().Select(v => v as GraphicsBase));

			drawingCanvas.GraphicsList.Clear();

			foreach (var v in list)
				drawingCanvas.GraphicsList.Add(v);
		}
	}
}