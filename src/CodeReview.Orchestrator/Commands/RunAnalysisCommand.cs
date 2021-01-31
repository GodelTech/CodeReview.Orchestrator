﻿using System;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Activities;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Services;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public class RunAnalysisCommand : AnalysisRunnerBase, IRunAnalysisCommand
    {
        private readonly IActivityFactory _activityFactory;
        private readonly IProcessingContextFactory _processingContextFactory;

        public RunAnalysisCommand(
            IAnalysisManifestProvider analysisManifestProvider,
            IManifestValidator manifestValidator,
            IManifestExpressionExpander manifestExpressionExpander,
            IActivityFactory activityFactory,
            IProcessingContextFactory processingContextFactory,
            ILogger<AnalysisRunnerBase> logger)
            : base(analysisManifestProvider, manifestValidator, manifestExpressionExpander, logger)
        {
            _activityFactory = activityFactory ?? throw new ArgumentNullException(nameof(activityFactory));
            _processingContextFactory = processingContextFactory ?? throw new ArgumentNullException(nameof(processingContextFactory));
        }

        public async Task<int> ExecuteAsync(string manifestPath)
        {
            if (string.IsNullOrWhiteSpace(manifestPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(manifestPath));
            
            var manifest = await GetManifestAsync(manifestPath);
            if (manifest == null)
                return Constants.ErrorExitCode;
            
            Logger.LogInformation("Preparing Docker Volumes...");

            await using (var context = _processingContextFactory.Create())
            {
                await context.InitializeAsync();
                Logger.LogInformation("Docker Volumes created");

                var activity = _activityFactory.Create(manifest);

                Logger.LogInformation("Processing started.");
                var isSuccess = await activity.ExecuteAsync(context);
                Logger.LogInformation("Processing completed: {executionResult}", isSuccess ? "Success" : "Failure");

                return isSuccess ? Constants.SuccessExitCode : Constants.ErrorExitCode;
            }
        }
    }
}