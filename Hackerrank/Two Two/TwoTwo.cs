using System;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TwoTwo
{
    public static void Main(string[] args)
    {
        var n = new BigInteger(1);
        var t = new TernarySearchTree<char>();
        t.Add("1");

        for (int i=1; i<=800; i++)
        {
            n=n*2;
            t.Add(n.ToString());
        }

        int m = Convert.ToInt32(Console.ReadLine());
        for (int v = 0; v < m; ++v) {
            string a = Console.ReadLine();

            var c = 0;
            for(int i=0; i<a.Length; ++i)
            {
                var matcher = new SequenceMatcher(a, i);
                c += t.Match(matcher).Count();
            }

            Console.WriteLine(c);
        }
    }
}

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

public struct TrieMatch<T, V>
{
    public IEnumerable<T> Key { get; }

    public V Value { get; }

    public TrieMatch(IEnumerable<T> key, V value)
    {
        this.Key = key;
        this.Value = value;
    }
}

public class SequenceMatcher : ITrieMatcher<char>
{
    private readonly string items;
    private int index;

    public SequenceMatcher(string items, int first)
    {
        this.items = items;
        this.index = first -1;
    }


    public void Reset()
    {
        index = -1;
    }

    public bool IsFinal()
    {
       return true;
    }

    public bool Next(char p)
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