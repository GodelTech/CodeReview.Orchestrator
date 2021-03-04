using System;
using GodelTech.CodeReview.Orchestrator.Services;

namespace GodelTech.CodeReview.Orchestrator.Activities
{
    public class ProcessingContextFactory : IProcessingContextFactory
    {
        private readonly IContainerService _containerService;
        private readonly IPathService _pathService;

        public ProcessingContextFactory(IContainerService containerService, IPathService pathService)
        {
            _containerService = containerService ?? throw new ArgumentNullException(nameof(containerService));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
        }

        public IProcessingContext Create(string manifestFilePath)
        {
            if (string.IsNullOrWhiteSpace(manifestFilePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(manifestFilePath));
            
            return new ProcessingContext(manifestFilePath, _containerService, _pathService);
        }
    }
}