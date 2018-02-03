using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaghChal
{
    public class BaghChalPiece
    {

    }

    public class Tiger : BaghChalPiece
    {

    }

    public class Goat : BaghChalPiece
    {

    }

    public enum Pieces : int
    {
        Tiger = 0,
        Goat = 1,
        Any = 2,
        Empty = 3
    }

    public class GameBoard
    {
        /// <summary>
        /// Holds the state of the board.
        /// First index is for tigers, goats, and both.
        /// Second index is for the bords position, starting at the top left.
        /// New rows wraps after seven positions. Outer rows are out of bounds.
        /// </summary>
        public long[] Board { get; set; }

        public GameBoard()
        {
            Board = new long[3];
        }

        /// <summary>
        /// For test. During game, only goats can be placed.
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="index"></param>
        public void PlacePeiceAtIndex(Pieces piece, int index)
        {
            if (piece != Pieces.Tiger && piece != Pieces.Goat)
                throw new Exception("Can only place tigers and goats on board.");
            if (IsPieceAtIndex(Pieces.Any, index))
                throw new Exception("Position already occupied.");

            long shiftMe = 0;
            Board[(int)piece] = Board[(int)piece] | (shiftMe << index);
        }

        public bool IsPieceAtIndex(Pieces type, int index)
        {
            long shiftMe = 0;
            return ((shiftMe << index) & Board[(int)type]) != 0;
        }
    }


    public class BoardPosition
    {
        public int X { get; set; }
        public int Y { get; set; }

        public BoardPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public enum InvalidGameMove : int
    {
        OutOfBounds = 1,
        TryToMoveIncorrectPiece = 2,
        TargetLocationOccupied = 3,
    }

    public class GameMoveException : Exception
    {
        public InvalidGameMove TypeOfViolation { get; set; }

        public GameMoveException(string message, InvalidGameMove type) : base(message)
        {
            TypeOfViolation = type;
        }
    }

    public class GameMove
    {

        public bool TryMove(Pieces piece, BoardPosition startPosition, BoardPosition endPosition, GameBoard board)
        {
            if(IsOutOfBounds(startPosition) || IsOutOfBounds(endPosition))
            {
                throw new GameMoveException("Start or end position is out of bounds.", InvalidGameMove.OutOfBounds);
            }
            else if (IsPieceAtLocation(piece, startPosition, board))
            {
                throw new GameMoveException("Selected piece is not at start position.", InvalidGameMove.TryToMoveIncorrectPiece);
            }
            else if (IslocationEmpty(endPosition, board))
            {
                throw new GameMoveException("Target location is not empty.", InvalidGameMove.TargetLocationOccupied);
            }
            else
                return false;
        }

        private bool IsPieceAtLocation(Pieces piece, BoardPosition position, GameBoard board)
        {
            return board.IsPieceAtIndex(piece, TranslateToBoardIndex(position));
        }

        private bool IslocationEmpty(BoardPosition position, GameBoard board)
        {
            return !IsPieceAtLocation(Pieces.Any, position, board);
        }

        public bool IsOutOfBounds(BoardPosition position)
        {
            return position.X < 0 || position.Y < 0; // TODO: Implement logic.
        }

        /// <summary>
        /// Translate a <see cref="BoardPosition"/> into a index position on 
        /// the internal board storage.
        /// </summary>
        private int TranslateToBoardIndex(BoardPosition position)
        {
            return position.X + (position.Y % 7);
        }
    }
}
