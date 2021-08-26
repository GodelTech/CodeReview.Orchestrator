using System.Net;
using GodelTech.StoryLine;
using GodelTech.StoryLine.Contracts;
using GodelTech.StoryLine.Wiremock.Actions;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerExtractArchiveToContainerAction : IAction
    {
        public string ContainerId { private get; init; }
        public string DockerPath { private get; init; }
        public bool AllowOverwriteDirWithFile { private get; init; }
        
        public void Execute(IActor actor)
        {
            Scenario.New()
                .When(actor)
                .Performs<MockHttpRequest>(mock => mock
                    .Request(req => req
                        .UrlPath($"/containers/{ContainerId}/archive")
                        .QueryParam("path", DockerPath)
                        .QueryParam("AllowOverwriteDirWithFile", AllowOverwriteDirWithFile.ToString())
                        .Method("PUT"))
                    .Response(res => res
                        .Status(HttpStatusCode.OK)))
                .Run();
        }
    }
}