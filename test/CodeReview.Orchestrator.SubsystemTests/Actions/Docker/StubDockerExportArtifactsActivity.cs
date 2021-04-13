using System;
using System.IO;
using CodeReview.Orchestrator.SubsystemTests.Utils;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerExportArtifactsActivity : IActionBuilder
    {
        private string _image;
        private string _sourceId;
        private string _resourceName;

        public StubDockerExportArtifactsActivity WithArtifactZipResource(string resourceName)
        {
            _resourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));

            return this;
        }
        
        public StubDockerExportArtifactsActivity WithSourceId(string sourceId)
        {
            _sourceId = sourceId ?? throw new ArgumentNullException(nameof(sourceId));

            return this;
        }
        
        public StubDockerExportArtifactsActivity WithImage(string imageName)
        {
            _image = imageName ?? throw new ArgumentNullException(nameof(imageName));
            
            return this;
        }

        public IAction Build()
        {
            return new StubDockerExportActivityAction
            {
                Image = _image,
                Mounts = new[]
                {
                    new
                    {
                        Type = "volume",
                        Source = _sourceId,
                        Target = "/artifacts"
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