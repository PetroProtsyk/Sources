using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Protsyk.Collections
{
    public class KruskalAlgorithm
    {
        public static List<Edge> MinimumSpanningTree(IGraph g)
        {
            var result = new List<Edge>();

            var edges = g.Edges().OrderBy(e => e.weight).ToArray();
            var vertexCount = g.VertexesCount();
            var sets = new DisjointSets<int>();
            foreach (var edge in edges)
            {
                if (sets.ItemCount == vertexCount)
                {
                    break;
                }

                var setFrom = sets.MakeSet(edge.from);
                var setTo = sets.MakeSet(edge.to);

                if (setFrom != setTo)
                {
                    sets.Union(setFrom, setTo);
                    result.Add(edge);
                }
            }

            return result;
        }
    }
}
