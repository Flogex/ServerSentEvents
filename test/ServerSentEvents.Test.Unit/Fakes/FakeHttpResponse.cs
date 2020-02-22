using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ServerSentEvents.Test.Unit.Fakes
{
    internal class FakeHttpResponse : HttpResponse
    {
        private bool _hasStarted;

        public FakeHttpResponse()
        {
            Body = new MemoryStream();
            BodyWriter = PipeWriter.Create(Body);
            Headers = new HeaderDictionary();
        }

        public override Stream Body { get; set; }
        public override PipeWriter BodyWriter { get; }
        public override long? ContentLength { get; set; }
        public override string ContentType { get; set; }
        public override IResponseCookies Cookies => throw new NotImplementedException();
        public override bool HasStarted => _hasStarted;
        public override IHeaderDictionary Headers { get; }
        public override HttpContext HttpContext => throw new NotImplementedException();
        public override int StatusCode { get; set; }

        public override Task CompleteAsync() => base.CompleteAsync();
        public override void OnCompleted(Func<Task> callback) => base.OnCompleted(callback);
        public override void OnCompleted(Func<object, Task> callback, object state) => throw new NotImplementedException();
        public override void OnStarting(Func<Task> callback) => base.OnStarting(callback);
        public override void OnStarting(Func<object, Task> callback, object state) => throw new NotImplementedException();
        public override void Redirect(string location) => base.Redirect(location);
        public override void Redirect(string location, bool permanent) => throw new NotImplementedException();
        public override void RegisterForDispose(IDisposable disposable) => base.RegisterForDispose(disposable);
        public override void RegisterForDisposeAsync(IAsyncDisposable disposable) => base.RegisterForDisposeAsync(disposable);
        public override Task StartAsync(CancellationToken cancellationToken = default)
        {
            _hasStarted = true;
            return Task.CompletedTask;
        }
    }
}
