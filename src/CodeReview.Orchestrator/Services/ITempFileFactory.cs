namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface ITempFileFactory
    {
        TempFile Create();
        TempFile Create(string ext);
    }
}