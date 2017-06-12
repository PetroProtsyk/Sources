using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LargestRectangle
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] bars = { 1, 5, 5, 2, 3, 2, 2, 1, 5, 5, 1, 2, 1 };

            Console.WriteLine(Solve(bars));
        }

        private static int Solve(int[] bars)
        {
            int best = 0;

            Stack<KeyValuePair<int, int>> s = new Stack<KeyValuePair<int, int>>();
            for (int i = 0; i < bars.Length; ++i)
            {
                int range_start = i + 1;
                while (s.Count > 0)
                {
                    var v = s.Peek();
                    if (v.Key <= bars[i])
                        break;

                    best = Math.Max(best, v.Key * (i + 1 - v.Value));

                    if (v.Value < range_start)
                        range_start = v.Value;

                    s.Pop();
                }

                if (s.Count > 0 && s.Peek().Key == bars[i])
                    continue;
                else
                    s.Push(new KeyValuePair<int, int>(bars[i], range_start));
            }

            while (s.Count > 0)
            {
                var v = s.Pop();
                best = Math.Max(best, v.Key * (bars.Length + 1 - v.Value));
            }

            return best;
        }
    }
}
