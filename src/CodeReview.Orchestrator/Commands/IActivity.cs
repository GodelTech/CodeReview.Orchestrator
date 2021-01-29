using System.Threading.Tasks;

namespace CodeReview.Orchestrator.Commands
{
    public interface IActivity
    {
        Task<bool> ExecuteAsync(IProcessingContext context);
    }
}