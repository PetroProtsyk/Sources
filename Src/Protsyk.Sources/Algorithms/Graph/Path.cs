using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Protsyk.DataStructures;

namespace Protsyk.Sources.Algorithms.Graph
{
    public class PathStep
    {
        public readonly int V;
        public readonly List<PathStep> before;
        public readonly int length;

        public PathStep(int start)
        {
            this.V = start;
            this.before = new List<PathStep>();
            this.length = 0;
        }

        public PathStep(PathStep from, int to, int toWeight)
        {
            this.V = to;
            this.before = new List<PathStep>() { from };
            this.length = from.length + toWeight;
        }

        public void AddAlternative(PathStep path)
        {
            before.Add(path);
        }
    }

    public static class ShortestPath
    {
        public static IEnumerable<PathStep> FindAllShortestPathsUsingBFS(IGraph g, int from, int to)
        {
            var wave = new Queue<PathStep>();
            var inwave = new Dictionary<int, PathStep>();
            var bestSoFar = new Dictionary<int, int>();
            var bestPaths = new List<PathStep>();

            int minLength = int.MaxValue;
            var p = new PathStep(from);

            inwave.Add(from, p);
            bestSoFar.Add(from, 0);
            wave.Enqueue(p);

            while (wave.Count > 0)
            {
                var current = wave.Dequeue();
                inwave.Remove(current.V);

                if (current.V == to)
                {
                    if (current.length <= minLength)
                    {
                        bestPaths.Add(current);
                        minLength = current.length;
                    }
                    continue;
                }

                if (current.length >= minLength)
                {
                    continue;
                }

                foreach (var edge in g.EdgesFrom(current.V))
                {
                    if (edge.weight != 1)
                    {
                        throw new Exception("This algorithm does not work on weighted graphs");
                    }

                    if (!bestSoFar.TryGetValue(edge.to, out var kl) || kl >= current.length + 1)
                    {
                            bestSoFar[edge.to] = current.length + 1;
                            if (!inwave.TryGetValue(edge.to, out var knownPath))
                            {
                                var nextStep = new PathStep(current, edge.to, 1);
                                wave.Enqueue(nextStep);
                                inwave.Add(edge.to, nextStep);
                            }
                            else
                            {
                                // The algorithm have already reached vertex edge.to
                                // If previous length is the same as current, then
                                // a new alternative path is found
                                if (knownPath.length == current.length + 1)
                                {
                                    knownPath.AddAlternative(current);
                                }
                            }
                    }
                }
            }
            return bestPaths;
        }

        public static PathStep FindPathsWithLowestWeightBFS(IGraph graph, int from, int to)
        {
            var seen = new HashSet<int>();
            var wave = new Heap<PathStep>(
                    Comparer<PathStep>.Create((x, y) => x.length - y.length));

            wave.Add(new PathStep(from));

            while(!wave.IsEmpty)
            {
                var top = wave.RemoveTop();

                if (top.V == to)
                {
                    return top;
                }

                foreach (var edge in graph.EdgesFrom(top.V))
                {
                    if (edge.weight <= 0)
                    {
                        throw new Exception("This algorithm does not work on graphs with negative or zero edges");
                    }

                    if (seen.Add(edge.to))
                    {
                        var nextStep = new PathStep(top, edge.to, edge.weight);
                        wave.Add(nextStep);
                    }
                }
            }

            return null;
        }

        public static void DepthFirstSearch(IGraph graph, int root)
        {
            var pre = new Dictionary<int, int>();
            var post = new Dictionary<int, int>();
            DepthFirstSearch(graph, root, pre, post);
        }

        private static void DepthFirstSearch(IGraph graph, int root, Dictionary<int, int> pre, Dictionary<int, int> post)
        {
            pre.Add(root, pre.Count);
            foreach(var edge in graph.EdgesFrom(root))
            {
                if (pre.ContainsKey(edge.to))
                {
                    continue;
                }
                DepthFirstSearch(graph, edge.to, pre, post);
            }
            post.Add(root, post.Count);
        }

        /// Returns graph edges of the maximum flow
        public static IEnumerable<Edge> FordFulkerson(ArrayGraph graph, int source, int target) 
        {
            var residualGraph = ArrayGraph.CloneFromGraph(graph);
            int maxFlow = 0;

            while (true)
            {
                // Find augementing path from source to sink in the residual graph
                var path = FindPathsWithLowestWeightBFS(residualGraph, source, target);
                if (path == null)
                {
                    break;
                }

                var pathFlow = path.ToEdges()
                                   .Select(e => residualGraph[e.from, e.to])
                                   .Min();

                foreach(var edge in path.ToEdges())
                {
                    residualGraph[edge.from, edge.to] -= pathFlow;
                    residualGraph[edge.to, edge.from] += pathFlow;
                }

                maxFlow += pathFlow;
            } 

            // Flow from the source should be equal to max flow
            var fromSource = graph
                                .EdgesFrom(source)
                                .Where(e => residualGraph[e.from, e.to] < graph[e.from, e.to])
                                .Sum(e => graph[e.from, e.to] - residualGraph[e.from, e.to]);

            if (maxFlow != fromSource)
            {
                throw new Exception($"Something is broken. Expected {maxFlow}, actual {fromSource}");
            }

            for (int i=0; i<residualGraph.VertexesCount(); i++)
            {
                for (int j=0; j<residualGraph.VertexesCount(); j++)
                {
                    if (residualGraph[i,j] < graph[i,j])
                    {
                        yield return new Edge(i, j, graph[i,j] - residualGraph[i,j]);
                    }
                }
            }
        }
    }

    public static class PathExtensions
    {
        public static void Print(this PathStep path)
        {
            Print(path, new List<int>());
        }

        private static void Print(PathStep path, List<int> verticies)
        {
            verticies.Add(path.V);
            if (path.before ==  null || path.before.Count == 0)
            {
                for (int i=0; i<verticies.Count; ++i)
                {
                    Console.Write(verticies[verticies.Count - 1 - i]);
                    if (i < verticies.Count - 1)
                    {
                        Console.Write(" -> ");
                    }
                }
                Console.WriteLine();
            }
            else
            {
                foreach(var before in path.before)
                {
                    Print(before, verticies);
                }
            }
            verticies.RemoveAt(verticies.Count - 1);
        }

        public static IEnumerable<(int from, int to)> ToEdges(this PathStep path)
        {
            var to = path;
            while (to.before.Count > 0)
            {
                if (to.before.Count > 1)
                {
                    throw new Exception("Path is not unique");
                }

                var from = to.before.FirstOrDefault();
                yield return (from.V, to.V);
                to = from;
            }
        }
    }

}