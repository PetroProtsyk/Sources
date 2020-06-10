using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Protsyk.DataStructures
{
    /// <summary>
    /// A Disjoint Set data structure keeps track of a set of elements
    /// partitioned into a number of disjoint subsets.
    /// It allows to efficiently perform the following operations:
    /// <ul>
    ///    <li>Determine a subset to which a given element belongs. This can be used for determining if two elements are in the same subset.</li>
    ///    <li>Join two subsets into a single new subset.</li>
    ///    <li>Add new subset.</li>
    ///</ul>
    ///The computational complexity of these operations are near constant.
    ///This allows to solve certain problems on Graphs very efficiently.
    /// </summary>
    /// <typeparam name="T">Type of elements</typeparam>
    public class DisjointSets<T>
    {
        #region Fields

        private readonly IEqualityComparer<T> comparer;
        private readonly Dictionary<T, Node> values;
        private readonly HashSet<Node> sets;

        #endregion

        #region Properties

        public int SetCount => sets.Count;

        public int ItemCount => values.Count;

        public IEnumerable<IRootedSet<T>> Sets => sets;

        public IEnumerable<KeyValuePair<T, IRootedSet<T>>> Items => values.Select(v => new KeyValuePair<T, IRootedSet<T>>(v.Key, Find(v.Key)));

        #endregion

        #region Constructors

        public DisjointSets()
            : this(EqualityComparer<T>.Default) { }


        public DisjointSets(IEqualityComparer<T> comparer)
        {
            this.comparer = comparer;
            this.values = new Dictionary<T, Node>(this.comparer);
            this.sets = new HashSet<Node>();
        }

        #endregion

        #region Methods

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
            var xRoot = (Node) x;
            var yRoot = (Node) y;

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

            if (xRoot.rank > yRoot.rank)
            {
                yRoot.parent = xRoot;
                xRoot.count += yRoot.count;
                sets.Remove(yRoot);
                return xRoot;
            }

            yRoot.parent = xRoot;
            xRoot.count += yRoot.count;
            xRoot.rank = xRoot.rank + 1;
            sets.Remove(yRoot);
            return xRoot;
        }

        #endregion

        #region Types

        private class Node : IRootedSet<T>
        {
            #region Fields

            private readonly DisjointSets<T> owner;
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

        #endregion
    }

    /// <summary>
    /// Set that belongs to a forest of disjoint sets
    /// </summary>
    /// <typeparam name="T">Type of elements</typeparam>
    public interface IRootedSet<T> : IEnumerable<T>
    {
        /// <summary>
        /// Defining element of the set
        /// </summary>
        T Root { get; }

        /// <summary>
        /// Check if element belongs to this set
        /// </summary>
        bool Contains(T x);
    }
}
