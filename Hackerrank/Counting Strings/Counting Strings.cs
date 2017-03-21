using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace CS
{
    class ParseRegex
    {
        public NFA Parse(string input)
        {
            //Shunting-Yard algorithm
            Stack<char> s = new Stack<char>();
            List<char> q = new List<char>();

            var c = input.GetEnumerator();
            char prev = '\0';
            while (c.MoveNext())
            {
                switch (c.Current)
                {
                    case 'a':
                    case 'b':
                        q.Add(c.Current);
                        if (prev == 'a' || prev == 'b' || prev == ')')
                        {
                            s.Push('-');
                        }
                        break;
                    case '(':
                        if (prev == 'a' || prev == 'b' || prev == ')')
                        {
                            s.Push('-');
                        }
                        s.Push(c.Current);
                        break;
                    case '*':
                    case '|':
                        s.Push(c.Current);
                        break;
                    case ')':
                        while (s.Count > 0 && s.Peek() != '(')
                        {
                            q.Add(s.Pop());
                        }
                        s.Pop();
                        break;
                }
                prev = c.Current;
            }

            Stack<NFA> calc = new Stack<NFA>();
            foreach (var i in q)
            {
                if (i == '*')
                {
                    calc.Push(NFA.Star(calc.Pop()));
                }
                else if (i == '-')
                {
                    var right = calc.Pop();
                    var left = calc.Pop();
                    calc.Push(NFA.Concat(left, right));
                }
                else if (i == '|')
                {
                    var right = calc.Pop();
                    var left = calc.Pop();
                    calc.Push(NFA.AnyOf(left, right));
                }
                else
                {
                    NFA nfa = new NFA();
                    nfa.AddState(0, false);
                    nfa.AddState(1, true);
                    nfa.AddTransition(0, 1, i);
                    calc.Push(nfa);
                }
            }

            return calc.Pop();
        }
    }

    class NFA
    {
        public static char Epsilon = 'ε';

        List<int> states = new List<int>();
        List<Tuple<int, int, char>> transitions = new List<Tuple<int, int, char>>();
        HashSet<int> final = new HashSet<int>();

        public void AddState(int state, bool isFinal)
        {
            states.Add(state);
            if (isFinal)
            {
                final.Add(state);
            }
        }

        public void AddTransition(int from, int to, char c)
        {
            transitions.Add(new Tuple<int, int, char>(from, to, c));
        }

        public static NFA AnyOf(NFA left, NFA right)
        {
            var result = new NFA();
            result.AddState(0, false);
            result.AddTransition(0, result.Add(left), Epsilon);
            result.AddTransition(0, result.Add(right), Epsilon);
            return result;
        }

        public static NFA Star(NFA input)
        {
            var result = new NFA();

            var newFinal = result.NewState();
            result.AddState(newFinal, true);

            var start = result.Add(input);
            var finals = result.final.Where(h=>h!=newFinal).ToArray();
            foreach (var final in finals)
            {
                result.AddTransition(final, start, Epsilon);
            }
            result.AddTransition(newFinal, start, Epsilon);

            return result;
        }

        public static NFA Concat(NFA left, NFA right)
        {
            var result = new NFA();
            result.Add(left);

            var leftFinals = result.final.ToArray();
            result.final.Clear();

            int rightInitial = result.Add(right);
            foreach (var final in leftFinals)
            {
                result.AddTransition(final, rightInitial, Epsilon);
            }
            return result;
        }

        private int Add(NFA left)
        {
            int stateOffset = NewState();
            foreach (var s in left.states)
            {
                AddState(s + stateOffset, left.final.Contains(s));
            }
            foreach (var t in left.transitions)
            {
                AddTransition(t.Item1 + stateOffset, t.Item2 + stateOffset, t.Item3);
            }
            return stateOffset;
        }

        private int NewState()
        {
            if (states.Count == 0)
            {
                return 0;
            }
            return states.Max() + 1;
        }

        private void EpsilonClosure(HashSet<int> set_state)
        {
            Stack<int> frontier = new Stack<int>(set_state);

            while (frontier.Count > 0)
            {
                var from_state = frontier.Pop();

                foreach (var ept in FindTransitions(from_state).Where(t => t.Item3 == Epsilon))
                {
                    if (set_state.Add(ept.Item2))
                    {
                        frontier.Push(ept.Item2);
                    }
                }
            }
        }

        private IEnumerable<Tuple<int, int, char>> FindTransitions(int from_state)
        {
            return transitions.Where(t => t.Item1 == from_state);
        }

        class SetStateComparer : IEqualityComparer<HashSet<int>>
        {
            public bool Equals(HashSet<int> x, HashSet<int> y)
            {
                if (x.Count != y.Count)
                {
                    return false;
                }

                return x.All(t => y.Contains(t));
            }

            public int GetHashCode(HashSet<int> x)
            {
                uint hash = int.MaxValue;
                foreach (var value in x.OrderBy(t => t))
                {
                    hash ^= (uint)value + 0x9e3779b9 + (hash << 6) + (hash >> 2);
                }
                return (int)hash;
            }
        }

        bool ContainsFinalState(HashSet<int> x)
        {
            return x.Intersect(final).Any();
        }

        public NFA Determinize()
        {
            NFA target = new NFA();

            Stack<Tuple<int, HashSet<int>>> frontier = new Stack<Tuple<int, HashSet<int>>>();
            Dictionary<HashSet<int>, int> seen = new Dictionary<HashSet<int>, int>(new SetStateComparer());
            int set_key = 0;

            HashSet<int> set_initial = new HashSet<int>();
            set_initial.Add(0);
            EpsilonClosure(set_initial);

            target.AddState(set_key, ContainsFinalState(set_initial));

            frontier.Push(new Tuple<int, HashSet<int>>(set_key, set_initial));
            seen[set_initial] = set_key;

            while (frontier.Count > 0)
            {
                var current = frontier.Pop();
                var current_key = current.Item1;
                var current_set = current.Item2;

                bool hasA = false, hasB = false;
                foreach (var st in current_set)
                {
                    foreach (var t in FindTransitions(st))
                    {
                        if (t.Item3 == Epsilon) continue;
                        else if (t.Item3 == 'a') hasA = true;
                        else if (t.Item3 == 'b') hasB = true;
                        if (hasA && hasB) break;
                    }
                    if (hasA && hasB) break;
                }

                var inputs = "ab";
                if (!(hasA & hasB))
                {
                    inputs = hasA ? "a" : "b";
                }

                foreach (var i in inputs)
                {
                    HashSet<int> new_state = new HashSet<int>();
                    HashSet<char> new_transitions = new HashSet<char>();

                    foreach (var st in current_set)
                    {
                        foreach (var t in FindTransitions(st).Where(j => j.Item3 == i))
                        {
                            new_transitions.Add(t.Item3);
                            new_state.Add(t.Item2);
                        }
                    }

                    EpsilonClosure(new_state);

                    int seen_state_key;
                    if (!seen.TryGetValue(new_state, out seen_state_key))
                    {
                        set_key++;

                        target.AddState(set_key, ContainsFinalState(new_state));

                        foreach (var t in new_transitions)
                        {
                            target.AddTransition(current_key, set_key, t);
                        }

                        frontier.Push(new Tuple<int, HashSet<int>>(set_key, new_state));
                        seen[new_state] = set_key;
                    }
                    else
                    {
                        foreach (var t in new_transitions)
                        {
                            target.AddTransition(current_key, seen_state_key, t);
                        }
                    }
                }
            }

            return target;
        }

        public uint Count(uint length)
        {
            Matrix m = new Matrix(states.Count, states.Count);
            foreach (var t in transitions)
            {
                m[t.Item1, t.Item2]++;
            }
            m = Matrix.Power(m, length);
            BigInteger r = 0;
            foreach (var f in final)
            {
                r += m[0, f];
            }
            return (uint)(r % 1000000007);

            int[] a = new int[states.Count];
            int[] b = new int[states.Count];
            for (int i = 0; i < states.Count; i++)
            {
                a[i] = -1;
                b[i] = -1;
            }
            foreach (var t in transitions)
            {
                if (t.Item3 == 'a') a[t.Item1] = t.Item2;
                if (t.Item3 == 'b') b[t.Item1] = t.Item2;
            }
            uint[,] table = new uint[states.Count, 2];
            for (int i = 0; i < states.Count; i++)
            {
                if (final.Contains(i))
                    table[i, 0] = 1;
            }
            for (int i = 1; i <= length; i++)
            {
                int t1 = i % 2;
                int t0 = 1 - t1;
                for (int j = 0; j < states.Count; j++)
                {
                    uint result = 0;
                    if (a[j] >= 0)
                    {
                        result = (result + table[a[j], t0]) % 1000000007;
                    }
                    if (b[j] >= 0)
                    {
                        result = (result + table[b[j], t0]) % 1000000007;
                    }
                    table[j, t1] = result;
                }
            }
            return table[0, length % 2];
        }
    }

    public class Matrix
    {
        public int rows;
        public int cols;
        public BigInteger[,] mat;

        public Matrix(int iRows, int iCols)
        {
            rows = iRows;
            cols = iCols;
            mat = new BigInteger[rows, cols];
        }

        public BigInteger this[int iRow, int iCol]
        {
            get { return mat[iRow, iCol]; }
            set { mat[iRow, iCol] = value; }
        }

        public Matrix Duplicate()
        {
            Matrix matrix = new Matrix(rows, cols);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = mat[i, j];
            return matrix;
        }

        public static Matrix ZeroMatrix(int iRows, int iCols)
        {
            Matrix matrix = new Matrix(iRows, iCols);
            for (int i = 0; i < iRows; i++)
                for (int j = 0; j < iCols; j++)
                    matrix[i, j] = 0;
            return matrix;
        }

        public static Matrix IdentityMatrix(int iRows, int iCols)
        {
            Matrix matrix = ZeroMatrix(iRows, iCols);
            for (int i = 0; i < Math.Min(iRows, iCols); i++)
                matrix[i, i] = 1;
            return matrix;
        }

        public static Matrix Power(Matrix m, uint pow)
        {
            if (pow == 0) return IdentityMatrix(m.rows, m.cols);
            if (pow == 1) return m.Duplicate();

            Matrix x = m.Duplicate();

            Matrix ret = IdentityMatrix(m.rows, m.cols);
            while (pow != 0)
            {
                if ((pow & 1) == 1) ret = Mutiply(ret, x);
                x = Mutiply(x, x);
                pow >>= 1;
            }
            return ret;
        }

        public static Matrix Mutiply(Matrix m1, Matrix m2)
        {
            if (m1.cols != m2.rows) throw new Exception("Wrong dimensions of matrix!");

            Matrix result = ZeroMatrix(m1.rows, m2.cols);
            for (int i = 0; i < result.rows; i++)
                for (int j = 0; j < result.cols; j++)
                {
                    for (int k = 0; k < m1.cols; k++)
                    {
                        result[i, j] += (m1[i, k] * m2[k, j]) % 1000000007;
                    }
                    result[i, j] %= 1000000007;
                }
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var parser = new ParseRegex();
            var T = int.Parse(Console.ReadLine());
            for (int i=0; i< T; ++i)
            {
                var r = Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine(parser.Parse(r[0]).Determinize().Count(uint.Parse(r[1])));
            }
        }
    }
}
