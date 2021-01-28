using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReviewItEasy.Orchestrator.Model
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
        public Dictionary<string, ActivityManifest> PreProcessors { get; set; } = new();

        [Required]
        public Dictionary<string, ActivityManifest> Analyzers { get; set; } = new();

        [Required]
        public Dictionary<string, ActivityManifest> PostProcessors { get; set; } = new();
    }
}
