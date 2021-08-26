using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public interface IProcessingContext : IAsyncDisposable
    {
        public IEnumerable<Volume> Volumes { get; }

        Task InitializeAsync(ICollection<Volume> volumes);
        string ResolvePath(string relativeOrAbsolutePath);
        string ResolveVolumeId(string volumeName);
        Task CleanUpVolumesAsync();
    }
}