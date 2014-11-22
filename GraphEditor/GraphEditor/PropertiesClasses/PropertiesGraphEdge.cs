using System;
using GraphEditor.GraphStruct;

namespace GraphEditor.PropertiesClasses
{
    public class PropertiesGraphEdge : PropertiesGraphBase
    {
        private String weight;
        private int begin;
        private int end;

        public PropertiesGraphEdge() { }

        public PropertiesGraphEdge(GraphElementEdge e) 
        {
            if (e == null)
            {
                throw new ArgumentNullException("GraphElementVertex");
            }
            id = e.ID;
            weight = e.Weight;
            begin = e.Begin.ID;
            end = e.End.ID;
        }

        public String Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        public int Begin
        {
            get { return begin; }
            set { begin = value; }
        }

        public int End
        {
            get { return end; }
            set { end = value; }
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
    }
}
