using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaghChal;

namespace BaghChalConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {

            var board = new GameBoard();

            board.PlacePeiceAtIndex(Pieces.Tiger, 8);
            board.PlacePeiceAtIndex(Pieces.Tiger, 12);
            board.PlacePeiceAtIndex(Pieces.Tiger, 36);
            board.PlacePeiceAtIndex(Pieces.Tiger, 40);

            int i = 0;
            PrintGameBoard(board, ++i);

            board.PlacePeiceAtIndex(Pieces.Goat, 10);

            PrintGameBoard(board, ++i);
        }

        static void PrintGameBoard(GameBoard board, int round)
        {
            char[] symbols = { 'T', 'G', '/', '-', };
            for (int row = 1; row < 6; row++)
            {
                for (int column = 1; column < 6; column++)
                {
                    var piece = board.GetPieceAtIndex(column + (row * 7));
                    Console.Write(symbols[(int)piece]);
                }
                Console.WriteLine();
            }
            Console.WriteLine($"Round {round}");
        }
    }
}
