using System;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions
{
    public abstract class WiremockScenarioActionBuilder<TBuilder> : IActionBuilder
        where TBuilder : WiremockScenarioActionBuilder<TBuilder>
    {
        protected string NewWiremockState { get; set; }

        public virtual TBuilder SetWiremockState(string state)
        {
            NewWiremockState = state ?? throw new ArgumentNullException(nameof(state));

            return (TBuilder)this;
        }

        public abstract IAction Build();
    }
}