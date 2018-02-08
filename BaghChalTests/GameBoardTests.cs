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
    public class GameBoardTests
    {
        [TestMethod()]
        public void PositionsAreLinkedTest()
        {
            // Same place.
            Assert.AreEqual(GameBoard.PositionsAreLinked(8, 8), MoveType.OutOfReach, "0");
            // Cross.
            Assert.AreEqual(GameBoard.PositionsAreLinked(16, 15), MoveType.Linked, "1");
            Assert.AreEqual(GameBoard.PositionsAreLinked(16, 17), MoveType.Linked, "2");
            Assert.AreEqual(GameBoard.PositionsAreLinked(16, 9), MoveType.Linked, "3");
            Assert.AreEqual(GameBoard.PositionsAreLinked(16, 23), MoveType.Linked, "4");
            // Diagonals.
            Assert.AreEqual(GameBoard.PositionsAreLinked(16, 8), MoveType.Linked, "5");
            Assert.AreEqual(GameBoard.PositionsAreLinked(16, 10), MoveType.Linked, "6");
            Assert.AreEqual(GameBoard.PositionsAreLinked(16, 22), MoveType.Linked, "7");
            Assert.AreEqual(GameBoard.PositionsAreLinked(16, 24), MoveType.Linked, "8");
            // Cross jumps.
            Assert.AreEqual(GameBoard.PositionsAreLinked(8, 22), MoveType.Jump, "9");
            Assert.AreEqual(GameBoard.PositionsAreLinked(38, 40), MoveType.Jump, "10");
            Assert.AreEqual(GameBoard.PositionsAreLinked(10, 8), MoveType.Jump, "11");
            Assert.AreEqual(GameBoard.PositionsAreLinked(25, 11), MoveType.Jump, "12");
            // Diagonal jumps.
            Assert.AreEqual(GameBoard.PositionsAreLinked(24, 8), MoveType.Jump, "13");
            Assert.AreEqual(GameBoard.PositionsAreLinked(24, 12), MoveType.Jump, "14");
            Assert.AreEqual(GameBoard.PositionsAreLinked(24, 36), MoveType.Jump, "15");
            Assert.AreEqual(GameBoard.PositionsAreLinked(24, 40), MoveType.Jump, "16");
            // Incorrect diagonals.
            Assert.AreEqual(GameBoard.PositionsAreLinked(17, 9), MoveType.OutOfReach, "17");
            Assert.AreEqual(GameBoard.PositionsAreLinked(17, 11), MoveType.OutOfReach, "18");
            Assert.AreEqual(GameBoard.PositionsAreLinked(17, 23), MoveType.OutOfReach, "19");
            Assert.AreEqual(GameBoard.PositionsAreLinked(17, 25), MoveType.OutOfReach, "20");
            // Out of reach.
            Assert.AreEqual(GameBoard.PositionsAreLinked(24, 29), MoveType.OutOfReach, "21");
            Assert.AreEqual(GameBoard.PositionsAreLinked(40, 8), MoveType.OutOfReach, "22");
            Assert.AreEqual(GameBoard.PositionsAreLinked(17, 38), MoveType.OutOfReach, "23");
            Assert.AreEqual(GameBoard.PositionsAreLinked(11, 26), MoveType.OutOfReach, "24");
        }
    }
}