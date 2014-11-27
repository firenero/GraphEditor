using System;
using System.Collections.Generic;
using GraphEditor.PropertiesClasses;

namespace GraphEditor.GraphStruct
{
	public class GraphElementVertex : GraphElementBase
	{
		private List<GraphElementEdge> connections;

		public GraphElementVertex()
		{
			ID = 0;
			Label = "";
			connections = new List<GraphElementEdge>();
		}

		public GraphElementVertex(String lable, int id)
		{
			ID = id;
			Label = lable;
			connections = new List<GraphElementEdge>();
		}

		public string Label { get; set; }

		public List<GraphElementEdge> Connections
		{
			get { return connections; }
		}

		public void AddConnection(GraphElementEdge edge)
		{
			connections.Add(edge);
		}

		public void RemoveConnection(GraphElementEdge edge)
		{
			connections.Remove(edge);
		}

		public override PropertiesGraphBase CreateSerializedObject()
		{
			return new PropertiesGraphVertex(this);
		}
	}
}