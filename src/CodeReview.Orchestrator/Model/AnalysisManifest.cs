using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GodelTech.CodeReview.Orchestrator.Model
{
    public class AnalysisManifest
    {
        [Required]
        public ImportedDataSettings Imports { get; set; } = new();

        [Required] 
        public ArtifactsSettings Artifacts { get; set; } = new();

        [Required]
        public Dictionary<string, string> Variables { get; set; } = new();

        [Required]
        public Dictionary<string, ActivityManifest> Activities { get; set; } = new();
    }
}
