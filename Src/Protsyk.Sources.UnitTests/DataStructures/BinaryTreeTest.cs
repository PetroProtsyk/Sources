using System;
using System.Linq;
using System.Text;
using Protsyk.Combinatorics.Combinations;
using Xunit;
using static Protsyk.Sources.DataStructures.BinaryTree;

namespace Protsyk.Sources.UnitTests.Combinatorics
{
    public class BinaryTreeTest
    {
        [Fact]
        public void TestCombinations()
        {
            // (2+3)*4
            var expr = Node<string>.From(
                "*",
                Node<string>.From(
                    "+",
                    Node<string>.From("2"),
                    Node<string>.From("3")
                ),
                Node<string>.From("4")
            );

            {
                var sb = new StringBuilder();
                Traverse(expr, TraverseType.PreOrder, x => { sb.Append(x); sb.Append(" "); });
    
                Assert.Equal("* + 2 3 4 ", sb.ToString());
            }
            {
                var sb = new StringBuilder();
                Traverse(expr, TraverseType.InOrder, x => { sb.Append(x); sb.Append(" "); } );


                Assert.Equal("2 + 3 * 4 ", sb.ToString());
            }
            {
                var sb = new StringBuilder();
                Traverse(expr, TraverseType.PostOrder, x => { sb.Append(x); sb.Append(" "); } );


                Assert.Equal("2 3 + 4 * ", sb.ToString());
            }
        }
    }
}
