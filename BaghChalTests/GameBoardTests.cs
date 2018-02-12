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
            Assert.AreEqual(GameBoard.PositionsAreLinked(8, 8), MoveType.OutOfReach, "Able to move to current position.");
            // Cross.
            Assert.AreEqual(GameBoard.PositionsAreLinked(16, 15), MoveType.Linked, "Unable to move left.");
            Assert.AreEqual(GameBoard.PositionsAreLinked(16, 17), MoveType.Linked, "Unable to move right.");
            Assert.AreEqual(GameBoard.PositionsAreLinked(16, 9), MoveType.Linked, "Unable to move up.");
            Assert.AreEqual(GameBoard.PositionsAreLinked(16, 23), MoveType.Linked, "Unable to move down.");
            // Diagonals.
            Assert.AreEqual(GameBoard.PositionsAreLinked(16, 8), MoveType.Linked, "Unable to move up left.");
            Assert.AreEqual(GameBoard.PositionsAreLinked(16, 10), MoveType.Linked, "Unable to move up right.");
            Assert.AreEqual(GameBoard.PositionsAreLinked(16, 22), MoveType.Linked, "Unable to move down left.");
            Assert.AreEqual(GameBoard.PositionsAreLinked(16, 24), MoveType.Linked, "Unable to move down right.");
            // Cross jumps.
            Assert.AreEqual(GameBoard.PositionsAreLinked(8, 22), MoveType.Jump, "Unable to jump down.");
            Assert.AreEqual(GameBoard.PositionsAreLinked(38, 40), MoveType.Jump, "Unable to jump right.");
            Assert.AreEqual(GameBoard.PositionsAreLinked(10, 8), MoveType.Jump, "Unable to jump left.");
            Assert.AreEqual(GameBoard.PositionsAreLinked(25, 11), MoveType.Jump, "Unable to jump up.");
            // Diagonal jumps.
            Assert.AreEqual(GameBoard.PositionsAreLinked(24, 8), MoveType.Jump, "Unable to jump up left.");
            Assert.AreEqual(GameBoard.PositionsAreLinked(24, 12), MoveType.Jump, "Unable to jump up right.");
            Assert.AreEqual(GameBoard.PositionsAreLinked(24, 36), MoveType.Jump, "Unable to jump down left.");
            Assert.AreEqual(GameBoard.PositionsAreLinked(24, 40), MoveType.Jump, "Unable to jump down right.");
            // Incorrect diagonals.
            Assert.AreEqual(GameBoard.PositionsAreLinked(17, 9), MoveType.OutOfReach, "Invalid diagonal move up left.");
            Assert.AreEqual(GameBoard.PositionsAreLinked(17, 11), MoveType.OutOfReach, "Invalid diagonal move up right.");
            Assert.AreEqual(GameBoard.PositionsAreLinked(17, 23), MoveType.OutOfReach, "Invalid diagonal move down left.");
            Assert.AreEqual(GameBoard.PositionsAreLinked(17, 25), MoveType.OutOfReach, "Invalid diagonal move down right");
            // Out of reach.
            Assert.AreEqual(GameBoard.PositionsAreLinked(24, 29), MoveType.OutOfReach, "Out of reach move 24 to 29.");
            Assert.AreEqual(GameBoard.PositionsAreLinked(40, 8), MoveType.OutOfReach, "Out of reach move 40 to 8.");
            Assert.AreEqual(GameBoard.PositionsAreLinked(17, 38), MoveType.OutOfReach, "Out of reach move 17 to 38.");
            Assert.AreEqual(GameBoard.PositionsAreLinked(11, 26), MoveType.OutOfReach, "Out of reach move 11 to 26.");
        }

        [TestMethod()]
        public void MoveTest()
        {
            // TODO: Initiate a game board for testing. Also add a good test function.
            var board = new GameBoard();
            // TODO: Copy board state so we can reuse it for later tests.
            var boardCopy = board.Clone();

            // TODO: Set up a good start board for move test. Don't use one dependent on previous moves.
            var result = board.Move(Pieces.Goat, (0, 0), (3, 1));
            Assert.AreEqual(result, MoveResult.MoveOK, "Unable to placing first goat at 3,1.");
            result = board.Move(Pieces.Goat, (0, 0), (3, 1));
            Assert.AreEqual(result, MoveResult.NotPlayersTurn, "Goat able to move out of turn.");
            result = board.Move(Pieces.Tiger, (1, 1), (2, 1));
            Assert.AreEqual(result, MoveResult.MoveOK, "Tiger unable to move from 1,1 to 2,1.");
            result = board.Move(Pieces.Tiger, (1, 1), (2, 1));
            Assert.AreEqual(result, MoveResult.NotPlayersTurn, "Tiger able to move out of turn.");
            result = board.Move(Pieces.Goat, (0, 0), (1, 3));
            Assert.AreEqual(result, MoveResult.MoveOK, "Unable to placing second goat at 1,3.");
            result = board.Move(Pieces.Tiger, (2, 1), (4, 1));
            Assert.AreEqual(result, MoveResult.GoatCaptured, "Tiger capture goat at 3,1.");
            result = board.Move(Pieces.Goat, (0, 0), (5, 3));
            Assert.AreEqual(result, MoveResult.MoveOK, "Unable to placing third goat at 5,3.");
            result = board.Move(Pieces.Tiger, (5, 3), (5, 4));
            Assert.AreEqual(result, MoveResult.TryToMoveIncorrectPiece, "Tiger able to move goat at 5,3.");
            result = board.Move(Pieces.Tiger, (5, 1), (4, 1));
            Assert.AreEqual(result, MoveResult.TargetLocationOccupied, "Tiger able to stack at 4,1.");
            result = board.Move(Pieces.Tiger, (-1, 1), (4, 1));
            Assert.AreEqual(result, MoveResult.OutOfBounds, "Able to select Tiger out of bounds at -1,1.");
            result = board.Move(Pieces.Tiger, (4, 1), (4, -1));
            Assert.AreEqual(result, MoveResult.OutOfBounds, "Able to move Tiger at 4,1 out of bounds.");
            result = board.Move(Pieces.Tiger, (4, 1), (2, 1));
            Assert.AreEqual(result, MoveResult.TargetLocationOutOfReach, "Tiger able to jump over empty space.");
            result = board.Move(Pieces.Tiger, (1, 1), (2, 1));
            Assert.AreEqual(result, MoveResult.TryToMoveIncorrectPiece, "Able to move tiger from empty position 1,1.");
            result = board.Move(Pieces.Tiger, (1, 1), (2, 1));
            Assert.AreEqual(result, MoveResult.TryToMoveIncorrectPiece, "Able to move tiger from empty position 1,1.");

            // TODO: Implement all the game move checks.
            // Test move to location occopied by goat.
            // Test move to location occopied by tiger.
            // Test move to location to far away.
            // Test jump where target is occopied.
            // Test jump where there is nothing to jump over.
            // Test jump with goat.
            // Test move goat when all are not placed.

            // Test if tiger has valid move.
            // Test if board position has occured before.
            // Test undo move.
            //Assert.Fail();
        }
    }
}