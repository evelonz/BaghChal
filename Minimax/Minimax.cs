using System;

namespace Minimax
{
    public class Minimax
    {
        private delegate int MinOrMaximazing(int score, int bestMove);
        private readonly MinOrMaximazing MaxScore = (score, bestMove) => { return Math.Max(score, bestMove); };
        private readonly MinOrMaximazing MinScore = (score, bestMove) => { return Math.Min(score, bestMove); };
        
        public int Run(IMinimaxNode node, int depth, bool maximizing)
        {
            if (depth == 0 || node.IsTerminal())
            {
                var score = node.Evaluate();
                return score;
            }

            int bestMove = maximizing ? int.MinValue : int.MaxValue;
            var MinOrMax = maximizing ? MaxScore : MinScore;

            var childNodes = node.GetChildren();

            foreach (var child in childNodes)
            {
                var score = Run(child, depth - 1, !maximizing);
                bestMove = MinOrMax(score, bestMove);
            }

            return bestMove;
        }
    }
}
