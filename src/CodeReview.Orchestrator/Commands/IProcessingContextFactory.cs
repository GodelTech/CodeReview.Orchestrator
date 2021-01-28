namespace ReviewItEasy.Orchestrator.Commands
{
    public interface IProcessingContextFactory
    {
        IProcessingContext Create();
    }
}