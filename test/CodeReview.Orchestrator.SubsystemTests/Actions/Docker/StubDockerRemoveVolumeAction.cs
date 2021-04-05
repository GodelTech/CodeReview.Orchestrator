using System;
using System.Net;
using GodelTech.StoryLine;
using GodelTech.StoryLine.Contracts;
using GodelTech.StoryLine.Wiremock.Actions;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerRemoveVolumeAction : IAction
    {
        public string VolumeId { private get; init; }
        
        public void Execute(IActor actor)
        {
            Scenario.New()
                .When()
                .Performs<MockHttpRequest>(b => b
                    .Request(req => req
                        .UrlPath(Uri.EscapeUriString($"/volumes/{VolumeId}"))
                        .Method("DELETE"))
                    .Response(res => res
                        .Status(HttpStatusCode.OK)))
                .Run();
        }
    }
}