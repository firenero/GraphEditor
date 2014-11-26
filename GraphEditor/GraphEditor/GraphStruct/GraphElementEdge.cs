using System;
using GraphEditor.PropertiesClasses;

namespace GraphEditor.GraphStruct
{
    public class GraphElementEdge : GraphElementBase
    {
        private double weight;
        private GraphElementVertex begin;
        private GraphElementVertex end;

        public GraphElementEdge()
        {
            ID = 0;
            Weight = 0.0;
            Begin = End = null;
        }

        public GraphElementVertex Begin
        {
            get { return begin; }
            set { begin = value; }
        }

        public GraphElementVertex End
        {
            get { return end; }
            set { end = value; }
        }

        public double Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        public override PropertiesGraphBase CreateSerializedObject()
        {
            return new PropertiesGraphEdge(this);
        }
    }
}
