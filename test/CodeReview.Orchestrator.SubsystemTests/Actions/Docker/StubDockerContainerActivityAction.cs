using System;
using System.Collections.Generic;
using System.Net;
using GodelTech.StoryLine;
using GodelTech.StoryLine.Contracts;
using GodelTech.StoryLine.Wiremock.Actions;
using GodelTech.StoryLine.Wiremock.Builders;
using Newtonsoft.Json;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerContainerActivityAction : IAction
    {
        public ICollection<string> Envs { private get; init; }
        public string SrcVolumeId { private get; init; }
        public string ArtVolumeId { private get; init; }
        public string ImpVolumeId { private get; init; }
        public string Image { private get; init; }
        
        public void Execute(IActor actor)
        {
            var containerId = Guid.NewGuid().ToString();
            var jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            var requestBody = CreateRequestBody();
            var responseBody = CreateResponseBody(containerId);
            
            Scenario.New()
                .When()
                .Performs<MockHttpRequest>(b => b
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
                
                .Performs<MockHttpRequest>(b => b
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
                HostConfig = new
                {
                    Mounts = new[]
                    {
                        new
                        {
                            Type = "volume",
                            Source = SrcVolumeId,
                            Target = "/src",
                            ReadOnly = new bool?()
                        },
                        new
                        {
                            Type = "volume",
                            Source = ArtVolumeId,
                            Target = "/artifacts",
                            ReadOnly = new bool?()
                        },
                        new
                        {
                            Type = "volume",
                            Source = ImpVolumeId,
                            Target = "/imports",
                            ReadOnly = new bool?(true)
                        }
                    }
                }
            };
        }
    }
}