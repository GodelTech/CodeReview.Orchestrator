using System.Threading.Tasks;
using ReviewItEasy.Orchestrator.Model;

namespace ReviewItEasy.Orchestrator.Services
{
    public interface IAnalysisManifestProvider
    {
        Task<AnalysisManifest> GetAsync(string path);
    }
}