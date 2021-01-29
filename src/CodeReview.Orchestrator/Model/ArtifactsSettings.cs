﻿using System.ComponentModel.DataAnnotations;

namespace CodeReview.Orchestrator.Model
{
    public class ArtifactsSettings
    {
        [Required]
        [MaxLength(Constants.MaxPathLength)]
        public string FolderPath { get; set; } = "./artifacts";

        public bool ExportOnCompletion { get; set; } = false;
    }
}