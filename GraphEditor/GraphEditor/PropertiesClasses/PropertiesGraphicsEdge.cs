using System;
using System.Windows;
using System.Windows.Media;
using GraphEditor.Graphics;

namespace GraphEditor.PropertiesClasses
{
    /// <summary>
    /// Line object properties
    /// </summary>
    public class PropertiesGraphicsEdge : PropertiesGraphicsBase
    {
        private Point start;
        private Point end;
        private double lineWidth;

        public PropertiesGraphicsEdge()
        {

        }

        public PropertiesGraphicsEdge(GraphicsEdge line)
        {
            if ( line == null )
            {
                throw new ArgumentNullException("line");
            }

            start = line.Start;
            end = line.End;
            lineWidth = line.LineWidth;
            objectColor = line.ObjectColor;
            selectedColor = line.SelectedColor;
            actualScale = line.ActualScale;
	        textColor = line.TextColor;
            ID = line.Id;
            selected = line.IsSelected;
            label = line.Label;
        }

        public override GraphicsBase CreateGraphics()
        {
            GraphicsBase b = new GraphicsEdge(start, end, label, lineWidth, objectColor, selectedColor, textColor, actualScale, false);

            if (this.ID != 0)
            {
                b.Id = this.ID;
                b.IsSelected = this.selected;
            }

            return b;
        }

        #region Properties

        /// <summary>
        /// Start Point
        /// </summary>
        public Point Start
        {
            get { return start; }
            set { start = value; }
        }

        /// <summary>
        /// End Point
        /// </summary>
        public Point End
        {
            get { return end; }
            set { end = value; }
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

        public Color SelectedColor
        {
            get { return selectedColor; }
            set { selectedColor = value; }
        }

		public Color TextColor
		{
			get { return textColor; }
			set { textColor = value; }
		}

        public bool IsSelected
        {
            get { return selected; }
            set { selected = value; }
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
