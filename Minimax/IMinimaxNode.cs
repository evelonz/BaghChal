using System;
using System.Collections.Generic;
using System.Text;

namespace Minimax
{
    /// <summary>
    /// TGameMove is the instructions needed for the game to reach the next desired state.
    /// </summary>
    public interface IMinimaxNode//<TGameMove>
    {
        IEnumerable<IMinimaxNode/*<TGameMove>*/> GetChildren();
        int Evaluate();
        bool IsTerminal();
        //TGameMove GetMove();
    }
}
