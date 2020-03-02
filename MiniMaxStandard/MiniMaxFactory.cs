using System;
using System.Collections.Generic;
using System.Text;

namespace MiniMaxStandard
{
    public static class MiniMaxFactory
    {
        public static IMinimaxRunner<TGameMove> GetInstance<TGameMove>(bool AlphaBeta = false) where TGameMove : IGameMove, new()
        {
            return AlphaBeta ? (IMinimaxRunner<TGameMove>)new AlphaBetaPruning<TGameMove>() : new Minimax<TGameMove>();
        }

        public static IMinimaxParallelRunner<TGameMove> GetParallelInstance<TGameMove>() where TGameMove : IGameMove, new()
        {
            return new AlphaBetaPruningParallel<TGameMove>();
        }
    }
}
