using System.ComponentModel.DataAnnotations;

namespace GodelTech.CodeReview.Orchestrator.Model
{
    public class DockerEngineManifest
    {
        public OperatingSystemType Os { get; set; }
        
        [Required]
        public string Url { get; set; }

        [Required]
        [MaxLength(Constants.MaxDockerImageNameLength)]
        public string WorkerImage { get; set; }

        public string[] Features { get; set; }
    }
}