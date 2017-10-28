using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// http://user.it.uu.se/~arnea/abs/simp.html
// https://en.wikipedia.org/wiki/AA_tree

namespace Protsyk.DataStructures
{
    public class AAtree<TEntry> : IEnumerable<TEntry>
    {
        #region Fields

        private int count;
        private Node bottom;
        private readonly Node sentinel = new Node();
        private readonly IComparer<TEntry> comparer;

        #endregion

        #region Properties

        public IComparer<TEntry> Comparer => comparer;

        #endregion

        #region Constructor

        public AAtree()
            : this(Comparer<TEntry>.Default) { }


        public AAtree(IComparer<TEntry> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            this.comparer = comparer;

            Initialize();
        }

        #endregion

        #region Implementation

        private void Initialize()
        {
            this.bottom = sentinel;
            this.count = 0;
        }


        private void Skew(ref Node node)
        {
            if (node.level != node.left.level)
                return;

            // rotate right
            var left = node.left;
            node.left = left.right;
            left.right = node;
            node = left;
        }


        private void Split(ref Node node)
        {
            if (node.right.right.level != node.level)
                return;

            // rotate left
            var right = node.right;
            node.right = right.left;
            right.left = node;
            node = right;
            node.level++;
        }


        private bool Insert(ref Node node, TEntry entry)
        {
            if (node == sentinel)
            {
                ++count;
                node = new Node(entry, sentinel);
                return true;
            }

            int compare = comparer.Compare(entry, node.entry);
            bool result = false;
            if (compare < 0)
            {
                result = Insert(ref node.left, entry);
            }
            else if (compare > 0)
            {
                result = Insert(ref node.right, entry);
            }

            if (result)
            {
                Skew(ref node);
                Split(ref node);
            }

            return result;
        }


        private bool Delete(ref Node node, TEntry entry)
        {
            Node deleted = null;
            return DeleteInternal(ref node, ref deleted, entry);
        }


        private bool DeleteInternal(ref Node node, ref Node deleted, TEntry entry)
        {
            if (node == sentinel)
            {
                return (deleted != null);
            }

            int compare = comparer.Compare(entry, node.entry);
            if (compare < 0)
            {
                if (!DeleteInternal(ref node.left, ref deleted, entry))
                {
                    return false;
                }
            }
            else
            {
                if (compare == 0)
                {
                    --count;
                    deleted = node;
                }
                if (!DeleteInternal(ref node.right, ref deleted, entry))
                {
                    return false;
                }
            }

            if (deleted != null)
            {
                deleted.entry = node.entry;
                deleted = null;
                node = node.right;
            }
            else if (node.left.level < node.level - 1
                     || node.right.level < node.level - 1)
            {
                --node.level;
                if (node.right.level > node.level)
                {
                    node.right.level = node.level;
                }
                Skew(ref node);
                Skew(ref node.right);
                Skew(ref node.right.right);
                Split(ref node);
                Split(ref node.right);
            }

            return true;
        }


        private Node Search(Node root, TEntry entry)
        {
            var node = root;
            while (node != sentinel)
            {
                int compare = comparer.Compare(entry, node.entry);
                if (compare < 0)
                {
                    node = node.left;
                }
                else if (compare > 0)
                {
                    node = node.right;
                }
                else
                {
                    return node;
                }
            }
            return null;
        }


        private IEnumerable<Node> Visit(Node root)
        {
            var stack = new Stack<Node>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current == sentinel)
                {
                    continue;
                }

                stack.Push(current.left);
                stack.Push(current.right);

                yield return current;
            }
        }

        #endregion

        #region API

        public int Count => count;


        public void Clear()
        {
            Initialize();
        }


        public bool Contains(TEntry entry)
        {
            return Search(bottom, entry) != null;
        }


        public bool Add(TEntry entry)
        {
            return Insert(ref bottom, entry);
        }


        public bool Remove(TEntry entry)
        {
            return Delete(ref bottom, entry);
        }


        public IEnumerator<TEntry> GetEnumerator()
        {
            return Visit(bottom).Select(n => n.entry).GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
                text.AppendLine($"node{index} [label=\"{node.entry}\"]");

                if (node.left != sentinel)
                {
                    int leftIndex = 0;
                    if (!labels.TryGetValue(node.left, out leftIndex))
                    {
                        leftIndex = labels.Count + 1;
                        labels.Add(node.left, leftIndex);
                    }
                    text.AppendLine($"node{index} -> node{leftIndex}");
                }

                if (node.right != sentinel)
                {
                    int rightIndex = 0;
                    if (!labels.TryGetValue(node.right, out rightIndex))
                    {
                        rightIndex = labels.Count + 1;
                        labels.Add(node.right, rightIndex);
                    }
                    text.AppendLine($"node{index} -> node{rightIndex}");
                }
            }

            text.AppendLine("}");
            return text.ToString();
        }


        private IEnumerable<Node> Visit()
        {
            var stack = new Stack<Node>();
            if (bottom != null && bottom != sentinel)
            {
                stack.Push(bottom);
            }

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current.left != sentinel)
                {
                    stack.Push(current.left);
                }
                if (current.right != sentinel)
                {
                    stack.Push(current.right);
                }
                yield return current;
            }
        }

        #endregion

        #region Types

        private class Node
        {
            internal Node left;
            internal Node right;
            internal int level;

            internal TEntry entry;


            public Node()
            {
                entry = default(TEntry);
                left = this;
                right = this;
                level = 0;
            }


            public Node(TEntry entry, Node sentinel)
            {
                this.level = 1;
                this.left = sentinel;
                this.right = sentinel;
                this.entry = entry;
            }
        }

        #endregion
    }
}
