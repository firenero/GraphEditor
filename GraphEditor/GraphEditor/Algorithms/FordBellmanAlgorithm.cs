using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Windows;
using GraphEditor.GraphStruct;

namespace GraphEditor.Algorithms
{
	internal class FordBellmanAlgorithm : SelectionAlgorithm
	{

		protected bool fromOlder;

		public FordBellmanAlgorithm(GraphCanvas drawingCanvas) : base(drawingCanvas)
		{
		}

		public FordBellmanAlgorithm(GraphCanvas drawingCanvas, bool fromOlder) : base(drawingCanvas)
		{
			this.fromOlder = fromOlder;
		}


		protected override bool IsInputCorrect(out string message)
		{
			if (!DrawingCanvas.IsOrientedGraph)
			{
				message = "Algorithm is available only for oriented graphs.";
				return false;
			}
			if (GetSelectedVertices().Count != 2)
			{
				message = "Only 2 vertices must be selected.";
				return false;
			}
			if (GetSelectedVertices().Count != 2)
			{
				message = "Only 2 vertices must be selected.";
				return false;
			}
			message = "There is no path between selected vertices.";
			var isUsed = SameTreeChecking();
			return isUsed[(GraphElementVertex) HelperFunctions.GetGraphElement(DrawingCanvas, GetSelectedVertices()[0])] &&
			       isUsed[(GraphElementVertex) HelperFunctions.GetGraphElement(DrawingCanvas, GetSelectedVertices()[1])];
		}

		protected override AlgorithmResult RunAlgorithm()
		{
			GraphElementVertex startVertex; 
			GraphElementVertex endVertex;
			if (fromOlder)
			{
				startVertex = (GraphElementVertex)HelperFunctions.GetGraphElement(DrawingCanvas, GetSelectedVertices()[0]);
				endVertex = (GraphElementVertex)HelperFunctions.GetGraphElement(DrawingCanvas, GetSelectedVertices()[1]); 
			}
			else
			{
				startVertex = (GraphElementVertex)HelperFunctions.GetGraphElement(DrawingCanvas, GetSelectedVertices()[1]);
				endVertex = (GraphElementVertex)HelperFunctions.GetGraphElement(DrawingCanvas, GetSelectedVertices()[0]); 
			}
			var vertices = DrawingCanvas.GraphStructure.Vertices;
			var edges = DrawingCanvas.GraphStructure.GetAllEdges().ToList();

			var dist = new Dictionary<GraphElementVertex, double>(vertices.Count);
			var path = new Dictionary<GraphElementVertex, GraphElementVertex>();

			foreach (GraphElementVertex vertex in vertices)
			{
				dist.Add(vertex, Double.PositiveInfinity);
				path.Add(vertex, null);
			}
			dist[startVertex] = 0.0;

			GraphElementVertex x = null;
			for (int iter = 0; iter < vertices.Count; iter++)
			{
				x = null;
				for (int v = 0; v < vertices.Count; v++)
				{
					for (int i = 0; i < vertices[v].Connections.Count; i++)
					{
						if (vertices[v].Connections[i].Begin == vertices[v])
						{

							var end = vertices[v].Connections[i].End;
							if (dist[end] > dist[vertices[v]] + vertices[v].Connections[i].Weight)
							{
								dist[end] = dist[vertices[v]] + vertices[v].Connections[i].Weight;
								path[end] = vertices[v];
								x = end;
							}
						}
					}
				}
			}

			if (double.IsPositiveInfinity(dist[endVertex]))
				throw new ArgumentException("There is no path between vertices.");
			if (x != null)
				throw new ArgumentException("There is negative cycle in the graph so it is impossible to find shortest way.");

			var resultVertexes = new List<GraphElementVertex>();
			var resultEdges = new HashSet<GraphElementEdge>();
			for (var cur = endVertex; cur != startVertex; cur = path[cur])
			{
				resultVertexes.Add(cur);
				if (path[cur] != null)
					resultEdges.Add(DrawingCanvas.GraphStructure.GetEdgeBetweenVertex(cur, path[cur]));
			}
			resultVertexes.Add(startVertex);
			var result = new List<GraphElementBase>(resultVertexes);
			result.AddRange(resultEdges);
			return new AlgorithmResult(dist[endVertex], result);
		}

		private bool HasNegativeCycle()
		{
			var vertices = DrawingCanvas.GraphStructure.Vertices;
			var edges = DrawingCanvas.GraphStructure.GetAllEdges().ToList();
			var startVertex = (GraphElementVertex)HelperFunctions.GetGraphElement(DrawingCanvas, GetSelectedVertices().First());

			var dest = new Dictionary<GraphElementVertex, double>(vertices.Count);
			var path = new Dictionary<GraphElementVertex, GraphElementVertex>();

			var resultVertexes = new List<GraphElementVertex>();
			var resultEdges = new HashSet<GraphElementEdge>();

			for (int i = 0; i < vertices.Count; i++)
			{
				dest.Add(vertices[i], Double.PositiveInfinity);
				path.Add(vertices[i], null);
			}
			dest[startVertex] = 0.0;

			GraphElementVertex x = null;
			for (int i = 0; i < vertices.Count; i++)
			{
				x = null;
				for (int j = 0; j < edges.Count; j++)
				{
					if (!Double.IsPositiveInfinity(dest[edges[j].Begin]))
					{
						if (dest[edges[j].End] > dest[edges[j].Begin] + edges[j].Weight)
						{
							dest[edges[j].End] = Math.Max(Double.NegativeInfinity, dest[edges[j].Begin] + edges[j].Weight);
							path[edges[j].End] = edges[j].Begin;
							x = edges[j].End;
						}
					}
				}
			}
			return x == null;
		}
	}
}