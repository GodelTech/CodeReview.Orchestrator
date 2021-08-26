using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CodeReview.Orchestrator.SubsystemTests.Actions.Wiremock;
using CodeReview.Orchestrator.SubsystemTests.Models;
using GodelTech.StoryLine.Contracts;
using GodelTech.StoryLine.Wiremock.Actions;
using GodelTech.StoryLine.Wiremock.Builders;
using Newtonsoft.Json;
using Scenario = GodelTech.StoryLine.Scenario;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Activities
{
    public class StubContainerActivityAction : WiremockScenarioAction
    {
        public ICollection<MountedVolume> MountedVolumes { private get; init; }
        public ICollection<string> Commands { private get; init; }
        public ICollection<string> Envs { private get; init; }
        public string Image { private get; init; }
        
        public override void Execute(IActor actor)
        {
            var containerId = Guid.NewGuid().ToString();
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
                            .UrlPath($"/containers/{containerId}/start")
                            .Method("POST"))
                        .Response(res => res
                            .Status(HttpStatusCode.NotModified)))
                    
                    .Performs<MockHttpRequest>(b => b
                        .Request(req => req
                            .UrlPath($"/containers/{containerId}/wait")
                            .Method("POST"))
                        .Response(res => res
                            .JsonObjectBody(CreateWaitResponseBody(), jsonSettings)
                            .Status(HttpStatusCode.OK)))
                    
                    .Performs<MockHttpRequest>(b => b
                        .Request(req => req
                            .UrlPath($"/containers/{containerId}/logs")
                            .Method("GET"))
                        .Response(res => res
                            .BinaryBody(Array.Empty<byte>())
                            .Status(HttpStatusCode.OK)))
                    
                    .Performs<MockHttpRequest>(b => b
                        .Request(req => req
                            .UrlPath($"/containers/{containerId}/json")
                            .Method("GET"))
                        .Response(res => res
                            .JsonObjectBody(CreateInspectContainerResponseBody(containerId))
                            .Status(HttpStatusCode.OK)))
                    
                    .Performs<MockHttpRequest>(b => b
                        .Request(req => req
                            .UrlPath($"/containers/{containerId}/stop")
                            .Method("POST"))
                        .Response(res => res
                            .Status(HttpStatusCode.NotModified)))
                    
                    .Performs<MockScenarioHttpRequest>(b => b
                        .Scenario(ConfigureScenario)
                        .Request(req => req
                            .UrlPath($"/containers/{containerId}")
                            .Method("DELETE"))
                        .Response(res => res
                            .Status(HttpStatusCode.OK)))
                .Run();
        }

        private static object CreateInspectContainerResponseBody(string containerId)
        {
            return new
            {
                ID = containerId,
                State = new
                {
                    Status = "status",
                    ExitCode = 0
                }
            };
        }
        
        private static object CreateWaitResponseBody()
        {
            return new
            {
                StatusCode = 0
            };
        }
        
        private static object CreateResponseBody(string containerId)
        {
            return new
            {
                Id = containerId
            };
        }

        private object CreateRequestBody()
        {
            return new
            {
                Env = Envs,
                Image = Image,
                Cmd = Commands.Any() ? Commands : null,
                HostConfig = new
                {
                    Mounts = MountedVolumes
                }
            };
        }
    }
}