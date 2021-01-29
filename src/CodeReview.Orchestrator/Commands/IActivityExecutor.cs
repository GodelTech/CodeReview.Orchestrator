using System.Threading.Tasks;
using CodeReview.Orchestrator.Model;

namespace CodeReview.Orchestrator.Commands
{
    public interface IActivityExecutor
    {
        Task<ExecutionResult> ExecuteAsync(ActivityManifest manifest, bool readLogs, string srcVolumeId, string artifactsVolumeId, string importsVolumeId);
    }
}