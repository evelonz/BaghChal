using MiniMaxStandard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinimaxTests
{

    public class MockIGameMove : IGameMove
    {
        public int Score { get; set; }
    }

    /// <summary>
    /// Base for creating fake <see cref="IMinimaxNode"/> implementations.
    /// Does not have to hold any real game logic, only create node trees
    /// of known outcome to test the algorithm.
    /// </summary>
    public abstract class MockIMinimaxNode : IMinimaxNode<MockIGameMove>
    {
        protected readonly int Value;

        protected MockIMinimaxNode(int value)
        {
            Value = value;
        }

        public int Evaluate()
        {
            return Value;
        }

        public abstract IEnumerable<IMinimaxNode<MockIGameMove>> GetChildren();

        public MockIGameMove GetMove()
        {
            return new MockIGameMove
            {
                Score = Evaluate()
            };
        }

        public abstract bool IsTerminal();

    }

    /// <summary>
    /// Fake interface implementation. Meant to test at least 3 leavels.
    /// Expected result: 4.
    /// Simulates the following game tree:
    ///     0
    ///   1   2
    ///  5 3 4 7
    /// </summary>
    public class SimpleMockIMinimaxNode : MockIMinimaxNode
    {
        public SimpleMockIMinimaxNode() : base(0) { }

        private SimpleMockIMinimaxNode(int value) : base(value) { }

        public override IEnumerable<IMinimaxNode<MockIGameMove>> GetChildren()
        {
            switch (Value)
            {
                case 0: return new[] { new SimpleMockIMinimaxNode(1), new SimpleMockIMinimaxNode(2) };
                case 1:
                    return new[] { new SimpleMockIMinimaxNode(5), new SimpleMockIMinimaxNode(3) };
                case 2:
                    return new[] { new SimpleMockIMinimaxNode(7), new SimpleMockIMinimaxNode(4) };
                default:
                    return new MockIMinimaxNode[] { };
            }
        }

        public override bool IsTerminal()
        {
            var finalStates = new int[] { 3, 4, 5, 7 };
            return finalStates.Contains(Value);
        }
    }

    /// <summary>
    /// Fake interface implementation. Meant to test at least 4 leavels.
    /// Made to test cutof for minimax. Should only check 5 of the leaf nodes.
    /// Expected result: 5.
    /// Simulates the following game tree:
    ///           -1
    ///      -2        -3
    ///   -4   -5    -6   -7
    ///  3  5 6  9  1  2 0  4
    /// </summary>
    public class Simple2MockIMinimaxNode : MockIMinimaxNode
    {
        public Simple2MockIMinimaxNode() : base(-1) { }

        private Simple2MockIMinimaxNode(int value) : base(value) { }

        public override IEnumerable<IMinimaxNode<MockIGameMove>> GetChildren()
        {
            switch (Value)
            {
                case -1: return new[] { new Simple2MockIMinimaxNode(-2), new Simple2MockIMinimaxNode(-3) };

                case -2:
                    return new[] { new Simple2MockIMinimaxNode(-4), new Simple2MockIMinimaxNode(-5) };
                case -3:
                    return new[] { new Simple2MockIMinimaxNode(-6), new Simple2MockIMinimaxNode(-7) };

                case -4:
                    return new[] { new Simple2MockIMinimaxNode(3), new Simple2MockIMinimaxNode(5) };
                case -5:
                    return new[] { new Simple2MockIMinimaxNode(6), new Simple2MockIMinimaxNode(9) };
                case -6:
                    return new[] { new Simple2MockIMinimaxNode(1), new Simple2MockIMinimaxNode(2) };
                case -7:
                    return new[] { new Simple2MockIMinimaxNode(0), new Simple2MockIMinimaxNode(4) };
                default:
                    return new MockIMinimaxNode[] { };
            }
        }

        public override bool IsTerminal()
        {
            var finalStates = new int[] { 3, 5, 6, 9, 1, 2, 0, 4 };
            return finalStates.Contains(Value);
        }
    }

    /// <summary>
    /// Used to performance test the Minimax algorithm itself.
    /// Endless creation of nodes of a fixed value. Never returns an end state.
    /// </summary>
    public class InfinteMockIMinimaxNode : MockIMinimaxNode
    {
        public InfinteMockIMinimaxNode() : base(0) { }

        private InfinteMockIMinimaxNode(int value) : base(value) { }

        public override IEnumerable<IMinimaxNode<MockIGameMove>> GetChildren()
        {
            return new MockIMinimaxNode[] { new InfinteMockIMinimaxNode(1) };
        }

        public override bool IsTerminal()
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a given number of levels with a given number of child nodes.
    /// All nodes have the same evaluation.
    /// </summary>
    public class FiniteMockIMinimaxNode : MockIMinimaxNode
    {
        private readonly int NumberOfLevels;
        private readonly int NumberOfChildrenPerLevel;

        public FiniteMockIMinimaxNode(int numberOfLevels, int numberOfChildrenPerLevel) : base(0)
        {
            NumberOfLevels = numberOfLevels;
            NumberOfChildrenPerLevel = numberOfChildrenPerLevel;
        }

        public override IEnumerable<IMinimaxNode<MockIGameMove>> GetChildren()
        {
            var ret = new MockIMinimaxNode[NumberOfChildrenPerLevel];
            for (int i = 0; i < NumberOfChildrenPerLevel; i++)
            {
                ret[i] = new FiniteMockIMinimaxNode(NumberOfLevels - 1, NumberOfChildrenPerLevel);
            }
            return ret;
        }

        public override bool IsTerminal()
        {
            return NumberOfLevels == 0;
        }
    }
}
