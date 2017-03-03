// https://www.facebook.com/hackercup/problem/582062045257424/
// This solution was not accepted
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cooking_the_Books {
    class Program {
        static void Main(string[] args) {
            int N = int.Parse(Console.ReadLine());
            for (int i = 0; i < N; ++i) {
                var number = Console
                                .ReadLine()
                                .Select(s => (int)(s - '0'))
                                .ToArray();

                int maxPos = 0;
                int minPos = 0;
                for (int j = 0; j < number.Length; ++j) {
                    if (number[j] == 0) {
                        continue;
                    }
                    if (number[maxPos] < number[j]) {
                        maxPos = j;
                    }
                    if (number[minPos] > number[j]) {
                        minPos = j;
                    }
                }

                Swap(number, 0, minPos);
                string min = string.Join("", number.Select(s=>s.ToString()));
                Swap(number, 0, minPos);
                Swap(number, 0, maxPos);
                string max = string.Join("", number.Select(s=>s.ToString()));

                Console.WriteLine("Case #{0}: {1} {2}", i + 1, min, max);
            }
        }

        static void Swap<T>(T[] a, int i, int j) {
            if (i == j) return;
            var t = a[i]; a[i] = a[j]; a[j] = t;
        }
    }
}
