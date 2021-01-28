namespace ReviewItEasy.Orchestrator.Services
{
    public interface IEnvironmentVariableValueProvider
    {
        string Get(string environmentVariableName);
    }
}