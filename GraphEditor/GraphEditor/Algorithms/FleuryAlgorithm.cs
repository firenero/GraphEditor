using System.Collections.Generic;
using System.Linq;
using GraphEditor.GraphStruct;

namespace GraphEditor.Algorithms
{
	internal class FleuryAlgorithm : Algorithm
	{
		public FleuryAlgorithm(GraphCanvas drawingCanvas) : base(drawingCanvas)
		{
		}

		protected override bool IsInputCorrect(out string message)
		{
			if (DrawingCanvas.IsOrientedGraph)
			{
				message = "Algorithms are available only for not-oriented graphs.";
				return false;
			}
			if (!SameTreeChecking().All(pair => pair.Value))
			{
				message = "Graph must be connected.";
				return false;
			}
			message = "There are no Eulerian cycle in the graph.";
			return DrawingCanvas.GraphStructure.Vertices.All(vertex => (vertex.Connections.Count % 2 == 0));
		}

		protected override AlgorithmResult RunAlgorithm()
		{
			var resultVertices = new List<GraphElementVertex>();
			var resultEdges = new HashSet<GraphElementEdge>();

			var edges = DrawingCanvas.GraphStructure.GetAllEdges().ToList();
			var isUsed = new Dictionary<GraphElementEdge, bool>(edges.Count);
			foreach (var edge in edges)
			{
				isUsed.Add(edge, false);
			}
			var stack = new Stack<GraphElementVertex>();
			stack.Push(DrawingCanvas.GraphStructure.Vertices[0]);
			while (stack.Count != 0)
			{
				var vertex = stack.Peek();
				if (GetUnusedConnections(vertex, isUsed) == 0)
				{
					resultVertices.Add(vertex);
					stack.Pop();
				}
				else
				{
					foreach (var edge in vertex.Connections)
					{
						if (!isUsed[edge])
						{
							isUsed[edge] = true;
							stack.Push(edge.Begin == vertex ? edge.End : edge.Begin);
							break;
						}
					}
				}
			}
			for (int i = 1; i < resultVertices.Count; i++)
			{
				resultEdges.Add(DrawingCanvas.GraphStructure.GetEdgeBetweenVertex(resultVertices[i - 1], resultVertices[i]));
			}
			var result = new List<GraphElementBase>(resultVertices);
			result.AddRange(resultEdges);
			return new AlgorithmResult(DrawingCanvas.GraphStructure.Vertices.Count, result);
		}

		private int GetUnusedConnections(GraphElementVertex vertex, Dictionary<GraphElementEdge, bool> isUsed)
		{
			return vertex.Connections.Count(edge => !isUsed[edge]);
		}
	}
}