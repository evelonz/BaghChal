using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniMaxStandard;

namespace MinimaxTests
{
    [TestClass]
    public class AlphaBetaPruningTests
    {
        [TestMethod]
        public void ReturnsCorrectValueWithPruning()
        {
            var node = new Simple2MockIMinimaxNode();
            var runner = new AlphaBetaPruning<MockIGameMove>();
            var result = runner.Run(node, 3, true);
            var checkedNodes = runner.EndNodesChecked;

            Assert.AreEqual(5, result.Score, "Expected result does not match.");
            Assert.AreEqual(5, checkedNodes, "Expected number of checked leaf nodes does not match.");
        }
    }

}
