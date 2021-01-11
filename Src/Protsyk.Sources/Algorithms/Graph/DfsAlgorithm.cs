using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Protsyk.Sources.Algorithms.Graph
{
    public static class DfsAlgorithm
    {
        public static IEnumerable<int> FindArticulationPoints(IGraph g)
        {
            return g.Vertexes().Where(x => IsArticulationPoint(g, x));
        }

        public static IEnumerable<int> FindArticulationPointsDFS(IGraph g)
        {
            var result = new HashSet<int>();
            FindAllArticulationDFSRecursive(g,
                                            g.Vertexes().First(),
                                            g.Vertexes().First(),
                                            result,
                                            new Dictionary<int, int>(),
                                            new Dictionary<int, int>());
            return result.OrderBy(x => x);
        }

        public static bool IsArticulationPoint(IGraph g, int vertex)
        {
            var allCount = g.VertexesCount();
            var visited = new HashSet<int>() { vertex };
            var root = g.Vertexes().Where(x => !x.Equals(vertex)).First();
            DFSRecursive(g, root, null, visited);
            return (allCount != visited.Count);
        }

        public static void FindAllArticulationDFSRecursive(IGraph g,
                                                    int rootId,
                                                    int parentId,
                                                    HashSet<int> articulationPoints,
                                                    Dictionary<int, int> visitOrder,
                                                    Dictionary<int, int> minRoots)
        {
            if (visitOrder.ContainsKey(rootId))
            {
                throw new Exception("This should be the case");
            }

            var thisOrder = visitOrder.Count + 1;
            visitOrder.Add(rootId, thisOrder);

            var minV = parentId;
            var subtrees = 0;
            foreach (var t in g.EdgesFrom(rootId).Select(tr => tr.to))
            {
                if (t.Equals(parentId))
                {
                    continue;
                }
                else if (visitOrder.ContainsKey(t))
                {
                    if (visitOrder[t] < visitOrder[minV])
                    {
                        minV = t;
                    }
                }
                else
                {
                    FindAllArticulationDFSRecursive(g, t, rootId, articulationPoints, visitOrder, minRoots);
                    var minT = minRoots[t];
                    var orderT = visitOrder[minT];
                    if (visitOrder[minV] > orderT)
                    {
                        minV = minT;
                    }

                    if (thisOrder <= orderT)
                    {
                        if (!rootId.Equals(parentId))
                        {
                            articulationPoints.Add(rootId);
                        }
                    }

                    ++subtrees;
                }
            }

            minRoots[rootId] = minV;

            if (rootId.Equals(parentId))
            {
                // Special case for the root node of DFS
                // Root is an articulation point, if in DFS tree it has more than one subtree
                if (subtrees > 1)
                {
                    articulationPoints.Add(rootId);
                }
            }
        }

        public static void DFSRecursive(IGraph g, int rootId, Action<int> visitor)
        {
            DFSRecursive(g, rootId, visitor, new HashSet<int>());
        }

        public static void DFSRecursive(IGraph g, int rootId, Action<int> visitor, HashSet<int> visited)
        {
            if (!visited.Add(rootId))
            {
                return;
            }
            visitor?.Invoke(rootId);
            foreach (var t in g.EdgesFrom(rootId).Select(tr => tr.to))
            {
                if (!visited.Contains(t))
                {
                    DFSRecursive(g, t, visitor, visited);
                }
            }
        }

        public static void DFS(IGraph g, int rootId, Action<int> visitor)
        {
            var visited = new HashSet<int>();
            var stack = new Stack<int>();

            stack.Push(rootId);
            while (stack.Count > 0)
            {
                var currentId = stack.Pop();
                if (!visited.Add(currentId))
                {
                    continue;
                }

                visitor?.Invoke(currentId);

                foreach (var t in g.EdgesFrom(currentId).Select(tr => tr.to).Reverse())
                {
                    if (!visited.Contains(t))
                    {
                        stack.Push(t);
                    }
                }
            }
        }
    }
}
