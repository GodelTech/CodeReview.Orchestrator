using System;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerPullImage : IActionBuilder
    {
        private string _imageName;
        private string _tag = "latest";

        public StubDockerPullImage WithImage(string imageName)
        {
            _imageName = imageName ?? throw new ArgumentNullException(nameof(imageName));
            
            return this;
        }

        public StubDockerPullImage WithTag(string tag)
        {
            _tag = tag ?? throw new ArgumentNullException(nameof(tag));

            return this;
        }
        
        public IAction Build()
        {
            return new StubDockerPullImageAction
            {
                Image = _imageName,
                Tag = _tag
            };
        }
    }
}