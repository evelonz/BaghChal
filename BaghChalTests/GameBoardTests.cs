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
            Assert.AreEqual(MoveType.OutOfReach, GameBoard.PositionsAreLinked(8, 8), "Able to move to current position.");
            // Cross.
            Assert.AreEqual(MoveType.Linked, GameBoard.PositionsAreLinked(16, 15), "Unable to move left.");
            Assert.AreEqual(MoveType.Linked, GameBoard.PositionsAreLinked(16, 17), "Unable to move right.");
            Assert.AreEqual(MoveType.Linked, GameBoard.PositionsAreLinked(16, 9),  "Unable to move up.");
            Assert.AreEqual(MoveType.Linked, GameBoard.PositionsAreLinked(16, 23), "Unable to move down.");
            // Diagonals.
            Assert.AreEqual(MoveType.Linked, GameBoard.PositionsAreLinked(16, 8),  "Unable to move up left.");
            Assert.AreEqual(MoveType.Linked, GameBoard.PositionsAreLinked(16, 10), "Unable to move up right.");
            Assert.AreEqual(MoveType.Linked, GameBoard.PositionsAreLinked(16, 22), "Unable to move down left.");
            Assert.AreEqual(MoveType.Linked, GameBoard.PositionsAreLinked(16, 24), "Unable to move down right.");
            // Cross jumps.
            Assert.AreEqual(MoveType.Jump, GameBoard.PositionsAreLinked(8, 22),  "Unable to jump down.");
            Assert.AreEqual(MoveType.Jump, GameBoard.PositionsAreLinked(38, 40), "Unable to jump right.");
            Assert.AreEqual(MoveType.Jump, GameBoard.PositionsAreLinked(10, 8),  "Unable to jump left.");
            Assert.AreEqual(MoveType.Jump, GameBoard.PositionsAreLinked(25, 11), "Unable to jump up.");
            // Diagonal jumps.
            Assert.AreEqual(MoveType.Jump, GameBoard.PositionsAreLinked(24, 8),  "Unable to jump up left.");
            Assert.AreEqual(MoveType.Jump, GameBoard.PositionsAreLinked(24, 12), "Unable to jump up right.");
            Assert.AreEqual(MoveType.Jump, GameBoard.PositionsAreLinked(24, 36), "Unable to jump down left.");
            Assert.AreEqual(MoveType.Jump, GameBoard.PositionsAreLinked(24, 40), "Unable to jump down right.");
            // Incorrect diagonals.
            Assert.AreEqual(MoveType.OutOfReach, GameBoard.PositionsAreLinked(17, 9),  "Invalid diagonal move up left.");
            Assert.AreEqual(MoveType.OutOfReach, GameBoard.PositionsAreLinked(17, 11), "Invalid diagonal move up right.");
            Assert.AreEqual(MoveType.OutOfReach, GameBoard.PositionsAreLinked(17, 23), "Invalid diagonal move down left.");
            Assert.AreEqual(MoveType.OutOfReach, GameBoard.PositionsAreLinked(17, 25), "Invalid diagonal move down right");
            // Out of reach.
            Assert.AreEqual(MoveType.OutOfReach, GameBoard.PositionsAreLinked(24, 29), "Out of reach move 24 to 29.");
            Assert.AreEqual(MoveType.OutOfReach, GameBoard.PositionsAreLinked(40, 8),  "Out of reach move 40 to 8.");
            Assert.AreEqual(MoveType.OutOfReach, GameBoard.PositionsAreLinked(17, 38), "Out of reach move 17 to 38.");
            Assert.AreEqual(MoveType.OutOfReach, GameBoard.PositionsAreLinked(11, 26), "Out of reach move 11 to 26.");
        }

        [TestMethod()]
        public void MoveTest()
        {
            // Test that don't effect board, in order (goat first then tiger).
            // Set board for testing illigal moves during placement.
            //T---- 
            //GT---
            //GGG--
            //TT---
            //-----
            var board = new GameBoard(8, 16, 0, Pieces.Goat,
                (Pieces.Tiger, (1, 1)), (Pieces.Tiger, (2, 2)), (Pieces.Tiger, (1, 4)), (Pieces.Tiger, (2, 4)),
                (Pieces.Goat, (1, 2)), (Pieces.Goat, (1, 3)), (Pieces.Goat, (2, 3)), (Pieces.Goat, (3, 3))
                );

            // Out of turn move.
            var result = board.Move(Pieces.Tiger, (1, 1), (2, 1));
            Assert.AreEqual(MoveResult.NotPlayersTurn, result, "Tiger able to move out of turn.");
            // Move goat during placement.
            result = board.Move(Pieces.Goat, (3, 3), (3, 4));
            Assert.AreEqual(MoveResult.GoatMoveDuringPlacement, result, "Goat able to move during placement.");
            // Place goat in incorrect space.
            result = board.Move(Pieces.Goat, (0, 0), (-3, 4));
            Assert.AreEqual(MoveResult.OutOfBounds, result, "Goat able to be placed out of bounds.");
            result = board.Move(Pieces.Goat, (0, 0), (1, 3));
            Assert.AreEqual(MoveResult.TargetLocationOccupied, result, "Goat able to be placed on other goat.");
            result = board.Move(Pieces.Goat, (0, 0), (1, 4));
            Assert.AreEqual(MoveResult.TargetLocationOccupied, result, "Goat able to be placed on tiger.");
            // Move incorrect piece.
            result = board.Move(Pieces.Goat, (1, 1), (2, 1));
            Assert.AreEqual(MoveResult.TryToMoveIncorrectPiece, result, "Goat able to move tiger.");
            result = board.Move(Pieces.Goat, (2, 1), (2, 2));
            Assert.AreEqual(MoveResult.TryToMoveIncorrectPiece, result, "Goat able to move empty space.");
            result = board.Move(Pieces.Goat, (2, -1), (2, -2));
            Assert.AreEqual(MoveResult.OutOfBounds, result, "Goat able to move empty space out of bounds.");

            // Set board to tigers turn.
            board = new GameBoard(7, 16, 0, Pieces.Tiger,
                (Pieces.Tiger, (1, 1)), (Pieces.Tiger, (2, 2)), (Pieces.Tiger, (1, 4)), (Pieces.Tiger, (2, 4)),
                (Pieces.Goat, (1, 2)), (Pieces.Goat, (1, 3)), (Pieces.Goat, (2, 3)), (Pieces.Goat, (3, 3))
                );
            // Move incorrect piece.
            result = board.Move(Pieces.Tiger, (3, 3), (3, 4));
            Assert.AreEqual(MoveResult.TryToMoveIncorrectPiece, result, "Tiger able to move Goat.");
            result = board.Move(Pieces.Tiger, (2, 1), (2, 2));
            Assert.AreEqual(MoveResult.TryToMoveIncorrectPiece, result, "Tiger able to move empty space.");
            result = board.Move(Pieces.Tiger, (2, -1), (2, -2));
            Assert.AreEqual(MoveResult.OutOfBounds, result, "Tiger able to move empty space out of bounds.");

            // Move out of bounds.
            result = board.Move(Pieces.Tiger, (1, 1), (1, 0));
            Assert.AreEqual(MoveResult.OutOfBounds, result, "Tiger able to move out of bounds.");
            // Jump over incorrect piece.
            result = board.Move(Pieces.Tiger, (1, 4), (3, 4));
            Assert.AreEqual(MoveResult.InvalidJump, result, "Tiger able to jump tiger.");
            result = board.Move(Pieces.Tiger, (1, 1), (3, 1));
            Assert.AreEqual(MoveResult.InvalidJump, result, "Tiger able to jump over empty space.");
            // Jump to incorrect piece.
            result = board.Move(Pieces.Tiger, (2, 2), (2, 4));
            Assert.AreEqual(MoveResult.TargetLocationOccupied, result, "Tiger able to jump over goat to tiger.");
            result = board.Move(Pieces.Tiger, (2, 2), (0, 2));
            Assert.AreEqual(MoveResult.OutOfBounds, result, "Tiger able to jump over goat to out of bounds.");
            result = board.Move(Pieces.Tiger, (1, 1), (1, 3));
            Assert.AreEqual(MoveResult.TargetLocationOccupied, result, "Tiger able to jump over goat to goat.");
            // Move out of reach.
            result = board.Move(Pieces.Tiger, (1, 4), (2, 5));
            Assert.AreEqual(MoveResult.TargetLocationOutOfReach, result, "Tiger able to move where link is missing.");
            //result = board.Move(Pieces.Tiger, (1, 1), (1, 3));
            //Assert.AreEqual(MoveResult.OutOfBounds, result, "Tiger able to jump where link is missing.");
            result = board.Move(Pieces.Tiger, (2, 4), (5, 4));
            Assert.AreEqual(MoveResult.TargetLocationOutOfReach, result, "Tiger able to jump too long.");
            // Move to current place.
            result = board.Move(Pieces.Tiger, (1, 1), (1, 1));
            Assert.AreEqual(MoveResult.TargetLocationOccupied, result, "Tiger able to move in place.");

            // Set board to goats move turn.
            board = new GameBoard(7, 0, 3, Pieces.Goat,
                (Pieces.Tiger, (1, 1)), (Pieces.Tiger, (2, 2)), (Pieces.Tiger, (1, 4)), (Pieces.Tiger, (2, 4)),
                (Pieces.Goat, (1, 2)), (Pieces.Goat, (1, 3)), (Pieces.Goat, (2, 3)), (Pieces.Goat, (3, 3))
                );
            // Move out of bounds.
            result = board.Move(Pieces.Goat, (1, 2), (0, 2));
            Assert.AreEqual(MoveResult.OutOfBounds, result, "Goat able to move out of bounds.");
            // Jump over incorrect piece.
            result = board.Move(Pieces.Goat, (1, 2), (3, 2));
            Assert.AreEqual(MoveResult.InvalidJump, result, "Goat able to jump tiger.");
            result = board.Move(Pieces.Goat, (3, 3), (5, 3));
            Assert.AreEqual(MoveResult.InvalidJump, result, "Goat able to jump over empty space.");
            result = board.Move(Pieces.Goat, (2, 3), (4, 3));
            Assert.AreEqual(MoveResult.InvalidJump, result, "Goat able to jump over goat.");
            // Jump to incorrect piece.
            result = board.Move(Pieces.Goat, (1, 3), (1, 1));
            Assert.AreEqual(MoveResult.TargetLocationOccupied, result, "Goat able to jump over goat to tiger.");
            result = board.Move(Pieces.Goat, (2, 3), (0, 3));
            Assert.AreEqual(MoveResult.OutOfBounds, result, "Goat able to jump over goat to out of bounds.");
            result = board.Move(Pieces.Goat, (1, 3), (3, 3));
            Assert.AreEqual(MoveResult.TargetLocationOccupied, result, "Goat able to jump over goat to goat.");
            // Move out of reach.
            result = board.Move(Pieces.Goat, (2, 3), (3, 4));
            Assert.AreEqual(MoveResult.TargetLocationOutOfReach, result, "Goat able to move where link is missing.");
            //result = board.Move(Pieces.Goat, (2, 3), (1, 3));
            //Assert.AreEqual(MoveResult.OutOfBounds, result, "Goat able to jump where link is missing.");
            result = board.Move(Pieces.Goat, (2, 3), (4, 3));
            Assert.AreEqual(MoveResult.InvalidJump, result, "Goat able to jump too long.");

            // Test valid moves.
            result = board.Move(Pieces.Goat, (3, 3), (4, 3));
            Assert.AreEqual(MoveResult.MoveOK, result, "Goat unable to move one space.");
            result = board.Move(Pieces.Tiger, (1, 1), (2, 1));
            Assert.AreEqual(MoveResult.MoveOK, result, "Tiger unable to move one space.");
            result = board.Move(Pieces.Goat, (2, 3), (3, 3));
            Assert.AreEqual(MoveResult.MoveOK, result, "Goat unable to move one space.");
            result = board.Move(Pieces.Tiger, (2, 2), (4, 4));
            Assert.AreEqual(MoveResult.GoatCaptured, result, "Tiger unable to capture goat.");
            result = board.Move(Pieces.Goat, (1, 3), (2, 3));
            Assert.AreEqual(MoveResult.MoveOK, result, "Goat unable to move one space.");
            result = board.Move(Pieces.Tiger, (2, 4), (2, 2));
            Assert.AreEqual(MoveResult.TigerWin, result, "Tiger unable to win.");

            // Set board up for goat win.
            //TTGG- 
            //TTG-G
            //GGG--
            //GG-G-
            //-----
            // Check if goat victory is found.
            board = new GameBoard(46, 0, 4, Pieces.Goat,
                (Pieces.Tiger, (1, 1)), (Pieces.Tiger, (1, 2)), (Pieces.Tiger, (2, 1)), (Pieces.Tiger, (2, 2)),
                (Pieces.Goat, (3, 1)), (Pieces.Goat, (4, 1)), (Pieces.Goat, (3, 2)), (Pieces.Goat, (5, 2)),
                (Pieces.Goat, (1, 3)), (Pieces.Goat, (2, 3)), (Pieces.Goat, (1, 4)), (Pieces.Goat, (2, 4)),
                (Pieces.Goat, (3, 3)), (Pieces.Goat, (4, 4))
                );
            result = board.Move(Pieces.Goat, (5, 2), (4, 2));
            Assert.AreEqual(MoveResult.GoatWin, result, "Goat unable to win.");

            // Test if Tiger finds jump to escape.
            board = new GameBoard(46, 0, 4, Pieces.Goat,
                (Pieces.Tiger, (1, 1)), (Pieces.Tiger, (1, 2)), (Pieces.Tiger, (2, 1)), (Pieces.Tiger, (2, 2)),
                (Pieces.Goat, (3, 1)), (Pieces.Goat, (4, 1)), (Pieces.Goat, (3, 2)), (Pieces.Goat, (5, 2)),
                (Pieces.Goat, (1, 3)), (Pieces.Goat, (2, 3)), (Pieces.Goat, (1, 4)), (Pieces.Goat, (2, 4)),
                (Pieces.Goat, (3, 3)), (Pieces.Goat, (4, 4))
                );
            result = board.Move(Pieces.Goat, (5, 2), (5, 3));
            Assert.AreEqual(MoveResult.MoveOK, result, "Tiger unable to escape loss using jump.");


            // Test if board position has occured before.
            // Test undo move.
        }

        [TestMethod()]
        public void GetGoatMovesTest()
        {
            // Check goat placement.
            var board = new GameBoard(20, 10, 0, Pieces.Goat,
                (Pieces.Tiger, (1, 1)), (Pieces.Tiger, (1, 2)), (Pieces.Tiger, (2, 1)), (Pieces.Tiger, (2, 2)),
                (Pieces.Goat, (3, 1)), (Pieces.Goat, (4, 1)), (Pieces.Goat, (3, 2)), (Pieces.Goat, (5, 2)),
                (Pieces.Goat, (1, 3)), (Pieces.Goat, (2, 3)), (Pieces.Goat, (1, 4)), (Pieces.Goat, (2, 4)),
                (Pieces.Goat, (3, 3)), (Pieces.Goat, (4, 4))
                );
            var moves = board.GetGoatMoves();

            var moveResults = new List<(MoveResult result, (int x, int y) positions)>()
            {
                (MoveResult.MoveOK, (5, 1)),
                (MoveResult.MoveOK, (4, 2)),
                (MoveResult.MoveOK, (4, 3)),(MoveResult.MoveOK, (5, 3)),
                (MoveResult.MoveOK, (3, 4)),(MoveResult.MoveOK, (5, 4)),
                (MoveResult.MoveOK, (1, 5)),(MoveResult.MoveOK, (2, 5)),(MoveResult.MoveOK, (3, 5)),(MoveResult.MoveOK, (4, 5)),(MoveResult.MoveOK, (5, 5))
            };
            var res = Enumerable.SequenceEqual(moves[(0, 0)].OrderBy(t => t.positions), moveResults.OrderBy(t => t.positions));
            Assert.AreEqual(true, res, "Goat placement incorrect.");

            // Check single goat move.
            board = new GameBoard(20, 0, 4, Pieces.Goat,
                (Pieces.Goat, (3, 3))
                );
            moves = board.GetGoatMoves();

            moveResults = new List<(MoveResult result, (int x, int y) positions)>()
            {
                (MoveResult.MoveOK, (2, 2)),(MoveResult.MoveOK, (3, 2)),(MoveResult.MoveOK, (4, 2)),
                (MoveResult.MoveOK, (2, 3)),                            (MoveResult.MoveOK, (4, 3)),
                (MoveResult.MoveOK, (2, 4)),(MoveResult.MoveOK, (3, 4)),(MoveResult.MoveOK, (4, 4)),
            };
            res= Enumerable.SequenceEqual(moves[(3, 3)].OrderBy(t => t.positions), moveResults.OrderBy(t => t.positions));
            Assert.AreEqual(true, res, "Goat moves incorrect from center.");

            // Check goat next to border, tigers, and other goats.
            board = new GameBoard(40, 0, 0, Pieces.Goat,
                (Pieces.Tiger, (2, 1)), (Pieces.Goat, (3, 1)), (Pieces.Goat, (4, 1))
            );
            moves = board.GetGoatMoves();

            moveResults = new List<(MoveResult result, (int x, int y) positions)>()
            {
                (MoveResult.OutOfBounds, (2, 0)),            (MoveResult.OutOfBounds, (3, 0)), (MoveResult.OutOfBounds, (4, 0)),
                (MoveResult.TargetLocationOccupied, (2, 1)),                                   (MoveResult.TargetLocationOccupied, (4, 1)),
                (MoveResult.MoveOK, (2, 2)),                 (MoveResult.MoveOK, (3, 2)),      (MoveResult.MoveOK, (4, 2)),
            };
            res = Enumerable.SequenceEqual(moves[(3, 1)].OrderBy(t => t.positions), moveResults.OrderBy(t => t.positions));
            Assert.AreEqual(true, res, "Goat able to move to incorrect location.");

        }
    }
}