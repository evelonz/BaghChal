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
            var sw = new System.Diagnostics.Stopwatch();
            var sw2 = new System.Diagnostics.Stopwatch();
            sw2.Start();
            int numberOfTestTurns = 20;
            while(numberOfTestTurns-- > 0)
            {
                //HumanMove(board);
                //board = AIMove(board, sw);
                board = AIMove2(board, sw);
                board = AIMove3(board, sw);
                Console.ReadKey();
            }
            sw2.Stop();
            Console.WriteLine(sw2.ElapsedMilliseconds);
            Console.ReadKey();
            //board.PlacePeiceAtIndex(Pieces.Tiger, 8);
            //board.PlacePeiceAtIndex(Pieces.Tiger, 12);
            //board.PlacePeiceAtIndex(Pieces.Tiger, 36);
            //board.PlacePeiceAtIndex(Pieces.Tiger, 40);
            //board = new GameBoard(7, 16, 0, Pieces.Tiger,
            //    (Pieces.Tiger, (1, 1)), (Pieces.Tiger, (2, 2)), (Pieces.Tiger, (1, 4)), (Pieces.Tiger, (2, 4)),
            //    (Pieces.Goat, (1, 2)), (Pieces.Goat, (1, 3)), (Pieces.Goat, (2, 3)), (Pieces.Goat, (3, 3))
            //    );

            //int i = 0;
            //PrintGameBoard(board, ++i);

            //board.PlacePeiceAtIndex(Pieces.Goat, 10);
            //var res = board.GetTigerMoves();
            //PrintGameBoard(board, ++i);
        }

        private static GameBoard HumanMove(GameBoard board)
        {
            Console.WriteLine("Type start coordinate then enter (x,y).");
            var inputStart = Console.ReadLine().Split(',');
            Console.WriteLine("Type end coordinate then enter (x,y).");
            var inputEnd = Console.ReadLine().Split(',');

            while(true)
            {
                if (int.TryParse(inputStart[0], out int xs) && int.TryParse(inputStart[1], out int ys) &&
                int.TryParse(inputEnd[0], out int xe) && int.TryParse(inputEnd[1], out int ye))
                {
                    var humanRes = board.Move(board.CurrentUsersTurn, (xs, ys), (xe, ye));
                    Console.WriteLine(humanRes);
                    Console.WriteLine(board.ToString());
                    return humanRes.nextState;
                }
                else
                {
                    Console.WriteLine("Invalid input, state input as int, int for the coordinates.");
                }
            }

        }

        private static GameBoard AIMove(GameBoard board, System.Diagnostics.Stopwatch sw)
        {
            sw.Restart();
            var move = BaghChalAI.MinMax.GetMove(board);
            sw.Stop();
            var aiRes = board.Move(move.Piece, move.Start, move.End);
            Console.WriteLine($"MoveResult: {aiRes.move}, Time (ms): {sw.ElapsedMilliseconds}, Checks/ms: {move.Checks/(sw.ElapsedMilliseconds+1)}, {move.ToString()}.");
            Console.WriteLine(aiRes.nextState.ToString());
            return aiRes.nextState;
        }

        private static GameBoard AIMove2(GameBoard board, System.Diagnostics.Stopwatch sw)
        {
            sw.Restart();
            var move = BaghChalAI.MinMaxExternal.GetMove(board);
            sw.Stop();
            var aiRes = board.Move(move.Piece, move.Start, move.End);
            Console.WriteLine($"MoveResult2: {aiRes.move}, Time (ms): {sw.ElapsedMilliseconds}, Checks/ms: {move.Checks / (sw.ElapsedMilliseconds + 1)}, {move.ToString()}.");
            Console.WriteLine(aiRes.nextState.ToString());
            return aiRes.nextState;
        }

        private static GameBoard AIMove3(GameBoard board, System.Diagnostics.Stopwatch sw)
        {
            sw.Restart();
            var move = BaghChalAI.MinMaxExternalParallel.GetMove(board);
            sw.Stop();
            var aiRes = board.Move(move.Piece, move.Start, move.End);
            Console.WriteLine($"MoveResult3: {aiRes.move}, Time (ms): {sw.ElapsedMilliseconds}, Checks/ms: {move.Checks / (sw.ElapsedMilliseconds + 1)}, {move.ToString()}.");
            Console.WriteLine(aiRes.nextState.ToString());
            return aiRes.nextState;
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
