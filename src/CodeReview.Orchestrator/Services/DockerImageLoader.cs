using System;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Options;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class DockerImageLoader : IDockerImageLoader
    {
        private readonly IDockerEngineProvider _engineProvider;
        private readonly IDockerEngineContext _engineContext;
        private readonly IContainerService _containerService;
        private readonly ILogger<DockerImageLoader> _logger;

        public DockerImageLoader(
            IDockerEngineProvider engineProvider,
            IDockerEngineContext engineContext,
            IContainerService containerService,
            ILogger<DockerImageLoader> logger)
        {
            _engineProvider = engineProvider ?? throw new ArgumentNullException(nameof(engineProvider));
            _engineContext = engineContext ?? throw new ArgumentNullException(nameof(engineContext));
            _containerService = containerService ?? throw new ArgumentNullException(nameof(containerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task LoadImagesAsync(ImageLoadBehavior behavior, AnalysisManifest manifest)
        {
            if (manifest == null) 
                throw new ArgumentNullException(nameof(manifest));

            if (behavior == ImageLoadBehavior.None)
                return;

            foreach (var engine in _engineProvider.ListAll())
            {
                _engineContext.Engine = engine;

                await ProcessImageAsync(behavior, engine.WorkerImage);
            }
            
            foreach (var (activityName, activity) in manifest.Activities)
            {
                _logger.LogInformation("Looking for Docker Engine matching activity requirements. Activity = {activity}", activityName);
                
                var engine = _engineProvider.Find(activity.Requirements);
                
                _engineContext.Engine = engine ?? throw new InvalidOperationException("Failed to find engine");
                
                _logger.LogInformation("Docker Engine found. Name = {name}", engine.Name);
                
                _logger.LogInformation("Downloading image. Image = {imageName}", activity.Image);

                await ProcessImageAsync(behavior, activity.Image);

                _logger.LogInformation("Image downloaded");
            }
        }

        private async Task ProcessImageAsync(ImageLoadBehavior behavior, string image)
        {
            if (behavior == ImageLoadBehavior.LoadIfMissing)
            {
                var imageExists = await _containerService.ImageExists(image);
                if (!imageExists)
                    await _containerService.PullImageAsync(image);
            }
            else
            {
                await _containerService.PullImageAsync(image);
            }
        }
    }
}
