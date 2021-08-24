using System;
using CodeReview.Orchestrator.SubsystemTests.Models;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Wiremock
{
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
}