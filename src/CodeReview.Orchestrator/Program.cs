using System;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using GodelTech.CodeReview.Orchestrator.Activities;
using GodelTech.CodeReview.Orchestrator.Commands;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Options;
using GodelTech.CodeReview.Orchestrator.Services;
using GodelTech.CodeReview.Orchestrator.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator
{
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
            return await container.GetRequiredService<IValidateManifestCommand>().ExecuteAsync(options.File, options.OutputPath);
        }

        private static async Task<int> ProcessNewAsync(NewOptions options, IServiceProvider container)
        {
            return await container.GetRequiredService<ICreateNewManifestCommand>().ExecuteAsync(options.File);
        }

        private static async Task<int> ProcessRunAsync(RunOptions options, IServiceProvider container)
        {
            return await container.GetRequiredService<IRunAnalysisCommand>().ExecuteAsync(options);
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

            serviceProvider.AddTransient<IDockerImageLoader, DockerImageLoader>();
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
            serviceProvider.AddTransient<IRunAnalysisCommand, RunAnalysisCommand>();
            serviceProvider.AddTransient<ICreateNewManifestCommand, CreateNewManifestCommand>();
            serviceProvider.AddTransient<IValidateManifestCommand, ValidateManifestCommand>();
            serviceProvider.AddTransient<IOutputFolderPathCalculator, OutputFolderPathCalculator>();

            return serviceProvider.BuildServiceProvider();
        }
    }
}
