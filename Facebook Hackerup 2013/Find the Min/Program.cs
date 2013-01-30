using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protsyk.Puzzles.FindTheMin
{
  class Program
  {
    class Counter
    {
      public int Value;
      public Counter() { Value = 1; }
      public void Inc()
      {
        Value++;
      }

      public void Dec()
      {
        Value--;
      }
    }

    static void Main(string[] args)
    {
      int nc = int.Parse(Console.ReadLine());

      for (int i = 0; i < nc; i++)
      {
        string [] kn = Console.ReadLine().Split(' ');
        string [] abcr = Console.ReadLine().Split(' ');

        int n = int.Parse(kn[0]);
        int k = int.Parse(kn[1]);

        int a = int.Parse(abcr[0]);
        int b = int.Parse(abcr[1]);
        int c = int.Parse(abcr[2]);
        int r = int.Parse(abcr[3]);

        List<int> v = new List<int>();
        SortedDictionary<int, Counter> sd = new SortedDictionary<int, Counter>();

        long m = a;
        v.Add(a);
        sd.Add(a, new Counter());

        for (int j = 1; j < k; j++)
        {
          m = (b * m + c) % r;

          v.Add((int)m);
          Counter cv;
          if (sd.TryGetValue((int)m, out cv))
          {
            cv.Inc();
          }
          else
          {
            sd.Add((int)m, new Counter());
          }
        }

        CleanUp(ref sd, k + 1);

        HashSet<int> active = new HashSet<int>(sd.Keys);

        int l = 0;
        int p = 0;
        int next_p = int.MaxValue;
        while (true)
        {
          if (active.Contains(p))
            p = FindNext(sd);
          next_p = int.MaxValue;

          if (p == k && (sd.First().Key == 0) && (sd.Last().Key == k-1))
          {
            // We have all values in [0..k-1], there is no need to search for other values
            int f;
            long ma = ((long)(n-k)) % ((long)k * (long)(k+1));
            for (int o = 0; o < ma; o++, l++)
            {
              l = l % v.Count;
              f = v[l];
              v[l] = p;
              p = f;
            }

            if (l > 0)
              p = v[l - 1];
            else
              p = v[v.Count - 1];

            break;
          }

          if (l < v.Count)
          {
            l++;
          }
          else
          {
            l = 1;
          }

          Counter cv;
          if (sd.TryGetValue(v[l - 1], out cv))
          {
            cv.Dec();
            if (cv.Value == 0)
            {
              if (p > v[l - 1])
              {
                next_p = v[l - 1];
              }

              active.Remove(v[l - 1]);
              sd.Remove(v[l - 1]);
            }
          }

          //if (sd.TryGetValue((int)p, out cv))
          //{
          //  cv.Inc();
          //}
          //else
          {
            sd.Add((int)p, new Counter());
            active.Add(p);
          }

          v[l - 1] = p;

          n--;

          if (n == k)
          {
            break;
          }

          if (next_p == int.MaxValue)
          {
            p++;
          }
          else
          {
            p = next_p;
          }
        }


        /// Naive enumeration
        /*
        int p = 0;
        int l = 0;
        while (true)
        {
          if (!v.Contains(p))
          {

            if (l < v.Count)
            {
              v[l] = p;
              l++;
            }
            else
            {
              v[0] = p;
              l = 1;
            }

            n--;

            if (n == k)
            {
              break;
            }

            p = 0;
          }
          else
          {
            p++;
          }
        }*/

        Console.WriteLine("Case #{0}: {1}", i + 1, p);
      }
    }

    private static void CleanUp(ref SortedDictionary<int, Counter> sd, int k)
    {
      if (sd.Count == 0)
        return;

      int prev = 0;
      int count = 0;

      SortedDictionary<int, Counter> clean = new SortedDictionary<int, Counter>();

      foreach (var sv in sd)
      {
        count += sv.Key - prev - 1;
        prev = sv.Key;

        if (count > k) break;

        clean.Add(sv.Key, sv.Value);
      }

      sd = clean;
    }

    private static int FindNext(SortedDictionary<int, Counter> sd)
    {
      if (sd.Count == 0)
        return 0;

      int prev = 0;
      int current = 0;
      foreach (int sv in sd.Keys)
      {
        current = sv;
        if (prev < current)
        {
          return prev;
        }
        else
        {
          prev = current + 1;
        }
      }
      return current + 1;
    }

    private static bool Contains(List<long> v, int p, int k)
    {
      for (int i = 0; i < k; i++)
      {
        if (v[v.Count - k + i] == p)
          return true;
      }
      return false;
    }
  }
}
