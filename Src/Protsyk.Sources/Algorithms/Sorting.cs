using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Protsyk.Algorithms.Sorting
{

    class Implementation
    {

        #region Common
        static void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        static bool Compare(int[] a, int[] b)
        {
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i])
                    return false;

            return true;
        }
        #endregion

        #region Insertion Sort
        static void InsertionSort(int[] input, int left, int right)
        {
            for (int i = left + 1; i < right; i++)
            {
                int e = input[i];
                int j = i;

                while (j > left && input[j - 1] > e)
                {
                    input[j] = input[j - 1];
                    --j;
                }

                input[j] = e;
            }
        }
        #endregion

        #region Bubble Sort
        static void BubbleSort(int[] input, int left, int right)
        {
            if (left == right)
                return;

            for (int i = left; i < right - 1; i++)
            {
                for (int j = i + 1; j < right; j++)
                {
                    if (input[i] > input[j])
                    {
                        Swap(ref input[i], ref input[j]);
                    }
                }
            }
        }
        #endregion

        #region Selection Sort
        static void SelectionSort(int[] input, int left, int right)
        {
            if (left == right)
                return;

            for (int i = left; i < right; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < right; j++)
                {
                    if (input[minIndex] > input[j])
                    {
                        minIndex = j;
                    }
                }
                Swap(ref input[i], ref input[minIndex]);
            }
        }
        #endregion

        #region Heap Sort
        static void PushDown(int[] input, int i, int left, int right)
        {
            int child1 = (i << 1) + 1 + left;
            int child2 = child1 + 1;
            int max = i + left;

            if (child1 < right && input[max] < input[child1])
            {
                max = child1;
            }

            if (child2 < right && input[max] < input[child2])
            {
                max = child2;
            }

            if (max == i + left)
            {
                return;
            }

            Swap(ref input[max], ref input[i + left]);

            PushDown(input, max - left, left, right);
        }

        static void MakeHeap(int[] input, int left, int right)
        {
            int length = right - left;
            for (int i = right - length / 2; i >= left; i--)
            {
                PushDown(input, i - left, left, right);
            }
        }

        static void HeapSort(int[] input, int left, int right)
        {
            int length = right - left;
            MakeHeap(input, left, right);
            for (int i = 1; i < length; i++)
            {
                Swap(ref input[left], ref input[right - i]);
                PushDown(input, 0, left, right - i);
            }
        }
        #endregion

        #region Quick Sort
        static void QuickSort(int[] input, int left, int right)
        {
            if (right - left <= 32)
            {
                InsertionSort(input, left, right);
            }
            else if (right > left + 1)
            {
                int leftI = left;
                int rightI = right - 1;
                int pivot = input[right - 1];

                while (leftI != rightI)
                {
                    while (input[leftI] < pivot && leftI != rightI) leftI++;
                    while (input[rightI] >= pivot && leftI != rightI) rightI--;

                    Swap(ref input[leftI], ref input[rightI]);
                }

                input[right - 1] = input[rightI];
                input[rightI] = pivot;

                QuickSort(input, left, rightI);
                QuickSort(input, rightI + 1, right);
            }
        }
        #endregion

        #region Merge Sort
        static void MergeSort(int[] input, int left, int right)
        {
            if (right - left <= 32)
            {
                InsertionSort(input, left, right);
            }
            else
            if (right > left + 1)
            {
                int middle = (right + left) >> 1;

                if (middle > left)
                    MergeSort(input, left, middle);

                if (right > middle)
                    MergeSort(input, middle, right);

                Merge(input, left, middle, right);
            }
        }

        static void Merge(int[] input, int left, int middle, int right)
        {
            int[] merged = new int[right - left];

            int i = left;
            int j = middle;
            int c = 0;

            while (i < middle && j < right)
            {
                if (input[i] > input[j])
                {
                    merged[c] = input[j];
                    ++j;
                }
                else
                {
                    merged[c] = input[i];
                    ++i;
                }
                ++c;
            }

            for (int k = i; k < middle; k++, c++) merged[c] = input[k];
            for (int k = j; k < right; k++, c++) merged[c] = input[k];

            Array.Copy(merged, 0, input, left, right - left);
        }
        #endregion

        #region Shell Sort
        static void ShellSort(int[] input, int left, int right)
        {
            int n = right - left;
            int h = 1;
            while (h < n / 3)
            {
                h = 3 * h + 1;
            }
            while (h >= 1)
            {
                for (int i = left + h; i < right; i++)
                {
                    for (int j= i; (j>= h) && (input[j] < input[j - h]); j-= h)
                    {
                        Swap(ref input[j], ref input[j - h]);
                    }
                }
                h = h / 3;
            }
        }
        #endregion

        public static void Test()
        {
            //int[] a = { 4, 1, 3, 2, 16, 9, 10, 14, 8, 7 };
            //Console.WriteLine(string.Join(",", a));

            int n = 50000;
            int N = n * 100;

            Stopwatch sw;
            List<int> test = new List<int>();
            Random r = new Random();
            for (int i = 0; i < N; i++)
            {
                test.Add(r.Next());
            }

            Console.WriteLine($"O(log(N)) sorting of {N} elements:");

            int[] etalon = test.ToArray();
            {
                sw = Stopwatch.StartNew();
                Array.Sort(etalon, 0, etalon.Length);
                Console.WriteLine("\tLibrary sort:" + sw.ElapsedMilliseconds);
            }

            {
                int[] a = test.ToArray();
                sw = Stopwatch.StartNew();
                HeapSort(a, 0, a.Length);
                Console.WriteLine("\tHeap sort:" + sw.ElapsedMilliseconds);
                if (!Compare(a, etalon))
                {
                    throw new Exception("Sorting failed");
                }
            }

            {
                int[] a = test.ToArray();
                sw = Stopwatch.StartNew();
                QuickSort(a, 0, a.Length);
                Console.WriteLine("\tQuick sort:" + sw.ElapsedMilliseconds);
                if (!Compare(a, etalon))
                {
                    throw new Exception("Sorting failed");
                }
            }

            {
                int[] a = test.ToArray();
                sw = Stopwatch.StartNew();
                MergeSort(a, 0, a.Length);
                Console.WriteLine("\tMerge sort:" + sw.ElapsedMilliseconds);
                if (!Compare(a, etalon))
                {
                    throw new Exception("Sorting failed");
                }
            }

            Console.WriteLine($"Sub quadratic sorting of {N} elements:");

            {
                int[] a = test.ToArray();
                sw = Stopwatch.StartNew();
                ShellSort(a, 0, a.Length);
                Console.WriteLine("\tShell sort:" + sw.ElapsedMilliseconds);
                if (!Compare(a, etalon))
                {
                    throw new Exception("Sorting failed");
                }
            }

            Console.WriteLine($"O(N^2) sorting of {n} elements:");

            test.Clear();
            for (int i = 0; i < n; i++)
            {
                test.Add(r.Next());
            }

            etalon = test.ToArray();
            {
                sw = Stopwatch.StartNew();
                Array.Sort(etalon, 0, etalon.Length);
                Console.WriteLine("\tLibrary sort:" + sw.ElapsedMilliseconds);
            }

            {
                int[] a = test.ToArray();
                sw = Stopwatch.StartNew();
                InsertionSort(a, 0, a.Length);
                Console.WriteLine("\tInsertion sort:" + sw.ElapsedMilliseconds);
                if (!Compare(a, etalon))
                {
                    throw new Exception("Sorting failed");
                }
            }

            {
                int[] a = test.ToArray();
                sw = Stopwatch.StartNew();
                SelectionSort(a, 0, a.Length);
                Console.WriteLine("\tSelection sort:" + sw.ElapsedMilliseconds);
                if (!Compare(a, etalon))
                {
                    throw new Exception("Sorting failed");
                }
            }

            {
                int[] a = test.ToArray();
                sw = Stopwatch.StartNew();
                BubbleSort(a, 0, a.Length);
                Console.WriteLine("\tBubble sort:" + sw.ElapsedMilliseconds);
                if (!Compare(a, etalon))
                {
                    throw new Exception("Sorting failed");
                }
            }

        }
    }
}
