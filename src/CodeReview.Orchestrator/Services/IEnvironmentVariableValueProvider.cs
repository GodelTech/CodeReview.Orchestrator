namespace CodeReview.Orchestrator.Services
{
    public interface IEnvironmentVariableValueProvider
    {
        string Get(string environmentVariableName);
    }
}