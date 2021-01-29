﻿using System;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class NewManifestRunner : INewManifestRunner
    {
        private readonly IFileService _fileService;
        private readonly IYamlSerializer _yamlSerializer;

        public NewManifestRunner(IFileService fileService, IYamlSerializer yamlSerializer)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _yamlSerializer = yamlSerializer ?? throw new ArgumentNullException(nameof(yamlSerializer));
        }

        public async Task<int> RunAsync(string filePath)
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
                PreProcessors = new ()
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
                    }
                },

                Analyzers = new()
                {
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
                },
                PostProcessors = new()
                {
                    ["git"] = new()
                    {
                        Environment = new()
                        {
                            ["MY_VAR_1"] = "Value1",
                            ["MY_VAR_2"] = "Value2"
                        },
                        Image = "dragon/persister",
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
                },
            };

            var yaml = _yamlSerializer.Serialize(manifest);

            await _fileService.WriteAllTextAsync(filePath, yaml);

            return Constants.SuccessExitCode;
        }
    }
}