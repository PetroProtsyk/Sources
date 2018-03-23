using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Protsyk.DataStructures
{
    /// <summary>
    /// Ukkonen linear time O(n) algorithm
    /// As described in the book by D. Gusfield, Algorithms on Strings, Trees and Sequences
    /// </summary>
    public class SuffixTreeUkkonenLinear : SuffixTree
    {
        #region Fields
        private static readonly int currentPosition = int.MinValue;

        private readonly Node root;
        private readonly string text;
        #endregion

        #region Constructor
        public SuffixTreeUkkonenLinear(string inputText)
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
            var node = Navigate(root, 0, substring.Length, substring, text, false);
            if (!node.isFound)
            {
                yield break;
            }

            var stack = new Stack<Node>();
            if (node.childIndex < 0)
            {
                stack.Push(node.parent);
            }
            else
            {
                stack.Push(node.parent.children[node.childIndex]);
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
        private static Location Navigate(Node parent, int from, int to, string substring, string text, bool useSkipCount)
        {
            var node = parent;

            if (from == to)
            {
                return new Location(true, node, from, 0, -1);
            }

            // Navigate to the end of substring
            var k = from;
            while (true)
            {
                var childIndex = FindChild(substring[k], node, text);
                if (childIndex < 0)
                {
                    return new Location(false, node, k, 0, -1);
                }

                var child = node.children[childIndex];
                var m = 0;

                if (useSkipCount)
                {
                    var skip = Math.Min(to - k, end(child, to + 1) - child.start);
                    m += skip;
                    k += skip;
                }
                else
                {
                    while (child.start + m < end(child, to + 1) &&
                           k < to &&
                           text[child.start + m] == substring[k])
                    {
                        ++m;
                        ++k;
                    }
                }

                if (k == to)
                {
                    return new Location(true, node, k, m, childIndex);
                }
                else if (child.start + m == end(child, to + 1))
                {
                    if (!IsInternal(child))
                    {
                        return new Location(false, node, k, m, childIndex);
                    }
                    node = child;
                }
                else
                {
                    return new Location(false, node, k, m, childIndex);
                }
            }
        }

        private static int end(Node node, int pos)
        {
            if (node.end == currentPosition)
                return pos - 1;

            return node.end;
        }

        struct Location
        {
            public bool isFound;
            public Node parent;
            public int offsetInString;
            public int offsetInEdge;
            public int childIndex;

            public Location(bool isFound, Node parent, int offsetInString, int offsetInEdge, int childIndex)
            {
                this.isFound = isFound;
                this.parent = parent;
                this.offsetInString = offsetInString;
                this.offsetInEdge = offsetInEdge;
                this.childIndex = childIndex;
            }
        }


        private static int FindChild(char c, Node node, string text)
        {
            for (int i = 0; i < node.children.Count; ++i)
            {
                var child = node.children[i];
                if (text[child.start] == c)
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

            private Node activeLeaf;
            private Node nodeThatRequireSuffixLink;

            public UkkonenBuilder(string text)
            {
                this.text = text;
                this.root = new Node();
            }

            public Node Build()
            {
                activeLeaf = ConstructT(0);
                var nk = 1;

                for (int i = 1; i < text.Length; ++i)
                {
                    nodeThatRequireSuffixLink = null;

                    // Phase i+1

                    // So the first extension of any phase is special and only takes constant time since the algorithm
                    // has a pointer to the end of the current full string
                    // I.e. when j = 0
                    ApplyRule1(activeLeaf, i + 1);

                    var next = default(Next);
                    for (int j = nk; j < i; ++j)
                    {
                        if (next != null && next.node.suffixLink != null)
                        {
                            var location = Navigate(next.node.suffixLink, i - next.len, i, text, text, true);
                            next = ApplyRule(location, j, i);
                        }
                        else
                        {
                            var location = Navigate(root, j, i, text, text, true);
                            next = ApplyRule(location, j, i);
                        }

                        if (next.rule == 3)
                        {
                            // Rule 3 stops
                            nk = j;
                            break;
                        }
                    }

                    if (next == null || next.rule != 3)
                    {
                        nk = 1;
                    }

                    // Do not put TerminationCharacter to the tree
                    if (i < text.Length - 1)
                    {
                        // Extend empty suffix, by putting the next character to the tree
                        ConstructT(i);
                    }
                }

                foreach (var node in Visit(root))
                {
                    if (node.end == currentPosition)
                    {
                        node.end = text.Length;
                    }
                }

                return root;
            }

            private class Next
            {
                public Node node;
                public int len;
                public int rule;

                public Next(Node node, int len, int rule)
                {
                    this.node = node;
                    this.len = len;
                    this.rule = rule;
                }
            }

            private Next ApplyRule(Location location, int j, int i)
            {
                if (location.childIndex == -1)
                {
                    ApplyRule2(location.parent, -1, location.offsetInEdge, location.offsetInString, j, i + 1);
                    return new Next(location.parent, 0, 2);
                }

                if (location.offsetInString != i)
                {
                    throw new Exception("What?");
                }

                var child = location.parent.children[location.childIndex];
                if (child.start + location.offsetInEdge == end(child, i + 1))
                {
                    if (IsInternal(child))
                    {
                        if (FindChild(child.children, text[i]) >= 0)
                        {
                            ApplyRule3();
                            return new Next(location.parent, end(child, i + 1) - child.start, 3);
                        }
                        else
                        {
                            ApplyRule2(child, -1, 0, location.offsetInString, j, i + 1);
                            return new Next(location.parent, end(child, i + 1) - child.start, 2);
                        }
                    }
                    else
                    {
                        ApplyRule1(child, i + 1);
                        return new Next(location.parent, location.offsetInEdge, 1);
                    }
                }
                else
                {
                    if (text[child.start + location.offsetInEdge] == text[i])
                    {
                        ApplyRule3();
                        return new Next(location.parent, location.offsetInEdge, 3);
                    }
                    else
                    {
                        ApplyRule2(location.parent, location.childIndex, location.offsetInEdge, location.offsetInString, j, i + 1);
                        return new Next(location.parent, location.offsetInEdge, 2);
                    }
                }
            }

            private Node ConstructT(int t)
            {
                var childIndex = FindChild(root.children, text[t]);
                if (childIndex >= 0)
                {
                    return root.children[childIndex];
                }

                var newNode = new Node
                {
                    start = t,
                    end = currentPosition,
                    pos = t
                };

                root.children.Add(newNode);
                return newNode;
            }

            private int FindChild(IList<Node> children, char c)
            {
                for (int i = 0; i < children.Count; ++i)
                {
                    if (text[children[i].start] == c)
                    {
                        return i;
                    }
                }
                return -1;
            }

            private void ApplyRule1(Node leaf, int newEnd)
            {
                //Rule 1. Path ends at a leaf. Extend implicitly
            }

            private void ApplyRule2(Node parent, int childIndex, int m, int k, int pos, int to)
            {
                var newParent = default(Node);
                if (childIndex >= 0)
                {
                    //Rule 2. Split label and add new leaf
                    var child = parent.children[childIndex];

                    // 1) replace child with internal node
                    newParent = new Node
                    {
                        start = child.start,
                        end = child.start + m,
                        suffixLink = null
                    };

                    parent.children[childIndex] = newParent;

                    // 2) adjust start position of the child and add it to the new internal node as a child
                    child.start += m;
                    newParent.children.Add(child);

                    // Assign suffix link
                    {
                        // Corollary 6.1.1. In Ukkonen’s algorithm, any newly created internal node will have a
                        // suffix link from it by the end of the next extension.
                        //
                        // I.e. if an edge is split, which means a new node was created, and if that was not the first
                        // node created during the current phase, then connect the previously inserted node and the new node
                        // through a suffix link.
                        if (nodeThatRequireSuffixLink != null)
                        {
                            if (nodeThatRequireSuffixLink.suffixLink != null)
                            {
                                throw new Exception("What?");
                            }
                            nodeThatRequireSuffixLink.suffixLink = newParent;
                        }
                        nodeThatRequireSuffixLink = newParent;
                    }
                }
                else
                {
                    newParent = parent;
                }

                // 3) add the rest of the suffix as a new child
                newParent.children.Add(new Node
                {
                    start = k,
                    end = currentPosition,
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
            foreach (var node in Visit(root))
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
                        dotText.AppendLine($"node{index} -> node{childIndex} [label=\"{text.Substring(child.start, end(child, text.Length + 1) - child.start)}\"]");
                    }
                }

                if (node.suffixLink != null && node.suffixLink != root)
                {
                    int suffixIndex = GetLabelIndex(labels, node.suffixLink);
                    dotText.AppendLine($"node{index} -> node{suffixIndex} [style=\"dashed\", constraint=false, color=silver]");
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

        private static IEnumerable<Node> Visit(Node root)
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

            // Suffix Link
            public Node suffixLink;

            // Leaf
            public int pos;

            // Internal
            public readonly IList<Node> children = new List<Node>();
        }
        #endregion
    }
}
