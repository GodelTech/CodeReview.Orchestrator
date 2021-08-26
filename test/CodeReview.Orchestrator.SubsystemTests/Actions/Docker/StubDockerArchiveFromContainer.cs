using System;
using System.IO;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerArchiveFromContainer : IActionBuilder
    {
        private string _containerId = string.Empty;
        private string _dockerPath = string.Empty;
        private string _resourceName = string.Empty;
        
        public StubDockerArchiveFromContainer WithContainerId(string containerId)
        {
            _containerId = containerId ?? throw new ArgumentNullException(nameof(containerId));
            
            return this;
        }

        public StubDockerArchiveFromContainer WithDockerPath(string dockerPath)
        {
            _dockerPath = dockerPath ?? throw new ArgumentNullException(nameof(dockerPath));
            
            return this;
        }

        public StubDockerArchiveFromContainer WithResource(string resourceName)
        {
            _resourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
            
            return this;
        }
        
        public IAction Build()
        {
            return new StubDockerArchiveFromContainerAction
            {
                ContainerId = _containerId,
                SourcePath = _dockerPath,
                Resource = ReadBytesFromResource(_resourceName)
            };
        }
        
        private static byte[] ReadBytesFromResource(string resourceName)
        {
            using var stream = Config.Assembly.GetManifestResourceStream(Config.ResourcePath + '.' + resourceName);
            using var reader = new BinaryReader(stream);

            return reader.ReadBytes((int) stream.Length);
        }
    }
}