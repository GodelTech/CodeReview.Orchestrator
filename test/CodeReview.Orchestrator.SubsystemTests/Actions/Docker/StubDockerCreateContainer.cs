using System;
using System.Collections.Generic;
using CodeReview.Orchestrator.SubsystemTests.Models;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerCreateContainer : IActionBuilder
    {
        private readonly List<string> _commandLineArguments = new();
        private readonly List<MountedVolume> _mountedVolumes = new();
        private readonly Dictionary<string, string> _envVariables = new();
        
        private string _image = string.Empty;

        public StubDockerCreateContainer WithImage(string image)
        {
            if (_image == null) throw new ArgumentNullException(nameof(image));
            
            _image = image;

            return this;
        }
        
        public StubDockerCreateContainer WithMountedVolume(Action<MountedVolumeBuilder> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var builder = new MountedVolumeBuilder();
            action(builder);

            _mountedVolumes.Add(builder.Build());
            
            return this;
        }

        public StubDockerCreateContainer WithCommandLineArguments(params string[] arguments)
        {
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));

            _commandLineArguments.AddRange(arguments);
            
            return this;
        }

        public StubDockerCreateContainer WithEnvVariable(string name, string value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));
            
            _envVariables.Add(name, value);

            return this;
        }
        
        public IAction Build()
        {
            return new StubDockerCreateContainerAction(
                _image,
                _commandLineArguments,
                _envVariables,
                _mountedVolumes);
        }
    }
}