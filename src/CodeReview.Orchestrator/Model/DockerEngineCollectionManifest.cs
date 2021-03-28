using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GodelTech.CodeReview.Orchestrator.Model
{
    public class DockerEngineCollectionManifest
    {
        [Required]
        public string DefaultEngine { get; set; }
        
        [Required] 
        public Dictionary<string, DockerEngineManifest> Engines { get; set; } = new();
    }
}