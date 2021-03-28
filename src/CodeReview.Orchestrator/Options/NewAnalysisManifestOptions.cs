using CommandLine;

namespace GodelTech.CodeReview.Orchestrator.Options
{
    [Verb("new", HelpText = "Creates draft of analysis manifest.")]
    public class NewAnalysisManifestOptions
    {
        [Option('o', "output", Required = true, HelpText = "Path to output file")]
        public string File { get; set; }
    }
}