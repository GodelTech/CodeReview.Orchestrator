using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GodelTech.CodeReview.Orchestrator.Model
{
    public class ActivityManifest
    {
        [Required]
        [MaxLength(Constants.MaxDockerImageNameLength)]
        public string Image { get; set; }

        [Required] 
        public Dictionary<string, string> Environment { get; set; } = new();

        public VolumeCollection Volumes { get; set; } = new();
        
        [Required]
        public ActivitySettings Settings { get; set; } = new();

        [Required]
        public RequirementsManifest Requirements { get; set; } = new();

        public string[] Command { get; set; }
    }
}