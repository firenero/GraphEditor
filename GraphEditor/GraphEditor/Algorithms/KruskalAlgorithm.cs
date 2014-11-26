using System.Collections.Generic;
using System.Linq;
using GraphEditor.Graphics;
using GraphEditor.GraphStruct;

namespace GraphEditor.Algorithms
{
	class KruskalAlgorithm : Algorithm
	{
		public KruskalAlgorithm(GraphCanvas drawingCanvas) : base(drawingCanvas)
		{
		}

		protected override bool IsInputCorrect()
		{
			if (DrawingCanvas.GraphStructure.Vertices.Count == 0)
				return false;

			var vertices = DrawingCanvas.GraphStructure.Vertices;
			var verticesCount = vertices.Count;
			var startVertex = DrawingCanvas.GraphStructure.Vertices[0];
			var queue = new Queue<GraphElementVertex>();
			queue.Enqueue(startVertex);
			var isUsed = new Dictionary<GraphElementVertex, bool>(verticesCount);
			for (int i = 0; i < verticesCount; i++)
			{
				isUsed.Add(DrawingCanvas.GraphStructure.Vertices[i], false);
			}


			isUsed[startVertex] = true;
			var resultVertexes = new List<GraphElementVertex>() { startVertex };
			var resultEdges = new HashSet<GraphElementEdge>();
			while (queue.Count != 0)
			{
				var vertex = queue.Dequeue();
				foreach (GraphElementEdge edge in vertex.Connections)
				{
					var to = edge.Begin == vertex ? edge.End : edge.Begin;
					if (!isUsed[to])
					{
						isUsed[to] = true;
						queue.Enqueue(to);

						resultVertexes.Add(to);
						resultEdges.Add(edge);
					}
				}
			}
			return isUsed.All(pair => pair.Value);
		}

		protected override AlgorithmResult RunAlgorithm()
		{
			var edges = DrawingCanvas.GraphStructure.GetAllEdges().ToList();
			var sum = 0.0;
			var treeId = new Dictionary<GraphElementVertex, int>(DrawingCanvas.GraphStructure.Vertices.Count);
			var resultEdges = new List<GraphElementBase>();
			var resultVertexes = new HashSet<GraphElementBase>();
			for (int i = 0; i < DrawingCanvas.GraphStructure.Vertices.Count; i++)
			{
				treeId.Add(DrawingCanvas.GraphStructure.Vertices[i], i);
			}
			for (int i = 0; i < edges.Count; i++)
			{
				GraphElementVertex begin = edges[i].Begin, end = edges[i].End;
				var weight = edges[i].Weight;
				if (treeId[begin] != treeId[end])
				{
					sum += weight;
					resultEdges.Add(edges[i]);
					resultVertexes.Add(begin);
					resultVertexes.Add(end);
					int oldId = treeId[end], newId = treeId[begin];
					for (int j = 0; j < DrawingCanvas.GraphStructure.Vertices.Count; ++j)
						if (treeId[DrawingCanvas.GraphStructure.Vertices[j]] == oldId)
							treeId[DrawingCanvas.GraphStructure.Vertices[j]] = newId;
				}
			}
			resultEdges.AddRange(resultVertexes);
			return new AlgorithmResult(sum, resultEdges);
		}

	}
}