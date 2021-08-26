using System;
using System.IO;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Activities
{
    public class StubExportActivity : IActionBuilder
    {
        private string _image;
        private string _sourceId;
        private string _resourceName;
        private string _targetFolder;

        public StubExportActivity WithResource(string resourceName)
        {
            _resourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));

            return this;
        }
        
        public StubExportActivity WithSourceId(string sourceId)
        {
            _sourceId = sourceId ?? throw new ArgumentNullException(nameof(sourceId));

            return this;
        }
        
        public StubExportActivity WithImage(string imageName)
        {
            _image = imageName ?? throw new ArgumentNullException(nameof(imageName));
            
            return this;
        }

        public StubExportActivity WithTargetFolder(string targetFolder)
        {
            _targetFolder = targetFolder ?? throw new ArgumentNullException(nameof(targetFolder));

            return this;
        }

        public IAction Build()
        {
            return new StubExportActivityAction
            {
                Image = _image,
                Mounts = new[]
                {
                    new
                    {
                        Type = "volume",
                        Source = _sourceId,
                        Target = _targetFolder
                    }
                },
                Resource = ReadBytesFromResource(_resourceName)
            };
        }

        private static byte[] ReadBytesFromResource(string resourceName)
        {
            using var stream = Config.Assembly.GetManifestResourceStream(Config.ResourcePath + '.' + resourceName);
            using var reader = new BinaryReader(stream);

            return reader.ReadBytes((int) stream.Length);
        }
    }
}