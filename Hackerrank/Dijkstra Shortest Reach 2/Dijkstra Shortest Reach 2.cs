using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Dijkstra: Shortest Reach 2
// https://www.hackerrank.com/challenges/dijkstrashortreach
namespace Solution {
    
    public class Heap<T>
    {
        #region Fields
        private readonly List<T> items;
        private readonly IComparer<T> comparer;
        #endregion

        #region Constructors
        public Heap()
            : this(Comparer<T>.Default) { }

        public Heap(IEnumerable<T> elements)
            : this(Comparer<T>.Default, elements) { }

        public Heap(IComparer<T> comparer)
            : this(comparer, Enumerable.Empty<T>()) { }

        public Heap(IComparer<T> comparer, IEnumerable<T> range)
        {
            this.comparer = comparer;
            this.items = new List<T>(range);
            MakeHeap();
        }
        #endregion

        #region Methods
        private void AddInternal(T item)
        {
            items.Add(item);
            SiftUp(Count - 1);
        }

        private void RemoveAtInternal(int index)
        {
            Swap(index, Count - 1);
            items.RemoveAt(Count - 1);
            SiftDown(index);
        }

        private void SiftDown(int k)
        {
            int left = LeftChild(k);
            int right = left + 1;
            int max = k;

            while (left < Count)
            {
                if (IsOutOfOrder(max, left))
                {
                    max = left;
                }

                if (right < Count && IsOutOfOrder(max, right))
                {
                    max = right;
                }

                if (max == k)
                {
                    break;
                }

                Swap(max, k);

                k = max;
                left = LeftChild(k);
                right = left + 1;
            }
        }

        private void SiftUp(int k)
        {
            int parent = ParentOf(k);
            while (k > 0 && IsOutOfOrder(parent, k))
            {
                Swap(k, parent);
                k = parent;
                parent = ParentOf(k);
            }
        }

        private void MakeHeap()
        {
            for (int i = ParentOf(Count); i >= 0; i--)
            {
                SiftDown(i);
            }
        }

        private void Swap(int i, int j)
        {
            var tmp = items[i];
            items[i] = items[j];
            items[j] = tmp;
        }

        private bool IsOutOfOrder(int i, int j)
        {
            return comparer.Compare(items[i], items[j]) > 0;
        }

        private static int ParentOf(int index)
        {
            return (index - 1) >> 1;
        }

        private static int LeftChild(int index)
        {
            return (index << 1) + 1;
        }

        private static int RightChild(int index)
        {
            return LeftChild(index) + 1;
        }
        #endregion

        #region Public Properties
        public int Count
        {
            get { return items.Count; }
        }

        public bool IsEmpty
        {
            get { return items.Count == 0; }
        }

        public T Top
        {
            get
            {
                return items[0];
            }
        }
        #endregion

        #region Public API
        public void Add(T item)
        {
            AddInternal(item);
        }

        public void AddRange(IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                AddInternal(item);
            }
        }

        public T RemoveTop()
        {
            var result = Top;
            RemoveAtInternal(0);
            return result;
        }
        #endregion

    }    

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

    internal class HashCombine
    {
        public static int Combine(params int[] values)
        {
            unchecked
            {
                int hash = 5381;
                for (int i = 0; i < values.Length; ++i)
                {
                    hash = ((hash << 5) + hash) ^ values[i];
                }
                return hash;
            }
        }
    }
    
    class Program {
        static int next(string s, ref int pos)
        {
         while (pos<s.Length && !char.IsDigit(s[pos])) pos++;
         int val = 0;
         while (pos<s.Length && char.IsDigit(s[pos])) { val = val*10+(int)s[pos]-(int)'0'; pos++;}
         return val;
        }

        static void Main(string[] args) {
            var b = Console.In.ReadToEnd();
            var d = 0;
            int T = next(b, ref d);
            for (int i = 0; i < T; ++i) {
                int N = next(b, ref d);
                int M = next(b, ref d);

                var V = new List<Edge>[N+1];
                var z = new HashSet<Edge>(EdgeComparer.UndirectedEdgeComparer);

                for (int j = 0; j < N; j++) {
                    V[j] = new List<Edge>();
                }

                for (int j = 0; j < M; j++) {
                    var s2_0 = next(b, ref d);
                    var s2_1 = next(b, ref d);
                    var s2_2 = next(b, ref d);

                    if (z.Add(new Edge(s2_0, s2_1, s2_2)))
                    {
                      V[s2_0 - 1].Add(new Edge(s2_0 - 1, s2_1 - 1, s2_2));
                      V[s2_1 - 1].Add(new Edge(s2_1 - 1, s2_0 - 1, s2_2));
                    }
                }

                int S = next(b, ref d) - 1;
                var r = Solve(N, M, V, S);

                bool first = true;
                var sb = new StringBuilder();
                for (int g = 0; g < r.Length; ++g) {
                    if (g != S) {
                        if (!first) {
                            sb.Append(' ');
                        }
                        sb.Append(r[g]);
                        first = false;
                    }
                }
                if (i > 0) Console.WriteLine();
                Console.Write(sb);
            }
        }

        private static long[] Solve(int N, int M, List<Edge>[] V, int S) {
            var cost = new long[N];
            for (int i = 0; i < cost.Length; ++i) cost[i] = -1;
            cost[S] = 0;

            var wave = new Heap<Tuple<long, int>>();
            wave.Add(new Tuple<long, int>(0, S));

            while (wave.Count > 0) {
                var f = wave.RemoveTop();
                var ls = V[f.Item2];
                foreach (var l in ls) {
                    var newCost = f.Item1 + l.weight;
                    if (cost[l.to] != -1 && cost[l.to] < newCost) {
                        continue;
                    }

                    cost[l.to] = newCost;
                    wave.Add(new Tuple<long, int>(newCost, l.to));
                }
            }

            return cost;
        }
    }
}
