using System;
using CodeReview.Orchestrator.SubsystemTests.Models;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Activities
{
    public class StubImportActivity : WiremockScenarioActionBuilder<StubImportActivity>, IActionBuilder
    {
        private readonly MountedVolume _mountedVolume = new();

        private string _image;
        
        public StubImportActivity WithImage(string image)
        {
            _image = image ?? throw new ArgumentNullException(nameof(image));

            return this;
        }

        public StubImportActivity WithSourceId(string sourceId)
        {
            _mountedVolume.Source = sourceId ?? throw new ArgumentNullException(nameof(sourceId));

            return this;
        }

        public StubImportActivity WithTargetFolder(string targetFolder)
        {
            _mountedVolume.Target = targetFolder ?? throw new ArgumentNullException(nameof(targetFolder));

            return this;
        }
        
        public override IAction Build()
        {
            return new StubImportActivityAction
            {
                Image = _image,
                NewWiremockState = NewWiremockState,
                Mounts = new[]
                {
                    _mountedVolume
                }
            };
        }
    }
}