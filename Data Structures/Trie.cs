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
            var stack = new Stack<KeyValuePair<INode, IEnumerator<KeyValuePair<T, INode>>>>();
            stack.Push(new KeyValuePair<INode, IEnumerator<KeyValuePair<T, INode>>>(root, GetChildrenFromNode(root, order).GetEnumerator()));

            var result = new List<T>();

            try
            {
                while (stack.Count > 0)
                {
                    var current = stack.Peek();
                    if (!current.Value.MoveNext())
                    {
                        if (result.Count > 0)
                        {
                            matcher.Pop();
                            result.RemoveAt(result.Count - 1);
                        }

                        current.Value.Dispose();
                        stack.Pop();
                        continue;
                    }

                    var key = current.Value.Current.Key;
                    var match = matcher.Next(key);

                    if (match)
                    {
                        result.Add(key);

                        var matchNode = current.Value.Current.Value;
                        stack.Push(
                            new KeyValuePair<INode, IEnumerator<KeyValuePair<T, INode>>>(matchNode, GetChildrenFromNode(matchNode, order).GetEnumerator()));

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

                    current.Value.Dispose();
                    stack.Pop();
                }
            }
        }


        private IEnumerable<KeyValuePair<T, INode>> GetChildrenFromNode(INode node, TrieOrder order)
        {
            var result = node.Children;
            if (order == TrieOrder.Asc)
            {
                result = result.OrderBy(c => c.Key, comparer);
            }
            else if (order == TrieOrder.Desc)
            {
                result = result.OrderByDescending(c => c.Key, comparer);
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

            foreach (var child in GetChildrenFromNode(root, order))
            {
                int childIndex = 0;
                if (!labels.TryGetValue(child.Value, out childIndex))
                {
                    childIndex = labels.Count + 1;
                    labels.Add(child.Value, childIndex);
                }
                text.AppendLine($"node1 -> node{childIndex} [label=\"&nbsp;{child.Key}\"]");
            }

            foreach (var node in Visit(order))
            {
                int index = 0;
                if (!labels.TryGetValue(node.Value, out index))
                {
                    index = labels.Count + 1;
                    labels.Add(node.Value, index);
                }

                if (node.Value.IsFinal)
                {
                    text.AppendLine($"node{index}[label=\"*\"]");
                }
                else
                {
                    text.AppendLine($"node{index}[label=\"\"]");
                }

                foreach (var child in GetChildrenFromNode(node.Value, order))
                {
                    int childIndex = 0;
                    if (!labels.TryGetValue(child.Value, out childIndex))
                    {
                        childIndex = labels.Count + 1;
                        labels.Add(child.Value, childIndex);
                    }
                    text.AppendLine($"node{index} -> node{childIndex} [label=\"&nbsp;{child.Key}\"]");
                }
            }

            text.AppendLine("}");
            return text.ToString();
        }


        private IEnumerable<KeyValuePair<T, INode>> Visit(TrieOrder order)
        {
            var stack = new Stack<KeyValuePair<T, INode>>();
            stack.Push(new KeyValuePair<T, INode>(default(T), root));
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                foreach (var child in GetChildrenFromNode(current.Value, order))
                {
                    stack.Push(child);
                }

                if (current.Value != root)
                {
                    yield return current;
                }
            }
        }

        #endregion

        #region Types

        private interface INode
        {
            IEnumerable<KeyValuePair<T, INode>> Children { get; }

            bool IsFinal { get; set; }

            bool Add(T part, out INode node);

            INode Find(T part);
        }


        private class Node : INode
        {
            private readonly List<KeyValuePair<T, INode>> children = new List<KeyValuePair<T, INode>>();


            public IEnumerable<KeyValuePair<T, INode>> Children
            {
                get
                {
                    return children;
                }
            }

            public bool IsFinal { get; set; }

            public bool Add(T part, out INode node)
            {
                node = Find(part);
                if (node != null)
                {
                    return false;
                }

                node = new Node();
                children.Add(new KeyValuePair<T, INode>(part, node));
                return true;
            }

            public INode Find(T part)
            {
                foreach (var child in children)
                {
                    if (Equals(child.Key, part))
                    {
                        return child.Value;
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