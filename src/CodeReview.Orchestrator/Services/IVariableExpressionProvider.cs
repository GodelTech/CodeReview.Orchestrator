using System.Collections.Generic;

namespace ReviewItEasy.Orchestrator.Services
{
    public interface IVariableExpressionProvider
    {
        string Get(string variable);
        void SetVariables(IReadOnlyDictionary<string, string> expressionValues);
    }
}