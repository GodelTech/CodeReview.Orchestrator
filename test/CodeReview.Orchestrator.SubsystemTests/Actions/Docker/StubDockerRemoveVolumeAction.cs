using System;
using System.Net;
using CodeReview.Orchestrator.SubsystemTests.Actions.Wiremock;
using GodelTech.StoryLine.Contracts;
using GodelTech.StoryLine.Wiremock.Actions;
using Scenario = GodelTech.StoryLine.Scenario;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerRemoveVolumeAction : WiremockScenarioAction
    {
        public string VolumeId { private get; init; }
        
        public override void Execute(IActor actor)
        {
            Scenario.New()
                .When(actor)
                    .Performs<MockScenarioHttpRequest>(b => b
                        .Scenario(ConfigureScenario)
                        .Request(req => req
                            .UrlPath(Uri.EscapeUriString($"/volumes/{VolumeId}"))
                            .Method("DELETE"))
                        .Response(res => res
                            .Status(HttpStatusCode.OK)))
                .Run();
        }
        
        
    }
}