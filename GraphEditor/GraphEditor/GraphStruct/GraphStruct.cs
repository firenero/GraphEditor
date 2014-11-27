using System;
using System.Collections.Generic;
using System.Linq;
using GraphEditor.PropertiesClasses;

namespace GraphEditor.GraphStruct
{
	public class GraphStruct
	{
		private List<GraphElementVertex> vertices;

		public GraphStruct()
		{
			vertices = new List<GraphElementVertex>();
		}

		public List<GraphElementVertex> Vertices
		{
			get { return vertices; }
		}

		public void AddElement(PropertiesGraphBase element)
		{
			if (element is PropertiesGraphVertex)
				AddVertex((element as PropertiesGraphVertex).Label, element.id);
			else if (element is PropertiesGraphEdge)
				AddConnection((element as PropertiesGraphEdge).Begin, (element as PropertiesGraphEdge).End, (element as PropertiesGraphEdge).Weight, element.id);
		}

		public void AddVertex(String label, int id)
		{
			if (vertices.Any(val => id == val.ID))
			{
				throw new Exception("This vertex already exist!");
			}
			var vertex = new GraphElementVertex(label, id);
			vertices.Add(vertex);
		}

		public void RemoveVertex(int id)
		{
			foreach (var del in vertices)
				if (id == del.ID)
				{
					var idlist = del.Connections.Select(edg => edg.ID).ToList();
					foreach (var idedg in idlist)
						RemoveConnection(idedg);
					vertices.Remove(del);
					break;
				}
		}

		public void AddConnection(int idBegin, int idEnd, double weight, int id)
		{
			if (vertices.SelectMany(ver => ver.Connections).Any(edg => id == edg.ID))
			{
				throw new Exception("Connection with this id already exist!");
			}
			var edge = new GraphElementEdge();
			var begin = GetVertex(idBegin);
			var end = GetVertex(idEnd);
			edge.Begin = begin;
			edge.End = end;
			edge.Weight = weight;
			edge.ID = id;
			begin.AddConnection(edge);
			end.AddConnection(edge);
		}

		public void RemoveConnection(int idBegin, int idEnd)
		{
			int vert = 2;
			foreach (var ver in vertices)
			{
				if (0 == vert) break;
				foreach (var edg in ver.Connections)
					if (idBegin == edg.Begin.ID && idEnd == edg.End.ID)
					{
						ver.RemoveConnection(edg);
						vert--;
						break;
					}
			}
		}

		public void RemoveConnection(int id)
		{
			int vert = 2;
			foreach (var ver in vertices)
			{
				if (0 == vert) break;
				foreach (var edg in ver.Connections)
					if (id == edg.ID)
					{
						ver.RemoveConnection(edg);
						vert--;
						break;
					}
			}
		}

		public void ChangeState(PropertiesGraphBase changed)
		{
			if (changed is PropertiesGraphVertex)
			{
				var v = GetVertex(changed.id);
				v.Label = (changed as PropertiesGraphVertex).Label;
			}
			else if (changed is PropertiesGraphEdge)
			{
				var e = GetEdge(changed.id);
				e.Weight = (changed as PropertiesGraphEdge).Weight;
			}
		}

		public void Clear()
		{
			vertices.Clear();
		}


		public GraphElementVertex GetVertex(int id)
		{
			return vertices.FirstOrDefault(val => id == val.ID);
		}

		public GraphElementEdge GetEdge(int id)
		{
			return vertices.SelectMany(ver => ver.Connections).FirstOrDefault(edg => id == edg.ID);
		}

		/// <summary>
		///     Get set o graph edges ordered by weight ascending.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<GraphElementEdge> GetAllEdges()
		{
			var set = new HashSet<GraphElementEdge>();
			foreach (var edge in vertices.SelectMany(vertex => vertex.Connections))
			{
				set.Add(edge);
			}
			return set.OrderBy(edge => edge.Weight);
		}

		public GraphElementEdge GetEdgeBetweenVertex(GraphElementVertex first, GraphElementVertex second)
		{
			return (from edge in first.Connections let res = edge.Begin == first ? edge.End : edge.Begin where res == second select edge).FirstOrDefault();
		}

		public GraphElementBase GetElement(int id)
		{
			var v = GetVertex(id);
			if (v != null) return v;
			var e = GetEdge(id);
			if (e != null) return e;
			return null;
		}
	}
}