using System;
using System.Collections.Generic;
using System.IO;

// No Prefix Set
// https://www.hackerrank.com/challenges/no-prefix-set/problem
class Solution {
    class Node
    {
       string prefix;
       bool word = false;
       List<Node> children = new List<Node>();
             
       public Node(string prefix)
       {
           this.prefix = prefix;
       }
        
       public bool Add(string s)
       {
           for (int i=0; i<children.Count; ++i)
           {
             var c = children[i];
             int l = CommonPrefix(c.prefix, s);
             if ((l == c.prefix.Length && c.word) || (l == s.Length))
             {
                 return false;
             }
             if (l > 0)
             {
                 string w = c.prefix;
                 if (l == c.prefix.Length)
                 {
                   if (!c.Add(s.Substring(l))) return false;
                 }
                 else
                 {
                   children.RemoveAt(i);
                   Node nc = new Node(w.Substring(0, l)) { word = false };
	  	   children.Add(nc);

                   c.prefix = w.Substring(l);
                   nc.children.Add(c);
                   if (!nc.Add(s.Substring(l))) return false;
                 }
                 return true;
             }
           }
           
           children.Add(new Node(s) { word = true });
           return true;
       }
        
       public static int CommonPrefix(string s1, string s2)
       {
          int i = 0;
          while(i<s1.Length && i<s2.Length && s1[i] == s2[i]) ++i;
          return i;
       }
    }
    
    static void Main(String[] args) {
        int n = int.Parse(Console.ReadLine());
        Node root = new Node("");
        for (int i=0; i<n; ++i){
            string s = Console.ReadLine();
            if (!root.Add(s)){
                Console.WriteLine("BAD SET");
                Console.WriteLine(s);
                return;
            }
        }
        Console.WriteLine("GOOD SET");
    }
}