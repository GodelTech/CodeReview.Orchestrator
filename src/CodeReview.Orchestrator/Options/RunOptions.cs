using CommandLine;

namespace GodelTech.CodeReview.Orchestrator.Options
{
    [Verb("run", true, HelpText = "Runs workflow defined by manifest file.")]
    public class RunOptions
    {
        [Option('f', "file", Required = true, HelpText = "Path to workflow file")]
        public string File { get; set; }

        [Option('b', "behavior", Default = ImageLoadBehavior.LoadIfMissing, Required = false, HelpText = "Defines behavior of Docker Image loading")]
        public ImageLoadBehavior LoadBehavior { get; set; }

        [Option('e', "exit", Default = false, Required = false, HelpText = "Terminates execution of program after images are Docker Images are downloaded")]
        public bool ExitAfterImageDownload { get; set; }
    }
}
