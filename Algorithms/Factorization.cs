using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protsyk.Algorithms {

    public class Factorization {

        static IEnumerable<Tuple<int, int>> Factorize(int s) {
            int s1 = 1;
            int threshold = (int)Math.Sqrt(s) + 1;

            while (s1 <= threshold) {
                int remainder = s % s1;

                if (remainder == 0) {
                    int s2 = s / s1;
                    if (s2 < s1) yield break;
                    yield return new Tuple<int, int>(s1, s2);
                }
                ++s1;
            }
        }

        static long GCD(long a, long b) {
            long t;
            while (b != 0) {
                t = b; 
                b = a % b;
                a = t; 
            }
            return a;
        }

    }
}
