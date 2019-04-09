using MiniMaxStandard;
using System;

namespace MiniMaxStandard
{
    public class Minimax<TGameMove> : IMinimaxRunner<TGameMove> where TGameMove : IGameMove, new()
    {
        public int EndNodesChecked { get; set; } = 0;

        private delegate TGameMove MinOrMaximazing(int newScore, TGameMove bestMove, TGameMove newMove);
        private readonly MinOrMaximazing MaxScore = (newScore, bestMove, newMove) => { if (newScore > bestMove.Score) { newMove.Score = newScore; return newMove; } return bestMove; };
        private readonly MinOrMaximazing MinScore = (newScore, bestMove, newMove) => { if (newScore < bestMove.Score) { newMove.Score = newScore; return newMove; } return bestMove; };

        public TGameMove Run(IMinimaxNode<TGameMove> node, int depth, bool maximizing) 
        {
            if (depth == 0 || node.IsTerminal())
            {
                //var score = node.Evaluate();
                EndNodesChecked++;
                return node.GetMove();
            }

            var bestMove = new TGameMove();
            bestMove.Score = maximizing ? int.MinValue : int.MaxValue;
            var MinOrMax = maximizing ? MaxScore : MinScore;

            var childNodes = node.GetChildren();

            foreach (var child in childNodes)
            {
                var GameMove = Run(child, depth - 1, !maximizing);
                // Should not return GameMove. Compare GameMove score, but set move to the childs move.
                bestMove = MinOrMax(GameMove.Score, bestMove, child.GetMove());
            }

            return bestMove;
        }
    }
}
