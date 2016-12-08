using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEA2016.FuzzySearch
{
    /// <summary>
    /// Calculate Levenshtein distance between two strings using Levenshtein automaton
    /// https://e...content-available-to-author-only...a.org/wiki/Levenshtein_automaton
    /// </summary>
    public static class LevenshteinAutomaton
    {
        public static bool Match(string pattern, string text, int d)
        {
            var dfa = CreateAutomaton(pattern, d).Determinize();
            var s = 0;
            for (int i=0; i<text.Length; ++i)
            {
                s = dfa.Next(s, text[i]);
            }
            return dfa.IsFinal(s);
        }

        public static NFA CreateAutomaton(string a, int k)
        {
            if (a.Contains('*'))
            {
                throw new ArgumentException("Star is a reserved character");
            }

            var result = new NFA();
            var m = a.Length + 1;

            /* Create |a|*k states */
            for (int i = 0; i < m; ++i)
            {
                for (int j = 0; j <= k; ++j)
                {
                    result.AddState(i + m * j, i == a.Length);
                }
            }

            /* Create transitions */
            for (int i = 0; i < m; ++i)
            {
                for (int j = 0; j <= k; ++j)
                {
                    if (i < m - 1)
                    {
                        result.AddTransition(i + m * j, i + 1 + m * j, a[i]);
                    }

                    if (j < k)
                    {
                        if (i < m - 1)
                        {
                            result.AddTransition(i + m * j, i + 1 + m * (j + 1), NFA.Any);
                            result.AddTransition(i + m * j, i + 1 + m * (j + 1), NFA.Epsilon);
                        }

                        result.AddTransition(i + m * j, i + m * (j + 1), NFA.Any);
                    }
                }
            }

            return result;
        }
    }
    
    public class DFA
    {
        private readonly List<int> star = new List<int>();
        private readonly List<List<Tuple<char, int>>> transitions = new List<List<Tuple<char, int>>>();
        private readonly HashSet<int> final = new HashSet<int>();

        public void AddState(int state, bool isFinal)
        {
            if (state != transitions.Count)
            {
                throw new ArgumentException();
            }

            star.Add(-1);
            transitions.Add(new List<Tuple<char, int>>());
            if (isFinal)
            {
                final.Add(state);
            }
        }

        public void AddTransition(int from, int to, char c)
        {
            if (c == NFA.Any)
            {
                star[from] = to;
            }
            else
            {
                transitions[from].Add(new Tuple<char, int>(c, to));
            }
        }

        public int Next(int s, char c)
        {
            if (s == -1) return -1;
            foreach (var t in transitions[s])
            {
                if (t.Item1 == c) return t.Item2;
            }
            return star[s];
        }

        public bool IsFinal(int s)
        {
            return final.Contains(s);
        }

        public string ToDotNotation()
        {
            var result = new StringBuilder();
            result.AppendLine("digraph DFA {");
            result.AppendLine("rankdir = LR;");
            result.AppendLine("orientation = Portrait;");

            for (int i = 0; i < transitions.Count; ++i)
            {
                if (i == 0)
                {
                    result.AppendFormat("{0}[label = \"{0}\", shape = circle, style = bold, fontsize = 14]", i);
                    result.AppendLine();
                }
                else if (final.Contains(i))
                {
                    result.AppendFormat("{0}[label = \"{0}\", shape = doublecircle, style = bold, fontsize = 14]", i);
                    result.AppendLine();
                }
                else {
                    result.AppendFormat("{0}[label = \"{0}\", shape = circle, style = solid, fontsize = 14]", i);
                    result.AppendLine();
                }

                foreach (var t in transitions[i])
                {
                    result.AppendFormat("{0}->{1} [label = \"{2}\", fontsize = 14];", i, t.Item2, t.Item1);
                    result.AppendLine();
                }

                if (star[i] >= 0)
                {
                    result.AppendFormat("{0}->{1} [label = \"*\", fontsize = 14];", i, star[i]);
                    result.AppendLine();
                }
            }

            result.AppendLine("}");
            return result.ToString();
        }
    }

    /// <summary>
    /// Nondeterministic finite automaton
    /// https://e...content-available-to-author-only...a.org/wiki/Nondeterministic_finite_automaton
    /// </summary>
    public class NFA
    {
        public static char Epsilon = 'ε';
        public static char Any = '*';

        private int initial = 0;
        private readonly List<int> states = new List<int>();
        private readonly List<Tuple<int, int, char>> transitions = new List<Tuple<int, int, char>>();
        private readonly HashSet<int> final = new HashSet<int>();

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
            if (!transitions.Any(t => t.Item1 == from && t.Item2 == to && t.Item3 == c))
            {
                transitions.Add(new Tuple<int, int, char>(from, to, c));
            }
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
            var finals = result.final.Where(h => h != newFinal).ToArray();
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

        public string ToDotNotation()
        {
            var result = new StringBuilder();
            result.AppendLine("digraph NFA {");
            result.AppendLine("rankdir = LR;");
            result.AppendLine("orientation = Portrait;");

            for (int i = 0; i < states.Count; ++i)
            {
                if (states[i] == initial)
                {
                    result.AppendFormat("{0}[label = \"{0}\", shape = circle, style = bold, fontsize = 14]", states[i]);
                    result.AppendLine();
                }
                else if (final.Contains(states[i]))
                {
                    result.AppendFormat("{0}[label = \"{0}\", shape = doublecircle, style = bold, fontsize = 14]", states[i]);
                    result.AppendLine();
                }
                else {
                    result.AppendFormat("{0}[label = \"{0}\", shape = circle, style = solid, fontsize = 14]", states[i]);
                    result.AppendLine();
                }

                foreach (var t in transitions)
                {
                    if (t.Item1 != states[i]) continue;
                    if (t.Item3 == Any)
                    {
                        result.AppendFormat("{0}->{1} [label = \"*\", fontsize = 14];", t.Item1, t.Item2);
                        result.AppendLine();
                    }
                    else if (t.Item3 == Epsilon)
                    {
                        result.AppendFormat("{0}->{1} [label = \"&epsilon;\", fontsize = 14];", t.Item1, t.Item2);
                        result.AppendLine();
                    }
                    else
                    {
                        result.AppendFormat("{0}->{1} [label = \"{2}\", fontsize = 14];", t.Item1, t.Item2, t.Item3);
                        result.AppendLine();
                    }
                }
            }

            result.AppendLine("}");
            return result.ToString();
        }

        public DFA Determinize()
        {
            var target = new DFA();

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

                HashSet<char> inputs = new HashSet<char>();
                foreach (var st in current_set)
                {
                    foreach (var t in FindTransitions(st))
                    {
                        if (t.Item3 == Epsilon)
                        {
                            continue;
                        }
                        inputs.Add(t.Item3);
                    }
                }

                foreach (var i in inputs)
                {
                    HashSet<int> new_state = new HashSet<int>();

                    foreach (var st in current_set)
                    {
                        foreach (var t in FindTransitions(st).Where(j => j.Item3 == i || j.Item3 == Any))
                        {
                            new_state.Add(t.Item2);
                        }
                    }

                    EpsilonClosure(new_state);

                    int seen_state_key;
                    if (!seen.TryGetValue(new_state, out seen_state_key))
                    {
                        set_key++;

                        target.AddState(set_key, ContainsFinalState(new_state));
                        target.AddTransition(current_key, set_key, i);

                        frontier.Push(new Tuple<int, HashSet<int>>(set_key, new_state));
                        seen[new_state] = set_key;
                    }
                    else
                    {
                        target.AddTransition(current_key, seen_state_key, i);
                    }
                }
            }

            return target;
        }
    }

    public static class Program
    {
    	public static void Main(string[] args)
        {
           var words = Console.ReadLine().Split(' ');
 
           Console.WriteLine($"Matching words {words[0]} and {words[1]} using Levenshtein automaton with distance {words[2]}:");
           Console.WriteLine(LevenshteinAutomaton.Match(words[0], words[1], int.Parse(words[2])));
        }
    }    
}
