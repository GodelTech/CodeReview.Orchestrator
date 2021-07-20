using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Activities;
using GodelTech.CodeReview.Orchestrator.Model;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IDockerVolumeImporter
    {
        Task ImportVolumesAsync(IProcessingContext context, params Volume[] volumes);
    }
}