using System;
using CodeReview.Orchestrator.SubsystemTests.Models;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Wiremock
{
    public class UpdateScenarioStateAction : IAction
    {
        private readonly string _state;

        public UpdateScenarioStateAction(string state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public void Execute(IActor actor)
        {
            if (actor == null) throw new ArgumentNullException(nameof(actor));

            var scenarioState = actor.Artifacts.Get<ScenarioState>();

            if (scenarioState == null) return;

            scenarioState.RequiredState = _state;
        }
    }
}