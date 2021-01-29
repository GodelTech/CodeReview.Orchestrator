namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public class ExecutionResult
    {
        public string StdErr { get; set; }
        public string StdOut { get; set; }
        public long ExitCode { get; set; }
    }
}