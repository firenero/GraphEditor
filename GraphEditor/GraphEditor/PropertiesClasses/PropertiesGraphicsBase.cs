using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Media;

namespace GraphLib
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

        public abstract GraphicsBase CreateGraphics();
    }
}
