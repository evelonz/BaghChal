using System;
using System.Collections.Generic;
using System.Text;

namespace MiniMaxStandard
{
    /// <summary>
    /// TGameMove is the instructions needed for the game to reach the next desired state.
    /// </summary>
    public interface IMinimaxNode<TGameMove> where TGameMove : IGameMove
    {
        IEnumerable<IMinimaxNode<TGameMove>> GetChildren();
        int Evaluate();
        bool IsTerminal();
        TGameMove GetMove();
    }

    public interface IGameMove
    {
        int Score { get; set; }
    }
}
