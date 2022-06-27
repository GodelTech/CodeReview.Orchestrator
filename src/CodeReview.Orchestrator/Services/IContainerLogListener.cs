using System;
using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IContainerLogListener : IAsyncDisposable
    {
        Task CloseAsync();
    }
}