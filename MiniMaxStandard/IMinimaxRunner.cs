
namespace MiniMaxStandard
{
    public interface IMinimaxRunner<TGameMove> where TGameMove : IGameMove, new()
    {
        int EndNodesChecked { get; set; }

        TGameMove Run(IMinimaxNode<TGameMove> node, int depth, bool maximizing);
    }
}
