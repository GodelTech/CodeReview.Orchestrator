using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public interface IProcessingContext : IAsyncDisposable
    {
        IReadOnlyDictionary<string, string> Volumes { get; }

        Task InitializeAsync(ICollection<string> volumes);
        
        string ResolvePath(string relativeOrAbsolutePath);
        Task CleanUpVolumesAsync();
    }
}