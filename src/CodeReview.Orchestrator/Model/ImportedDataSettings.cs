using System.ComponentModel.DataAnnotations;

namespace CodeReview.Orchestrator.Model
{
    public class ImportedDataSettings
    {
        [Required]
        [MaxLength(Constants.MaxPathLength)]
        public string FolderPath { get; set; } = "./imports";
    }
}