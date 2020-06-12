using System;
using System.Linq;
using Protsyk.Sources.Puzzles.EggDrop;
using Xunit;

namespace Protsyk.Sources.UnitTests.Puzzles.EggDrop
{
    public class EggDropTest
    {
        [Fact]
        public void TestEggDropSolution()
        {
            {
                int result = new EggDropSolution().BestDrop(1, 100);
                Assert.Equal(100, result);
            }

            {
                int result = new EggDropSolution().BestDrop(2, 36);
                Assert.Equal(8, result);
            }
        }
    }
}
