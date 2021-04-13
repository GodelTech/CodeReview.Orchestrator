using System;
using System.Collections.Generic;
using System.Text;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions
{
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