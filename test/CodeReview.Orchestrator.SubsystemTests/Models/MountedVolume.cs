namespace CodeReview.Orchestrator.SubsystemTests.Models
{
    public class MountedVolume
    {
        public string Type { get; set; } = "volume";
        public string Source { get; set; }
        public string Target { get; set; }
    }
}