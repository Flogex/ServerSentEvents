﻿using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerSentEvents.Events;

namespace ServerSentEvents.Test.Unit.Fakes
{
    internal class FakeEvent : IEvent
    {
        private static readonly byte[] _message = Encoding.UTF8.GetBytes("Hello World!");

        public async Task WriteToStream(
            Stream stream,
            CancellationToken cancellationToken = default)
            => await stream.WriteAsync(_message);
    }
}
