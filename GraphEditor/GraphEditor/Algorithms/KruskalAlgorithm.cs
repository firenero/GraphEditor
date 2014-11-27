using System.Collections.Generic;
using System.Linq;
using GraphEditor.GraphStruct;

namespace GraphEditor.Algorithms
{
	internal class KruskalAlgorithm : Algorithm
	{
		public KruskalAlgorithm(GraphCanvas drawingCanvas) : base(drawingCanvas)
		{
		}

		protected override bool IsInputCorrect(out string message)
		{
			if (DrawingCanvas.IsOrientedGraph)
			{
				message = "Algorithms are available only for not-oriented graphs.";
				return false;
			}
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
			double sum = 0.0;
			var treeId = new Dictionary<GraphElementVertex, int>(DrawingCanvas.GraphStructure.Vertices.Count);
			var resultEdges = new List<GraphElementBase>();
			var resultVertexes = new HashSet<GraphElementBase>();
			for (int i = 0; i < DrawingCanvas.GraphStructure.Vertices.Count; i++)
			{
				treeId.Add(DrawingCanvas.GraphStructure.Vertices[i], i);
			}
			foreach (var edge in edges)
			{
				GraphElementVertex begin = edge.Begin, end = edge.End;
				double weight = edge.Weight;
				if (treeId[begin] != treeId[end])
				{
					sum += weight;
					resultEdges.Add(edge);
					resultVertexes.Add(begin);
					resultVertexes.Add(end);
					int oldId = treeId[end], newId = treeId[begin];
					foreach (var vertex in DrawingCanvas.GraphStructure.Vertices)
						if (treeId[vertex] == oldId)
							treeId[vertex] = newId;
				}
			}
			resultEdges.AddRange(resultVertexes);
			return new AlgorithmResult(sum, resultEdges);
		}
	}
}