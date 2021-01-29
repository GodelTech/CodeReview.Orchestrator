using System;
using System.Threading.Tasks;

namespace CodeReview.Orchestrator.Commands
{
    public interface IProcessingContext : IAsyncDisposable
    {
        string SourceCodeVolumeId { get; }
        string ArtifactsVolumeId { get; }
        string ImportsVolumeId { get; }
        Task InitializeAsync();
    }
}