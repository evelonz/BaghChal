using BaghChal;
using MiniMaxStandard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaghChalAI
{
    public class MinMaxExternalParallel
    {
        public static GameMove2 GetMove(GameBoard board)
        {
            var node = new MinMaxExternalNode(board, board.CurrentUsersTurn, new GameMove2(-1));
            //var res = MiniMax(node, 4, true);

            //var runner = MiniMaxFactory.GetInstance<GameMove2>();
            var runner = MiniMaxFactory.GetParallelInstance<GameMove2>();
            var res = runner.Run(node, 4, true, 10000, Environment.ProcessorCount);

            res.Checks = runner.EndNodesChecked;
            return res;
        }
    }

    public class MinMaxExternal
    {
        public static GameMove2 GetMove(GameBoard board)
        {
            var node = new MinMaxExternalNode(board, board.CurrentUsersTurn, new GameMove2(-1));
            //var res = MiniMax(node, 4, true);

            //var runner = MiniMaxFactory.GetInstance<GameMove2>();
            var runner = MiniMaxFactory.GetInstance<GameMove2>(true);
            var res = runner.Run(node, 4, true);

            res.Checks = runner.EndNodesChecked;
            return res;
        }
    }

    public class MinMaxExternalNode : IMinimaxNode<GameMove2>
    {
        public GameBoard GameBoard { get; set; }
        public Pieces Player { get; set; }
        public GameMove2 Step { get; set; }
        public static readonly MoveResult[] GoodMoves = { MoveResult.MoveOK, MoveResult.TigerWin, MoveResult.GoatCaptured, MoveResult.GoatPlaced, MoveResult.GoatWin, MoveResult.Draw };

        public MinMaxExternalNode(GameBoard gameBoard, Pieces player, GameMove2 step)
        {
            GameBoard = gameBoard;
            Player = player;
            Step = step;
        }

        public int Evaluate()
        {
            if (Player == Pieces.Goat)
            {
                if (GameBoard.CheckGameEnd(Pieces.Tiger)) { return int.MinValue; }
            }
            else
            {
                if (GameBoard.CheckGameEnd(Pieces.Goat)) { return int.MaxValue; }
            }

            return AttempOnBetterValuation();
        }

        private int NaiveValuation()
        {
            // TODO: Improve this, we need good scores long before a goat is captured or game ends.

            // Count score for Tiger, then revert for goat.
            int sum = GameBoard.GoatsCaptured * 1000;
            // Adding play to return to try and play longer.
            return (Player == Pieces.Tiger) ? sum : -sum + GameBoard.Ply;
        }

        private int AttempOnBetterValuation()
        {
            // This paper used 26 features as input to thier neural network.
            // Trying to use some of their features.
            // https://pdfs.semanticscholar.org/a204/1ce875afff7a5d8c6d452e138fdd0e61ca93.pdf

            // Count is for Tiger, inversed for goats.
            var goatsCaptured = GameBoard.GoatsCaptured * 1000;
            var tigerMoves = GameBoard.GetTigerMoves();
            var tigerMoveCount = tigerMoves.Select(s => s.Value.Count).Sum();
            var trappedTigers = tigerMoves.Count(s => s.Value.Count == 0);

            var tigers = tigerMoves.Keys.ToArray();
            var manhattanDistance = 0;
            for (int i = 0; i < tigers.Length - 1; i++)
            {
                for (int j = 0; j < tigers.Length; j++)
                {
                    manhattanDistance += ManhattanDistance(tigers[i], tigers[j]);
                }
            }

            var sum =
                goatsCaptured * 1000 +
                tigerMoveCount * 10 +
                trappedTigers * -500 +
                manhattanDistance * 8000 +
                GameBoard.Ply;

            // Adding play to return to try and play longer.
            return (Player == Pieces.Tiger) ? sum : -sum;
        }

        public static int ManhattanDistance((int x, int y) self, (int x, int y) target)
        {
            return Math.Abs(self.x - target.x) + Math.Abs(self.y - target.y);
        }

        public IEnumerable<IMinimaxNode<GameMove2>> GetChildren()
        {
            // Start just playing the tiger and goat placements.
            if (Player == Pieces.Tiger)
            {
                var moves = GameBoard.GetTigerMoves();
                foreach (var piece in moves)
                {
                    foreach (var (result, positions) in piece.Value.Where(x => GoodMoves.Contains(x.result)))
                    {
                        var (_, nextState) = GameBoard.MoveDanger(Pieces.Tiger, piece.Key, positions, result);
                        yield return new MinMaxExternalNode(nextState, Pieces.Goat, new GameMove2(Pieces.Tiger, piece.Key, positions));
                    }
                }
            }
            else
            {
                // Default first move for Gaot. Only one to not lose a goat in the first 10 moves.
                if (GameBoard.Ply == 0)
                {
                    var (_, nextState) = GameBoard.Move(Pieces.Goat, (0, 0), (3, 1));
                    yield return new MinMaxExternalNode(nextState, Pieces.Tiger, new GameMove2(Pieces.Goat, (0, 0), (3, 1)));
                }
                // Else, play as normal.
                else
                {
                    var moves = GameBoard.GetGoatMoves();
                    foreach (var piece in moves)
                    {
                        foreach (var (result, positions) in piece.Value.Where(x => GoodMoves.Contains(x.result)))
                        {
                            var (_, nextState) = GameBoard.MoveDanger(Pieces.Goat, piece.Key, positions, result);
                            yield return new MinMaxExternalNode(nextState, Pieces.Tiger, new GameMove2(Pieces.Goat, piece.Key, positions));
                        }
                    }
                }
            }
        }

        public bool IsTerminal()
        {
            return GameBoard.CheckGameEnd(Player, false);
        }

        public GameMove2 GetMove()
        {
            return Step;
        }
    }

    public class GameMove2 : IGameMove
    {
        public Pieces Piece { get; set; }
        public (int x, int y) Start { get; set; }
        public (int x, int y) End { get; set; }
        public int Score { get; set; }
        public int Checks { get; set; } = 1;
        public int HashHits { get; set; } = 0;
        public GameMove2() { Score = 0; }
        public GameMove2(int score) { Score = score; }
        public GameMove2(Pieces p, (int x, int y) s, (int x, int y) e)
        {
            Piece = p;
            Start = s;
            End = e;
        }

        public override string ToString()
        {
            return $"S: {Score}, C: {Checks}, H: {HashHits}, P: {Piece}, S: {Start}, E: {End}";
        }
    }


    public class MinMax
    {
        public static Dictionary<GameBoard, int> LookupTable { get; set; } = new Dictionary<GameBoard, int>();

        public static GameMove GetMove(GameBoard board)
        {
            LookupTable = new Dictionary<GameBoard, int>();
            var node = new NodeBaseClass(board, board.CurrentUsersTurn, new GameMove(-1));
            var res = MiniMax(node, 4, true);
            //var res = AlphaBetaPruning(node, 6, true, int.MinValue, int.MaxValue);
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
            if(LookupTable.ContainsKey(node.GameBoard))
            {
                var move = new GameMove(LookupTable[node.GameBoard]);
                move.HashHits = 1;
                return move;
            }
            if (depth == 0 || node.GameIsFinished())
            {
                var score = node.EvaluateNode();
                LookupTable.Add(node.GameBoard, score);
                return new GameMove(score);
            }

            int checkedNodes = 0;
            int cacheHits = 0;
            if (maximizing)
            {
                var bestVal = int.MinValue;
                GameMove bestMove = null;
                var childNodes = node.GetChildNodes();
                foreach (var move in childNodes)
                {
                    var res = MiniMax(move, depth - 1, !maximizing);
                    checkedNodes += res.Checks;
                    cacheHits += res.HashHits;
                    if (res.Score > bestVal)
                    {
                        bestVal = res.Score;
                        bestMove = move.Step;
                    }
                }
                bestMove.Score = bestVal;
                bestMove.Checks = checkedNodes + 1;
                bestMove.HashHits = cacheHits;
                LookupTable.Add(node.GameBoard, bestVal);
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
                    cacheHits += res.HashHits;
                    if (res.Score < bestVal)
                    {
                        bestVal = res.Score;
                        bestMove = move.Step;
                    }
                }
                bestMove.Score = bestVal;
                bestMove.Checks = checkedNodes + 1;
                bestMove.HashHits = cacheHits;
                LookupTable.Add(node.GameBoard, bestVal);
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
            if (LookupTable.ContainsKey(node.GameBoard))
            {
                var move = new GameMove(LookupTable[node.GameBoard]);
                move.HashHits = 1;
                return move;
            }
            if (depth == 0 || node.GameIsFinished())
            {
                var score = node.EvaluateNode();
                LookupTable.Add(node.GameBoard, score);
                return new GameMove(score);
            }

            int checkedNodes = 0;
            int cacheHits = 0;
            if (maximizing)
            {
                var bestVal = int.MinValue;
                GameMove bestMove = null;
                var childNodes = node.GetChildNodes();
                foreach (var move in childNodes)
                {
                    var res = AlphaBetaPruning(move, depth - 1, !maximizing, alpha, beta);
                    checkedNodes += res.Checks;
                    cacheHits += res.HashHits;
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
                bestMove.HashHits = cacheHits;
                LookupTable.Add(node.GameBoard, bestVal);
                return bestMove;
            }
            else
            {
                var bestVal = int.MaxValue;
                GameMove bestMove = new GameMove(-1);
                var childNodes = node.GetChildNodes();
                foreach (var move in childNodes)
                {
                    var res = AlphaBetaPruning(move, depth - 1, !maximizing, alpha, beta);
                    checkedNodes += res.Checks;
                    cacheHits += res.HashHits;
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
                bestMove.HashHits = cacheHits;
                LookupTable.Add(node.GameBoard, bestVal);
                return bestMove;
            }

        }
    }



    public class NodeBaseClass
    {
        public GameBoard GameBoard { get; set; }
        public Pieces Player { get; set; }
        public GameMove Step { get; set; }
        public static readonly MoveResult[] GoodMoves = { MoveResult.MoveOK, MoveResult.TigerWin, MoveResult.GoatCaptured, MoveResult.GoatPlaced, MoveResult.GoatWin, MoveResult.Draw };

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
            var result = new List<NodeBaseClass>();
            // Start just playing the tiger and goat placements.
            if(Player == Pieces.Tiger)
            {
                var moves = GameBoard.GetTigerMoves();
                foreach (var piece in moves)
                {
                    foreach (var move in piece.Value.Where(x => GoodMoves.Contains(x.result)))
                    {
                        var moveResult = GameBoard.MoveDanger(Pieces.Tiger, piece.Key, move.positions, move.result);
                        result.Add(new NodeBaseClass(moveResult.nextState, Pieces.Goat, new GameMove(Pieces.Tiger, piece.Key, move.positions)));
                    }
                }
            }
            else
            {
                // Default first move for Gaot. Only one to not lose a goat in the first 10 moves.
                if (GameBoard.Ply == 0)
                {
                    var moveResult = GameBoard.Move(Pieces.Goat, (0, 0), (3, 1));
                    result.Add(new NodeBaseClass(moveResult.nextState, Pieces.Tiger, new GameMove(Pieces.Goat, (0, 0), (3, 1))));
                }
                // Else, play as normal.
                else
                {
                    var moves = GameBoard.GetGoatMoves();
                    foreach (var piece in moves)
                    {
                        foreach (var move in piece.Value.Where(x => GoodMoves.Contains(x.result)))
                        {
                            var moveResult = GameBoard.MoveDanger(Pieces.Goat, piece.Key, move.positions, move.result);
                            result.Add(new NodeBaseClass(moveResult.nextState, Pieces.Tiger, new GameMove(Pieces.Goat, piece.Key, move.positions)));
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
        public int HashHits { get; set; } = 0;
        public GameMove(int score) { Score = score; }
        public GameMove(Pieces p, (int x, int y) s, (int x, int y) e)
        {
            Piece = p;
            Start = s;
            End = e;
        }

        public override string ToString()
        {
            return $"S: {Score}, C: {Checks}, H: {HashHits}, P: {Piece}, S: {Start}, E: {End}";
        }
    }
}
