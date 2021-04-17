using System;
using System.IO;
using Xunit.Abstractions;
using Microsoft.Extensions.Configuration;

namespace CodeReview.Orchestrator.SubsystemTests
{
    public class StartUpFixture : IDisposable
    {
        public StartUpFixture(IMessageSink messageSink)
        {
            var configuration = GetConfiguration();
            Config.Configuration = configuration;

            Config.Assembly = typeof(TestBase).Assembly;
            Config.ResourcePath = "CodeReview.Orchestrator.SubsystemTests.Resources";
            GodelTech.StoryLine.Wiremock.Config.SetBaseAddress(configuration["WiremockAddress"]);
        }

        public void Dispose()
        {
            if (Directory.Exists(Config.OutputDirectoryPath))
                Directory.Delete(Config.OutputDirectoryPath, true);
        }

        private static IConfiguration GetConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", false, false)
                .AddJsonFile($"appsettings.{environmentName}.json", true, false);

            return builder.Build();
        }
    }
}