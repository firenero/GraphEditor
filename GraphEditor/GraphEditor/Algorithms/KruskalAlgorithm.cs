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

		protected override bool IsInputCorrect(out string message)
		{
			if (DrawingCanvas.GraphStructure.Vertices.Count == 0)
			{
				message = "No vertices in graph.";
				return false;
			}

			message = "Graph must be connected.";
			return SameTreeChecking().All(pair => pair.Value);
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