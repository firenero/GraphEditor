using System.Linq;
using GraphEditor.Graphics;
using GraphEditor.GraphStruct;
using GraphEditor.PropertiesClasses;

namespace GraphEditor.Commands
{
	internal class CommandAdd : CommandBase
	{
		private PropertiesGraphBase newElementClone;
		private PropertiesGraphicsBase newObjectClone;

		public CommandAdd(GraphicsBase newObject, GraphElementBase newElement)
		{
			newObjectClone = newObject.CreateSerializedObject();
			newElementClone = newElement.CreateSerializedObject();
		}

		public int ElementId
		{
			get { return newElementClone.id; }
		}

		public int GraphicsId
		{
			get { return newObjectClone.id; }
		}

		public override void Undo(GraphCanvas drawingCanvas)
		{
			drawingCanvas.UnselectAll();
			var objectToDelete = drawingCanvas.GraphicsList.Cast<GraphicsBase>().FirstOrDefault(b => b.Id == newObjectClone.id);

			if (objectToDelete != null)
			{
				drawingCanvas.GraphicsList.Remove(objectToDelete);
			}

			if (newElementClone is PropertiesGraphVertex)
				drawingCanvas.GraphStructure.RemoveVertex(newElementClone.id);
			else if (newElementClone is PropertiesGraphEdge)
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
	}
}