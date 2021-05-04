using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Options;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public interface IExtractMetadataCommand
    {
        Task<int> ExecuteAsync(ExtractMetadataOptions options);
    }
}