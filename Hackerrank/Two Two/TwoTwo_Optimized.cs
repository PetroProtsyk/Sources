using System;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// https://www.hackerrank.com/challenges/two-two/problem
public class TwoTwo
{
    public static void Main(string[] args)
    {
        var n = new BigInteger(1);
        var t = new TrieSet();
        t.Add("1");

        for (int i=1; i<=800; i++)
        {
            n=n*2;
            t.Add(n.ToString());
        }

        int m = Convert.ToInt32(Console.ReadLine());
        for (int v = 0; v < m; ++v) {
            var a = Console.ReadLine().ToArray();

            var c = 0;
            for(int i=0; i<a.Length; ++i)
            {
                c += t.Match(a, i);
            }

            Console.WriteLine(c);
        }
    }
}

public class TrieSet
{
    #region Fields
    private readonly Trie<byte> trie;
    #endregion

    #region Constructor
    public TrieSet()
    {
        trie = new Trie<byte>();
    }
    #endregion

    #region Api
    public int Count => trie.Count;

    public bool Add(IEnumerable<char> key)
    {
        // Value does not matter
        return trie.Add(key, 0);
    }

    public int Match(char[] text, int offset)
    {
        return trie.Match(text, offset);
    }
    #endregion
}

internal class Trie<V>
{
    #region Fields
    private readonly INode root;
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
    {
        this.count = 0;
        this.root = new Node();
    }
    #endregion

    #region Methods

    public bool Add(IEnumerable<char> key, V value)
    {
        var node = root;
        foreach (var part in key)
        {
            var added = node.Add(part, out node);
        }

        if (!node.IsFinal)
        {
            node.IsFinal = true;
            node.Value = value;

            count++;
            return true;
        }

        return false;
    }

    public int Match(char[] text, int offset)
    {
       var result = 0;
       var node = root;
       while (node !=  null && offset < text.Length)
       {
        node = node.Find(text[offset]);
        ++offset;
        if (node != null && node.IsFinal)
        {
           ++result;
        }
       }
       return result;
    }
    #endregion

    #region Types

    private interface INode
    {
        bool IsFinal { get; set; }

        V Value { get; set; }

        bool Add(char label, out INode node);

        INode Find(char label);
    }

    private class Node : INode
    {
        private readonly INode[] children = new INode[10];

        public bool IsFinal { get; set; }

        public V Value { get; set; }

        public bool Add(char label, out INode node)
        {
            node = Find(label);
            if (node != null)
            {
                return false;
            }

            var i = (int)label - (int)'0';
            node = new Node();
            children[i] = node;
            return true;
        }

        public INode Find(char label)
        {
            var i = (int)label - (int)'0';
            return children[i];
        }
    }

    #endregion
}