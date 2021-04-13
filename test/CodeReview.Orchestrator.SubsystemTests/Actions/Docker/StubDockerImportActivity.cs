using System;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public abstract class StubDockerImportActivity<TBuilder> : IActionBuilder
        where TBuilder : StubDockerImportActivity<TBuilder>
    {
        private string _image;
        
        protected abstract object Mounts { get; }

        public TBuilder WithImage(string imageName)
        {
            _image = imageName ?? throw new ArgumentNullException(nameof(imageName));
            
            return (TBuilder)this;
        }
        
        public virtual IAction Build()
        {
            return new StubDockerImportActivityAction
            {
                Image = _image,
                Mounts = Mounts
            };
        }
    }
}