using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protsyk.Algorithms
{
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

    public class Prims
    {
        public static List<Edge> BuildMinimumSpanningTree(IGraph g)
        {
          var seen = new HashSet<int>();
          var notSeen = new HashSet<int>(g.Vertexes());
          var result = new List<Edge>();

          var first = notSeen.First();
          notSeen.Remove(first);
          seen.Add(first);

          while (notSeen.Count > 0)
          {
              var minE = new Edge(0,0,int.MaxValue);
              var found = false; 

              foreach (var s in seen)
              {
                 foreach (var e in g.EdgesFrom(s))
                 {
                     if (seen.Contains(e.from)
                         && !seen.Contains(e.to)
                         && minE.weight > e.weight)
                     {
                         minE = e;
                         found = true;
                     }
                 }
              }

              if (!found)
              {
                  // Graph is not connected
                  return null;
              }

              seen.Add(minE.to);
              notSeen.Remove(minE.to);
              result.Add(minE);
          }

          return result;
        }

        public static List<Edge> BuildMinimumSpanningTreeWithHeap(IGraph g)
        {
          var seen = new HashSet<int>();
          var notSeen = new HashSet<int>(g.Vertexes());
          var result = new List<Edge>();
          var prio = new Heap<Edge>(Comparer<Edge>.Create((x,y)=>x.weight-y.weight));

          var first = notSeen.First();
          notSeen.Remove(first);
          seen.Add(first);

          foreach (var e in g.EdgesFrom(first))
          {
           prio.Add(e);
          }

          while (notSeen.Count > 0)
          {
              var minE = prio.RemoveTop();
              if (seen.Add(minE.to))
              {
                notSeen.Remove(minE.to);

                // Decrease key
                foreach (var e in g.EdgesFrom(minE.to))
                {
                  prio.Add(e);
                }

                result.Add(minE);
              }
          }

          return result;
        }
    }

    public class Heap<T>
    {
        #region Fields

        private readonly List<T> elements;
        private readonly IComparer<T> comparer;

        #endregion

        #region Constructors
        public Heap()
            : this(Comparer<T>.Default)
        {
        }

        public Heap(IEnumerable<T> elements)
            : this(Comparer<T>.Default, elements)
        {
        }

        public Heap(IComparer<T> comparer)
            : this(comparer, Enumerable.Empty<T>())
        {
        }

        public Heap(IComparer<T> comparer, IEnumerable<T> elements)
        {
            this.comparer = comparer;
            this.elements = new List<T>(elements);
            Heapify();
        }

        #endregion

        #region Methods
        private void AddInternal(T item)
        {
            elements.Add(item);
            Up(Count - 1);
        }

        private void AddRangeInternal(IEnumerable<T> items)
        {
            elements.AddRange(items);
            Heapify();
        }

        private T RemoveTopInternal()
        {
            CheckNotEmpty();
            var lastIndex = Count - 1;
            Swap(0, lastIndex);
            var min = elements[lastIndex];
            elements.RemoveAt(lastIndex);
            Down(0);
            return min;
        }

        private void Down(int k)
        {
            while (true)
            {
                var j = k << 1;
                if (j >= Count) break;
                if (j + 1 < Count && IsOutOfOrder(j, j + 1)) j++;
                if (!IsOutOfOrder(k, j)) break;
                Swap(k, j);
                k = j;
            }
        }

        private void Up(int k)
        {
            while (k > 0 && IsOutOfOrder(k >> 1, k))
            {
                Swap(k, k >> 1);
                k >>= 1;
            }
        }

        private void Heapify()
        {
            for (int i = Count / 2; i >= 0; i--)
            {
                Down(i);
            }
        }

        private void Swap(int i, int j)
        {
            var tmp = elements[i];
            elements[i] = elements[j];
            elements[j] = tmp;
        }

        protected bool IsOutOfOrder(int i, int j) => comparer.Compare(elements[i], elements[j]) > 0;

        private void CheckNotEmpty()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Heap contains no elements");
            }
        }

        #endregion

        #region Public Properties
        public int Count => elements.Count;

        public bool IsEmpty => elements.Count == 0;

        public T Top
        {
            get
            {
                CheckNotEmpty();
                return elements[0];
            }
        }
        #endregion

        #region Public API
        public void Add(T item)
        {
            AddInternal(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            AddRangeInternal(items);
        }


        public T RemoveTop()
        {
            return RemoveTopInternal();
        }
        #endregion
    }

    public static class Program
    {
    	public static void Main(string[] args)
        {
           // Examples are taken from this article
           // http://www.geeksforgeeks.org/greedy-algorithms-set-5-prims-minimum-spanning-tree-mst-2/
           var graph = new int[,] {
                        {0, 2, 0, 6, 0},
                        {2, 0, 3, 8, 5},
                        {0, 3, 0, 0, 7},
                        {6, 8, 0, 0, 9},
                        {0, 5, 7, 9, 0},
                       };

           var graph1 = new int[,] {
                        {0, 4, 0, 0, 0, 0, 0, 8, 0},
                        {4, 0, 8, 0, 0, 0, 0, 11, 0},
                        {0, 8, 0, 7, 0, 4, 0, 0, 2},
                        {0, 0, 7, 0, 9, 14, 0, 0, 0},
                        {0, 0, 0, 9, 0, 10, 0, 0, 0},
                        {0, 0, 4, 14, 10, 0, 2, 0, 0},
                        {0, 0, 0, 0, 0, 2, 0, 1, 6},
                        {8, 11, 0, 0, 0, 0, 1, 0, 7},
                        {0, 0, 2, 0, 0, 0, 6, 7, 0},
                       };


            foreach(var edge in Prims.BuildMinimumSpanningTreeWithHeap(new ArrayGraph(graph1)))
            {
                Console.WriteLine($"{edge.from} -> {edge.to} \t {edge.weight}");
            }
        }
    }
}
