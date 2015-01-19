using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Protsyk.Algorithms.Primes {
    class Program {

        static List<int> PrimesBit(int max) {
            List<int> result = new List<int>();
            BitArray p = new BitArray(max + 1, true);
            result.Add(2);
            for (int i = 3; i <= max; i += 2) {
                if (p[i]) {
                    for (int j = 2 * i; j <= max; j += i) {
                        p[j] = false;
                    }
                    result.Add(i);
                }
            }
            return result;
        }


        static List<int> Primes(int max) {
            HashSet<int> p = new HashSet<int>(Enumerable.Range(2, max - 1));
            for (int i = 2; i <= max; i++) {
                if (p.Contains(i)) {
                    for (int j = 2 * i; j <= max; j += i) {
                        p.Remove(j);
                    }
                }
            }
            return p.OrderBy(i => i).ToList();
        }

        static void Main(string[] args) {
            Stopwatch sw = Stopwatch.StartNew();
            var r = PrimesBit(int.Parse(args[0]));
            Console.WriteLine(r.Count + " " + sw.Elapsed);
        }
    }
}
