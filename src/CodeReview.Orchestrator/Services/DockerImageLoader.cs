using System;
using System.Linq;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Options;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class DockerImageLoader : IDockerImageLoader
    {
        private readonly IContainerService _containerService;
        private readonly ILogger<DockerImageLoader> _logger;

        public DockerImageLoader(
            IContainerService containerService,
            ILogger<DockerImageLoader> logger)
        {
            _containerService = containerService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task LoadImagesAsync(ImageLoadBehavior behavior, AnalysisManifest manifest)
        {
            if (manifest == null) 
                throw new ArgumentNullException(nameof(manifest));

            var imagesToDownload = manifest.Activities.Select(x => x.Value.Image)
                .Concat(new [] { Constants.WorkerImage })
                .Distinct()
                .ToArray();

            foreach (var image in imagesToDownload)
            {
                _logger.LogInformation("Validating image exists... Image = {imageName}", image);

                await _containerService.EnsureImageDownloadedAsync(image);
                
                _logger.LogInformation("Image validated");
            }
        }
    }
}
