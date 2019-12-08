using System;
using System.Linq;
using Protsyk.Common.UnitTests;

namespace Protsyk.Collections.GraphTests
{
    public static class GraphTests
    {
        // Examples are taken from this article
        // http://www.geeksforgeeks.org/greedy-algorithms-set-5-prims-minimum-spanning-tree-mst-2/
        private static int[,] graphA = new int[,] {
                                   {0, 2, 0, 6, 0},
                                   {2, 0, 3, 8, 5},
                                   {0, 3, 0, 0, 7},
                                   {6, 8, 0, 0, 9},
                                   {0, 5, 7, 9, 0},
                               };

        private static Edge[] MstGraphA = new Edge[]
                                          {
                                              new Edge(0, 1, 2),
                                              new Edge(1, 2, 3),
                                              new Edge(1, 4, 5),
                                              new Edge(0, 3, 6),
                                          };

        private static int[,] graphB = new int[,] {
                                    {0, 4, 0, 0, 0, 0, 0, 8, 0},
                                    {4, 0, 8, 0, 0, 0, 0, 11, 0},
                                    {0, 8, 0, 7, 0, 4, 0, 0, 2},
                                    {0, 0, 7, 0, 9, 14, 0, 0, 0},
                                    {0, 0, 0, 9, 0, 10, 0, 0, 0},
                                    {0, 0, 4, 14, 10, 0, 2, 0, 0},
                                    {0, 0, 0, 0, 0, 2, 0, 1, 6},
                                    {8, 11, 0, 0, 0, 0, 1, 0, 7},
                                    {0, 0, 2, 0, 0, 0, 6, 7, 0},
                                };

        private static Edge[] MstGraphB = new Edge[]
                                          {
                                              new Edge(0, 1, 4),
                                              new Edge(0, 7, 8),
                                              new Edge(2, 8, 2),
                                              new Edge(2, 3, 7),
                                              new Edge(3, 4, 9),
                                              new Edge(5, 2, 4),
                                              new Edge(6, 5, 2),
                                              new Edge(7, 6, 1),
                                          };

        public static void PrimsMinimumSpanningTree_1()
        {
            {
                var expected = MstGraphA.OrderBy(e => e.from).ToArray();
                var actual = PrimAlgorithm.MinimumSpanningTreeWithHeap(new ArrayGraph(graphA))
                                          .OrderBy(e => e.from)
                                          .ToArray();

                Assert.AreEqualSequences(expected, actual, EdgeComparer.UndirectedEdgeComparer);
            }

            {
                var expected = MstGraphB.OrderBy(e => e.from).ToArray();
                var actual = PrimAlgorithm.MinimumSpanningTreeWithHeap(new ArrayGraph(graphB))
                                          .OrderBy(e => e.from)
                                          .ToArray();

                Assert.AreEqualSequences(expected, actual, EdgeComparer.UndirectedEdgeComparer);
            }

            //foreach (var edge in actual)
            //{
            //    Console.WriteLine($"{edge.from} -> {edge.to} \t {edge.weight}");
            //}
        }

        public static void PrimsMinimumSpanningTree_2()
        {
            {
                var expected = MstGraphA.OrderBy(e => e.from).ToArray();
                var actual = PrimAlgorithm.MinimumSpanningTree(new ArrayGraph(graphA))
                                          .OrderBy(e => e.from)
                                          .ToArray();

                Assert.AreEqualSequences(expected, actual, EdgeComparer.UndirectedEdgeComparer);
            }

            {
                var expected = MstGraphB.OrderBy(e => e.from).ToArray();
                var actual = PrimAlgorithm.MinimumSpanningTree(new ArrayGraph(graphB))
                                          .OrderBy(e => e.from)
                                          .ToArray();

                Assert.AreEqualSequences(expected, actual, EdgeComparer.UndirectedEdgeComparer);
            }
        }

        public static void KruskalMinimumSpanningTree()
        {
            {
                var expected = MstGraphA.OrderBy(e => e.weight).ToArray();
                var actual = KruskalAlgorithm.MinimumSpanningTree(new ArrayGraph(graphA))
                                          .OrderBy(e => e.weight)
                                          .ToArray();

                Assert.AreEqualSequences(expected, actual, EdgeComparer.UndirectedEdgeComparer);
            }

            {
                var expected = MstGraphB.OrderBy(e => e.weight).ToArray();
                var actual = KruskalAlgorithm.MinimumSpanningTree(new ArrayGraph(graphB))
                                          .OrderBy(e => e.weight)
                                          .ToArray();

                Assert.AreEqualSequences(expected, actual, EdgeComparer.UndirectedEdgeComparer);
            }
        }

        public static void MaxBipartitie()
        {
            var graph = new ArrayGraph(new int[,]
                                        { // 0  1  2  3  4  5  6  7  8  9
                                            {0, 1, 1, 1, 1, 0, 0, 0, 0, 0}, // 0
                                            {0, 0, 0, 0, 0, 1, 0, 1, 0, 0}, // 1
                                            {0, 0, 0, 0, 0, 1, 1, 0, 0, 0}, // 2
                                            {0, 0, 0, 0, 0, 1, 0, 1, 1, 0}, // 3
                                            {0, 0, 0, 0, 0, 0, 1, 1, 0, 0}, // 4
                                            {0, 0, 0, 0, 0, 0, 0, 0, 0, 1}, // 5
                                            {0, 0, 0, 0, 0, 0, 0, 0, 0, 1}, // 6
                                            {0, 0, 0, 0, 0, 0, 0, 0, 0, 1}, // 7
                                            {0, 0, 0, 0, 0, 0, 0, 0, 0, 1}, // 8
                                            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}  // 9
                                        });

            int maxFlow = ShortestPath
                            .FordFulkerson(graph, 0, 9)
                            .Where(e => e.from == 0)
                            .Sum(e => e.weight);

            Assert.AreEqual(4, maxFlow);

            // Max bipartitie match:
            foreach (var edge in ShortestPath
                                    .FordFulkerson(graph, 0, 9)
                                    .Where(e => e.from != 0 && e.to != 9))
            {
                Console.WriteLine($"{edge.from} --> {edge.to}");
            }
        }
    }
}
