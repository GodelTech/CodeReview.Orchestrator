using System;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerImportDataActivity : StubDockerImportActivity<StubDockerImportDataActivity>
    {
        private string _sourceId;

        public StubDockerImportDataActivity WithSourceId(string sourceId)
        {
            _sourceId = sourceId ?? throw new ArgumentNullException(nameof(sourceId));

            return this;
        }
        
        protected override object Mounts
        {
            get
            {
                return new[]
                {
                    new
                    {
                        Type = "volume",
                        Source = _sourceId,
                        Target = "/imports"
                    }
                };
            }
        }
    }
}