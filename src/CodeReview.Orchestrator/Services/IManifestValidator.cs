namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IManifestValidator
    {
        bool IsValid(object manifest);
    }
}