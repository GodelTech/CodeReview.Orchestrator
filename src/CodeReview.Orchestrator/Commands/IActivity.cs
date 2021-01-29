using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public interface IActivity
    {
        Task<bool> ExecuteAsync(IProcessingContext context);
    }
}