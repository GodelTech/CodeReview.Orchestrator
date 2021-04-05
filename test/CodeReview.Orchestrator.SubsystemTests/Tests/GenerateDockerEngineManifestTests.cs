using System.IO;
using CodeReview.Orchestrator.SubsystemTests.Actions;
using CodeReview.Orchestrator.SubsystemTests.Expectations;
using CodeReview.Orchestrator.SubsystemTests.Utils;
using GodelTech.StoryLine;
using Xunit;

namespace CodeReview.Orchestrator.SubsystemTests.Tests
{
    public class GenerateDockerEngineManifestTests : TestBase
    {
        private const string ManifestName = "docker-engines.yml";
        private const string ManifestResourcePath = "GenerateDockerEngine." + ManifestName;
        private const string OutputManifestFileName = nameof(GenerateDockerEngineManifestTests) + "_docker-engine.yml";
        
        private static readonly string OutputManifestFullName = FileHelper.GetOutputPath(OutputManifestFileName);
        
        [Fact]
        public void When_Output_Parameter_Missing_Should_Return_Error()
        {
            Scenario.New()
                .When()
                    .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("new-engine-collection", string.Empty))
                .Then()
                .Expects<ProcessResult>(proc => proc
                    .ExitCode(Constants.ExitCode.Error))
                .Run();
        }
        
        [Theory]
        [InlineData("-o")]
        [InlineData("--output")]
        public void When_All_Ok_Should_Create_DockerEngineManifest(string argName)
        {
            Scenario.New()
                .When()
                    .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("new-engine-collection", string.Empty)
                    .WithCommandArg(argName, OutputManifestFullName))
                .Then()
                    .Expects<ProcessResult>(proc => proc
                    .FileEqualToTheResource(OutputManifestFullName, ManifestResourcePath)
                    .ExitCode(Constants.ExitCode.Ok))
                .Run();
        }
    }
}