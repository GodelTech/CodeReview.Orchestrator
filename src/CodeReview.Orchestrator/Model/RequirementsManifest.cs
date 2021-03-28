using System;

namespace GodelTech.CodeReview.Orchestrator.Model
{
    public class RequirementsManifest
    {
        public OperatingSystemType? Os { get; set; }
        public string[] Features { get; set; } = Array.Empty<string>();
    }
}