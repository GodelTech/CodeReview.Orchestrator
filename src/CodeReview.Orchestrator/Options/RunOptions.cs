using CommandLine;

namespace ReviewItEasy.Orchestrator.Options
{
    [Verb("run", true, HelpText = "Runs workflow defined by manifest file.")]
    public class RunOptions
    {
        [Option('f', "file", Required = true, HelpText = "Path to workflow file")]
        public string File { get; set; }
    }
}
