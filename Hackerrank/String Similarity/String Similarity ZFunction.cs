using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Protsyk.Hackerrank.StringSimilarity
{
    public class ZFunction
    {
        #region Fields
        private readonly int[] z;
        #endregion

        #region Properties
        public int Count => z.Length;

        public int this[int index] => z[index];
        #endregion

        #region Methods
        public override string ToString() => string.Join(" ", z.Select(i => i.ToString()));
        #endregion

        #region Construction
        private ZFunction(int[] z)
        {
            this.z = z ?? throw new ArgumentNullException(nameof(z));
        }

        public static ZFunction FromString(string text, ZAlgorithm algorithm)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            switch(algorithm)
            {
                case ZAlgorithm.Naive:
                    return FromStringNaive(text);
                case ZAlgorithm.Linear:
                    return FromStringLinear(text);
                default:
                    throw new NotImplementedException($"Algorithm {algorithm} is not implemented");
            }
        }

        private static ZFunction FromStringNaive(string text)
        {
            var z = new int[text.Length];
            z[0] = 0;

            for (int i=1; i<text.Length; ++i)
            {
                int m = 0;
                int k = i;

                while (k<text.Length && text[m] == text[k])
                {
                    m++;
                    k++;
                }

                z[i] = m;
            }

            return new ZFunction(z);
        }

        private static ZFunction FromStringLinear(string text)
        {
            var z = new int[text.Length];
            z[0] = 0;

            int l = 0;
            int r = 0;

            for (int i = 1; i < text.Length; ++i)
            {
                var zi = 0;
                if (i < r)
                {
                    zi = Math.Min(z[i - l], r - i);
                }

                int m = zi;
                int k = i + zi;
                while (k < text.Length && text[m] == text[k])
                {
                    ++m;
                    ++k;
                    ++zi;
                }

                if (k > r)
                {
                    l = i;
                    r = k;
                }

                z[i] = zi;
            }

            return new ZFunction(z);
        }
        #endregion
    }

    public enum ZAlgorithm
    {
        Naive,
        Linear,
        SuffixTree
    }

    public static class ZFunctionTest
    {
        static void Main(string[] args)
        {
            var t = int.Parse(Console.ReadLine());
            for (int i=0; i<t; ++i)
            {
                var txt = Console.ReadLine();
                var z2 = ZFunction.FromString(txt, ZAlgorithm.Linear);

                long s = 0;
                for (int j=0; j<txt.Length; ++j) s+=z2[j];
                Console.WriteLine(s + txt.Length);
            }
        }
    }
}
