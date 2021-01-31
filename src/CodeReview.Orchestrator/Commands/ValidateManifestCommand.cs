using System;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Services;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public class ValidateManifestCommand : AnalysisRunnerBase, IValidateManifestCommand
    {
        private readonly IFileService _fileService;
        private readonly IYamlSerializer _yamlSerializer;

        public ValidateManifestCommand(
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

        public async Task<int> ExecuteAsync(string manifestPath, string outputFilePath)
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