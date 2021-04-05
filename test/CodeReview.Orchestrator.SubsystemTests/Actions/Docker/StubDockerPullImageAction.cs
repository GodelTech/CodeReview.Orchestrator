using System.Net;
using GodelTech.StoryLine;
using GodelTech.StoryLine.Contracts;
using GodelTech.StoryLine.Wiremock.Actions;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerPullImageAction : IAction
    {
        public string Image { private get; init; }
        public string Tag { private get; init; }
        
        public void Execute(IActor actor)
        {
            Scenario.New()
                .When()
                .Performs<MockHttpRequest>(b => b
                    .Request(req => req
                        .UrlPath("/images/create")
                        .QueryParam("fromImage", Image)
                        .QueryParam("tag", Tag)
                        .Method("POST"))
                    .Response(res => res
                        .Status(HttpStatusCode.OK)))
                .Run();
        }
    }
}