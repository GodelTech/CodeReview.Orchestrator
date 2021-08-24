using System;
using Xunit;

namespace CodeReview.Orchestrator.SubsystemTests
{
    [Collection(nameof(StartUpFixture))]
    public class WiremockTestBase : TestBase, IDisposable
    {
        protected WiremockTestBase()
        {
            GodelTech.StoryLine.Wiremock.Config.ResetAll();
        }

        public void Dispose()
        {
            GodelTech.StoryLine.Wiremock.Config.ResetAll();
        }
    }
}