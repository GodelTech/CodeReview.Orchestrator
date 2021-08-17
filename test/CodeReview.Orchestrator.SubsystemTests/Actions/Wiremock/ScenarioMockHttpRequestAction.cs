using System;
using CodeReview.Orchestrator.SubsystemTests.Models;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Wiremock
{
    public class ScenarioMockHttpRequestAction : IAction
    {
        private readonly WiremockScenarioApiStubState _state;

        public ScenarioMockHttpRequestAction(WiremockScenarioApiStubState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public void Execute(IActor actor)
        {
            var scenarioState = actor.Artifacts.Get<ScenarioState>();
            if (scenarioState != null)
            {
                _state.WiremockScenarioState.Name = scenarioState.ScenarioName;
                _state.WiremockScenarioState.WhenState = scenarioState.RequiredState;
            }
            
            GodelTech.StoryLine.Wiremock.Config.Client.Create(new WiremockScenarioMapping
            {
                Request = _state.RequestState,
                Response = _state.ResponseState,
                ScenarioName = _state.WiremockScenarioState.Name,
                NewScenarioState = _state.WiremockScenarioState.WillSetState,
                RequiredScenarioState = _state.WiremockScenarioState.WhenState
            });
        }
    }
}