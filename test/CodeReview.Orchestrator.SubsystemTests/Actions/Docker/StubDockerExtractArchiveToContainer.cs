using System;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerExtractArchiveToContainer : IActionBuilder
    {
        private string _containerId = string.Empty;
        private string _dockerPath = string.Empty;
        private bool _allowOverwriteDirWithFile;

        public StubDockerExtractArchiveToContainer WithContainerId(string containerId)
        {
            _containerId = containerId ?? throw new ArgumentNullException(nameof(containerId));

            return this;
        }

        public StubDockerExtractArchiveToContainer WithDockerPath(string dockerPath)
        {
            _dockerPath = dockerPath ?? throw new ArgumentNullException(nameof(dockerPath));

            return this;
        }

        public StubDockerExtractArchiveToContainer AllowOverwriteDirWithFile(bool allowOverwrite)
        {
            _allowOverwriteDirWithFile = allowOverwrite;

            return this;
        }
        
        public IAction Build()
        {
            return new StubDockerExtractArchiveToContainerAction
            {
                ContainerId = _containerId,
                DockerPath = _dockerPath,
                AllowOverwriteDirWithFile = _allowOverwriteDirWithFile
            };
        }
    }
}