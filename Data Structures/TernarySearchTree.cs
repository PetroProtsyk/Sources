using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protsyk.Collections
{
    /// <summary>
    /// Ternary Search Tree
    /// 
    /// https://en.wikipedia.org/wiki/Ternary_search_tree
    /// http://www.cs.princeton.edu/~rs/strings/
    /// </summary>
    public class TernarySearchTree<T>
    {
        #region Fields

        private readonly IComparer<T> comparer;
        private int count;
        private Node root;

        #endregion

        #region Properties

        public int Count
        {
            get { return count; }
        }

        #endregion

        #region Constructors

        public TernarySearchTree()
            : this(Comparer<T>.Default) { }


        public TernarySearchTree(IComparer<T> comparer)
        {
            this.count = 0;
            this.comparer = comparer;
        }

        #endregion

        #region Methods

        /// <summary>
        // Add element
        /// </summary>
        public bool Add(IEnumerable<T> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var sequence = item.GetEnumerator();
            if (!sequence.MoveNext())
            {
                throw new ArgumentNullException(nameof(item));
            }

            var temp = count;
            root = InsertNonRecursive(root, sequence);
            return temp != count;
        }

        /// <summary>
        /// Recursive version as in the article
        /// </summary>
        private Node InsertRecursive(Node node, IEnumerator<T> sequence)
        {
            if (node == null)
            {
                node = new Node(sequence.Current);
            }

            var compare = comparer.Compare(sequence.Current, node.Split);

            if (compare < 0)
            {
                node.Lokid = InsertRecursive(node.Lokid, sequence);
            }
            else if (compare == 0)
            {
                if (!sequence.MoveNext())
                {
                    if (!node.IsFinal)
                    {
                        node.IsFinal = true;
                        ++count;
                    }
                }
                else
                {
                    node.Eqkid = InsertRecursive(node.Eqkid, sequence);
                }
            }
            else
            {
                node.Hikid = InsertRecursive(node.Hikid, sequence);
            }

            return node;
        }

        /// <summary>
        /// Non recursive version
        /// </summary>
        private Node InsertNonRecursive(Node node, IEnumerator<T> sequence)
        {
            if (node == null)
            {
                node = new Node(sequence.Current);
            }

            var current = node;
            while (true)
            {
                var label = sequence.Current;
                while (true)
                {
                    var next = comparer.Compare(label, current.Split);
                    if (next == 0)
                    {
                        break;
                    }

                    if (next < 0)
                    {
                        if (current.Lokid == null)
                        {
                            current.Lokid = new Node(label);
                        }

                        current = current.Lokid;
                    }
                    else
                    {
                        if (current.Hikid == null)
                        {
                            current.Hikid = new Node(label);
                        }

                        current = current.Hikid;
                    }
                }

                if (!sequence.MoveNext())
                {
                    if (!current.IsFinal)
                    {
                        current.IsFinal = true;
                        ++count;
                    }
                    break;
                }
                else
                {
                    if (current.Eqkid == null)
                    {
                        current.Eqkid = new Node(sequence.Current);
                    }
                    current = current.Eqkid;
                }
            }

            return node;
        }


        /// <summary>
        /// Match values in the tree
        /// </summary>
        public IEnumerable<IEnumerable<T>> Match(ITrieMatcher<T> matcher)
        {
            var prefix = new List<T>();
            var stack = new Stack<KeyValuePair<Node, bool>>();

            if (root != null)
            {
                stack.Push(new KeyValuePair<Node, bool>(root, false));
            }

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current.Key == null)
                {
                    matcher.Pop();
                    prefix.RemoveAt(prefix.Count - 1);
                    continue;
                }

                if (current.Value)
                {
                    matcher.Next(current.Key.Split);
                    prefix.Add(current.Key.Split);

                    if (current.Key.IsFinal && matcher.IsFinal())
                    {
                        yield return prefix;
                    }
                    continue;
                }

                if (current.Key.Hikid != null)
                {
                    stack.Push(new KeyValuePair<Node, bool>(current.Key.Hikid, false));
                }

                if (matcher.Next(current.Key.Split))
                {
                    matcher.Pop();

                    stack.Push(new KeyValuePair<Node, bool>(null, false));

                    if (current.Key.Eqkid != null)
                    {
                        stack.Push(new KeyValuePair<Node, bool>(current.Key.Eqkid, false));
                    }

                    stack.Push(new KeyValuePair<Node, bool>(current.Key, true));
                }

                if (current.Key.Lokid != null)
                {
                    stack.Push(new KeyValuePair<Node, bool>(current.Key.Lokid, false));
                }
            }
        }


        /// <summary>
        /// Check if item is in the tree
        /// </summary>
        public bool Contains(IEnumerable<T> s)
        {
            var parent = default(Node);
            var current = root;
            foreach (var label in s)
            {
                while (true)
                {
                    if (current == null)
                    {
                        return false;
                    }

                    var compare = comparer.Compare(label, current.Split);
                    if (compare == 0)
                    {
                        parent = current;
                        current = current.Eqkid;
                        break;
                    }

                    current = compare < 0 ? current.Lokid : current.Hikid;
                }
            }

            if (parent == null)
            {
                return false;
            }
            return parent.IsFinal;
        }
        #endregion

        #region Visualization

        public string ToDotNotation()
        {
            var text = new StringBuilder();
            text.AppendLine("digraph g {");
            text.AppendLine("node[shape = circle];");

            var labels = new Dictionary<Node, int>();
            // Nodes
            foreach (var node in Visit())
            {
                int index = 0;
                if (!labels.TryGetValue(node, out index))
                {
                    index = labels.Count + 1;
                    labels.Add(node, index);
                }

                if (node.IsFinal)
                {
                    text.AppendLine($"node{index}[shape = doublecircle, style = bold, label=\"{node.Split}\"]");
                }
                else
                {
                    text.AppendLine($"node{index}[label=\"{node.Split}\"]");
                }

                if (node.Lokid != null)
                {
                    int childIndex;
                    if (!labels.TryGetValue(node.Lokid, out childIndex))
                    {
                        childIndex = labels.Count + 1;
                        labels.Add(node.Lokid, childIndex);
                    }
                    text.AppendLine($"node{index} -> node{childIndex}");
                }

                if (node.Eqkid != null)
                {
                    int childIndex;
                    if (!labels.TryGetValue(node.Eqkid, out childIndex))
                    {
                        childIndex = labels.Count + 1;
                        labels.Add(node.Eqkid, childIndex);
                    }
                    text.AppendLine($"node{index} -> node{childIndex}");
                }

                if (node.Hikid != null)
                {
                    int childIndex;
                    if (!labels.TryGetValue(node.Hikid, out childIndex))
                    {
                        childIndex = labels.Count + 1;
                        labels.Add(node.Hikid, childIndex);
                    }
                    text.AppendLine($"node{index} -> node{childIndex}");
                }
            }

            text.AppendLine("}");
            return text.ToString();
        }

        private IEnumerable<Node> Visit()
        {
            var stack = new Stack<KeyValuePair<Node, bool>>();

            if (root != null)
            {
                stack.Push(new KeyValuePair<Node, bool>(root, false));
            }

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current.Key == null)
                {
                    continue;
                }

                if (current.Value)
                {
                    yield return current.Key;
                    continue;
                }

                if (current.Key.Hikid != null)
                {
                    stack.Push(new KeyValuePair<Node, bool>(current.Key.Hikid, false));
                }

                stack.Push(new KeyValuePair<Node, bool>(null, false));

                if (current.Key.Eqkid != null)
                {
                    stack.Push(new KeyValuePair<Node, bool>(current.Key.Eqkid, false));
                }

                stack.Push(new KeyValuePair<Node, bool>(current.Key, true));

                if (current.Key.Lokid != null)
                {
                    stack.Push(new KeyValuePair<Node, bool>(current.Key.Lokid, false));
                }
            }
        }

        #endregion


        #region Types
        private class Node
        {
            public readonly T Split;

            public Node Lokid;
            public Node Eqkid;
            public Node Hikid;
            public bool IsFinal;

            public Node(T split)
            {
                this.Split = split;
            }
        }
        #endregion
    }
}
