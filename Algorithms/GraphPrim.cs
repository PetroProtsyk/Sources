using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protsyk.Algorithms
{
    public struct Edge
    {
        public readonly int from;
        public readonly int to;
        public readonly int weight;

        public Edge(int from, int to, int weight)
        {
            this.from = from;
            this.to = to;
            this.weight = weight;
        }
    }

    public interface IGraph
    {
        int VertexesCount();

        IEnumerable<int> Vertexes();

        IEnumerable<Edge> Edges();

        IEnumerable<Edge> EdgesFrom(int v); 
    }

    public class ArrayGraph : IGraph
    {
        private readonly int [,] adjacencyMatrix;

        public ArrayGraph(int [,] adjacencyMatrix)
        {
            if (adjacencyMatrix == null)
                throw new ArgumentNullException();
            if (adjacencyMatrix.GetLength(0) != adjacencyMatrix.GetLength(1))
                throw new ArgumentException();

            this.adjacencyMatrix = adjacencyMatrix;
        }

        public int VertexesCount()
        {
            return adjacencyMatrix.GetLength(0);
        }

        public IEnumerable<int> Vertexes()
        {
            return Enumerable.Range(0, adjacencyMatrix.GetLength(0));
        }

        public IEnumerable<Edge> Edges()
        {
            for (int i=0; i<adjacencyMatrix.GetLength(0); ++i)
            {
              for (int j=0; j<adjacencyMatrix.GetLength(1); ++j)
              {
                if (adjacencyMatrix[i,j]>0)
                {
                    yield return new Edge(i,j,adjacencyMatrix[i,j]);
                }
              }
            }
        }

        public IEnumerable<Edge> EdgesFrom(int v)
        {
              for (int j=0; j<adjacencyMatrix.GetLength(1); ++j)
              {
                if (adjacencyMatrix[v,j]>0)
                    yield return new Edge(v,j,adjacencyMatrix[v,j]);
              }
        }

    }

    public class Prims
    {
        public static List<Edge> BuildMinimumSpanningTree(IGraph g)
        {
          var seen = new HashSet<int>();
          var notSeen = new HashSet<int>(g.Vertexes());
          var result = new List<Edge>();

          var first = notSeen.First();
          notSeen.Remove(first);
          seen.Add(first);

          while (notSeen.Count > 0)
          {
              var minE = new Edge(0,0,int.MaxValue);
              var found = false; 

              foreach (var s in seen)
              {
                 foreach (var e in g.EdgesFrom(s))
                 {
                     if (seen.Contains(e.from)
                         && !seen.Contains(e.to)
                         && minE.weight > e.weight)
                     {
                         minE = e;
                         found = true;
                     }
                 }
              }

              if (!found)
              {
                  // Graph is not connected
                  return null;
              }

              seen.Add(minE.to);
              notSeen.Remove(minE.to);
              result.Add(minE);
          }
          return result;
        }
    }

    public static class Program
    {
    	public static void Main(string[] args)
        {
           // Examples are taken from this article
           // http://www.geeksforgeeks.org/greedy-algorithms-set-5-prims-minimum-spanning-tree-mst-2/
           var graph = new int[,] {
                        {0, 2, 0, 6, 0},
                        {2, 0, 3, 8, 5},
                        {0, 3, 0, 0, 7},
                        {6, 8, 0, 0, 9},
                        {0, 5, 7, 9, 0},
                       };

           var graph1 = new int[,] {
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


            foreach(var edge in Prims.BuildMinimumSpanningTree(new ArrayGraph(graph1)))
            {
                Console.WriteLine($"{edge.from} -> {edge.to} \t {edge.weight}");
            }
        }
    }
}
