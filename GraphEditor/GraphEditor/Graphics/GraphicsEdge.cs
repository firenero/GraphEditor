using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GraphEditor.PropertiesClasses;

namespace GraphEditor.Graphics
{
    /// <summary>
    ///  Line graphics object.
    /// </summary>
    public class GraphicsEdge : GraphicsBase
    {
        #region Class Members

        protected Point lineStart;
        protected Point lineEnd;
        protected Point labelPos;
        protected bool oriented;
        #endregion Class Members

        private Pen connect_pen;

        #region Constructors

        public GraphicsEdge(Point start, Point end, String weight, double lineWidth, Color objectColor, Color selectedColor, Color textColor, double actualScale, bool oriented)
        {
            this.lineStart = start;
            this.lineEnd = end;
            labelPos = new Point();
            this.graphicsLineWidth = lineWidth;
            this.graphicsObjectColor = objectColor;
            this.graphicsSelectedColor = selectedColor;
            this.graphicsActualScale = actualScale;
	        this.graphicsTextColor = textColor;
            this.oriented = oriented;
            this.label = new FormattedText(weight, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 16, new SolidColorBrush(textColor));
        }

        public GraphicsEdge()
            :
            this(new Point(0.0, 0.0), new Point(100.0, 100.0), "", 1.0, Colors.Black, Colors.Red, Colors.Black, 1.0, false)
        {
        }

        #endregion Constructors

        #region Properties

        public bool IsOriented
        {
            get { return oriented; }
            set { oriented = value; RefreshDrawing(); }
        }

        public Point Start
        {
            get { return lineStart; }
            set { lineStart = value; }
        }
        public Point LabelPosition
        {
            get { return labelPos; }
        }
        public Point End
        {
            get { return lineEnd; }
            set { lineEnd = value; }
        }

        #endregion Properties

        #region Overrides

        /// <summary>
        /// Draw object
        /// </summary>
        public override void Draw(DrawingContext drawingContext)
        {
            if ( drawingContext == null )
            {
                throw new ArgumentNullException("drawingContext");
            }
			connect_pen = new Pen(new SolidColorBrush(CurrentColor), ActualLineWidth);
            drawingContext.DrawLine(connect_pen, lineStart, lineEnd);

            if (IsSelected) this.label.SetForegroundBrush(new SolidColorBrush(SelectedColor));
            else this.label.SetForegroundBrush(new SolidColorBrush(TextColor));
            Point Z = new Point(lineEnd.X - lineStart.X, lineEnd.Y - lineStart.Y);
            double len_Z = Math.Sqrt(Z.X * Z.X + Z.Y * Z.Y);
            Point EZ = new Point(Z.X / len_Z, Z.Y / len_Z);
            Point OZ = new Point(lineEnd.X - len_Z / 2 * EZ.X - label.Width / 2, lineEnd.Y - len_Z / 2 * EZ.Y - label.Height / 2);
            Point M = new Point(lineStart.Y - lineEnd.Y, lineEnd.X - lineStart.X);
            double len_M = Math.Sqrt(M.X * M.X + M.Y * M.Y);
            Point EM = new Point(M.X / len_M, M.Y / len_M);
            Point AM = new Point(OZ.X + EM.X * (label.Width / 2 + 3), OZ.Y + EM.Y * (label.Height / 2 + 3));
            Point BM = new Point(OZ.X + EM.X * (-(label.Width / 2 + 3)), OZ.Y + EM.Y * (-(label.Height / 2 + 3)));
            double alpha = Math.Atan2(End.Y-Start.Y,End.X-Start.X);
            if (alpha < Math.PI / 2 && alpha >= -Math.PI / 2)
            {
                drawingContext.DrawText(this.label, BM);
                labelPos = BM;
            }
            else
            {
                drawingContext.DrawText(this.label, AM);
                labelPos = AM;
            }

            if (oriented)
            {
                /*
                double angel = Math.Atan2(lineStart.Y - lineEnd.Y, lineStart.X - lineEnd.X);
                drawingContext.DrawLine(new Pen(new SolidColorBrush(CurrentColor), ActualLineWidth), lineEnd, new Point(Convert.ToDouble(lineEnd.X + 15 * Math.Cos(0.3 + angel)), Convert.ToDouble(lineEnd.Y + 15 * Math.Sin(0.3 + angel))));
                drawingContext.DrawLine(new Pen(new SolidColorBrush(CurrentColor), ActualLineWidth), lineEnd, new Point(Convert.ToDouble(lineEnd.X + 15 * Math.Cos(angel - 0.3)), Convert.ToDouble(lineEnd.Y + 15 * Math.Sin(angel - 0.3))));
                */
                Point L = new Point(lineEnd.X - lineStart.X, lineEnd.Y - lineStart.Y);
                double len_L = Math.Sqrt(L.X * L.X + L.Y * L.Y);
                Point E = new Point(L.X / len_L, L.Y / len_L);
                Point O = new Point(lineEnd.X - 15 * E.X, lineEnd.Y - 15 * E.Y);
                Point N = new Point(lineStart.Y - lineEnd.Y, lineEnd.X - lineStart.X);
                double len_N = Math.Sqrt(N.X * N.X + N.Y * N.Y);
                Point EN = new Point(N.X / len_N, N.Y / len_N);
                Point A = new Point(O.X + EN.X * 5, O.Y + EN.Y * 5);
                Point B = new Point(O.X + EN.X * (-5), O.Y + EN.Y * (-5));
                drawingContext.DrawLine(new Pen(new SolidColorBrush(CurrentColor), ActualLineWidth), lineEnd, A);
                drawingContext.DrawLine(new Pen(new SolidColorBrush(CurrentColor), ActualLineWidth), lineEnd, B);
            }
            
            base.Draw(drawingContext);
        }

        /// <summary>
        /// Test whether object contains point
        /// </summary>
        public override bool Contains(Point point)
        {
            LineGeometry g = new LineGeometry(lineStart, lineEnd);

            return g.StrokeContains(new Pen(Brushes.Black, LineHitTestWidth), point);
        }

        /// <summary>
        /// XML serialization support
        /// </summary>
        /// <returns></returns>
        public override PropertiesGraphicsBase CreateSerializedObject()
        {
            return new PropertiesGraphicsEdge(this);
        }

        /// <summary>
        /// Get number of handles
        /// </summary>
        public override int HandleCount
        {
            get
            {
                //return 2;
                return 0;
            }
        }

        /// <summary>
        /// Get handle point by 1-based number
        /// </summary>
        public override Point GetHandle(int handleNumber)
        {
            if (handleNumber == 1)
                return lineStart;
            else
                return lineEnd;
        }

        /// <summary>
        /// Hit test.
        /// Return value: -1 - no hit
        ///                0 - hit anywhere
        ///                > 1 - handle number
        /// </summary>
        public override int MakeHitTest(Point point)
        {
            if (IsSelected)
            {
                for (int i = 1; i <= HandleCount; i++)
                {
                    if (GetHandleRectangle(i).Contains(point))
                        return i;
                }
            }

            if (Contains(point))
                return 0;

            return -1;
        }


        /// <summary>
        /// Test whether object intersects with rectangle
        /// </summary>
        public override bool IntersectsWith(Rect rectangle)
        {
            RectangleGeometry rg = new RectangleGeometry(rectangle);

            LineGeometry lg = new LineGeometry(lineStart, lineEnd);
            PathGeometry widen = lg.GetWidenedPathGeometry(new Pen(Brushes.Black, LineHitTestWidth));

            PathGeometry p = Geometry.Combine(rg, widen, GeometryCombineMode.Intersect, null);

            return (!p.IsEmpty());
        }

        /// <summary>
        /// Get cursor for the handle
        /// </summary>
        public override Cursor GetHandleCursor(int handleNumber)
        {
			if (IsTrackerOn)
				switch (handleNumber)
				{
					case 1:
					case 2:
						return Cursors.SizeAll;
					default:
						return HelperFunctions.DefaultCursor;
				}
			return HelperFunctions.DefaultCursor;
        }

        /// <summary>
        /// Move handle to new point (resizing)
        /// </summary>
        public override void MoveHandleTo(Point point, int handleNumber)
        {
            if (handleNumber == 1)
                lineStart = point;
            else
                lineEnd = point;

            RefreshDrawing();
        }

        /// <summary>
        /// Move object
        /// </summary>
        public override void Move(double deltaX, double deltaY)
        {
            lineStart.X += deltaX;
            lineStart.Y += deltaY;

            lineEnd.X += deltaX;
            lineEnd.Y += deltaY;

            RefreshDrawing();
        }


        #endregion Overrides
    }
}
