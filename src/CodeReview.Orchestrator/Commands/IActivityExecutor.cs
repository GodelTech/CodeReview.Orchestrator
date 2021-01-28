using System.Threading.Tasks;
using ReviewItEasy.Orchestrator.Model;

namespace ReviewItEasy.Orchestrator.Commands
{
    public interface IActivityExecutor
    {
        Task<ExecutionResult> ExecuteAsync(ActivityManifest manifest, bool readLogs, string srcVolumeId, string artifactsVolumeId, string importsVolumeId);
    }
}