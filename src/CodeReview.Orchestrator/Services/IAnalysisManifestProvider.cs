using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IAnalysisManifestProvider
    {
        Task<AnalysisManifest> GetAsync(string path);
    }
}