using System.Collections.Generic;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IDockerEngineProvider
    {
        Task InitializeAsync(string manifestFilePath);
        DockerEngine Find(RequirementsManifest requirements);
        IEnumerable<DockerEngine> ListAll();
    }
}