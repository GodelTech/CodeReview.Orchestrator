using System;
using CodeReview.Orchestrator.SubsystemTests.Utils;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions
{
    public class MoveFileFromResource : IActionBuilder
    {
        private string _resourceName;
        private string _destinationFilePath;

        public MoveFileFromResource FromResource(string resourceName)
        {
            _resourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));

            return this;
        }

        public MoveFileFromResource ToPhysicalFile(string filePath)
        {
            _destinationFilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

            return this;
        }
        
        public IAction Build()
        {
            var filePath = FileHelper.GetOutputPath(_destinationFilePath);

            return new MoveFileFromResourceAction
            {
                ResourceName = _resourceName,
                FilePath = filePath
            };
        }
    }
}