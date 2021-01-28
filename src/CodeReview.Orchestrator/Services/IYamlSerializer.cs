namespace ReviewItEasy.Orchestrator.Services
{
    public interface IYamlSerializer
    {
        T Deserialize<T>(string content);
        string Serialize(object person);
    }
}