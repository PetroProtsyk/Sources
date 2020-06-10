//http://en.wikipedia.org/wiki/Combination

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protsyk.Combinatorics.Combinations
{
    public class CombinationsCalculator
    {
        public static ulong Combinations(ulong n, ulong k)
        {
            if (n == 0) return 0;
            if (k > n)  return 0;
            if (k == 0) return 1;
            if (k == n) return 1;
            return Combinations(n-1, k-1) + Combinations(n-1, k);
        }

        public static ulong CombinationsFormula(ulong n, ulong k)
        {
            if (n == 0) return 0;
            if (k > n)  return 0;
            if (k == 0) return 1;
            if (k == n) return 1;

            // n! / ((n-k)! * k!)

            ulong result = 1;
            for (ulong i=n-k+1; i<=n; ++i)
            {
              result = (result*i); 
            }

            ulong kf = 1;
            for (ulong i=2; i<=k; ++i)
            {
              kf = (kf*i); 
            }

            result = (result / kf);
            return result;
        }

        public static ulong CombinationsKnuth(ulong n, ulong k)
        {
            if (k > n)
            {
                return 0UL;
            }
            ulong r = 1;
            for (ulong d = 1; d <= k; ++d)
            {
                r *= n--;
                r /= d;
            }
            return r;
        }


        static Lazy<ulong[,]> combinationsCache = new Lazy<ulong[,]>(()=>InitializeCombinations(64));

        static ulong[,] InitializeCombinations(int size)
        {
          var r = new ulong[size + 1,size + 1];

          for (int i=0; i<size+1; ++i)
          {
            r[i,0] = 1;
            r[i,i] = 1;
          }

          for (int i=1; i<size+1; ++i)
          {
           for (int j=1; j<size+1; ++j)
           {
             checked
             {
               r[i,j] = r[i-1,j-1] + r[i-1,j];
             }
           }
          }
          return r;
        }

        public static ulong CombinationsCached(ulong n, ulong k)
        {
          if (k > n)  return 0;
          return combinationsCache.Value[n,k];
        }


        class State
        {
            public State(int m, int i) { this.m = m; this.i = i; }

            public int m;
            public int i;

            public override string ToString()
            {
                return "[" + m + "," + i + "]";
            }
        }

        public static IEnumerable<T[]> ProduceCombinations<T>(int k, T[] input)
        {
            int n = input.Length;
            List<State> state = new List<State>();

            // Build state
            for (int i = 0; i <= k; i++)
            {
                state.Add(new State(k - i, 0));
            }

            while (state.Count > 0)
            {

                State currentState = state.Last();
                state.RemoveAt(state.Count - 1);

                if (currentState.m == 0)
                {
                    // We are done, produce combination
                    int index = 0;
                    T[] result = new T[k];
                    foreach (State s in state)
                    {
                        result[index] = input[s.i + index];
                        index++;
                    }
                    yield return result;

                    continue;
                }

                if (currentState.i + 1 < n - state.Count)
                {
                    state.Add(new State(currentState.m, currentState.i + 1));
                    state.Add(new State(currentState.m - 1, currentState.i));
                }
            }
        }
    }
}
