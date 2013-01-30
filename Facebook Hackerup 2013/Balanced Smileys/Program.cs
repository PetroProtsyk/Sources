using System;
using System.Text;
using System.Collections.Generic;

namespace Protsyk.Puzzles.BalancedSmileys
{
  class Program
  {
    static void Main(String[] args)
    {
      int n = int.Parse(Console.ReadLine());

      for (int i = 0; i < n; i++)
      {
        string s = Console.ReadLine();

        // Step 1. Remove all chars, replace smiles with special braces
        StringBuilder simplified = new StringBuilder();
        for (int j = 0; j < s.Length; j++)
        {
          if ((s[j] == ':') && (j < s.Length - 1) && (s[j + 1] == ')' || s[j + 1] == '('))
          {
            simplified.Append(s[j + 1] == ')' ? '}' : '{');
            j++;
          }
          else if (s[j] == ')' || s[j] == '(')
          {
            simplified.Append(s[j]);
          }
        }

        // Step 2. Remove all closed and balanced braces (to simplify debugging)
        s = Normalize(simplified.ToString());

        int balance = 0;
        int leftbalance = 0;
        int rightbalance = 0;

        for (int j = 0; j < s.Length; j++)
        {
          if (s[j] == '(')
          {
            rightbalance = Math.Min(balance, rightbalance);
            balance++;
          }
          else
            if (s[j] == '{') leftbalance++;
            else
              if (s[j] == '}') rightbalance++;
              else
                if (s[j] == ')')
                {
                  if (balance > 0)
                  {
                    balance--;
                    if (balance == 0)
                    {
                      rightbalance = 0;
                    }
                  }
                  else if (leftbalance > 0)
                  {
                    leftbalance--;
                    if (leftbalance == 0)
                    {
                      rightbalance = 0;
                    }
                  }
                  else
                  {
                    balance = int.MaxValue;
                    break;
                  }
                }
        }

        if (balance == 0 || balance <= rightbalance)
          Console.WriteLine("Case #{0}: YES", i + 1);
        else
          Console.WriteLine("Case #{0}: NO", i + 1);

      }
    }

    private static string Normalize(string p)
    {
      List<char> stack = new List<char>();
      StringBuilder result = new StringBuilder();
      for (int i = 0; i < p.Length; i++)
      {
        if (p[i] == '(') stack.Add('(');
        else if (p[i] == ')')
        {
          if (stack.Count > 0 && stack[stack.Count - 1] == '(')
          {
            stack.RemoveAt(stack.Count - 1);
          }
          else
          {
            result.Append(stack.ToArray());
            result.Append(p[i]);
            stack.Clear();
          }
        }
        else if (p[i] == '}' || p[i] == '{')
        {
          result.Append(stack.ToArray());
          result.Append(p[i]);
          stack.Clear();
        }
      }
      result.Append(stack.ToArray());
      return result.ToString();
    }
  }

    //static int FindBalanced(string s, int pos)
    //{
    //  if (pos >= s.Length)
    //    return int.MaxValue;

    //  for (int i = pos; i < s.Length; )
    //  {
    //    if (s[i] == '(')
    //    {
    //      i = FindBalanced(s, pos + 1);
    //      if (i == int.MaxValue) return i;
    //      i = i + 1;
    //    }
    //    else if (s[i] == ')')
    //    {
    //      return i;
    //    }
    //    else
    //    {
    //      i++;
    //    }
    //  }

    //  return int.MaxValue;
    //}
    //bool yes = true;
    //for (int j = 0; j < s.Length; )
    //{
    //  if (s[j] == '(')
    //  {
    //    j = FindBalanced(s, j + 1);
    //    if (j == int.MaxValue)
    //    {
    //      yes = false;
    //      break;
    //    }
    //    else
    //    {
    //      j = j + 1;
    //    }
    //  }
    //  else if (s[j] == ')')
    //  {
    //  }
    //}

}