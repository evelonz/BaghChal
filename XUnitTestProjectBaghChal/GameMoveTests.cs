using BaghChal;
using System;
using Xunit;
using FluentAssertions;

namespace XUnitTestProjectBaghChal
{
    public class UnitTest1
    {
        [Theory]
        [InlineData(1, 1, 8)]
        [InlineData(5, 1, 12)]
        [InlineData(1, 5, 36)]
        [InlineData(5, 5, 40)]
        [InlineData(3, 3, 24)]
        [InlineData(0, 0, 0)]
        [InlineData(-1, -1, -8)]
        public void TranslateToBoardIndexTest(int x, int y, int expected)
        {
            var position = GameBoard.TranslateToBoardIndex((x, y));
            position.Should().Be(expected);
        }
    }
}
