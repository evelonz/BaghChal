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
        private long[] Board { get; set; }

        /// <summary>
        /// The player who may move next. Tiger starts.
        /// </summary>
        public Pieces CurrentUsersTurn { get; private set; } = Pieces.Tiger;

        /// <summary>
        /// Set up a default game with four tigers in the corner.
        /// </summary>
        public GameBoard() : this((Pieces.Tiger, (1, 1)), (Pieces.Tiger, (5, 1)), (Pieces.Tiger, (1, 5)), (Pieces.Tiger, (5, 5)))
        {
        }

        /// <summary>
        /// Load a preset board with units in the given positions.
        /// Does not include history, so will be unable to undo moves or 
        /// fully determain of a board state has occured earlier.
        /// </summary>
        /// <param name="args"></param>
        public GameBoard(params (Pieces pieceType, (int x, int y) position)[] args)
        {
            Board = new long[3];

            foreach (var piece in args)
            {
                var index = GameMove.TranslateToBoardIndex(piece.position);
                PlacePeiceAtIndex(piece.pieceType, index);
            }
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

            long shiftMe = 1L;
            long shift = (shiftMe << index);
            Board[(int)piece] = Board[(int)piece] | shift;
            Board[(int)Pieces.Any] = Board[(int)Pieces.Any] | shift;
        }

        public bool IsPieceAtIndex(Pieces type, int index)
        {
            long shiftMe = 1L;
            return ((shiftMe << index) & Board[(int)type]) != 0L;
        }

        public Pieces GetPieceAtIndex(int index)
        {
            long shiftMe = 1L;
            if (((shiftMe << index) & Board[(int)Pieces.Any]) != 0L) {
                if (((shiftMe << index) & Board[(int)Pieces.Tiger]) != 0L)
                    return Pieces.Tiger;
                else
                    return Pieces.Goat;
            }
            else
                return Pieces.Empty;
        }

        public MoveResult Move(Pieces piece, (int x, int y) start, (int x, int y) end)
        {
            if (piece != CurrentUsersTurn)
                return MoveResult.NotPlayersTurn;
            if (IsOutOfBounds(start) || IsOutOfBounds(end))
                return MoveResult.OutOfBounds;
            var startIndex = TranslateToBoardIndex(start);
            if (piece != GetPieceAtIndex(startIndex))
                return MoveResult.TryToMoveIncorrectPiece;
            var endIndex = TranslateToBoardIndex(end);
            if (IsPieceAtIndex(Pieces.Any, endIndex))
                return MoveResult.TargetLocationOccupied;
            var moveType = PositionsAreLinked(startIndex, endIndex);
            if (moveType == MoveType.OutOfReach)
                return MoveResult.TargetLocationOutOfReach;
            // TODO: check that goat is between tiger and location.
            if (moveType == MoveType.Jump && piece == Pieces.Tiger && false)
                return MoveResult.TargetLocationOutOfReach;

            return MoveResult.MoveOK;
        }

        public static MoveType PositionsAreLinked(int start, int end)
        {
            // Skipping check of Out of bounds here. Assume it's done.
            // Are positions up, down, left, and right are connected.
            // Assume positive indexes here.
            var dx = Math.Abs(start - end);
            if (dx == 1 || dx == 7)
                return MoveType.Linked;
            // A jump is simply a move of two steps. Though only tigers can do it and there must be a goat between.
            // Though this code only checks if it's a possible move from a board perspective.
            else if (dx == 2 || dx == 14)
                return MoveType.Jump;
            // There are also diagonal lines going going from even coordinates (-2, 0, 2, 4) down right
            // and the same down left (2, 4, 6, 8)
            // First are multiples of 8. Second case are multiples of 6.
            // Since this can occure on other lines as well, we have to check that one position is on a line.
            // All diagonal lines are on even squares.
            // Fun note. % is not modulus, but reminder, in C#.
            var even = (start % 2) == 0;
            if (even && (dx == 6 || dx == 8))
                return MoveType.Linked;
            // Diagonal jump.
            else if (even && (dx == 12 || dx == 16))
                return MoveType.Jump;
            else
                return MoveType.OutOfReach;
        }

        /// <summary>
        /// Check that a jump move is valued.
        /// Can only be done by a tiger.
        /// Must be done over a goat, into a empty space.
        /// </summary>
        private bool IsJumpValid(Pieces piece, int start, int end)
        {
            // TODO: Check if jump to self? Check if target empty? Currently assumes these things.
            if (piece != Pieces.Tiger)
                return false;
            if (IsPieceAtIndex(Pieces.Any, end))
                return false;

            var middleIndex = ((start - end) / 2) + end;
            if (IsPieceAtIndex(Pieces.Goat, middleIndex))
                return true;
            else
                return false;
        }
        
        /// <summary>
        /// Translate a 1 indexed x, y coordinate into a index position on the 
        /// internal board storage.
        /// </summary>
        private static int TranslateToBoardIndex((int x, int y) position)
        {
            // TODO: Should perhaps check fo overflow, in case the IsOutOfBounds check uses index instead of position.
            return position.x + (position.y * 7);
        }

        private bool IsOutOfBounds((int x, int y) position)
        {
            // TODO: Is it better to check using index instead?
            return position.x < 1 || position.y < 1 ||
                position.x > 5 || position.y > 5;
        }
    }

    public enum MoveResult : int
    {
        MoveOK = 1,
        OutOfBounds = 2,
        TryToMoveIncorrectPiece = 3,
        TargetLocationOccupied = 4,
        TargetLocationOutOfReach = 5,
        StartPositionEmpty = 6,
        NotPlayersTurn = 7,
    }

    public enum MoveType : int
    {
        Linked = 1,
        Jump = 2,
        OutOfReach = 3,
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

        public bool TryMove(Pieces piece, (int x, int y) startPosition, (int x, int y) endPosition, GameBoard board)
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

        private bool IsPieceAtLocation(Pieces piece, (int x, int y) position, GameBoard board)
        {
            return board.IsPieceAtIndex(piece, TranslateToBoardIndex(position));
        }

        private bool IslocationEmpty((int x, int y) position, GameBoard board)
        {
            return !IsPieceAtLocation(Pieces.Any, position, board);
        }

        public bool IsOutOfBounds((int x, int y) position)
        {
            return position.x < 0 || position.y < 0; // TODO: Implement logic.
        }

        /// <summary>
        /// Translate a <see cref="BoardPosition"/> into a index position on 
        /// the internal board storage.
        /// </summary>
        public static int TranslateToBoardIndex((int x, int y) position)
        {
            return position.x + (position.y * 7);
        }
    }
}
