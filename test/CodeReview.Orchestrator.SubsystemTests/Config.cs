using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace CodeReview.Orchestrator.SubsystemTests
{
    public static class Config
    {
        public static Assembly Assembly { get; set; }
        public static string ResourcePath { get; set; } = string.Empty;
        public static IConfiguration Configuration { get; set; }

        public static string ExePath => Configuration?["ExePath"] ?? string.Empty;
        public static string OutputDirectoryPath => Configuration?["OutputDirectoryPath"] ?? string.Empty;
    }
}