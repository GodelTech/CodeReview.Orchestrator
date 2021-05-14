using System;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Options;
using GodelTech.CodeReview.Orchestrator.Services;
using GodelTech.CodeReview.Orchestrator.Utils;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public class ExtractMetadataCommand : IExtractMetadataCommand
    {
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;
        private readonly IDirectoryService _directoryService;
        private readonly ILogger<ExtractMetadataCommand> _logger;
        private readonly IOptionMetadataProvider _optionMetadataProvider;

        public ExtractMetadataCommand(
            IFileService fileService, 
            IPathService pathService,
            IDirectoryService directoryService, 
            ILogger<ExtractMetadataCommand> logger,
            IOptionMetadataProvider optionMetadataProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            _optionMetadataProvider = optionMetadataProvider ?? throw new ArgumentNullException(nameof(optionMetadataProvider));
        } 

        public async Task<int> ExecuteAsync(ExtractMetadataOptions options)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.OutputPath)) throw new ArgumentNullException(nameof(options.OutputPath), "Value cannot be null or whitespace.");

            var fullPath = _pathService.GetFullPath(options.OutputPath);
            var directoryPath = _pathService.GetDirectoryName(options.OutputPath);

            if (!_directoryService.Exists(directoryPath))
            {
                _logger.LogError("Could not find a part of the path '{Path}'", fullPath);
                
                return Constants.ErrorExitCode;
            }
            
            var metadata = _optionMetadataProvider.GetOptionsMetadata();
            
            await _fileService.WriteAllTextAsync(options.OutputPath, metadata);
            
            return Constants.SuccessExitCode;
        }
    }
}