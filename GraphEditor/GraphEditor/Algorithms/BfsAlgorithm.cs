using System.Collections.Generic;
using System.Linq;
using GraphEditor.Graphics;
using GraphEditor.GraphStruct;

namespace GraphEditor.Algorithms
{
	class BfsAlgorithm : Algorithm
	{
		public BfsAlgorithm(GraphCanvas drawingCanvas) : base(drawingCanvas)
		{
		}

		protected override bool IsInputCorrect()
		{
			return GetSelectedVertices().Count == 1;
		}

		protected override AlgorithmResult RunAlgorithm()
		{
			var vertices = DrawingCanvas.GraphStructure.Vertices;
			var verticesCount = vertices.Count;
			var startVertex = (GraphElementVertex) HelperFunctions.GetGraphElement(DrawingCanvas, GetSelectedVertices().First());
			var queue = new Queue<GraphElementVertex>();
			queue.Enqueue(startVertex);
			var isUsed = new Dictionary<GraphElementVertex, bool>(verticesCount);
			//TODO Get know what 'd' and 'p' is.
			var d = new Dictionary<GraphElementVertex, int>(verticesCount);
			var p = new Dictionary<GraphElementVertex, GraphElementVertex>(verticesCount);
			for (int i = 0; i < verticesCount; i++)
			{
				isUsed.Add(DrawingCanvas.GraphStructure.Vertices[i], false);
				d.Add(DrawingCanvas.GraphStructure.Vertices[i], 0);
				p.Add(DrawingCanvas.GraphStructure.Vertices[i], DrawingCanvas.GraphStructure.Vertices[i]);
			}


			isUsed[startVertex] = true;
			p[startVertex] = startVertex;
			while (queue.Count != 0)
			{
				var vertex = queue.Dequeue();
				for (int i = 0; i < vertex.Connections.Count; i++)
				{
					var to = vertex.Connections[i].Begin == vertex ? vertex.Connections[i].End : vertex.Connections[i].Begin;
					if (!isUsed[to])
					{
						isUsed[to] = true;
						queue.Enqueue(to);
						d[to] = d[vertex] + 1;
						p[to] = vertex;
					}
				}
			}
			var minPathDest = new GraphElementVertex();
			int min = d.Max(pair => pair.Value);
			foreach (var pair in d)
			{
				if (isUsed[pair.Key] && pair.Value != 0 && min > pair.Value)
				{
					min = pair.Value;
				}
			}
			foreach (var pair in d.Where(pair => pair.Value == min))
			{
				minPathDest = pair.Key;
			}
			var result = new List<GraphElementBase>();
			for (var i = minPathDest; i != startVertex; i = p[i])
			{
				result.Add(i);
			}
			//TODO Find edges is needed to be selected.
			return new AlgorithmResult(min, result);
		}

		private List<GraphicsBase> GetSelectedVertices()
		{
			return DrawingCanvas.GraphicsList.Cast<GraphicsVertex>().Where(vertex => vertex.IsSelected).Cast<GraphicsBase>().ToList();
		}
	}
}