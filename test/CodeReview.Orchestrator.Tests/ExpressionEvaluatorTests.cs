using CodeReview.Orchestrator.Services;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace CodeReview.Orchestrator.Tests
{
    public class ExpressionEvaluatorTests
    {
        private readonly ExpressionEvaluator _underTest;
        
        private readonly IEnvironmentVariableValueProvider _envVarProvider;
        private readonly IVariableExpressionProvider _varExprProvider;

        public ExpressionEvaluatorTests()
        {
            _envVarProvider = A.Fake<IEnvironmentVariableValueProvider>();
            _varExprProvider = A.Fake<IVariableExpressionProvider>();
            
            _underTest = new ExpressionEvaluator(
                _varExprProvider,
                _envVarProvider);
        }
        
        [Theory]
        [InlineData("test", "test")]
        [InlineData("xxx", "xxx")]
        public void Evaluate_When_LiteralExpressionProvided_Should_ReturnLiteral(string expression, string expectedResult)
        {
            _underTest.Evaluate(expression).Should().Be(expectedResult);
        }


        [Theory]
        [InlineData("{ Env:Path }", "test", "test")]
        [InlineData("{ env:Path }", "ffff", "ffff")]
        public void Evaluate_When_When_EnvironmentVariableReferenced_Should_ReturnEnvironmentVariableValue(string expression, string envVarValue, string expectedResult)
        {
            A.CallTo(() => _envVarProvider.Get("Path")).Returns(envVarValue);
            
            _underTest.Evaluate(expression).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("{ var:Path }", "test", "test")]
        [InlineData("{ VAR:Path }", "ffff", "ffff")]
        public void Evaluate_When_When_ValiableReferenced_Should_ReturnEnvironmentVariableValue(string expression, string varValue, string expectedResult)
        {
            A.CallTo(() => _varExprProvider.Get("Path")).Returns(varValue);

            _underTest.Evaluate(expression).Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("{ var:Path }_{env:dragon}", "aaa", "bbb", "aaa_bbb")]
        [InlineData("{ var:Path }_{env:xxx}", "aaa", "bbb", "aaa_")]
        [InlineData("{ var:xxx }_{env:dragon}", "aaa", "bbb", "_bbb")]
        public void Evaluate_When_When_ValiableAndEnvVarSpecified_Should_ReturnExpectedValue(string expression, string varValue, string envVarValue, string expectedResult)
        {
            A.CallTo(() => _varExprProvider.Get("Path")).Returns(varValue);
            A.CallTo(() => _envVarProvider.Get("dragon")).Returns(envVarValue);

            _underTest.Evaluate(expression).Should().Be(expectedResult);
        }
    }
}
