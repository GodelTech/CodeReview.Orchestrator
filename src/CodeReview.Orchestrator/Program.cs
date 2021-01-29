using System;
using System.IO;
using System.Threading.Tasks;
using CodeReview.Orchestrator.Commands;
using CodeReview.Orchestrator.Model;
using CodeReview.Orchestrator.Options;
using CodeReview.Orchestrator.Services;
using CodeReview.Orchestrator.Utils;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodeReview.Orchestrator
{
    // Replace PreProcessors, PostProcess and Analyzers with Activities and Parallel\Sequential run block which contains activities to execute in parallel (e.g. execution streams)
    // 1. If tool is published to NuGet it can be executed within pipline which has access to Docker Service and .NET Core
    // 2. Add ability to specify URL to docker service
    // 3. Print logs immediately once they are produced
    class Program
    {
        private static int Main(string[] args)
        {
            using var container = CreateServiceProvider();

            var parser = new Parser(x =>
            {
                x.HelpWriter = TextWriter.Null;
            });

            var result = parser.ParseArguments<RunOptions, NewOptions, EvaluateOptions>(args);

            var exitCode = result
                .MapResult(
                    (RunOptions x) => ProcessRunAsync(x, container).GetAwaiter().GetResult(),
                    (NewOptions x) => ProcessNewAsync(x, container).GetAwaiter().GetResult(),
                    (EvaluateOptions x) => ProcessEvaluateAsync(x, container).GetAwaiter().GetResult(),
                    _ => ProcessErrors(result));

            return exitCode;
        }

        private static int ProcessErrors(ParserResult<object> result)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = false;
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);
            
            Console.WriteLine(helpText);

            return Constants.ErrorExitCode;
        }

        private static async Task<int> ProcessEvaluateAsync(EvaluateOptions options, IServiceProvider container)
        {
            return await container.GetRequiredService<IManifestValidationRunner>().RunAsync(options.File, options.OutputPath);
        }

        private static async Task<int> ProcessNewAsync(NewOptions options, IServiceProvider container)
        {
            return await container.GetRequiredService<INewManifestRunner>().RunAsync(options.File);
        }

        private static async Task<int> ProcessRunAsync(RunOptions options, IServiceProvider container)
        {
            return await container.GetRequiredService<IAnalysisRunner>().RunAsync(options.File);
        }

        private static ServiceProvider CreateServiceProvider()
        {
            var serviceProvider = new ServiceCollection();

            serviceProvider.AddLogging(x =>
            {
                x.ClearProviders();
                x.AddProvider(new SimplifiedConsoleLoggerProvider());
            });

            serviceProvider.AddSingleton<IYamlSerializer, YamlSerializer>();
            serviceProvider.AddSingleton<INameFactory, NameFactory>();
            serviceProvider.AddSingleton<IFileService, FileService>();
            serviceProvider.AddSingleton<IPathService, PathService>();
            serviceProvider.AddSingleton<IDirectoryService, DirectoryService>();

            serviceProvider.AddSingleton<IVariableExpressionProvider, VariableExpressionProvider>();

            serviceProvider.AddTransient<ITarArchiveService, TarArchiveService>();
            serviceProvider.AddTransient<IEnvironmentVariableValueProvider, EnvironmentVariableValueProvider>();
            serviceProvider.AddTransient<IExpressionEvaluator, ExpressionEvaluator>();
            serviceProvider.AddTransient<IAnalysisManifestProvider, AnalysisManifestProvider>();
            serviceProvider.AddTransient<IActivityExecutor, ActivityExecutor>();
            serviceProvider.AddTransient<IActivityFactory, ActivityFactory>();
            serviceProvider.AddTransient<IProcessingContextFactory, ProcessingContextFactory>();
            serviceProvider.AddTransient<IDockerClientFactory, DockerClientFactory>();
            serviceProvider.AddTransient<IContainerService, ContainerService>();
            serviceProvider.AddTransient<IManifestValidator, ManifestValidator>();
            serviceProvider.AddTransient<IManifestExpressionExpander, ManifestExpressionExpander>();
            serviceProvider.AddTransient<IAnalysisRunner, AnalysisRunner>();
            serviceProvider.AddTransient<INewManifestRunner, NewManifestRunner>();
            serviceProvider.AddTransient<IManifestValidationRunner, ManifestValidationRunner>();
            serviceProvider.AddTransient<IOutputFolderPathCalculator, OutputFolderPathCalculator>();

            return serviceProvider.BuildServiceProvider();
        }
    }
}
