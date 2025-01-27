﻿using System.Threading.Tasks;
using FluentAssertions;
using ServerSentEvents.Test.Unit.Fakes;
using Xunit;

namespace ServerSentEvents.Test.Unit
{
    public class EventTransmitterTests_SendComment
    {
        private async Task<string> GetResponseBodyAfterCommentBeingSent(string comment)
        {
            var sut = new EventTransmitter();
            var client = new FakeClient();

            await sut.SendComment(client, comment);

            return await client.ReadStreamFromStart();
        }

        [Fact]
        public async Task CommentShouldBePrefixedByColon()
        {
            var body = await GetResponseBodyAfterCommentBeingSent("Hello World");
            body.Should().Be(":Hello World\n\n");
        }

        [Fact]
        public async Task IfCommentStartsWithColon_ColonShouldBePreservedInBody()
        {
            var body = await GetResponseBodyAfterCommentBeingSent(":comment");
            body.Should().Be("::comment\n\n");
        }

        [Fact]
        public async Task IfCommentHasMultipleLines_BodyShouldContainMultipleComments()
        {
            var body = await GetResponseBodyAfterCommentBeingSent("line1\nline2");
            body.Should().Be(":line1\n:line2\n\n");
        }

        [Fact]
        public async Task ConsecutiveAndIrrelevantLineFeedsShouldBeIgnored()
        {
            var body = await GetResponseBodyAfterCommentBeingSent("\nline1\n\nline2\n");
            body.Should().Be(":line1\n:line2\n\n");
        }

        [Fact]
        public async Task IfCommentContaninsCRLF_CarriageReturnShouldBeIgnored()
        {
            var body = await GetResponseBodyAfterCommentBeingSent("line1\r\nline2");
            body.Should().Be(":line1\n:line2\n\n");
        }
    }
}
