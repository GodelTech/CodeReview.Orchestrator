using System.Net;
using GodelTech.StoryLine;
using GodelTech.StoryLine.Contracts;
using GodelTech.StoryLine.Wiremock.Actions;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerImageAction : IAction
    {
        public string ImageName { private get; init; }
        public bool IsExist { private get; init; }
        
        public void Execute(IActor actor)
        {
            var rawJson = "{\"reference\":{\""+ ImageName +"\":true}}";
            var responseBody = IsExist
                ? "[{\"Id\":\"" + ImageName + "\"}]"
                : "[]";

            Scenario.New()
                .When()
                .Performs<MockHttpRequest>(b => b
                    .Request(req => req
                        .UrlPath("/images/json")
                        .QueryParam("filters", rawJson)
                        .Method("GET"))
                    .Response(res => res
                        .Body(responseBody)
                        .Status(HttpStatusCode.OK)))
                .Run();
        }
    }
}