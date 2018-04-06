using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PMS.Common.Text.Exact
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

    public static class ZFunctionExtensions
    {
        public static IEnumerable<int> FindAll(this string text, string pattern)
        {
            var z = ZFunction.FromString($"{pattern}{'\0'}{text}", ZAlgorithm.Linear);
            for (int i=0; i<text.Length; ++i)
            {
                if (z[i + pattern.Length + 1] >= pattern.Length)
                    yield return i;
            }
        }
    }

    public enum ZAlgorithm
    {
        Naive,
        Linear,
        SuffixTree
    }

    public static class ZFunctionTest
    {
        static string RandomString(int n, string alphabet = "xy")
        {
            var r = new Random();
            var b = new char[n];
            var i = 0;
            while (i < n)
            {
                var c = alphabet[r.Next(alphabet.Length)];
                var m = 1 + r.Next(n - i);
                for (int j = 0; j < m; ++j)
                {
                    b[i] = c;
                    i++;
                }
            }
            return new string(b);
        }

        static void Main(string[] args)
        {
            var c = "xyxyxxxxzyyxyxy".FindAll("xy").ToArray();

            Console.WriteLine(ZFunction.FromString("aabxaabk", ZAlgorithm.Linear));
            Console.WriteLine(ZFunction.FromString("aaaaa", ZAlgorithm.Linear));
            Console.WriteLine(ZFunction.FromString("aaabaab", ZAlgorithm.Linear));
            Console.WriteLine(ZFunction.FromString("abacaba", ZAlgorithm.Linear));
        }
    }
}
