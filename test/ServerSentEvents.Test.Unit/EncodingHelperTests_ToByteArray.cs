using System;
using FluentAssertions;
using ServerSentEvents.Events;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class EncodingHelperTests_ToByteArray
    {
        [Fact]
        public void IfNumberIsSmallerZero_ArgumentExceptionShouldBeThrown()
        {
            var number = -1;
            Action act = () => number.ToByteArray();
            act.Should().Throw<ArgumentException>().WithMessage("*zero*")
               .And.ParamName.Should().Be("number");
        }

        [Theory]
        [InlineData(0, new byte[] { 48 })]
        [InlineData(1, new byte[] { 49 })]
        [InlineData(10, new byte[] { 49, 48 })]
        [InlineData(987, new byte[] { 57, 56, 55 })]
        [InlineData(23456, new byte[] { 50, 51, 52, 53, 54 })]
        public void ResultShouldBeAsExpected(int number, byte[] expected)
        {
            var result = number.ToByteArray();
            result.Should().BeEquivalentTo(expected);
        }
    }
}
