using System;

namespace MiniMaxStandard
{
    public class AlphaBetaPruning<TGameMove> : IMinimaxRunner<TGameMove> where TGameMove : IGameMove, new()
    {
        public int EndNodesChecked { get; set; } = 0;

        private delegate TGameMove MinOrMaximazing(int newScore, TGameMove bestMove, TGameMove newMove);
        private readonly MinOrMaximazing MaxScore = (newScore, bestMove, newMove) => { if (newScore > bestMove.Score) { newMove.Score = newScore; return newMove; } return bestMove; };
        private readonly MinOrMaximazing MinScore = (newScore, bestMove, newMove) => { if (newScore < bestMove.Score) { newMove.Score = newScore; return newMove; } return bestMove; };

        private delegate (int a, int b) UpdateAlphaBeta(int score, int bestMove, int alpha, int beta);
        private readonly UpdateAlphaBeta Maximizing = (score, bestMove, alpha, beta) => { return (Math.Max(score, bestMove), beta); };
        private readonly UpdateAlphaBeta Minimizing = (score, bestMove, alpha, beta) => { return (alpha, Math.Min(score, bestMove)); };

        public TGameMove Run(IMinimaxNode<TGameMove> node, int depth, bool maximizing)
        {
            return Run(node, depth, maximizing, int.MinValue, int.MaxValue);
        }

        private TGameMove Run(IMinimaxNode<TGameMove> node, int depth, bool maximizing, int alpha, int beta)
        {
            if (depth == 0 || node.IsTerminal())
            {
                EndNodesChecked++;
                var nextMove = node.GetMove();
                nextMove.Score = node.Evaluate();
                return nextMove;
            }

            var bestMove = new TGameMove();
            bestMove.Score = maximizing ? int.MinValue : int.MaxValue;
            var MinOrMax = maximizing ? MaxScore : MinScore;
            var updateAlphaBeta = maximizing ? Maximizing : Minimizing;

            var childNodes = node.GetChildren();

            foreach (var child in childNodes)
            {
                var nextMove = Run(child, depth - 1, !maximizing, alpha, beta);
                bestMove = MinOrMax(nextMove.Score, bestMove, child.GetMove());
                (alpha, beta) = updateAlphaBeta(nextMove.Score, bestMove.Score, alpha, beta);

                if (alpha >= beta)
                    break;
            }

            return bestMove;
        }

    }
}
