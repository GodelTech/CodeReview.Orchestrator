using CodeReview.Orchestrator.SubsystemTests.Actions;
using CodeReview.Orchestrator.SubsystemTests.Expectations;
using CodeReview.Orchestrator.SubsystemTests.Utils;
using GodelTech.StoryLine;
using Xunit;

namespace CodeReview.Orchestrator.SubsystemTests.Tests
{
    public class ExtractMetadataTests : TestBase
    {
        private const string MetadataName = nameof(ExtractMetadataTests) + "_metadata_output.txt";
        
        private static readonly string MetadataOutputPath = FileHelper.GetOutputPath(MetadataName);
        
        [Fact]
        public void When_Missing_Output_Parameter_Should_Return_ErrorCode()
        {
            Scenario.New()
                .When()
                .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("extract-metadata"))
                .Then()
                .Expects<ProcessResult>(proc => proc
                    .ExitCode(Constants.ExitCode.Error))
                .Run();
        }
        
        [Theory]
        [InlineData("-o", "")]
        [InlineData("-o", " ")]
        [InlineData("--output", "")]
        [InlineData("--output", " ")]
        public void When_Missing_Output_Parameter_Invalid_Should_Return_ErrorCode(string outputArg, string outputArgValue)
        {
            Scenario.New()
                .When()
                .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("extract-metadata")
                    .WithCommandArg(outputArg, outputArgValue))
                .Then()
                .Expects<ProcessResult>(proc => proc
                    .ExitCode(Constants.ExitCode.Error))
                .Run();
        }
        
        [Theory]
        [InlineData("-o")]
        [InlineData("--output")]
        public void When_All_Ok_Should_Generate_Metadata_File_And_Return_Ok(string outputArg)
        {
            const string metadataResourceName = "ExtractMetadata.metadata_output.txt";
            
            Scenario.New()
                .When()
                .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("extract-metadata")
                    .WithCommandArg(outputArg, MetadataOutputPath))
                .Then()
                .Expects<ProcessResult>(proc => proc
                    .FileEqualToTheResource(MetadataOutputPath, metadataResourceName)
                    .ExitCode(Constants.ExitCode.Ok))
                .Run();
        }
    }
}
