using System.ComponentModel.DataAnnotations;

namespace GodelTech.CodeReview.Orchestrator.Model
{
    public class VolumesManifest
    {
        private const string LinuxSourcesPath = "/src";
        private const string WindowsSourcePath = "C:\\src";
        private const string LinuxArtifactsPath = "/artifacts";
        private const string WindowsArtifactsPath = "C:\\artifacts";
        private const string LinuxImportsPath = "/imports";
        private const string WindowsImportsPath = "C:\\imports";

        private string _sources;
        private string _artifacts;
        private string _imports;

        public bool UseWindowsDefaults { get; set; }

        [Required]
        [MaxLength(Constants.MaxPathLength)]
        public string Sources
        {
            get => _sources ?? (UseWindowsDefaults ? WindowsSourcePath : LinuxSourcesPath);
            set => _sources = value;
        }

        [Required]
        [MaxLength(Constants.MaxPathLength)]
        public string Artifacts
        {
            get => _artifacts ?? (UseWindowsDefaults ? WindowsArtifactsPath : LinuxArtifactsPath);
            set => _artifacts = value;
        }

        [Required]
        [MaxLength(Constants.MaxPathLength)]
        public string Imports
        {
            get => _imports ?? (UseWindowsDefaults ? WindowsImportsPath : LinuxImportsPath);
            set => _imports = value;
        }
    }
}