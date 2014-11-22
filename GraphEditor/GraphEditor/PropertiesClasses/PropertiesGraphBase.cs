using System.Xml.Serialization;

namespace GraphEditor.PropertiesClasses
{
    public abstract class PropertiesGraphBase
    {
        [XmlIgnore]
        internal int id = 0;
    }
}
