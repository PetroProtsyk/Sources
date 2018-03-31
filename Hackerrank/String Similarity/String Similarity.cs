using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ccs
{
    // https://www.hackerrank.com/challenges/string-similarity/problem
    class Program
    {
        static void Main(string[] args)
        {
            var t = int.Parse(Console.ReadLine());
            for (int i=0; i<t; ++i)
            {
                var txt = Console.ReadLine();
                var st = new SuffixTreeUkkonenV3(txt);

                long s = 0;
                int mk = 0;
                st.VisitInternal((n,k)=>
                {
                    s += k * n.leafs;
                    mk += k;
                });
                Console.WriteLine(s + txt.Length - mk);
            }
        }
    }

    public class SuffixTreeUkkonenV3
    {
        #region Fields
        private static readonly int currentPosition = int.MinValue;

        private readonly Node root;
        private readonly string text;
        #endregion

        #region Constructor
        public SuffixTreeUkkonenV3(string inputText)
        {
            text = inputText + '$';
            root = Build(text);
        }
        #endregion

        #region Api
        public void VisitInternal(Action<Node, int> handle)
        {
            Navigate(root, 0, text.Length, text, text, true, handle);
        }
        #endregion

        #region Methods
        private static Location Navigate(Node parent, int from, int to, string substring, string text, bool useSkipCount, Action<Node, int> handle = null)
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
                    if (!HasChildren(child))
                    {
                        return new Location(false, node, k, m, childIndex);
                    }
                    else
                    {
                      handle?.Invoke(child, m);
                      node = child;
                    }
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

        private static bool HasChildren(Node node)
        {
            return node.children != null;
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
                this.root = new Node
                {
                    children = new List<Node>()
                };
            }

            public Node Build()
            {
                activeLeaf = ConstructT(0);
                var next = new Next(root, 1, 1, 0);

                for (int i = 1; i < text.Length; ++i)
                {
                    nodeThatRequireSuffixLink = null;

                    // Phase i+1

                    // So the first extension of any phase is special and only takes constant time since the algorithm
                    // has a pointer to the end of the current full string
                    // I.e. when j = 0
                    ApplyRule1(activeLeaf, i + 1);

                    while (next.j < i)
                    {
                        var location = Navigate(next.node, next.from, i, text, text, true);
                        next = ApplyRule(location, next.j, i);

                        if (next.rule == 3)
                        {
                            // Rule 3 stops current phase
                            break;
                        }
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

                CountLeafs(root);

                return root;
            }

            private struct Next
            {
                public Node node;
                public int j;
                public int from;
                public int rule;

                public Next(Node node, int j, int from, int rule)
                {
                    this.node = node;
                    this.j = j;
                    this.from = from;
                    this.rule = rule;
                }
            }

            private Next ApplyRule(Location location, int j, int i)
            {
                if (location.childIndex == -1)
                {
                    ApplyRule2(location.parent, -1, location.offsetInEdge, location.offsetInString, j);

                    if (location.parent.suffixLink == null)
                    {
                        return new Next(root, j + 1, j + 1, 2);
                    }
                    else
                    {
                        return new Next(location.parent.suffixLink, j + 1, i, 2);
                    }
                }

                if (location.offsetInString != i)
                {
                    throw new Exception("What?");
                }

                var child = location.parent.children[location.childIndex];
                if (child.start + location.offsetInEdge == end(child, i + 1))
                {
                    if (HasChildren(child))
                    {
                        if (FindChild(child.children, text[i]) >= 0)
                        {
                            ApplyRule3();

                            return new Next(location.parent,
                                        j,
                                        i - location.offsetInEdge,
                                        3);
                        }
                        else
                        {
                            ApplyRule2(child, -1, 0, location.offsetInString, j);

                            if (child.suffixLink != null)
                            {
                                return new Next(child.suffixLink.parent,
                                                j + 1,
                                                i - (end(child.suffixLink, i + 1) - child.suffixLink.start),
                                                2);
                            }
                            else if (location.parent.suffixLink == null)
                            {
                                return new Next(root, j + 1, j + 1, 2);
                            }
                            else
                            {
                                return new Next(location.parent.suffixLink,
                                            j + 1,
                                            i - (end(child, i + 1) - child.start),
                                            2);
                            }
                        }
                    }
                    else
                    {
                        ApplyRule1(child, i + 1);

                        if (location.parent.suffixLink == null)
                        {
                            return new Next(root, j + 1, j + 1, 1);
                        }
                        else
                        {
                            return new Next(location.parent.suffixLink,
                                        j + 1,
                                        i - location.offsetInEdge,
                                        1);
                        }
                    }
                }
                else
                {
                    if (text[child.start + location.offsetInEdge] == text[i])
                    {
                        ApplyRule3();

                        return new Next(location.parent,
                                        j,
                                        i - location.offsetInEdge,
                                        3);
                    }
                    else
                    {
                        ApplyRule2(location.parent, location.childIndex, location.offsetInEdge, location.offsetInString, j);

                        if (location.parent.suffixLink == null)
                        {
                            if (location.parent.parent != null && location.parent.parent.suffixLink != null)
                            {
                                return new Next(location.parent.parent.suffixLink,
                                                j + 1,
                                                i - location.offsetInEdge - (end(location.parent, i + 1) - location.parent.start),
                                                2);
                            }
                            else
                            {
                                return new Next(root, j + 1, j + 1, 2);
                            }
                        }
                        else
                        {
                            return new Next(location.parent.suffixLink,
                                        j + 1,
                                        i - location.offsetInEdge,
                                        2);
                        }
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
                    pos = t,
                    parent = root
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

            private void ApplyRule2(Node parent, int childIndex, int m, int k, int pos)
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
                        parent = parent,

                        suffixLink = null,
                        children = new List<Node>()
                    };

                    parent.children[childIndex] = newParent;

                    // 2) adjust start position of the child and add it to the new internal node as a child
                    child.start += m;
                    child.parent = newParent;
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
                if (newParent.children == null)
                {
                    newParent.children = new List<Node>();
                }

                newParent.children.Add(new Node
                {
                    start = k,
                    end = currentPosition,
                    parent = newParent,

                    pos = pos
                });
            }

            private void ApplyRule3()
            {
                //Rule 3. Suffix is already in the tree
                // Do nothing
            }

            private static void CountLeafs(Node root)
            {
                var s = new Stack<ValueTuple<Node, int>>();
                s.Push(new ValueTuple<Node, int>(root, 0));

                while (s.Count > 0)
                {
                    var c = s.Pop();

                    if (c.Item2 == 0)
                    {
                    if (c.Item1.children != null && c.Item1.children.Count > 0)
                    {
                        s.Push(new ValueTuple<Node, int>(c.Item1, 1));

                        foreach(var child in c.Item1.children)
                        {
                        s.Push(new ValueTuple<Node, int>(child, 0));
                        }
                    }
                    else
                    {
                        c.Item1.leafs = 1;
                    }
                    }
                    else
                    {
                        foreach(var child in c.Item1.children)
                        {
                        c.Item1.leafs += child.leafs;
                        }
                    }

                }

                /*
                if (root.leafs > 0)
                {
                    return root.leafs;
                }

                if (root.children==null || root.children.Count == 0)
                {
                    root.leafs = 1;
                    return 1;
                }

                int leafs = 0;
                foreach(var child in root.children)
                {
                    leafs += CountLeafs(child);
                }
                
                root.leafs = leafs;
                return leafs;*/
            }
        }

        private static IEnumerable<Node> Visit(Node root)
        {
            var stack = new Stack<Node>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current.children!=null)
                foreach (var child in current.children)
                {
                    stack.Push(child);
                }

                yield return current;
            }
        }

        #endregion

        #region Types
        public class Node
        {
            public int start;
            public int end;
            public int leafs;
            public Node parent;

            // Leaf
            public int pos;

            // Internal
            public Node suffixLink;
            public IList<Node> children;
        }
        #endregion
    }
}
