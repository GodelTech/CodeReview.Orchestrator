namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public interface IProcessingContextFactory
    {
        IProcessingContext Create(string manifestFilePath);
    }
}