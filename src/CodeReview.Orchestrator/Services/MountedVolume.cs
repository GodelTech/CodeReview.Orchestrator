namespace ReviewItEasy.Orchestrator.Services
{
    public class MountedVolume
    {
        public bool IsBindMount { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public bool IsReadOnly { get; set; }
    }
}