using System;
using System.Collections.Generic;
using System.Linq;

namespace Protsyk.Collections
{
    /// <summary>
    /// Undirected Edge
    /// </summary>
    public struct Edge
    {
        public readonly int from;
        public readonly int to;
        public readonly int weight;

        public Edge(int from, int to, int weight)
        {
            this.from = from;
            this.to = to;
            this.weight = weight;
        }
    }

    public static class EdgeComparer
    {
        public static IEqualityComparer<Edge> UndirectedEdgeComparer = new UndirectedEdgeComparer();

        public static IEqualityComparer<Edge> DirectedEdgeComparer = new DirectedEdgeComparer();
    }

    public class UndirectedEdgeComparer : IEqualityComparer<Edge>
    {
        public bool Equals(Edge x, Edge y)
        {
            return ((x.from == y.from && x.to == y.to) ||
                    (x.to == y.from && x.from == y.to)) &&
                   x.weight == y.weight;
        }

        public int GetHashCode(Edge x)
        {
            return HashCombine.Combine(x.from, x.to, x.weight);
        }
    }

    public class DirectedEdgeComparer : IEqualityComparer<Edge>
    {
        public bool Equals(Edge x, Edge y)
        {
            return x.from == y.from && x.to == y.to && x.weight == y.weight;
        }

        public int GetHashCode(Edge x)
        {
            return HashCombine.Combine(x.from, x.to, x.weight);
        }
    }

    public interface IGraph
    {
        int VertexesCount();

        IEnumerable<int> Vertexes();

        IEnumerable<Edge> Edges();

        IEnumerable<Edge> EdgesFrom(int v);
    }

    public class ArrayGraph : IGraph
    {
        private readonly int [,] adjacencyMatrix;

        public ArrayGraph(int [,] adjacencyMatrix)
        {
            if (adjacencyMatrix == null)
                throw new ArgumentNullException();
            if (adjacencyMatrix.GetLength(0) != adjacencyMatrix.GetLength(1))
                throw new ArgumentException();

            this.adjacencyMatrix = adjacencyMatrix;
        }

        public int VertexesCount()
        {
            return adjacencyMatrix.GetLength(0);
        }

        public IEnumerable<int> Vertexes()
        {
            return Enumerable.Range(0, adjacencyMatrix.GetLength(0));
        }

        public IEnumerable<Edge> Edges()
        {
            for (int i=0; i<adjacencyMatrix.GetLength(0); ++i)
            {
              for (int j=0; j<adjacencyMatrix.GetLength(1); ++j)
              {
                if (adjacencyMatrix[i,j]>0)
                {
                    yield return new Edge(i,j,adjacencyMatrix[i,j]);
                }
              }
            }
        }

        public IEnumerable<Edge> EdgesFrom(int v)
        {
              for (int j=0; j<adjacencyMatrix.GetLength(1); ++j)
              {
                if (adjacencyMatrix[v,j]>0)
                    yield return new Edge(v,j,adjacencyMatrix[v,j]);
              }
        }
    }

    public class DictionaryGraph : IGraph
    {
        private readonly Dictionary<int, List<Edge>> graph;

        public DictionaryGraph(Dictionary<int, List<Edge>> graph)
        {
            if (graph == null)
                throw new ArgumentNullException();

            this.graph = graph;
        }

        public int VertexesCount()
        {
            return graph.Count;
        }

        public IEnumerable<int> Vertexes()
        {
            return graph.Keys;
        }

        public IEnumerable<Edge> Edges()
        {
            throw new NotSupportedException();
        }

        public IEnumerable<Edge> EdgesFrom(int v)
        {
           List<Edge> eds;
           if (graph.TryGetValue(v, out eds))
           {
               return eds;
           }
           return Enumerable.Empty<Edge>();
        }
    }
}
