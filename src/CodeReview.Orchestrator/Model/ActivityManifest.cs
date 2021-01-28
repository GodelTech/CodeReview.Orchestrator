using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReviewItEasy.Orchestrator.Model
{
    public class ActivityManifest
    {
        [Required]
        [MaxLength(Constants.MaxDockerImageNameLength)]
        public string Image { get; set; }

        [Required] 
        public Dictionary<string, string> Environment { get; set; } = new Dictionary<string, string>();
        
        [Required]
        public VolumesManifest Volumes { get; set; } = new VolumesManifest();
        
        [Required]
        public ActivitySettings Settings { get; set; } = new ActivitySettings();

        public string[] Command { get; set; }
    }
}