using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface INewManifestRunner
    {
        Task<int> RunAsync(string filePath);
    }
}