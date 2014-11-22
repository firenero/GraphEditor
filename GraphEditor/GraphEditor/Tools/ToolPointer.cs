using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GraphEditor.Commands;
using GraphEditor.Graphics;

namespace GraphEditor.Tools
{
    class ToolPointer : Tool
    {
        private enum SelectionMode
        {
            None,
            Move,           // object(s) are moved
            Size,           // object is resized
            GroupSelection
        }

        private SelectionMode selectMode = SelectionMode.None;

        // Object which is currently resized:
        private GraphicsBase resizedObject;
        private int resizedObjectHandle;

        // Keep state about last and current point (used to move and resize objects)
        private Point lastPoint = new Point(0, 0);

        private CommandChangeState commandChangeState;
        bool wasMove;

        public ToolPointer()
        {
        }

        /// <summary>
        /// Handle mouse down.
        /// Start moving, resizing or group selection.
        /// </summary>
        public override void OnMouseDown(GraphCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            commandChangeState = null;
            wasMove = false;


            Point point = e.GetPosition(drawingCanvas);

            selectMode = SelectionMode.None;

            GraphicsBase o;
            GraphicsBase movedObject = null;
            int handleNumber;

            // Test for resizing (only if control is selected, cursor is on the handle)
            for (int i = drawingCanvas.GraphicsList.Count - 1; i >= 0; i--)
            {
                o = drawingCanvas[i];

                if (o.IsSelected && o.IsTrackerOn)
                {
                    handleNumber = o.MakeHitTest(point);

                    if (handleNumber > 0)
                    {
                        selectMode = SelectionMode.Size;

                        // keep resized object in class member
                        resizedObject = o;
                        resizedObjectHandle = handleNumber;

                        // Since we want to resize only one object, unselect all other objects
                        HelperFunctions.UnselectAll(drawingCanvas);
                        o.IsSelected = true;

                        commandChangeState = new CommandChangeState(drawingCanvas);

                        break;
                    }
                }
            }
            
            // Test for move (cursor is on the object)
            if (selectMode == SelectionMode.None)
            {
                for (int i = drawingCanvas.GraphicsList.Count - 1; i >= 0; i--)
                {
                    o = drawingCanvas[i];

                    if (o.MakeHitTest(point) == 0)
                    {
                        movedObject = o;
                        break;
                    }
                }

                if (movedObject != null)
                {
                    selectMode = SelectionMode.Move;

                    // Unselect all if Ctrl is not pressed and clicked object is not selected yet
                    if (Keyboard.Modifiers != ModifierKeys.Control && !movedObject.IsSelected)
                    {
                        HelperFunctions.UnselectAll(drawingCanvas);
                    }

                    // Select clicked object
                    movedObject.IsSelected = true;
                    HelperFunctions.SeclectConnections(drawingCanvas, movedObject, false);

                    // Set move cursor
                    drawingCanvas.Cursor = Cursors.SizeAll;

                    commandChangeState = new CommandChangeState(drawingCanvas);
                }
            }

            // Click on background
            if (selectMode == SelectionMode.None)
            {
                // Unselect all if Ctrl is not pressed
                if (Keyboard.Modifiers != ModifierKeys.Control)
                {
                    HelperFunctions.UnselectAll(drawingCanvas);
                }

                // Group selection. Create selection rectangle.
                GraphicsSelectionRectangle r = new GraphicsSelectionRectangle(
                    point.X, point.Y,
                    point.X + 1, point.Y + 1,
                    drawingCanvas.ActualScale);

                r.Clip = new RectangleGeometry(new Rect(0, 0, drawingCanvas.ActualWidth, drawingCanvas.ActualHeight));

                drawingCanvas.GraphicsList.Add(r);

                selectMode = SelectionMode.GroupSelection;
            }


            lastPoint = point;

            // Capture mouse until MouseUp event is received
            drawingCanvas.CaptureMouse();
        }

        /// <summary>
        /// Handle mouse move.
        /// Se cursor, move/resize, make group selection.
        /// </summary>
        public override void OnMouseMove(GraphCanvas drawingCanvas, MouseEventArgs e)
        {
            // Exclude all cases except left button on/off.
            if (e.MiddleButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            {
                return;
            }

            Point point = e.GetPosition(drawingCanvas);

            // Set cursor when left button is not pressed
            if (e.LeftButton == MouseButtonState.Released)
            {
                Cursor cursor = null;

                for (int i = 0; i < drawingCanvas.Count; i++)
                {
                    int n = drawingCanvas[i].MakeHitTest(point);

                    if (n > 0)
                    {
                        cursor = drawingCanvas[i].GetHandleCursor(n);
                        break;
                    }
                }

                if (cursor == null)
                    cursor = HelperFunctions.DefaultCursor;

                drawingCanvas.Cursor = cursor;

                return;

            }

            if (!drawingCanvas.IsMouseCaptured)
            {
                return;
            }

            wasMove = true;

            // Find difference between previous and current position
            double dx = point.X - lastPoint.X;
            double dy = point.Y - lastPoint.Y;

            lastPoint = point;
            
            // Resize
            if (selectMode == SelectionMode.Size)
            {
                if (resizedObject != null)
                {
                    resizedObject.MoveHandleTo(point, resizedObjectHandle);
                }
            }
            
            // Move
            if (selectMode == SelectionMode.Move)
            {
                int cnt = 0;
                using (var enumerator = drawingCanvas.Selection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                        cnt++;
                }
                foreach (GraphicsBase o in drawingCanvas.Selection)
                {
                    if (!(o is GraphicsEdge))             //если пытаемся переместить только ребро
                        o.Move(dx, dy);
                    HelperFunctions.SeclectConnections(drawingCanvas, o, true);
                }
            }

            // Group selection
            if (selectMode == SelectionMode.GroupSelection)
            {
                // Resize selection rectangle
                drawingCanvas[drawingCanvas.Count - 1].MoveHandleTo(
                    point, 5);
            }
        }

        /// <summary>
        /// Handle mouse up.
        /// Return to normal state.
        /// </summary>
        public override void OnMouseUp(GraphCanvas drawingCanvas, MouseButtonEventArgs e)
        {

            if (!drawingCanvas.IsMouseCaptured)
            {
                drawingCanvas.Cursor = HelperFunctions.DefaultCursor;
                selectMode = SelectionMode.None;
                return;
            }
            
            if (resizedObject != null)
            {
                // after resizing
                resizedObject.Normalize();
                /*
                // Special case for text
                if (resizedObject is GraphicsText)
                {
                    ((GraphicsText)resizedObject).UpdateRectangle();
                }
                */
                resizedObject = null;
            }
        
            if (selectMode == SelectionMode.GroupSelection)
            {
                GraphicsSelectionRectangle r = (GraphicsSelectionRectangle)drawingCanvas[drawingCanvas.Count - 1];
                r.Normalize();
                Rect rect = r.Rectangle;

                drawingCanvas.GraphicsList.Remove(r);

                foreach (GraphicsBase g in drawingCanvas.GraphicsList)
                {
                    if (g.IntersectsWith(rect))
                    {
                        g.IsSelected = true;
                        HelperFunctions.SeclectConnections(drawingCanvas, g, false);
                    }
                }
            }

            drawingCanvas.ReleaseMouseCapture();

            drawingCanvas.Cursor = HelperFunctions.DefaultCursor;

            selectMode = SelectionMode.None;

            AddChangeToHistory(drawingCanvas);
        }

        /// <summary>
        /// Set cursor
        /// </summary>
        public override void SetCursor(GraphCanvas drawingCanvas)
        {
            drawingCanvas.Cursor = HelperFunctions.DefaultCursor;
        }


        /// <summary>
        /// Add change to history.
        /// Called after finishing moving/resizing.
        /// </summary>
        public void AddChangeToHistory(GraphCanvas drawingCanvas)
        {
            if (commandChangeState != null && wasMove)
            {
                // Keep state after moving/resizing and add command to history
                commandChangeState.NewState(drawingCanvas);
                drawingCanvas.AddCommandToHistory(commandChangeState);
                commandChangeState = null;
            }
        }
    }
}
