using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public interface IActivity
    {
        Task<bool> ExecuteAsync(IProcessingContext context);
    }
}