using System;
using System.Linq;
using System.Windows.Media;
using System.Xml.Serialization;
using GraphEditor.Graphics;
using GraphEditor.PropertiesClasses;

namespace GraphEditor.IO
{
	[XmlRoot("Graphics")]
	public class SerializationHelper
	{
		private PropertiesGraphBase[] elements;
		private PropertiesGraphicsBase[] graphics;

		public SerializationHelper()
		{
		}

		public SerializationHelper(VisualCollection collection, GraphStruct.GraphStruct graph)
		{
			if (collection == null)
				throw new ArgumentNullException("VisualCollection");
			if (graph == null)
				throw new ArgumentNullException("GraphStruct");

			graphics = new PropertiesGraphicsBase[collection.Count];
			int i = 0;
			foreach (GraphicsBase g in collection)
				graphics[i++] = g.CreateSerializedObject();
			var tmp = graph.Vertices.Select(v => v.CreateSerializedObject()).ToList();

			foreach (var v in graph.Vertices)
				foreach (var e in v.Connections)
				{
					bool present = tmp.Any(el => e.ID == el.id);
					if (!present) tmp.Add(e.CreateSerializedObject());
				}

			elements = new PropertiesGraphBase[tmp.Count];
			i = 0;
			foreach (var el in tmp)
				elements[i++] = el;
		}

		[XmlArrayItem(typeof (PropertiesGraphicsVertex)),
		 XmlArrayItem(typeof (PropertiesGraphicsEdge)),]
		public PropertiesGraphicsBase[] Graphics
		{
			get { return graphics; }
			set { graphics = value; }
		}

		[XmlArrayItem(typeof (PropertiesGraphVertex)),
		 XmlArrayItem(typeof (PropertiesGraphEdge)),]
		public PropertiesGraphBase[] Elements
		{
			get { return elements; }
			set { elements = value; }
		}
	}
}