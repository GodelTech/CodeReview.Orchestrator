using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReviewItEasy.Orchestrator.Model;

namespace ReviewItEasy.Orchestrator.Services
{
    public class ManifestValidationRunner : AnalysisRunnerBase, IManifestValidationRunner
    {
        private readonly IFileService _fileService;
        private readonly IYamlSerializer _yamlSerializer;

        public ManifestValidationRunner(
            IAnalysisManifestProvider analysisManifestProvider,
            IManifestValidator manifestValidator,
            IManifestExpressionExpander manifestExpressionExpander,
            IFileService fileService,
            IYamlSerializer yamlSerializer,
            ILogger<AnalysisRunnerBase> logger)
            : base(analysisManifestProvider, manifestValidator, manifestExpressionExpander, logger)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _yamlSerializer = yamlSerializer ?? throw new ArgumentNullException(nameof(yamlSerializer));
        }

        public async Task<int> RunAsync(string manifestPath, string outputFilePath)
        {
            if (string.IsNullOrWhiteSpace(manifestPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(manifestPath));

            var manifest = await GetManifestAsync(manifestPath);
            if (manifest == null)
                return Constants.ErrorExitCode;

            var yaml = _yamlSerializer.Serialize(manifest);

            await _fileService.WriteAllTextAsync(outputFilePath, yaml);

            return Constants.SuccessExitCode;
        }
    }
}