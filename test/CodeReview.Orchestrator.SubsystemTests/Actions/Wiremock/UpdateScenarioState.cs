using System;
using CodeReview.Orchestrator.SubsystemTests.Models;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Wiremock
{
    public class InitWiremockScenario : IActionBuilder
    {
        private string _name;

        public InitWiremockScenario WithName(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));

            return this;
        }

        public IAction Build()
        {
            return new InitWiremockScenarioAction(_name);
        }
    }

    public class InitWiremockScenarioAction : IAction
    {
        private readonly string _name;

        public InitWiremockScenarioAction(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void Execute(IActor actor)
        {
            actor.Artifacts.Add(new ScenarioState
            {
                ScenarioName = _name
            });
        }
    }
    
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