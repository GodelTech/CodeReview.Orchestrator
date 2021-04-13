using System.IO;
using System.Text;
using GodelTech.StoryLine.Contracts;
using GodelTech.StoryLine.Exceptions;

namespace CodeReview.Orchestrator.SubsystemTests.Expectations
{
    public class ProcessResultExpectation : IExpectation
    {
        private readonly int _exitCode;
        
        public (string Path, string Content) FileInfo { get; set; }

        public ProcessResultExpectation(int exitCode)
        {
            _exitCode = exitCode;
        }

        public void Validate(IActor actor)
        {
            var artifact = actor.Artifacts.GetAll<ProcessResultArtifact>()[0];

            ValidateExitCode(artifact);
            ValidateFileContent();
        }

        private void ValidateExitCode(ProcessResultArtifact artifact)
        {
            if (artifact.ExitCode != _exitCode)
                throw new ExpectationException($"Exit code should be {_exitCode} but found {artifact.ExitCode}.");
        }

        private void ValidateFileContent()
        {
            if (FileInfo == default)
                return;

            if (!File.Exists(FileInfo.Path))
                throw new ExpectationException($"File with path '{FileInfo.Path}' does not exist.");

            var fileContent = File.ReadAllText(FileInfo.Path, Encoding.UTF8);

            if (fileContent.Length != FileInfo.Content.Length)
                throw new ExpectationException($"File content length expected to be {FileInfo.Content.Length} but found {fileContent.Length}");
            
            for (int i = 0; i < fileContent.Length; i++)
            {
                if (fileContent[i] != FileInfo.Content[i])
                    throw new ExpectationException($"File content does not match at the {i} position.");
            }
        }
    }
}