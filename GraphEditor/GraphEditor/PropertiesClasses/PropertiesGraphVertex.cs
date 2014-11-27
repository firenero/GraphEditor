using System;
using GraphEditor.GraphStruct;

namespace GraphEditor.PropertiesClasses
{
	public class PropertiesGraphVertex : PropertiesGraphBase
	{
		public PropertiesGraphVertex()
		{
			Label = String.Empty;
		}

		public PropertiesGraphVertex(GraphElementVertex v)
		{
			if (v == null)
			{
				throw new ArgumentNullException("GraphElementVertex");
			}
			id = v.ID;
			Label = v.Label;
		}

		public string Label { get; set; }

		public int Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}