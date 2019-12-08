using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PMS.Common.Collections
{
    public class PathStep
    {
        public int V;
        public List<PathStep> before;
        public int length;
    }

    public static class ShortestPath
    {
        public static IEnumerable<PathStep> FindBFS(int from, int to, IGraph g)
        {
            var wave = new Queue<PathStep>();
            var inwave = new Dictionary<int, PathStep>();
            var bestSoFar = new Dictionary<int, int>();
            var bestPaths = new List<PathStep>();

            int minLength = int.MaxValue;

            PathStep p = new PathStep
            {
                V = from,
                before = new List<PathStep>(),
                length = 0
            };

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
                    if (!bestSoFar.TryGetValue(edge.to, out var kl) || kl >= current.length + 1)
                    {
                            bestSoFar[edge.to] = current.length + 1;
                            if (!inwave.TryGetValue(edge.to, out var knownPath))
                            {
                                PathStep nextStep = new PathStep
                                {
                                    V = edge.to,
                                    before = new List<PathStep>() { current },
                                    length = current.length + 1
                                };

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
                                    knownPath.before.Add(current);
                                }
                            }
                    }
                }
            }
            return bestPaths;
        }
    

        /* Returns true if there is a path 
        from source 's' to sink 't' in residual 
        graph. Also fills parent[] to store the 
        path */
        private static bool bfs(int [,]rGraph, int s, int t, int[] parent, int V) 
        { 
            // Create a visited array and mark 
            // all vertices as not visited 
            bool []visited = new bool[V]; 
            for(int i = 0; i < V; ++i) 
                visited[i] = false; 

            // Create a queue, enqueue source vertex and mark 
            // source vertex as visited 
            List<int> queue = new List<int>(); 
            queue.Add(s); 
            visited[s] = true; 
            parent[s] = -1; 

            // Standard BFS Loop 
            while (queue.Count != 0) 
            { 
                int u = queue[0]; 
                    queue.RemoveAt(0); 

                for (int v = 0; v < V; v++) 
                { 
                    if (visited[v] == false && rGraph[u, v] > 0) 
                    { 
                        queue.Add(v); 
                        parent[v] = u; 
                        visited[v] = true; 
                    } 
                } 
            } 

            // If we reached sink in BFS 
            // starting from source, then 
            // return true, else false 
            return (visited[t] == true); 
        } 

/*
           Output should be 4

            int [,]graph =new int[,] {// 0  1  2  3  4  5  6  7  8  9
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
                                    };
            Console.WriteLine("The maximum possible flow is " + 
                           ShortestPath.FordFulkerson(graph, 0, 9, 10));

*/

        // Returns tne maximum flow 
        // from s to t in the given graph 
        public static int FordFulkerson(int [,]graph, int s, int t, int V) 
        { 
            int u, v; 

            // Create a residual graph and fill 
            // the residual graph with given 
            // capacities in the original graph as 
            // residual capacities in residual graph 

            // Residual graph where rGraph[i,j] 
            // indicates residual capacity of 
            // edge from i to j (if there is an 
            // edge. If rGraph[i,j] is 0, then 
            // there is not) 
            int [,]rGraph = new int[V, V];

            for (u = 0; u < V; u++) 
                for (v = 0; v < V; v++) 
                    rGraph[u, v] = graph[u, v]; 

            // This array is filled by BFS and to store path 
            int []parent = new int[V]; 

            int max_flow = 0; // There is no flow initially 

            // Augment the flow while tere is path from source 
            // to sink 
            while (bfs(rGraph, s, t, parent, V))
            { 
                // Find minimum residual capacity of the edhes 
                // along the path filled by BFS. Or we can say 
                // find the maximum flow through the path found. 
                int path_flow = int.MaxValue; 
                for (v = t; v != s; v = parent[v]) 
                { 
                    u = parent[v]; 
                    path_flow = Math.Min(path_flow, rGraph[u,v]); 
                } 

                // update residual capacities of the edges and 
                // reverse edges along the path 
                for (v = t; v != s; v = parent[v]) 
                { 
                    u = parent[v]; 
                    rGraph[u,v] -= path_flow; 
                    rGraph[v,u] += path_flow; 
                } 

                // Add path flow to overall flow 
                max_flow += path_flow; 
            } 

            for (int i=1; i<V-1; i++)
              for (int j=1; j<V-1; j++)
                if (i < j && rGraph[j,i] > 0)
                    Console.WriteLine($"{i} -> {j}");

            // Return the overall flow 
            return max_flow;
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

    }

}