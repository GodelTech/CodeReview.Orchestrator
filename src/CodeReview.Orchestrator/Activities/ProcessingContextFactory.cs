using System;
using GodelTech.CodeReview.Orchestrator.Services;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public class ProcessingContextFactory : IProcessingContextFactory
    {
        private readonly IDockerEngineContext _dockerEngineContext;
        private readonly IContainerService _containerService;
        private readonly IPathService _pathService;

        public ProcessingContextFactory(IDockerEngineContext dockerEngineContext, IContainerService containerService, IPathService pathService)
        {
            _dockerEngineContext = dockerEngineContext ?? throw new ArgumentNullException(nameof(dockerEngineContext));
            _containerService = containerService ?? throw new ArgumentNullException(nameof(containerService));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
        }

        public IProcessingContext Create(string manifestFilePath)
        {
            if (string.IsNullOrWhiteSpace(manifestFilePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(manifestFilePath));
            
            return new ProcessingContext(manifestFilePath, _containerService, _pathService, _dockerEngineContext);
        }
    }
}