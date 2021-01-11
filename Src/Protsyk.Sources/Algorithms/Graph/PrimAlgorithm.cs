using System.Collections.Generic;
using System.Linq;
using Protsyk.DataStructures;

namespace Protsyk.Sources.Algorithms.Graph
{
    public class PrimAlgorithm
    {
        public static List<Edge> MinimumSpanningTree(IGraph g)
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

        public static List<Edge> MinimumSpanningTreeWithHeap(IGraph g)
        {
            var seen = new HashSet<int>();
            var notSeen = new HashSet<int>(g.Vertexes());
            var result = new List<Edge>();
            var prio = new Heap<Edge>(Comparer<Edge>.Create((x, y) => x.weight - y.weight));

            var first = notSeen.First();
            notSeen.Remove(first);
            seen.Add(first);

            foreach (var e in g.EdgesFrom(first))
            {
                prio.Add(e);
            }

            while (notSeen.Count > 0)
            {
                var minE = prio.RemoveTop();
                if (seen.Add(minE.to))
                {
                    notSeen.Remove(minE.to);

                    // Decrease key
                    foreach (var e in g.EdgesFrom(minE.to))
                    {
                        prio.Add(e);
                    }

                    result.Add(minE);
                }
            }

            return result;
        }
    }
}
