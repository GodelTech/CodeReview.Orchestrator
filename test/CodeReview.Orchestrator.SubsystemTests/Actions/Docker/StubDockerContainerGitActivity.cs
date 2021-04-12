using System;
using System.Collections.Generic;
using System.Linq;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerContainerGitActivity : IActionBuilder
    {
        private readonly Dictionary<string, string> _envVariables = new();
        
        private string _srcVolumeId;
        private string _artVolumeId;
        private string _impVolumeId;
        private string _image;

        public StubDockerContainerGitActivity WithSrcVolumeId(string srcVolumeId)
        {
            _srcVolumeId = srcVolumeId ?? throw new ArgumentNullException(nameof(srcVolumeId));

            return this;
        }
        
        public StubDockerContainerGitActivity WithArtVolumeId(string artVolumeId)
        {
            _artVolumeId = artVolumeId ?? throw new ArgumentNullException(nameof(artVolumeId));

            return this;
        }
        
        public StubDockerContainerGitActivity WithImpVolumeId(string impVolumeId)
        {
            _impVolumeId = impVolumeId ?? throw new ArgumentNullException(nameof(impVolumeId));

            return this;
        }
        
        public StubDockerContainerGitActivity WithImage(string image)
        {
            _image = image ?? throw new ArgumentNullException(nameof(image));

            return this;
        }

        public StubDockerContainerGitActivity WithEnv(string variableName, string value)
        {
            if (variableName is null) throw new ArgumentNullException(nameof(variableName));

            _envVariables[variableName] = value;
            
            return this;
        }
        
        public IAction Build()
        {
            return new StubDockerContainerActivityAction
            {
                Envs = _envVariables.Select(p => $"{p.Key}={p.Value}").ToList(),
                SrcVolumeId = _srcVolumeId,
                ArtVolumeId = _artVolumeId,
                ImpVolumeId = _impVolumeId,
                Image = _image
            };
        }
    }
}