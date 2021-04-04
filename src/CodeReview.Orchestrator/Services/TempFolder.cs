using System;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class TempFolder : IDisposable
    {
        private readonly Action _cleanUpAction;
        
        public string Path { get; }

        public TempFolder(string tempFolderPath, Action cleanUpAction)
        {
            _cleanUpAction = cleanUpAction ?? throw new ArgumentNullException(nameof(cleanUpAction));
            Path = tempFolderPath ?? throw new ArgumentNullException(nameof(tempFolderPath));
        }

        void IDisposable.Dispose()
        {
            _cleanUpAction();
        }
    }
}