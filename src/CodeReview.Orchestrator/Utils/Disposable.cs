using System;
using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Utils
{
    public static class Disposable
    {
        public static readonly IAsyncDisposable AsyncEmpty = new AsyncDisposable();

        public static IDisposable CreateDisposableAction(Action action)
            => new DisposableAction(action);
        
        public static IDisposable CreateDisposableAction<T>(T value, Action<T> action)
            => new DisposableAction<T>(action, value);

        private class AsyncDisposable : IAsyncDisposable
        {
            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        }

        private sealed class DisposableAction : IDisposable
        {
            private readonly Action _action;

            public DisposableAction(Action action)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));
            }

            public void Dispose()
            {
                Dispose(true);
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _action();
                }
            }
        }

        public sealed class DisposableAction<T> : IDisposable
        {
            private readonly Action<T> _action;

            public DisposableAction(Action<T> action, T val)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));
                Value = val;
            }

            public T Value { get; }

            public void Dispose()
            {
                Dispose(true);
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _action(Value);
                }
            }
        }
    }
}