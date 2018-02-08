using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaghChal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaghChal.Tests
{
    [TestClass()]
    public class GameMoveTests
    {
        [TestMethod()]
        public void TryMoveTest()
        {
            // TODO: Initiate a game board for testing. Also add a good test function.
            // Test out of bound start position.
            // Test out of bound target position.
            // Test move from empty location.
            // Test move from location with incorrect piece.
            // Test move out of turn.
            // Test move to location occopied by goat.
            // Test move to location occopied by tiger.
            // Test move to location to far away.
            // Test jump where target is occopied.
            // Test jump where there is nothing to jump over.
            // Test jump with goat.
            // Test jump of tiger.
            // Test move goat when all are not placed.

            // Test if tiger has valid move.
            // Test if board position has occured before.
            // Test undo move.
            Assert.Fail();
        }

        [TestMethod()]
        public void TranslateToBoardIndexTest()
        {
            Assert.AreEqual<int>(8, GameMove.TranslateToBoardIndex((1, 1)));
            Assert.AreEqual<int>(12, GameMove.TranslateToBoardIndex((5, 1)));
            Assert.AreEqual<int>(36, GameMove.TranslateToBoardIndex((1, 5)));
            Assert.AreEqual<int>(40, GameMove.TranslateToBoardIndex((5, 5)));
            Assert.AreEqual<int>(24, GameMove.TranslateToBoardIndex((3, 3)));
            Assert.AreEqual<int>(0, GameMove.TranslateToBoardIndex((0, 0)));
            Assert.AreEqual<int>(-8, GameMove.TranslateToBoardIndex((-1, -1)));
        }
    }
}