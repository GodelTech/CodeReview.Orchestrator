using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IManifestProvider
    {
        Task<T> GetAsync<T>(string path)
            where T : class;
    }
}