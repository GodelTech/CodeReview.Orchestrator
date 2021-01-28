using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReviewItEasy.Orchestrator.Model;

namespace ReviewItEasy.Orchestrator.Services
{
    public class AnalysisManifestProvider : IAnalysisManifestProvider
    {
        private readonly IYamlSerializer _yamlSerializer;
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;
        private readonly ILogger<AnalysisManifestProvider> _logger;

        public AnalysisManifestProvider(
            IYamlSerializer yamlSerializer,
            IFileService fileService,
            IPathService pathService,
            ILogger<AnalysisManifestProvider> logger)
        {
            _yamlSerializer = yamlSerializer ?? throw new ArgumentNullException(nameof(yamlSerializer));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<AnalysisManifest> GetAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

            if (!_fileService.Exists(path))
            {
                _logger.LogError("File doesn't exists: {filePath}", _pathService.GetFullPath(path));
                
                return null;
            }
            
            var content = await _fileService.ReadAllTextAsync(path);

            try
            {
                return _yamlSerializer.Deserialize<AnalysisManifest>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize manifest");
            }

            return null;
        }
    }
}