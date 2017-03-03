// https://www.facebook.com/hackercup/problem/1036037553088752/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2 {
    class Program {
        static void Main(string[] args) {
            int T = int.Parse(Console.ReadLine());
            for (int i = 0; i < T; i++) {
                var goal = Console.ReadLine()
                                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => int.Parse(s))
                                .ToArray();

                int N = int.Parse(Console.ReadLine());
                List<int[]> p = new List<int[]>();

                for (int j = 0; j < N; j++) {
                    var product = Console.ReadLine()
                                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(s => int.Parse(s))
                                    .ToArray();

                    p.Add(product);
                }

                // Remove products that are comparable
                p.Sort((p1, p2) => {
                    return p2.Min() - p1.Min();
                });
                HashSet<int> toRemove = new HashSet<int>();
                for (int v = p.Count - 1; v > 0; --v) {
                    if (toRemove.Contains(v)) continue;
                    for (int m = v - 1; m >= 0; --m) {
                        if (p[m][0] % p[v][0] == 0 &&
                            p[m][1] % p[v][1] == 0 &&
                            p[m][2] % p[v][2] == 0 &&
                            p[m][0] / p[v][0] == p[m][1] / p[v][1] &&
                            p[m][2] / p[v][2] == p[m][1] / p[v][1]) {
                            toRemove.Add(m);
                        }
                    }
                }
                List<int[]> pn = new List<int[]>();
                for (int j = 0; j < p.Count; j++) {
                    if (toRemove.Contains(j)) continue;
                    pn.Add(p[j]);
                }
                p = pn;

                p.Sort((p1, p2) => {
                    int[] ranges1 = new int[3];
                    int[] ranges2 = new int[3];
                    for (int l = 0; l < 3; l++) {
                        ranges1[l] = goal[l] / p1[l];
                        ranges2[l] = goal[l] / p2[l];
                    }

                    Array.Sort(ranges1);
                    Array.Sort(ranges2);

                    for (int l = 0; l < 3; l++) {
                        if (ranges1[l] > ranges2[l]) {
                            return 1;
                        } else if (ranges1[l] < ranges2[l]) {
                            return -1;
                        }
                    }
                    return 0;
                });

                bool result = Solve(goal, p, 0);
                Console.WriteLine("Case #{0}: {1}", i + 1, result ? "yes" : "no");
            }
        }

        private static bool Solve(int[] goal, List<int[]> p, int c) {
            if (goal[0] == 0 && goal[1] == 0 && goal[2] == 0) {
                return true;
            }

            if (c >= p.Count) {
                return false;
            }

            int minRange = int.MaxValue;
            for (int l = 0; l < 3; l++) {
                minRange = Math.Min(minRange, goal[l] / p[c][l]);
            }

            int[] newGoal = new int[3];
            for (int i = 0; i <= minRange; ++i) {
                newGoal[0] = goal[0] - p[c][0] * i;
                newGoal[1] = goal[1] - p[c][1] * i;
                newGoal[2] = goal[2] - p[c][2] * i;
                if (Solve(newGoal, p, c + 1)) {
                    return true;
                }
            }

            return false;
        }
    }
}
