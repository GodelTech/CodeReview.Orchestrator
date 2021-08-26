using System;
using System.Collections.Generic;
using System.Linq;
using CodeReview.Orchestrator.SubsystemTests.Models;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Activities
{
    public class StubContainerActivity : IActionBuilder
    {
        private readonly Dictionary<string, string> _envVariables = new();
        private readonly List<MountedVolume> _mountedVolumes = new();
        private readonly List<string> _commands = new();

        private string _newState;
        private string _image;

        public StubContainerActivity SetWiremockState(string state)
        {
            _newState = state ?? throw new ArgumentNullException(nameof(state));

            return this;
        }
        
        public StubContainerActivity WithMountedVolume(Action<MountedVolumeBuilder> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var mountedVolumeBuilder = new MountedVolumeBuilder();
            action(mountedVolumeBuilder);

            _mountedVolumes.Add(mountedVolumeBuilder.Build());
            
            return this;
        }
        
        public StubContainerActivity WithImage(string image)
        {
            _image = image ?? throw new ArgumentNullException(nameof(image));

            return this;
        }

        public StubContainerActivity WithCommands(params string[] commands)
        {
            if (commands == null) throw new ArgumentNullException(nameof(commands));
            
            _commands.AddRange(commands);

            return this;
        }

        public StubContainerActivity WithEnv(string variableName, string value)
        {
            if (variableName is null) throw new ArgumentNullException(nameof(variableName));

            _envVariables[variableName] = value;
            
            return this;
        }
        
        public IAction Build()
        {
            return new StubContainerActivityAction
            {
                Envs = _envVariables.Select(p => $"{p.Key}={p.Value}").ToList(),
                MountedVolumes = _mountedVolumes,
                NewWiremockState = _newState,
                Commands = _commands,
                Image = _image
            };
        }
    }
}