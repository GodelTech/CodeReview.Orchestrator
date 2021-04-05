using System;
using Xunit;

namespace CodeReview.Orchestrator.SubsystemTests
{
    [Collection(nameof(StartUpFixture))]
    public class TestBase : IDisposable
    {
        protected TestBase()
        {
            GodelTech.StoryLine.Wiremock.Config.ResetAll();
        }

        public void Dispose()
        {
            GodelTech.StoryLine.Wiremock.Config.ResetAll();
        }
    }
}