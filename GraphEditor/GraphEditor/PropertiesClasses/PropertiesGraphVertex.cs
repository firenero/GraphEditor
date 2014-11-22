using System;
using GraphEditor.GraphStruct;

namespace GraphEditor.PropertiesClasses
{
    public class PropertiesGraphVertex : PropertiesGraphBase
    {
        private String lable;

        public PropertiesGraphVertex() { lable = String.Empty; }

        public PropertiesGraphVertex(GraphElementVertex v) 
        {
            if (v == null)
            {
                throw new ArgumentNullException("GraphElementVertex");
            }
            id = v.ID;
            lable = v.Label;
        }

        public String Label
        {
            get { return lable; }
            set { lable = value; }
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

    }
}
