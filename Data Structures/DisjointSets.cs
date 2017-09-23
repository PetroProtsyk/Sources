using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protsyk.DataStructures
{
    // https://en.wikipedia.org/wiki/Disjoint-set_data_structure

    public interface IRootedSet<T> : IEnumerable<T>
    {
        T Root { get; }

        bool Contains(T x);
    }

    public class DisjointSets<T>
    {
        #region Fields
        private readonly IEqualityComparer<T> comparer;
        private readonly Dictionary<T, Node> values;
        private readonly HashSet<Node> sets;
        #endregion

        public int SetCount => sets.Count;

        public int ItemCount => values.Count;

        public IEnumerable<IRootedSet<T>> Sets => sets;

        public IEnumerable<KeyValuePair<T, IRootedSet<T>>> Items => values.Select(v => new KeyValuePair<T, IRootedSet<T>>(v.Key, Find(v.Key)));

        public DisjointSets()
            : this(EqualityComparer<T>.Default)
        {
        }

        public DisjointSets(IEqualityComparer<T> comparer)
        {
            this.comparer = comparer;
            this.values = new Dictionary<T, Node>(this.comparer);
            this.sets = new HashSet<Node>();
        }

        public IRootedSet<T> MakeSet(T x)
        {
            var node = FindInternal(x);
            if (node == null)
            {
                node = new Node(this, x);
                values.Add(x, node);
                sets.Add(node);
            }

            return node;
        }

        public IRootedSet<T> Find(T x)
        {
            return FindInternal(x);
        }

        private Node FindInternal(T x)
        {
            Node node;
            if (!values.TryGetValue(x, out node))
            {
                return null;
            }

            var rootNode = node;
            while (rootNode.parent != rootNode)
            {
                rootNode = rootNode.parent;
            }

            // Path-compression
            while (node.parent != node)
            {
                var temp = node;
                node = node.parent;
                temp.parent = rootNode;
            }

            return rootNode;
        }

        public IRootedSet<T> Union(T x, T y)
        {
            var xRoot = FindInternal(x);
            var yRoot = FindInternal(y);
            return Union(xRoot, yRoot);
        }

        public IRootedSet<T> Union(IRootedSet<T> x, IRootedSet<T> y)
        {
            var xRoot = (Node)x;
            var yRoot = (Node)y;

            if (!sets.Contains(xRoot) ||
                !sets.Contains(yRoot))
            {
                throw new ArgumentException("One of arguments is not a root set of this forest");
            }

            if (xRoot == yRoot)
            {
                return xRoot;
            }

            if (xRoot.rank < yRoot.rank)
            {
                xRoot.parent = yRoot;
                yRoot.count += xRoot.count;
                sets.Remove(xRoot);
                return yRoot;
            }
            else if (xRoot.rank > yRoot.rank)
            {
                yRoot.parent = xRoot;
                xRoot.count += yRoot.count;
                sets.Remove(yRoot);
                return xRoot;
            }
            else
            {
                yRoot.parent = xRoot;
                xRoot.count += yRoot.count;
                xRoot.rank = xRoot.rank + 1;
                sets.Remove(yRoot);
                return xRoot;
            }
        }

        private class Node : IRootedSet<T>
        {
            #region Fields
            private readonly  DisjointSets<T> owner;
            private readonly T value;

            public Node parent;
            public int rank;
            public int count;
            #endregion

            #region Methods
            public Node(DisjointSets<T> owner, T value)
            {
                this.owner = owner;
                this.value = value;
                this.parent = this;
                this.rank = 0;
                this.count = 1;
            }
            #endregion

            #region IRootedSet
            public T Root => value;
            
            public int Count => count;

            public bool Contains(T x)
            {
                return (owner.Find(x) == owner.Find(value));
            }

            public IEnumerator<T> GetEnumerator()
            {
                var set = owner.Find(value);
                return owner.values.Where(v => owner.Find(v.Key) == set).Select(v => v.Key).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            #endregion
        }
    }
}
