using CommandLine;

namespace GodelTech.CodeReview.Orchestrator.Options
{
    [Verb("new-engine-collection", HelpText = "Creates draft of Docker engine collection manifest.")]
    public class NewDockerEngineCollectionManifestOptions
    {
        [Option('o', "output", Required = true, HelpText = "Path to output file")]
        public string File { get; set; }
    }
}