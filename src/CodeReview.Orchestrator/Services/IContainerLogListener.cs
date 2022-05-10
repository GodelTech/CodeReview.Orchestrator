namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IContainerLogListener
    {
        void StartListening();
        void StopListening();
    }
}