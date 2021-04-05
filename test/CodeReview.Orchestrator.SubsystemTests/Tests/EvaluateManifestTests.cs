using CodeReview.Orchestrator.SubsystemTests.Actions;
using CodeReview.Orchestrator.SubsystemTests.Expectations;
using CodeReview.Orchestrator.SubsystemTests.Utils;
using GodelTech.StoryLine;
using Xunit;

namespace CodeReview.Orchestrator.SubsystemTests.Tests
{
    public class EvaluateManifestTests : TestBase
    {
        private const string EvalOutputName = nameof(EvaluateManifestTests) + "_eval_output.txt";
        private const string ManifestOutputName = nameof(EvaluateManifestTests) + "_git-roslyn-converter.yaml";
        private const string InvalidManifestOutputName = nameof(EvaluateManifestTests) + "_git-roslyn-converter_invalid.yaml";

        private static readonly string EvalOutputPath = FileHelper.GetOutputPath(EvalOutputName);
        private static readonly string ManifestOutputPath = FileHelper.GetOutputPath(ManifestOutputName);
        private static readonly string InvalidManifestOutputPath = FileHelper.GetOutputPath(InvalidManifestOutputName);

        static EvaluateManifestTests()
        {
            const string manifestResourceName = "EvaluateManifest.git-roslyn-converter.yaml";
            const string invalidManifestResourceName = "EvaluateManifest.git-roslyn-converter_invalid.yaml";
            
            FileHelper.CopyFromResource(manifestResourceName, ManifestOutputPath);
            FileHelper.CopyFromResource(invalidManifestResourceName, InvalidManifestOutputPath);
        }
        
        [Theory]
        [InlineData("-f")]
        [InlineData("--file")]
        public void When_Missing_Output_Args_Should_Return_Error(string fileArg)
        {
            Scenario.New()
                .When()
                .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("eval", string.Empty)
                    .WithCommandArg(fileArg, ManifestOutputPath))
                .Then()
                .Expects<ProcessResult>(proc => proc
                    .ExitCode(Constants.ExitCode.Error))
                .Run();
        }
        
        [Theory]
        [InlineData("--output")]
        [InlineData("-output")]
        public void When_Missing_File_Args_Should_Return_Error(string outputArg)
        {
            Scenario.New()
                .When()
                .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("eval", string.Empty)
                    .WithCommandArg(outputArg, EvalOutputPath))
                .Then()
                .Expects<ProcessResult>(proc => proc
                    .ExitCode(Constants.ExitCode.Error))
                .Run();
        }
        
        [Theory]
        [InlineData("-f", "-o")]
        [InlineData("--file", "-o")]
        [InlineData("-f", "--output")]
        [InlineData("--file", "-output")]
        public void When_Missing_Manifest_File_Should_Return_ErrorCode(string fileArg, string outputArg)
        {
            Scenario.New()
                .When()
                .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("eval", string.Empty)
                    .WithCommandArg(fileArg, "path")
                    .WithCommandArg(outputArg, EvalOutputPath))
                .Then()
                .Expects<ProcessResult>(proc => proc
                    .ExitCode(Constants.ExitCode.Error))
                .Run();
        }
        
        [Theory]
        [InlineData("-f", "-o")]
        [InlineData("--file", "-o")]
        [InlineData("-f", "--output")]
        [InlineData("--file", "-output")]
        public void When_Manifest_Is_Invalid_Should_Return_ErrorCode(string fileArg, string outputArg)
        {
            Scenario.New()
                .When()
                .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("eval", string.Empty)
                    .WithCommandArg(fileArg, InvalidManifestOutputPath)
                    .WithCommandArg(outputArg, EvalOutputPath))
                .Then()
                .Expects<ProcessResult>(proc => proc
                    .ExitCode(Constants.ExitCode.Error))
                .Run();
        }
        
        [Theory]
        [InlineData("-f", "-o")]
        [InlineData("--file", "-o")]
        [InlineData("-f", "--output")]
        [InlineData("--file", "--output")]
        public void When_Manifest_Is_Valid_Should_Return_Ok(string fileArg, string outputArg)
        {
            const string evalResourceName = "EvaluateManifest.eval_output.txt";

            Scenario.New()
                .When()
                .Performs<ExecuteProcess>(proc => proc
                    .WithCommandArg("eval", string.Empty)
                    .WithCommandArg(fileArg, ManifestOutputPath)
                    .WithCommandArg(outputArg, EvalOutputPath))
                .Then()
                    .Expects<ProcessResult>(proc => proc
                    .FileEqualToTheResource(EvalOutputPath, evalResourceName)
                    .ExitCode(Constants.ExitCode.Ok))
                .Run();
        }
    }
}