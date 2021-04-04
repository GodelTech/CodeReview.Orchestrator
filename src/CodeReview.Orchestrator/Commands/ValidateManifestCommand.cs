using System;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Options;
using GodelTech.CodeReview.Orchestrator.Services;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public class ValidateManifestCommand : AnalysisRunnerBase, IValidateManifestCommand
    {
        private readonly IFileService _fileService;
        private readonly IYamlSerializer _yamlSerializer;

        public ValidateManifestCommand(
            IManifestProvider analysisManifestProvider,
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

        public async Task<int> ExecuteAsync(EvaluateOptions options)
        {
            if (options == null) 
                throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.File))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(options.File));

            var manifest = await GetManifestAsync(options.File);
            if (manifest == null)
                return Constants.ErrorExitCode;

            var yaml = _yamlSerializer.Serialize(manifest);

            await _fileService.WriteAllTextAsync(options.OutputPath, yaml);

            return Constants.SuccessExitCode;
        }
    }
}