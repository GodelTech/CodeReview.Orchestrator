using System.Net;
using GodelTech.StoryLine;
using GodelTech.StoryLine.Contracts;
using GodelTech.StoryLine.Wiremock.Actions;
using GodelTech.StoryLine.Wiremock.Builders;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerCreateVolumeAction : IAction
    {
        public string VolumePrefix { private get; init; }
        public string VolumeId { private get; init; }
        
        public void Execute(IActor actor)
        {
            var responseBody = CreateResponseBody();
            
            Scenario.New()
                .When()
                .Performs<MockHttpRequest>(b => b
                    .Request(req => req
                        .UrlPath("/volumes/create")
                        .Body()
                        .MatchingJsonPath($"$[?(@.Name =~ /{VolumePrefix}-.*/)]")
                        .Method("POST"))
                    .Response(res => res
                        .JsonObjectBody(responseBody)
                        .Status(HttpStatusCode.Created)))
                .Run();
        }

        private object CreateResponseBody()
        {
            return new
            {
                Name = VolumeId
            };
        }
    }
}