using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CodeReview.Orchestrator.Services
{
    public class VariableExpressionProvider : IVariableExpressionProvider
    {
        private IReadOnlyDictionary<string, string> _variableExpressions = ImmutableDictionary<string, string>.Empty;

        public string Get(string variable)
        {
            if (_variableExpressions.TryGetValue(variable, out var value))
                return value;
            
            return string.Empty;
        }

        public void SetVariables(IReadOnlyDictionary<string, string> expressionValues)
        {
            if (expressionValues == null) 
                throw new ArgumentNullException(nameof(expressionValues));
            
            _variableExpressions = expressionValues.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
        }
    }
}