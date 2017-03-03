// https://www.facebook.com/hackercup/problem/313229895540583/
using System;
using System.Collections.Generic;

namespace fb2
{
 class Node
 {
  public char C;
  public List<Node> child;

  public Node(char c)
  {
    C = c;
    child = new List<Node>();
  }

  public int Add(string s)
  {
   Node n = this;
   int t = 0;
   bool v = true;

   for (int i=0; i<s.Length; ++i)
   {
    var k = true;
   foreach (var f in n.child)
   {
     if (f.C == s[i])
     {
      ++t;
      n = f;
      k = false;    
      break; 
     }
   }

   if (!k) continue;  

    Node n1 = new Node(s[i]);
    n.child.Add(n1);
    if (v) ++t;
    v = false;
    n = n1; 
   }
   return t;
  }
 }


 static class My
 {
  public static void Main()
  {
    int T = int.Parse(Console.ReadLine());
    for (int i=0; i<T; i++)
    {    
       int N = int.Parse(Console.ReadLine());
       int total = 0;
       Node tree = new Node('#');
       for (int j=0; j<N; ++j)
       {    
        string term = Console.ReadLine();
        total += tree.Add(term);
       }
       Console.WriteLine("Case #{0}: {1}", i+1, total);
    }
  }
 }

}