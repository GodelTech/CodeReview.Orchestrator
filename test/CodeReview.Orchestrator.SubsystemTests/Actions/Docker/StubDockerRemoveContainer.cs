using System;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerRemoveContainer : WiremockScenarioActionBuilder<StubDockerRemoveContainer>
    {
        private bool? _force;
        private string _containerId;
        
        public StubDockerRemoveContainer WithId(string containerId)
        {
            _containerId = containerId ?? throw new ArgumentNullException(nameof(containerId));
            
            return this;
        }
        
        public StubDockerRemoveContainer WithForce(bool force)
        {
            _force = force;

            return this;
        }
        
        public override IAction Build()
        {
            return new StubDockerRemoveContainerAction
            {
                Force = _force,
                ContainerId = _containerId,
                NewWiremockState = NewWiremockState
            };
        }
    }
}