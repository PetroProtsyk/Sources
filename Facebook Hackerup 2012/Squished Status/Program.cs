// Program description: http://protsyk.com/cms/?p=450
// Puzzle origin: http://www.facebook.com/hackercup/problems.php?pid=348968131789235&round=225705397509134

using System;
using System.Collections.Generic;
using System.Linq;

namespace Protsyk.Puzzles.SquishedStatus {
    class Program {
        static void Main(string[] args) {
            ConsoleReader reader = new ConsoleReader();

            /// Read test cases count
            reader.MoveNext();
            int N = int.Parse(reader.Current);

            for (int j = 0; j < N; j++) {

                /// Read Max value
                reader.MoveNext();
                int M = int.Parse(reader.Current);

                List<int> parsedNumbers = new List<int>();

                reader.MoveNext();
                string squished_status = reader.Current;

                /// Split squished_status into the smallest valid numbers
                /// Example: M = 5, squished_status = 141 => parsedNumbers = { 1, 4, 1 }
                /// Example: M = 12, squished_status = 101 => parsedNumbers = { 10, 1 }
                /// Example: M = 2, squished_status = 101 => Error
                int number = 0;
                foreach (char c in squished_status) {
                    if (c == '0') {
                        if (number == 0) {
                            parsedNumbers.Clear();
                            break;
                        } else {
                            number *= 10;
                            if (number > M) {
                                number = 0;
                                parsedNumbers.Clear();
                                break;
                            }
                        }
                    } else {
                        if (number != 0) {
                            parsedNumbers.Add(number);
                        }
                        number = c - '0';
                    }
                }
                if (number > 0) {
                    parsedNumbers.Add(number);
                }

                if (parsedNumbers.Count == 0) {
                    Console.WriteLine(string.Format("Case #{0}: {1}", j + 1, 0));
                } else {

                    List<int> numbers = new List<int>();
                    List<uint> sequenceCounts = new List<uint>();
                    sequenceCounts.Add(1);

                    /// Count numbers using recursive formula
                    for (int i = parsedNumbers.Count - 1; i >= 0; i--) {
                        SolveCounting(M, parsedNumbers[i], numbers, sequenceCounts);
                    }

                    /// For small inputs use exhaustive search for verification
                    if (parsedNumbers.Count < 15) {
                        LinkedList<int> input = new LinkedList<int>(parsedNumbers);
                        if (sequenceCounts.Last() != (1 + SolveEnumerating(M, input.First, input)) % 0xfaceb00c) {
                            Console.WriteLine(string.Format("Case #{0}: {1}", j + 1, "Error"));
                        }
                    }

                    Console.WriteLine(string.Format("Case #{0}: {1}", j + 1, sequenceCounts.Last()));
                }
            }
        }

        private static int Combine(int next, int previous) {
            if (previous < 10) previous += next * 10;
            else if (previous < 100) previous += next * 100;
            else { return int.MaxValue; }
            return previous;
        }

        private static void SolveCounting(int max, int i, List<int> numbers, List<uint> sequenceCounts) {
            ulong newCount = sequenceCounts[sequenceCounts.Count - 1];

            // Combine two digits
            if (numbers.Count > 0) {
                int combine1 = Combine(i, numbers[0]);
                if (combine1 <= max) {
                    checked {
                        newCount += sequenceCounts[sequenceCounts.Count - 2];
                    }

                    // Combine three digits
                    if (numbers.Count > 1) {
                        int combine2 = Combine(combine1, numbers[1]);
                        if (combine2 <= max) {
                            checked {
                                newCount += sequenceCounts[sequenceCounts.Count - 3];
                            }
                        }
                    }
                }
            }

            sequenceCounts.Add((uint)(newCount % 0xfaceb00c));
            numbers.Insert(0, i);
        }

        private static int SolveEnumerating(int max, LinkedListNode<int> current, LinkedList<int> input) {
            int solutions = 0;
            while (current.Next != null) {

                if (current.Value < max) {

                    /// Try to combine consecutive input numbers
                    int combine = Combine(current.Value, current.Next.Value);
                    if (combine > max) {
                        current = current.Next;
                        continue;
                    }

                    int currentValue = current.Value;
                    int nextValue = current.Next.Value;

                    /// Replace number with combined value
                    current.Value = combine;
                    input.Remove(current.Next);

                    /// Recursively call solve method on reduced input
                    solutions += 1 + SolveEnumerating(max, current, input);

                    /// Restore original values
                    current.Value = currentValue;
                    input.AddAfter(current, nextValue);
                }

                current = current.Next;
            }

            return solutions;
        }

        /// <summary>
        /// Helper class for parsing input
        /// </summary>
        class ConsoleReader : IEnumerable<string>, IEnumerator<string> {
            public IEnumerator<string> GetEnumerator() {
                return this;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
                return this;
            }

            public void Dispose() {
            }

            public string Current {
                get { return strings[position]; }
            }

            object System.Collections.IEnumerator.Current {
                get { return strings[position]; }
            }

            public bool MoveNext() {
                position++;
                if (strings != null && position < strings.Length) {
                    return true;
                }

                strings = Console.ReadLine().Split(new char[] { '\r', '\n', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                position = 0;
                return strings.Length > 0;
            }

            public void Reset() {
                position = 0;
                strings = null;
            }

            private int position;
            private string[] strings;
        }

    }
}
