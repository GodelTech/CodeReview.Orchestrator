using System;
using System.IO;
using CodeReview.Orchestrator.SubsystemTests.Actions;
using CodeReview.Orchestrator.SubsystemTests.Actions.Activities;
using CodeReview.Orchestrator.SubsystemTests.Actions.Docker;
using CodeReview.Orchestrator.SubsystemTests.Actions.Wiremock;
using CodeReview.Orchestrator.SubsystemTests.Expectations;
using CodeReview.Orchestrator.SubsystemTests.Models;
using CodeReview.Orchestrator.SubsystemTests.Utils;
using Xunit;
using GodelTech.StoryLine;

namespace CodeReview.Orchestrator.SubsystemTests.Tests
{
    public class RunTests : WiremockTestBase
    {
        private const string AlpineImage = "alpine";
        private const string ClocImage = "aldanial/cloc";
        private const string GitProviderImage = "godeltech/codereview.tools.gitprovider";

        private const string SingleImageManifestOutputName = nameof(RunTests) + "_single-image-manifest.yaml";
        private const string SeveralImageManifestOutputName = nameof(RunTests) + "_several-image-manifest.yaml";
        private const string EngineManifestOutputName = nameof(RunTests) + "_run_engines.yaml";

        private static readonly string SingleImageManifestOutputPath = FileHelper.GetOutputPath(SingleImageManifestOutputName);
        private static readonly string SeveralImageManifestOutputPath = FileHelper.GetOutputPath(SeveralImageManifestOutputName);
        private static readonly string EngineManifestOutputPath = FileHelper.GetOutputPath(EngineManifestOutputName);

        static RunTests()
        {
            const string singleImageManifestResourceName = "Run.single-image-manifest.yaml";
            const string severalImageManifestResourceName = "Run.several-image-manifest.yaml";
            const string engineManifestResourceName = "Run.engines.yaml";
            
            FileHelper.CreateDirectory("src");
            FileHelper.CreateDirectory("imports");
            
            FileHelper.CopyFromResource(singleImageManifestResourceName, SingleImageManifestOutputPath);
            FileHelper.CopyFromResource(severalImageManifestResourceName, SeveralImageManifestOutputPath);
            FileHelper.CopyFromResource(engineManifestResourceName, EngineManifestOutputPath);
        }
        
        [Fact]
        public void When_Exit_Present_And_Behavior_LoadIfMissingShould_Return_Ok()
        {
            Scenario.New()
                .Given()
                    .HasPerformed<StubDockerImageExist>(m => m.WithImage(AlpineImage))
                    .HasPerformed<StubDockerImageExist>(m => m.WithImage(GitProviderImage))
                    .HasPerformed<StubDockerPullImage>(m => m.WithImage(AlpineImage))
                    .HasPerformed<StubDockerPullImage>(m => m.WithImage(GitProviderImage))
                .When()
                    .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("run")
                    .WithCommandArg("-b", "LoadIfMissing")
                    .WithCommandArg("-f", SingleImageManifestOutputPath)
                    .WithCommandArg("-d", EngineManifestOutputPath)
                    .WithCommandArg("-e"))
                .Then()
                    .Expects<ProcessResult>(proc => proc
                    .ExitCode(Constants.ExitCode.Ok))
                .Run();
        }
        
        [Fact]
        public void When_Exit_Present_And_Images_Already_Exist_And_Behavior_LoadIfMissing_Should_Return_Ok()
        {
            Scenario.New()
                .Given()
                    .HasPerformed<StubDockerImageExist>(m => m.WithImage(AlpineImage).IsExist(true))
                    .HasPerformed<StubDockerImageExist>(m => m.WithImage(GitProviderImage).IsExist(true))
                .When()
                    .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("run")
                    .WithCommandArg("-b", "LoadIfMissing")
                    .WithCommandArg("-f", SingleImageManifestOutputPath)
                    .WithCommandArg("-d", EngineManifestOutputPath)
                    .WithCommandArg("-e"))
                .Then()
                .Expects<ProcessResult>(proc => proc
                    .ExitCode(Constants.ExitCode.Ok))
                .Run();
        }
        
        [Fact]
        public void When_Exit_Present_And_Behavior_Argument_Missing_Return_Ok()
        {
            Scenario.New()
                .Given()
                    .HasPerformed<StubDockerImageExist>(m => m.WithImage(AlpineImage))
                    .HasPerformed<StubDockerImageExist>(m => m.WithImage(GitProviderImage))
                    .HasPerformed<StubDockerPullImage>(m => m.WithImage(AlpineImage))
                    .HasPerformed<StubDockerPullImage>(m => m.WithImage(GitProviderImage))
                .When()
                    .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("run")
                    .WithCommandArg("-f", SingleImageManifestOutputPath)
                    .WithCommandArg("-d", EngineManifestOutputPath)
                    .WithCommandArg("-e"))
                .Then()
                    .Expects<ProcessResult>(proc => proc
                    .ExitCode(Constants.ExitCode.Ok))
                .Run();
        }
        
        [Fact]
        public void When_Exit_Present_And_Images_Already_Exist_And_Behavior_Argument_Missing_Should_Return_Ok()
        {
            Scenario.New()
                .Given()
                    .HasPerformed<StubDockerImageExist>(m => m.WithImage(AlpineImage).IsExist(true))
                    .HasPerformed<StubDockerImageExist>(m => m.WithImage(GitProviderImage).IsExist(true))
                .When()
                    .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("run")
                    .WithCommandArg("-f", SingleImageManifestOutputPath)
                    .WithCommandArg("-d", EngineManifestOutputPath)
                    .WithCommandArg("-e"))
                .Then()
                .Expects<ProcessResult>(proc => proc
                    .ExitCode(Constants.ExitCode.Ok))
                .Run();
        }
        
        [Fact]
        public void When_Exit_Present_And_Behavior_LoadLatest_Should_Return_Ok()
        {
            Scenario.New()
                .Given()
                    .HasPerformed<StubDockerPullImage>(m => m.WithImage(AlpineImage))
                    .HasPerformed<StubDockerPullImage>(m => m.WithImage(GitProviderImage))
                .When()
                    .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("run")
                    .WithCommandArg("-b", "LoadLatest")
                    .WithCommandArg("-f", SingleImageManifestOutputPath)
                    .WithCommandArg("-d", EngineManifestOutputPath)
                    .WithCommandArg("-e"))
                .Then()
                .Expects<ProcessResult>(proc => proc
                    .ExitCode(Constants.ExitCode.Ok))
                .Run();
        }
        
        [Fact]
        public void When_Exit_Present_And_Behavior_None_Should_Return_Ok()
        {
            Scenario.New()
                .When()
                    .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("run")
                    .WithCommandArg("-b", "None")
                    .WithCommandArg("-f", SingleImageManifestOutputPath)
                    .WithCommandArg("-d", EngineManifestOutputPath)
                    .WithCommandArg("-e"))
                .Then()
                .Expects<ProcessResult>(proc => proc
                    .ExitCode(Constants.ExitCode.Ok))
                .Run();
        }

        [Fact]
        public void When_Single_Image_Manifest_Should_Return_Ok()
        {
            const string srcResourceName = "Run.src.txt";

            const string srcOutputName = nameof(RunTests) + "src.txt";
            
            var sourceFilePath = Path.Combine("src", srcOutputName);
            var importFilePath = Path.Combine("imports", srcOutputName);
            
            var sourceVolumeId = $"sources-{Guid.NewGuid()}";
            var importsVolumeId = $"imports-{Guid.NewGuid()}";
            var artifactsVolumeId = $"artifacts-{Guid.NewGuid()}";
            
            Scenario.New()
                .Given()
                    .HasPerformed<MoveFileFromResource>(m => m.FromResource(srcResourceName).ToPhysicalFile(sourceFilePath))
                    .HasPerformed<MoveFileFromResource>(m => m.FromResource(srcResourceName).ToPhysicalFile(importFilePath))
                
                    .HasPerformed<StubDockerPullImage>(m => m.WithImage(AlpineImage))
                    .HasPerformed<StubDockerPullImage>(m => m.WithImage(GitProviderImage))
                
                    .HasPerformed<StubDockerCreateVolume>(m => m.WithVolumePrefix("sources").WithResponseVolumeId(sourceVolumeId))
                    .HasPerformed<StubDockerCreateVolume>(m => m.WithVolumePrefix("imports").WithResponseVolumeId(importsVolumeId))
                    .HasPerformed<StubDockerCreateVolume>(m => m.WithVolumePrefix("artifacts").WithResponseVolumeId(artifactsVolumeId))
                    
                    .HasPerformed<StubImportActivity>(m => m.WithImage(AlpineImage).WithSourceId(importsVolumeId).WithTargetFolder("/imports"))
                    .HasPerformed<StubImportActivity>(m => m.WithImage(AlpineImage).WithSourceId(sourceVolumeId).WithTargetFolder("/src"))
                
                    .HasPerformed<StubContainerActivity>(m => m
                        .WithEnv("GIT_REPOSITORY_URL", "https://github.com/GodelTech/CodeReview.Orchestrator.git")
                        .WithEnv("GIT_BRANCH", "main")
                        .WithImage(GitProviderImage)
                        .WithMountedVolume(volume => volume.WithTarget("/artifacts").WithSource(artifactsVolumeId))
                        .WithMountedVolume(volume => volume.WithTarget("/src").WithSource(sourceVolumeId))
                        .WithMountedVolume(volume => volume.WithTarget("/imports").WithSource(importsVolumeId).WithReadOnly(true)))
                
                    .HasPerformed<StubExportActivity>(m => m
                        .WithImage(AlpineImage)
                        .WithSourceId(artifactsVolumeId)
                        .WithResource("Run.artifacts.zip"))
                
                    .HasPerformed<StubDockerRemoveVolume>(m => m.WithVolumeId(sourceVolumeId))
                    .HasPerformed<StubDockerRemoveVolume>(m => m.WithVolumeId(artifactsVolumeId))
                    .HasPerformed<StubDockerRemoveVolume>(m => m.WithVolumeId(importsVolumeId))
                .When()
                    .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("run")
                    .WithCommandArg("-b", "LoadLatest")
                    .WithCommandArg("-f", SingleImageManifestOutputPath)
                    .WithCommandArg("-d", EngineManifestOutputPath))
                .Then()
                    .Expects<ProcessResult>(proc => proc.ExitCode(Constants.ExitCode.Ok))
                .Run();
        }

        [Fact]
        public void When_Several_Image_Manifest_Should_Return_Ok()
        {
            const string srcResourceName = "Run.src.txt";
            const string artifactResourceName = "Run.artifacts.zip";
            
            const string srcOutputName = nameof(RunTests) + "src.txt";
            
            var sourceFilePath = Path.Combine("src", srcOutputName);
            
            var sourceVolumeId = $"sources-{Guid.NewGuid()}";
            var newSourceVolumeId = $"sources-{Guid.NewGuid()}-new";

            Scenario.New()
                .Given()
                    .HasPerformed<InitWiremockScenario>(scenario => scenario.WithName(nameof(When_Several_Image_Manifest_Should_Return_Ok).ToLower()))
                    .HasPerformed<MoveFileFromResource>(m => m.FromResource(srcResourceName).ToPhysicalFile(sourceFilePath))
                    
                    .HasPerformed<StubDockerPullImage>(m => m.WithImage(ClocImage))
                    .HasPerformed<StubDockerPullImage>(m => m.WithImage(AlpineImage))
                    .HasPerformed<StubDockerPullImage>(m => m.WithImage(GitProviderImage))
                    
                    .HasPerformed<StubDockerCreateVolume>(m => m
                        .WithVolumePrefix("sources")
                        .WithResponseVolumeId(sourceVolumeId))
                    
                    .HasPerformed<StubImportActivity>(m => m
                        .WithImage(AlpineImage)
                        .WithSourceId(sourceVolumeId)
                        .WithTargetFolder("/src")
                        .SetWiremockState("source_imported"))
                    
                    .HasPerformed<UpdateScenarioState>(art => art.State("source_imported"))
                
                    .HasPerformed<StubContainerActivity>(m => m
                        .WithEnv("GIT_REPOSITORY_URL", "https://github.com/GodelTech/CodeReview.Orchestrator.git")
                        .WithEnv("GIT_BRANCH", "main")
                        .WithImage(GitProviderImage)
                        .WithMountedVolume(volume => volume.WithTarget("/src").WithSource(sourceVolumeId))
                        .SetWiremockState("gitprovider_activity_completed"))
                
                    .HasPerformed<UpdateScenarioState>(art => art.State("gitprovider_activity_completed"))
                
                    // Switch engine
                    // Export volumes
                    .HasPerformed<StubDockerCreateContainer>(m => m
                        .WithImage(AlpineImage)
                        .WithMountedVolume(volume => volume.WithTarget("/sources").WithSource(sourceVolumeId)))
                    .HasPerformed<StubDockerArchiveFromContainer, ContainerId>((m, containerId) => m
                        .WithContainerId(containerId.Id)
                        .WithDockerPath("/sources")
                        .WithResource(artifactResourceName))
                    .HasPerformed<StubDockerRemoveContainer, ContainerId>((m, containerId) => m
                        .WithId(containerId.Id))
                    .HasPerformed<StubDockerRemoveVolume>(m => m
                        .WithVolumeId(sourceVolumeId)
                        .SetWiremockState("cloc_activity_switch_engine_started"))
                    
                    .HasPerformed<UpdateScenarioState>(art => art.State("cloc_activity_switch_engine_started"))
                
                    // Import volumes
                    .HasPerformed<StubDockerCreateVolume>(m => m
                        .WithVolumePrefix("sources")
                        .WithResponseVolumeId(newSourceVolumeId))
                    .HasPerformed<StubDockerCreateContainer>(m => m
                        .WithImage(AlpineImage)
                        .WithMountedVolume(volume => volume
                            .WithTarget("/sources")
                            .WithSource(newSourceVolumeId)))
                    .HasPerformed<StubDockerExtractArchiveToContainer>(m => m
                        .WithContainerId(newSourceVolumeId)
                        .WithDockerPath("/sources")
                        .AllowOverwriteDirWithFile(true))
                    .HasPerformed<StubDockerRemoveContainer, ContainerId>((m, containerId) => m
                        .WithId(containerId.Id)
                        .SetWiremockState("cloc_activity_switch_engine_completed"))
                
                    .HasPerformed<UpdateScenarioState>(art => art.State("cloc_activity_switch_engine_completed"))
                
                    .HasPerformed<StubContainerActivity>(m => m
                        .WithImage(ClocImage)
                        .WithCommands(
                            "--by-file",
                            "--report-file=/artifacts/stat.cloc.json",
                            "--json",
                            ".")
                        .WithMountedVolume(volume => volume
                            .WithTarget("/source")
                            .WithSource(newSourceVolumeId)))
                    
                    .HasPerformed<StubDockerRemoveVolume>(m => m.WithVolumeId(newSourceVolumeId))
                .When()
                    .Performs<ExecuteProcess>(proc => proc
                        .WithCommandArg("run")
                        .WithCommandArg("-b", "LoadLatest")
                        .WithCommandArg("-f", SeveralImageManifestOutputPath)
                        .WithCommandArg("-d", EngineManifestOutputPath))
                .Then()
                    .Expects<ProcessResult>(proc => proc.ExitCode(Constants.ExitCode.Ok))
                .Run();
        }
    }
}