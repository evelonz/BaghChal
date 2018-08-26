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
        MoveNotImplemented = 15,
    }

    public enum MoveType : int
    {
        Linked = 1,
        Jump = 2,
        OutOfReach = 3,
    }
}
