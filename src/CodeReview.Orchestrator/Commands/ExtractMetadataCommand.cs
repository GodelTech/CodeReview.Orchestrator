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
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;
        private readonly IDirectoryService _directoryService;
        private readonly IOptionMetadataProvider _optionMetadataProvider;

        public ExtractMetadataCommand(
            IFileService fileService, 
            IPathService pathService, 
            IDirectoryService directoryService, 
            IOptionMetadataProvider optionMetadataProvider)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            _optionMetadataProvider = optionMetadataProvider ?? throw new ArgumentNullException(nameof(optionMetadataProvider));
        } 

        public async Task<int> ExecuteAsync(ExtractMetadataOptions options)
        {
            if (options is null) throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.OutputPath)) throw new ArgumentNullException(nameof(options.OutputPath), "Value cannot be null or whitespace.");

            var metadata = _optionMetadataProvider.GetOptionsMetadata();
            var directoryPath = _pathService.GetDirectoryName(options.OutputPath);
            
            _directoryService.CreateDirectory(directoryPath);
            
            await _fileService.WriteAllTextAsync(options.OutputPath, metadata);
            
            return Constants.SuccessExitCode;
        }
    }
}