using GodelTech.CodeReview.Orchestrator.Model;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class DockerEngine
    {
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public OperatingSystemType Os { get; set; }
        public string Url { get; set; }
        public string[] Features { get; set; }
        public string WorkerImage { get; set; }
    }
}