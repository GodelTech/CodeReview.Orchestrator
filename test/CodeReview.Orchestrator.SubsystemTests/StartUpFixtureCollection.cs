using Xunit;

namespace CodeReview.Orchestrator.SubsystemTests
{
    [CollectionDefinition(nameof(StartUpFixture))]
    public class StartUpFixtureCollection : ICollectionFixture<StartUpFixture>
    {
    }
}