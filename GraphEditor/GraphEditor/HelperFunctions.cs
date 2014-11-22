using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GraphEditor.Commands;
using GraphEditor.Graphics;
using GraphEditor.GraphStruct;

namespace GraphEditor
{
    class HelperFunctions
    {
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
        public static Cursor DefaultCursor
        {
            get
            {
                MemoryStream stream = new MemoryStream(GraphEditor.Properties.Resources.Normal);
                return new Cursor(stream);
            }
        }
        /// <summary>
        /// Apply new line width
        /// </summary>
        public static bool ApplyLineWidth(GraphCanvas drawingCanvas, double value, bool addToHistory)
        {
            CommandChangeState command = new CommandChangeState(drawingCanvas);
            bool wasChange = false;


            // LineWidth is set for all objects except of GraphicsText.
            // Though GraphicsText has this property, it should remain constant.

            foreach (GraphicsBase g in drawingCanvas.Selection)
            {
                if (/*g is GraphicsRectangle ||
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
        /// Apply new color
        /// </summary>
        public static bool ApplyColor(GraphCanvas drawingCanvas, Color value, bool addToHistory)
        {
            CommandChangeState command = new CommandChangeState(drawingCanvas);
            bool wasChange = false;

            foreach (GraphicsBase g in drawingCanvas.Selection)
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
        
        public static void SeclectConnections(GraphCanvas drawingCanvas, GraphicsBase graphics, bool move_to_center)
        {
            if (graphics is GraphicsVertex)
            {
                int v_id = ((GraphicsVertex)graphics).Id;
                GraphElementVertex v = drawingCanvas.GraphStructure.GetVertex(v_id);
                if (null == v) 
                    return;
                List<GraphElementEdge> con = v.Connections;
                foreach (var edge in con)
                {
                    if (edge.Begin.ID == v.ID)
                    {
                        foreach (var line in drawingCanvas.GraphicsList)
                            if (edge.ID == ((GraphicsBase)line).Id)
                            {
                                if (move_to_center)
                                {
                                    GraphicsVertex ge =null;
                                    foreach (var el in drawingCanvas.GraphicsList)
                                        if (((GraphicsBase)el).Id == edge.End.ID)
                                        {
                                            ge = (GraphicsVertex)el;
                                            break;
                                        }
                                    Point A = ((GraphicsVertex)graphics).Center;
                                    Point B = ge.Center;
                                    VertexMargin(ref A, ref B, ((GraphicsVertex)graphics).GetRadius(A, B), ((GraphicsVertex)ge).GetRadius(A, B));
                                    ((GraphicsEdge)line).MoveHandleTo(A, 1);
                                    ((GraphicsEdge)line).MoveHandleTo(B, 2);
                                }
                                else ((GraphicsEdge)line).IsSelected = true;
                                break;
                            }
                    }
                    if (edge.End.ID == v.ID)
                    {
                        foreach (var line in drawingCanvas.GraphicsList)
                            if (edge.ID == ((GraphicsBase)line).Id)
                            {
                                if (move_to_center)
                                {
                                    GraphicsVertex ge = null;
                                    foreach (var el in drawingCanvas.GraphicsList)
                                        if (((GraphicsBase)el).Id == edge.Begin.ID)
                                        {
                                            ge = (GraphicsVertex)el;
                                            break;
                                        }
                                    Point A = ((GraphicsVertex)graphics).Center;
                                    Point B = ge.Center;
                                    VertexMargin(ref A, ref B, ((GraphicsVertex)graphics).GetRadius(A, B), ((GraphicsVertex)ge).GetRadius(A, B));
                                    ((GraphicsEdge)line).MoveHandleTo(A, 2);
                                    ((GraphicsEdge)line).MoveHandleTo(B, 1);
                                }
                                else ((GraphicsEdge)line).IsSelected = true;
                                break;
                            }
                    }
                }
            }
        }

        public static void VertexMargin(ref Point A, ref Point B, double radiusA, double radiusB)
        {
            double a = Math.Atan2(B.Y - A.Y,B.X - A.X);
            A.X = A.X + radiusA * Math.Cos(a);
            A.Y = A.Y + radiusA * Math.Sin(a);
            B.X = B.X - radiusB * Math.Cos(a);
            B.Y = B.Y - radiusB * Math.Sin(a);
        }

        public static GraphicsBase GetGraphics(GraphCanvas canvas, GraphElementBase element)
        {
            foreach (GraphicsBase vert in canvas.GraphicsList)
                if (vert.Id == element.ID)
                    return vert;
            return null;
        }

        public static GraphElementBase GetGraphElement(GraphCanvas canvas, GraphicsBase graphics)
        {
             return canvas.GraphStructure.GetElement(graphics.Id);
        }

        public static GraphicsVertex GetBeginEdge(GraphCanvas canvas, GraphicsEdge edge)
        {
            foreach (GraphicsBase el in canvas.GraphicsList)
                if (el.Id == ((GraphElementEdge)GetGraphElement(canvas, edge)).Begin.ID)
                    return (GraphicsVertex)el;
            return null;
        }
        public static GraphicsVertex GetEndEdge(GraphCanvas canvas, GraphicsEdge edge)
        {
            foreach (GraphicsBase el in canvas.GraphicsList)
                if (el.Id == ((GraphElementEdge)GetGraphElement(canvas, edge)).End.ID)
                    return (GraphicsVertex)el;
            return null;
        }


        /// <summary>
        /// Delete selected graphic objects
        /// </summary>
        public static void DeleteSelection(GraphCanvas drawingCanvas)
        {
            CommandDelete command = new CommandDelete(drawingCanvas);
            bool wasChange = false;

            foreach (GraphicsBase g in drawingCanvas.GraphicsList)
                if (g.IsSelected)
                    if (g is GraphicsVertex)
                        drawingCanvas.GraphStructure.RemoveVertex(g.Id);
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
            List<GraphicsBase> list = new List<GraphicsBase>();

            foreach(var v in drawingCanvas.GraphicsList)
                if (v is GraphicsEdge)
                    list.Add(v as GraphicsBase);
            foreach (var v in drawingCanvas.GraphicsList)
                if (v is GraphicsVertex)
                    list.Add(v as GraphicsBase);
            drawingCanvas.GraphicsList.Clear();

            foreach (var v in list)
                drawingCanvas.GraphicsList.Add(v);
        }
    }
}
