using System;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Options;
using GodelTech.CodeReview.Orchestrator.Services;
using GodelTech.CodeReview.Orchestrator.Utils;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public class ExtractMetadataCommand : IExtractMetadataCommand
    {
        private static readonly Type[] Options =
        {
            typeof(RunOptions),
            typeof(EvaluateOptions),
            typeof(ExtractMetadataOptions),
            typeof(NewAnalysisManifestOptions),
            typeof(NewDockerEngineCollectionManifestOptions)
        };

        private readonly IFileService _fileService;
        private readonly IPathService _pathService;
        private readonly IDirectoryService _directoryService;
        private readonly ICommandLineJsonConvert _commandLineJsonConvert;

        public ExtractMetadataCommand(
            IFileService fileService, 
            IPathService pathService, 
            IDirectoryService directoryService, 
            ICommandLineJsonConvert commandLineJsonConvert)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            _commandLineJsonConvert = commandLineJsonConvert ?? throw new ArgumentNullException(nameof(commandLineJsonConvert));
        }

        public async Task<int> ExecuteAsync(ExtractMetadataOptions options)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.OutputPath)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(options.OutputPath));

            var jsonString = _commandLineJsonConvert.Serialize(Options);

            var directoryPath = _pathService.GetDirectoryName(options.OutputPath);
            _directoryService.CreateDirectory(directoryPath);
            
            await _fileService.WriteAllTextAsync(options.OutputPath, jsonString);
            
            return Constants.SuccessExitCode;
        }
    }
}