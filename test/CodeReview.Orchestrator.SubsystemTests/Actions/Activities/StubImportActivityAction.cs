using System;
using System.Collections.Immutable;
using System.Net;
using CodeReview.Orchestrator.SubsystemTests.Actions.Wiremock;
using GodelTech.StoryLine.Contracts;
using GodelTech.StoryLine.Wiremock.Actions;
using GodelTech.StoryLine.Wiremock.Builders;
using Newtonsoft.Json;
using Scenario = GodelTech.StoryLine.Scenario;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Activities
{
    public class StubImportActivityAction : WiremockScenarioAction
    {
        public object Mounts { private get; init; }
        public string Image { private get; init; }
        
        
        public override void Execute(IActor actor)
        {
            var containerId = $"{Guid.NewGuid().ToString()}-import";
            var jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            
            var requestBody = CreateRequestBody();
            var responseBody = CreateResponseBody(containerId);
            
            Scenario.New()
                .When(actor)
                    .Performs<MockScenarioHttpRequest>(b => b
                        .Request(req => req
                            .UrlPath("/containers/create")
                            .Body()
                            .EqualToJsonObjectBody(requestBody, jsonSettings)
                            .Method("POST"))
                        .Response(res => res
                            .JsonObjectBody(responseBody, jsonSettings)
                            .Status(HttpStatusCode.Created)))
                    .Performs<MockHttpRequest>(b => b
                        .Request(req => req
                            .UrlPath($"/containers/{containerId}/archive")
                            .Method("PUT"))
                        .Response(res => res
                            .Status(HttpStatusCode.OK)))
                    .Performs<MockScenarioHttpRequest>(b => b
                        .Scenario(ConfigureScenario)
                        .Request(req => req
                            .UrlPath($"/containers/{containerId}")
                            .Method("DELETE"))
                        .Response(res => res
                            .Status(HttpStatusCode.OK)))
                .Run();
        }

        private object CreateRequestBody()
        {
            return new
            {
                Env = ImmutableList<string>.Empty,
                Cmd = ImmutableList<string>.Empty,
                Image = Image,
                HostConfig = new
                {
                    Mounts = Mounts
                }
            };
        }
        
        private static object CreateResponseBody(string containerId)
        {
            return new
            {
                Id = containerId
            };
        }
    }
}