using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MiniMaxStandard
{
    public class AlphaBetaPruningParallel<TGameMove> : IMinimaxParallelRunner<TGameMove> where TGameMove : IGameMove, new()
    {
        public int EndNodesChecked { get; private set; } = 0;

        private delegate TGameMove MinOrMaximazing(int newScore, TGameMove bestMove, TGameMove newMove);
        private readonly MinOrMaximazing MaxScore = (newScore, bestMove, newMove) => { if (newScore > bestMove.Score) { newMove.Score = newScore; return newMove; } return bestMove; };
        private readonly MinOrMaximazing MinScore = (newScore, bestMove, newMove) => { if (newScore < bestMove.Score) { newMove.Score = newScore; return newMove; } return bestMove; };

        private delegate (int a, int b) UpdateAlphaBeta(int score, int bestMove, int alpha, int beta);
        private readonly UpdateAlphaBeta Maximizing = (score, bestMove, alpha, beta) => { return (Math.Max(score, bestMove), beta); };
        private readonly UpdateAlphaBeta Minimizing = (score, bestMove, alpha, beta) => { return (alpha, Math.Min(score, bestMove)); };

        private CancellationTokenSource _timeoutTokenSource;
        private CancellationToken _timeoutToken;
        private int _degreeOfParallelism;
        private int _taskCount;

        public TGameMove Run(IMinimaxNode<TGameMove> node, int depth, bool maximizing, int millisecondsTimeout, int degreeOfParallelism)
        {
            _timeoutTokenSource = new CancellationTokenSource();
            _timeoutToken = _timeoutTokenSource.Token;
            _degreeOfParallelism = degreeOfParallelism;

            // Start the timeout timer.  Done using a dedicated thread to minimize delay 
            // in cancellation due to lack of threads in the pool to run the callback.
            var timeoutTask = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(millisecondsTimeout);
                _timeoutTokenSource.Cancel();
            }, TaskCreationOptions.LongRunning);

            return RunInternal(node, depth, maximizing, int.MinValue, int.MaxValue, CancellationToken.None);
        }

        private TGameMove RunInternal(IMinimaxNode<TGameMove> node, int depth, bool maximizing, int alpha, int beta, CancellationToken cancellationToken)
        {
            if (depth == 0 || node.IsTerminal() || _timeoutToken.IsCancellationRequested)
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

            Queue<Task> workers = new Queue<Task>();
            object bigLock = new object();
            CancellationTokenSource ChildrensCancellationTokenSource = new CancellationTokenSource();

            var childNodes = node.GetChildren();

            foreach (var child in childNodes)
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    ChildrensCancellationTokenSource.Cancel();
                    break;
                }

                var threadSafeMoveReference = child;
                if (_taskCount < _degreeOfParallelism && depth - 1 != 0)
                {
                    Interlocked.Increment(ref _taskCount);
                    workers.Enqueue(Task.Factory.StartNew(() =>
                    {
                        var nextMoveThread = RunInternal(threadSafeMoveReference, depth - 1, !maximizing, alpha, beta, ChildrensCancellationTokenSource.Token);
                        lock (bigLock)
                        {
                            bestMove = MinOrMax(nextMoveThread.Score, bestMove, child.GetMove());
                            (alpha, beta) = updateAlphaBeta(nextMoveThread.Score, bestMove.Score, alpha, beta);

                            if (alpha >= beta)
                                ChildrensCancellationTokenSource.Cancel();
                        }
                        Interlocked.Decrement(ref _taskCount);
                    }));
                }
                else
                {
                    bool isPruning = false;
                    var nextMoveLocal = RunInternal(threadSafeMoveReference, depth - 1, !maximizing, alpha, beta, ChildrensCancellationTokenSource.Token);

                    // If there are no tasks, no need to lock.
                    bool lockTaken = false;
                    try
                    {
                        if (workers.Count > 0) Monitor.Enter(bigLock, ref lockTaken);

                        bestMove = MinOrMax(nextMoveLocal.Score, bestMove, child.GetMove());
                        (alpha, beta) = updateAlphaBeta(nextMoveLocal.Score, bestMove.Score, alpha, beta);

                        if (alpha >= beta)
                            isPruning = true;
                    }
                    finally { if (lockTaken) Monitor.Exit(bigLock); }

                    if (isPruning)
                    {
                        ChildrensCancellationTokenSource.Cancel();
                        break;
                    }
                }
            }

            Task.WaitAll(workers.ToArray());

            return bestMove;
        }
    }
}
