using CommandLine;

namespace GodelTech.CodeReview.Orchestrator.Options
{
    [Verb("extract-metadata", HelpText = "Generate a json with all commands and parameters.")]
    public class ExtractMetadataOptions
    {
        [Option('o', "output", Required = true, HelpText = "Path to output file.")]
        public string OutputPath { get; set; }
    }
}