namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public class ExecutionResult
    {
        public string StdErr { get; set; }
        public string StdOut { get; set; }
        public long ExitCode { get; set; }
    }
}