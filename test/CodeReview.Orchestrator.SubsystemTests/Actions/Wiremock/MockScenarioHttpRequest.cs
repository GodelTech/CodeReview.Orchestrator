using System;
using GodelTech.StoryLine.Contracts;
using GodelTech.StoryLine.Wiremock.Builders;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Wiremock
{
    public class MockScenarioHttpRequest : IActionBuilder
    {
        private Action<RequestBuilder> _requestConfig = req => req.Method("GET").UrlPath("/");
        private Action<ResponseBuilder> _responseConfig = res => res.Body("Ok").Status(200);
        private Action<WiremockScenarioBuilder> _scenarioConfig = _ => { };  

        IAction IActionBuilder.Build()
        {
            var apiStubState = new WiremockScenarioApiStubState();
            var requestBuilder = new RequestBuilder(apiStubState);
            _requestConfig(requestBuilder);

            var responseBuilder = new ResponseBuilder(apiStubState);
            _responseConfig(responseBuilder);

            var scenarioBuilder = new WiremockScenarioBuilder(apiStubState);
            _scenarioConfig(scenarioBuilder);

            return new ScenarioMockHttpRequestAction(apiStubState);
        }

        public MockScenarioHttpRequest Scenario(Action<WiremockScenarioBuilder> scenarioConfig)
        {
            _scenarioConfig = scenarioConfig ?? throw new ArgumentNullException(nameof(scenarioConfig));
            
            return this;
        }

        public MockScenarioHttpRequest Request(Action<RequestBuilder> requestConfig)
        {
            _requestConfig = requestConfig ?? throw new ArgumentNullException(nameof(requestConfig));

            return this;
        }

        public MockScenarioHttpRequest Response(Action<ResponseBuilder> responseConfig)
        {
            _responseConfig = responseConfig ?? throw new ArgumentNullException(nameof(responseConfig));

            return this;
        }
    }
}