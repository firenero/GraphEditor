using System;
using GraphEditor.GraphStruct;

namespace GraphEditor.PropertiesClasses
{
	public class PropertiesGraphEdge : PropertiesGraphBase
	{
		public PropertiesGraphEdge()
		{
		}

		public PropertiesGraphEdge(GraphElementEdge e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("GraphElementVertex");
			}
			id = e.ID;
			Weight = e.Weight;
			Begin = e.Begin.ID;
			End = e.End.ID;
		}

		public double Weight { get; set; }

		public int Begin { get; set; }

		public int End { get; set; }

		public int Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}