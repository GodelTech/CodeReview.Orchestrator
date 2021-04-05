using System;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerImageExist : IActionBuilder
    {
        private string _imageName;
        private bool _isExist;
        
        public StubDockerImageExist WithImage(string imageName)
        {
            _imageName = imageName ?? throw new ArgumentNullException(nameof(imageName));
            
            return this;
        }

        public StubDockerImageExist IsExist(bool isExists)
        {
            _isExist = isExists;
            
            return this;
        }
        
        public IAction Build()
        {
            return new StubDockerImageAction
            {
                ImageName = _imageName,
                IsExist = _isExist
            };
        }
    }
}