using System.Threading.Tasks;

namespace ReviewItEasy.Orchestrator.Commands
{
    public interface IActivity
    {
        Task<bool> ExecuteAsync(IProcessingContext context);
    }
}