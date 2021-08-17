using CodeReview.Orchestrator.SubsystemTests.Actions.Wiremock;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions
{
    public abstract class WiremockScenarioAction : IAction
    {
        public string NewWiremockState { private get; init; }
        
        public abstract void Execute(IActor actor);
        
        protected void ConfigureScenario(WiremockScenarioBuilder builder)
        {
            if (NewWiremockState != null)
                builder.WillSetState(NewWiremockState);
        }
    }
}