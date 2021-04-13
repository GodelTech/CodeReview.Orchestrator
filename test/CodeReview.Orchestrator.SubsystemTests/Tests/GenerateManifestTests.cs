using System.IO;
using CodeReview.Orchestrator.SubsystemTests.Actions;
using CodeReview.Orchestrator.SubsystemTests.Expectations;
using CodeReview.Orchestrator.SubsystemTests.Utils;
using GodelTech.StoryLine;
using Xunit;

namespace CodeReview.Orchestrator.SubsystemTests.Tests
{
    public class GenerateManifestTests : TestBase
    {
        private const string ManifestResourceName = "GenerateManifest.manifest.yml";

        private const string ManifestOutputName = nameof(GenerateManifestTests) + "_manifest.yml";
        
        private static readonly string ManifestOutputPath = FileHelper.GetOutputPath(ManifestOutputName);
        
        [Fact]
        public void When_Output_Parameter_Missing_Should_Return_Error()
        {
            Scenario.New()
                .When()
                    .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("new", string.Empty))
                .Then()
                .Expects<ProcessResult>(proc => proc
                    .ExitCode(Constants.ExitCode.Error))
                .Run();
        }
        
        [Theory]
        [InlineData("-o")]
        [InlineData("--output")]
        public void When_All_Ok_Should_Create_Manifest(string argName)
        {
            Scenario.New()
                .When()
                    .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("new", string.Empty)
                    .WithCommandArg(argName, ManifestOutputPath))
                .Then()
                    .Expects<ProcessResult>(proc => proc
                    .FileEqualToTheResource(ManifestOutputPath, ManifestResourceName)
                    .ExitCode(Constants.ExitCode.Ok))
                .Run();
        }
    }
}