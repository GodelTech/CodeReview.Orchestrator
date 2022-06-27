using System;
using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Utils
{
    public static class Disposable
    {
        public static readonly IAsyncDisposable AsyncEmpty = new AsyncDisposable();
        
        private class AsyncDisposable : IAsyncDisposable
        {
            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        }
    }
}