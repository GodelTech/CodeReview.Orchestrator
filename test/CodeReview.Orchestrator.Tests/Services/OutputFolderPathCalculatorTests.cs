using CodeReview.Orchestrator.Services;
using FluentAssertions;
using Xunit;

namespace CodeReview.Orchestrator.Tests.Services
{
    public class OutputFolderPathCalculatorTests
    {
        private readonly OutputFolderPathCalculator _underTest;

        public OutputFolderPathCalculatorTests()
        {
            _underTest = new OutputFolderPathCalculator(new PathService());
        }

        [Theory]
        [InlineData(@"d:\folder", "/artifacts", "artifacts/data.zip", @"d:/folder")]
        [InlineData(@"d:\folder", "/artifacts", "artifacts/roslyn/data.json", @"d:/folder/roslyn")]
        [InlineData(@"d:\folder", "artifacts", "artifacts/data.zip", @"d:/folder")]
        [InlineData(@"d:\folder", "artifacts", "artifacts/roslyn/data.json", @"d:/folder/roslyn")]
        [InlineData(@"d:\folder", "artifacts/", "artifacts/data.zip", @"d:/folder")]
        [InlineData(@"d:\folder", "artifacts/", "artifacts/roslyn/data.json", @"d:/folder/roslyn")]
        [InlineData(@"d:\folder", "/artifacts/", "artifacts/data.zip", @"d:/folder")]
        [InlineData(@"d:\folder", "/artifacts/", "artifacts/roslyn/data.json", @"d:/folder/roslyn")]
        public void CalculateOutputDirectoryName_Should_ReturnExpectedResult(
            string dstFolderPath, string originalPath, string archiveEntryPath, string expectedResult)
        {
            _underTest.CalculateOutputDirectoryName(dstFolderPath, originalPath, archiveEntryPath).Should().Be(expectedResult);
        }
    }
}
