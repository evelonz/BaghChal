using System.Threading;

namespace MiniMaxStandard
{
    public interface IMinimaxParallelRunner<TGameMove> where TGameMove : IGameMove, new()
    {
        int EndNodesChecked { get; }

        TGameMove Run(IMinimaxNode<TGameMove> node, int depth, bool maximizing, int millisecondsTimeout, int degreeOfParallelism);
    }
}
