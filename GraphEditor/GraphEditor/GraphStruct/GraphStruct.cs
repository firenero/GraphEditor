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

        public void AddElement(PropertiesGraphBase element)
        {
            if (element is PropertiesGraphVertex)
                AddVertex(((PropertiesGraphVertex)element).Label, ((PropertiesGraphVertex)element).id);
            else if(element is PropertiesGraphEdge)
                AddConnection(((PropertiesGraphEdge)element).Begin, ((PropertiesGraphEdge)element).End, ((PropertiesGraphEdge)element).Weight, ((PropertiesGraphEdge)element).id);
        }
        public void AddVertex(String label, int id)
        {
            foreach (var val in vertices)
            {
                if (id == val.ID) throw new Exception("This vertex already exist!");
                //if (label == val.Label) label += "!";
            }
            GraphElementVertex vertex = new GraphElementVertex(label, id);
            vertices.Add(vertex);
        }
        public void RemoveVertex(int id)
        {
            foreach (var del in vertices)
                if (id == del.ID)
                {
                    List<int> idlist = new List<int>();
                    foreach (var edg in del.Connections)
                        idlist.Add(edg.ID);
                    foreach (var idedg in idlist)
                        RemoveConnection(idedg);
                    vertices.Remove(del);
                    break;
                }
        }
        public void AddConnection(int id_begin, int id_end, double weight, int id)
        {
            foreach (var ver in vertices)
                foreach (var edg in ver.Connections)
                if (id == edg.ID) throw new Exception("Connection with this id already exist!");
            GraphElementEdge edge = new GraphElementEdge();
            GraphElementVertex begin = GetVertex(id_begin);
            GraphElementVertex end = GetVertex(id_end);
            edge.Begin = begin;
            edge.End = end;
            edge.Weight = weight;
            edge.ID = id;
            begin.AddConnection(edge);
            end.AddConnection(edge);

        }
        public void RemoveConnection(int id_begin, int id_end)
        {
            int vert = 2;
            foreach (var ver in vertices)
            {
                if (0 == vert) break;
                foreach (var edg in ver.Connections)
                    if (id_begin == edg.Begin.ID && id_end == edg.End.ID)
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
               GraphElementVertex v = GetVertex(changed.id);
               v.Label = ((PropertiesGraphVertex)changed).Label;
            }
            else if (changed is PropertiesGraphEdge)
            {
                GraphElementEdge e = GetEdge(changed.id);
                e.Weight = ((PropertiesGraphEdge)changed).Weight;
            }
        }
        public void Clear()
        {
            vertices.Clear();
        }


        public GraphElementVertex GetVertex(int id)
        {
            foreach (var val in vertices)
                if (id == val.ID) return val;
            return null;
        }
        public GraphElementEdge GetEdge(int id)
        {
            foreach (var ver in vertices)
                foreach (var edg in ver.Connections)
                    if (id == edg.ID) return edg;
            return null;
        }
		/// <summary>
		/// Get set o graph edges ordered by ascending.
		/// </summary>
		/// <returns></returns>
	    public SortedSet<GraphElementEdge> GetAllEdges()
	    {
		    var set = new SortedSet<GraphElementEdge>();
		    foreach (var edge in vertices.SelectMany(vertex => vertex.Connections))
		    {
			    set.Add(edge);
		    }
			return set;
	    }

	    public GraphElementBase GetElement(int id)
        {
            GraphElementVertex v = GetVertex(id);
            if (v != null) return v;
            GraphElementEdge e = GetEdge(id);
            if (e != null) return e;
            return null;
        }
        public List<GraphElementVertex> Vertices
        {
            get { return vertices; }
        }
    }
}
