using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//Required file - AutomatonLevenshtein.cs
using SEA2016.FuzzySearch;

namespace Protsyk.Collections
{
    /// <summary>
    /// Trie Data Structure
    /// </summary>
    public class Trie<T>
    {
        #region Fields
        private readonly INode root;
        private readonly IComparer<T> comparer;
        private int count;
        #endregion

        #region Properties

        public int Count
        {
            get { return count; }
        }

        #endregion

        #region Constructors

        public Trie()
            : this(Comparer<T>.Default)
        {
        }

        public Trie(IComparer<T> comparer)
        {
            this.count = 0;
            this.comparer = comparer;
            this.root = new Node();
        }
        #endregion

        #region Methods

        public bool Add(IEnumerable<T> item)
        {
            var node = root;
            foreach (var part in item)
            {
                var added = node.Add(part, out node);
            }

            if (!node.IsFinal)
            {
                node.IsFinal = true;
                count++;
                return true;
            }

            return false;
        }


        public bool Contains(IEnumerable<T> item)
        {
            // Can be also
            //return Match(new SequenceMatcher<T>(item), TrieOrder.None).Any();

            var node = root;
            foreach (var part in item)
            {
                node = node.Find(part);
                if (node == null)
                {
                    return false;
                }
            }
            return node.IsFinal;
        }


        public IEnumerable<TrieMatch<T>> Match(ITrieMatcher<T> matcher, TrieOrder order)
        {
            var stack = new Stack<IEnumerator<ChildLink>>();
            stack.Push(GetChildrenFromNode(root, order).GetEnumerator());

            var result = new List<T>();

            try
            {
                while (stack.Count > 0)
                {
                    var current = stack.Peek();
                    if (!current.MoveNext())
                    {
                        if (result.Count > 0)
                        {
                            matcher.Pop();
                            result.RemoveAt(result.Count - 1);
                        }

                        current.Dispose();
                        stack.Pop();
                        continue;
                    }

                    var key = current.Current.Label;
                    var match = matcher.Next(key);

                    if (match)
                    {
                        result.Add(key);

                        //TODO: Improve performance by checking only intersection of next characters from matcher and children

                        var matchNode = current.Current.Node;
                        stack.Push(GetChildrenFromNode(matchNode, order).GetEnumerator());

                        if (matchNode.IsFinal && matcher.IsFinal())
                        {
                            yield return new TrieMatch<T>(result);
                        }
                    }
                }
            }
            finally
            {
                while (stack.Count > 0)
                {
                    var current = stack.Peek();
                    if (result.Count > 0)
                    {
                        matcher.Pop();
                        result.RemoveAt(result.Count - 1);
                    }

                    current.Dispose();
                    stack.Pop();
                }
            }
        }


        private IEnumerable<ChildLink> GetChildrenFromNode(INode node, TrieOrder order)
        {
            var result = node.Children;
            if (order == TrieOrder.Asc)
            {
                result = result.OrderBy(c => c.Label, comparer);
            }
            else if (order == TrieOrder.Desc)
            {
                result = result.OrderByDescending(c => c.Label, comparer);
            }
            return result;
        }
        #endregion

        #region Visualization

        public string ToDotNotation(TrieOrder order)
        {
            var text = new StringBuilder();
            text.AppendLine("digraph g {");
            text.AppendLine("node[shape = circle];");

            var labels = new Dictionary<INode, int>();
            // Nodes
            labels.Add(root, 1);
            text.AppendLine($"node1[label=\"root\"]");

            foreach (var childLink in GetChildrenFromNode(root, order))
            {
                int childIndex = 0;
                if (!labels.TryGetValue(childLink.Node, out childIndex))
                {
                    childIndex = labels.Count + 1;
                    labels.Add(childLink.Node, childIndex);
                }
                text.AppendLine($"node1 -> node{childIndex} [label=\"&nbsp;{childLink.Label}\"]");
            }

            foreach (var link in Visit(order))
            {
                int index = 0;
                if (!labels.TryGetValue(link.Node, out index))
                {
                    index = labels.Count + 1;
                    labels.Add(link.Node, index);
                }

                if (link.Node.IsFinal)
                {
                    text.AppendLine($"node{index}[label=\"*\"]");
                }
                else
                {
                    text.AppendLine($"node{index}[label=\"\"]");
                }

                foreach (var childLink in GetChildrenFromNode(link.Node, order))
                {
                    int childIndex = 0;
                    if (!labels.TryGetValue(childLink.Node, out childIndex))
                    {
                        childIndex = labels.Count + 1;
                        labels.Add(childLink.Node, childIndex);
                    }
                    text.AppendLine($"node{index} -> node{childIndex} [label=\"&nbsp;{childLink.Label}\"]");
                }
            }

            text.AppendLine("}");
            return text.ToString();
        }


        private IEnumerable<ChildLink> Visit(TrieOrder order)
        {
            var stack = new Stack<ChildLink>();

            foreach (var child in GetChildrenFromNode(root, order))
            {
                stack.Push(child);
            }

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                foreach (var child in GetChildrenFromNode(current.Node, order))
                {
                    stack.Push(child);
                }

                yield return current;
            }
        }

        #endregion

        #region Types

        private interface INode
        {
            IEnumerable<ChildLink> Children { get; }

            bool IsFinal { get; set; }

            bool Add(T label, out INode node);

            INode Find(T label);
        }

        private struct ChildLink
        {
            /// <summary>
            /// Label of the link
            /// </summary>
            public readonly T Label;

            /// <summary>
            /// Children, associated with the label
            /// </summary>
            public readonly INode Node;

            public ChildLink(T label, INode node)
            {
                this.Label = label;
                this.Node = node;
            }
        }


        private class Node : INode
        {
            private readonly List<ChildLink> children = new List<ChildLink>();


            public IEnumerable<ChildLink> Children
            {
                get
                {
                    return children;
                }
            }

            public bool IsFinal { get; set; }

            public bool Add(T label, out INode node)
            {
                node = Find(label);
                if (node != null)
                {
                    return false;
                }

                node = new Node();
                children.Add(new ChildLink(label, node));
                return true;
            }

            public INode Find(T label)
            {
                foreach (var child in children)
                {
                    if (Equals(child.Label, label))
                    {
                        return child.Node;
                    }
                }

                return null;
            }
        }

        #endregion
    }

    public enum TrieOrder
    {
        None,
        Asc,
        Desc
    }

    public interface ITrieMatcher<in T>
    {
        void Reset();

        bool Next(T p);

        bool IsFinal();

        void Pop();
    }

    public struct TrieMatch<T>
    {
        public IEnumerable<T> Value { get; }

        public TrieMatch(IEnumerable<T> value)
        {
            this.Value = value;
        }
    }

    /// <summary>
    /// Match all elements in Trie
    /// </summary>
    public class AnyMatcher<T> : ITrieMatcher<T>
    {
        public void Reset()
        {
        }


        public bool Next(T p)
        {
            return true;
        }


        public bool IsFinal()
        {
            return true;
        }


        public void Pop()
        {
        }
    }

    /// <summary>
    /// Match single element in Trie (word)
    /// </summary>
    public class SequenceMatcher<T> : ITrieMatcher<T>
    {
        private readonly T[] items;
        private int index;


        public SequenceMatcher(IEnumerable<T> items)
        {
            this.items = items.ToArray();
            this.index = -1;
        }


        public void Reset()
        {
            index = -1;
        }


        public bool IsFinal()
        {
            return index + 1 == items.Length;
        }


        public bool Next(T p)
        {
            var next = index + 1;
            if (next >= items.Length)
            {
                return false;
            }

            if (Equals(items[next], p))
            {
                index = next;
                return true;
            }

            return false;
        }


        public void Pop()
        {
            if (index == -1)
            {
                throw new InvalidOperationException();
            }
            --index;
        }
    }

    /// <summary>
    /// Match all elements in the Trie within given Levenshtein distance
    /// from the target element.
    /// 
    /// Works for Tries built from a set of strings.
    /// </summary>
    public class LevenshteinMatcher : ITrieMatcher<char>
    {
        private readonly DFA dfa;
        private int current;
        private int[] states;

        public LevenshteinMatcher(string pattern, int degree)
        {
            this.dfa = LevenshteinAutomaton.CreateAutomaton(pattern, degree).Determinize();
            this.states = new int[pattern.Length + degree + 1];
        }


        public void Reset()
        {
            current = 0;
            states[current] = 0;
        }


        public bool IsFinal()
        {
            if (current < 1)
            {
                return dfa.IsFinal(0);
            }
            return dfa.IsFinal(states[current - 1]);
        }


        public bool Next(char p)
        {
            var next = dfa.Next(current == 0 ? 0 : states[current - 1], p);
            if (next == -1)
            {
                return false;
            }

            states[current++] = next;
            return true;
        }

        public void Pop()
        {
            if (current == 0)
            {
                throw new InvalidOperationException();
            }
            --current;
        }
    }
}