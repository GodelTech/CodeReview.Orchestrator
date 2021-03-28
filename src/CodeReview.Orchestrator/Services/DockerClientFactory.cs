using System;
using Docker.DotNet;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class DockerClientFactory : IDockerClientFactory
    {
        private readonly IDockerEngineContext _engineContext;

        public DockerClientFactory(IDockerEngineContext engineContext)
        {
            _engineContext = engineContext ?? throw new ArgumentNullException(nameof(engineContext));
        }
        
        public IDockerClient Create()
        {
            return new DockerClientConfiguration(
                    new Uri(_engineContext.Engine.Url))
                .CreateClient();
        }
    }
}