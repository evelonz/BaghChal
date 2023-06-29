using BaghChal;
using FluentAssertions;
using Xunit;

namespace XUnitTestProjectBaghChal
{
    public class GameBoardTests
    {
        [Theory]
        // Move in same place
        [InlineData(8, 8, MoveType.OutOfReach, "we should not be able to move to current position")]
        // Cross
        [InlineData(16, 15, MoveType.Linked, "we should be able to move left")]
        [InlineData(16, 17, MoveType.Linked, "we should be able to move right")]
        [InlineData(16,  9, MoveType.Linked, "we should be able to move up")]
        [InlineData(16, 23, MoveType.Linked, "we should be able to move down")]
        // Diagonals
        public void PositionsAreLinkedTest(int startIndex, int endIndex, MoveType expected, string testDescription)
        {
            var moveResult = GameBoard.PositionsAreLinked(startIndex, endIndex);
            moveResult.Should().Be(expected, because: testDescription);
            
            //// Diagonals.
            //Assert.AreEqual(MoveType.Linked, GameBoard.PositionsAreLinked(16, 8), "Unable to move up left.");
            //Assert.AreEqual(MoveType.Linked, GameBoard.PositionsAreLinked(16, 10), "Unable to move up right.");
            //Assert.AreEqual(MoveType.Linked, GameBoard.PositionsAreLinked(16, 22), "Unable to move down left.");
            //Assert.AreEqual(MoveType.Linked, GameBoard.PositionsAreLinked(16, 24), "Unable to move down right.");
            //// Cross jumps.
            //Assert.AreEqual(MoveType.Jump, GameBoard.PositionsAreLinked(8, 22), "Unable to jump down.");
            //Assert.AreEqual(MoveType.Jump, GameBoard.PositionsAreLinked(38, 40), "Unable to jump right.");
            //Assert.AreEqual(MoveType.Jump, GameBoard.PositionsAreLinked(10, 8), "Unable to jump left.");
            //Assert.AreEqual(MoveType.Jump, GameBoard.PositionsAreLinked(25, 11), "Unable to jump up.");
            //// Diagonal jumps.
            //Assert.AreEqual(MoveType.Jump, GameBoard.PositionsAreLinked(24, 8), "Unable to jump up left.");
            //Assert.AreEqual(MoveType.Jump, GameBoard.PositionsAreLinked(24, 12), "Unable to jump up right.");
            //Assert.AreEqual(MoveType.Jump, GameBoard.PositionsAreLinked(24, 36), "Unable to jump down left.");
            //Assert.AreEqual(MoveType.Jump, GameBoard.PositionsAreLinked(24, 40), "Unable to jump down right.");
            //// Incorrect diagonals.
            //Assert.AreEqual(MoveType.OutOfReach, GameBoard.PositionsAreLinked(17, 9), "Invalid diagonal move up left.");
            //Assert.AreEqual(MoveType.OutOfReach, GameBoard.PositionsAreLinked(17, 11), "Invalid diagonal move up right.");
            //Assert.AreEqual(MoveType.OutOfReach, GameBoard.PositionsAreLinked(17, 23), "Invalid diagonal move down left.");
            //Assert.AreEqual(MoveType.OutOfReach, GameBoard.PositionsAreLinked(17, 25), "Invalid diagonal move down right");
            //// Out of reach.
            //Assert.AreEqual(MoveType.OutOfReach, GameBoard.PositionsAreLinked(24, 29), "Out of reach move 24 to 29.");
            //Assert.AreEqual(MoveType.OutOfReach, GameBoard.PositionsAreLinked(40, 8), "Out of reach move 40 to 8.");
            //Assert.AreEqual(MoveType.OutOfReach, GameBoard.PositionsAreLinked(17, 38), "Out of reach move 17 to 38.");
            //Assert.AreEqual(MoveType.OutOfReach, GameBoard.PositionsAreLinked(11, 26), "Out of reach move 11 to 26.");
        }
    }
}
