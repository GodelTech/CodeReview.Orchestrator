using System;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Wiremock
{
    public class WiremockScenarioBuilder
    {
        private readonly WiremockScenarioApiStubState _apiStubState;

        public WiremockScenarioBuilder(WiremockScenarioApiStubState apiStubState)
        {
            _apiStubState = apiStubState ?? throw new ArgumentNullException(nameof(apiStubState));
        }
        
        public WiremockScenarioBuilder Name(string name)
        {
            _apiStubState.WiremockScenarioState.Name = name ?? throw new ArgumentNullException(nameof(name));
            
            return this;
        }

        public WiremockScenarioBuilder WhenState(string scenarioState)
        {
            _apiStubState.WiremockScenarioState.WhenState = scenarioState ?? throw new ArgumentNullException(nameof(scenarioState));
            
            return this;
        }

        public WiremockScenarioBuilder WillSetState(string scenarioState)
        {
            _apiStubState.WiremockScenarioState.WillSetState = scenarioState ?? throw new ArgumentNullException(nameof(scenarioState));
            
            return this;
        }
    }
}