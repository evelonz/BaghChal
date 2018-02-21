﻿using System;
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

            while(true)
            {
                var move = BaghChalAI.MinMax.GetMove(board);
                var aiRes = board.Move(move.Piece, move.Start, move.End);
                Console.WriteLine(aiRes);
                Console.WriteLine(board.ToString());
                var inputStart = Console.ReadLine().Split(',');
                var inputEnd = Console.ReadLine().Split(',');
                if (int.TryParse(inputStart[0], out int xs) && int.TryParse(inputStart[1], out int ys) &&
                    int.TryParse(inputEnd[0], out int xe) && int.TryParse(inputEnd[1], out int ye)) {
                    var humanRes = board.Move(board.CurrentUsersTurn, (xs, ys), (xe, ye));
                    Console.WriteLine(humanRes);
                }
                Console.WriteLine(board.ToString());
            }

            //board.PlacePeiceAtIndex(Pieces.Tiger, 8);
            //board.PlacePeiceAtIndex(Pieces.Tiger, 12);
            //board.PlacePeiceAtIndex(Pieces.Tiger, 36);
            //board.PlacePeiceAtIndex(Pieces.Tiger, 40);
            board = new GameBoard(7, 16, 0, Pieces.Tiger,
                (Pieces.Tiger, (1, 1)), (Pieces.Tiger, (2, 2)), (Pieces.Tiger, (1, 4)), (Pieces.Tiger, (2, 4)),
                (Pieces.Goat, (1, 2)), (Pieces.Goat, (1, 3)), (Pieces.Goat, (2, 3)), (Pieces.Goat, (3, 3))
                );

            

            int i = 0;
            PrintGameBoard(board, ++i);

            board.PlacePeiceAtIndex(Pieces.Goat, 10);
            var res = board.GetTigerMoves();
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
