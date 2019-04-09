using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniMaxStandard;

namespace MinimaxTests
{
    [TestClass]
    public class MinimaxTests
    {
        [TestMethod]
        public void ReturnsCorrectValue()
        {
            var node = new SimpleMockIMinimaxNode();
            var result = (new Minimax<MockIGameMove>()).Run(node, 3, maximizing: true);

            Assert.AreEqual(4, result.Score);
        }        

        [TestMethod]
        public void EndsAtSetDepth()
        {
            var node = new InfinteMockIMinimaxNode();
            var result = (new Minimax<MockIGameMove>()).Run(node, 15, maximizing: true);

            Assert.AreEqual(1, result.Score);
        }

    }

}
