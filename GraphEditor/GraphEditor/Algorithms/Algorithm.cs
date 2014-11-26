using System;
using System.Text;
using System.Threading.Tasks;

namespace GraphEditor.Algorithms
{
	public abstract class Algorithm
	{
		public GraphCanvas DrawingCanvas { get; private set; }

		protected abstract bool IsInputCorrect();

		protected abstract AlgorithmResult RunAlgorithm();

		public virtual double Execute()
		{
			if (IsInputCorrect())
			{
				HelperFunctions.UnselectAll(DrawingCanvas);
				var result = RunAlgorithm();

				DrawingCanvas.SelectElements(result.ItemsToSelect);
				return result.Cost;
			}
			else
				throw new ArgumentException("Algorithm input is not valid!");

		}

		protected Algorithm(GraphCanvas drawingCanvas)
		{
			DrawingCanvas = drawingCanvas;
		}
	}
}
