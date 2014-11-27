using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using GraphEditor.Graphics;
using GraphEditor.PropertiesClasses;

namespace GraphEditor.Commands
{
	internal class CommandChangeState : CommandBase
	{
		// Selected object(s) before operation

		// Selected object(s) after operation
		private List<PropertiesGraphicsBase> listAfter;
		private List<PropertiesGraphicsBase> listBefore;
		private List<PropertiesGraphBase> listElAfter;
		private List<PropertiesGraphBase> listElBefore;

		public CommandChangeState(GraphCanvas drawingCanvas)
		{
			// Keep objects state before operation.
			FillList(drawingCanvas.GraphicsList, drawingCanvas.GraphStructure, ref listBefore, ref listElBefore);
		}

		// Fill list from selection
		private static void FillList(VisualCollection graphicsList, GraphStruct.GraphStruct graph, ref List<PropertiesGraphicsBase> listToFill,
		                             ref List<PropertiesGraphBase> listToFillEl)
		{
			listToFill = new List<PropertiesGraphicsBase>();
			listToFillEl = new List<PropertiesGraphBase>();

			foreach (GraphicsBase g in graphicsList)
			{
				if (g.IsSelected)
				{
					listToFill.Add(g.CreateSerializedObject());
					var a = graph.GetElement(g.Id);
					var b = a.CreateSerializedObject();
					listToFillEl.Add(b);
				}
			}
		}

		// Replace objects in graphicsList with objects from clone list
		private static void ReplaceObjects(VisualCollection graphicsList, GraphStruct.GraphStruct graph, List<PropertiesGraphicsBase> list,
		                                   IEnumerable<PropertiesGraphBase> listEl)
		{
			for (int i = 0; i < graphicsList.Count; i++)
			{
				var replacement = list.FirstOrDefault(o => o.id == ((GraphicsBase) graphicsList[i]).Id);

				if (replacement != null)
				{
					// Replace object with its clone
					graphicsList.RemoveAt(i);
					graphicsList.Insert(i, replacement.CreateGraphics());
				}
			}

			foreach (var el in listEl)
				graph.ChangeState(el);
		}

		// Create this command BEFORE operation.

		// Call this function AFTER operation.
		public void NewState(GraphCanvas drawingCanvas)
		{
			// Keep objects state after operation.
			FillList(drawingCanvas.GraphicsList, drawingCanvas.GraphStructure, ref listAfter, ref listElAfter);
		}

		/// <summary>
		///     Restore selection to its state before change.
		/// </summary>
		public override void Undo(GraphCanvas drawingCanvas)
		{
			drawingCanvas.UnselectAll();
			// Replace all objects in the list with objects from listBefore
			ReplaceObjects(drawingCanvas.GraphicsList, drawingCanvas.GraphStructure, listBefore, listElBefore);

			drawingCanvas.RefreshClip();
		}

		/// <summary>
		///     Restore selection to its state after change.
		/// </summary>
		public override void Redo(GraphCanvas drawingCanvas)
		{
			// Replace all objects in the list with objects from listAfter
			drawingCanvas.UnselectAll();
			ReplaceObjects(drawingCanvas.GraphicsList, drawingCanvas.GraphStructure, listAfter, listElAfter);

			drawingCanvas.RefreshClip();
		}
	}
}