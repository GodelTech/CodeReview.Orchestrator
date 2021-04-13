using System.IO;
using System.Text;
using CodeReview.Orchestrator.SubsystemTests.Utils;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Expectations
{
    public class ProcessResult : IExpectationBuilder
    {
        private int _exitCode;
        private (string path, string content) _fileInfo;
        
        public ProcessResult ExitCode(int exitCode)
        {
            _exitCode = exitCode;
            
            return this;
        }

        public ProcessResult FileEqualToTheResource(string filePath, string resourceName)
        {
            var content = ReadFromResource(resourceName);
            _fileInfo = new(filePath, content);

            return this;
        }

        public ProcessResult FileContain(string filePath, string content)
        {
            _fileInfo = new(filePath, content);

            return this;
        }
        
        public IExpectation Build()
        {
            return new ProcessResultExpectation(_exitCode)
            {
                FileInfo = _fileInfo
            };
        }
        
        private static string ReadFromResource(string fileName)
        {
            using var stream = Config.Assembly.GetManifestResourceStream(Config.ResourcePath + '.' + fileName);
            using var reader = new StreamReader(stream, Encoding.UTF8);

            return reader.ReadToEnd();
        }
    }
}