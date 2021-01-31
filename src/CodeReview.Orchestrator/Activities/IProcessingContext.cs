using System;
using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public interface IProcessingContext : IAsyncDisposable
    {
        string SourceCodeVolumeId { get; }
        string ArtifactsVolumeId { get; }
        string ImportsVolumeId { get; }
        Task InitializeAsync();
    }
}