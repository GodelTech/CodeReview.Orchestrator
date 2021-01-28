using CommandLine;

namespace ReviewItEasy.Orchestrator.Options
{
    [Verb("eval", HelpText = "Validates specified manifest.")]
    public class EvaluateOptions
    {
        [Option('f', "file", Required = true, HelpText = "Path to workflow file")]
        public string File { get; set; }

        [Option('o', "output", Required = true, HelpText = "Path workflow definition containing all resolved variables")]
        public string OutputPath { get; set; }
    }
}