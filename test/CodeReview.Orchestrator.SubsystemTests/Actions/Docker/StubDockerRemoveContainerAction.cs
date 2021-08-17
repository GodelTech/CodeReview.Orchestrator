using System.Net;
using CodeReview.Orchestrator.SubsystemTests.Actions.Wiremock;
using GodelTech.StoryLine.Contracts;
using GodelTech.StoryLine.Wiremock.Builders;
using Scenario = GodelTech.StoryLine.Scenario;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerRemoveContainerAction : WiremockScenarioAction
    {
        public bool? Force { private get; init; }
        public string ContainerId { private get; init; }
        
        public override void Execute(IActor actor)
        {
            Scenario.New()
                .When(actor)
                    .Performs<MockScenarioHttpRequest>(mock => mock
                        .Scenario(ConfigureScenario)
                        .Request(ConfigureRequest)
                        .Response(res => res
                            .Status(HttpStatusCode.OK)))
                .Run();
        }

        private void ConfigureRequest(RequestBuilder builder)
        {
            builder.UrlPath($"/containers/{ContainerId}")
                .Method("DELETE");

            if (Force.HasValue)
                builder.QueryParam("force", Force.ToString());
        }
    }
}