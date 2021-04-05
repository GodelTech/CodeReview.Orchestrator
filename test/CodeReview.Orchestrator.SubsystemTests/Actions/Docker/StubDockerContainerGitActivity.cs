using System;
using System.Collections.Generic;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerContainerGitActivity : IActionBuilder
    {
        private readonly List<string> _envs = new();
        
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

        public StubDockerContainerGitActivity WithEnv(string env)
        {
            if (env is null) throw new ArgumentNullException(nameof(env));

            _envs.Add(env);
            
            return this;
        }
        
        public IAction Build()
        {
            return new StubDockerContainerActivityAction
            {
                Envs = _envs,
                SrcVolumeId = _srcVolumeId,
                ArtVolumeId = _artVolumeId,
                ImpVolumeId = _impVolumeId,
                Image = _image
            };
        }
    }
}