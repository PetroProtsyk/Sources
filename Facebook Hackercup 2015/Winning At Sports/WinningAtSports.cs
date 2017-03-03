//https://www.facebook.com/hackercup/problem/688426044611322/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protsyk.Puzzles.WinningAtSports {

    class Program {

        static int max = 2001;
        static ulong[,] d;
        static ulong dv = 1000000007UL;

        static void Main(string[] args) {
            BuildCache();

            int T = int.Parse(Console.ReadLine());
            for (int testcase = 0; testcase < T; testcase++) {
                var g = Console
                            .ReadLine()
                            .Split(new char[] {'-'}, StringSplitOptions.RemoveEmptyEntries)
                            .Select(b=>int.Parse(b))
                            .ToArray();

                Console.WriteLine(string.Format("Case #{0}: {1} {2}", testcase + 1, 
                     d[g[0]- 1, g[1]], // stress-free
                     d[g[1], g[1]]   // stressful
                ));
            }
        }

        static void BuildCache() {
          checked{
           d = new ulong[max, max];
           for (int i = 0; i < max; i++) {
                      for (int j = i; j < max; j++) {
                          if (i == 0) {
                              d[i, j] = 1;
                              d[j, i] = 1;
                          } else if (i == j) {
                              d[i, i] = d[i - 1, i];
                          }
                          else {
                              d[i, j] = (d[i - 1, j] + d[i, j - 1]) % dv;
                              d[j, i] = d[i, j];
                          }
                      }
                  }}
        }

    }
}
