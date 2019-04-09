using System;

namespace Minimax
{
    public class AlphaBetaPruning
    {
        public int EndNodesChecked = 0;

        private delegate int MinOrMaximazing(int score, int bestMove);
        private readonly MinOrMaximazing MaxScore = (score, bestMove) => { return Math.Max(score, bestMove); };
        private readonly MinOrMaximazing MinScore = (score, bestMove) => { return Math.Min(score, bestMove); };

        private delegate (int a, int b) UpdateAlphaBeta(int score, int bestMove, int alpha, int beta);
        private readonly UpdateAlphaBeta Maximizing = (score, bestMove, alpha, beta) => { return (Math.Max(score, bestMove), beta); };
        private readonly UpdateAlphaBeta Minimizing = (score, bestMove, alpha, beta) => { return (alpha, Math.Min(score, bestMove)); };

        public int Run(IMinimaxNode node, int depth, bool maximizing, int alpha, int beta)
        {
            if (depth == 0 || node.IsTerminal())
            {
                EndNodesChecked++;
                var score = node.Evaluate();
                return score;
            }

            int bestMove = maximizing ? int.MinValue : int.MaxValue;
            var MinOrMax = maximizing ? MaxScore : MinScore;
            var updateAlphaBeta = maximizing ? Maximizing : Minimizing;

            var childNodes = node.GetChildren();

            foreach (var child in childNodes)
            {
                var score = Run(child, depth - 1, !maximizing, alpha, beta);
                bestMove = MinOrMax(score, bestMove);
                (alpha, beta) = updateAlphaBeta(score, bestMove, alpha, beta);

                if (alpha >= beta)
                    break;
            }

            return bestMove;
        }
    }
}
