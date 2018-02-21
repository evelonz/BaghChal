using BaghChal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaghChalAI
{
    public class MinMax
    {
        public static GameMove GetMove(GameBoard board)
        {
            var node = new NodeBaseClass(board, board.CurrentUsersTurn, new GameMove(-1));
            var res = MiniMax(node, 2, true);
            return res;
        }

        public static GameMove MiniMax(NodeBaseClass node, int depth, bool maximizing)
        {
            if (depth == 0 || node.GameIsFinished())
                return new GameMove(node.EvaluateNode());

            if (maximizing)
            {
                var bestVal = int.MinValue;
                GameMove bestMove = null;
                foreach (var move in node.GetChildNodes())
                {
                    var res = MiniMax(move, depth - 1, !maximizing);
                    if(res.Score > bestVal)
                    {
                        bestVal = res.Score;
                        bestMove = move.Step;
                    }
                }
                bestMove.Score = bestVal;
                return bestMove;
            }
            else
            {
                var bestVal = int.MaxValue;
                GameMove bestMove = new GameMove(-1);
                foreach (var move in node.GetChildNodes())
                {
                    var res = MiniMax(move, depth - 1, !maximizing);
                    if (res.Score < bestVal)
                    {
                        bestVal = res.Score;
                        bestMove = move.Step;
                    }
                }
                bestMove.Score = bestVal;
                return bestMove;
            }

        }
    }



    public class NodeBaseClass
    {
        public GameBoard GameBoard { get; set; }
        public Pieces Player { get; set; }
        public GameMove Step { get; set; }
        public static readonly MoveResult[] GoodMoves = { MoveResult.MoveOK, MoveResult.TigerWin, MoveResult.GoatCaptured, MoveResult.GoatWin, MoveResult.Draw};

        public NodeBaseClass(GameBoard gameBoard, Pieces player, GameMove step)
        {
            GameBoard = gameBoard;
            Player = player;
            Step = step;
        }

        public int EvaluateNode()
        {
            // TODO: Improve this, we need good scores long before a goat is captured or game ends.
            if (Player == Pieces.Goat)
            {
                if (GameBoard.CheckGameEnd(Pieces.Tiger)) { return int.MaxValue; }
            }
            else
            {
                if (GameBoard.CheckGameEnd(Pieces.Goat)) { return int.MaxValue; }
            }
                

            // Count score for Tiger, then revert for goat.
            int sum = GameBoard.GoatsCaptured * 1000;
            // Adding play to return to try and play longer.
            return (Player == Pieces.Tiger) ? sum : -sum + GameBoard.Ply;
        }

        public List<NodeBaseClass> GetChildNodes()
        {
            var goodMove = new List<MoveResult>() { MoveResult.MoveOK, MoveResult.TigerWin, MoveResult.GoatCaptured, MoveResult.GoatWin, MoveResult.Draw };
            var tempBoard = (GameBoard)GameBoard.Clone();
            var result = new List<NodeBaseClass>();
            // Start just playing the tiger and goat placements.
            if(Player == Pieces.Tiger)
            {
                var moves = GameBoard.GetTigerMoves();
                foreach (var piece in moves)
                {
                    foreach (var move in piece.Value.Where(x => GoodMoves.Contains(x.result)))
                    {
                        var board = (GameBoard)(tempBoard).Clone();
                        var moveResult = board.Move(Pieces.Tiger, piece.Key, move.positions);
                        result.Add(new NodeBaseClass(board, Pieces.Goat, new GameMove(Pieces.Tiger, piece.Key, move.positions)));
                    }
                }
            }
            else
            {
                // Default first move for Gaot. Only one to not lose a goat in the first 10 moves.
                if (GameBoard.Ply == 0)
                {
                    var board = (GameBoard)(tempBoard).Clone();
                    var moveResult = board.Move(Pieces.Goat, (0, 0), (3, 1));
                    result.Add(new NodeBaseClass(board, Pieces.Tiger, new GameMove(Pieces.Goat, (0, 0), (3, 1))));
                }
                // Else, play as normal.
                else
                {
                    var moves = GameBoard.GetGoatMoves();
                    foreach (var piece in moves)
                    {
                        foreach (var move in piece.Value.Where(x => GoodMoves.Contains(x.result)))
                        {
                            var board = (GameBoard)(tempBoard).Clone();
                            var moveResult = board.Move(Pieces.Goat, piece.Key, move.positions);
                            result.Add(new NodeBaseClass(board, Pieces.Tiger, new GameMove(Pieces.Tiger, piece.Key, move.positions)));
                        }
                    }
                }
            }
            return result;
        }

        public bool GameIsFinished()
        {
            if(Player == Pieces.Goat)
                return GameBoard.CheckGameEnd(Pieces.Tiger);
            else
                return GameBoard.CheckGameEnd(Pieces.Goat);

        }
    }

    public class GameMove
    {
        public Pieces Piece { get; set; }
        public (int x, int y) Start { get; set; }
        public (int x, int y) End { get; set; }
        public int Score { get; set; }
        public GameMove(int score) { Score = score; }
        public GameMove(Pieces p, (int x, int y) s, (int x, int y) e)
        {
            Piece = p;
            Start = s;
            End = e;
        }
    }
}
