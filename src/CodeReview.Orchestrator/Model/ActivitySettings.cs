using System.ComponentModel.DataAnnotations;

namespace ReviewItEasy.Orchestrator.Model
{
    public class ActivitySettings
    {
        [Required]
        [Range(0, int.MaxValue)]
        public long WaitTimeoutSeconds { get; set; } = 900;
    }
}