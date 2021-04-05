using System;
using System.Collections.Generic;
using System.Text;
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

    public class MoveFileFromResourceAction : IAction
    {
        public string ResourceName { private get; init; }
        public string FilePath { private get; init; }
        
        public void Execute(IActor actor)
        {
            FileHelper.CopyFromResource(ResourceName, FilePath);
        }
    }
    
    public class ExecuteProcess : IActionBuilder
    {
        private readonly Dictionary<string, string> _commandArgs = new();
        
        private TimeSpan _timeout = TimeSpan.FromSeconds(10);

        public ExecuteProcess WithCommandArg(string key, string value = "")
        {
            _commandArgs[key] = value;
            
            return this;
        }

        public ExecuteProcess SetTimeout(TimeSpan timeSpan)
        {
            _timeout = timeSpan;

            return this;
        }
        
        public IAction Build()
        {
            var builder = new StringBuilder();

            foreach (var (argumentKey, argumentValue) in _commandArgs) 
                builder.Append($"{argumentKey} {argumentValue} ");

            return new RunProcessAction(builder.ToString(), _timeout);
        }
    }
}