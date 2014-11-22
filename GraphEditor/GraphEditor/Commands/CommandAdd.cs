using GraphEditor.Graphics;
using GraphEditor.GraphStruct;
using GraphEditor.PropertiesClasses;

namespace GraphEditor.Commands
{
    class CommandAdd : CommandBase
    {
        PropertiesGraphicsBase newObjectClone;
        PropertiesGraphBase newElementClone;

       /* public CommandAdd(GraphicsBase newObject): base()
        {
            this.newObjectClone = newObject.CreateSerializedObject();
        }
        */
        public CommandAdd(GraphicsBase newObject, GraphElementBase newElement): base()
        {
            this.newObjectClone = newObject.CreateSerializedObject();
            this.newElementClone = newElement.CreateSerializedObject();
        }

        public override void Undo(GraphCanvas drawingCanvas)
        {
            drawingCanvas.UnselectAll();
            GraphicsBase objectToDelete = null;

            foreach (GraphicsBase b in drawingCanvas.GraphicsList)
            {
                if (b.Id == newObjectClone.id)
                {
                    objectToDelete = b;
                    break;
                }
            }

            if (objectToDelete != null)
            {
                drawingCanvas.GraphicsList.Remove(objectToDelete);
            }

            if (newElementClone is PropertiesGraphVertex)
                drawingCanvas.GraphStructure.RemoveVertex(newElementClone.id);
            else if(newElementClone is PropertiesGraphEdge)
                drawingCanvas.GraphStructure.RemoveConnection(newElementClone.id);

        }

        public override void Redo(GraphCanvas drawingCanvas)
        {
            drawingCanvas.UnselectAll();

            // Create full object from the clone and add it to list
            drawingCanvas.GraphicsList.Add(newObjectClone.CreateGraphics());
            drawingCanvas.GraphStructure.AddElement(newElementClone);
            // Object created from the clone doesn't contain clip information,
            // refresh it.
            drawingCanvas.RefreshClip();
        }

        public int ElementID
        {
            get { return newElementClone.id; }
        }

        public int GraphicsID
        {
            get { return newObjectClone.id; }
        }
    }
}
