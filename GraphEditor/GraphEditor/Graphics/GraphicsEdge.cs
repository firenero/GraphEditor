using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GraphEditor.PropertiesClasses;

namespace GraphEditor.Graphics
{
	/// <summary>
	///     Line graphics object.
	/// </summary>
	public class GraphicsEdge : GraphicsBase
	{
		#region Class Members

		protected Point labelPos;
		protected Point lineEnd;
		protected Point lineStart;
		protected bool oriented;

		#endregion Class Members

		private Pen connectPen;

		#region Constructors

		public GraphicsEdge(Point start, Point end, String weight, double lineWidth, Color objectColor, Color selectedColor, Color textColor, double actualScale, bool oriented)
		{
			lineStart = start;
			lineEnd = end;
			labelPos = new Point();
			graphicsLineWidth = lineWidth;
			graphicsObjectColor = objectColor;
			graphicsSelectedColor = selectedColor;
			graphicsActualScale = actualScale;
			graphicsTextColor = textColor;
			this.oriented = oriented;
			label = new FormattedText(weight, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 16, new SolidColorBrush(textColor));
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
			set
			{
				oriented = value;
				RefreshDrawing();
			}
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
		///     Get number of handles
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
		///     Draw object
		/// </summary>
		public override void Draw(DrawingContext drawingContext)
		{
			if (drawingContext == null)
			{
				throw new ArgumentNullException("drawingContext");
			}
			connectPen = new Pen(new SolidColorBrush(CurrentColor), ActualLineWidth);
			drawingContext.DrawLine(connectPen, lineStart, lineEnd);

			label.SetForegroundBrush(IsSelected ? new SolidColorBrush(SelectedColor) : new SolidColorBrush(TextColor));
			var z = new Point(lineEnd.X - lineStart.X, lineEnd.Y - lineStart.Y);
			double lenZ = Math.Sqrt(z.X * z.X + z.Y * z.Y);
			var ez = new Point(z.X / lenZ, z.Y / lenZ);
			var oz = new Point(lineEnd.X - lenZ / 2 * ez.X - label.Width / 2, lineEnd.Y - lenZ / 2 * ez.Y - label.Height / 2);
			var m = new Point(lineStart.Y - lineEnd.Y, lineEnd.X - lineStart.X);
			double lenM = Math.Sqrt(m.X * m.X + m.Y * m.Y);
			var em = new Point(m.X / lenM, m.Y / lenM);
			var am = new Point(oz.X + em.X * (label.Width / 2 + 3), oz.Y + em.Y * (label.Height / 2 + 3));
			var bm = new Point(oz.X + em.X * (-(label.Width / 2 + 3)), oz.Y + em.Y * (-(label.Height / 2 + 3)));
			double alpha = Math.Atan2(End.Y - Start.Y, End.X - Start.X);
			if (alpha < Math.PI / 2 && alpha >= -Math.PI / 2)
			{
				drawingContext.DrawText(label, bm);
				labelPos = bm;
			}
			else
			{
				drawingContext.DrawText(label, am);
				labelPos = am;
			}

			if (oriented)
			{
				/*
                double angel = Math.Atan2(lineStart.Y - lineEnd.Y, lineStart.X - lineEnd.X);
                drawingContext.DrawLine(new Pen(new SolidColorBrush(CurrentColor), ActualLineWidth), lineEnd, new Point(Convert.ToDouble(lineEnd.X + 15 * Math.Cos(0.3 + angel)), Convert.ToDouble(lineEnd.Y + 15 * Math.Sin(0.3 + angel))));
                drawingContext.DrawLine(new Pen(new SolidColorBrush(CurrentColor), ActualLineWidth), lineEnd, new Point(Convert.ToDouble(lineEnd.X + 15 * Math.Cos(angel - 0.3)), Convert.ToDouble(lineEnd.Y + 15 * Math.Sin(angel - 0.3))));
                */
				var l = new Point(lineEnd.X - lineStart.X, lineEnd.Y - lineStart.Y);
				double lenL = Math.Sqrt(l.X * l.X + l.Y * l.Y);
				var e = new Point(l.X / lenL, l.Y / lenL);
				var o = new Point(lineEnd.X - 15 * e.X, lineEnd.Y - 15 * e.Y);
				var n = new Point(lineStart.Y - lineEnd.Y, lineEnd.X - lineStart.X);
				double lenN = Math.Sqrt(n.X * n.X + n.Y * n.Y);
				var en = new Point(n.X / lenN, n.Y / lenN);
				var a = new Point(o.X + en.X * 5, o.Y + en.Y * 5);
				var b = new Point(o.X + en.X * (-5), o.Y + en.Y * (-5));
				drawingContext.DrawLine(new Pen(new SolidColorBrush(CurrentColor), ActualLineWidth), lineEnd, a);
				drawingContext.DrawLine(new Pen(new SolidColorBrush(CurrentColor), ActualLineWidth), lineEnd, b);
			}

			base.Draw(drawingContext);
		}

		/// <summary>
		///     Test whether object contains point
		/// </summary>
		public override bool Contains(Point point)
		{
			var g = new LineGeometry(lineStart, lineEnd);

			return g.StrokeContains(new Pen(Brushes.Black, LineHitTestWidth), point);
		}

		/// <summary>
		///     XML serialization support
		/// </summary>
		/// <returns></returns>
		public override PropertiesGraphicsBase CreateSerializedObject()
		{
			return new PropertiesGraphicsEdge(this);
		}

		/// <summary>
		///     Get handle point by 1-based number
		/// </summary>
		public override Point GetHandle(int handleNumber)
		{
			if (handleNumber == 1)
				return lineStart;
			return lineEnd;
		}

		/// <summary>
		///     Hit test.
		///     Return value: -1 - no hit
		///     0 - hit anywhere
		///     > 1 - handle number
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
		///     Test whether object intersects with rectangle
		/// </summary>
		public override bool IntersectsWith(Rect rectangle)
		{
			var rg = new RectangleGeometry(rectangle);

			var lg = new LineGeometry(lineStart, lineEnd);
			var widen = lg.GetWidenedPathGeometry(new Pen(Brushes.Black, LineHitTestWidth));

			var p = Geometry.Combine(rg, widen, GeometryCombineMode.Intersect, null);

			return (!p.IsEmpty());
		}

		/// <summary>
		///     Get cursor for the handle
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
		///     Move handle to new point (resizing)
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
		///     Move object
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