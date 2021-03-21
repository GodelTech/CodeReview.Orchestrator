using System.ComponentModel.DataAnnotations;

namespace GodelTech.CodeReview.Orchestrator.Model
{
    public class SourcesDataSettings
    {
        [Required]
        [MaxLength(Constants.MaxPathLength)]
        public string FolderPath { get; set; } = "./src";
    }
}