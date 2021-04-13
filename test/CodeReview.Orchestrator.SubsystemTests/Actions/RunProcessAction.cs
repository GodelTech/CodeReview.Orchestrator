using System;
using System.Diagnostics;
using GodelTech.StoryLine.Contracts;
using GodelTech.StoryLine.Exceptions;

namespace CodeReview.Orchestrator.SubsystemTests.Actions
{
    public class RunProcessAction : IAction
    {
        private readonly TimeSpan _timeout;
        private readonly string _commandArgs;

        public RunProcessAction(string commandArgs, TimeSpan timeout)
        {
            _commandArgs = commandArgs ?? string.Empty;
            _timeout = timeout;
        }
        
        public void Execute(IActor actor)
        {
            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = Config.ExePath,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = _commandArgs
            };
            
            using var process = Process.Start(startInfo);

            if (process is null) 
                throw new ExpectationException($"The process cannot be started. Please check the path '{Config.ExePath}'");
            
            process.WaitForExit((int)_timeout.TotalMilliseconds);
            
            if (!process.HasExited)
            {
                process.Kill();
                throw new ExpectationException("The process was forcibly closed. Timeout error.");
            }

            var artifact = new ProcessResultArtifact
            {
                ExitCode = process.ExitCode
            };
            
            actor.Artifacts.Add(artifact);
        }
    }
}