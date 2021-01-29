using System.Threading.Tasks;
using CodeReview.Orchestrator.Model;

namespace CodeReview.Orchestrator.Services
{
    public interface IAnalysisManifestProvider
    {
        Task<AnalysisManifest> GetAsync(string path);
    }
}