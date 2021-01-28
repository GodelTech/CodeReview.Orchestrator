﻿using System;

namespace ReviewItEasy.Orchestrator.Services
{
    public class EnvironmentVariableValueProvider : IEnvironmentVariableValueProvider
    {
        public string Get(string environmentVariableName)
        {
            if (string.IsNullOrWhiteSpace(environmentVariableName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(environmentVariableName));
            
            return Environment.GetEnvironmentVariable(environmentVariableName);
        }
    }
}