using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GraphEditor.Graphics;
using GraphEditor.PropertiesClasses;

namespace GraphEditor.Commands
{
	internal class CommandDelete : CommandBase
	{
		private List<PropertiesGraphBase> cloneGraphList;
		private List<PropertiesGraphicsBase> cloneList; // contains selected items which are deleted
		private List<int> indexes; // contains indexes of deleted items

		// Create this command BEFORE applying Delete function.
		public CommandDelete(GraphCanvas drawingCanvas)
		{
			cloneList = new List<PropertiesGraphicsBase>();
			cloneGraphList = new List<PropertiesGraphBase>();
			indexes = new List<int>();

			// Make clone of the list selection.

			int currentIndex = 0;

			foreach (var g in drawingCanvas.Selection)
			{
				cloneList.Add(g.CreateSerializedObject());
				cloneGraphList.Add(drawingCanvas.GraphStructure.GetElement(g.Id).CreateSerializedObject());
				indexes.Add(currentIndex);
				currentIndex++;
			}
		}

		/// <summary>
		///     Restore deleted objects
		/// </summary>
		public override void Undo(GraphCanvas drawingCanvas)
		{
			drawingCanvas.UnselectAll();
			// Insert all objects from cloneList to GraphicsList

			int currentIndex = 0;

			foreach (var val in cloneGraphList)
				if (val is PropertiesGraphVertex)
					drawingCanvas.GraphStructure.AddElement(val);

			foreach (var val in cloneGraphList)
				if (val is PropertiesGraphEdge)
					drawingCanvas.GraphStructure.AddElement(val);

			foreach (var o in cloneList)
			{
				int indexToInsert = indexes[currentIndex];

				if (indexToInsert >= 0 && indexToInsert <= drawingCanvas.GraphicsList.Count) // "<=" is correct !
				{
					drawingCanvas.GraphicsList.Insert(indexToInsert, o.CreateGraphics());
				}
				else
				{
					// Bug: we should not be here.
					// Add to the end anyway.
					drawingCanvas.GraphicsList.Add(o.CreateGraphics());
					Trace.WriteLine("CommandDelete.Undo - incorrect index");
				}

				currentIndex++;
			}


			drawingCanvas.RefreshClip();
		}

		/// <summary>
		///     Delete objects again.
		/// </summary>
		public override void Redo(GraphCanvas drawingCanvas)
		{
			drawingCanvas.UnselectAll();
			// Delete from list all objects kept in cloneList.
			// Use object IDs for deleting, don't beleive to objects order.

			int n = drawingCanvas.GraphicsList.Count;

			for (int i = n - 1; i >= 0; i--)
			{
				var currentObject = (GraphicsBase) drawingCanvas.GraphicsList[i];

				bool toDelete = cloneList.Any(o => o.id == currentObject.Id);

				if (toDelete)
				{
					drawingCanvas.GraphicsList.RemoveAt(i);
				}
			}

			foreach (var p in cloneGraphList)
				if (p is PropertiesGraphVertex)
					drawingCanvas.GraphStructure.RemoveVertex(p.id);
				else if (p is PropertiesGraphEdge)
					drawingCanvas.GraphStructure.RemoveConnection(p.id);
		}
	}
}