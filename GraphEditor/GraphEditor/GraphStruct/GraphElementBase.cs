using GraphEditor.PropertiesClasses;

namespace GraphEditor.GraphStruct
{
	public abstract class GraphElementBase
	{
		public int ID { get; set; }

		public abstract PropertiesGraphBase CreateSerializedObject();
	}
}