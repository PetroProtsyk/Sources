//http://en.wikipedia.org/wiki/Combination

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protsyk.Combinatorics.Combinations
{
    class Program
    {

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

        static void Main(string[] args)
        {

            string[] input = { "1", "2", "3", "4", "5" };
            int n = input.Length;
            int m = 3;

            List<State> state = new List<State>();

            // Build state
            for (int i = 0; i <= m; i++)
            {
                state.Add(new State(m - i, 0));
            }

            while (state.Count > 0)
            {

                State currentState = state.Last();
                state.RemoveAt(state.Count - 1);

                if (currentState.m == 0)
                {
                    // We are done, print combination
                    int index = 0;
                    foreach (State s in state)
                    {
                        Console.Write(input[s.i + index]);
                        index++;
                    }
                    Console.WriteLine();

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
