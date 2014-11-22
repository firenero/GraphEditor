using System;
using GraphEditor.PropertiesClasses;

namespace GraphEditor.GraphStruct
{
    public class GraphElementEdge : GraphElementBase
    {
        private String weight;
        private GraphElementVertex begin;
        private GraphElementVertex end;

        public GraphElementEdge()
        {
            ID = 0;
            Weight = "";
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

        public String Weight
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
