using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using GraphEditor.Graphics;

namespace GraphEditor.Algorithms
{
	abstract class SelectionAlgorithm : Algorithm
	{
		protected SelectionAlgorithm(GraphCanvas drawingCanvas) : base(drawingCanvas)
		{
		}

		protected List<GraphicsBase> GetSelectedVertices()
		{
			return
				(from Visual graphic in DrawingCanvas.GraphicsList
				 where graphic is GraphicsVertex && (graphic as GraphicsVertex).IsSelected
				 select graphic as GraphicsVertex)
					.Cast<GraphicsBase>().ToList();
		}
	}
}