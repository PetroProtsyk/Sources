using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protsyk.DataStructures;

namespace Protsyk.Algorithms
{
    public class HeapExtensions
    {
        public static IEnumerable<T> TopN<T>(IEnumerable<T> items, int n)
        {
            var heap = new Heap<T>();
            foreach (var item in items)
            {
                heap.Add(item);

                if (heap.Count > n)
                    heap.RemoveTop();
            }

            var result = new T[heap.Count];
            for (int i = 0; i < result.Length; ++i)
            {
                result[result.Length - 1 - i] = heap.RemoveTop();
            }

            return result;
        }

    }
}
