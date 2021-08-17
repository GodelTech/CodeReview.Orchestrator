using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CodeReview.Orchestrator.SubsystemTests.Actions.Wiremock;
using CodeReview.Orchestrator.SubsystemTests.Models;
using GodelTech.StoryLine.Contracts;
using GodelTech.StoryLine.Utils.Actions;
using GodelTech.StoryLine.Wiremock.Builders;
using Newtonsoft.Json;
using Scenario = GodelTech.StoryLine.Scenario;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerCreateContainerAction : IAction
    {
        private readonly string _image;
        private readonly ICollection<string> _commandLineArguments;
        private readonly IDictionary<string, string> _envVariables;
        private readonly ICollection<MountedVolume> _mountedVolumes;

        public StubDockerCreateContainerAction(
            string image,
            ICollection<string> commandLineArguments, 
            IDictionary<string, string> envVariables, 
            ICollection<MountedVolume> mountedVolumes)
        {
            _image = image ?? throw new ArgumentNullException(nameof(image));
            _commandLineArguments = commandLineArguments ?? throw new ArgumentNullException(nameof(commandLineArguments));
            _envVariables = envVariables ?? throw new ArgumentNullException(nameof(envVariables));
            _mountedVolumes = mountedVolumes ?? throw new ArgumentNullException(nameof(mountedVolumes));
        }


        public void Execute(IActor actor)
        {
            var containerId = new ContainerId {Id = $"{Guid.NewGuid().ToString()}-created"};
            var jsonSettings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};

            var requestBody = CreateRequestBody();
            
            Scenario.New()
                .When(actor)
                    .Performs<MockScenarioHttpRequest>(mock => mock
                        .Request(req => req
                            .UrlPath("/containers/create")
                            .Body()
                            .EqualToJsonObjectBody(requestBody, jsonSettings)
                            .Method("POST"))
                        .Response(res => res
                            .JsonObjectBody(containerId, jsonSettings)
                            .Status(HttpStatusCode.Created)))
                    .Performs<AddArtifact>(art => art.Value(containerId))
                .Run();
                        
        }

        private object CreateRequestBody()
        {
            return new
            {
                Env = _envVariables.Select(x => x.Key + "=" + x.Value).ToArray(),
                Cmd = _commandLineArguments,
                Image = _image,
                HostConfig = new
                {
                    Mounts = _mountedVolumes
                }
            };
        }
    }
}