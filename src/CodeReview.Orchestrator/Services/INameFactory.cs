namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface INameFactory
    {
        string CreateVolumeName(string prefix);
    }
}