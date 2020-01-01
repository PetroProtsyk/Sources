using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protsyk.Algorithms.FuzzySearch
{
    /// <summary>
    /// Check if two strings are within specified Levenshtein distance from each other
    /// using shift-and algorithm also known as Bitap or Baeza-Yates–Gonnet algorithm
    /// https://en.wikipedia.org/wiki/Bitap_algorithm
    /// </summary>
    public static class LevenshteinBitap
    {
        public static bool FuzzyMatch(string pattern, string text, int d)
        {
            int m = pattern.Length;
            if (m > sizeof(UInt64) * 8 - d)
            {
                throw new ArgumentException("Pattern is too long");
            }

            if (m == 0)
            {
                return false;
            }

            var alphabet = pattern.Select(y => y).Distinct().OrderBy(y => y).ToArray();
            var first_a = alphabet.First();
            var last_a = alphabet.Last();

            var T = new UInt64[last_a - first_a + 1];
            for (int i = 0; i < T.Length; ++i)
            {
                T[i] = ~(0ul);
            }

            /* Initialize characteristic vectors T */
            for (int i = 0; i < m; ++i)
            {
                int c = (int)(pattern[i] - first_a);
                T[c] &= ~(1ul << (i + d));
            }

            /* Initialize the bit arrays R. */
            UInt64[] R = new UInt64[d + 1];

            UInt64[] prevR = new UInt64[d + 1];
            for (int i = 0; i < d + 1; ++i)
            {
                prevR[i] = (~0ul) << (i + d);
            }

            UInt64 mask = 1ul << (m + d - 1);

            for (int i = 0; i < text.Length; ++i)
            {
                char c = text[i];
                UInt64 Tc = (c >= first_a && c <= last_a) ? T[(int)(c - first_a)] : (~0ul);

                R[0] = (prevR[0] << 1) | Tc;
                for (int j = 1; j < d + 1; ++j)
                {
                    R[j] = (prevR[j - 1]) &
                           (R[j - 1] << 1) &
                           (prevR[j - 1] << 1) &
                           ((prevR[j] << 1) | Tc);
                }

                var t = R; R = prevR; prevR = t;
            }

            if ((prevR[d] & mask) == 0)
            {
                return true;
            }

            return false;
        }
    }

    public static class LevenshteinBitapTest
    {
        public static void Test(string[] args)
        {
            var words = Console.ReadLine().Split(' ');

            Console.WriteLine($"Matching words {words[0]} and {words[1]} using bitap algorithm with distance {words[2]}:");
            Console.WriteLine(LevenshteinBitap.FuzzyMatch(words[0], words[1], int.Parse(words[2])));
        }
    }
}
