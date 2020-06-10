using System;
using System.Linq;
using Protsyk.Combinatorics.Combinations;
using Xunit;

namespace Protsyk.Sources.UnitTests.Combinatorics
{
    public class CombinationsTest
    {
        [Fact]
        public void TestCombinations()
        {
            Assert.Equal(10ul, CombinationsCalculator.Combinations(5, 3));
            Assert.Equal(10ul, CombinationsCalculator.CombinationsCached(5, 3));
            Assert.Equal(10ul, CombinationsCalculator.CombinationsFormula(5, 3));
            Assert.Equal(10ul, CombinationsCalculator.CombinationsKnuth(5, 3));
        }

        [Fact]
        public void TestCombinationProducer()
        {
            var result = CombinationsCalculator
                            .ProduceCombinations(3, new string[] { "1", "2", "3", "4", "5" })
                            .ToArray();
            Assert.Equal(10, result.Length);
        }
    }
}
