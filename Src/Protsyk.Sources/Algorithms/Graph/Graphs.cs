using Protsyk.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Protsyk.Sources.Algorithms.Graph
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
            return HashCode.Combine(x.from, x.to, x.weight);
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

        public static ArrayGraph CloneFromGraph(IGraph graph)
        {
            if (graph is ArrayGraph maybeArray)
            {
                return CloneFromArray(maybeArray.adjacencyMatrix);
            }

            throw new NotSupportedException();
        }

        public static ArrayGraph CloneFromArray(int[,] graph)
        {
            var vCount = graph.GetLength(0);
            var copy = new int[vCount, vCount];

            for (int i=0; i<vCount; ++i)
            {
                for (int j=0; j<vCount; ++j)
                {
                    copy[i, j] = graph[i, j];
                }
            }

            return new ArrayGraph(copy);
        }

        public int this[int u, int v]
        {
            get => adjacencyMatrix[u, v];
            set => adjacencyMatrix[u, v] = value;
        }
    }

    public class DictionaryGraph : IGraph
    {
        private readonly Dictionary<int, List<Edge>> graph;

        public DictionaryGraph(Dictionary<int, List<Edge>> graph)
        {
            this.graph = graph ?? throw new ArgumentNullException();
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
            if (graph.TryGetValue(v, out List<Edge> eds))
            {
                return eds;
            }
            return Enumerable.Empty<Edge>();
        }
    }

    public class LabeledGraph<T> : IGraph
    {
        private readonly Dictionary<T, LabeledVertex<T>> vertecies = new Dictionary<T, LabeledVertex<T>>();
        private readonly Dictionary<T, int> labelToId = new Dictionary<T, int>();
        private readonly Dictionary<int, T> idTolabel = new Dictionary<int, T>();

        public LabeledVertex<T> GetVertexByLabel(T label) => vertecies[label];

        public LabeledVertex<T> GetVertexById(int id) => vertecies[idTolabel[id]];

        public int GetIdByLabel(T label) => labelToId[label];

        public static LabeledGraph<T> From(IEnumerable<LabeledVertex<T>> vertecies)
        {
            var g = new LabeledGraph<T>();
            int nr = 0;
            foreach (var v in vertecies)
            {
                g.vertecies.Add(v.Label, v);
                g.labelToId.Add(v.Label, nr);
                g.idTolabel.Add(nr, v.Label);
                nr++;
            }
            return g;
        }

        public int VertexesCount() => vertecies.Count;

        public IEnumerable<int> Vertexes() => idTolabel.Keys;

        public IEnumerable<Edge> Edges()
        {
            return vertecies.Values.SelectMany(v => v.GetAdjacent().Select(s => new Edge(labelToId[v.Label], labelToId[s], 1)));
        }

        public IEnumerable<Edge> EdgesFrom(int v)
        {
            if (idTolabel.TryGetValue(v, out T key))
            {
                return vertecies[key].GetAdjacent().Select(s => new Edge(v, labelToId[s], 1));
            }
            throw new Exception($"No vertex {v}");
        }
    }

    public class LabeledVertex<T>
    {
        private List<T> adjacent;

        public T Label { get; private set; }

        public IEnumerable<T> GetAdjacent()
        {
            return adjacent;
        }

        public static LabeledVertex<T> From(T label, IEnumerable<T> adjacent)
        {
            return new LabeledVertex<T>
            {
                Label = label,
                adjacent = new List<T>(adjacent ?? Enumerable.Empty<T>())
            };
        }
    }
}
