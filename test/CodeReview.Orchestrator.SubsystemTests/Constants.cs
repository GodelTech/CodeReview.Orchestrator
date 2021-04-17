using System.Runtime.InteropServices;

namespace CodeReview.Orchestrator.SubsystemTests
{
    public static class Constants
    {
        public static class ExitCode
        {
            public static readonly int Error =
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? -1
                    : 255;
            
            public static int Ok = 0;
        }
    }
}