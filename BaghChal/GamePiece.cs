using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaghChal
{
    abstract class GamePiece
    {
        /// <summary>
        /// Set here since it is used in a lot of logic.
        /// Of not here, then it would have to be hardcoded in each derived
        /// class. Not sure what is best.
        /// </summary>
        protected Pieces Piece { get; set; }

        public MoveResult TryMove(GameBoard board, (int x, int y) start, (int x, int y) end)
        {
            if (NotCurrentPlyersTurn(Piece, board.CurrentUsersTurn))
                return MoveResult.NotPlayersTurn;

            if (GameBoard.IsOutOfBounds(end))
                return MoveResult.OutOfBounds;

            var endIndex = GameBoard.TranslateToBoardIndex(end);

            if (TargetLocationOccupied(board, endIndex))
                return MoveResult.TargetLocationOccupied;

            var startIndex = GameBoard.TranslateToBoardIndex(start);
            
            if(TryToMoveIncorrectPiece(board, Piece, startIndex))
                    return MoveResult.TryToMoveIncorrectPiece;

            var moveType = GameBoard.PositionsAreLinked(startIndex, endIndex);
            if (LocationOutOfReach(moveType))
                return MoveResult.TargetLocationOutOfReach;

            // this function has to be overriden. Children can call this,
            // and then implement thire piece move in the end.
            return MoveResult.MoveNotImplemented;
        }

        #region TempImplementationOfMoves

        public MoveResult TryMoveTiger(GameBoard board, MoveType moveType, int startIndex, int endIndex)
        {
            if (moveType == MoveType.Jump)
            {
                if (board.IsJumpValid(Piece, startIndex, endIndex, out int captureIndex))
                {
                    return MoveResult.GoatCaptured;
                }
                else
                    return MoveResult.InvalidJump;
            }
            // All checks should have passed. Now perform normal 1 slot move.
            return MoveResult.MoveOK;
        }

        public MoveResult TryMoveGoat(GameBoard board, MoveType moveType, int startIndex, int endIndex)
        {
            if (Piece == Pieces.Goat && startIndex == 0)
            {
                if(board.GoatsLeftToPlace != 0)
                {
                    return MoveResult.GoatMoveDuringPlacement;
                }
                return MoveResult.GoatPlaced;
            }
            // All checks should have passed. Now perform normal 1 slot move.
            return MoveResult.MoveOK;
        }

        #endregion

        #region PartialGameMoveRules

        protected bool NotCurrentPlyersTurn(Pieces tryingToMove, Pieces CurrentPlayersTurn)
            => tryingToMove != CurrentPlayersTurn;

        protected bool LocationOutOfReach(MoveType moveType)
        {
            if (moveType == MoveType.OutOfReach)
                return true;
            return false;
        }

        protected bool TargetLocationOccupied(GameBoard board, int endIndex)
            => board.IsPieceAtIndex(Pieces.Any, endIndex);

        protected bool TryToMoveIncorrectPiece(GameBoard board, Pieces tryingToMove, int startIndex)
        {
            // Special logic for goats. Let them handle this check as well.
            if (tryingToMove == Pieces.Goat && startIndex == 0)
            {
                return false;
            }
            else if (Piece != board.GetPieceAtIndex(startIndex))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
