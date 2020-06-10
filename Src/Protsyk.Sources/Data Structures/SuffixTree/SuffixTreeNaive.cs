using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Protsyk.DataStructures
{
    /// <summary>
    /// Naive construction. O(n^2)
    /// As described in the book by D. Gusfield, Algorithms on Strings, Trees and Sequences
    /// in the Section 5.4. A naive algorithm to build a suffix tree
    /// </summary>
    public class SuffixTreeNaive : SuffixTree
    {
        #region Fields
        private readonly InternalNode root;
        private readonly string text;
        #endregion

        #region Constructor
        public SuffixTreeNaive(string text)
        {
            this.root = new InternalNode();
            this.text = text + TerminationCharacter;
            Build();
        }
        #endregion

        #region Api
        public override bool IsMatch(string substring)
        {
            return Match(substring).Any();
        }

        public override IEnumerable<int> Match(string substring)
        {
            var node = Navigate(root, 0, substring);
            if (!node.Item1)
            {
                yield break;
            }

            var stack = new Stack<Node>();
            if (node.Item5 < 0)
            {
                stack.Push(node.Item2);
            }
            else
            {
                stack.Push(node.Item2.children[node.Item5]);
            }

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                var internalNode = current as InternalNode;
                if (internalNode != null)
                {
                    foreach (var child in internalNode.children)
                    {
                        stack.Push(child);
                    }
                }

                var leafNode = current as LeafNode;
                if (leafNode != null)
                {
                    yield return leafNode.pos;
                }
            }
        }
        #endregion

        #region Methods
        private ValueTuple<bool, InternalNode, int, int, int> Navigate(InternalNode parent, int from, string substring)
        {
            var node = parent;

            if (string.IsNullOrEmpty(substring))
            {
                return new ValueTuple<bool, InternalNode, int, int, int>(true, node, from, 0, -1);
            }

            // Navigate to the end of substring
            var k = from;
            while (true)
            {
                var childIndex = FindChild(substring, node, k);
                if (childIndex < 0)
                {
                    return new ValueTuple<bool, InternalNode, int, int, int>(false, node, k, 0, -1);
                }

                var child = node.children[childIndex];
                var m = 0;

                while (child.start + m < child.end &&
                       k < substring.Length &&
                       text[child.start + m] == substring[k])
                {
                    m++;
                    k++;
                }

                if (k == substring.Length)
                {
                    return new ValueTuple<bool, InternalNode, int, int, int>(true, node, k, m, childIndex);
                }
                else if (child.start + m == child.end)
                {
                    if (!(child is InternalNode))
                    {
                        return new ValueTuple<bool, InternalNode, int, int, int>(false, node, k, m, childIndex);
                    }
                    node = (InternalNode)child;
                }
                else
                {
                    return new ValueTuple<bool, InternalNode, int, int, int>(false, node, k, m, childIndex);
                }
            }
        }

        private int FindChild(string substring, InternalNode node, int k)
        {
            for (int i = 0; i < node.children.Count; ++i)
            {
                var child = node.children[i];
                if (text[child.start] == substring[k])
                {
                    return i;
                }
            }
            return -1;
        }
        #endregion

        #region Construction
        private void Build()
        {
            for (int i = 0; i < text.Length - 1; ++i)
            {
                AddSuffixFrom(root, i, i);
            }
        }

        private void AddSuffixFrom(InternalNode parent, int suffixStart, int offset)
        {
            var result = Navigate(parent, offset, text);
            int k = result.Item3;
            int m = result.Item4;
            int childIndex = result.Item5;
            parent = result.Item2;

            if (childIndex < 0)
            {
                // No child matched, create new child
                parent.children.Add(new LeafNode
                {
                    start = k,
                    end = text.Length,
                    pos = suffixStart
                });
            }
            else
            {
                // Split label

                var child = parent.children[childIndex];

                // 1) replace child with internal node
                var newParent = new InternalNode
                {
                    start = child.start,
                    end = child.start + m,
                };

                parent.children[childIndex] = newParent;

                // 2) adjust start position of the child and add it to the new internal node as a child
                child.start += m;
                newParent.children.Add(child);

                // 3) add the rest of the suffix as a new child
                AddSuffixFrom(newParent, suffixStart, k);
            }
        }
        #endregion

        #region Visualization

        public override string ToDotNotation()
        {
            var dotText = new StringBuilder();
            dotText.AppendLine("digraph g {");
            dotText.AppendLine("node[shape = circle];");

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

                var leafNode = node as LeafNode;
                if (leafNode != null)
                {
                    dotText.AppendLine($"node{index} [label=\"{leafNode.pos}\"]");
                }

                var internalNode = node as InternalNode;
                if (internalNode != null)
                {
                    dotText.AppendLine($"node{index} [label=\"\"]");

                    foreach (var child in internalNode.children.OrderBy(c => text[c.start]))
                    {
                        int childIndex = 0;
                        if (!labels.TryGetValue(child, out childIndex))
                        {
                            childIndex = labels.Count + 1;
                            labels.Add(child, childIndex);
                        }

                        dotText.AppendLine($"node{index} -> node{childIndex} [label=\"{text.Substring(child.start, child.end - child.start)}\"]");
                    }
                }

            }

            dotText.AppendLine("}");
            return dotText.ToString();
        }


        private IEnumerable<Node> Visit()
        {
            var stack = new Stack<Node>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                var internalNode = current as InternalNode;
                if (internalNode != null)
                {
                    foreach (var child in internalNode.children)
                    {
                        stack.Push(child);
                    }
                }

                yield return current;
            }
        }

        #endregion

        #region Types
        class Node
        {
            public int start;
            public int end;
        }

        class LeafNode : Node
        {
            public int pos;
        }

        class InternalNode : Node
        {
            public readonly IList<Node> children = new List<Node>();
        }
        #endregion
    }
}
