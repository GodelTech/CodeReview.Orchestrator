using System.ComponentModel.DataAnnotations;

namespace GodelTech.CodeReview.Orchestrator.Model
{
    public class VolumesManifest
    {
        [Required]
        [MaxLength(Constants.MaxPathLength)]
        public string Sources { get; set; } = "/src";

        [Required]
        [MaxLength(Constants.MaxPathLength)]
        public string Artifacts { get; set; } = "/artifacts";

        [Required]
        [MaxLength(Constants.MaxPathLength)]
        public string Imports { get; set; } = "/imports";
    }
}