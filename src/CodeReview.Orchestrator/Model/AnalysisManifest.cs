using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GodelTech.CodeReview.Orchestrator.Model
{
    public class AnalysisManifest
    {
        [Required]
        public ImportedDataSettings Imports { get; set; } = new();

        [Required]
        public SourcesDataSettings Sources { get; set; } = new();

        [Required] 
        public ArtifactsSettings Artifacts { get; set; } = new();

        public Dictionary<string, Volume> Volumes { get; set; } = new();

        [Required]
        public Dictionary<string, string> Variables { get; set; } = new();

        [Required]
        public Dictionary<string, ActivityManifest> Activities { get; set; } = new();
    }

    public class Volume
    {
        public bool ReadOnly { get; set; } = true;
        
        [MaxLength(Constants.MaxPathLength)]
        public string TargetFolder { get; set; }
        
        [MaxLength(Constants.MaxPathLength)]
        public string FolderToOutput { get; set; }
        
        [MaxLength(Constants.MaxPathLength)]
        public string FolderToImport { get; set; }
    }
}
