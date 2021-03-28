using System;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Options;
using GodelTech.CodeReview.Orchestrator.Services;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public class CreateManifestCommand : ICreateManifestCommand
    {
        private readonly IFileService _fileService;
        private readonly IYamlSerializer _yamlSerializer;

        public CreateManifestCommand(IFileService fileService, IYamlSerializer yamlSerializer)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _yamlSerializer = yamlSerializer ?? throw new ArgumentNullException(nameof(yamlSerializer));
        }

        public async Task<int> ExecuteAsync(NewAnalysisManifestOptions options)
        {
            if (options == null) 
                throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.File))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(options.File));

            var manifest = new AnalysisManifest
            {
                Imports = new ()
                {
                    FolderPath = "./imports"
                },
                
                Sources = new ()
                {
                    FolderPath = "./src"
                },
                
                Artifacts = new ()
                {
                    FolderPath = "./artifacts",
                    ExportOnCompletion = true
                },
                
                Variables = new()
                {
                    ["MY_VAR_1"] = "Value1",
                    ["MY_VAR_2"] = "Value2"
                },
                Activities = new ()
                {
                    ["git"] = new()
                    {
                        Environment = new()
                        {
                            ["MY_VAR_1"] = "Value1",
                            ["MY_VAR_2"] = "Value2"
                        },
                        Image = "dragon/jetbrains",
                        Volumes = new()
                        {
                            UseWindowsDefaults = false,
                            Artifacts = "/artifacts",
                            Sources = "/src",
                            Imports = "/imports"
                        },
                        Settings = new()
                        {
                            WaitTimeoutSeconds = 3000
                        },
                        Requirements = new()
                        {
                            Os = OperatingSystemType.Linux,
                            Features = new[]
                            {
                                "feature1"
                            }
                        },
                        Command = new []
                        {
                            "mkdir",
                            "/mydir"
                        }
                    },
                    ["resharper"] = new()
                    {
                        Environment = new()
                        {
                            ["MY_VAR_1"] = "Value1",
                            ["MY_VAR_2"] = "Value2"
                        },
                        Image = "dragon/jetbrains",
                        Volumes = new()
                        {
                            Artifacts = "/artifacts",
                            Sources = "/src",
                            Imports = "/imports"
                        },
                        Settings = new()
                        {
                            WaitTimeoutSeconds = 3000
                        },
                        Requirements = new()
                        {
                            Os = OperatingSystemType.Windows,
                            Features = new[]
                            {
                                "feature2", 
                                "feature3"
                            }
                        },

                        Command = new[]
                        {
                            "mkdir",
                            "/mydir"
                        }
                    },
                    ["roslyn"] = new()
                    {
                        Environment = new()
                        {
                            ["MY_VAR_1"] = "Value1",
                            ["MY_VAR_2"] = "Value2"
                        },
                        Image = "dragon/roslyn",
                        Volumes = new()
                        {
                            UseWindowsDefaults = true
                        },
                        Settings = new()
                        {
                            WaitTimeoutSeconds = 3000
                        },
                        Requirements = null,
                        Command = new[]
                        {
                            "mkdir",
                            "/mydir"
                        }
                    }
                }
            };

            var yaml = _yamlSerializer.Serialize(manifest);

            await _fileService.WriteAllTextAsync(options.File, yaml);

            return Constants.SuccessExitCode;
        }
    }
}