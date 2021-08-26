using System;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerRemoveVolume : WiremockScenarioActionBuilder<StubDockerRemoveVolume>
    {
        private string _volumeId; 

        public StubDockerRemoveVolume WithVolumeId(string volumeId)
        {
            _volumeId = volumeId ?? throw new ArgumentNullException(nameof(volumeId));

            return this;
        }
        
        public override IAction Build()
        {
            return new StubDockerRemoveVolumeAction
            {
                VolumeId = _volumeId,
                NewWiremockState = NewWiremockState
            };
        }
    }
}