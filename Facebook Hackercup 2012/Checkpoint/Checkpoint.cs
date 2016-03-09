// Program description: http://protsyk.com/cms/?p=449
// Puzzle origin: http://www.facebook.com/hackercup/problems.php?pid=191596157517194&round=225705397509134

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protsyk.Puzzles.Checkpoint {
    class Program {

        /// <summary>
        /// Mapping from number N to list of points < (x1, y1), ... >
        /// For each point (x1, y1) in the list of number N following property holds
        /// G(x1, y1) = N, where G(x,y) - is the number of shortest paths from the origin
        /// to (x,y) when only moving right and up is allowed.
        /// </summary>
        static Dictionary<int, List<Tuple<int, int>>> mapping = new Dictionary<int, List<Tuple<int, int>>>();

        static void Main(string[] args) {
            BuildCache();

            int R = int.Parse(Console.ReadLine());
            for (int testcase = 0; testcase < R; testcase++) {
                int S = int.Parse(Console.ReadLine());

                if (S == 1) {
                    Console.WriteLine(string.Format("Case #{0}: {1}", testcase + 1, 2));
                    continue;
                }

                int bestX = int.MaxValue / 2, bestY = int.MaxValue / 2;

                // For each s1,s2 such that s1*s2 = S
                foreach (var factorization in Factorize(S)) {
                    int s1 = factorization.Item1;
                    int s2 = factorization.Item2;

                    // For each grid points for s1
                    foreach (var point1 in GetPointsForNumber(s1)) {
                        // For each grid points for s2
                        foreach (var point2 in GetPointsForNumber(s2)) {

                            // Find best point (with shortest path)
                            if (point1.Item1 + point2.Item1 + point1.Item2 + point2.Item2 < bestX + bestY) {
                                bestX = point1.Item1 + point2.Item1;
                                bestY = point1.Item2 + point2.Item2;
                            }

                        }
                    }
                }

                Console.WriteLine(string.Format("Case #{0}: {1}", testcase + 1, bestX + bestY));
            }
        }

        /// <summary>
        /// Factorize number into two multipliers
        /// </summary>
        static IEnumerable<Tuple<int, int>> Factorize(int s) {
            int s1 = 1;
            int threshold = (int)Math.Sqrt(s) + 1;

            while (s1 <= threshold) {
                int remainder = s % s1;

                if (remainder == 0) {
                    int s2 = s / s1;
                    if (s2 < s1) yield break;
                    yield return new Tuple<int, int>(s1, s2);
                }
                ++s1;
            }
        }

        static Dictionary<int, List<Tuple<int, int>>> BuildCache() {
            List<int> prev = null;
            List<int> next = new List<int>();

            // Start from the second row of the paths matrix G.
            // No need to calculate first and second row, because:
            // G(x,0)=G(0,x)=1
            // G(x,1)=G(1,x)=x+1
            int i = 2;
            int j = 2;

            while (true) {
                while (true) {
                    int number;
                    if (i == j) {
                        if (i == 2)
                            number = 2 * (i + 1); // Use G(x,1)=x+1
                        else
                            number = 2 * prev[1]; // Use previous row
                    } else {
                        if (i == 2)
                            number = next[j - i - 1] + (j + 1); // Use G(x,1)=x+1
                        else
                            number = next[j - i - 1] + prev[j - i + 1]; // G(x,y) = G(x-1,y)+G(x,y-1)
                    }

                    // Task threshold
                    if (number > 10000000)
                        break;

                    List<Tuple<int, int>> pointslist = new List<Tuple<int, int>>();
                    if (!mapping.TryGetValue(number, out pointslist)) {
                        pointslist = new List<Tuple<int, int>>();
                        pointslist.Add(new Tuple<int, int>(number - 1, 1));
                        mapping.Add(number, pointslist);
                    }
                    pointslist.Add(new Tuple<int, int>(i, j));

                    next.Add(number);
                    ++j;
                }

                if (next.Count < 2) {
                    // Only one number, break
                    break;
                } else {
                    prev = next;
                    next = new List<int>(next.Count + 1);
                }

                ++i;
                j = i;
            }
            return mapping;
        }

        /// <summary>
        /// Get points from the cache
        /// </summary>
        static IEnumerable<Tuple<int, int>> GetPointsForNumber(int s) {
            if (s == 1) {
                // Return point closest to origin
                yield return new Tuple<int, int>(1, 0);
                yield break;
            }

            List<Tuple<int, int>> result;
            if (mapping.TryGetValue(s, out result)) {
                foreach (Tuple<int, int> point in result)
                    yield return point;
            } else {
                // There is only one point for such number,
                // it is in the first row of the matrix
                yield return new Tuple<int, int>(s - 1, 1);
            }
        }

    }
}
