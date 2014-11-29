using System;
using System.Collections.Generic;
using GraphEditor.GraphStruct;

namespace GraphEditor.Algorithms
{
	public abstract class Algorithm
	{
		protected Algorithm(GraphCanvas drawingCanvas)
		{
			DrawingCanvas = drawingCanvas;
		}

		protected GraphCanvas DrawingCanvas { get; private set; }

		protected abstract bool IsInputCorrect(out string message);

		protected abstract AlgorithmResult RunAlgorithm();

		public virtual double Execute()
		{
			string message = "";
			if (IsInputCorrect(out message))
			{
				var result = RunAlgorithm();
				HelperFunctions.UnselectAll(DrawingCanvas);
				DrawingCanvas.SelectElements(result.ItemsToSelect);
				return result.Cost;
			}
			throw new ArgumentException(message);
		}

		protected Dictionary<GraphElementVertex, bool> SameTreeChecking()
		{
			var vertices = DrawingCanvas.GraphStructure.Vertices;
			int verticesCount = vertices.Count;
			var startVertex = DrawingCanvas.GraphStructure.Vertices[0];
			var queue = new Queue<GraphElementVertex>();
			queue.Enqueue(startVertex);
			var isUsed = new Dictionary<GraphElementVertex, bool>(verticesCount);
			for (int i = 0; i < verticesCount; i++)
			{
				isUsed.Add(DrawingCanvas.GraphStructure.Vertices[i], false);
			}


			isUsed[startVertex] = true;
			while (queue.Count != 0)
			{
				var vertex = queue.Dequeue();
				foreach (var edge in vertex.Connections)
				{
					var to = edge.Begin == vertex ? edge.End : edge.Begin;
					if (!isUsed[to])
					{
						isUsed[to] = true;
						queue.Enqueue(to);
					}
				}
			}
			return isUsed;
		}
	}
}