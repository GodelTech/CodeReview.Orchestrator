using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public interface IActivityExecutor
    {
        Task<ExecutionResult> ExecuteAsync(ActivityManifest manifest, bool readLogs, string srcVolumeId, string artifactsVolumeId, string importsVolumeId);
    }
}