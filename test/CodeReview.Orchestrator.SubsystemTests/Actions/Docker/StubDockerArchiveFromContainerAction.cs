using System.Net;
using GodelTech.StoryLine;
using GodelTech.StoryLine.Contracts;
using GodelTech.StoryLine.Wiremock.Actions;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerArchiveFromContainerAction : IAction
    {
        public string ContainerId { private get; init; }
        public string SourcePath { private get; init; }
        public byte[] Resource { private get; init; }

        public void Execute(IActor actor)
        {
            Scenario.New()
                .When(actor)
                .Performs<MockHttpRequest>(b => b
                    .Request(req => req
                        .UrlPath($"/containers/{ContainerId}/archive")
                        .QueryParam("path", SourcePath)
                        .Method("GET"))
                    .Response(res => res
                        .Header("X-Docker-Container-Path-Stat", string.Empty)
                        .BinaryBody(Resource)
                        .Status(HttpStatusCode.OK)))
                .Run();
        }
    }
}