using System;
using CodeReview.Orchestrator.Services;

namespace CodeReview.Orchestrator.Commands
{
    public class ProcessingContextFactory : IProcessingContextFactory
    {
        private readonly IContainerService _containerService;

        public ProcessingContextFactory(IContainerService containerService)
        {
            _containerService = containerService ?? throw new ArgumentNullException(nameof(containerService));
        }

        public IProcessingContext Create()
        {
            return new ProcessingContext(_containerService);
        }
    }
}