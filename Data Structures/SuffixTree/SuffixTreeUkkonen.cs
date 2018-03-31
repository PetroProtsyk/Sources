using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protsyk.DataStructures
{
    // Suffix Tree construction using Ukkonen's Algortihm implementation, ported from
    // https://www.geeksforgeeks.org/ukkonens-suffix-tree-construction-part-1
    public class SuffixTreeUkkonen : SuffixTree
    {
        #region Fields
        private const int oo = int.MinValue;
        private const int ALPHABET_SIZE = 128;
        private const int MAXN = 2000000;

        private readonly Node[] tree = new Node[2 * MAXN];
        private readonly char[] text = new char[MAXN];

        private int root;
        private int last_added;
        private int pos;
        private int needSL;
        private int remainder;
        private int active_node;
        private int active_e;
        private int active_len;
        #endregion

        #region Constructor
        public SuffixTreeUkkonen(string text)
        {
            Init();
            for (int i = 0; i < text.Length; ++i)
            {
                Extend(text[i]);
            }
            Extend(TerminationCharacter);
            tree[root].next[(int)TerminationCharacter] = 0;

            // Fix 'end' in the leaf nodes
            for (int i = 1; i <= last_added; ++i)
            {
                if (tree[i].end == oo)
                {
                    tree[i].end = text.Length + 1;
                }
            }
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

            var stack = new Stack<int>();
            if (node.Item5 < 0)
            {
                stack.Push(node.Item2);
            }
            else
            {
                stack.Push(node.Item5);
            }

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                var currentNode = tree[current];

                if (IsInternal(currentNode))
                {
                    foreach (var child in currentNode.next)
                    {
                        if (child > 0)
                        {
                            stack.Push(child);
                        }
                    }
                }
                else
                {
                    yield return currentNode.pos;
                }
            }
        }
        #endregion

        #region Methods
        private ValueTuple<bool, int, int, int, int> Navigate(int parent, int from, string substring)
        {
            var node = parent;

            if (string.IsNullOrEmpty(substring))
            {
                return new ValueTuple<bool, int, int, int, int>(true, node, from, 0, -1);
            }

            // Navigate to the end of substring
            var k = from;
            while (true)
            {
                var childIndex = FindChild(substring, tree[node], k);
                if (childIndex <= 0)
                {
                    return new ValueTuple<bool, int, int, int, int>(false, node, k, 0, -1);
                }

                var child = tree[childIndex];
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
                    return new ValueTuple<bool, int, int, int, int>(true, node, k, m, childIndex);
                }
                else if (child.start + m == child.end)
                {
                    if (IsInternal(child))
                    {
                        node = childIndex;
                    }
                    else
                    {
                        return new ValueTuple<bool, int, int, int, int>(false, node, k, m, childIndex);
                    }
                }
                else
                {
                    return new ValueTuple<bool, int, int, int, int>(false, node, k, m, childIndex);
                }
            }
        }

        private int FindChild(string substring, Node node, int k)
        {
            if (node.next[substring[k]] > 0)
            {
                return node.next[substring[k]];
            }

            return -1;
        }
        #endregion

        #region Construction
        private int NewNode(int start, int end = oo)
        {
            Node nd = new Node();
            nd.start = start;
            nd.end = end;
            nd.pos = -1;
            nd.slink = 0;
            for (int i = 0; i < ALPHABET_SIZE; i++)
                nd.next[i] = 0; //TODO: Change to -1?
            tree[++last_added] = nd;
            return last_added;
        }

        private char ActiveEdge()
        {
            return text[active_e];
        }

        private void AddSuffixLink(int node)
        {
            if (needSL > 0)
            {
                tree[needSL].slink = node;
            }
            needSL = node;
        }

        private bool WalkDown(int node)
        {
            if (active_len >= tree[node].edge_length(pos))
            {
                active_e += tree[node].edge_length(pos);
                active_len -= tree[node].edge_length(pos);
                active_node = node;
                return true;
            }
            return false;
        }

        private void Init()
        {
            needSL = 0;
            last_added = 0;
            pos = -1;
            remainder = 0;
            active_node = 0;
            active_e = 0;
            active_len = 0;

            root = NewNode(-1, -1);
            tree[root].slink = -1;

            active_node = root;
        }

        private void Extend(char c)
        {
            text[++pos] = c;
            needSL = 0;
            remainder++;
            while (remainder > 0)
            {
                if (active_len == 0) active_e = pos;
                if (tree[active_node].next[ActiveEdge()] == 0)
                {
                    int leaf = NewNode(pos);
                    tree[leaf].pos = pos - remainder + 1;
                    tree[active_node].next[ActiveEdge()] = leaf;
                    AddSuffixLink(active_node); //rule 2
                }
                else
                {
                    int nxt = tree[active_node].next[ActiveEdge()];
                    if (WalkDown(nxt)) continue; //observation 2
                    if (text[tree[nxt].start + active_len] == c)
                    { //observation 1
                        active_len++;
                        AddSuffixLink(active_node); //observation 3
                        break;
                    }
                    int split = NewNode(tree[nxt].start, tree[nxt].start + active_len);
                    tree[active_node].next[ActiveEdge()] = split;
                    int leaf = NewNode(pos);
                    tree[leaf].pos = pos - remainder + 1;
                    tree[split].next[c] = leaf;
                    tree[nxt].start += active_len;
                    tree[split].next[text[tree[nxt].start]] = nxt;
                    AddSuffixLink(split); //rule 2
                }
                remainder--;
                if (active_node == root && active_len > 0)
                { //rule 1
                    active_len--;
                    active_e = pos - remainder + 1;
                }
                else
                    active_node = tree[active_node].slink > 0 ? tree[active_node].slink : root; //rule 3
            }
        }
        #endregion

        #region Visualization
        public override string ToDotNotation()
        {
            var dotText = new StringBuilder();
            dotText.AppendLine("digraph g {");
            dotText.AppendLine("node[shape = point];");

            var labels = new Dictionary<Node, int>();

            // Nodes
            foreach (var node in Visit())
            {
                int index = GetLabelIndex(labels, node);

                if (!IsInternal(node))
                {
                    dotText.AppendLine($"node{index} [label=\"{node.pos}\", shape = circle]");
                }
                else
                {
                    dotText.AppendLine($"node{index}");

                    foreach (var child in node.next.Where(c => c > 0).Select(c => tree[c]).OrderBy(c => text[c.start]))
                    {
                        int childIndex = GetLabelIndex(labels, child);
                        dotText.AppendLine($"node{index} -> node{childIndex} [label=\"{new string(text, child.start, child.edge_length(pos))}\"]");
                    }
                }

                if (node.slink > 0)
                {
                    int suffixIndex = GetLabelIndex(labels, tree[node.slink]);
                    dotText.AppendLine($"node{index} -> node{suffixIndex} [style=\"dashed\", constraint=false, color=silver]");
                }
            }

            dotText.AppendLine("}");
            return dotText.ToString();
        }

        private static bool IsInternal(Node node)
        {
            return node.next.Any(n => n > 0);
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
            stack.Push(tree[root]);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                foreach (var child in current.next)
                {
                    if (child > 0)
                    {
                        stack.Push(tree[child]);
                    }
                }

                yield return current;
            }
        }
        #endregion

        #region Types
        //TODO: Struct?
        private class Node
        {
            /*
               There is no need to create an "Edge" struct.
               Information about the edge is stored right in the node.
               [start; end) interval specifies the edge,
               by which the node is connected to its parent node.
            */

            public int start, end, slink, pos;
            public int[] next = new int[ALPHABET_SIZE];

            public int edge_length(int pos)
            {
                if (end != oo)
                    return end - start;

                return pos + 1 - start;
            }
        }
        #endregion
    }
}
