using System;
using System.Linq;
using System.Text;
using Protsyk.Common.UnitTests;

namespace Protsyk.Sources.Algorithms.Graph
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

        public static void FindAllArticulationDFSRecursive()
        {
            // Sedgewick Algorithms p.439 figure 30.2
            var g = LabeledGraph<string>.From(new LabeledVertex<string>[]
                {
                        LabeledVertex<string>.From("A", new string[]{ "F", "B", "C", "G" }),
                        LabeledVertex<string>.From("B", new string[]{ "A" }),
                        LabeledVertex<string>.From("C", new string[]{ "A", "G" }),
                        LabeledVertex<string>.From("D", new string[]{ "F", "E" }),
                        LabeledVertex<string>.From("E", new string[]{ "F", "G", "D" }),
                        LabeledVertex<string>.From("F", new string[]{ "A", "E", "D" }),
                        LabeledVertex<string>.From("G", new string[]{ "A", "L", "E", "H", "J", "C" }),
                        LabeledVertex<string>.From("H", new string[]{ "I", "G" }),
                        LabeledVertex<string>.From("I", new string[]{ "H" }),
                        LabeledVertex<string>.From("J", new string[]{ "G", "L", "K", "M" }),
                        LabeledVertex<string>.From("K", new string[]{ "J" }),
                        LabeledVertex<string>.From("L", new string[]{ "G", "J", "M" }),
                        LabeledVertex<string>.From("M", new string[]{ "J", "L" })
                }
            );

            var sb1 = new StringBuilder();
            DfsAlgorithm.DFS(g, g.GetIdByLabel("A"), x => sb1.AppendFormat("{0} ", g.GetVertexById(x).Label));

            var sb2 = new StringBuilder();
            DfsAlgorithm.DFSRecursive(g, g.GetIdByLabel("A"), x => sb2.AppendFormat("{0} ", g.GetVertexById(x).Label));

            Assert.AreEqual("A F E G L J K M H I C D B ", sb1.ToString());

            Assert.AreEqual(sb1.ToString(), sb2.ToString());

            Assert.AreEqual("A G H J", string.Join(" ", DfsAlgorithm.FindArticulationPoints(g).Select(id => g.GetVertexById(id).Label)));

            Assert.AreEqual("A G H J", string.Join(" ", DfsAlgorithm.FindArticulationPointsDFS(g).Select(id => g.GetVertexById(id).Label)));
        }
    }
}
