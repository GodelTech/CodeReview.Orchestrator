using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GodelTech.CodeReview.Orchestrator.Model
{
    public class AnalysisManifest
    {
        public VolumeCollection Volumes { get; set; } = new();

        [Required] 
        public Dictionary<string, string> Variables { get; set; } = new();

        [Required] 
        public Dictionary<string, ActivityManifest> Activities { get; set; } = new();
    }
}