using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Protsyk.DataStructures
{
    /// <summary>
    /// Ukkonen O(n^3) algorithm.
    /// As described in the book by D. Gusfield, Algorithms on Strings, Trees and Sequences
    /// in the Section 6.1
    /// </summary>
    public class SuffixTreeUkkonenCubic : SuffixTree
    {
        #region Fields
        private readonly Node root;
        private readonly string text;
        #endregion

        #region Constructor
        public SuffixTreeUkkonenCubic(string inputText)
        {
            text = inputText + TerminationCharacter;
            root = Build(text);
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

                if (IsInternal(current))
                {
                    foreach (var child in current.children)
                    {
                        stack.Push(child);
                    }
                }
                else
                {
                    yield return current.pos;
                }
            }
        }
        #endregion

        #region Methods
        private ValueTuple<bool, Node, int, int, int> Navigate(Node parent, int from, string substring)
        {
            var node = parent;

            if (string.IsNullOrEmpty(substring))
            {
                return new ValueTuple<bool, Node, int, int, int>(true, node, from, 0, -1);
            }

            // Navigate to the end of substring
            var k = from;
            while (true)
            {
                var childIndex = FindChild(substring, node, k);
                if (childIndex < 0)
                {
                    return new ValueTuple<bool, Node, int, int, int>(false, node, k, 0, -1);
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
                    return new ValueTuple<bool, Node, int, int, int>(true, node, k, m, childIndex);
                }
                else if (child.start + m == child.end)
                {
                    if (!IsInternal(child))
                    {
                        return new ValueTuple<bool, Node, int, int, int>(false, node, k, m, childIndex);
                    }
                    node = child;
                }
                else
                {
                    return new ValueTuple<bool, Node, int, int, int>(false, node, k, m, childIndex);
                }
            }
        }

        private int FindChild(string substring, Node node, int k)
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

        private static bool IsInternal(Node node)
        {
            return node.children.Count > 0;
        }
        #endregion

        #region Construction
        private Node Build(string text)
        {
            var builder = new UkkonenBuilder(text);
            return builder.Build();
        }

        private class UkkonenBuilder
        {
            private readonly string text;
            private readonly Node root;

            public UkkonenBuilder(string text)
            {
                this.text = text;
                this.root = new Node();
            }

            public Node Build()
            {
                for (int i = 0; i < text.Length; ++i)
                {
                    // Phase i+1
                    for (int j = 0; j < i; ++j)
                    {
                        // Extension j
                        Extend(j, i);
                    }

                    // Do not put TerminationCharacter to the tree
                    if (i < text.Length - 1)
                    {
                        // Extend empty suffix, by putting the next character to the tree
                        ConstructT(i);
                    }
                }

                return root;
            }

            private void ConstructT(int t)
            {
                var childIndex = FindChild(root.children, text[t]);
                if (childIndex >= 0)
                {
                    return;
                }

                var newNode = new Node
                {
                    start = t,
                    end = t + 1,
                    pos = t
                };

                root.children.Add(newNode);
            }

            private int FindChild(IList<Node> children, char c)
            {
                for (int i=0; i< children.Count; ++i)
                {
                    if (text[children[i].start] == c)
                    {
                        return i;
                    }
                }
                return -1;
            }

            private void Extend(int from, int to)
            {
                // Navigate to the end of substring
                var node = root;
                var k = from;
                while (k < to)
                {
                    var childIndex = FindChild(node.children, text[k]);
                    if (childIndex < 0)
                    {
                        throw new Exception("What?");
                    }

                    var child = node.children[childIndex];
                    var m = 0;

                    while (child.start + m < child.end &&
                           k < to &&
                           text[child.start + m] == text[k])
                    {
                        m++;
                        k++;
                    }

                    if (k == to)
                    {
                        if (child.start + m == child.end)
                        {
                            if (IsInternal(child))
                            {
                                if (FindChild(child.children, text[k]) >= 0)
                                {
                                    ApplyRule3();
                                }
                                else
                                {
                                    child.children.Add(new Node
                                    {
                                        start = k,
                                        end = to + 1,
                                        pos = from
                                    });
                                }
                            }
                            else
                            {
                                ApplyRule1(child, to + 1);
                            }
                        }
                        else
                        {
                            if (text[child.start + m] == text[k])
                            {
                                ApplyRule3();
                            }
                            else
                            {
                                ApplyRule2(node, childIndex, m, k, from, to + 1);
                            }
                        }
                    }
                    else if (child.start + m == child.end)
                    {
                        if (IsInternal(child))
                        {
                            node = child;
                        }
                        else
                        {
                            throw new Exception("What?");
                        }
                    }
                    else
                    {
                        throw new Exception("What?");
                    }
                }
            }

            private void ApplyRule1(Node leaf, int newEnd)
            {
                //Rule 1. Path ends at a leaf. Extend label
                leaf.end = newEnd;
            }

            private void ApplyRule2(Node parent, int childIndex, int m, int k, int pos, int to)
            {
                //Rule 2. Split label and add new leaf
                var child = parent.children[childIndex];

                // 1) replace child with internal node
                var newParent = new Node
                {
                    start = child.start,
                    end = child.start + m,
                };

                parent.children[childIndex] = newParent;

                // 2) adjust start position of the child and add it to the new internal node as a child
                child.start += m;
                newParent.children.Add(child);

                // 3) add the rest of the suffix as a new child
                newParent.children.Add(new Node
                {
                    start = k,
                    end = to,
                    pos = pos
                });
            }

            private void ApplyRule3()
            {
                //Rule 3. Suffix is already in the tree
                // Do nothing
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
                int index = GetLabelIndex(labels, node);

                if (!IsInternal(node))
                {
                    dotText.AppendLine($"node{index} [label=\"{node.pos}\"]");
                }
                else
                {
                    dotText.AppendLine($"node{index} [label=\"\"]");

                    foreach (var child in node.children.OrderBy(c => text[c.start]))
                    {
                        int childIndex = GetLabelIndex(labels, child);
                        dotText.AppendLine($"node{index} -> node{childIndex} [label=\"{text.Substring(child.start, child.end - child.start)}\"]");
                    }
                }

            }

            dotText.AppendLine("}");
            return dotText.ToString();
        }

        private static int GetLabelIndex(Dictionary<Node, int> labels, Node node)
        {
            if (!labels.TryGetValue(node, out var index))
            {
                index = labels.Count + 1;
                labels.Add(node, index);
            }

            return index;
        }

        private IEnumerable<Node> Visit()
        {
            var stack = new Stack<Node>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                foreach (var child in current.children)
                {
                    stack.Push(child);
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

            // Leaf
            public int pos;

            // Internal
            public readonly IList<Node> children = new List<Node>();
        }
        #endregion
    }
}
