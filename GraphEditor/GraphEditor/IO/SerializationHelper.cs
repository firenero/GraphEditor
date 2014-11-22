using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Xml.Serialization;
using GraphEditor.Graphics;
using GraphEditor.GraphStruct;
using GraphEditor.PropertiesClasses;

namespace GraphEditor.IO
{

    [XmlRoot("Graphics")]
    public class SerializationHelper
    {
        PropertiesGraphicsBase[] graphics;
        PropertiesGraphBase[] elements;

        public SerializationHelper()
        {

        }

        public SerializationHelper(VisualCollection collection, GraphStruct.GraphStruct graph)
        {
            if ( collection == null )
                throw new ArgumentNullException("VisualCollection");
            if (graph == null)
                throw new ArgumentNullException("GraphStruct");

            graphics = new PropertiesGraphicsBase[collection.Count];
            int i = 0;
            foreach (GraphicsBase g in collection)
                graphics[i++] = g.CreateSerializedObject();
            List<PropertiesGraphBase> tmp = new List<PropertiesGraphBase>();
            foreach (GraphElementVertex v in graph.Vertices)
                tmp.Add(v.CreateSerializedObject());

            foreach (GraphElementVertex v in graph.Vertices)
                foreach (GraphElementEdge e in v.Connections)
                {
                    bool present = false;
                    foreach (var el in tmp)
                        if (e.ID == el.id)
                        {
                            present = true;
                            break;
                        }
                    if (!present) tmp.Add(e.CreateSerializedObject());
                }

            elements = new PropertiesGraphBase[tmp.Count];
            i = 0;
            foreach (var el in tmp)
                elements[i++] = el;
        }

        [XmlArrayItem(typeof(PropertiesGraphicsVertex)),
         XmlArrayItem(typeof(PropertiesGraphicsEdge)),]
        public PropertiesGraphicsBase[] Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }

        [XmlArrayItem(typeof(PropertiesGraphVertex)),
         XmlArrayItem(typeof(PropertiesGraphEdge)),]
        public PropertiesGraphBase[] Elements
        {
            get { return elements; }
            set { elements = value; }
        }
    }
}
