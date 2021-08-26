using System;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Wiremock
{
    public class UpdateScenarioState : IActionBuilder
    {
        private string _state = "Started";

        public UpdateScenarioState State(string state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));

            return this;
        }
        
        public IAction Build()
        {
            return new UpdateScenarioStateAction(_state);
        }
    }
}