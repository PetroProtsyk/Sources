using System;
using System.Collections.Generic;
using System.Linq;

namespace Numbers
{
    class Program
    {
        static void Test(string[] args)
        {
            int[] numbers = Console.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            int target = int.Parse(Console.ReadLine());
            foreach(var expression in GetAllExpressions(numbers, target))
            {
                Console.WriteLine($"{expression} = {target}");
            }
        }

        private static IEnumerable<string> GetAllExpressions(int[] values, int target)
        {
            for (int i = 0; i < values.Length; ++i)
            {
                foreach (var expression in GetAllExpressionsOfSize(i, values, new bool[values.Length]))
                {
                    if (expression.Item2 == target)
                    {
                        yield return expression.Item1;
                    }
                }
            }
        }

        private static IEnumerable<Tuple<string, int>> GetAllExpressionsOfSize(int size, int[] values, bool[] used)
        {
            if (size == 0)
            {
                for (int j = 0; j < used.Length; j++)
                {
                    if (used[j])
                    {
                        continue;
                    }

                    used[j] = true;
                    yield return Tuple.Create(values[j].ToString(), values[j]);
                    used[j] = false;
                }
            }

            for (int i = 0; i < size; i++)
            {
                foreach (var nodeLeft in GetAllExpressionsOfSize(i, values, used))
                {
                    foreach (var nodeRight in GetAllExpressionsOfSize(size - 1 - i, values, used))
                    {
                        if (nodeLeft.Item2 <= nodeRight.Item2)
                        {
                            yield return Tuple.Create($"({nodeLeft.Item1}+{nodeRight.Item1})", nodeLeft.Item2 + nodeRight.Item2);

                            if (nodeLeft.Item2 != 1)
                            {
                                yield return Tuple.Create($"({nodeLeft.Item1}*{nodeRight.Item1})", nodeLeft.Item2 * nodeRight.Item2);
                            }
                        }

                        if (nodeLeft.Item2 - nodeRight.Item2 > 0)
                        {
                            yield return Tuple.Create($"({nodeLeft.Item1}-{nodeRight.Item1})", nodeLeft.Item2 - nodeRight.Item2);
                        }

                        if (nodeRight.Item2 != 1 && nodeLeft.Item2 % nodeRight.Item2 == 0)
                        {
                            yield return Tuple.Create($"({nodeLeft.Item1}/{nodeRight.Item1})", nodeLeft.Item2 / nodeRight.Item2);
                        }
                    }
                }
            }
        }
    }
}
