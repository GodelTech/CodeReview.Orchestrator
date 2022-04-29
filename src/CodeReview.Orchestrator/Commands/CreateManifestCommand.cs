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
                Volumes = new()
                {
                    ["imports"] = new()
                    {
                        ReadOnly = false,
                        TargetFolder = "/imports",
                        FolderToImport = "./imports"
                    },
                    ["sources"] = new()
                    {
                        ReadOnly = true,
                        TargetFolder = "/src",
                        FolderToImport = "./src"
                    },
                    ["artifacts"] = new()
                    {
                        ReadOnly = false,
                        TargetFolder = "/artifacts",
                        FolderToOutput = "./artifacts"
                    }
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
                        Settings = new()
                        {
                            WaitTimeoutSeconds = 3000
                        },
                        Volumes = new()
                        {
                            ["sources"] = new()
                            {
                                TargetFolder = "/sources"
                            }
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
                        Volumes = new()
                        {
                            ["sources"] = new() { TargetFolder = "C:\\src" },
                            ["imports"] = new() { TargetFolder = "C:\\imports" },
                            ["artifacts"] = new() { TargetFolder = "C:\\artifacts" }
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