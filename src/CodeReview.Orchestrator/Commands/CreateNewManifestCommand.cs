using System;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Services;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public class CreateNewManifestCommand : ICreateNewManifestCommand
    {
        private readonly IFileService _fileService;
        private readonly IYamlSerializer _yamlSerializer;

        public CreateNewManifestCommand(IFileService fileService, IYamlSerializer yamlSerializer)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _yamlSerializer = yamlSerializer ?? throw new ArgumentNullException(nameof(yamlSerializer));
        }

        public async Task<int> ExecuteAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));

            var manifest = new AnalysisManifest
            {
                Imports = new ()
                {
                    FolderPath = "./imports"
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
                            Artifacts = "/artifacts",
                            Sources = "/src",
                            Imports = "/imports"
                        },
                        Settings = new()
                        {
                            WaitTimeoutSeconds = 3000
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
                            Artifacts = "/artifacts",
                            Sources = "/src",
                            Imports = "/imports"
                        },
                        Settings = new()
                        {
                            WaitTimeoutSeconds = 3000
                        },
                        Command = new[]
                        {
                            "mkdir",
                            "/mydir"
                        }
                    }
                }
            };

            var yaml = _yamlSerializer.Serialize(manifest);

            await _fileService.WriteAllTextAsync(filePath, yaml);

            return Constants.SuccessExitCode;
        }
    }
}