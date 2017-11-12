using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

//Prim's (MST) : Special Subtree
//https://www.hackerrank.com/challenges/primsmstsub/problem
class Solution {
    
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
        public static List<Edge> BuildMinimumSpanningTree(int first, IGraph g)
        {
          var seen = new HashSet<int>();
          var notSeen = new HashSet<int>(g.Vertexes());
          var result = new List<Edge>();
          var prio = new Heap<Edge>(Comparer<Edge>.Create((x,y)=>x.weight-y.weight));

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
    
    static void Main(String[] args) {
        var mn = Console.ReadLine().Split(' ').Select(int.Parse).ToArray();
        var g = new Dictionary<int, List<Edge>>();
        for (int i=0; i<mn[1]; ++i)
        {
           var xyr = Console.ReadLine().Split(' ').Select(int.Parse).ToArray();
           List<Edge> eds;
           if (!g.TryGetValue(xyr[0], out eds))
           {
               eds = new List<Edge>();
               g.Add(xyr[0], eds);
           }
           eds.Add(new Edge(xyr[0], xyr[1], xyr[2]));
            
           if (!g.TryGetValue(xyr[1], out eds))
           {
               eds = new List<Edge>();
               g.Add(xyr[1], eds);
           }
           eds.Add(new Edge(xyr[1], xyr[0], xyr[2]));            
        }
        var first = int.Parse(Console.ReadLine());
        
        int sum = 0;
        foreach(var edge in Prims.BuildMinimumSpanningTree(first, new DictionaryGraph(g)))
        {
            sum += edge.weight;
        }
        
        Console.WriteLine(sum);
    }
}