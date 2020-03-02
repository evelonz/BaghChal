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

        [TestMethod]
        public void ReturnsCorrectValueWithPruningParallel()
        {
            var node = new Simple2MockIMinimaxNode();
            var runner = new AlphaBetaPruningParallel<MockIGameMove>();
            var result = runner.Run(node, 3, true, 1000, 0);
            var checkedNodes = runner.EndNodesChecked;

            Assert.AreEqual(5, result.Score, "Expected result does not match.");
            Assert.AreEqual(5, checkedNodes, "Expected number of checked leaf nodes does not match.");
        }

        [TestMethod]
        public void ReturnsAfterTimeout()
        {
            var node = new InfinteMockIMinimaxNode();
            var runner = new AlphaBetaPruningParallel<MockIGameMove>();
            runner.Run(node, int.MaxValue, true, 1, 0);
            var checkedNodes = runner.EndNodesChecked;

            Assert.IsTrue(checkedNodes < int.MaxValue, "The timeout should occure before the max number of nodes are checked.");
        }

        [TestMethod]
        public void SeemToBeRunningParallel()
        {
            var nodes = new FiniteMockIMinimaxNode(12, 5);
            var runner = new AlphaBetaPruningParallel<MockIGameMove>();
            runner.Run(nodes, int.MaxValue, true, 500, 0);
            var checkedNodes = runner.EndNodesChecked;

            var nodes2 = new FiniteMockIMinimaxNode(12, 5);
            var runnerParallel = new AlphaBetaPruningParallel<MockIGameMove>();
            runnerParallel.Run(nodes2, int.MaxValue, true, 500, 4);
            var checkedNodesParallel = runnerParallel.EndNodesChecked;

            var msg = "Because of a quirk in FiniteMockIMinimaxNode, a threaded operation should return more checked nodes. Even after the fact that the count may be off.";
            Assert.IsTrue(checkedNodesParallel > checkedNodes, msg);
        }
    }

}
