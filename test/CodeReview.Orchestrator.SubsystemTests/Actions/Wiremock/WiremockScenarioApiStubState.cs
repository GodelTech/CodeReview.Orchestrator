using System;
using GodelTech.StoryLine.Wiremock.Builders;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Wiremock
{
    public class WiremockScenarioApiStubState : ApiStubState
    {
        private WiremockScenario _wiremockScenarioState = new();

        public WiremockScenario WiremockScenarioState
        {
            get => _wiremockScenarioState;
            set => _wiremockScenarioState = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}