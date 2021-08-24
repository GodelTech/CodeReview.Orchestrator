using System;
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
}