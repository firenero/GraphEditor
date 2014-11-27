using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GraphEditor.PropertiesClasses;

namespace GraphEditor.Graphics
{
	public abstract class GraphicsBase : DrawingVisual
	{
		protected const double HandleSize = 12.0;
		private static SolidColorBrush ext_track_brush = new SolidColorBrush(Color.FromArgb(255, 65, 177, 225));
		// internal
		private static SolidColorBrush int_track_brush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
		protected double graphicsActualScale;
		protected double graphicsLineWidth;
		protected Color graphicsObjectColor;
		protected Color graphicsSelectedColor;
		protected Color graphicsTextColor;
		protected FormattedText label = new FormattedText("", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 16, Brushes.Black);
		protected bool selected;
		protected bool tracker;

		// external rectangle

		protected GraphicsBase()
		{
			Id = GetHashCode();
		}

		public int Id { get; set; }

		public bool IsSelected
		{
			get { return selected; }
			set
			{
				selected = value;
				RefreshDrawing();
			}
		}

		public bool IsTrackerOn
		{
			get { return tracker; }
			set
			{
				tracker = value;
				RefreshDrawing();
			}
		}

		public abstract int HandleCount { get; }

		protected double ActualLineWidth
		{
			get { return graphicsActualScale <= 0 ? graphicsLineWidth : graphicsLineWidth / graphicsActualScale; }
		}

		public double ActualScale
		{
			get { return graphicsActualScale; }

			set
			{
				graphicsActualScale = value;

				RefreshDrawing();
			}
		}

		public Color ObjectColor
		{
			get { return graphicsObjectColor; }

			set
			{
				graphicsObjectColor = value;

				RefreshDrawing();
			}
		}

		public Color TextColor
		{
			get { return graphicsTextColor; }
			set
			{
				graphicsTextColor = value;
				RefreshDrawing();
			}
		}

		public Color CurrentColor
		{
			get { return IsSelected ? graphicsSelectedColor : graphicsObjectColor; }
		}

		public Color SelectedColor
		{
			get { return graphicsSelectedColor; }

			set
			{
				graphicsSelectedColor = value;

				RefreshDrawing();
			}
		}

		public double LineWidth
		{
			get { return graphicsLineWidth; }

			set
			{
				graphicsLineWidth = value;

				RefreshDrawing();
			}
		}

		public String Label
		{
			get { return label.Text; }
			set
			{
				label = new FormattedText(value, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 16, new SolidColorBrush(ObjectColor));
			}
		}

		public FormattedText FormatedLabel
		{
			get { return label; }
		}

		protected double LineHitTestWidth
		{
			get
			{
				// Ensure that hit test area is not too narrow
				return Math.Max(8.0, ActualLineWidth);
			}
		}

		public abstract Point GetHandle(int handleNumber);

		/// <summary>
		///     Hit test.
		///     Return value: -1 - no hit
		///     0 - hit anywhere
		///     > 1 - handle number
		/// </summary>
		public abstract int MakeHitTest(Point point);

		/// <summary>
		///     Hit test, should be overwritten in derived classes.
		/// </summary>
		public abstract bool Contains(Point point);

		/// <summary>
		///     Get cursor for the handle
		/// </summary>
		public abstract Cursor GetHandleCursor(int handleNumber);

		/// <summary>
		///     Move handle to the point
		/// </summary>
		public abstract void MoveHandleTo(Point point, int handleNumber);

		/// <summary>
		///     Move object
		/// </summary>
		public abstract void Move(double deltaX, double deltaY);

		/// <summary>
		///     Create object for serialization
		/// </summary>
		public abstract PropertiesGraphicsBase CreateSerializedObject();

		/// <summary>
		///     Normalize object.
		///     Call this function in the end of object resizing,
		/// </summary>
		public virtual void Normalize()
		{
			// Empty implementation is OK for classes which don't require
			// normalization, like line.
			// Normalization is required for rectangle-based classes.
		}

		/// <summary>
		///     Test whether object intersects with rectangle
		/// </summary>
		public abstract bool IntersectsWith(Rect rectangle);

		public Rect GetHandleRectangle(int handleNumber)
		{
			var point = GetHandle(handleNumber);

			// Handle rectangle should have constant size, except of the case
			// when line is too width.
			double size = Math.Max(HandleSize / graphicsActualScale, ActualLineWidth * 1.1);

			return new Rect(point.X - size / 2, point.Y - size / 2,
			                size, size);
		}

		private static void DrawTrackerRectangle(DrawingContext drawingContext, Rect rectangle)
		{
			// External rectangle
			drawingContext.DrawRectangle(ext_track_brush, null,
			                             new Rect(rectangle.Left + rectangle.Width / 6,
			                                      rectangle.Top + rectangle.Height / 6,
			                                      rectangle.Width * 2 / 3,
			                                      rectangle.Height * 2 / 3));

			// Middle
			drawingContext.DrawRectangle(int_track_brush, null,
			                             new Rect(rectangle.Left + rectangle.Width / 3,
			                                      rectangle.Top + rectangle.Height / 3,
			                                      rectangle.Width / 3,
			                                      rectangle.Height / 3));
		}

		public virtual void DrawTracker(DrawingContext drawingContext)
		{
			for (int i = 1; i <= HandleCount; i++)
			{
				DrawTrackerRectangle(drawingContext, GetHandleRectangle(i));
			}
		}

		public virtual void Draw(DrawingContext drawingContext)
		{
			if (IsSelected && IsTrackerOn)
			{
				DrawTracker(drawingContext);
			}
		}

		public void RefreshDrawing()
		{
			var dc = RenderOpen();
			Draw(dc);
			dc.Close();
		}
	}
}