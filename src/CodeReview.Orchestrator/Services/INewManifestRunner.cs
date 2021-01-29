using System.Threading.Tasks;

namespace CodeReview.Orchestrator.Services
{
    public interface INewManifestRunner
    {
        Task<int> RunAsync(string filePath);
    }
}