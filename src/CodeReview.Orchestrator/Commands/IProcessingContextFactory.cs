namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public interface IProcessingContextFactory
    {
        IProcessingContext Create();
    }
}