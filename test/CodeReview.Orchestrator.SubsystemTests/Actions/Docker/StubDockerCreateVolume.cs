using System;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerCreateVolume : IActionBuilder
    {
        private string _volumePrefix;
        private string _volumeId;

        public StubDockerCreateVolume WithVolumePrefix(string volumePrefix)
        {
            _volumePrefix = volumePrefix ?? throw new ArgumentNullException(nameof(volumePrefix));
            
            return this;
        }

        public StubDockerCreateVolume WithResponseVolumeId(string volumeId)
        {
            _volumeId = volumeId ?? throw new ArgumentNullException(nameof(volumeId));

            return this;
        }
        
        public IAction Build()
        {
            return new StubDockerCreateVolumeAction
            {
                VolumeId = _volumeId,
                VolumePrefix = _volumePrefix
            };
        }
    }
}