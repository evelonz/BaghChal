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
        Any = 3,
        Empty = 4
    }

    public class GameBoard
    {
        /// <summary>
        /// Holds the state of the board.
        /// First index is for tigers, goats, and both.
        /// Second index is for the bords position, starting at the top left.
        /// New rows wraps after seven positions. Outer rows are out of bounds.
        /// </summary>
        public byte[,] Board { get; set; }

        public GameBoard()
        {
            Board = new byte[3, 48];
        }
    }


    public class BoardPosition
    {
        public int X { get; set; }
        public int Y { get; set; }

        public BoardPosition(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
    public class GameMove
    {
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
