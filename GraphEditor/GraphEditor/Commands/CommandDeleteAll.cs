using System.Collections.Generic;
using GraphEditor.Graphics;
using GraphEditor.PropertiesClasses;

namespace GraphEditor.Commands
{
	internal class CommandDeleteAll : CommandBase
	{
		private List<PropertiesGraphicsBase> cloneList;

		// Create this command BEFORE applying Delete All function.
		public CommandDeleteAll(GraphCanvas drawingCanvas)
		{
			cloneList = new List<PropertiesGraphicsBase>();

			// Make clone of the whole list.
			foreach (GraphicsBase g in drawingCanvas.GraphicsList)
			{
				cloneList.Add(g.CreateSerializedObject());
			}
		}

		/// <summary>
		///     Add all deleted objects to GraphicsList
		/// </summary>
		public override void Undo(GraphCanvas drawingCanvas)
		{
			foreach (var o in cloneList)
			{
				drawingCanvas.GraphicsList.Add(o.CreateGraphics());
			}

			drawingCanvas.RefreshClip();
		}

		/// <summary>
		///     Detete All again
		/// </summary>
		public override void Redo(GraphCanvas drawingCanvas)
		{
			drawingCanvas.GraphicsList.Clear();
		}
	}
}