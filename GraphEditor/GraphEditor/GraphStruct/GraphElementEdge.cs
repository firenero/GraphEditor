using GraphEditor.PropertiesClasses;

namespace GraphEditor.GraphStruct
{
	public class GraphElementEdge : GraphElementBase
	{
		public GraphElementEdge()
		{
			ID = 0;
			Weight = 0.0;
			Begin = End = null;
		}

		public GraphElementVertex Begin { get; set; }

		public GraphElementVertex End { get; set; }

		public double Weight { get; set; }

		public override PropertiesGraphBase CreateSerializedObject()
		{
			return new PropertiesGraphEdge(this);
		}
	}
}