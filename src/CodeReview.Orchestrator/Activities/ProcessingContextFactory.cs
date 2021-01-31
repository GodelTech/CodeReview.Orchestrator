using System;
using GodelTech.CodeReview.Orchestrator.Services;

namespace GodelTech.CodeReview.Orchestrator.Activities
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