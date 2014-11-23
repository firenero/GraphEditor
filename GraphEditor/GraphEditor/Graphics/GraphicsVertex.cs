using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using GraphEditor.PropertiesClasses;

namespace GraphEditor.Graphics
{
    /// <summary>
    ///  Rectangle graphics object.
    /// </summary>
    public class GraphicsVertex : GraphicsRectangleBase
    {
        Point center;

        #region Constructors

        public GraphicsVertex(Point center, double radius, String label, double lineWidth, Color objectColor, Color selectedColor, Color textColor, double actualScale)
        {
			this.label = new FormattedText(label, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 16, new SolidColorBrush(textColor));
			//if (this.label.Width / 2 + 5 > radius)
			//{
			//	radius = this.label.Width / 2 + 5;
			//}

            this.rectangleTop = center.Y + radius;
            this.rectangleBottom = center.Y - radius;
            this.graphicsLineWidth = lineWidth;
            this.graphicsObjectColor = objectColor;
            this.SelectedColor = selectedColor;
	        this.TextColor = textColor;
            this.graphicsActualScale = actualScale;

            Rect r = Rectangle;
            this.center = center;

            

            this.rectangleLeft = center.X + radius;
            this.rectangleRight = center.X - radius;
        }

        public GraphicsVertex(double left, double top, double right, double bottom, String label, double lineWidth, Color objectColor, Color selectedColor, Color textColor, double actualScale)
        {
			this.label = new FormattedText(label, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 16, new SolidColorBrush(textColor));
			//if (this.label.Width / 2 + 5 > (right - left) / 2)
			//{
			//	double radius = this.label.Width / 2 + 5;
			//	this.rectangleLeft = center.X + radius;
			//	this.rectangleRight = center.X - radius;
			//}

            this.rectangleLeft = left;
            this.rectangleTop = top;
            this.rectangleRight = right;
            this.rectangleBottom = bottom;
            this.graphicsLineWidth = lineWidth;
            this.graphicsObjectColor = objectColor;
            this.SelectedColor = selectedColor;
			this.TextColor = textColor;
            this.graphicsActualScale = actualScale;

            Rect r = Rectangle;
            center = new Point(
                (r.Left + r.Right) / 2.0,
                (r.Top + r.Bottom) / 2.0);
          

        }

        public GraphicsVertex()
            :
            this(0.0, 0.0, 100.0, 100.0, "", 1.0, Colors.Black, Colors.Red, Colors.Black, 1.0)
        {
        }

        #endregion Constructors
        public Point Center
        {
            get
            {
                return center;
            }
        }
        
        public void SetSize(double defRadius, int count, double val)
        {
			//if (this.label.Width / 2 + 5 > def_radius)
			//{
			//	def_radius = this.label.Width / 2 + 5;
			//}

            this.rectangleTop = center.Y - defRadius;
            this.rectangleBottom = center.Y + defRadius;

            this.rectangleLeft = center.X - defRadius;
            this.rectangleRight = center.X + defRadius;
            for (int i = 0; i < count; i++)
                IncSize(val);
            if (count == 0) RefreshDrawing();
        }
        public void IncSize(double val)
        {
           // double radiusX = (this.rectangleRight - this.rectangleLeft) / 2.0;
           // double radiusY = (this.rectangleBottom - this.rectangleTop) / 2.0;
            //if(radiusY == radiusX)
            //{
                this.rectangleLeft -= val / 2;
                this.rectangleRight += val / 2;
                this.rectangleTop -= val / 2;
                this.rectangleBottom += val / 2;
            //}
           /* else if (radiusY > radiusX)
            {
                this.rectangleTop -= val / 2;
                this.rectangleBottom += val / 2;
                double tmp = Math.Abs((this.rectangleBottom - this.rectangleTop) / 2.0 - radiusX);
                this.rectangleLeft -= tmp;
                this.rectangleRight += tmp;
            }
            else
            {
                this.rectangleTop -= val / 2;
                this.rectangleBottom += val / 2;
                radiusY = (this.rectangleBottom - this.rectangleTop) / 2.0;
                if (radiusY > radiusX)
                {
                    double tmp = Math.Abs(radiusY - radiusX);
                    this.rectangleLeft -= tmp;
                    this.rectangleRight += tmp;
                }
            }*/
            RefreshDrawing();
        }
        public void DecSize(double val)
        {
            IncSize(-val);
        }
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
            Normalize();
            /*if (this.label.Width / 2 + 5 > rectangleRight - rectangleLeft / 2)
            {
                double radius = this.label.Width / 2 + 5;
                this.rectangleLeft = center.X + radius;
                this.rectangleRight = center.X - radius;
            }*/

            Rect r = Rectangle;

            center = new Point(
                (r.Left + r.Right) / 2.0,
                (r.Top + r.Bottom) / 2.0);

            double radiusX = (r.Right - r.Left) / 2.0;
            double radiusY = (r.Bottom - r.Top) / 2.0;
            if (IsSelected) this.label.SetForegroundBrush(new SolidColorBrush(SelectedColor));
            else this.label.SetForegroundBrush(new SolidColorBrush(TextColor));

            // drawingContext.DrawRectangle(new SolidColorBrush(ObjectColor), new Pen(), new Rect(new Point(center.X - radiusX * 0.8, center.Y - radiusY * 0.8), new Size(radiusX * 1.6, radiusY * 1.6)));

			drawingContext.DrawEllipse(new SolidColorBrush(ObjectColor), new Pen(new SolidColorBrush(CurrentColor), ActualLineWidth), center, radiusX, radiusY);
            drawingContext.DrawText(this.label, new Point(center.X - this.label.Width / 2, center.Y + radiusY));
            
            base.Draw(drawingContext);
        }

        public double GetRadius(Point A, Point B)
        {
            //double fi = Math.Atan2(B.Y - A.Y, B.X - A.X);
            Rect rect = Rectangle;
            double a = (rect.Right - rect.Left) / 2.0;
            //double b = (rect.Bottom - rect.Top) / 2.0;
            return a;// (a * b) / Math.Sqrt(b * b * Math.Cos(fi) * Math.Cos(fi) + a * a * Math.Sin(fi) * Math.Sin(fi));
        }

        /// <summary>
        /// Test whether object contains point
        /// </summary>
        public override bool Contains(Point point)
        {
            if ( IsSelected )
            {
                return this.Rectangle.Contains(point);
            }
            else
            {
                EllipseGeometry g = new EllipseGeometry(Rectangle);

                return g.FillContains(point) || g.StrokeContains(new Pen(Brushes.Black, ActualLineWidth), point);
            }
        }

        /// <summary>
        /// Test whether object intersects with rectangle
        /// </summary>
        public override bool IntersectsWith(Rect rectangle)
        {
            RectangleGeometry rg = new RectangleGeometry(rectangle);    // parameter
            EllipseGeometry eg = new EllipseGeometry(Rectangle);        // this object rectangle

            PathGeometry p = Geometry.Combine(rg, eg, GeometryCombineMode.Intersect, null);

            return (!p.IsEmpty());
        }


        /// <summary>
        /// Serialization support
        /// </summary>
        public override PropertiesGraphicsBase CreateSerializedObject()
        {
            return new PropertiesGraphicsVertex(this);
        }

        #endregion Overrides

    }
}