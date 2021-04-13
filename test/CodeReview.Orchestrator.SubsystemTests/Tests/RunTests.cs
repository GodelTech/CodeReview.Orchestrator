using System;
using System.IO;
using CodeReview.Orchestrator.SubsystemTests.Actions;
using CodeReview.Orchestrator.SubsystemTests.Actions.Docker;
using CodeReview.Orchestrator.SubsystemTests.Expectations;
using CodeReview.Orchestrator.SubsystemTests.Utils;
using GodelTech.StoryLine;
using Xunit;

namespace CodeReview.Orchestrator.SubsystemTests.Tests
{
    public class RunTests : TestBase
    {
        private const string AlpineImage = "alpine";
        private const string GitProviderImage = "godeltech/codereview.tools.gitprovider";

        private const string ManifestOutputName = nameof(RunTests) + "_git-roslyn-converter.yaml";
        private const string EngineManifestOutputName = nameof(RunTests) + "_run_engines.yaml";

        private static readonly string ManifestOutputPath = FileHelper.GetOutputPath(ManifestOutputName);
        private static readonly string EngineManifestOutputPath = FileHelper.GetOutputPath(EngineManifestOutputName);

        static RunTests()
        {
            const string manifestResourceName = "Run.git-roslyn-converter.yaml";
            const string engineManifestResourceName = "Run.engines.yaml";
            
            FileHelper.CreateDirectory("src");
            FileHelper.CreateDirectory("imports");
            
            FileHelper.CopyFromResource(manifestResourceName, ManifestOutputPath);
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
                    .WithCommandArg("-f", ManifestOutputPath)
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
                    .WithCommandArg("-f", ManifestOutputPath)
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
                    .WithCommandArg("-f", ManifestOutputPath)
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
                    .WithCommandArg("-f", ManifestOutputPath)
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
                    .WithCommandArg("-f", ManifestOutputPath)
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
                    .WithCommandArg("-f", ManifestOutputPath)
                    .WithCommandArg("-d", EngineManifestOutputPath)
                    .WithCommandArg("-e"))
                .Then()
                .Expects<ProcessResult>(proc => proc
                    .ExitCode(Constants.ExitCode.Ok))
                .Run();
        }

        [Fact]
        public void When_All_Ok_Should_Return_Ok()
        {
            const string srcResourceName = "Run.src.txt";
            const string srcOutputName = nameof(RunTests) + "src.txt";
            
            var srcFilePath = Path.Combine("src", srcOutputName);
            var importFilePath = Path.Combine("imports", srcOutputName);
            
            var srcVolumeId = $"src-{Guid.NewGuid()}";
            var artVolumeId = $"art-{Guid.NewGuid()}";
            var impVolumeId = $"imp-{Guid.NewGuid()}";
            
            Scenario.New()
                .Given()
                    .HasPerformed<MoveFileFromResource>(m => m.FromResource(srcResourceName).ToPhysicalFile(srcFilePath))
                    .HasPerformed<MoveFileFromResource>(m => m.FromResource(srcResourceName).ToPhysicalFile(importFilePath))
                
                    .HasPerformed<StubDockerPullImage>(m => m.WithImage(AlpineImage))
                    .HasPerformed<StubDockerPullImage>(m => m.WithImage(GitProviderImage))
                
                    .HasPerformed<StubDockerCreateVolume>(m => m.WithVolumePrefix("src").WithResponseVolumeId(srcVolumeId))
                    .HasPerformed<StubDockerCreateVolume>(m => m.WithVolumePrefix("art").WithResponseVolumeId(artVolumeId))
                    .HasPerformed<StubDockerCreateVolume>(m => m.WithVolumePrefix("imp").WithResponseVolumeId(impVolumeId))
                    
                    .HasPerformed<StubDockerImportDataActivity>(m => m.WithImage(AlpineImage).WithSourceId(impVolumeId))
                    .HasPerformed<StubDockerImportSourceActivity>(m => m.WithImage(AlpineImage).WithSourceId(srcVolumeId))
                
                    .HasPerformed<StubDockerContainerGitActivity>(m => m
                        .WithEnv("GIT_REPOSITORY_URL", "https://github.com/GodelTech/CodeReview.Orchestrator.git")
                        .WithEnv("GIT_BRANCH", "main")
                        .WithImage(GitProviderImage)
                        .WithArtVolumeId(artVolumeId)
                        .WithImpVolumeId(impVolumeId)
                        .WithSrcVolumeId(srcVolumeId))
                
                    .HasPerformed<StubDockerExportArtifactsActivity>(m => m
                        .WithImage(AlpineImage)
                        .WithSourceId(artVolumeId)
                        .WithArtifactZipResource("Run.artifacts.zip"))
                
                    .HasPerformed<StubDockerRemoveVolume>(m => m.WithVolumeId(srcVolumeId))
                    .HasPerformed<StubDockerRemoveVolume>(m => m.WithVolumeId(artVolumeId))
                    .HasPerformed<StubDockerRemoveVolume>(m => m.WithVolumeId(impVolumeId))
                .When()
                    .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("run")
                    .WithCommandArg("-b", "LoadLatest")
                    .WithCommandArg("-f", ManifestOutputPath)
                    .WithCommandArg("-d", EngineManifestOutputPath))
                .Then()
                    .Expects<ProcessResult>(proc => proc
                    .ExitCode(Constants.ExitCode.Ok))
                .Run();
        }
    }
}