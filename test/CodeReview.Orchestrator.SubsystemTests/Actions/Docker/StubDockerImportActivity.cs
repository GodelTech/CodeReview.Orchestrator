using System;
using CodeReview.Orchestrator.SubsystemTests.Models;
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

    public class StubDockerImportActivity : IActionBuilder
    {
        private readonly MountedVolume _mountedVolume = new();
        
        private string _image;

        public StubDockerImportActivity WithImage(string image)
        {
            _image = image ?? throw new ArgumentNullException(nameof(image));

            return this;
        }

        public StubDockerImportActivity WithSourceId(string sourceId)
        {
            _mountedVolume.Source = sourceId ?? throw new ArgumentNullException(nameof(sourceId));

            return this;
        }

        public StubDockerImportActivity WithTargetFolder(string targetFolder)
        {
            _mountedVolume.Target = targetFolder ?? throw new ArgumentNullException(nameof(targetFolder));

            return this;
        }
        
        public IAction Build()
        {
            return new StubDockerImportActivityAction
            {
                Image = _image,
                Mounts = new[]
                {
                    _mountedVolume
                }
            };
        }
    }
}