using System;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class ManifestProvider : IManifestProvider
    {
        private readonly IYamlSerializer _yamlSerializer;
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;
        private readonly ILogger<ManifestProvider> _logger;

        public ManifestProvider(
            IYamlSerializer yamlSerializer,
            IFileService fileService,
            IPathService pathService,
            ILogger<ManifestProvider> logger)
        {
            _yamlSerializer = yamlSerializer ?? throw new ArgumentNullException(nameof(yamlSerializer));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<T> GetAsync<T>(string path)
            where T : class
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
                return _yamlSerializer.Deserialize<T>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize manifest");
            }

            return null;
        }
    }
}