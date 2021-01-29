using System;
using System.Text.RegularExpressions;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class ExpressionEvaluator : IExpressionEvaluator
    {
        private const int MaxRecursionDepth = 100;
        
        private readonly IVariableExpressionProvider _variableExpressionProvider;
        private readonly IEnvironmentVariableValueProvider _environmentVariableValueProvider;
        private const string EnvironmentVariableExpressionType = "env";
        private const string VariableExpressionType = "var";
        
        private static readonly Regex ExpressionPattern = new Regex(@"\${\s*(?<expressionType>var|env)\s*\.\s*(?<expression>[^}]+)\s*}", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public ExpressionEvaluator(
            IVariableExpressionProvider variableExpressionProvider,
            IEnvironmentVariableValueProvider environmentVariableValueProvider)
        {
            _variableExpressionProvider = variableExpressionProvider ?? throw new ArgumentNullException(nameof(variableExpressionProvider));
            _environmentVariableValueProvider = environmentVariableValueProvider ?? throw new ArgumentNullException(nameof(environmentVariableValueProvider));
        }

        public string Evaluate(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return EvaluateInternal(expression, 0);
        }

        private string EvaluateInternal(string expression, int recursionDepth)
        {
            if (recursionDepth > MaxRecursionDepth)
                throw new ArgumentException("Expression seems to cause infinite recursion. Max recursion depth = " + MaxRecursionDepth);
            
            return ExpressionPattern.Replace(expression, x =>
            {
                var expressionType = x.Groups["expressionType"].Value.ToLowerInvariant();
                var expressionValue = x.Groups["expression"].Value.Trim();

                var result = expressionType switch
                {
                    EnvironmentVariableExpressionType => _environmentVariableValueProvider.Get(expressionValue),
                    VariableExpressionType => EvaluateInternal(_variableExpressionProvider.Get(expressionValue), recursionDepth + 1),
                    _ => string.Empty
                };

                return result;
            });
        }
    }
}
