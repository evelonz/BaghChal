using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaghChal
{

    public class GameBoard
    {
        #region Properties and fields
        
        /// <summary>
        /// Holds the state of the board.
        /// First index is for tigers, goats, and both.
        /// Second index is for the bords position, starting at the top left.
        /// New rows wraps after seven positions. Outer rows are out of bounds.
        /// </summary>
        private long[] Board { get; set; }

        public long this[Pieces index]
        {
            get
            {
                return Board[(int)index];
            }
        }

        /// <summary>
        /// The player who may move next. Tiger starts.
        /// </summary>
        public Pieces CurrentUsersTurn { get; private set; } = Pieces.Goat;

        /// <summary>
        /// Ply is a half turn in the game.
        /// One turn is a move by first goat then by tiger. So a ply is the move of one of them.
        /// </summary>
        public readonly int Ply = 0;

        public readonly int GoatsLeftToPlace = 20;

        public readonly int GoatsCaptured = 0;

        #endregion

        #region constructors and factories

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
                var index = GameBoard.TranslateToBoardIndex(position);
                PlacePeiceAtIndex(pieceType, index);
            }
        }

        #endregion

        #region interfaces
        
        #endregion

        #region overrides

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
            // TODO: Not sure if this is a good hash function. Issue added to make it handle transposed positions.
            return Board[0].GetHashCode() ^ Board[1].GetHashCode();
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

        #endregion

        #region methods

        #region Basic game board functionality

        public bool IsPieceAtIndex(Pieces type, int index)
        {
            long shiftMe = 1L;
            return ((shiftMe << index) & Board[(int)type]) != 0L;
        }

        public Pieces GetPieceAtIndex(int index)
        {
            long shiftMe = 1L;
            if (((shiftMe << index) & Board[(int)Pieces.Any]) != 0L)
            {
                if (((shiftMe << index) & Board[(int)Pieces.Tiger]) != 0L)
                    return Pieces.Tiger;
                else
                    return Pieces.Goat;
            }
            else
                return Pieces.Empty;
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

        private int[] GetLinkedPositions(int start)
        {
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

        /// <summary>
        /// Check if two positions on the board are linked, either directly or by a jump.
        /// Calls to this function assumes that the positions given have already been checked so that they
        /// are within bounds.
        /// </summary>
        /// <param name="start">The start position to check from.</param>
        /// <param name="end">The end position to check connection to.</param>
        /// <returns>
        /// The type of connection, positive ones are <see cref="MoveType.Linked"/> and
        /// <see cref="MoveType.Jump"/> while no connection is <see cref="MoveType.OutOfReach"/>.
        /// </returns>
        public static MoveType PositionsAreLinked(int start, int end)
        {
            // Positions up, down, left, and right are connected.
            var dx = Math.Abs(start - end);
            if (dx == 1 || dx == 7)
                return MoveType.Linked;

            // A jump is simply a move of two steps. Though only tigers can do it and there must be a goat 
            // between. This code only checks that the move is possible from a boards perspective.
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
        /// Check that a jump move is valid.
        /// Can only be done by a tiger.
        /// Must be done over a goat, into a empty space.
        /// </summary>
        internal bool IsJumpValid(Pieces piece, int start, int end, out int captureIndex)
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

        /// <summary>
        /// Translate a 1 indexed x, y coordinate into a index position on the 
        /// internal board storage.
        /// </summary>
        public static int TranslateToBoardIndex((int x, int y) position)
        {
            // TODO: Should rewrite the entire internal code to only work with indexes. Tried a dictionary and it was slower!
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

        /// <summary>
        /// Check that a position is not out of bounds on the board.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool IsOutOfBounds((int x, int y) position)
        {
            return position.x < 1 || position.y < 1 ||
                position.x > 5 || position.y > 5;
        }

        #endregion

        #region Game logic

        /// <summary>
        /// Progress the game forward one ply.
        /// </summary>
        private (Pieces NextPlayerTurn, int Ply) Tick()
        {
            return (
                (CurrentUsersTurn == Pieces.Goat) ? Pieces.Tiger : Pieces.Goat,
                Ply + 1
                );
        }

        /// <summary>
        /// Check if any of the game ending conditions are met.
        /// </summary>
        /// <param name="piece">The piece from whoms position the checks should be made.</param>
        /// <param name="checkAfterMove">If true, check if won. Else check if lost.</param>
        /// <returns></returns>
        public bool CheckGameEnd(Pieces piece, bool checkAfterMove = true)
        {
            // TODO: Refactor this function. The logic/namings are kind of confusing.
            switch (piece)
            {
                case Pieces.Tiger:
                    if (checkAfterMove)
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

        /// <summary>
        /// Check that any tiger on the board is able to move.
        /// If no tiger can move, then tigers have lost.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Get all posible moves for the tigers on the board.
        /// </summary>
        /// <returns>
        /// A dictionary with coordinates of the tigers, together with a list of each tigers possible moves.
        /// </returns>
        public Dictionary<(int x, int y), List<(MoveResult result, (int x, int y) positions)>> GetTigerMoves()
        {
            var res = new Dictionary<(int x, int y), List<(MoveResult result, (int x, int y) posistion)>>(4);
            for (int i = 8; i < 41; i++)
            {
                if ((Board[(int)Pieces.Tiger] & (1L << i)) > 0)
                {
                    var returnValue = new List<(MoveResult result, (int x, int y) posistion)>(16);
                    var tigerPosition = TranslatetFromBoardIndex(i);
                    var linkedPositions = GetLinkedPositions(i);
                    for (int target = 0; target < linkedPositions.Length; target++)
                    {
                        var targetPosition = TranslatetFromBoardIndex(linkedPositions[target]);
                        var moveResult = TryMove(Pieces.Tiger, tigerPosition, targetPosition);
                        returnValue.Add((moveResult, targetPosition));
                    }
                    // TODO: Now checks if there are any jump points. Might be faster to filter on adjacent goats first?
                    linkedPositions = GetJumpLinkedPositions(i);
                    for (int target = 0; target < linkedPositions.Length; target++)
                    {
                        var endIndex = linkedPositions[target];
                        // TODO: This check is not complete. Create a index OutOfBounds check function and use it instead.
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

        /// <summary>
        /// Get all possible moves for the goats.
        /// During placement phase, all posssible moves are added to a (0, 0) key. 
        /// </summary>
        /// <returns>
        /// A dictionary with coordinates of the goats, together with a list of each goats possible moves.
        /// During placement phase, all possible placements are added to a (0, 0) key.
        /// </returns>
        public Dictionary<(int x, int y), List<(MoveResult result, (int x, int y) positions)>> GetGoatMoves()
        {
            Dictionary<(int x, int y), List<(MoveResult result, (int x, int y) positions)>> res = new Dictionary<(int x, int y), List<(MoveResult result, (int x, int y) posistion)>>();
            if (GoatsLeftToPlace != 0)
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

        /// <summary>
        /// Performe a move in the game.
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public (MoveResult move, GameBoard nextState) Move(Pieces piece, (int x, int y) start, (int x, int y) end)
        {
            var move = TryMove(piece, start, end);
            return MoveDanger(piece, start, end, move);
        }

        /// <summary>
        /// This move function skips the TryMove check. Used to speed up the AI. Use with care.
        /// </summary>
        public (MoveResult move, GameBoard nextState) MoveDanger(Pieces piece, (int x, int y) start, (int x, int y) end, MoveResult move)
        {
            var endIndex = TranslateToBoardIndex(end);
            var startIndex = TranslateToBoardIndex(start);
            GameBoard nextState;
            switch (move)
            {
                case MoveResult.GoatPlaced:
                    nextState = PerformGameMove(piece, -1, endIndex);
                    return //(CheckGameEnd(piece)) ? MoveResult.GoatWin : 
                    (move, nextState);
                case MoveResult.GoatCaptured:
                    IsJumpValid(piece, startIndex, endIndex, out int captureIndex);
                    nextState = PerformGameMove(piece, startIndex, endIndex, captureIndex);
                    return //(CheckGameEnd(piece)) ? MoveResult.TigerWin : 
                    (move, nextState);
                case MoveResult.MoveOK:
                    nextState = PerformGameMove(piece, startIndex, endIndex);
                    return //(CheckGameEnd(piece)) ? MoveResult.GoatWin : 
                    (move, nextState);
            }
            return (move, this);
        }

        /// <summary>
        /// Test if a given move is valid.
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public MoveResult TryMove(Pieces piece, (int x, int y) start, (int x, int y) end)
        {
            // Perhaps split the move validation into static and none static?
            if (piece != CurrentUsersTurn)
                return MoveResult.NotPlayersTurn;
            // If 0,0 is used as start position during goat placement, then check this early before errors on start position is found.
            if (IsOutOfBounds(end))
                return MoveResult.OutOfBounds;
            var endIndex = TranslateToBoardIndex(end);
            if (piece == Pieces.Goat && start.Equals((0, 0)))
            {
                if (IsPieceAtIndex(Pieces.Any, endIndex))
                    return MoveResult.TargetLocationOccupied;
                return MoveResult.GoatPlaced;
            }
            if (IsOutOfBounds(start))
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

        /// <summary>
        /// Performe a game move and progress the game.
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="captureIndex"></param>
        private GameBoard PerformGameMove(Pieces piece, int startIndex, int endIndex, int captureIndex = -1)
        {
            var returnBoard = new long[3];
            returnBoard[(int)Pieces.Tiger] = Board[(int)Pieces.Tiger];
            returnBoard[(int)Pieces.Goat] = Board[(int)Pieces.Goat];
            returnBoard[(int)Pieces.Any] = Board[(int)Pieces.Any];
            var goatsLeftToPlace = GoatsLeftToPlace;
            var goatsCaptured = GoatsCaptured;

            if (captureIndex != -1)
            {
                var helper = this.Board;
                var shiftToRemoveGoat = ~(1L << captureIndex);
                // clear goat.
                returnBoard[(int)Pieces.Goat] = returnBoard[(int)Pieces.Goat] & shiftToRemoveGoat;
                returnBoard[(int)Pieces.Any] = returnBoard[(int)Pieces.Any] & shiftToRemoveGoat;
                // Move tiger.
                // TODO: Can this be done in one operation? Yes, OR the indexes and then XOR the array!
                // Maybe just OR the Any Bord to save one operation.
                long shiftToRemove = ~(1L << startIndex);
                long shift = (1L << endIndex);
                returnBoard[(int)Pieces.Tiger] = returnBoard[(int)Pieces.Tiger] & shiftToRemove;
                returnBoard[(int)Pieces.Tiger] = returnBoard[(int)Pieces.Tiger] | shift;
                returnBoard[(int)Pieces.Any] = returnBoard[(int)Pieces.Any] & shiftToRemove;
                returnBoard[(int)Pieces.Any] = returnBoard[(int)Pieces.Any] | shift;
                goatsCaptured++;
            }
            // Place goat.
            else if (startIndex == -1)
            {
                long shift = (1L << endIndex);
                returnBoard[(int)Pieces.Goat] = returnBoard[(int)Pieces.Goat] | shift;
                returnBoard[(int)Pieces.Any] = returnBoard[(int)Pieces.Any] | shift;
                goatsLeftToPlace--;
            }
            // Else normal move.
            else
            {
                long shiftToRemove = ~(1L << startIndex);
                long shift = (1L << endIndex);
                returnBoard[(int)piece] = returnBoard[(int)piece] & shiftToRemove;
                returnBoard[(int)piece] = returnBoard[(int)piece] | shift;
                returnBoard[(int)Pieces.Any] = returnBoard[(int)Pieces.Any] & shiftToRemove;
                returnBoard[(int)Pieces.Any] = returnBoard[(int)Pieces.Any] | shift;
            }

            var(nextPlayerTurn, ply) = Tick();

            var nextState = new GameBoard(returnBoard, nextPlayerTurn, ply, goatsLeftToPlace, goatsCaptured);

            return nextState;
        }

        #endregion

        #endregion

    }

}
