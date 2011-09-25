using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Protsyk.Puzzles {
    class Program {
        static int N;

        static void Main(string[] args) {
            string s = File.ReadAllText(args[0]);
            string[] splits = s.Split(new char[] { '\n', '\r' },
                              StringSplitOptions.RemoveEmptyEntries);

            N = int.Parse(splits[0]);
            for (int i = 1; i < N + 1; i++) {
                string[] words = splits[i]
                                         .Split(' ')
                                         .Skip(1)
                                         .ToArray();

                Console.WriteLine(Solve(words));
            }
        }

        private static string Solve(string[] words) {
            string prefix = string.Empty;
            List sorted = new List(words);

            while (true) {
                sorted = sorted.OrderBy(x => x).ToList();
                if (sorted.Count == 0) break;

                string pr = sorted.First();
                string sf = sorted.Skip(1)
                                .Where(a => a.StartsWith(pr))
                                .Select(s => s.Substring(pr.Length))
                                .Where(s => !string.IsNullOrEmpty(s))
                                .OrderBy(x => x + pr)
                                .FirstOrDefault();

                if (string.Compare(pr + pr, pr + sf) < 0)
                    sf = null;

                prefix += pr + sf;
                sorted.Remove(pr + sf);
            }

            return prefix;
        }
    }
}