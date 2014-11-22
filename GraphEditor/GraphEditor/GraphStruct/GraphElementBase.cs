using GraphEditor.PropertiesClasses;

namespace GraphEditor.GraphStruct
{
    public abstract class GraphElementBase
    {
        private int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public abstract PropertiesGraphBase CreateSerializedObject();
    }
}
