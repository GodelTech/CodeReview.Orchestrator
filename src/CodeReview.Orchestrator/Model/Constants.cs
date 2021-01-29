namespace GodelTech.CodeReview.Orchestrator.Model
{
    internal static class Constants
    {
        public const string WorkerImage = "alpine";
        
        public const int SuccessExitCode = 0;
        public const int ErrorExitCode = -1;
        
        public const int MaxPathLength = 256;
        public const int MaxDockerImageNameLength = 128;
    }
}