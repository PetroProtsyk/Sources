using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protsyk.Combinatorics.Permutations {
    class Program {

        static bool NextPermutation(int[] numbers) {
            return Permutation(numbers, (a, b) => a < b);
        }

        static bool PrevPermutation(int[] numbers) {
            return Permutation(numbers, (a, b) => a >= b);
        }

        // Algorithm:

        // 1. Find the largest index k such that a[k] < a[k + 1]. If no such index exists, the permutation is the last permutation.
        // 2. Find the largest index l such that a[k] < a[l]. Since k + 1 is such an index, l is well defined and satisfies k < l.
        // 3. Swap a[k] with a[l].
        // 4. Reverse the sequence from a[k + 1] up to and including the final element a[n].

        // Alternative: http://en.wikipedia.org/wiki/Steinhaus–Johnson–Trotter_algorithm

        static bool Permutation(int[] numbers, Func<int, int, bool> comparison) {
            if (numbers.Length == 1)
                return false;

            for (int k = numbers.Length - 2; k >= 0; k--) {
                if (comparison(numbers[k], numbers[k + 1])) {
                    for (int l = numbers.Length - 1; l > k; l--) {
                        if (comparison(numbers[k], numbers[l])) {

                            var temp = numbers[k];
                            numbers[k] = numbers[l];
                            numbers[l] = temp;

                            Array.Reverse(numbers, k + 1, numbers.Length - k - 1);

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        static void Test(string[] args) {
            int[] numbers = { 1, 2, 3, 4 };

            Console.WriteLine("Permutations in increasing order:");

            do {
                Console.WriteLine(string.Join(",", numbers.Select(s=>s.ToString()).ToArray()));
            } while (NextPermutation(numbers));

            Console.WriteLine("Permutations in decreasing order:");

            do {
                Console.WriteLine(string.Join(",", numbers.Select(s=>s.ToString()).ToArray()));
            } while (PrevPermutation(numbers));

        }
    }
}
