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
            //var res = MiniMax(node, 4, true);
            var res = AlphaBetaPruning(node, 6, true, int.MinValue, int.MaxValue);
            return res;
        }
        /// <summary>
        /// Simple minimax function.
        /// Some stats:
        /// 2 Ply: ~120ms, 250 checks.
        /// 4 ply: ~3 000ms, 60 000-40 000 checks.
        /// 5 ply: 374 000ms, 915 000 checks.
        /// </summary>
        public static GameMove MiniMax(NodeBaseClass node, int depth, bool maximizing)
        {
            if (depth == 0 || node.GameIsFinished())
                return new GameMove(node.EvaluateNode());

            int checkedNodes = 0;
            if (maximizing)
            {
                var bestVal = int.MinValue;
                GameMove bestMove = null;
                var childNodes = node.GetChildNodes();
                foreach (var move in childNodes)
                {
                    var res = MiniMax(move, depth - 1, !maximizing);
                    checkedNodes += res.Checks;
                    if (res.Score > bestVal)
                    {
                        bestVal = res.Score;
                        bestMove = move.Step;
                    }
                }
                bestMove.Score = bestVal;
                bestMove.Checks = checkedNodes + 1;
                return bestMove;
            }
            else
            {
                var bestVal = int.MaxValue;
                GameMove bestMove = new GameMove(-1);
                foreach (var move in node.GetChildNodes())
                {
                    var res = MiniMax(move, depth - 1, !maximizing);
                    checkedNodes += res.Checks;
                    if (res.Score < bestVal)
                    {
                        bestVal = res.Score;
                        bestMove = move.Step;
                    }
                }
                bestMove.Score = bestVal;
                bestMove.Checks = checkedNodes + 1;
                return bestMove;
            }

        }

        /// <summary>
        /// Simple minimax function.
        /// Some stats:
        /// 6 Ply: 2 000-10 000ms 12 000 - 55 000 checks.
        /// </summary>
        public static GameMove AlphaBetaPruning(NodeBaseClass node, int depth, bool maximizing, int alpha, int beta)
        {
            if (depth == 0 || node.GameIsFinished())
                return new GameMove(node.EvaluateNode());

            int checkedNodes = 0;
            if (maximizing)
            {
                var bestVal = int.MinValue;
                GameMove bestMove = null;
                var childNodes = node.GetChildNodes();
                foreach (var move in childNodes)
                {
                    var res = AlphaBetaPruning(move, depth - 1, !maximizing, alpha, beta);
                    checkedNodes += res.Checks;
                    if (res.Score > bestVal)
                    {
                        bestVal = res.Score;
                        bestMove = move.Step;
                    }
                    alpha = (bestVal > alpha) ? bestVal : alpha;
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                bestMove.Score = bestVal;
                bestMove.Checks = checkedNodes + 1;
                return bestMove;
            }
            else
            {
                var bestVal = int.MaxValue;
                GameMove bestMove = new GameMove(-1);
                foreach (var move in node.GetChildNodes())
                {
                    var res = AlphaBetaPruning(move, depth - 1, !maximizing, alpha, beta);
                    checkedNodes += res.Checks;
                    if (res.Score < bestVal)
                    {
                        bestVal = res.Score;
                        bestMove = move.Step;
                    }
                    beta = (bestVal < beta) ? bestVal : beta;
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                bestMove.Score = bestVal;
                bestMove.Checks = checkedNodes + 1;
                return bestMove;
            }

        }
    }



    public class NodeBaseClass
    {
        public GameBoard GameBoard { get; set; }
        public Pieces Player { get; set; }
        public GameMove Step { get; set; }
        public static readonly MoveResult[] GoodMoves = { MoveResult.MoveOK, MoveResult.TigerWin, MoveResult.GoatCaptured, MoveResult.GoatPlaced, MoveResult.GoatWin, MoveResult.Draw};

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
                            if(GoodMoves.Contains(moveResult))
                                result.Add(new NodeBaseClass(board, Pieces.Tiger, new GameMove(Pieces.Goat, piece.Key, move.positions)));
                        }
                    }
                }
            }
            return result;
        }

        public bool GameIsFinished()
        {
            return GameBoard.CheckGameEnd(Player, false);
        }
    }

    public class GameMove
    {
        public Pieces Piece { get; set; }
        public (int x, int y) Start { get; set; }
        public (int x, int y) End { get; set; }
        public int Score { get; set; }
        public int Checks { get; set; } = 1;
        public GameMove(int score) { Score = score; }
        public GameMove(Pieces p, (int x, int y) s, (int x, int y) e)
        {
            Piece = p;
            Start = s;
            End = e;
        }

        public override string ToString()
        {
            return $"S: {Score}, C: {Checks}, P: {Piece}, S: {Start}, E: {End}.";
        }
    }
}
