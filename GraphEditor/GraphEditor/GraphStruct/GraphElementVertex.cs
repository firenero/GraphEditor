using System;
using System.Collections.Generic;
using GraphEditor.PropertiesClasses;

namespace GraphEditor.GraphStruct
{
    public class GraphElementVertex : GraphElementBase
    {
        private String lable;
        private List<GraphElementEdge> connections;

        public GraphElementVertex()
        {
            ID = 0;
            Label = "";
            connections = new List<GraphElementEdge>();
        }

        public GraphElementVertex(String _lable, int _id)
        {
            ID = _id;
            Label = _lable;
            connections = new List<GraphElementEdge>();
        }

        public void AddConnection(GraphElementEdge edge)
        {
            connections.Add(edge);
        }
        public void RemoveConnection(GraphElementEdge edge)
        {
            connections.Remove(edge);
        }
        public String Label
        {
            get { return lable; }
            set { lable = value; }
        }

        public List<GraphElementEdge> Connections
        {
            get { return connections; }
        }

        public override PropertiesGraphBase CreateSerializedObject()
        {
            return new PropertiesGraphVertex(this);
        }

    }
}
