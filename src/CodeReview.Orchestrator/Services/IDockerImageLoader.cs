using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Options;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IDockerImageLoader
    {
        Task LoadImagesAsync(ImageLoadBehavior behavior, AnalysisManifest manifest);
    }
}