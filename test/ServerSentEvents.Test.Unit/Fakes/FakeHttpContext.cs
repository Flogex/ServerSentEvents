using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace ServerSentEvents.Test.Unit.Fakes
{
    internal class FakeHttpContext : HttpContext
    {
        private readonly CancellationTokenSource _abortTokenSource;

        public FakeHttpContext()
        {
            var request = new FakeHttpRequest
            {
                OverridableHttpContext = this
            };
            Request = request;

            var response = new FakeHttpResponse
            {
                OverridableHttpContext = this
            };
            Response = response;

            _abortTokenSource = new CancellationTokenSource();
            RequestAborted = _abortTokenSource.Token;
        }

        public override ConnectionInfo Connection { get; }

        public override IFeatureCollection Features { get; }

        public override IDictionary<object, object> Items { get; set; }

        public override HttpRequest Request { get; }

        public override CancellationToken RequestAborted { get; set; }

        public override IServiceProvider RequestServices { get; set; }

        public override HttpResponse Response { get; }

        public override ISession Session { get; set; }

        public override string TraceIdentifier { get; set; }

        public override ClaimsPrincipal User { get; set; }

        public override WebSocketManager WebSockets { get; }

        public override void Abort()
            => _abortTokenSource.Cancel();

        public static HttpContext NewHttpContext()
            => new FakeHttpContext();
    }
}
