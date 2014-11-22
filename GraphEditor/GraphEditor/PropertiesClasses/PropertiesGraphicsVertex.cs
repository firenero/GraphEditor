using System;
using System.Windows.Media;
using GraphEditor.Graphics;

namespace GraphEditor.PropertiesClasses
{
    /// <summary>
    /// Ellipse object properties.
    /// </summary>
    public class PropertiesGraphicsVertex : PropertiesGraphicsBase
    {
        private double left;
        private double top;
        private double right;
        private double bottom;
        private double lineWidth;

        public PropertiesGraphicsVertex()
        {

        }

        public PropertiesGraphicsVertex(GraphicsVertex ellipse)
        {
            if (ellipse == null)
            {
                throw new ArgumentNullException("ellipse");
            }

            left = ellipse.Left;
            top = ellipse.Top;
            right = ellipse.Right;
            bottom = ellipse.Bottom;
            label = ellipse.Label;

            lineWidth = ellipse.LineWidth;
            objectColor = ellipse.ObjectColor;
            selectedColor = ellipse.SelectedColor;
            actualScale = ellipse.ActualScale;
            id = ellipse.Id;
            selected = ellipse.IsSelected;
        }

        public override GraphicsBase CreateGraphics()
        {
            GraphicsBase b =  new GraphicsVertex(left, top, right, bottom, label, lineWidth, objectColor, selectedColor, actualScale);
            if ( this.id != 0 )
            {
                b.Id = this.id;
                b.IsSelected = this.selected;
            }

            return b;
        }

        #region Properties

        /// <summary>
        /// Left bounding rectangle side, X
        /// </summary>
        public double Left
        {
            get { return left; }
            set { left = value; }
        }

        /// <summary>
        /// Top bounding rectangle side, Y
        /// </summary>
        public double Top
        {
            get { return top; }
            set { top = value; }
        }

        /// <summary>
        /// Right bounding rectangle side, X
        /// </summary>
        public double Right
        {
            get { return right; }
            set { right = value; }
        }

        /// <summary>
        /// Bottom bounding rectangle side, Y
        /// </summary>
        public double Bottom
        {
            get { return bottom; }
            set { bottom = value; }
        }

        /// <summary>
        /// Line Width
        /// </summary>
        public double LineWidth
        {
            get { return lineWidth; }
            set { lineWidth = value; }
        }

        /// <summary>
        /// Color
        /// </summary>
        public Color ObjectColor
        {
            get { return objectColor; }
            set { objectColor = value; }
        }

        /// <summary>
        /// SelectedColor
        /// </summary>
        public Color SelectedColor
        {
            get { return selectedColor; }
            set { selectedColor = value; }
        }

        public String Label
        {
            get { return label; }
            set { label = value; }
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        #endregion Properties
    }
}
