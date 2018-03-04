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
        public void TranslateToBoardIndexTest()
        {
            Assert.AreEqual<int>(8,  GameBoard.TranslateToBoardIndex((1, 1)));
            Assert.AreEqual<int>(12, GameBoard.TranslateToBoardIndex((5, 1)));
            Assert.AreEqual<int>(36, GameBoard.TranslateToBoardIndex((1, 5)));
            Assert.AreEqual<int>(40, GameBoard.TranslateToBoardIndex((5, 5)));
            Assert.AreEqual<int>(24, GameBoard.TranslateToBoardIndex((3, 3)));
            Assert.AreEqual<int>(0,  GameBoard.TranslateToBoardIndex((0, 0)));
            Assert.AreEqual<int>(-8, GameBoard.TranslateToBoardIndex((-1, -1)));
        }
    }
}