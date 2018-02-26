using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaghChal
{

    public enum Pieces : int
    {
        Tiger = 0,
        Goat = 1,
        Any = 2,
        Empty = 3
    }

    public class GameBoard : ICloneable
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
        public Pieces CurrentUsersTurn { get; private set; } = Pieces.Goat;

        /// <summary>
        /// Ply is a half turn in the game.
        /// One turn is a move by first goat then by tiger. So a ply is the move of one of them.
        /// </summary>
        public int Ply { get; private set; } = 0;

        public int GoatsLeftToPlace { get; private set; } = 20;

        public int GoatsCaptured { get; private set; } = 0;

        public object Clone()
        {
            return new GameBoard(Board, CurrentUsersTurn, Ply, GoatsLeftToPlace, GoatsCaptured);
        }

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types. For now allow child types.
            if (obj == null || !(obj is GameBoard))
                return false;

            var p = (GameBoard)obj;
            return (Board[0] == p.Board[0]) && (Board[1] == p.Board[1])
                && (GoatsCaptured == p.GoatsCaptured) && GoatsLeftToPlace == p.GoatsLeftToPlace
                && (Ply == p.Ply);
        }

        public override int GetHashCode()
        {
            // TODO: Not sure if this is a good hash function.
            return Board[0].GetHashCode() ^ Board[1].GetHashCode();
        }

        /// <summary>
        /// Set up a default game with four tigers in the corner.
        /// </summary>
        public GameBoard() : this(0, 20, 0, Pieces.Goat, (Pieces.Tiger, (1, 1)), (Pieces.Tiger, (5, 1)), (Pieces.Tiger, (1, 5)), (Pieces.Tiger, (5, 5)))
        {
        }

        private GameBoard(long[] board, Pieces currentUsersTurn, int ply, int goatsLeftToPlace, int goatsCaptured)
        {
            Board = new long[]
            {
                board[0],
                board[1],
                board[2]
            };
            CurrentUsersTurn = currentUsersTurn;
            Ply = ply;
            GoatsLeftToPlace = goatsLeftToPlace;
            GoatsCaptured = goatsCaptured;
        }

        /// <summary>
        /// Load a preset board with units in the given positions.
        /// Does not include history, so will be unable to undo moves or 
        /// fully determain of a board state has occured earlier.
        /// </summary>
        /// <param name="args"></param>
        public GameBoard(int play, int goatsLeftToPlace, int goatsCaptured, Pieces currentUsersTurn, params (Pieces pieceType, (int x, int y) position)[] args)
        {
            Ply = play;
            GoatsLeftToPlace = goatsLeftToPlace;
            GoatsCaptured = goatsCaptured;
            CurrentUsersTurn = currentUsersTurn;

            Board = new long[3];

            foreach (var (pieceType, position) in args)
            {
                var index = GameMove.TranslateToBoardIndex(position);
                PlacePeiceAtIndex(pieceType, index);
            }
        }

        public override string ToString()
        {
            char[] symbols = { 'T', 'G', '/', '-', };
            var sb = new StringBuilder();
            sb.AppendLine($"=== Ply: {Ply}, P: {CurrentUsersTurn}, GP: {GoatsLeftToPlace}, GC: {GoatsCaptured} ===");
            for (int row = 1; row < 6; row++)
            {
                for (int column = 1; column < 6; column++)
                {
                    var piece = GetPieceAtIndex(column + (row * 7));
                    sb.Append(symbols[(int)piece]);
                }
                sb.AppendLine();
            }
            sb.AppendLine("=====================================");
            return sb.ToString();
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

        private void Tick()
        {
            CurrentUsersTurn = (CurrentUsersTurn == Pieces.Goat) ? Pieces.Tiger : Pieces.Goat;
            Ply++;
        }

        public bool CheckGameEnd(Pieces piece, bool checkAfterMove = true)
        {
            switch (piece)
            {
                case Pieces.Tiger:
                    if(checkAfterMove)
                        return (GoatsCaptured == 5);
                    return !TigerAbleToMove();
                case Pieces.Goat:
                    if (checkAfterMove)
                        return !TigerAbleToMove();
                    return (GoatsCaptured == 5); 
                default:
                    return false;
            }
        }

        private bool TigerAbleToMove()
        {
            var tigerMoves = GetTigerMoves();
            foreach (var entry in tigerMoves)
            {
                var anyMove = entry.Value.Any(a => a.result == MoveResult.MoveOK || a.result == MoveResult.GoatCaptured);
                if (anyMove)
                    return true;
            }
            return false;
        }

        public Dictionary<(int x, int y), List<(MoveResult result, (int x, int y) positions)>> GetTigerMoves()
        {
            Dictionary<(int x, int y), List<(MoveResult result, (int x, int y) positions)>> res = new Dictionary<(int x, int y), List<(MoveResult result, (int x, int y) posistion)>>(4);
            for (int i = 8; i < 41; i++)
            {
                if ((Board[(int)Pieces.Tiger] & (1L << i)) > 0)
                {
                    List<(MoveResult result, (int x, int y) posistion)> returnValue = new List<(MoveResult result, (int x, int y) posistion)>();
                    var tigerPosition = TranslatetFromBoardIndex(i);
                    var linkedPositions = GetLinkedPositions(i);
                    for (int target = 0; target < linkedPositions.Length; target++)
                    {
                        var targetPosition = TranslatetFromBoardIndex(linkedPositions[target]);
                        var moveResult = TryMove(Pieces.Tiger, tigerPosition, targetPosition);
                        returnValue.Add((moveResult, targetPosition));
                    }
                    // TODO: Now have to check if there are any jump points. Wonder if a filter on goats is faster than just checking all jumps?
                    linkedPositions = GetJumpLinkedPositions(i);
                    for (int target = 0; target < linkedPositions.Length; target++)
                    {
                        var endIndex = linkedPositions[target];
                        if (endIndex < 0 || endIndex > 48) continue; // Code to skip out of bounds for jumps.
                        var targetPosition = TranslatetFromBoardIndex(endIndex);
                        var moveResult = TryMove(Pieces.Tiger, tigerPosition, targetPosition);
                        returnValue.Add((moveResult, targetPosition));
                    }
                    res.Add(tigerPosition, returnValue);
                }
            }
            return res;
        }

        public Dictionary<(int x, int y), List<(MoveResult result, (int x, int y) positions)>> GetGoatMoves()
        {
            Dictionary<(int x, int y), List<(MoveResult result, (int x, int y) positions)>> res = new Dictionary<(int x, int y), List<(MoveResult result, (int x, int y) posistion)>>();
            if(GoatsLeftToPlace != 0)
            {
                List<(MoveResult result, (int x, int y) posistion)> returnValue = new List<(MoveResult result, (int x, int y) posistion)>();
                for (int row = 1; row < 6; row++)
                {
                    for (int column = 1; column < 6; column++)
                    {
                        var boardIndex = TranslateToBoardIndex((column, row));
                        var piece = GetPieceAtIndex(boardIndex);
                        if (piece == Pieces.Empty)
                            returnValue.Add((MoveResult.MoveOK, (column, row)));
                    }
                }
                res.Add((0, 0), returnValue);
            }
            else
            {
                for (int i = 8; i < 41; i++)
                {
                    if ((Board[(int)Pieces.Goat] & (1L << i)) > 0)
                    {
                        List<(MoveResult result, (int x, int y) posistion)> returnValue = new List<(MoveResult result, (int x, int y) posistion)>();
                        var goatPosition = TranslatetFromBoardIndex(i);
                        var linkedPositions = GetLinkedPositions(i);
                        for (int target = 0; target < linkedPositions.Length; target++)
                        {
                            var targetPosition = TranslatetFromBoardIndex(linkedPositions[target]);
                            var moveResult = TryMove(Pieces.Goat, goatPosition, targetPosition);
                            // TODO: Only store move result if successful? And only store to dictionary if any result?
                            returnValue.Add((moveResult, targetPosition));
                        }
                        res.Add(goatPosition, returnValue);
                    }
                }
            }
            return res;
        }


        public bool GetAllMoves(Pieces piece, (int, int) position)
        {
            // TODO: Implement.
            return false;
        }

        private int[] GetLinkedPositions(int start)
        {
            // TODO: Include jump links here all at ones?
            return new int[] {
                start - 8, start - 7, start - 6,
                start - 1,            start + 1,
                start + 6, start + 7, start + 8,
            };
        }

        private int[] GetJumpLinkedPositions(int start)
        {
            // TODO: Create a index for positions instead (left up, up, right up, right, and so on.
            // move is one step in index direction (-8, -7, -6, +1, ...). Jump is just 2 times that.
            return new int[] {
                start - 16, start - 14, start - 12,
                start -  2,             start +  2,
                start + 12, start + 14, start + 16,
            };
        }
        
        public MoveResult Move(Pieces piece, (int x, int y) start, (int x, int y) end)
        {
            var move = TryMove(piece, start, end);
            return MoveDanger(piece, start, end, move);

        }

        /// <summary>
        /// This move function skips the TryMove check. Used to speed up the AI. Use with care.
        /// </summary>
        public MoveResult MoveDanger(Pieces piece, (int x, int y) start, (int x, int y) end, MoveResult move)
        {
            var endIndex = TranslateToBoardIndex(end);
            var startIndex = TranslateToBoardIndex(start);
            switch (move)
            {
                case MoveResult.GoatPlaced:
                    PerformGameMove(piece, -1, endIndex);
                    return //(CheckGameEnd(piece)) ? MoveResult.GoatWin : 
                    move;
                case MoveResult.GoatCaptured:
                    IsJumpValid(piece, startIndex, endIndex, out int captureIndex);
                    PerformGameMove(piece, startIndex, endIndex, captureIndex);
                    return //(CheckGameEnd(piece)) ? MoveResult.TigerWin : 
                    move;
                case MoveResult.MoveOK:
                    PerformGameMove(piece, startIndex, endIndex);
                    return //(CheckGameEnd(piece)) ? MoveResult.GoatWin : 
                    move;
            }
            return move;
        }

        public MoveResult TryMove(Pieces piece, (int x, int y) start, (int x, int y) end)
        {
            // Perhaps split the move validation into static and none static?
            if (piece != CurrentUsersTurn)
                return MoveResult.NotPlayersTurn;
            // If 0,0 is used as start position during goat placement, then check this early before errors on start position is found.
            var endIndex = TranslateToBoardIndex(end);
            if (piece == Pieces.Goat && start.Equals((0, 0)))
            {
                if (IsOutOfBounds(end))
                    return MoveResult.OutOfBounds;
                if (IsPieceAtIndex(Pieces.Any, endIndex))
                    return MoveResult.TargetLocationOccupied;
                return MoveResult.GoatPlaced;
            }
            if (IsOutOfBounds(start) || IsOutOfBounds(end))
                return MoveResult.OutOfBounds;
            var startIndex = TranslateToBoardIndex(start);
            if (piece != GetPieceAtIndex(startIndex))
                return MoveResult.TryToMoveIncorrectPiece;
            if (IsPieceAtIndex(Pieces.Any, endIndex))
                return MoveResult.TargetLocationOccupied;
            var moveType = PositionsAreLinked(startIndex, endIndex);
            if (moveType == MoveType.OutOfReach)
                return MoveResult.TargetLocationOutOfReach;
            // Tiger jump
            if (moveType == MoveType.Jump)
            {
                if (IsJumpValid(piece, startIndex, endIndex, out int captureIndex))
                {
                    return MoveResult.GoatCaptured;
                }
                else
                    return MoveResult.InvalidJump;
            }
            else
            {
                if (piece == Pieces.Goat && GoatsLeftToPlace != 0)
                    return MoveResult.GoatMoveDuringPlacement;
                // All checks should have passed. Now perform normal 1 slot move.
                return MoveResult.MoveOK;
            }
            
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
        private bool IsJumpValid(Pieces piece, int start, int end, out int captureIndex)
        {
            captureIndex = -1;
            if (piece != Pieces.Tiger)
                return false;
            // Should be able to omit this and leave it to the caller.
            if (start == end)
                return false;
            if (IsPieceAtIndex(Pieces.Any, end))
                return false;

            var middleIndex = ((start - end) / 2) + end;
            if (IsPieceAtIndex(Pieces.Goat, middleIndex) && !IsPieceAtIndex(Pieces.Any, end))
            {
                captureIndex = middleIndex;
                return true;
            }
            else
                return false;
        }

        private void PerformGameMove(Pieces piece, int startIndex, int endIndex, int captureIndex = -1)
        {
            // TODO: Fix this code! 3 moves: Place, move, and capture. Capture also use move.
            if (captureIndex != -1)
            {
                var helper = this.Board;
                var shiftToRemoveGoat = ~(1L << captureIndex);
                // clear goat.
                Board[(int)Pieces.Goat] = Board[(int)Pieces.Goat] & shiftToRemoveGoat;
                Board[(int)Pieces.Any] = Board[(int)Pieces.Any] & shiftToRemoveGoat;
                // Move tiger.
                // TODO: Can this be done in one operation? Yes, OR the indexes and then XOR the array!
                // Maybe just OR the Any Bord to save one operation.
                long shiftToRemove = ~(1L << startIndex);
                long shift = (1L << endIndex);
                Board[(int)Pieces.Tiger] = Board[(int)Pieces.Tiger] & shiftToRemove;
                Board[(int)Pieces.Tiger] = Board[(int)Pieces.Tiger] | shift;
                Board[(int)Pieces.Any] = Board[(int)Pieces.Any] & shiftToRemove;
                Board[(int)Pieces.Any] = Board[(int)Pieces.Any] | shift;
                GoatsCaptured++;
            }
            // Place goat.
            else if(startIndex == -1)
            {
                long shift = (1L << endIndex);
                Board[(int)Pieces.Goat] = Board[(int)Pieces.Goat] | shift;
                Board[(int)Pieces.Any] = Board[(int)Pieces.Any] | shift;
                GoatsLeftToPlace--;
            }
            // Else normal move.
            else
            {
                long shiftToRemove = ~(1L << startIndex);
                long shift = (1L << endIndex);
                Board[(int)piece] = Board[(int)piece] & shiftToRemove;
                Board[(int)piece] = Board[(int)piece] | shift;
                Board[(int)Pieces.Any] = Board[(int)Pieces.Any] & shiftToRemove;
                Board[(int)Pieces.Any] = Board[(int)Pieces.Any] | shift;
            }

            Tick();
            return;
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

        /// <summary>
        /// Translate a 1 indexed x, y coordinate into a index position on the 
        /// internal board storage.
        /// </summary>
        private static (int x, int y) TranslatetFromBoardIndex(int index)
        {
            int x = index % 7;
            int y = index / 7;
            return (x, y);
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
        TigerWin = 8,
        GoatWin = 9,
        Draw = 10,
        GoatCaptured = 11,
        GoatMoveDuringPlacement = 12,
        GoatPlaced = 13,
        InvalidJump = 14,
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
