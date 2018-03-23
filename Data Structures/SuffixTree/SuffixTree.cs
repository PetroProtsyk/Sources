using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Protsyk.DataStructures
{
    // Abstract Suffix Tree class. Defines interface for different implementations of
    // suffix tree construction algorithms and internal representations
    //
    // Usage example:
    //        var tree = SuffixTree.Build("cacao", SuffixTreeAlgorithm.UkkonenLinear);
    //        var matches = tree.Match("ca").ToArray();
    //        Console.WriteLine(tree.ToDotNotation());
    //
    // Dot notation can be rendered here: http://www.webgraphviz.com
    public abstract class SuffixTree
    {
        #region Fields
        protected static char TerminationCharacter = '$';
        #endregion

        #region Api
        public abstract bool IsMatch(string substring);

        public abstract IEnumerable<int> Match(string substring);

        public abstract string ToDotNotation();
        #endregion

        #region Construction
        public static SuffixTree Build(string text, SuffixTreeAlgorithm algorithm)
        {
            if (text ==  null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (text.Contains(TerminationCharacter))
            {
                throw new ArgumentException("Input contains termination character", nameof(text));
            }

            switch (algorithm)
            {
                case SuffixTreeAlgorithm.Naive:
                    return new SuffixTreeNaive(text);

                case SuffixTreeAlgorithm.UkkonenCubic: // O(n^3)
                    return new SuffixTreeUkkonenCubic(text);

                case SuffixTreeAlgorithm.UkkonenQuadratic: // O(n^2)
                    return new SuffixTreeUkkonenQuadratic(text);

                case SuffixTreeAlgorithm.UkkonenLinear: // O(n)
                    return new SuffixTreeUkkonenLinear(text);

                default:
                    throw new NotImplementedException($"No implementation for algoritm {algorithm}");
            }
        }
        #endregion
    }

    public enum SuffixTreeAlgorithm
    {
        Naive,
        UkkonenCubic,
        UkkonenQuadratic,
        UkkonenLinear,
        Weiner,
        McCreight
    }
}
