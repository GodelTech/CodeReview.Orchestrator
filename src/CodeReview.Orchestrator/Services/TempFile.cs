using System;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class TempFile : IDisposable
    {
        private readonly Action _cleanUpAction;

        public string Path { get; }

        public TempFile(string path, Action cleanUpAction)
        {
            _cleanUpAction = cleanUpAction ?? throw new ArgumentNullException(nameof(cleanUpAction));
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public void Dispose()
        {
            _cleanUpAction();
        }
    }
}