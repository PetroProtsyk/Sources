using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEA2016.FuzzySearch
{
    /// <summary>
    /// Calculate Levenshtein distance between two strings using brute-force recursive algorithm
    /// https://e...content-available-to-author-only...a.org/wiki/Levenshtein_distance#Recursive
    /// </summary>
    public static class LevenshteinBruteForce
    {
        public static int Calculate(string a, string b)
        {
            return CalculateRecursive(a, b, a.Length, b.Length);
        }

        private static int CalculateRecursive(string a, string b, int m, int n)
        {
            if (Math.Min(m, n) == 0)
            {
                return Math.Max(m, n);
            }

            var subCost = ((a[m - 1] == b[n - 1]) ? 0 : 1) + CalculateRecursive(a, b, m - 1, n - 1);
            var delCost = 1 + CalculateRecursive(a, b, m, n - 1);
            var insCost = 1 + CalculateRecursive(a, b, m - 1, n);

            return Math.Min(subCost, Math.Min(insCost, delCost));
        }

        private static int Calculate2(string a, string b, int m, int n)
        {
            if (b.Length == n)
            {
                return (a.Length - m);
            }

            if (a.Length == m)
            {
                return (b.Length - n);
            }

            if (a[m] == b[n])
            {
                return Calculate2(a, b, m + 1, n + 1);
            }
            else
            {
                var subCost = Calculate2(a, b, m + 1, n + 1);
                var delCost = Calculate2(a, b, m + 1, n);
                var insCost = Calculate2(a, b, m, n + 1);

                return 1 + Math.Min(subCost, Math.Min(insCost, delCost));
            }
        }
    }
    

    public static class Program
    {
    	public static void Main(string[] args)
        {
           var words = Console.ReadLine().Split(' ');

           Console.WriteLine($"Distance between words {words[0]} and {words[1]} using brute-force algorithm:");
           Console.WriteLine(LevenshteinBruteForce.Calculate(words[0], words[1]));
        }
    }
}
