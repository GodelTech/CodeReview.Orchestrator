using GodelTech.StoryLine.Wiremock.Services.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Wiremock
{
    public class WiremockScenarioMapping : Mapping
    {
        public string ScenarioName { get; set; }
        public string NewScenarioState { get; set; }
        public string RequiredScenarioState { get; set; }
    }
}