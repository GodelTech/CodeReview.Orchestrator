using CommandLine;

namespace ReviewItEasy.Orchestrator.Options
{
    [Verb("new", HelpText = "Validates specified manifest.")]
    public class NewOptions
    {
        [Option('o', "output", Required = true, HelpText = "Path to output file")]
        public string File { get; set; }
    }
}