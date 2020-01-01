using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Protsyk.Algorithms.Sampling
{
    class Program
    {
        public static void Test(string[] args)
        {
            foreach (var item in Sample(10, Enumerable.Range(0, 100)))
            {
                Console.WriteLine(item);
            }
        }

        public static T[] Sample<T>(int k, IEnumerable<T> items)
        {
            var reservoir = new T[k];
            var rand = new Random();

            var i = 0;
            foreach (var item in items)
            {
                if (i < k)
                {
                    reservoir[i] = item;
                }
                else
                {
                    var j = rand.Next(i);
                    if (j < k)
                    {
                        reservoir[j] = item;
                    }
                }
                i++;
            }

            return reservoir;
        }
    }
}