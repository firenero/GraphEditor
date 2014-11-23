using System;
using System.Windows.Media;
using System.Xml.Serialization;
using GraphEditor.Graphics;

namespace GraphEditor.PropertiesClasses
{
    public abstract class PropertiesGraphicsBase
    {
        [XmlIgnore]
        internal int id;

        [XmlIgnore]
        internal bool selected;

        [XmlIgnore]
        internal double actualScale;

        [XmlIgnore]
        internal String label;
       
        [XmlIgnore]
        internal Color objectColor;

        [XmlIgnore]
        internal Color selectedColor;

	    [XmlIgnore] 
		internal Color textColor;

        public abstract GraphicsBase CreateGraphics();
    }
}
